using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.InvoiceManagement.ErrorHandle;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Utility;
using Uxnet.Com.Security.UseCrypto;

namespace Business.Helper.InvoiceProcessor
{
    public static class InvoiceManagerExtensions
    {
        public static void UploadInvoiceAutoTrackNo(this InvoiceManagerV2 models, XmlDocument uploadData,Root result,out OrganizationToken token)
        {
            token = null;

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    InvoiceRoot invoice = uploadData.TrimAll().ConvertTo<InvoiceRoot>();
                    ///憑證資料檢查
                    ///
                    token = models.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                    if (token != null)
                    {
                        UploadInvoiceAutoTrackNoCore(models, result, token, invoice);
                    }
                    else
                    {
                        result.Result.message = "營業人憑證資料驗證不符!!";
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }

                EIVOPlatformFactory.Notify();
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
        }

        public static void UploadInvoiceAutoTrackNo(this InvoiceManagerV2 models, Root result, OrganizationToken token, InvoiceRoot invoice)
        {
            try
            {
                UploadInvoiceAutoTrackNoCore(models, result, token, invoice);

                EIVOPlatformFactory.Notify();
                GovPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
        }

        private static void UploadInvoiceAutoTrackNoCore(this InvoiceManagerV2 models, Root result, OrganizationToken token, InvoiceRoot invoice)
        {
            models.IgnoreDuplicateDataNumberException = token.Organization.OrganizationStatus?.IgnoreDuplicatedDataNumber == true;

            List<AutomationItem> automation = new List<AutomationItem>();
            var items = models.SaveUploadInvoiceAutoTrackNo(invoice, token);
            if (items.Count > 0)
            {
                result.Response = new RootResponse
                {
                    InvoiceNo =
                    items.Select(d => new RootResponseInvoiceNo
                    {
                        Value = invoice.Invoice[d.Key].DataNumber,
                        Description = d.Value.Message,
                        ItemIndexSpecified = true,
                        ItemIndex = d.Key
                    }).ToArray()
                };

                //失敗Response
                automation.AddRange(items.Select(d => new AutomationItem
                {
                    Description = d.Value.Message,
                    Status = 0,
                    Invoice = new AutomationItemInvoice
                    {
                        DataNumber = invoice.Invoice[d.Key].DataNumber,
                        SellerId = invoice.Invoice[d.Key].SellerId
                    }
                }));

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

            //成功Response
            if (models.EventItems != null && models.EventItems.Count > 0)
            {
                if (token.Organization.OrganizationStatus.DownloadDataNumber == true)
                {
                    automation.AddRange(models.EventItems.Select(i => new AutomationItem
                    {
                        Description = "",
                        Status = 1,
                        Invoice = new AutomationItemInvoice
                        {
                            SellerId = i.InvoiceSeller.ReceiptNo,
                            InvoiceNumber = i.TrackCode + i.No,
                            DataNumber = i.InvoicePurchaseOrder.OrderNo,
                            InvoiceDate = String.Format("{0:yyyy/MM/dd}", i.InvoiceDate),
                            InvoiceTime = String.Format("{0:HH:mm:ss}", i.InvoiceDate),
                            EncData = i.BuildEncryptedData(),
                        }
                    }));
                }
                else
                {
                    automation.AddRange(models.EventItems.Select(i => new AutomationItem
                    {
                        Description = "",
                        Status = 1,
                        Invoice = new AutomationItemInvoice
                        {
                            SellerId = i.InvoiceSeller.ReceiptNo,
                            InvoiceNumber = i.TrackCode + i.No,
                            DataNumber = i.InvoicePurchaseOrder.OrderNo,
                            InvoiceDate = String.Format("{0:yyyy/MM/dd}", i.InvoiceDate),
                            InvoiceTime = String.Format("{0:HH:mm:ss}", i.InvoiceDate),
                        }
                    }));
                }
            }

            result.Automation = automation.ToArray();
        }

        public static void BindProcessedItem(this InvoiceManagerV2 manager, ProcessRequest requestItem)
        {
            if (manager.HasItem)
            {
                foreach (var newItem in manager.EventItems)
                {
                    //if (newItem.CDS_Document.ProcessRequestDocument == null)
                    //{
                    //    newItem.CDS_Document.ProcessRequestDocument = new ProcessRequestDocument
                    //    {
                    //        TaskID = requestItem.TaskID
                    //    };
                    //}
                    manager.ExecuteCommand(@"
                        INSERT INTO [proc].ProcessRequestDocument
                                (TaskID, DocID)
                        SELECT  {0}, {1}
                        WHERE          (NOT EXISTS
                                            (SELECT          NULL
                                                FROM               [proc].ProcessRequestDocument AS p
                                                WHERE           (DocID = {1})))", 
                        requestItem.TaskID, newItem.InvoiceID);
                }
                //manager.SubmitChanges();
            }
        }

        public static void UploadInvoiceCancellation(this InvoiceManagerV2 models, XmlDocument uploadData, Root result)
        {
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    CancelInvoiceRoot item = uploadData.TrimAll().ConvertTo<CancelInvoiceRoot>();
                    ///憑證資料檢查
                    ///
                    var token = models.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                    if (token != null)
                    {
                        UploadInvoiceCancellation(models, result, item, token);
                    }
                    else
                    {
                        result.Result.message = "營業人憑證資料驗證不符!!";
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
        }

        public static void UploadInvoiceCancellation(this InvoiceManagerV2 models, Root result, CancelInvoiceRoot item, OrganizationToken token)
        {
            List<AutomationItem> automation = new List<AutomationItem>();
            var items = models.SaveUploadInvoiceCancellation(item, token);
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
                        ItemIndex = d.Key,
                        StatusCode = (d.Value is MarkToRetryException) ? "R01" : null,
                    }).ToArray()
                };

                automation.AddRange(items.Select(d => new AutomationItem
                {
                    Description = d.Value.Message,
                    Status = 0,
                    CancelInvoice = new AutomationItemCancelInvoice
                    {
                        CancelInvoiceNumber = item.CancelInvoice[d.Key].CancelInvoiceNumber,
                        SellerId = item.CancelInvoice[d.Key].SellerId
                    }
                }));

                var notifyingItems = new Dictionary<int, Exception>();
                foreach (var v in items.Where(d => !(d.Value is MarkToRetryException)))
                {
                    notifyingItems.Add(v.Key, v.Value);
                }
                if (notifyingItems.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                                        new ExceptionInfo
                                                        {
                                                            Token = token,
                                                            ExceptionItems = notifyingItems,
                                                            CancelInvoiceData = item
                                                        });
                }
            }
            else
            {
                result.Result.value = 1;
            }

            if (models.EventItems != null && models.EventItems.Count > 0)
            {
                automation.AddRange(models.EventItems.Select(d => new AutomationItem
                {
                    Description = "",
                    Status = 1,
                    CancelInvoice = new AutomationItemCancelInvoice
                    {
                        CancelInvoiceNumber = d.TrackCode + d.No,
                        SellerId = d.InvoiceSeller.ReceiptNo
                    }
                }));
            }

            result.Automation = automation.ToArray();
        }

        public static void UploadAllowance(this InvoiceManagerV2 models, XmlDocument uploadData, Root result)
        {
            try
            {
                CryptoUtility crypto = new CryptoUtility();

                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    AllowanceRoot allowance = uploadData.TrimAll().ConvertTo<AllowanceRoot>();

                    ///憑證資料檢查
                    ///
                    var token = models.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                    if (token != null)
                    {
                        models.UploadAllowance(result, allowance, token);
                    }
                    else
                    {
                        result.Result.message = "營業人憑證資料驗證不符!!";
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

        }

        public static void UploadAllowance(this InvoiceManagerV2 models, Root result, AllowanceRoot allowance, OrganizationToken token)
        {
            List<AutomationItem> automation = new List<AutomationItem>();
            var items = models.SaveUploadAllowance(allowance, token);
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
                        SellerId = allowance.Allowance[d.Key].SellerId,
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

            if (models.EventItems_Allowance != null && models.EventItems_Allowance.Count() > 0)
            {
                //上傳後折讓
                automation.AddRange(models.EventItems_Allowance.Select(d => new AutomationItem
                {
                    Description = "",
                    Status = 1,
                    Allowance = new AutomationItemAllowance
                    {
                        AllowanceNumber = d.AllowanceNumber,
                        SellerId = d.InvoiceAllowanceSeller.ReceiptNo
                    },
                }));
            }

            result.Automation = automation.ToArray();
        }
    }
}
