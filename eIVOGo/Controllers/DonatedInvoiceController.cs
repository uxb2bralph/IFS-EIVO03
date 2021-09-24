using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.DataEntity;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.Security.MembershipManagement;
using Business.Helper;
using System.Text;
using Model.Locale;
using Model.Models.ViewModel;

namespace eIVOGo.Controllers
{
    public class DonatedInvoiceController : SampleController<InvoiceItem>
    {
        protected ModelSourceInquiry<InvoiceItem> createModelInquiry()
        {
            UserProfileMember userProfile = WebPageUtility.UserProfile;


            return (ModelSourceInquiry<InvoiceItem>)(new InquireDonatedInvoice { ControllerName = "InquireInvoice", ActionName = "ByDonation", CurrentController = this })
                .Append(new InquireInvoiceByRole(userProfile) { CurrentController = this })
                .Append(new InquireInvoiceSeller { ControllerName = "InquireInvoice", ActionName = "BySeller", CurrentController = this })
                .Append(new InquireInvoiceDate { ControllerName = "InquireInvoice", ActionName = "ByInvoiceDate", CurrentController = this })
                .Append(new InquireDonatory { ControllerName = "InquireInvoice", ActionName = "ByDonatory", CurrentController = this });
        }

        public ActionResult ReportIndex(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();

            return View(models.Inquiry);
        }

        public ActionResult InquireReport(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            return View("ReportResult", models.Inquiry);
        }

        public ActionResult ReportGridPage(InquireInvoiceViewModel viewModel,int index,int size)
        {
            //ViewBag.HasQuery = true;
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            if (index > 0)
                index--;
            else
                index = 0;

            var items = models.Items.OrderByDescending(d => d.InvoiceID)
                .Skip(index * size).Take(size);

            return View(items);
        }

        public ActionResult DownloadCSV(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            Response.ContentEncoding = Encoding.GetEncoding(950);

            return View(models.Items);
        }

        public ActionResult PrintResult(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            ((ModelSource<InvoiceItem>)models).ResultModel = Naming.DataResultMode.Print;

            return View(models.Inquiry);
        }

    }
}
