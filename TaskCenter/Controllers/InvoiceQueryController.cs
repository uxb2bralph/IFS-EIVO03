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
using Model.Security;

namespace TaskCenter.Controllers
{
    public class InvoiceQueryController : SampleController
    {
        public ActionResult Inquire(InvoiceDataQueryViewModel viewModel)
        {
            Organization item = viewModel.CheckRequest(this);

            if (item != null)
            {
                switch(viewModel.QueryType)
                {
                    case DataQueryType.Invoice:
                    case DataQueryType.CountInvoice:
                        return JsonInvoice(viewModel, item);

                    case DataQueryType.VoidInvoice:
                    case DataQueryType.CountVoidInvoice:
                        return JsonVoidInvoice(viewModel, item);

                    case DataQueryType.Allowance:
                    case DataQueryType.CountAllowance:
                        return JsonAllowance(viewModel, item);

                    case DataQueryType.VoidAllowance:
                    case DataQueryType.CountVoidAllowance:
                        return JsonVoidAllowance(viewModel, item);
                }
            }

            if (!ModelState.IsValid)
            {
                return Json(new { result = false, errorCode = ModelState.AllErrorKey() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JsonInvoice(InvoiceDataQueryViewModel viewModel,Organization agent)
        {
            IQueryable<InvoiceItem> items = models.DataContext.GetInvoiceByAgent(agent.CompanyID);

            items = items.InquireInvoice(viewModel, models);

            if (viewModel.QueryType == DataQueryType.CountInvoice)
            {
                return Json(new { TotalCount = items.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (viewModel.PageIndex > 0 && viewModel.PageSize > 0)
                {
                    items = items.Skip((viewModel.PageIndex.Value - 1) * viewModel.PageSize.Value)
                        .Take(viewModel.PageSize.Value);
                }

                var dataItems = items.Select(c => c.CreateC0401(true)).ToList();
                return Content(dataItems.JsonStringify(), "application/json");
            }
        }

        public ActionResult JsonVoidInvoice(InvoiceDataQueryViewModel viewModel, Organization agent)
        {
            IQueryable<InvoiceItem> items = models.DataContext.GetInvoiceByAgent(agent.CompanyID);

            items = items.InquireVoidInvoice(viewModel, models);

            if (viewModel.QueryType == DataQueryType.CountVoidInvoice)
            {
                return Json(new { TotalCount = items.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (viewModel.PageIndex > 0 && viewModel.PageSize > 0)
                {
                    items = items.Skip((viewModel.PageIndex.Value - 1) * viewModel.PageSize.Value)
                        .Take(viewModel.PageSize.Value);
                }

                var dataItems = items.Select(c => c.CreateC0501(true)).ToList();
                return Content(dataItems.JsonStringify(), "application/json");
            }
        }

        public ActionResult JsonAllowance(InvoiceDataQueryViewModel viewModel, Organization agent)
        {
            IQueryable<InvoiceAllowance> items = models.DataContext.GetAllowanceByAgent(agent.CompanyID);

            items = items.InquireAllowance(viewModel, models);

            if (viewModel.QueryType == DataQueryType.CountAllowance)
            {
                return Json(new { TotalCount = items.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (viewModel.PageIndex > 0 && viewModel.PageSize > 0)
                {
                    items = items.Skip((viewModel.PageIndex.Value - 1) * viewModel.PageSize.Value)
                        .Take(viewModel.PageSize.Value);
                }
                var dataItems = items.Select(c => c.CreateD0401(models, true)).ToList();
                return Content(dataItems.JsonStringify(), "application/json");
            }
        }

        public ActionResult JsonVoidAllowance(InvoiceDataQueryViewModel viewModel, Organization agent)
        {
            IQueryable<InvoiceAllowance> items = models.DataContext.GetAllowanceByAgent(agent.CompanyID);

            items = items.InquireVoidAllowance(viewModel, models);

            if (viewModel.QueryType == DataQueryType.CountVoidAllowance)
            {
                return Json(new { TotalCount = items.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (viewModel.PageIndex > 0 && viewModel.PageSize > 0)
                {
                    items = items.Skip((viewModel.PageIndex.Value - 1) * viewModel.PageSize.Value)
                        .Take(viewModel.PageSize.Value);
                }

                var dataItems = items.Select(c => c.CreateD0501(true)).ToList();
                return Content(dataItems.JsonStringify(), "application/json");

            }

        }
    }
}