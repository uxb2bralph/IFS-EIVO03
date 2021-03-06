﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace eIVOGo.template
{
    public partial class ContentControlTemplate : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [Bindable(true)]
        public String ItemName
        {
            get
            {
                return actionItem.ItemName;
            }
            set
            {
                actionItem.ItemName = value;
            }
        }

        [Bindable(true)]
        public String FunctionTitle
        {
            get
            {
                return titleBar.ItemName;
            }
            set
            {
                titleBar.ItemName = value;
            }
        }

    }
}