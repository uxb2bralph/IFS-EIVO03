using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;
using Utility;
using Uxnet.Web.WebUI;

namespace Uxnet.Web.Module.DataModel
{
    public abstract partial class EntityDataSource<T, TEntity> : System.Web.UI.UserControl
        where T : DataContext, new()
        where TEntity : class, new()
    {

        protected LinqToSqlDataSource<T, TEntity> dsEntity;
        protected bool _deferredQuery;
        protected String _errorMsg;
        protected IQueryable<TEntity> _items;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.dsEntity.Select += new EventHandler<DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<TEntity>>(dsEntity_Select);
        }


        protected virtual void dsEntity_Select(object sender, DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<TEntity> e)
        {
            _deferredQuery = false;
            if (BuildQuery != null)
            {
                e.Query = BuildQuery(dsEntity.CreateDataManager().EntityList);
            }
            else if (QueryExpr != null)
            {
                e.QueryExpr = QueryExpr;
            }
            else if(_items!=null)
            {
                e.Query = _items;
            }
            else
            {
                e.QueryExpr = p => false;
                _deferredQuery = true;
            }

        }

        public IQueryable<TEntity> Select()
        {
            if (BuildQuery != null)
            {
                return BuildQuery(dsEntity.CreateDataManager().EntityList);
            }
            else if (QueryExpr != null)
            {
                return dsEntity.CreateDataManager().EntityList.Where(QueryExpr);
            }
            else if(_items!=null)
            {
                return _items;
            }
            else
            {
                return dsEntity.CreateDataManager().EntityList.Where(t => false);
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
                    _items = dsEntity.CreateDataManager().EntityList;
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public LinqToSqlDataSource<T, TEntity> DataSource
        {
            get
            {
                return dsEntity;
            }
        }
    }
}