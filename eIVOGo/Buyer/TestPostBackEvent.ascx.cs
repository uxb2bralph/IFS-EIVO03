using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyInvoice.Buyer
{
    public partial class TestPostBackEvent : System.Web.UI.UserControl,IPostBackDataHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            //            Page.ClientScript.RegisterForEventValidation(this.UniqueID);
            writer.Write("<input type=\"hidden\" name=\"{0}\" />", this.UniqueID);
        }

        #region IPostBackDataHandler Members

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return true;
        }

        public void RaisePostDataChangedEvent()
        {
            
        }

        #endregion
    }
}