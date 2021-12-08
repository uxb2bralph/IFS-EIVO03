﻿using System;
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

namespace Model.InvoiceManagement.InvoiceProcess
{
    public class D0501Handler
    {
        static D0501Handler()
        {
            Settings.Default.D0501Outbound.CheckStoredPath();
        }

        private GenericManager<EIVOEntityDataContext> models;
        private Table<D0501DispatchQueue> _table;

        public D0501Handler(GenericManager<EIVOEntityDataContext> models)
        {
            this.models = models;
            _table = models.GetTable<D0501DispatchQueue>();
        }

        public void WriteToTurnkey()
        {
            int docID = 0;
            IQueryable<D0501DispatchQueue> queryItems =
                _table
                    .Where(q => q.DocID > docID && q.StepID == (int)Naming.InvoiceStepDefinition.已開立)
                    .OrderBy(d => d.DocID);

            var buffer = queryItems.Take(4096).ToList();
            while (buffer.Count > 0)
            {
                foreach (var item in buffer)
                {
                    docID = item.DocID;
                    var allowance = item.CDS_Document.DerivedDocument.ParentDocument.InvoiceAllowance;
                    try
                    {
                        var fileName = Path.Combine(Settings.Default.D0501Outbound, $"D0501-{allowance.AllowanceID}-{allowance.AllowanceNumber}.xml");
                        allowance.CreateD0501().ConvertToXml().Save(fileName);

                        item.CDS_Document.PushLogOnSubmit(models, (Naming.InvoiceStepDefinition)item.StepID, Naming.DataProcessStatus.Done);
                        models.SubmitChanges();

                        if (allowance.InvoiceAllowanceSeller.Organization.OrganizationStatus.DownloadDispatch == true)
                        {
                            PushStepQueueOnSubmit(models, item.CDS_Document, Naming.InvoiceStepDefinition.回傳MIG);
                            models.SubmitChanges();
                        }

                        models.ExecuteCommand("delete [proc].D0501DispatchQueue where DocID={0} and StepID={1}",
                            item.DocID, item.StepID);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                }
                buffer = queryItems.Take(4096).ToList();
            }

        }

        public void NotifyIssued()
        {
            D0501DispatchQueue item;
            int docID = 0;
            IQueryable<D0501DispatchQueue> queryItems =
                _table
                    .Where(q => q.DocID > docID && q.StepID == (int)Naming.InvoiceStepDefinition.已接收資料待通知);

            while ((item = queryItems.FirstOrDefault()) != null)
            {
                docID = item.DocID;
                var invoiceItem = item.CDS_Document.DerivedDocument.ParentDocument.InvoiceItem;

                try
                {

                    EIVOPlatformFactory.NotifyIssuedInvoiceCancellation(new NotifyToProcessID
                    {
                        DocID = item.DocID,
                    });

                    models.ExecuteCommand("delete [proc].D0501DispatchQueue where DocID={0} and StepID={1}",
                        item.DocID, item.StepID);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

        }

        private void prepareStep(D0501DispatchQueue item, Naming.InvoiceStepDefinition targetStep)
        {
            item.CDS_Document.PushLogOnSubmit(models, (Naming.InvoiceStepDefinition)item.StepID, Naming.DataProcessStatus.Done);
            PushStepQueueOnSubmit(models, item.CDS_Document, targetStep);
        }

        public static void PushStepQueueOnSubmit(GenericManager<EIVOEntityDataContext> models, CDS_Document docItem, Naming.InvoiceStepDefinition stepID)
        {
            models.GetTable<D0501DispatchQueue>().InsertOnSubmit(
                new D0501DispatchQueue
                {
                    CDS_Document = docItem,
                    DispatchDate = DateTime.Now,
                    StepID = (int)stepID
                });

            docItem.PushLogOnSubmit(models, stepID, Naming.DataProcessStatus.Ready);
        }
    }

}
