using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Data.Linq;

namespace Uxnet.Web.Module.DataModel
{
    public class ItemSelectorV2 : System.Web.UI.UserControl
    {
        protected String _selectedValue ;

        protected global::System.Web.UI.WebControls.DropDownList selector;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ItemSelectorV2_PreRender);
            _selectedValue = Request[selector.UniqueID];
        }

        protected virtual void ItemSelectorV2_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_selectedValue))
            {
                selector.SelectedValue = _selectedValue;
            }

            if (!String.IsNullOrEmpty(SelectorIndication))
            {
                selector.Items.Insert(0, new ListItem(SelectorIndication, String.IsNullOrEmpty(SelectorIndicationValue) ? "" : SelectorIndicationValue));
            }
        }

        [Bindable(true)]
        public String SelectedValue
        {
            get
            {
                return _selectedValue;
            }
            set
            {
                selector.SelectedValue = value;
                _selectedValue = value;
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

        [Bindable(true)]
        public bool Enabled
        {
            get
            {
                return selector.Enabled;
            }
            set
            {
                selector.Enabled = value;
            }
        }


    }

    public class ItemSelectorV2<T, TEntity> : EntityDataSource<T, TEntity>
        where T : DataContext, new()
        where TEntity : class, new()
    {
        protected String _selectedValue;
        protected global::System.Web.UI.WebControls.DropDownList selector;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ItemSelectorV2_PreRender);
            _selectedValue = Request[selector.UniqueID];
        }

        protected virtual void ItemSelectorV2_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_selectedValue))
            {
                selector.SelectedValue = _selectedValue;
            }

            if (!String.IsNullOrEmpty(SelectorIndication))
            {
                selector.Items.Insert(0, new ListItem(SelectorIndication, String.IsNullOrEmpty(SelectorIndicationValue) ? "" : SelectorIndicationValue));
            }
        }

        [Bindable(true)]
        public String SelectedValue
        {
            get
            {
                return _selectedValue;
            }
            set
            {
                selector.SelectedValue = value;
                _selectedValue = value;
            }
        }

        public DropDownList Selector
        {
            get
            {
                return selector;
            }
        }

        protected virtual void assignDataSource()
        {

        }

        public virtual void BindData()
        {
            assignDataSource();
            selector.DataBind();
        }

        [Bindable(true)]
        public String SelectorIndication { get; set; }

        [Bindable(true)]
        public String SelectorIndicationValue { get; set; }

        [Bindable(true)]
        public bool Enabled
        {
            get
            {
                return selector.Enabled;
            }
            set
            {
                selector.Enabled = value;
            }
        }
    }
}