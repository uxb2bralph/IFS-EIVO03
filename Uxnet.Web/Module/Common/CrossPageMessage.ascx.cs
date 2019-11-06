using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Uxnet.Web.WebUI;

namespace Uxnet.Web.Module.Common
{
    public partial class CrossPageMessage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [Bindable(true)]
        public String Message
        {
            get
            {
                return (String)modelItem.DataItem;
            }
            set
            {
                modelItem.DataItem = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(CrossPageMessage_PreRender);
        }

        void CrossPageMessage_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Message))
            {
                this.AjaxAlert(Message);
                Message = null;
            }
        }
    }
}