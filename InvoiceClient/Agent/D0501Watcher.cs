using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Utility;

namespace InvoiceClient.Agent
{
    public class D0501Watcher : AllowanceCancellationWatcher
    {
        public D0501Watcher(String fullPath)
            : base(fullPath)
        {

        }

        protected override Root processUpload(WS_Invoice.eInvoiceService invSvc, XmlDocument docInv)
        {
           
            var result = invSvc.UploadAllowanceCancellationV2_D0501(docInv).ConvertTo<Root>();
            return result;
        }
        protected override bool processError(IEnumerable<RootResponseInvoiceNo> rootInvoiceNo, XmlDocument docInv, string fileName)
        {
            if (rootInvoiceNo != null && rootInvoiceNo.Count() > 0)
            {
                IEnumerable<String> message = rootInvoiceNo.Select(i => String.Format("作廢折讓號碼:{0}=>{1}", i.Value, i.Description));
                Logger.Warn(String.Format("在上傳作廢折讓檔({0})時,傳送失敗!!原因如下:\r\n{1}", fileName, String.Join("\r\n", message.ToArray())));

                Model.Schema.TurnKey.D0501.CancelAllowance invoice = docInv.TrimAll().ConvertTo<Model.Schema.TurnKey.D0501.CancelAllowance>();
                Model.Schema.TurnKey.D0501.CancelAllowance stored = docInv.TrimAll().ConvertTo<Model.Schema.TurnKey.D0501.CancelAllowance>();
                
                stored.ConvertToXml().SaveDocumentWithEncoding(Path.Combine(_failedTxnPath, fileName));
            }
            return true;
        }

    }
}
