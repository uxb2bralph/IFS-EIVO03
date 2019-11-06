using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.DataAccessLayer;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class InvoiceAuditController : SampleController<InvoiceItem>
    {
        // GET: InvoiceAudit
        public ActionResult QueryIndex(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            viewModel.QueryForm = "~/Views/InvoiceAudit/Module/InvoiceQuery.cshtml";
            viewModel.DocType = Naming.DocumentTypeDefinition.E_Invoice;
            return View("~/Views/InvoiceAudit/QueryIndex.cshtml");
        }

        public ActionResult AllowanceIndex(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            viewModel.QueryForm = "~/Views/InvoiceAudit/Module/AllowanceQuery.cshtml";
            viewModel.DocType = Naming.DocumentTypeDefinition.E_Allowance;
            return View("~/Views/InvoiceAudit/QueryIndex.cshtml");
        }

        public ActionResult InquireInvoice(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            IQueryable<InvoiceItem> items = models.GetDataContext().FilterInvoiceByRole(profile, models.Items)
                    .InquireInvoice(viewModel, models);

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/InvoiceAudit/Module/InvoiceTable.cshtml", items);
            }
            else
            {
                viewModel.ResultView = "~/Views/InvoiceAudit/Module/InvoiceTable.cshtml";
                return View("~/Views/InvoiceAudit/Module/InvoiceQueryResult.cshtml", items);
            }

        }

        public ActionResult InquireAllowance(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            IQueryable<InvoiceAllowance> items = models.GetDataContext().FilterAllowanceByRole(profile, models.GetTable<InvoiceAllowance>())
                    .InquireAllowance(viewModel, models);

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/InvoiceAudit/Module/AllowanceTable.cshtml", items);
            }
            else
            {
                viewModel.ResultView = "~/Views/InvoiceAudit/Module/AllowanceTable.cshtml";
                return View("~/Views/InvoiceAudit/Module/AllowanceQueryResult.cshtml", items);
            }

        }

    }
}