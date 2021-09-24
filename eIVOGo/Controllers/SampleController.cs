using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Business.Helper;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Models.ViewModel;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.DataAccessLayer;
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

        protected ActionResult PageResult(QueryViewModel viewModel, IQueryable<dynamic> items)
        {

            viewModel.RecordCount = items.Count();
            if (viewModel.InitQuery != true && viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View(viewModel.ResultView, items);
            }
            else
            {
                if (viewModel.QueryResult == null)
                {
                    viewModel.QueryResult = "~/Views/Common/Module/QueryResult.cshtml";
                }

                return View(viewModel.QueryResult, items);
            }
        }

        public ActionResult CreateExcelDownloadResult(IQueryable<dynamic> items, String tableName, String downloadFileName)
        {

            DataTable table = new DataTable(tableName);
            items.BuildDataColumns(table);

            ProcessRequest processItem = new ProcessRequest
            {
                Sender = HttpContext.GetUser()?.UID,
                SubmitDate = DateTime.Now,
                ProcessStart = DateTime.Now,
                ResponsePath = System.IO.Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".xlsx"),
            };
            models.GetTable<ProcessRequest>().InsertOnSubmit(processItem);
            models.SubmitChanges();

            SqlCommand sqlCmd = (SqlCommand)models.GetCommand(items);
            sqlCmd.SaveAsExcel(table, processItem.ResponsePath, processItem.TaskID);

            return View("~/Views/Shared/Module/PromptCheckDownload.cshtml",
                    new AttachmentViewModel
                    {
                        TaskID = processItem.TaskID,
                        FileName = processItem.ResponsePath,
                        FileDownloadName = downloadFileName,
                    });
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