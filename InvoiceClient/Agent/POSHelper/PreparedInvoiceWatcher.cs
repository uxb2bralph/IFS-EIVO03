using System;
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
    public class PreparedInvoiceWatcher : InvoiceWatcher
    {
        public PreparedInvoiceWatcher(String fullPath)
            : base(fullPath)
        {

        }

        protected override bool processError(IEnumerable<RootResponseInvoiceNo> rootInvoiceNo, XmlDocument docInv, string fileName)
        {
            if (rootInvoiceNo != null && rootInvoiceNo.Count() > 0)
            {
                InvoiceRoot invoice = docInv.TrimAll().ConvertTo<InvoiceRoot>();
                InvoiceRoot stored = docInv.TrimAll().ConvertTo<InvoiceRoot>();
                stored.Invoice = rootInvoiceNo.Where(i => i.ItemIndexSpecified).Select(i => invoice.Invoice[i.ItemIndex]).ToArray();

                stored.ConvertToXml().SaveDocumentWithEncoding(Path.Combine(_requestPath, $"{Guid.NewGuid()}.xml"));
            }
            return true;
        }

        private Root processUploadCore(eInvoiceService invSvc, XmlDocument docInv)
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

            List<InvoiceRootInvoice> eventItems = new List<InvoiceRootInvoice>();
            var items = createPrintFormat(invoice, eventItems);
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
            Console.WriteLine($"total seconds: {(DateTime.Now - ts).TotalSeconds}");

            return result;
        }


        protected override Root processUpload(WS_Invoice.eInvoiceService invSvc, XmlDocument docInv)
        {
            return processUploadCore(invSvc, docInv);
        }

        private Dictionary<int, Exception> createPrintFormat(InvoiceRoot item, List<InvoiceRootInvoice> eventItems)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();

            if (item != null && item.Invoice != null && item.Invoice.Length > 0)
            {
                using (WebClientEx client = new WebClientEx { Timeout = 43200000 })
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    for (int idx = 0; idx < item.Invoice.Length; idx++)
                    {
                        try
                        {
                            var invItem = item.Invoice[idx];
                            String tmpHtml = Path.Combine(POSReady._Settings.PrintInvoice, $"{invItem.InvoiceNumber}.htm");
                            File.WriteAllText(tmpHtml, client.UploadString(POSReady._Settings.PrintC0401, JsonConvert.SerializeObject(invItem)));
                            eventItems.Add(invItem);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            result.Add(idx, ex);
                        }
                    }
                }
            }

            return result;
        }


    }
}
