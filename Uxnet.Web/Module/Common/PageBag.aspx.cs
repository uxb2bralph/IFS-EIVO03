using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace Uxnet.Web.Module.Common
{
    public partial class PageBag : System.Web.UI.Page
    {
        public ControlCollection ChildControls
        {
            get
            {
                return theForm.Controls;
            }
        }

        public StringBuilder PageContent { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public UserControl LoadUserControl(String virtualPath)
        {
            UserControl uc = this.LoadControl(virtualPath) as UserControl;
            if (uc != null)
            {
                theForm.Controls.Add(uc);
                uc.InitializeAsUserControl(this);
            }
            return uc;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            PageContent = new StringBuilder();
            using (StringWriter sw = new StringWriter(PageContent))
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    base.Render(htw);
                    htw.Flush();
                }
                sw.Flush();
                sw.Close();
            }
        }
    }
}
