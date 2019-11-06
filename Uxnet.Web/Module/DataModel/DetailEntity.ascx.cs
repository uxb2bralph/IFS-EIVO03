using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Utility;

namespace Uxnet.Web.Module.DataModel
{
    public partial class DetailEntity : System.Web.UI.UserControl
    {
        protected IDictionary<String, Control> _values;

        public GridView DataGridView { get { return gvEntity; } }

        protected internal Dictionary<String, SortDirection> _sortExpression
        {
            get
            {
                if (ViewState["sort"] == null)
                {
                    ViewState["sort"] = new Dictionary<String, SortDirection>();
                }
                return (Dictionary<String, SortDirection>)ViewState["sort"];
            }
            set
            {
                ViewState["sort"] = value;
            }
        }

        protected bool _insertMode
        {
            get
            {
                return ViewState["insert"] != null ? (bool)ViewState["insert"] : false;
            }
            set
            {
                if (value)
                    ViewState["insert"] = true;
                else
                    ViewState.Remove("insert");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(DetailEntity_PreRender);
        }

        void DetailEntity_PreRender(object sender, EventArgs e)
        {

        }

        protected virtual void gvEntity_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEntity.EditIndex = e.NewEditIndex;
        }

        protected virtual void gvEntity_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _insertMode = false;
            if (gvEntity.Rows.Count == 1 && gvEntity.PageIndex > 0)
            {
                gvEntity.PageIndex--;
            }
        }

        protected virtual void gvEntity_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            _insertMode = false;
        }

        protected virtual void gvEntity_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEntity.EditIndex = -1;
            _insertMode = false;
        }

        protected virtual void gvEntity_Sorting(object sender, GridViewSortEventArgs e)
        {
            _sortExpression.AddSortExpression(e, true);
        }

        protected virtual void gvEntity_PreRender(object sender, EventArgs e)
        {
            if (ViewState["sort"] != null)
            {
                _sortExpression.CheckSortedGridViewHeader(gvEntity);
            }
        }

        protected virtual void gvEntity_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "New")
            {
                _insertMode = true;
                gvEntity.EditIndex = int.Parse((String)e.CommandArgument);
            }
            else if (e.CommandName == "Create")
            {
                OnEntityCreating(e);
            }

        }

        protected virtual void OnEntityCreating(GridViewCommandEventArgs e)
        {
            gvEntity.EditIndex = 0;
        }

        protected virtual void gvEntity_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {

        }

        public virtual void BindData()
        { 

        }

        protected virtual void extractInputValues<T>(IDictionary values, T item) where T : class
        {
            T newValue = values.ConvertToObjectByDataContract(item, "X_");
            newValue.AssignProperty(item, p => p.Name.StartsWith("X_"));
        }

    }
}