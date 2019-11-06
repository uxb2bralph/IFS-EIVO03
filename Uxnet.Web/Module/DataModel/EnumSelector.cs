using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Uxnet.Web.Module.DataModel
{
    public class EnumSelector : System.Web.UI.UserControl
    {
        private bool _isBound;

        protected global::System.Web.UI.WebControls.DropDownList selector;


        [Bindable(true)]
        public String TypeName
        {
            get;
            set;
        }

        [Bindable(true)]
        public Type EnumType
        {
            get;
            set;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.BindData();
        }

        public String SelectedValue
        {
            get
            {
                return _isBound ? selector.SelectedValue : Request[selector.UniqueID];
            }
            set
            {
                selector.SelectedValue = value;
            }
        }

        public DropDownList Selector
        {
            get
            {
                return selector;
            }
        }

        [Bindable(true)]
        public String SelectorIndication { get; set; }

        [Bindable(true)]
        public String SelectorIndicationValue { get; set; }

        public virtual void BindData()
        {
            selector.Items.Clear();
            if (EnumType == null && !String.IsNullOrEmpty(TypeName))
            {
                EnumType = Type.GetType(TypeName);
            }
            if (EnumType!=null)
            {
                String[] items = Enum.GetNames(EnumType);
                Array values = Enum.GetValues(EnumType);
                for (int i = 0; i < items.Length; i++)
                {
                    selector.Items.Add(new ListItem(items[i], ((int)values.GetValue(i)).ToString()));
                }

                if (!String.IsNullOrEmpty(SelectorIndication))
                {
                    selector.Items.Insert(0, new ListItem(SelectorIndication, String.IsNullOrEmpty(SelectorIndicationValue) ? "" : SelectorIndicationValue));
                }

                if (Request[selector.UniqueID] != null)
                {
                    var item = selector.Items.FindByValue(Request[selector.UniqueID]);
                    if (item != null)
                    {
                        selector.SelectedValue = Request[selector.UniqueID];
                    }
                }

                _isBound = true;
            }
        }
    }
}