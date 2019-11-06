using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.DataModel
{
    public class ListControlSelectorV2 : System.Web.UI.UserControl
    {
        protected bool _isBound;

        protected global::System.Web.UI.WebControls.ListControl selector;
        protected List<String> _selectedValues;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ListControlSelectorV2_PreRender);
            _selectedValues = new List<string>();
            foreach (var item in Request.Form.AllKeys.Where(k => k != null && k.StartsWith(selector.UniqueID)))
            {
                _selectedValues.AddRange(Request.Form.GetValues(item));
            }
        }

        protected virtual void ListControlSelectorV2_PreRender(object sender, EventArgs e)
        {
            foreach (var itemValue in _selectedValues)
            {
                var item = selector.Items.FindByValue(itemValue);
                if (item != null)
                    item.Selected = true;
            }
        }


        public List<String> SelectedValues
        {
            get
            {
                return _selectedValues;
            }

        }

        public ListControl Selector
        {
            get
            {
                return selector;
            }
        }
    }
}