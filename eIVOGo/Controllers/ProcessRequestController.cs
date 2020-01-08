using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using DataAccessLayer.basis;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Helper;
using Model.Resource;
using Model.Security.MembershipManagement;
using Utility;
using Model.InvoiceManagement.InvoiceProcess;
using Uxnet.Com.DataAccessLayer;
using ModelExtension.Helper;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class ProcessRequestController : SampleController<InvoiceItem>
    {
        // GET: ProcessRequest
        public ActionResult QueryIndex(ProcessRequestQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            viewModel.QueryForm = "~/Views/ProcessRequest/Module/ProcessRequestQuery.cshtml";
            return View("~/Views/Common/QueryIndex.cshtml");
        }

        public ActionResult ShowData(ProcessRequestQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireRequest(viewModel);
            viewModel.ResultView = "~/Views/ProcessRequest/Module/ProcessRequestTable.cshtml";
            viewModel.QueryForm = "~/Views/Common/Module/QueryResult.cshtml";
            result.ViewName = "~/Views/Common/QueryIndex.cshtml";
            return result;
        }


        public ActionResult InquireRequest(ProcessRequestQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            if (viewModel.KeyID != null)
            {
                viewModel.TaskID = viewModel.DecryptKeyValue();
            }

            IQueryable<ProcessRequest> items = models.GetDataContext().FilterProcessRequestByRole(profile, models.GetTable<ProcessRequest>());
            if(viewModel.TaskID.HasValue)
            {
                items = items.Where(p => p.TaskID == viewModel.TaskID);
            }

            if(viewModel.SubmitDateFrom.HasValue)
            {
                items = items.Where(p => p.SubmitDate >= viewModel.SubmitDateFrom);
            }

            if (viewModel.SubmitDateTo.HasValue)
            {
                items = items.Where(p => p.SubmitDate < viewModel.SubmitDateTo.Value.AddDays(1));
            }

            if (viewModel.ProcessStartFrom.HasValue)
            {
                items = items.Where(p => p.ProcessStart >= viewModel.ProcessStartFrom);
            }

            if (viewModel.ProcessStartTo.HasValue)
            {
                items = items.Where(p => p.ProcessStart < viewModel.ProcessStartTo.Value.AddDays(1));
            }

            if (viewModel.ProcessCompleteFrom.HasValue)
            {
                items = items.Where(p => p.ProcessComplete >= viewModel.ProcessCompleteFrom);
            }

            if (viewModel.ProcessCompleteTo.HasValue)
            {
                items = items.Where(p => p.ProcessComplete < viewModel.ProcessCompleteTo.Value.AddDays(1));
            }

            viewModel.RecordCount = items.Count();
            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/ProcessRequest/Module/ProcessRequestTable.cshtml", items);
            }
            else
            {
                viewModel.ResultView = "~/Views/ProcessRequest/Module/ProcessRequestTable.cshtml";
                return View("~/Views/Common/Module/QueryResult.cshtml", items);
            }

        }

    }
}