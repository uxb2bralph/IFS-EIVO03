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

namespace ProcessorUnit.Execution
{
    public class InvoiceXmlRequestForCBEProcessor : ExecutorForever
    {
        public InvoiceXmlRequestForCBEProcessor()
        {
            appliedProcessType = Naming.InvoiceProcessType.C0401_Xml_CBE;
            processRequest = (uploadData, requestItem) => 
            {
                Root result = this.CreateMessageToken();
                using (InvoiceManagerForCBE manager = new InvoiceManagerForCBE())
                {
                    manager.UploadInvoiceAutoTrackNo(uploadData, result, out OrganizationToken token);
                    manager.BindProcessedItem(requestItem);
                }
                return result.ConvertToXml();

            };
        }

        protected virtual XmlDocument prepareDocument(String invoiceFile)
        {
            XmlDocument docInv = new XmlDocument();
            docInv.Load(invoiceFile);

            ///去除"N/A"資料
            ///
            var nodes = docInv.SelectNodes("//*[text()='N/A']");
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes.Item(i);
                node.RemoveChild(node.SelectSingleNode("text()"));
            }
            ///
            return docInv;
        }


        protected Func<XmlDocument, ProcessRequest, XmlDocument> processRequest;

        protected override void ProcessRequestItem()
        {
            ProcessRequest requestItem = queueItem.ProcessRequest;
            String requestFile = requestItem.RequestPath.StoreTargetPath();
            if(File.Exists(requestFile))
            {
                Organization agent = requestItem.Organization;
                requestItem.ProcessStart = DateTime.Now;
                models.SubmitChanges();

                XmlDocument uploadData = prepareDocument(requestFile);
                var result = processRequest(uploadData, requestItem);
                String responseName = $"{Path.GetFileNameWithoutExtension(requestFile)}_Response.xml";
                String responsePath = Path.Combine(Path.GetDirectoryName(requestFile), responseName);

                if (SettingsHelper.Instance.ResponsePath != null)
                {
                    responsePath = responsePath.Replace(Uxnet.Com.Properties.AppSettings.AppRoot, SettingsHelper.Instance.ResponsePath);
                }

                result.Save(responsePath);
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
