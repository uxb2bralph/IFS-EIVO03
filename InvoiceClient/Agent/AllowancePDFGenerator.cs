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

namespace InvoiceClient.Agent
{

    public class AllowancePDFGenerator : InvoicePDFInspector
    {

        public AllowancePDFGenerator()
        {
            _prefix_name = "taiwan_uxb2b_scanned_sac_pdf_";
        }

        public override String GetSaleInvoices(int? index = null)
        {
            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();

            try
            {
                Root token = this.CreateMessageToken("下載電子折讓PDF");
                if (index >= 0 && Settings.Default.ProcessCount > 0)
                {
                    token.Request.processIndexSpecified = token.Request.totalProcessCountSpecified = true;
                    token.Request.processIndex = index.Value + Settings.Default.ProcessArrayIndex;
                    token.Request.totalProcessCount = Math.Max(Settings.Default.ProcessArrayCount, Settings.Default.ProcessCount);
                    Logger.Info($"retrieve PDF by ProcIdx:{token.Request.processIndex}/{token.Request.totalProcessCount}");
                }

                String storedPath = Settings.Default.DownloadDataInAbsolutePath ? Settings.Default.DownloadSaleInvoiceFolder : Path.Combine(Settings.Default.InvoiceTxnPath, Settings.Default.DownloadSaleInvoiceFolder);
                ValueValidity.CheckAndCreatePath(storedPath);

                String tmpPath = Path.Combine(Logger.LogDailyPath, $"A_{(index ?? DateTime.Now.Ticks):0000}");
                ValueValidity.CheckAndCreatePath(tmpPath);

                bool hasNew = false;

                XmlDocument signedReq = token.ConvertToXml().Sign();
                string[] items = invSvc.ReceiveAllowancePDF(signedReq, Settings.Default.ClientID);
                if (items != null && items.Length > 1)
                {
                    hasNew = true;
                    Directory.CreateDirectory(tmpPath);
                    String serviceUrl = items[0];

                    void proc(int i)
                    {
                        var item = items[i];
                        String[] paramValue = item.Split('\t');
                        String allowanceNo = paramValue[0];

                        String pdfFile = Path.Combine(tmpPath,
                            _prefix_name + allowanceNo + ".pdf");

                        var url = $"{serviceUrl}?keyID={paramValue[1]}";
                        Logger.Info($"retrieve PDF:{item}");
                        fetchPDF(pdfFile, url);
                    }

                    Parallel.For(1, items.Length, (idx) =>
                    {
                        proc(idx);
                    });

                }

                if (hasNew)
                {
                    String args = $"{_prefix_name}{DateTime.Now:yyyyMMddHHmmssffff}_{items.Length - 1:0000} \"{tmpPath}\" \"{storedPath}\"";

                    Logger.Info($"zip PDF:{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipAllowancePDF.bat")} {args}");
                    "ZipAllowancePDF.bat".RunBatch(args);
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
            get { return typeof(InvoiceClient.MainContent.GoogleInvoiceServerConfigForAllowancePDFGenerator); }
        }
    }
}
