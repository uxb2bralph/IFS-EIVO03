using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kiosk.Helper;
using Kiosk.Models.ViewModel;
using Kiosk.Properties;
using Model.Schema.EIVO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility;

namespace Kiosk.Controllers
{
    public class FrontEndController : Controller
    {
        public ActionResult get_invoice_number(POSDeviceViewModel viewModel)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return Content(client.UploadString(Settings.Default.InvoiceServiceHost + "/POSDevice/AllocateInvoiceNo", JsonConvert.SerializeObject(viewModel)), "application/json");
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

            return Json(new
            {
                invoice_issue = new object[]
                {
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult invoice_manifest(POSInvoiceViewModel viewModel)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    InvoiceViewModel item = new InvoiceViewModel
                    {
                        TrackCode = viewModel.sn.Substring(0, 2),
                        No = viewModel.sn.Substring(2),
                        InvoiceDate = viewModel.time,
                        CarrierId1 = viewModel.carrier,
                        CarrierId2 = viewModel.carrier,
                        CarrierType = viewModel.carrier_type,
                        TotalAmount = viewModel.amount,
                        SalesAmount = Math.Round(viewModel.amount.Value / 1.05m),
                        BuyerReceiptNo = viewModel.buyer,
                        Brief = new string[] { "停車費" },
                        Piece = new int?[] { 1 },
                        UnitCost = new decimal?[] { viewModel.amount },
                        CostAmount = new decimal?[] { viewModel.amount },
                        ItemNo = new string[] { "01" },
                        ItemRemark = new string[] { "" },
                        TaxType = 1
                    };

                    item.TaxAmount = item.TotalAmount - item.SalesAmount;

                    dynamic result = JObject.Parse(client.UploadString(Settings.Default.InvoiceServiceHost + "/POSDevice/CommitInvoice", JsonConvert.SerializeObject(item)));
                    if (result.result == true)
                    {
                        return Content("OK");
                    }
                    else
                    {
                        return Content("ERROR." + result.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content("ERROR." + ex.Message);
            }
        }

        public ActionResult get_invoice_pending(POSDeviceViewModel viewModel)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return Content(client.UploadString(Settings.Default.InvoiceServiceHost + "/POSDevice/GetIncompleteInvoiceNo", JsonConvert.SerializeObject(viewModel)), "application/json");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Json(new
            {
                invoice_pending = new object[]
                {
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reprint(String no)
        {
            var items = Directory.EnumerateFiles(AppSettings.Default.InvoiceClientLogPath, $"{no}*.htm", SearchOption.AllDirectories);
            var item = items.FirstOrDefault();
            if (item != null)
            {
                try
                {
                    System.IO.File.Move(item, Path.Combine(AppSettings.Default.PrintInvoice, Path.GetFileName(item)));
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false, message = "資料錯誤" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintC0401()
        {
            InvoiceRootInvoice viewModel = new InvoiceRootInvoice
            {

            };
            String data = Request.ReadToEnd();
            if(!String.IsNullOrEmpty(data))
            {
                viewModel = JsonConvert.DeserializeObject<InvoiceRootInvoice>(data);
            }
            return View("~/Views/FrontEnd/PrintC0401POS.cshtml", viewModel);
        }

        public ActionResult CommitSettings(String margin, decimal? zoom, PrintMode? printMode)
        {
            margin = margin.GetEfficientString();
            if (margin == null)
            {
                margin = "-0cm";
            }

            if (!(zoom > 0))
            {
                zoom = 100;
            }

            if(!printMode.HasValue)
            {
                printMode = PrintMode.ForPOS;
            }

            AppSettings.Default.Margin = margin;
            AppSettings.Default.Zoom = zoom.Value;
            AppSettings.Default.PrintMode = printMode.Value;
            AppSettings.Default.Save();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            if (!String.IsNullOrEmpty(actionName))
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
        }

    }
}