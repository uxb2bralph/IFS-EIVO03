using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.DataModel
{
    public class ListControlSelector : System.Web.UI.UserControl
    {
        protected bool _isBound;

        protected global::System.Web.UI.WebControls.ListControl selector;
        protected global::System.Web.UI.DataSourceControl dsEntity;


        public String[] SelectedValues
        {
            get
            {
                List<String> values = new List<string>();
                foreach (var item in Request.Form.AllKeys.Where(k => k!=null && k.StartsWith(selector.UniqueID)))
                {
                    values.AddRange(Request.Form.GetValues(item));
                }
                return values.ToArray();
//                return Request.Form.GetValues(selector.UniqueID);
            }

        }

        public ListControl Selector
        {
            get
            {
                return selector;
            }
        }

        public virtual void BindData()
        {
            selector.Items.Clear();
            selector.DataBind();
        }

        protected virtual void selector_DataBound(object sender, EventArgs e)
        {
            String[] values = SelectedValues;
            if (values!=null && values.Length>0)
            {
                foreach (var itemValue in values)
                {
                    var item = selector.Items.FindByValue(itemValue);
                    if (item != null)
                        item.Selected = true;
                }
            }

            _isBound = true;

        }

    }
}