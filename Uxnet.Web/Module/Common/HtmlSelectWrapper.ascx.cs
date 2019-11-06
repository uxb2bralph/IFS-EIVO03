using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Uxnet.Web.Module.Common
{
    public partial class HtmlSelectWrapper : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void BindData(object dataSource,  string textField,string valueField)
        {
            _Select.DataSource = dataSource;
            _Select.DataTextField = textField;
            _Select.DataValueField = valueField;
            _Select.DataBind();
        }

        public HtmlSelect Selector
        {
            get
            {
                return _Select;
            }
        }

        public string Value
        {
            get
            {
                return Request.Form[_Select.UniqueID];
            }
        }

        public string Text
        {
            get
            {
                return Request.Form[_Select.UniqueID];
            }
        }

        public void CopyItemsTo(HtmlSelectWrapper target)
        {
            target._Select.Items.Clear();

            foreach (ListItem item in _Select.Items)
            {
                target._Select.Items.Add(new ListItem(item.Text, item.Value));
            }
        }

    }
}