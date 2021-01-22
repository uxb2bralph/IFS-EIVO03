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
using Model.InvoiceManagement.Validator;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.DataAccessLayer;

namespace eIVOGo.Controllers
{
    public class IndividualProcessController : SampleController<InvoiceItem>
    {
        public ActionResult Index(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            return View("~/Views/IndividualProcess/Index.cshtml");

        }

        public ActionResult Inquire(InquireInvoiceViewModel viewModel,String ValidCode,String EncryptedCode)
        {
            ViewBag.ViewModel = viewModel;

            ValidCode = ValidCode.GetEfficientString();
            EncryptedCode = EncryptedCode.GetEfficientString();
            if (ValidCode == null)
            {
                ModelState.AddModelError("ValidCode", "請輸入驗證碼!!");
            }
            else if (EncryptedCode == null
                || String.Compare(ValidCode, Encoding.Default.GetString(AppResource.Instance.Decrypt(Convert.FromBase64String(EncryptedCode))), true) != 0)
            {
                ModelState.AddModelError("ValidCode", "驗證碼錯誤!!");
            }

            viewModel.InvoiceNo = viewModel.InvoiceNo.GetEfficientString();
            var match = viewModel.InvoiceNo.ParseInvoiceNo();

            viewModel.CarrierNo = viewModel.CarrierNo.GetEfficientString();

            if (viewModel.CarrierNo == null)
            {
                if (!match.Success)
                {
                    ModelState.AddModelError("InvoiceNo", "請輸入發票號碼!!");
                    ModelState.AddModelError("CarrierNo", "請輸入載具號碼!!");
                }

                if (!viewModel.InvoiceDate.HasValue)
                {
                    ModelState.AddModelError("InvoiceDate", "請輸入發票開立日期!!");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            IQueryable<InvoiceItem> items = models.GetTable<InvoiceItem>();

            if(match.Success)
            {
                items = items
                    .Where(i => i.TrackCode == match.Groups[1].Value && i.No == match.Groups[2].Value)
                    .Where(i => i.InvoiceDate >= viewModel.InvoiceDate && i.InvoiceDate < viewModel.InvoiceDate.Value.AddDays(1));
            }

            if (viewModel.CarrierNo != null)
            {
                items = items.Join(models.GetTable<InvoiceCarrier>()
                                    .Where(c => c.CarrierNo == viewModel.CarrierNo),
                                i => i.InvoiceID, c => c.InvoiceID, (i, c) => i);
            }

            viewModel.BuyerName = viewModel.BuyerName.GetEfficientString();
            if (viewModel.BuyerName != null)
            {
                items = items.Where(i => i.InvoiceBuyer.Name == viewModel.BuyerName);
            }

            viewModel.RandomNo = viewModel.RandomNo.GetEfficientString();
            if (viewModel.RandomNo != null)
            {
                items = items.Where(i => i.RandomNo == viewModel.RandomNo);
            }

            var item = items.FirstOrDefault();
            return View("~/Views/IndividualProcess/DataQuery/InvoiceQueryResult.cshtml", item);
        }

    }
}
