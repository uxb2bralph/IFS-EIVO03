using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DataAccessLayer.basis;
using Utility;
using Uxnet.Com.DataAccessLayer.Models;
using Uxnet.Web.Controllers;
using Uxnet.Web.WebUI;

namespace Uxnet.Web.Mvc.DataModel
{
    public abstract partial class EntityDataSource<T, TEntity> : ViewUserControl
        where T : DataContext, new()
        where TEntity : class, new()
    {

        protected ModelSource<T, TEntity> _models;
        protected IQueryable<TEntity> _items;
        protected ModelStateDictionary _modelState;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _modelState = (ModelStateDictionary)ViewBag.ModelState;
            _models = ((SampleController<T,TEntity>)ViewContext.Controller).DataSource;
        }


        public IQueryable<TEntity> Select()
        {
            if (BuildQuery != null)
            {
                return BuildQuery(_models.EntityList);
            }
            else if (QueryExpr != null)
            {
                return _models.EntityList.Where(QueryExpr);
            }
            else if(_items!=null)
            {
                return _items;
            }
            else
            {
                return _models.EntityList.Where(t => false);
            }
        }

        public virtual Expression<Func<TEntity, bool>> QueryExpr
        { get; set; }

        public virtual Func<Table<TEntity>, IQueryable<TEntity>> BuildQuery
        {
            get;
            set;
        }

        public IQueryable<TEntity> Items
        {
            get
            {
                if (_items == null)
                    _items = _models.EntityList;
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public ModelSource<T, TEntity> DataSource
        {
            get
            {
                return _models;
            }
        }
    }
}