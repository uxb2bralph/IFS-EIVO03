using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.Models.ViewModel;
using eIVOGo.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Utility;
using Model.InvoiceManagement;

namespace eIVOGo.Controllers
{
    public class AllowanceProcessController : SampleController<InvoiceItem>
    {
        protected UserProfileMember _userProfile;

        protected ModelSourceInquiry<InvoiceAllowance> createModelInquiry()
        {
            _userProfile = WebPageUtility.UserProfile;

            var inquireConsumption = new InquireAllowanceConsumption { CurrentController = this };
            
            return (ModelSourceInquiry<InvoiceAllowance>)(new InquireEffectiveAllowance { CurrentController = this })
                .Append(new InquireAllowanceByRole(_userProfile) { CurrentController = this })
                .Append(inquireConsumption)
                .Append(new InquireAllowanceSeller { CurrentController = this })
                .Append(new InquireAllowanceBuyer { CurrentController = this })
                .Append(new InquireAllowanceBuyerByName { CurrentController = this })
                .Append(new InquireAllowanceDate { CurrentController = this })
                .Append(new InquireAllowanceNo { CurrentController = this })
                .Append(new InquireAllowanceAgent { CurrentController = this });
        }

        public ActionResult Index(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/AllowanceProcess/Index.cshtml");
        }

        public ActionResult Inquire(InquireInvoiceViewModel viewModel)
        {

            ViewBag.ViewModel = viewModel;

            DataLoadOptions ops = new DataLoadOptions();
            ops.LoadWith<InvoiceAllowance>(i => i.InvoiceAllowanceBuyer);
            ops.LoadWith<InvoiceAllowance>(i => i.InvoiceAllowanceSeller);
            models.GetDataContext().LoadOptions = ops;

            var modelSource = new ModelSource<InvoiceAllowance>(models);

            modelSource.Inquiry = createModelInquiry();
            modelSource.BuildQuery();

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/AllowanceProcess/Module/ItemList.ascx", modelSource.Items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/AllowanceProcess/Module/QueryResult.cshtml", modelSource.Items);
            }
        }

        public ActionResult InquireToVoid(InquireInvoiceViewModel viewModel)
        {
            ViewResult result = (ViewResult)Index(viewModel);
            viewModel.ActionTitle = "作廢折讓";
            viewModel.CommitAction = "VoidAllowance";
            result.ViewName = "~/Views/AllowanceProcess/Index.cshtml";
            ViewBag.ResultAction = "VoidAllowance";
            return result;
        }

        public ActionResult InvokeCommitAction(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/AllowanceProcess/Module/InvokeCommitAction.cshtml");
        }

        public ActionResult CreateXlsx(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            DataLoadOptions ops = new DataLoadOptions();
            ops.LoadWith<InvoiceAllowance>(i => i.InvoiceAllowanceBuyer);
            ops.LoadWith<InvoiceAllowance>(i => i.InvoiceAllowanceSeller);
            models.GetDataContext().LoadOptions = ops;

            ModelSource<InvoiceAllowance> modelSource = new ModelSource<InvoiceAllowance>(models);

            modelSource.Inquiry = createModelInquiry();
            modelSource.BuildQuery();

            var items = modelSource.Items.OrderBy(i => i.AllowanceID)
                .Select(i => new
                {
                    折讓單號碼 = i.AllowanceNumber,
                    原發票號碼 = i.InvoiceAllowanceDetails.First().InvoiceAllowanceItem.InvoiceNo,
                    折讓日期 = i.AllowanceDate,
                    客戶ID = i.InvoiceAllowanceBuyer.CustomerID,
                    發票開立人 = i.InvoiceAllowanceSeller.CustomerName,
                    開立人統編 = i.InvoiceAllowanceSeller.ReceiptNo,
                    未稅金額 = i.TotalAmount,
                    稅額 = i.TaxAmount,
                    含稅金額 = i.TotalAmount + i.TaxAmount,
                    買受人名稱 = i.InvoiceAllowanceBuyer.CustomerName,
                    買受人統編 = i.InvoiceAllowanceBuyer.ReceiptNo,
                    連絡人名稱 = i.InvoiceAllowanceBuyer.ContactName,
                    連絡人地址 = i.InvoiceAllowanceBuyer.Address,
                    買受人EMail = i.InvoiceAllowanceBuyer.EMail,
                    備註 = i.InvoiceAllowanceDetails.First().InvoiceAllowanceItem.Remark
                    //備註 = String.Join("", i.InvoiceAllowanceDetails.Select(t => t.InvoiceAllowanceItem)
                    //    .Select(p => p.Remark))
                });


            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "message/rfc822";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("折讓資料明細.xlsx")));

            using (DataSet ds = new DataSet())
            {
                DataTable table = new DataTable("折讓資料明細");
                ds.Tables.Add(table);
                table.Columns.Add("折讓單號碼");
                table.Columns.Add("原發票號碼");
                table.Columns.Add("折讓日期");
                table.Columns.Add("客戶ID");
                table.Columns.Add("發票開立人");
                table.Columns.Add("開立人統編");
                table.Columns.Add("未稅金額");
                table.Columns.Add("稅額");
                table.Columns.Add("含稅金額");
                table.Columns.Add("買受人名稱");
                table.Columns.Add("買受人統編");
                table.Columns.Add("連絡人名稱");
                table.Columns.Add("連絡人地址");
                table.Columns.Add("買受人EMail");
                table.Columns.Add("備註");

                DataSource.GetDataSetResult(items, table);
                foreach (var r in table.Select("買受人統編 = '0000000000'"))
                {
                    r["買受人統編"] = "";
                }

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            return new EmptyResult();
        }

        public ActionResult VoidAllowance(int?[] chkItem, Naming.InvoiceProcessType? proceType)
        {
            if (chkItem != null && chkItem.Count() > 0)
            {
                InvoiceManager mgr = new InvoiceManager(models);
                mgr.VoidAllowance(chkItem);
                if (mgr.EventItems_Allowance != null && mgr.EventItems_Allowance.Count > 0)
                {
                    ViewBag.Message = "下列折讓已作廢完成!!\r\n" + String.Join("\r\n", mgr.EventItems_Allowance.Select(i => i.AllowanceNumber));
                    EIVOPlatformFactory.Notify();
                }
                return View("~/Views/Shared/AlertMessage.cshtml");
            }
            else
            {
                ViewBag.Message = "請選擇作廢資料!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

        }

        public ActionResult Print(int[] chkItem, RenderStyleViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var profile = HttpContext.GetUser();
            if (profile.EnqueueDocumentPrint(models, chkItem))
            {
                return View("~/Views/AllowanceProcess/Module/PrintResult.ascx");
            }
            else
                return Json(new { result = false, message = "資料已列印請重新選擇!!" });
        }

        public ActionResult IssueAllowanceNotice(int[] chkItem,bool? cancellation)
        {
            if (chkItem != null && chkItem.Count() > 0)
            {
                if (cancellation == true)
                {
                    chkItem.NotifyIssuedAllowanceCancellation();
                }
                else
                {
                    chkItem.NotifyIssuedAllowance();
                }
                ViewBag.Message = "Email通知已重送!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }
            else
            {
                ViewBag.Message = "請選擇重送資料!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

        }


    }
}
