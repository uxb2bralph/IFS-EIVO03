using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Uxnet.Web.Module.Common
{
    public partial class PageAnchor : System.Web.UI.UserControl
    {
        [Bindable(true)]
        public String TransferTo
        {
            get;
            set;
        }

        [Bindable(true)]
        public String RedirectTo
        {
            get;
            set;
        }

        [Bindable(true)]
        public String Message
        {
            get;
            set;
        }
    }
}