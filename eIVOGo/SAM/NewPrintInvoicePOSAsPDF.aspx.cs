using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Utility;
using eIVOGo.Helper;
using eIVOGo.Properties;

namespace eIVOGo.SAM
{
    public partial class NewPrintInvoicePOSAsPDF : System.Web.UI.Page
    {
        public static readonly String[] ThermalPOSPaper = new String[] { Settings.Default.ThermalPOS };
        protected void Page_Load(object sender, EventArgs e)
        {
            createInvoicePDF();
        }

        private void createInvoicePDF()
        {
            String pdfFile = Server.CreateContentAsPDF("~/SAM/NewPrintInvoicePOSPage.aspx", Session.Timeout, ThermalPOSPaper);
            if (pdfFile != null)
            {
                Response.WriteFileAsDownload(pdfFile, String.Format("{0:yyyy-MM-dd}.pdf", DateTime.Today), true);
            }
            else
            {
                Response.Output.WriteLine("系統忙錄中，請稍後再試...");
                Response.End();
            }
        }
    }
}