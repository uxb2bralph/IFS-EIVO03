using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Threading;

using Utility;
using eIVOGo.Helper;

namespace eIVOGo.SAM
{
    public partial class PrintPageAsPDF : System.Web.UI.Page
    {
        public String ContentRelativeUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ContentRelativeUrl))
                createPDF();
        }

        private void createPDF()
        {
            String pdfFile = Server.CreateContentAsPDF(ContentRelativeUrl, Session.Timeout);
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