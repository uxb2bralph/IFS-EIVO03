using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using eIVOGo.Properties;
using Model.InvoiceManagement;
using System.Xml;
using Model.Schema.TXN;
using Uxnet.Com.Security.UseCrypto;
using Model.Schema.EIVO;
using Model.DataEntity;
using System.Threading;
using Model.Helper;

using Utility;
using System.IO;
using Model.Locale;
using eIVOGo.Helper;

namespace eIVOGo.Published
{
    /// <summary>
    ///eInvoiceService_Google 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://www.uxb2b.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class eInvoiceServiceGoogleCBM : eInvoiceService_Google
    {

        [WebMethod]
        public override XmlDocument UploadAllowanceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            try
            {
                CryptoUtility crypto = new CryptoUtility();

                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    AllowanceRoot allowance = uploadData.TrimAll().ConvertTo<AllowanceRoot>();

                    using (GooglePlayInvoiceManager mgr = new GooglePlayInvoiceManager { ChannelID = _channelID,InvoiceClientID=_clientID })
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            List<AutomationItem> automation = new List<AutomationItem>();
                            var items = mgr.SaveUploadAllowance(allowance, token);
                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = allowance.Allowance[d.Key].AllowanceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                automation.AddRange(items.Select(d => new AutomationItem
                                {
                                    Description = d.Value.Message,
                                    Status = 0,
                                    Allowance = new AutomationItemAllowance
                                    {
                                        AllowanceNumber = allowance.Allowance[d.Key].AllowanceNumber,
                                    },
                                }));

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        ExceptionItems = items,
                                        AllowanceData = allowance
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }

                            if (mgr.EventItems_Allowance != null && mgr.EventItems_Allowance.Count() > 0)
                            {
                                //上傳後折讓
                                automation.AddRange(mgr.EventItems_Allowance.Select(d => new AutomationItem
                                {
                                    Description = "",
                                    Status = 1,
                                    Allowance = new AutomationItemAllowance
                                    {
                                        AllowanceNumber = d.AllowanceNumber,
                                        InvoiceNumber = d.InvoiceAllowanceDetails.Select(a => a.InvoiceAllowanceItem.InvoiceNo).ToArray()
                                    },
                                }));
                            }

                            result.Automation = automation.ToArray();
                        }
                        else
                        {
                            result.Result.message = "店家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

    }
}
