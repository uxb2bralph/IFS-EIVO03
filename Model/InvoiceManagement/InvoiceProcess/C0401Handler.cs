using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

using DataAccessLayer.basis;
using Model.DataEntity;
using Model.DocumentManagement;
using Model.Helper;
using Model.Locale;
using Model.Properties;
using Utility;
using System.Security.Cryptography.Pkcs;
using Uxnet.Com.Security.UseCrypto;
using System.Data.Linq;
using System.Threading.Tasks;

namespace Model.InvoiceManagement.InvoiceProcess
{
    public class C0401Handler
    {
        static C0401Handler()
        {
            Settings.Default.C0401Outbound.CheckStoredPath();
        }

        private GenericManager<EIVOEntityDataContext> models;
        private Table<C0401DispatchQueue> _table;

        public C0401Handler(GenericManager<EIVOEntityDataContext> models)
        {
            this.models = models;
            _table = models.GetTable<C0401DispatchQueue>();
        }

        public void WriteToTurnkey(int? procIdx = null, int? totalProc = null)
        {
            int docID = 0;
            IQueryable<C0401DispatchQueue> queryItems =
                _table
                    .Where(q => q.DocID > docID && q.StepID == (int)Naming.InvoiceStepDefinition.已開立)
                    .OrderBy(d => d.DocID);

            var buffer = (procIdx >= 0 && totalProc > 0)
                    ? queryItems.Where(d => (d.DocID % totalProc.Value) == procIdx.Value).Take(4096).ToList()
                    : queryItems.Take(4096).ToList();
            while (buffer.Count > 0)
            {
                foreach (var item in buffer)
                {
                    docID = item.DocID;
                    var invoiceItem = item.CDS_Document.InvoiceItem;
                    try
                    {
                        var fileName = Path.Combine(Settings.Default.C0401Outbound, $"C0401-{invoiceItem.InvoiceID}-{invoiceItem.TrackCode}{invoiceItem.No}.xml");
                        invoiceItem.CreateC0401().ConvertToXml().Save(fileName);

                        item.CDS_Document.PushLogOnSubmit(models, (Naming.InvoiceStepDefinition)item.StepID, Naming.DataProcessStatus.Done);
                        models.SubmitChanges();

                        if (invoiceItem.Organization.OrganizationStatus.SubscribeB2BInvoicePDF == true
                            && (!invoiceItem.InvoiceBuyer.IsB2C() || invoiceItem.Organization.OrganizationStatus.PrintAll == true)
                            && item.CDS_Document.DocumentSubscriptionQueue == null)
                        {
                            models.ExecuteCommand(
                                @"INSERT INTO DocumentSubscriptionQueue
                                (DocID)
                            SELECT  {0}
                            WHERE   (NOT EXISTS
                                    (SELECT NULL
                                        FROM DocumentSubscriptionQueue
                                        WHERE (DocID = {0})))", item.DocID);
                        }


                        if (invoiceItem.Organization.OrganizationStatus.DownloadDataNumber == true)
                        {
                            models.ExecuteCommand(@"INSERT INTO DocumentMappingQueue
                                            (DocID)
                                        SELECT  {0}
                                        WHERE   (NOT EXISTS
                                                (SELECT NULL
                                                    FROM DocumentMappingQueue
                                                    WHERE (DocID = {0})))", invoiceItem.InvoiceID);
                        }

                        models.ExecuteCommand("delete [proc].C0401DispatchQueue where DocID={0} and StepID={1}",
                            item.DocID, item.StepID);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                }
                buffer = (procIdx >= 0 && totalProc > 0)
                    ? queryItems.Where(d => (d.DocID % totalProc.Value) == procIdx.Value).Take(4096).ToList()
                    : queryItems.Take(4096).ToList();
            }
        }

        public void NotifyIssued()
        {

            C0401DispatchQueue item;
            int docID = 0;
            IQueryable<C0401DispatchQueue> queryItems =
                _table
                    .Where(q => q.DocID > docID && q.StepID == (int)Naming.InvoiceStepDefinition.已接收資料待通知);

            while ((item = queryItems.FirstOrDefault()) != null)
            {
                docID = item.DocID;
                var invoiceItem = item.CDS_Document.InvoiceItem;

                try
                {

                    EIVOPlatformFactory.NotifyIssuedInvoice(new NotifyToProcessID
                    {
                        DocID = item.DocID,
                        AppendAttachment = true,
                    });

                    models.ExecuteCommand("delete [proc].C0401DispatchQueue where DocID={0} and StepID={1}",
                        item.DocID, item.StepID);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

        }

        private void prepareStep(C0401DispatchQueue item, Naming.InvoiceStepDefinition targetStep)
        {
            item.CDS_Document.PushLogOnSubmit(models, (Naming.InvoiceStepDefinition)item.StepID, Naming.DataProcessStatus.Done);
            PushStepQueueOnSubmit(models, item.CDS_Document, targetStep);
        }

        public static void PushStepQueueOnSubmit(GenericManager<EIVOEntityDataContext> models, CDS_Document docItem, Naming.InvoiceStepDefinition stepID)
        {
            models.GetTable<C0401DispatchQueue>().InsertOnSubmit(
                new C0401DispatchQueue
                {
                    CDS_Document = docItem,
                    DispatchDate = DateTime.Now,
                    StepID = (int)stepID
                });

            docItem.PushLogOnSubmit(models, stepID, Naming.DataProcessStatus.Ready);
        }

        //TODO:yuki加一筆到ProcessRequestDocument
        //public static void PushProcessRequestDocumentOnSubmit(GenericManager<EIVOEntityDataContext> models, CDS_Document docItem, int? taskID)
        //{            
        //    models.GetTable<ProcessRequestDocument>().InsertOnSubmit(
        //                    new ProcessRequestDocument
        //                    {
        //                        CDS_Document = docItem,
        //                        TaskID = taskID,
        //                        CreateDate = DateTime.Now
        //                    });            
        //}

        private static Task __Turnkey;
        public static void WriteToTurnkeyInBatches()
        {
            //System.Diagnostics.Debugger.Launch();
            if (__Turnkey == null)
            {
                var t = Task.Run(() =>
                {
                    Logger.Debug($"{DateTime.Now}: Create C0401 WriteToTurnkey...");
                    Parallel.For(0, Settings.Default.CommonParallelProcessCount, idx =>
                    {
                        Logger.Debug($"C0401 WriteToTurnkey:{idx}");
                        using (InvoiceManager models = new InvoiceManager())
                        {
                            C0401Handler c0401 = new C0401Handler(models);
                            c0401.WriteToTurnkey(idx, Settings.Default.CommonParallelProcessCount);
                        }
                    });
                });

                t = t.ContinueWith(ts =>
                {
                    Task.Delay(Settings.Default.TaskDelayInMilliseconds).ContinueWith(ts1 =>
                    {
                        __Turnkey = null;
                        WriteToTurnkeyInBatches();
                    });
                });
                Interlocked.Exchange<Task>(ref __Turnkey, t);
            }
            //else
            //{
            //    Logger.Debug($"C0401 WriteToTurnkey is running...");
            //}
        }
    }

}
