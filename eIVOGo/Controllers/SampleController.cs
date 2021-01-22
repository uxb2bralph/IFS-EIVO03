using System;
using System.Collections.Generic;
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
using eIVOGo.Properties;
using Model.DataEntity;
using Utility;
using Uxnet.Web.Controllers;

namespace eIVOGo.Controllers
{
    public class SampleController<TEntity> : SampleController<EIVOEntityDataContext, TEntity>
        where TEntity : class, new()
    {

        protected SampleController() : base()
        {
            models = new ModelSource<TEntity>(models);
        }
        public new ModelSource<TEntity> DataSource
        {
            get
            {
                return (ModelSource<TEntity>)models;
            }
        }

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if(filterContext.Result is ViewResult)
        //    {
        //        var cookie = filterContext.HttpContext.Request.Cookies["cLang"];
        //        if (cookie?.Value != null && cookie.Value != Settings.Default.DefaultUILanguage)
        //        {
        //            ViewResult result = (ViewResult)filterContext.Result;
        //            var viewName = result.ViewName.GetEfficientString();
        //            if (viewName == null)
        //            {
        //                viewName = $"~/Views/{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{filterContext.ActionDescriptor.ActionName}.cshtml";
        //            }
        //            else if (!viewName.StartsWith("~/Views/"))
        //            {
        //                viewName = $"~/Views/{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{viewName}";
        //            }

        //            viewName = $"~/Views/{cookie.Value}/{viewName.Substring(8)}";
        //            if (System.IO.File.Exists(filterContext.HttpContext.Server.MapPath(viewName)))
        //            {
        //                result.ViewName = viewName;
        //            }

        //            ViewBag.Lang = cookie.Value;
        //        }
        //    }

        //    base.OnActionExecuted(filterContext);
        //}
    }
}