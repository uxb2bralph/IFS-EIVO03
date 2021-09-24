using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Properties;
using Utility;
using Model.Schema.TXN;
using InvoiceClient.Helper;
using InvoiceClient.TransferManagement;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using Uxnet.Com.Helper;

namespace InvoiceClient.Agent.POSHelper
{

    public class InvoiceNoInspector : InvoiceServerInspector
    {
        private static InvoiceNoInspector _Instance;

        private String _InvoiceNoInProgress;

        public InvoiceNoInspector()
        {

            _InvoiceNoInProgress = $"{POSReady._Settings.InvoiceNoPreload}(InProgress)";
            _InvoiceNoInProgress.CheckStoredPath();

            _Instance = this;
        }

        public override void StartUp()
        {
            InvokeService(ReceiveInvoiceNo);
        }

        private void ReceiveInvoiceNo()
        {
            String path = Path.Combine(POSReady._Settings.InvoiceNoPreload, $"{DateTime.Today.Year:0000}", $"{(DateTime.Today.Month + 1) / 2:00}");
            path.CheckStoredPath();

            if (Directory.EnumerateFiles(path).Count() > POSReady._Settings.LowVolumeAlert)
                return;

            String url = $"{POSReady._Settings.ServiceHost}{POSReady._Settings.LoadInvoiceNoUrl}";
            using (WebClientEx client = new WebClientEx())
            {
                client.Timeout = 43200000;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                String result = client.UploadString(url,
                    JsonConvert.SerializeObject(
                        new
                        {
                            SellerID = Settings.Default.SellerReceiptNo,
                            POSReady._Settings.Booklet,
                        }));
                try
                {
                    InvoiceNoRoot invoiceNo = JsonConvert.DeserializeObject<InvoiceNoRoot>(result);

                    if (invoiceNo?.invoice_issue!=null)
                    {
                        path = Path.Combine(POSReady._Settings.InvoiceNoPreload, $"{invoiceNo.Year:0000}", $"{invoiceNo.PeriodNo:00}");
                        path.CheckStoredPath();

                        foreach (var item in invoiceNo.invoice_issue)
                        {
                            String fileName = Path.Combine(path, $"{item.sn}.json");
                            File.WriteAllText(fileName, JsonConvert.SerializeObject(item));
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }

        public override Type UIConfigType
        {
            get { return null; }
        }

        public static InvoiceIssue ConsumeInvoiceNo()
        {
            InvoiceIssue result = null;
            if (_Instance != null)
            {
                String path = Path.Combine(POSReady._Settings.InvoiceNoPreload, $"{DateTime.Today.Year:0000}", $"{(DateTime.Today.Month + 1) / 2:00}");
                path.CheckStoredPath();
                lock (_Instance)
                {
                    var item = Directory.EnumerateFiles(path).FirstOrDefault();
                    if (item == null)
                    {
                        _Instance.ReceiveInvoiceNo();
                    }
                    item = Directory.EnumerateFiles(path).FirstOrDefault();
                    if (item != null)
                    {
                        result = JsonConvert.DeserializeObject<InvoiceIssue>(File.ReadAllText(item));
                        item.StoreFile(Path.Combine(Logger.LogDailyPath, Path.GetFileName(item)));
                    }
                }
            }
            return result;
        }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

}
