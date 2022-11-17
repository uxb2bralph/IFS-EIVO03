using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InvoiceClient.Properties;
using System.Xml;
using Utility;
using InvoiceClient.Helper;
using Model.Schema.EIVO;
using System.Threading;
using Model.Schema.TXN;
using System.Diagnostics;
using System.Globalization;
using Model.Locale;
using CsvHelper;
using Model.UploadManagement;
using XmlLib;
using System.Xml.Linq;
using Model.Helper;

namespace InvoiceClient.Agent.TurnkeyCSVHelper
{
    public class ERPInvoiceCancellationWatcher : InvoiceCancellationWatcherV2
    {
        protected String _errorFile;

        public ERPInvoiceCancellationWatcher(String fullPath) : base(fullPath)
        {

        }


        protected override XmlDocument prepareInvoiceDocument(string invoiceFile)
        {
            XElement root = ERPInvoiceCancellationParser.ConvertToXml(invoiceFile);
            return root.ToXmlDocument();
        }
        protected override bool processError(IEnumerable<RootResponseInvoiceNo> rootInvoiceNo, XmlDocument docInv, string fileName)
        {
            if (rootInvoiceNo != null && rootInvoiceNo.Count() > 0)
            {
                IEnumerable<String> message = rootInvoiceNo.Select(i => String.Format("作廢發票號碼:{0}=>{1}", i.Value, i.Description));
                Logger.Warn(String.Format("在上傳作廢發票檔({0})時,傳送失敗!!原因如下:\r\n{1}", fileName, String.Join("\r\n", message.ToArray())));

            }
            return true;
        }
    }
}
