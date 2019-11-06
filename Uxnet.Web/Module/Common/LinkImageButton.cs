using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

namespace Uxnet.Web.Module.Common
{
    public class LinkImageButton : LinkButton
    {
        [BindableAttribute(true)]
        public string ImageUrl
        {
            get
            {
                String s = (String)ViewState["imgUrl"];
                return ((s == null) ? String.Empty : s);
            }
            set
            {
                ViewState["imgUrl"] = value;
            }
        }

        [BindableAttribute(true)]
        public string OverImageUrl
        {
            get
            {
                String s = (String)ViewState["overUrl"];
                return ((s == null) ? String.Empty : s);
            }
            set
            {
                ViewState["overUrl"] = value;
            }
        }

        [BindableAttribute(true)]
        public string ClickImageUrl
        {
            get
            {
                String s = (String)ViewState["clickUrl"];
                return ((s == null) ? String.Empty : s);
            }
            set
            {
                ViewState["clickUrl"] = value;
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(LinkImageButton_PreRender);
        }

        void LinkImageButton_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ImageUrl))
            {
                Image img = new Image();
                img.ImageUrl = VirtualPathUtility.ToAbsolute(ImageUrl);
                this.Controls.Add(img);
                this.Attributes.Add("onmouseout", String.Format("document.all('{0}').src='{1}';", img.ClientID, img.ImageUrl));

                if (!String.IsNullOrEmpty(OverImageUrl))
                {
                    this.Attributes.Add("onmouseover", String.Format("document.all('{0}').src='{1}';", img.ClientID, VirtualPathUtility.ToAbsolute(OverImageUrl)));
                }

                if (!String.IsNullOrEmpty(ClickImageUrl))
                {
                    this.Attributes.Add("onclick", String.Format("document.all('{0}').src='{1}';", img.ClientID, VirtualPathUtility.ToAbsolute(ClickImageUrl)));
                }
            }

        }
    }
}
