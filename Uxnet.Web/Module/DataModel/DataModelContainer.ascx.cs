using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Utility;

namespace Uxnet.Web.Module.DataModel
{
    public partial class DataModelContainer : System.Web.UI.UserControl
    {
        private Object _item;

        protected void Page_Load(object sender, EventArgs e)
        {
            initializeData();
        }

        public Type ItemType
        {
            get;
            set;
        }

        public String Suffix
        {
            get;
            set;
        }

        public Object DataItem
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                if ( ItemType!= null)
                {
                    Page.Items[getDataID()] = _item;
                }
            }
        }

        private String getDataID()
        {
            return ItemType.Name + Suffix;
        }

        private void initializeData()
        {
            if (this.EnableViewState && _item==null)
            {
                if (ItemType != null)
                {
                    if (ViewState[getDataID()] != null)
                    {
                        DataItem = ((String)ViewState[getDataID()]).DeserializeDataContract(ItemType);
                    }
                    else if (DataItem == null && Page.PreviousPage != null)
                    {
                        DataItem = Page.PreviousPage.Items[getDataID()];
                    }
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(DataModelContainer_PreRender);
        }

        void DataModelContainer_PreRender(object sender, EventArgs e)
        {
            if (DataItem != null && ItemType != null && this.EnableViewState)
            {
                ViewState[getDataID()] = DataItem.SerializeDataContract(ItemType);
            }
        }

        public void PrepareToTransfer()
        {
            Page.Items[getDataID()] = DataItem;
        }
    }
}