using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Net;

using InvoiceClient.Properties;
using Utility;
using Model.Schema.TXN;
using InvoiceClient.Helper;
using Model.Models;
using Model.Locale;

namespace InvoiceClient.Agent
{

    public class InvoicePDFGeneratorForGooglePlay : InvoicePDFGenerator
    {
        public InvoicePDFGeneratorForGooglePlay() : base()
        {

        }

        public override String GetSaleInvoices(int? index = null)
        {
            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();

            try
            {
                Root token = this.CreateMessageToken("下載電子發票PDF");
                if (index >= 0 && Settings.Default.ProcessCount > 0)
                {
                    token.Request.processIndexSpecified = token.Request.totalProcessCountSpecified = true;
                    token.Request.processIndex = index.Value + Settings.Default.ProcessArrayIndex;
                    token.Request.totalProcessCount = Math.Max(Settings.Default.ProcessArrayCount, Settings.Default.ProcessCount);
                }
                String storedPath = Settings.Default.DownloadDataInAbsolutePath ? Settings.Default.DownloadSaleInvoiceFolder : Path.Combine(Settings.Default.InvoiceTxnPath, Settings.Default.DownloadSaleInvoiceFolder);
                ValueValidity.CheckAndCreatePath(storedPath);

                XmlDocument signedReq = token.ConvertToXml().Sign();
                string[] items = invSvc.ReceiveContentAsPDFForIssuer(signedReq, Settings.Default.ClientID);

                //為了有測試資料
                int enviroment;
                enviroment = int.TryParse(Settings.Default.Environment, out enviroment) == true ? enviroment : 3;
                if (items != null && items.Length < 2 && (enviroment == (int)Naming.Environment.Dev || enviroment == (int)Naming.Environment.Test))//yuki:加判斷是否為本機
                {
                    items = invSvc.ReceiveContentAsPDFForSeller(signedReq, Settings.Default.ClientID);
                }

                if (items != null && items.Length > 1)
                {
                    String serviceUrl = items[0];

                    List<InvoicePDFGeneratorForGooglePlayModel> logItems = new List<InvoicePDFGeneratorForGooglePlayModel>();

                    string path = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "logs");
                    string filePath = ValueValidity.GetDateStylePath(path);

                    filePath = $"{filePath}\\InvoicePDFGeneratorForGooglePlay.xml";

                    if (!File.Exists(filePath))
                    {
                        logItems.ConvertToXml().Save(filePath);
                    }

                    void proc(int i)
                    {
                        var item = items[i];
                        String[] paramValue = item.Split('\t');
                        String invNo = paramValue[0];

                        String pdfFile = Path.Combine(Settings.Default.PDFGeneratorOutput,
                            _prefix_name + paramValue[1] + "_" + invNo + ".pdf");
                        var url = $"{serviceUrl}?keyID={paramValue[2]}";
                        fetchPDF(pdfFile, url);

                        //Generate pdf write to log
                        //var textContent = $"{pdfFile} {paramValue[1]}";

                        //Logger.GeneratePdfInfo(textContent);
                        
                        InvoicePDFGeneratorForGooglePlayModel logItem = new InvoicePDFGeneratorForGooglePlayModel
                        {
                            Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                            Path = pdfFile,
                            OrderNo = paramValue[1]
                        };

                        logItems.Add(logItem);
                    }

                    //Parallel.For(1, items.Length, (idx) =>
                    //{
                    //    proc(idx);
                    //});
                    for (int idx = 1; idx < items.Length; idx++)
                    {
                        proc(idx);
                    }

                    //log write to xml
                    logItems.AppendChildToXml(filePath);

                    Logger.Debug($"fetch count:{items.Length - 1}");
                    return storedPath;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }        

        protected override void fetchPDF(string pdfFile, string url)
        {
            url = $"{url}&html={true}";
            Logger.Debug(url);
            url.ConvertHtmlToPDF(pdfFile, 1);
        }

        public override Type UIConfigType => typeof(InvoiceClient.MainContent.GoogleInvoiceServerConfigForPDFGeneratorGooglePlay);
    }
}
