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
    public class InvoiceWatcherV2 : InvoiceWatcher
    {
        public InvoiceWatcherV2(String fullPath)
            : base(fullPath)
        {

        }

        protected override Root processUpload(WS_Invoice.eInvoiceService invSvc, XmlDocument docInv)
        {
            Root result;
            if (PreferredProcessType.HasValue)
            {
                result = invSvc.UploadInvoiceByClient(docInv, Settings.Default.ClientID, (int)_channelID, false, (int)PreferredProcessType).ConvertTo<Root>();
            }
            else
            {
                result = invSvc.UploadInvoiceV2(docInv).ConvertTo<Root>();
            }

            return result;
        }

    }
}
