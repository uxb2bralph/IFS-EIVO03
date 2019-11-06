using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.ForBootstrap
{
    public partial class DropdownMenu : DropdownMenuItem
    {

        public override void DataBind()
        {
            menuBlock.DataItem = _dataItem;
            menuBlock.StaticLevel = this.StaticLevel;
            menuBlock._level = 1;
            base.DataBind();
        }

    }
}