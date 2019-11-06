using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvoiceClient.Helper;
using InvoiceClient.Properties;
using InvoiceClient.TransferManagement;
using System.ServiceProcess;
using InvoiceClient.Agent;

namespace InvoiceClient.MainContent
{
    public partial class GoogleInvoiceServerConfigForPDFGenerator : GoogleInvoiceServerConfig
    {

        public GoogleInvoiceServerConfigForPDFGenerator() :base()
        {
        }

        protected override void initialize()
        {
            _inspector = InvoiceClientTransferManager.GetServerInspector(typeof(InvoicePDFGenerator));
        }

    }
}
