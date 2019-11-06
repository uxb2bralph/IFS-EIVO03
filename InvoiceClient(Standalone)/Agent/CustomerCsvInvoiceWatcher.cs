using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Utility;

namespace InvoiceClient.Agent
{
    public class CustomerCsvInvoiceWatcher : CsvInvoiceWatcher
    {

        public CustomerCsvInvoiceWatcher(String fullPath)
            : base(fullPath)
        {

        }


        protected override Root processUpload(WS_Invoice.eInvoiceService invSvc, SignedCms docInv)
        {
            var result = invSvc.UploadInvoiceCmsCSV(docInv.Encode()).ConvertTo<Root>();
            return result;
        }

    }
}
