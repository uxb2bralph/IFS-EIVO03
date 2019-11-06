using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;

using eIVOGo.Module.Common;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using System.IO;
using System.Data;

namespace eIVOGo.Published
{

    public partial class eInvoiceService
    {

        static partial void AddOn()
        {
            EIVOPlatformFactory.DefaultUserCarrierType = Settings.Default.DefaultUserCarrierType;
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceV2(XmlDocument uploadData)
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
        public virtual XmlDocument UploadAllowanceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();

                if (crypto.VerifyXmlSignature(uploadData))
                {
                    AllowanceRoot allowance = uploadData.TrimAll().ConvertTo<AllowanceRoot>();

                    using (InvoiceManagerV2_Proxy mgr = new InvoiceManagerV2_Proxy())
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
        public virtual XmlDocument UploadInvoiceAutoTrackNoV2(XmlDocument uploadData)
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
        public virtual XmlDocument UploadInvoiceCmsCSVAutoTrackNoV2(byte[] uploadData)
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

        #region B2B寄送發票(CSV)

        public DataTable TxtConvertToDataTable(string File, string TableName, string delimiter)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            StreamReader s = new StreamReader(File, System.Text.Encoding.Default);
            //string ss = s.ReadLine();//skip the first line
            string[] columns = s.ReadLine().Split(delimiter.ToCharArray());
            ds.Tables.Add(TableName);
            foreach (string col in columns)
            {
                bool added = false;
                string next = "";
                int i = 0;
                while (!added)
                {
                    string columnname = col + next;
                    columnname = columnname.Replace("#", "");
                    columnname = columnname.Replace("'", "");
                    columnname = columnname.Replace("&", "");

                    if (!ds.Tables[TableName].Columns.Contains(columnname))
                    {
                        ds.Tables[TableName].Columns.Add(columnname.ToUpper());
                        added = true;
                    }
                    else
                    {
                        i++;
                        next = "_" + i.ToString();
                    }
                }
            }

            string AllData = s.ReadToEnd();
            string[] rows = AllData.Split("\n".ToCharArray());

            foreach (string r in rows)
            {
                string[] items = r.Split(delimiter.ToCharArray());
                ds.Tables[TableName].Rows.Add(items);
            }

            s.Close();
            dt = ds.Tables[0];

            return dt;
        }

        #endregion B2B寄送發票(CSV)

        [WebMethod]
        public virtual XmlDocument UploadInvoiceCancellationV2(XmlDocument uploadData)
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
        public virtual XmlDocument UploadAllowanceCancellationV2(XmlDocument uploadData)
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
                            var items = mgr.SaveUploadAllowanceCancellation(item,token);
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

        [WebMethod]
        public virtual XmlDocument UploadBranchTrack(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                 CryptoUtility crypto = new CryptoUtility();
                 if (crypto.VerifyXmlSignature(uploadData))
                 {
                     BranchTrack btrack = uploadData.TrimAll().ConvertTo<BranchTrack>();

                     using (B2BInvoiceManager mgr = new B2BInvoiceManager())
                     {
                         var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();

                         if (token != null)
                         {
                             mgr.SaveE0401(btrack, token);
                             result.Result.value = 1;
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