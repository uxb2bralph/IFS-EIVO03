﻿using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using ProcessorUnit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Utility;
using Model.InvoiceManagement;
using ClosedXML.Excel;
using System.Xml;
using Model.Schema.TXN;
using Business.Helper.InvoiceProcessor;
using ProcessorUnit.Helper;
using Newtonsoft.Json;
using Model.Schema.EIVO;

namespace ProcessorUnit.Execution
{
    public class InvoiceJsonRequestForCBEProcessor : ExecutorForever
    {
        public InvoiceJsonRequestForCBEProcessor()
        {
            System.Diagnostics.Debugger.Launch();
            appliedProcessType = Naming.InvoiceProcessType.C0401_Json_CBE;
            processRequest = (jsonData, requestItem) => 
            {
                Root result = this.CreateMessageToken();
                InvoiceRoot invoice = JsonConvert.DeserializeObject<InvoiceRoot>(jsonData);
                using (InvoiceManagerForCBE manager = new InvoiceManagerForCBE())
                {
                    var token = manager.GetTable<OrganizationToken>().Where(t => t.CompanyID == requestItem.AgentID).FirstOrDefault();
                    if(token!=null)
                    {
                        manager.UploadInvoiceAutoTrackNo(result, token, invoice);
                        manager.BindProcessedItem(requestItem);
                    }
                }
                return JsonConvert.SerializeObject(result);
            };
        }

        protected virtual String prepareDocument(String invoiceFile)
        {
            return File.ReadAllText(invoiceFile);
        }


        protected Func<String, ProcessRequest, String> processRequest;

        protected override void ProcessRequestItem()
        {
            ProcessRequest requestItem = queueItem.ProcessRequest;
            String requestFile = requestItem.RequestPath.StoreTargetPath();
            if(File.Exists(requestFile))
            {
                Organization agent = requestItem.Organization;
                requestItem.ProcessStart = DateTime.Now;
                models.SubmitChanges();

                var uploadData = prepareDocument(requestFile);
                var result = processRequest(uploadData, requestItem);
                String responseName = $"{Path.GetFileNameWithoutExtension(requestFile)}_Response.xml";
                String responsePath = Path.Combine(Path.GetDirectoryName(requestFile), responseName);

                if (SettingsHelper.Instance.ResponsePath != null)
                {
                    responsePath = responsePath.Replace(StorePathExtensions.AppRoot, SettingsHelper.Instance.ResponsePath);
                }

                File.WriteAllText(responsePath, result);
                requestItem.ProcessComplete = DateTime.Now;
                requestItem.ResponsePath = responsePath;
                if (requestItem.ProcessCompletionNotification == null)
                    requestItem.ProcessCompletionNotification = new ProcessCompletionNotification { };
                models.DeleteAnyOnSubmit<ProcessRequestQueue>(d => d.TaskID == queueItem.TaskID);
                models.SubmitChanges();

            }
        }

    }
}
