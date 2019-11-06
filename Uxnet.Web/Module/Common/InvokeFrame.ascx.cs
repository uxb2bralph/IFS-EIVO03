using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Utility;

namespace Uxnet.Web.Module.Common
{
    public partial class InvokeFrame : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(InvokeFrame_PreRender);
        }

        void InvokeFrame_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(InvokeScript) && !Page.ClientScript.IsClientScriptIncludeRegistered(typeof(InvokeFrame), "invoke"))
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(InvokeFrame), "invoke", VirtualPathUtility.ToAbsolute("~/Scripts/invoke.js"));
                BaseWebPageUtility.CreateObjectClientDeclarationWithScript(frameInvoker);
                frameInvoker.Visible = true;
            }
            else
            {
                frameInvoker.Visible = false;
            }
        }

        public String InvokeScript
        {
            get;
            set;
        }

        public String BuildInvoking(String url)
        {
            return String.Format("javascript:invokeFrame('{0}');", url);
        }
    }
}