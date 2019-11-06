using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.Module.SiteAction;

namespace Uxnet.Web.Module.ForBootstrap
{
    public partial class DropdownMenuBlock : DropdownMenuItem
    {

        public override void DataBind()
        {
            rpItem.DataSource = _dataItem.menuItem;
            base.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            rpItem.ItemDataBound += rpItem_ItemDataBound;
        }

        void rpItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SiteMenuItem item = (SiteMenuItem)e.Item.DataItem;
            if (item != null)
            {
                Uxnet.Web.Module.ForBootstrap.DropdownMenuItem menuItem;
                if (item.menuItem != null && item.menuItem.Length > 0)
                {
                    menuItem = (Uxnet.Web.Module.ForBootstrap.DropdownMenuItem)this.LoadControl("DropdownSubMenu.ascx");
                }
                else if (!String.IsNullOrEmpty(item.control))
                {
                    menuItem = (Uxnet.Web.Module.ForBootstrap.DropdownMenuItem)this.LoadControl(item.control);
                }
                else if (item.value == "-")
                {
                    menuItem = (Uxnet.Web.Module.ForBootstrap.DropdownMenuItem)this.LoadControl("DropdownMenuDivider.ascx");
                }
                else
                {
                    menuItem = (Uxnet.Web.Module.ForBootstrap.DropdownMenuItem)this.LoadControl("DropdownMenuItem.ascx");
                }

                menuItem.InitializeAsUserControl(this.Page);
                e.Item.Controls.Add(menuItem);
                menuItem._level = this._level;
                menuItem.StaticLevel = this.StaticLevel;
                menuItem.BindData(item);
            }
        }


    }
}