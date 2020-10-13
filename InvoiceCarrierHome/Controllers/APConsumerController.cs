using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceCarrierHome.Models.ViewModel;
using Model.DataEntity;
using InvoiceDetail = Model.DataEntity.InvoiceDetail;

namespace InvoiceCarrierHome.Controllers
{
    public class APConsumerController : SampleController<InvoiceItem>
    {
        // GET: APConsumer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
        
        [BotDetect.Web.Mvc.CaptchaValidation("" +
             "imagecode", "validatedigit", "Incorrect Captcha code!")]
        public JsonResult Post(APConsumerViewModel viewModel, bool captchaValid)
        {
            if (!ModelState.IsValid)
            {
                //Captcha validation failed, show error message
                TempData["ValidateError"] = "驗證碼錯誤";
                //return View("~/Views/APConsumer/Index.cshtml", viewModel);
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var invoiceItem = new InvoiceItemData();
                var date = DateTime.Parse(viewModel.InvoiceDate);
                //取得發票明細
                InvoiceItem item = models.GetTable<InvoiceItem>()
                    .Where(i => (i.TrackCode.Trim() + i.No.Trim()).Equals(viewModel.InvoiceNumber)
                    && (i.RandomNo).Equals(viewModel.RandomCode) && (i.InvoiceDate>= date))
                    .FirstOrDefault();

                if(item ==null)
                {
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }

                //幣別
                var currency = models.GetTable<CurrencyType>()
                    .Where(i => (i.CurrencyID).Equals(item.InvoiceAmountType.CurrencyID)).FirstOrDefault();

                //公司
                var organization = models.GetTable<Organization>()
                    .Where(i => (i.CompanyID).Equals(item.SellerID)).FirstOrDefault();

                invoiceItem.InvoiceNumber = item.InvoiceNo();
                invoiceItem.InvoiceDate = item.InvoiceDate.Value.ToString();
                invoiceItem.TotalAmount = Math.Round(item.InvoiceAmountType.TotalAmount.Value,0).ToString();
                if (currency != null)
                {
                    invoiceItem.Currency = currency.AbbrevName;
                }
                invoiceItem.InvoiceStatus = "已確認";
                invoiceItem.CompanyName = organization.CompanyName;
                invoiceItem.ReceiptNo = organization.ReceiptNo;
                invoiceItem.Address = organization.Addr;
                invoiceItem.Remarks = "";

                //明細
                var items = from o in models.GetTable<InvoiceProductItem>()
                            join n in models.GetTable<InvoiceProduct>()
                            on o.ProductID equals n.ProductID
                            join p in models.GetTable<InvoiceDetail>()
                            on o.ProductID equals p.ProductID
                            join j in models.GetTable<InvoiceItem>()
                            on p.InvoiceID equals j.InvoiceID
                            where j.InvoiceID == item.InvoiceID
                            select new { o.ProductID, o.Piece, o.UnitCost, o.CostAmount, o.Remark,n.Brief };

                var details = new List<InvoiceDetailData>();

                foreach (var data in items)
                {
                    var invoiceDetailData = new InvoiceDetailData();
                    invoiceDetailData.ProductName = data.Brief;
                    invoiceDetailData.Quantity = Math.Round(data.Piece.Value,0).ToString();
                    invoiceDetailData.UnitPrice = Math.Round(data.UnitCost.Value,0).ToString();
                    invoiceDetailData.Subtotal = Math.Round(data.CostAmount.Value,0).ToString();
                    
                    details.Add(invoiceDetailData);
                }

                invoiceItem.Results = details;

                viewModel.Result = invoiceItem;

                //return View("~/Views/APConsumer/Index.cshtml", viewModel);
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //[BotDetect.Web.Mvc.CaptchaValidation("" +
        //    "imagecode", "validatedigit", "Incorrect Captcha code!")]
        //public JsonResult Post(APConsumerViewModel viewModel, bool captchaValid)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //Captcha validation failed, show error message
        //        TempData["ValidateError"] = "驗證碼錯誤";
        //        //return View("~/Views/APConsumer/Index.cshtml", viewModel);
        //        return Json(viewModel, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var invoiceItem = new InvoiceItemData();

        //        //取得發票明細
        //        InvoiceItem item = models.GetTable<InvoiceItem>()
        //            .Where(i => (i.TrackCode.Trim() + i.No.Trim()).Equals(viewModel.InvoiceNumber)
        //            && (i.RandomNo).Equals(viewModel.RandomCode))
        //            .FirstOrDefault();

        //        //幣別
        //        var currency = models.GetTable<CurrencyType>()
        //            .Where(i => (i.CurrencyID).Equals(item.InvoiceAmountType.CurrencyID)).FirstOrDefault();

        //        //公司
        //        var organization = models.GetTable<Organization>()
        //            .Where(i => (i.CompanyID).Equals(item.SellerID)).FirstOrDefault();

        //        invoiceItem.InvoiceNumber = item.InvoiceNo();
        //        invoiceItem.InvoiceDate = item.InvoiceDate.Value.ToString();
        //        invoiceItem.TotalAmount = item.InvoiceAmountType.TotalAmount.Value.ToString();
        //        if (currency != null)
        //        {
        //            invoiceItem.Currency = currency.AbbrevName;
        //        }
        //        invoiceItem.InvoiceStatus = "已確認";
        //        invoiceItem.CompanyName = organization.CompanyName;
        //        invoiceItem.ReceiptNo = organization.ReceiptNo;
        //        invoiceItem.Address = organization.Addr;
        //        invoiceItem.Remarks = "";

        //        //明細
        //        var items = from o in models.GetTable<InvoiceProductItem>()
        //                          join p in models.GetTable<InvoiceDetail>()
        //                          on o.ProductID equals p.ProductID
        //                          join j in models.GetTable<InvoiceItem>()
        //                          on p.InvoiceID equals j.InvoiceID
        //                          where j.InvoiceID == item.InvoiceID
        //                          select new { o.ProductID,o.Piece, o.UnitCost, o.CostAmount ,o.Remark};

        //        var details = new List<InvoiceDetailData>();

        //        foreach(var data in items)
        //        {
        //            var invoiceDetailData = new InvoiceDetailData();
        //            invoiceDetailData.Quantity = data.Piece.Value.ToString();
        //            invoiceDetailData.UnitPrice = data.UnitCost.Value.ToString();
        //            invoiceDetailData.Subtotal = data.CostAmount.Value.ToString();

        //            var remark =models.GetTable<InvoiceProduct>()
        //                   .Where(i => (i.ProductID).Equals(data.ProductID)).FirstOrDefault();

        //            invoiceDetailData.Explanation = remark.Brief;//說明事項
        //            details.Add(invoiceDetailData);
        //        }

        //        invoiceItem.Results = details;

        //        viewModel.Results = invoiceItem;

        //        //return View("~/Views/APConsumer/Index.cshtml", viewModel);
        //        return Json(viewModel, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}