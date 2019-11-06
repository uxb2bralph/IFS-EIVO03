using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using Uxnet.Com.Security.UseCrypto;
using Utility;
using System.Threading;
using Model.Helper;
using eIVOGo.Module.Common;
using eIVOGo.Properties;
using System.IO;
using System.Text;


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
    public class eInvoiceService_Proxy : eInvoiceService
    {
        [WebMethod]
        public override XmlDocument UploadInvoiceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    InvoiceRoot invoice = uploadData.TrimAll().ConvertTo<InvoiceRoot>();

                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();

                        if (token != null)
                        {
                            var items = mgr.SaveUploadInvoice(invoice, token);

                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = invoice.Invoice[d.Key].InvoiceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        ExceptionItems = items,
                                        InvoiceData = invoice
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }

                            if (mgr.HasItem && token.Organization.OrganizationStatus.PrintAll == true)
                            {
                                SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                            }
                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public override XmlDocument UploadAllowanceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();

                if (crypto.VerifyXmlSignature(uploadData))
                {
                    AllowanceRoot allowance = uploadData.TrimAll().ConvertTo<AllowanceRoot>();

                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    //using (InvoiceManagerV3 mgr = new InvoiceManagerV3())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
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
                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public override XmlDocument UploadInvoiceAutoTrackNoV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    InvoiceRoot invoice = uploadData.TrimAll().ConvertTo<InvoiceRoot>();
                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var items = mgr.SaveUploadInvoiceAutoTrackNo(invoice, token);
                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = invoice.Invoice[d.Key].InvoiceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        ExceptionItems = items,
                                        InvoiceData = invoice
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;

                                //2014-02-18 程式碼修改簡化

                                //for (int idx = 0; idx < invoice.Invoice.Length; idx++)
                                //{
                                //    var newItem = mgr.GetTable<InvoiceItem>().Where(i => i.InvoicePurchaseOrder.OrderNo == invoice.Invoice[idx].DataNumber && i.CDS_Document.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice).FirstOrDefault();

                                //    //if (!String.IsNullOrEmpty(newItem.InvoiceBuyer.EMail) && (newItem.Organization.OrganizationStatus.PrintAll == false || newItem.Organization.OrganizationStatus.PrintAll == null))
                                //    if (!String.IsNullOrEmpty(newItem.InvoiceBuyer.EMail))
                                //    {
                                //        EIVOPlatformFactory.SendInvoiceB2B(this, new EventArgs<InvoiceItem> { Argument = newItem });
                                //    }

                                //    if (idx == 0)
                                //    {
                                //        if (newItem.Organization.OrganizationStatus.PrintAll == true)
                                //        {
                                //            SharedFunction.SendMailMessage(newItem.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, newItem.Organization.CompanyName + "電子發票開立郵件通知");
                                //        }
                                //    }
                                //}


                            }

                            if (mgr.HasItem && token.Organization.OrganizationStatus.PrintAll == true)
                            {
                                SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                            }
                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public override XmlDocument UploadInvoiceCmsCSVAutoTrackNoV2(byte[] uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                byte[] dataToSign;
                if (crypto.VerifyEnvelopedPKCS7(uploadData, out dataToSign))
                {
                    String fileName = Path.Combine(Logger.LogDailyPath, String.Format("Invoice_{0}.csv", Guid.NewGuid()));
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        fs.Write(dataToSign, 0, dataToSign.Length);
                        fs.Flush();
                        fs.Close();
                    }

                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            CsvInvoiceUploadManagerV2 csvMgr = new CsvInvoiceUploadManagerV2(mgr, token.CompanyID);
                            csvMgr.ParseData(null, fileName, Encoding.GetEncoding(Settings.Default.CsvUploadEncoding));
                            if (!csvMgr.Save())
                            {
                                var items = csvMgr.ErrorList;
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = d.DataContent,
                                        Description = d.Status,
                                        ItemIndexSpecified = true,
                                        ItemIndex = csvMgr.ItemList.IndexOf(d)
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        InvoiceError = items
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                                if (token.Organization.OrganizationStatus.PrintAll == true)
                                {
                                    SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                                }
                            }

                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

       

        [WebMethod]
        public override XmlDocument UploadInvoiceCancellationV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    CancelInvoiceRoot item = uploadData.TrimAll().ConvertTo<CancelInvoiceRoot>();
                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var items = mgr.SaveUploadInvoiceCancellation(item, token);
                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = item.CancelInvoice[d.Key].CancelInvoiceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                                                    new ExceptionInfo
                                                                    {
                                                                        Token = token,
                                                                        ExceptionItems = items,
                                                                        CancelInvoiceData = item
                                                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                                if (mgr.InvoiceID != null && mgr.InvoiceID.Count > 0)
                                {
                                    SharedFunction.SendInvoiceCancellationMail(mgr.InvoiceID);
                                }
                            }
                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public override XmlDocument UploadAllowanceCancellationV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    CancelAllowanceRoot item = uploadData.TrimAll().ConvertTo<CancelAllowanceRoot>();
                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var items = mgr.SaveUploadAllowanceCancellation(item, token);
                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = item.CancelAllowance[d.Key].CancelAllowanceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                                                    new ExceptionInfo
                                                                    {
                                                                        Token = token,
                                                                        ExceptionItems = items,
                                                                        CancelAllowanceData = item
                                                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }
                        }
                        else
                        {
                            result.Result.message = "商家憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                GovPlatformFactory.Notify();
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
