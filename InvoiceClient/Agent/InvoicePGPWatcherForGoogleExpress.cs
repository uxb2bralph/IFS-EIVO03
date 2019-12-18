using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InvoiceClient.Properties;
using System.Xml;
using Utility;
using InvoiceClient.Helper;
using Model.Schema.EIVO;
using System.Threading;
using Model.Schema.TXN;
using System.Diagnostics;
using System.Globalization;
using Model.Locale;
using InvoiceClient.WS_Invoice;
using Model.InvoiceManagement;
using Model.DataEntity;
using Model.TaskManagement;

namespace InvoiceClient.Agent
{
    public class InvoicePGPWatcherForGoogleExpress : InvoicePGPWatcherForGoogle
    {
        public int TaskID { get; set; }

        public InvoicePGPWatcherForGoogleExpress(String fullPath) : base(fullPath)
        {

        }

        protected override void processFile(String invFile)
        {
            if (!File.Exists(invFile))
                return;

            String fileName = Path.GetFileName(invFile);
            String fullPath = Path.Combine(_inProgressPath, fileName);
            try
            {
                File.Move(invFile, fullPath);
            }
            catch (Exception ex)
            {
                Logger.Warn("move file error: " + invFile);
                Logger.Error(ex);
                return;
            }

            if (fullPath.EndsWith(".gpg", StringComparison.CurrentCultureIgnoreCase)
                    || fullPath.EndsWith(".pgp", StringComparison.CurrentCultureIgnoreCase))
            {
                processPGP(fullPath);//Yuki 解密檔名就離開
                return;
            }

            using (GoogleInvoiceManagerV2 mgr = new GoogleInvoiceManagerV2 { InvoiceClientID = Settings.Default.ClientID, ChannelID = (int)_channelID, IgnoreDuplicateDataNumberException = true })
            {
                ///憑證資料檢查
                ///
                var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == AppSigner.SignerCertificate.Thumbprint).FirstOrDefault();
                if (token != null)
                {
                    Root result = new Root();

                    try
                    {
                        //Yuki ProcessRequest儲存
                        TaskManager taskMgr = new TaskManager();

                        TaskID = taskMgr.SaveUploadTask(mgr, fullPath, Naming.InvoiceProcessType.C0501_Xlsx).TaskID;

                        XmlDocument docInv = prepareInvoiceDocument(fullPath);

                        result = processUpload(null, docInv);

                        if (result.Result.value != 1)
                        {
                            if (result.Response != null && result.Response.InvoiceNo != null && result.Response.InvoiceNo.Length > 0)
                            {
                                processError(result.Response.InvoiceNo, docInv, fileName);
                                storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                            }
                            else
                            {
                                processError(result.Result.message, docInv, fileName);
                                storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
                            }
                        }
                        else
                        {
                            storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
                    }
                    finally
                    {
                        if (result.Automation != null)
                        {
                            Automation auto = new Automation { Item = result.Automation };
                            String responseName = fileName.Replace("request", "response")
                                    .Replace("_OUT_", "_IN_");
                            responseName = Path.Combine(_ResponsedPath, responseName);
                            auto.ConvertToXml().Save(responseName);
                            //yuki upadte ProcessRequest 
                            var processRequest = mgr.GetTable<ProcessRequest>().Where(t => t.TaskID == TaskID).FirstOrDefault();
                            processRequest.ResponsePath = responseName;
                            processRequest.ProcessComplete = DateTime.Now;
                            if (processRequest != null) mgr.SubmitChanges();                            
                        }
                    }
                }
            }
        }

        protected override Root processUpload(eInvoiceService invSvc, XmlDocument docInv)
        {
            DateTime ts = DateTime.Now;
            Console.WriteLine($"start converting xml to object at {ts}");
            InvoiceRoot invoice = docInv.TrimAll().ConvertTo<InvoiceRoot>();

            Root result = new Root
            {
                UXB2B = "電子發票系統",
                Result = new RootResult
                {
                    timeStamp = DateTime.Now,
                    value = 0
                }
            };

            using (GoogleInvoiceManagerV2 mgr = new GoogleInvoiceManagerV2 { InvoiceClientID = Settings.Default.ClientID, ChannelID = (int)_channelID, IgnoreDuplicateDataNumberException = true })
            {
                ///憑證資料檢查
                ///
                var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == AppSigner.SignerCertificate.Thumbprint).FirstOrDefault();

                List<AutomationItem> automation = new List<AutomationItem>();

                mgr.TaskID = TaskID;

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
                            ItemIndex = d.Key,
                        }).ToArray()
                    };

                    //失敗Response
                    automation.AddRange(items.Select(d => new AutomationItem
                    {
                        Description = d.Value.Message,
                        Status = 0,
                        Invoice = new AutomationItemInvoice
                        {
                            DataNumber = invoice.Invoice[d.Key].DataNumber
                        }
                    }));
                }
                else
                {
                    result.Result.value = 1;
                }

                //成功Response
                if (mgr.EventItems != null && mgr.EventItems.Count > 0)
                {
                    automation.AddRange(mgr.EventItems.Select(i => new AutomationItem
                    {
                        Description = "",
                        Status = 1,
                        Invoice = new AutomationItemInvoice
                        {
                            InvoiceNumber = i.TrackCode + i.No,
                            DataNumber = i.InvoicePurchaseOrder.OrderNo,
                            InvoiceDate = String.Format("{0:yyyy/MM/dd}", i.InvoiceDate),
                            InvoiceTime = String.Format("{0:HH:mm:ss}", i.InvoiceDate),
                        }
                    }));
                }

                result.Automation = automation.ToArray();
            }

            Console.WriteLine($"total seconds: {(DateTime.Now - ts).TotalSeconds}");

            return result;
        }

    }
}
