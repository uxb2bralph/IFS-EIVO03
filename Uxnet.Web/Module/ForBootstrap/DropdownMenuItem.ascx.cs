using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.ForBootstrap
{
    public partial class DropdownMenuItem : System.Web.UI.UserControl
    {
        protected Uxnet.Web.Module.SiteAction.SiteMenuItem _dataItem;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Uxnet.Web.Module.SiteAction.SiteMenuItem DataItem
        {
            get
            {
                return _dataItem;
            }
            set
            {
                _dataItem = value;
            }
        }

        public override void DataBind()
        {
            if (_dataItem != null)
            {
                base.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }

        public virtual void BindData(Uxnet.Web.Module.SiteAction.SiteMenuItem item)
        {
            _dataItem = item;
            this.DataBind();
        }

        protected internal int _level;

        [Bindable(true)]
        public int StaticLevel
        { get; set; }
    }
}