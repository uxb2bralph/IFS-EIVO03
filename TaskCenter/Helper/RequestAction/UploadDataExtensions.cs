using ApplicationResource;
using Business.Helper;
using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using Model.Helper;
using TaskCenter.Controllers;

namespace TaskCenter.Helper.RequestAction
{
    public static class UploadDataExtensions
    {
        public static ProcessRequest SaveProcessRequest(this InvoiceRequestViewModel viewModel, SampleController controller, Naming.InvoiceProcessType processType)
        {
            var ModelState = controller.ModelState;
            var ViewBag = controller.ViewBag;
            var HttpContext = controller.HttpContext;
            var Request = controller.Request;
            var models = controller.DataSource;

            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.AgentID = controller.DecryptKeyValue(viewModel, out bool expired);
                if (expired)
                {
                    ModelState.AddModelError("E1001", ErrorMessage.E1001);
                }
            }

            var item = models.GetTable<Organization>().Where(c => c.CompanyID == viewModel.AgentID).FirstOrDefault();
            if (item == null)
            {
                ModelState.AddModelError("E1003", ErrorMessage.E1003);
            }

            String requestPath = null;
            if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("E1002", ErrorMessage.E1002);
            }
            else
            {
                var file = Request.Files[0];
                requestPath = DateTime.Today.DailyStorePath(DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName), out string path);
                file.SaveAs(path);
            }

            if (!ModelState.IsValid)
            {
                return null;
            }

            var processItem = new ProcessRequest
            {
                AgentID = item.CompanyID,
                Sender = viewModel.Sender,
                SubmitDate = DateTime.Now,
                RequestPath = requestPath,
                ProcessType = (int)processType,
                ProcessRequestQueue = new ProcessRequestQueue
                {

                }
            };
            models.GetTable<ProcessRequest>().InsertOnSubmit(processItem);
            models.SubmitChanges();

            return processItem;

        }

    }
}