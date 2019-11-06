namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
    using System.Text;

	/// <summary>
	///		PrintingButton ªººK­n´y­z¡C
	/// </summary>
    public partial class PagePrintingButton : PrintingButton
	{

        public string UrlToPrint
        {
            get
            {
                return ContentGeneratorUrl;
            }
            set
            {
                ContentGeneratorUrl = value;
            }
        }
	}
}
