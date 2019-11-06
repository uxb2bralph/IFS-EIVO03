using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.DataEntity;
using System.ComponentModel;

namespace eIVOGo.Module.EIVO.Item
{
    public partial class ReceiptProductPrintView : System.Web.UI.UserControl
    {
        protected ReceiptItem _item;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [Bindable(true)]
        public ReceiptItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ReceiptProductPrintView_PreRender);
        }

        void ReceiptProductPrintView_PreRender(object sender, EventArgs e)
        {
            if (Item != null)
            {
                rpList.DataSource = Item.ReceiptDetail;
                rpList.DataBind();
                this.DataBind();
            }
        }
    }
}