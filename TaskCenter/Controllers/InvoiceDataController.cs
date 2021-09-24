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
using TaskCenter.Helper.RequestAction;
using TaskCenter.Properties;
using Model.InvoiceManagement;

namespace TaskCenter.Controllers
{
    public class InvoiceDataController : SampleController
    {
        public ActionResult DeleteProcessRequest(InvoiceRequestViewModel viewModel)
        {
            if (viewModel.KeyID != null)
            {
                viewModel.TaskID = DecryptKeyValue(viewModel, out bool expired);
                if (expired)
                {
                    ModelState.AddModelError("E1001", ErrorMessage.E1001);
                }
            }

            if (!ModelState.IsValid)
            {
                return Json(new { result = false, errorCode = ModelState.AllErrorKey() }, JsonRequestBehavior.AllowGet);
            }

            var count = models.ExecuteCommand("delete [proc].ProcessRequest where TaskID = {0}", viewModel.TaskID);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadProcessRequest(InvoiceRequestViewModel viewModel)
        {
            ViewBag.DataItem = viewModel.SaveProcessRequest(this);

            if (!ModelState.IsValid)
            {
                return Json(new { result = false, errorCode = ModelState.AllErrorKey() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadAttachment(InvoiceRequestViewModel viewModel)
        {
            viewModel.SaveAttachment(this);
            if (!ModelState.IsValid)
            {
                return Json(new { result = false, errorCode = ModelState.AllErrorKey() }, JsonRequestBehavior.AllowGet);
            }

            AttachmentManager.MatchAttachment();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadInvoiceRequestXlsx(InvoiceRequestViewModel viewModel)
        {
            if (!viewModel.ProcessType.HasValue)
            {
                viewModel.ProcessType = Naming.InvoiceProcessType.C0401_Xlsx;
            }

            return UploadProcessRequest(viewModel);

        }

        public ActionResult UploadVoidInvoiceRequestXlsx(InvoiceRequestViewModel viewModel)
        {
            viewModel.ProcessType = Naming.InvoiceProcessType.C0501_Xlsx;
            return UploadProcessRequest(viewModel);
        }

        public ActionResult UploadAllowanceRequestXlsx(InvoiceRequestViewModel viewModel)
        {
            viewModel.ProcessType = Naming.InvoiceProcessType.D0401_Xlsx;
            return UploadProcessRequest(viewModel);
        }

        public ActionResult UploadFullAllowanceRequestXlsx(InvoiceRequestViewModel viewModel)
        {
            viewModel.ProcessType = Naming.InvoiceProcessType.D0401_Full_Xlsx;
            return UploadProcessRequest(viewModel);
        }

        public ActionResult UploadVoidAllowanceRequestXlsx(InvoiceRequestViewModel viewModel)
        {
            viewModel.ProcessType = Naming.InvoiceProcessType.D0501_Xlsx;
            return UploadProcessRequest(viewModel);
        }

        public ActionResult GetResource(String path)
        {
            if (path != null)
            {
                String filePath = path.DecryptData().StoreTargetPath();
                if (System.IO.File.Exists(filePath))
                {
                    return File(filePath, "application/octet-stream", Path.GetFileName(filePath));
                }
            }
            return new EmptyResult { };
        }

        public ActionResult NotifyRequestCompletion(InvoiceRequestViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.AgentID = DecryptKeyValue(viewModel, out bool expired);
                if (expired)
                {
                    return Json(new { result = false, message = ErrorMessage.E1001 },JsonRequestBehavior.AllowGet);
                }
            }

            var items = models.GetTable<ProcessRequest>().Where(q => q.AgentID == viewModel.AgentID)
                                .Where(q => q.ProcessCompletionNotification != null);

            return Json(new
            {
                result = items.Count() > 0,
                data = items.Select(q =>
                        new
                        {
                            q.TaskID,
                            q.ProcessRequestType.ChannelName,
                            q.ProcessRequestType.ChannelResponse,
                            ResponseName = Path.GetFileName(q.ResponsePath)
                        }).ToArray()
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CommitProcessResponse(InvoiceRequestViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.AgentID = DecryptKeyValue(viewModel, out bool expired);
                if (expired)
                {
                    return new EmptyResult { };
                }
            }

            var result = models.ExecuteCommand(@"
                        DELETE FROM [proc].ProcessCompletionNotification
                        FROM              [proc].ProcessCompletionNotification INNER JOIN
                                                    [proc].ProcessRequest ON [proc].ProcessCompletionNotification.TaskID = [proc].ProcessRequest.TaskID
                        WHERE          ([proc].ProcessCompletionNotification.TaskID = {0}) AND ([proc].ProcessRequest.AgentID = {1})", viewModel.TaskID, viewModel.AgentID);

            if (result > 0)
            {
                var item = models.GetTable<ProcessRequest>().Where(q => q.TaskID == viewModel.TaskID).FirstOrDefault();
                if (item != null && System.IO.File.Exists(item.ResponsePath))
                {
                    String fileName = Path.GetFileName(item.ResponsePath);
                    fileName = fileName.Substring(fileName.IndexOf('_') + 1);
                    return File(item.ResponsePath, "application/octet-stream", fileName);
                }
            }
            return new EmptyResult { };
        }

    }
}