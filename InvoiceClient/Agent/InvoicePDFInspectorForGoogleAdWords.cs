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
using InvoiceClient.TransferManagement;
using System.Diagnostics;
using System.Threading.Tasks;
using Model.Locale;

namespace InvoiceClient.Agent
{

    public class InvoicePDFInspectorForGoogleAdWords : InvoicePDFInspector
    {

        public InvoicePDFInspectorForGoogleAdWords() : base()
        {
                _tasks = null;
        }


        public override String GetSaleInvoices(int? index = null)
        {
            var ret1 = retrieveFiles(index, Naming.ChannelIDType.ForGoogleOnLine);
            var ret2 = retrieveFiles(index,Naming.ChannelIDType.ForGoogleTerms);
            return ret1 ?? ret2;
        }

        private string retrieveFiles(int? index,Naming.ChannelIDType? channelID = null)
        {
            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();

            try
            {
                Root token = this.CreateMessageToken("下載電子發票PDF");
                if(channelID.HasValue)
                {
                    token.Request.channelIDSpecified = true;
                    token.Request.channelID = (int)channelID;
                }
                String storedPath = Settings.Default.DownloadDataInAbsolutePath ? Settings.Default.DownloadSaleInvoiceFolder : Path.Combine(Settings.Default.InvoiceTxnPath, Settings.Default.DownloadSaleInvoiceFolder);
                ValueValidity.CheckAndCreatePath(storedPath);
                //storedPath = ValueValidity.GetDateStylePath(storedPath);

                String tmpPath = Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}");

                bool hasNew = false;

                XmlDocument signedReq = token.ConvertToXml().Sign();
                string[] items = invSvc.ReceiveContentAsPDFForSeller(signedReq, Settings.Default.ClientID);
                if (items != null && items.Length > 1)
                {
                    hasNew = true;
                    Directory.CreateDirectory(tmpPath);
                    String serviceUrl = items[0];

                    void proc(int i)
                    {
                        var item = items[i];
                        String[] paramValue = item.Split('\t');
                        String invNo = paramValue[0];

                        String pdfFile = Path.Combine(tmpPath,
                            _prefix_name + paramValue[1] + "_" + invNo + ".pdf");
                        //String pdfFile = Path.Combine(storedPath, string.Format("{0}{1}.pdf", "taiwan_uxb2b_scanned_gui_pdf_", invNo));

                        //token = this.CreateMessageToken("已下載電子發票PDF:" + invNo);// 
                        //signedReq = token.ConvertToXml().Sign();
                        var url = $"{serviceUrl}?keyID={paramValue[2]}";
                        Logger.Info($"retrieve PDF:{item}");
                        //var content = signedReq.UploadData(url, 43200000);
                        fetchPDF(pdfFile, url);
                    }

                    //Parallel.For(1, items.Length, (idx) =>
                    //{
                    //    proc(idx);
                    //});

                    for (int idx = 1; idx < items.Length; idx++)
                    {
                        proc(idx);
                    }

                }

                if (hasNew)
                {
                    String args = String.Format("{0}{1:yyyyMMddHHmmssffff}-{4:0000}-AdWords{5} \"{2}\" \"{3}\"",
                        _prefix_name,
                        DateTime.Now,
                        tmpPath,
                        storedPath, index ?? 0,
                        channelID == Naming.ChannelIDType.ForGoogleOnLine
                        ? "-Online"
                        : channelID == Naming.ChannelIDType.ForGoogleTerms
                            ? "-Terms"
                            : null);
                    {
                        Logger.Info($"zip PDF:{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipPDF.bat")} {args}");
                        "ZipPDF.bat".RunBatch(args);
                    }
                }
                return hasNew ? storedPath : null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public override Type UIConfigType
        {
            get { return typeof(InvoiceClient.MainContent.GoogleInvoiceServerConfigForPDFInspectorAdWords); }
        }

    }
}
