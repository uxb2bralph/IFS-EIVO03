using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.ForBootstrap
{
    public partial class DropdownSubMenu : DropdownMenuItem
    {
        protected DropdownMenuItem _menuBlock;

        public override void DataBind()
        {
            if (_level <= StaticLevel)
            {
                _menuBlock = (DropdownMenuItem)this.LoadControl("DropdownStaticMenuBlock.ascx");
            }
            else
            {
                _menuBlock = (DropdownMenuItem)this.LoadControl("DropdownMenuBlock.ascx");
            }
            _menuBlock.ID = "aSubMenu";
            _menuBlock.InitializeAsUserControl(this.Page);
            subMenu.Controls.Add(_menuBlock);
            _menuBlock._level = this._level + 1;
            _menuBlock.StaticLevel = this.StaticLevel;
            _menuBlock.DataItem = _dataItem;
            base.DataBind();
        }
    }
}