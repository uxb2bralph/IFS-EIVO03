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
    public class ItemSelector : System.Web.UI.UserControl
    {
        protected bool _isBound;

        protected global::System.Web.UI.WebControls.DropDownList selector;
        protected global::System.Web.UI.DataSourceControl dsEntity;

        public event EventHandler SelectedIndexChanged;

        [Bindable(true)]
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

        public virtual void BindData()
        {
            selector.Items.Clear();
            selector.DataBind();
        }

        [Bindable(true)]
        public String SelectorIndication { get; set; }

        [Bindable(true)]
        public String SelectorIndicationValue { get; set; }

        protected virtual void selector_DataBound(object sender, EventArgs e)
        {

            if (Request[selector.UniqueID] != null)
            {
                var item = selector.Items.FindByValue(Request[selector.UniqueID]);
                if (item != null)
                {
                    selector.SelectedValue = Request[selector.UniqueID];
                }
            }

            if (!String.IsNullOrEmpty(SelectorIndication))
            {
                selector.Items.Insert(0, new ListItem(SelectorIndication, String.IsNullOrEmpty(SelectorIndicationValue) ? "" : SelectorIndicationValue));
            }
            _isBound = true;

            if (this.IsPostBack && (String)ViewState["lastValue"] != SelectedValue && SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, new EventArgs());
            }

            this.ViewState["lastValue"] = SelectedValue;
        }

        public override void DataBind()
        {
            if (!_isBound)
            {
                selector.DataBind();
            }
        }

    }

    public class ItemSelector<T, TEntity> : EntityDataSource<T, TEntity>
        where T : DataContext, new()
        where TEntity : class, new()
    {
        protected bool _isBound;
        protected global::System.Web.UI.WebControls.DropDownList selector;

        public event EventHandler SelectedIndexChanged;

        [Bindable(true)]
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

        public virtual void BindData()
        {
            selector.Items.Clear();
            selector.DataBind();
        }

        [Bindable(true)]
        public String SelectorIndication { get; set; }

        [Bindable(true)]
        public String SelectorIndicationValue { get; set; }

        protected virtual void selector_DataBound(object sender, EventArgs e)
        {

            if (Request[selector.UniqueID] != null)
            {
                var item = selector.Items.FindByValue(Request[selector.UniqueID]);
                if (item != null)
                {
                    selector.SelectedValue = Request[selector.UniqueID];
                }
            }

            if (!String.IsNullOrEmpty(SelectorIndication))
            {
                selector.Items.Insert(0, new ListItem(SelectorIndication, String.IsNullOrEmpty(SelectorIndicationValue) ? "" : SelectorIndicationValue));
            }
            _isBound = true;

            if (this.IsPostBack && (String)ViewState["lastValue"] != SelectedValue && SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, new EventArgs());
            }

            this.ViewState["lastValue"] = SelectedValue;
        }

        public override void DataBind()
        {
            if (!_isBound)
            {
                selector.DataBind();
            }
        }
    }
}