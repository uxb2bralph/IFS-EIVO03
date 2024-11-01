﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using InvoiceClient.WS_Invoice;
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Newtonsoft.Json;
using Utility;

namespace InvoiceClient.Agent.POSHelper
{
    public class InvoiceProcessWatcher : InvoiceWatcher
    {
        public InvoiceProcessWatcher(String fullPath)
            : base(fullPath)
        {

        }

        private Root processUploadCore(eInvoiceService invSvc, XmlDocument docInv)
        {
            DateTime ts = DateTime.Now;
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

            List<InvoiceRootInvoice> eventItems = new List<InvoiceRootInvoice>();
            var items = DoClientSideProcess(invoice, eventItems);
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
            }
            else
            {
                result.Result.value = 1;
            }

            return result;
        }


        protected override Root processUpload(WS_Invoice.eInvoiceService invSvc, XmlDocument docInv)
        {
            return processUploadCore(invSvc, docInv);
        }

        protected virtual Dictionary<int, Exception> DoClientSideProcess(InvoiceRoot item, List<InvoiceRootInvoice> eventItems)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            return result;
        }

    }
}
