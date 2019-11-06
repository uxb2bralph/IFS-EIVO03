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
using System.Collections.Generic;

    /// <summary>
    ///		PrintingButton ªººK­n´y­z¡C
    /// </summary>
    public partial class PrintingButton3 : PrintingButton
    {
        private List<String> _printControlSource = new List<string>();

        public List<String> PrintControlSource
        {
            get
            {
                return _printControlSource;
            }
        }

        protected override void _btnPrintIt_Click(object sender, System.EventArgs e)
        {
            OnBeforeClick(new EventArgs());

            if (_isValid && _printControlSource.Count > 0 && !String.IsNullOrEmpty(ContentGeneratorUrl))
            {
                Page.Items["PrintDoc"] = _printControlSource;
                Server.Transfer(ContentGeneratorUrl);
            }
        }
    }
}
