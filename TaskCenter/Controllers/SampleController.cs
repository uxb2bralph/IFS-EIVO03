using Model.DataEntity;
using Model.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uxnet.Web.Controllers;
using Business.Helper;
using Model.Helper;
using TaskCenter.Properties;
using Utility;

namespace TaskCenter.Controllers
{
    public class SampleController : SampleController<EIVOEntityDataContext>
    {
        // GET: Sample
        protected ModelSource models;
        protected SampleController() : base()
        {
            models = new ModelSource(modelsBase);
        }

        public ModelSource DataSource => models;

        public int DecryptKeyValue(QueryViewModel viewModel,out bool expired)
        {
            int keyID = viewModel.DecryptKeyValue(out long ticks);
            expired = (DateTime.Now.Ticks - ticks) > Settings.Default.TimeoutTicks;
            return keyID;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            System.Diagnostics.Debugger.Launch();
            if (filterContext.Exception != null)
            {
                Logger.Error(filterContext.Exception);

                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        result = false,
                        message = filterContext.Exception?.Message,
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                };
            }
        }
    }
}