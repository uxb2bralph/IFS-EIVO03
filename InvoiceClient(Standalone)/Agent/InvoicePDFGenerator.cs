﻿using System;
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

    public class InvoicePDFGenerator : InvoicePDFInspector
    {
        public InvoicePDFGenerator() : base()
        {
            Settings.Default.PDFGeneratorOutput.CheckStoredPath();
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

                XmlDocument signedReq = token.ConvertToXml().Sign();
                string[] items = invSvc.ReceiveContentAsPDF(signedReq, Settings.Default.ClientID);
                if (items != null && items.Length > 1)
                {
                    String serviceUrl = items[0];

                    void proc(int i)
                    {
                        var item = items[i];
                        String[] paramValue = item.Split('\t');
                        String invNo = paramValue[0];

                        String pdfFile = Path.Combine(Settings.Default.PDFGeneratorOutput,
                            _prefix_name + paramValue[1] + "_" + invNo + ".pdf");
                        var url = $"{serviceUrl}?keyID={paramValue[2]}";
                        fetchPDF(pdfFile, url);
                    }

                    Parallel.For(1, items.Length, (idx) =>
                    {
                        proc(idx);
                    });

                }
                return  null;
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
            url.ConvertHtmlToPDF(pdfFile, 1);
        }

        public override Type UIConfigType => typeof(InvoiceClient.MainContent.GoogleInvoiceServerConfigForPDFGenerator);
    }
}
