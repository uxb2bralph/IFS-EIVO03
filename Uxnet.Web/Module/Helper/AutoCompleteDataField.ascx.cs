using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.Helper
{
    public partial class AutoCompleteDataField : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public String ServicePath
        {
            get {
                return this.TextInfo_AutoCompleteExtender.ServicePath;
            }
            set
            {
                TextInfo_AutoCompleteExtender.ServicePath = value;
            }
        }

        public String ServiceMethod
        {
            get
            {
                return this.TextInfo_AutoCompleteExtender.ServiceMethod;
            }
            set
            {
                TextInfo_AutoCompleteExtender.ServiceMethod = value;
            }
        }

        public TextBox DataField
        {
            get { return TextInfo; }
        }
    }
}