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
    public class ERPAllowanceCancellationWatcher : AllowanceCancellationWatcherV2
    {
        protected String _errorFile;

        public ERPAllowanceCancellationWatcher(String fullPath) : base(fullPath)
        {

        }


        protected override XmlDocument prepareInvoiceDocument(string invoiceFile)
        {
            XElement root = ERPAllowanceCancellationParser.ConvertToXml(invoiceFile);
            return root.ToXmlDocument();
        }
        protected override bool processError(IEnumerable<RootResponseInvoiceNo> rootInvoiceNo, XmlDocument docInv, string fileName)
        {
            if (rootInvoiceNo != null && rootInvoiceNo.Count() > 0)
            {
                IEnumerable<String> message = rootInvoiceNo.Select(i => String.Format("作廢折讓號碼:{0}=>{1}", i.Value, i.Description));
                Logger.Warn(String.Format("在上傳作廢折讓檔({0})時,傳送失敗!!原因如下:\r\n{1}", fileName, String.Join("\r\n", message.ToArray())));

            }
            return true;
        }
    }
}
