using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
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
        public static void UploadInvoiceAutoTrackNo(this InvoiceManagerV2 mgr, XmlDocument uploadData,Root result,out OrganizationToken token)
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
                    token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                    if (token != null)
                    {
                        mgr.IgnoreDuplicateDataNumberException = token.Organization.OrganizationStatus?.IgnoreDuplicatedDataNumber == true;
                        List<AutomationItem> automation = new List<AutomationItem>();
                        var items = mgr.SaveUploadInvoiceAutoTrackNo(invoice, token);
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
                        if (mgr.EventItems != null && mgr.EventItems.Count > 0)
                        {
                            if (token.Organization.OrganizationStatus.DownloadDataNumber == true)
                            {
                                automation.AddRange(mgr.EventItems.Select(i => new AutomationItem
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
                                automation.AddRange(mgr.EventItems.Select(i => new AutomationItem
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

        public static void BindProcessedItem(this InvoiceManagerV2 manager, ProcessRequest requestItem)
        {
            if (manager.HasItem)
            {
                foreach (var newItem in manager.EventItems)
                {
                    if (newItem.CDS_Document.ProcessRequestDocument == null)
                    {
                        newItem.CDS_Document.ProcessRequestDocument = new ProcessRequestDocument
                        {
                            TaskID = requestItem.TaskID
                        };
                    }
                }
                manager.SubmitChanges();
            }
        }

    }
}
