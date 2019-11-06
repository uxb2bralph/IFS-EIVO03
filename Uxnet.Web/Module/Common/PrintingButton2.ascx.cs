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

    /// <summary>
    ///		PrintingButton ªººK­n´y­z¡C
    /// </summary>
    public partial class PrintingButton2 : PrintingButton
    {
        protected override void _btnPrintIt_Click(object sender, System.EventArgs e)
        {
            OnBeforeClick(new EventArgs());

            if (_isValid && _printControls.Count > 0 && !String.IsNullOrEmpty(ContentGeneratorUrl))
            {
                Page.Items["PrintDoc"] = _printControls;
                Server.Transfer(ContentGeneratorUrl);
            }
        }
    }
}
