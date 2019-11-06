using DataAccessLayer.basis;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using Uxnet.Com.DataAccessLayer.Models;

namespace Uxnet.Web.Controllers
{
    public class SampleController<T,TEntity> : SampleController<T>
        where T : DataContext, new()
        where TEntity : class, new()
    {

        protected ModelSource<T, TEntity> models;

        protected SampleController() :base()
        {
            models = new ModelSource<T, TEntity>(modelsBase);
        }

        public ModelSource<T,TEntity> DataSource
        {
            get
            {
                return models;
            }
        }

    }

    public class SampleController<T> : Controller
        where T : DataContext, new()
    {
        protected GenericManager<T> modelsBase;
        protected bool _dbInstance;

        protected SampleController() : base()
        {
            modelsBase = TempData["__DB_Instance"] as GenericManager<T>;
            if (modelsBase == null)
            {
                modelsBase = new GenericManager<T>();
                _dbInstance = true;
                TempData["__DB_Instance"] = modelsBase;
            }

        }

        public GenericManager<T> DataSourceBase => modelsBase;

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (_dbInstance)
                modelsBase.Dispose();
        }
    }

}