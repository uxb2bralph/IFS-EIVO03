using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;

namespace Uxnet.Web.Module.Common
{
    public class LinkImagePrintButton : LinkImageButton
    {
        private List<Control> _printControls = new List<Control>();
        private string _printPageUrl;
        private string _features;

        public string Features
        {
            get { return _features; }
            set { _features = value; }
        }

        public string PrintPageUrl
        {
            get { return _printPageUrl; }
            set { _printPageUrl = value; }
        }

        public List<Control> PrintControls
        {
            get { return _printControls; }
        }

        public string ClientScriptReference
        {
            get
            {
                return String.Format("{0}_print", this.ClientID);
            }
        }

        private void buildPrintAction()
        {
            StringBuilder args = new StringBuilder("new Array(");
            args.Append(String.Format("document.all('{0}')", _printControls[0].ClientID));
            for (int i = 1; i < _printControls.Count; i++)
            {
                args.Append(String.Format(",document.all('{0}')", _printControls[i].ClientID));
            }
            args.Append(")");

            StringBuilder sb = new StringBuilder();
            sb.Append("function ").Append(ClientScriptReference).Append("() {\r\n")
                .Append(String.Format("window.showModalDialog('{0}',{1},'{2}');", VirtualPathUtility.ToAbsolute(_printPageUrl), args.ToString(), _features))
                .Append("}\r\n");
            Page.ClientScript.RegisterClientScriptBlock(typeof(LinkImagePrintButton), this.UniqueID, sb.ToString(), true);

            this.Attributes.Add("href", String.Format("javascript:{0}();", ClientScriptReference));

        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(LinkImagePrintButton_PreRender);
        }

        void LinkImagePrintButton_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_printPageUrl) && _printControls.Count > 0)
            {
                buildPrintAction();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("function ").Append(ClientScriptReference).Append("() {\r\n")
                    .Append(this.OnClientClick)
                    .Append("}\r\n");
                Page.ClientScript.RegisterClientScriptBlock(typeof(LinkImagePrintButton), this.UniqueID, sb.ToString(), true);

                this.Attributes.Add("href", String.Format("javascript:{0}();", ClientScriptReference));
            }
        }
    }
}
