namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
    using Utility;

	/// <summary>
	///		DownloadingButton ªººK­n´y­z¡C
	/// </summary>
    public partial class CreatePDFButton : DownloadingButton
	{

        public String DocumentName 
        {
            get
            {
                return (String)ViewState["docName"];
            }
            set
            {
                ViewState["docName"] = value;
            }
        }

        protected override void _btnDownload_Click(object sender, System.EventArgs e)
        {
            OnBeforeClick();

            if (_controlList.Count > 0)
            {
                _Hash.Remove(Session.SessionID);
                _Hash.Add(Session.SessionID, _controlList);

                if (!String.IsNullOrEmpty(ContentGeneratorUrl))
                {
                    String pdf;
                    try
                    {
                        using (WS_DocumentService.DocumentCreator dc = new Uxnet.Web.WS_DocumentService.DocumentCreator())
                        {
                            pdf = dc.CreatePdfFromUrl(String.Format("{0}?sessionID={1}", ContentGeneratorUrl, Session.SessionID));
                        }
                        Response.DumpFileAsDownload(pdf, String.IsNullOrEmpty(DocumentName) ? "document.pdf" : DocumentName, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
        }
	}
}
