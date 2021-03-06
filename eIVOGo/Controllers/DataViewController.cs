﻿using System;
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
using eIVOGo.Properties;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.TXN;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Model.Schema.EIVO;
using Newtonsoft.Json;

namespace eIVOGo.Controllers
{
    
    public class DataViewController : SampleController<InvoiceItem>
    {
        // GET: DataView
        public ActionResult ShowAllowancePageView(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.id = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceAllowance>().Where(a => a.AllowanceID == viewModel.id).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }


            return View("~/Views/DataView/Module/Allowance.cshtml", item);
        }

        public ActionResult ShowAllowance(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ShowAllowancePageView(viewModel);

            InvoiceAllowance item = result.Model as InvoiceAllowance;
            if (item == null)
                return result;

            String[] useThermalPOSArgs;
            return View(getAllowanceViewPath(item, out useThermalPOSArgs), item);
        }

        protected String getInvoiceViewPath(InvoiceItem item, out String[] useThermalPOSArgs,String paperStyle = null)
        {
            useThermalPOSArgs = null;
            if (item.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.A0401)
            {
                return "~/Views/DataView/A0401.aspx";
            }
            else if (paperStyle == "POS")
            {
                useThermalPOSArgs = ThermalPOSPaper;
                return "~/Views/DataView/C0401_POS.cshtml";
            }
            else
            {
                return "~/Views/DataView/C0401_A4.aspx";
            }
        }

        protected String getAllowanceViewPath(InvoiceAllowance item,out String[] useThermalPOSArgs)
        {
            useThermalPOSArgs = null;
            if (item.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.B0401)
            {
                return "~/Views/DataView/B0401.aspx";
            }
            else
            {
                useThermalPOSArgs = ThermalPOSPaper;
                return "~/Views/DataView/D0401.cshtml";
            }
        }

        public ActionResult PrintSingleInvoiceAsPDF(RenderStyleViewModel viewModel)
        {
            ViewResult result = (ViewResult)ShowInvoice(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;
            if (item == null)
            {
                return new EmptyResult { };
            }

            this.TempData["viewModel"] = viewModel;
            String[] useThermalPOSArgs;
            String pdfFile = this.CreateContentAsPDF(getInvoiceViewPath(item, out useThermalPOSArgs, viewModel.PaperStyle), item, Session.Timeout, useThermalPOSArgs);

            if (pdfFile != null)
            {
                if (viewModel.NameOnly == true)
                {
                    String outputFile = Path.Combine(Logger.LogDailyPath, Path.GetFileName(pdfFile));
                    System.IO.File.Move(pdfFile, outputFile);

                    return Content(outputFile);
                }
                else
                {
                    return File(pdfFile, "application/pdf",$"{DateTime.Today:yyyy-MM-dd}.pdf");
                }
            }

            return new EmptyResult { };

        }

        public ActionResult GetCustomerInvoicePDF(RenderStyleViewModel viewModel, bool? ackDel, bool? html)
        {
            ViewBag.ViewModel = viewModel;
            viewModel.UseCustomView = viewModel.UseCustomView ?? true;

            if (viewModel.KeyID != null)
            {
                viewModel.DocID = viewModel.DecryptKeyValue();
            }

            if (ackDel == true)
            {
                models.ExecuteCommand("delete DocumentSubscriptionQueue where DocID = {0}", viewModel.DocID);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else if (html == true)
            {
                models.ExecuteCommand("delete DocumentSubscriptionQueue where DocID = {0}", viewModel.DocID);
            }

            var item = models.GetTable<InvoiceItem>().Where(i => i.InvoiceID == viewModel.DocID).FirstOrDefault();

            if (item != null)
            {
                if (html == true)
                {
                    String[] useThermalPOSArgs;
                    return View(getInvoiceViewPath(item, out useThermalPOSArgs, viewModel.PaperStyle), item);
                }
                else
                {
                    String outputFile = GetInvoicePDF(item, viewModel);
                    return File(outputFile, "application/pdf", $"{item.TrackCode}{item.No}.pdf");
                }
            }

            return new EmptyResult { };

        }

        protected String GetInvoicePDF(InvoiceItem item,RenderStyleViewModel viewModel)
        {
            String outputFile = Path.Combine(Logger.LogPath.GetDateStylePath(item.InvoiceDate.Value), String.Format("{0}{1}.pdf", item.TrackCode, item.No));
            if (viewModel.CreateNew != true && System.IO.File.Exists(outputFile))
            {

            }
            else
            {
                String[] useThermalPOSArgs;
                this.TempData["viewModel"] = viewModel;
                String pdfFile = this.CreateContentAsPDF(getInvoiceViewPath(item, out useThermalPOSArgs, viewModel.PaperStyle), item, Session.Timeout, useThermalPOSArgs);
                if (pdfFile != null)
                {
                    if (System.IO.File.Exists(outputFile))
                    {
                        System.IO.File.Delete(outputFile);
                    }
                    System.IO.File.Move(pdfFile, outputFile);
                }
            }

            return outputFile;
        }

        public ActionResult ZipInvoicePDF(RenderStyleViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var profile = HttpContext.GetUser();

            var items = models.GetTable<DocumentPrintQueue>()
                .Where(i => i.UID == profile.UID)
                .Join(models.GetTable<CDS_Document>()
                        .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice),
                    i => i.DocID, d => d.DocID, (i, d) => i);

            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
            Response.AppendCookie(new HttpCookie("FileDownloadToken", viewModel.FileDownloadToken));
            //Response.AddHeader("Cache-control", "max-age=1");
            //Response.ContentType = "application/octet-stream";
            //Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}({1:yyyy-MM-dd HH-mm-ss}).zip", HttpUtility.UrlEncode("電子發票下載"), DateTime.Now));

            String outFile = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".zip");
            using (var zipOut = System.IO.File.Create(outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    foreach (var doc in items)
                    {
                        InvoiceItem item = doc.CDS_Document.InvoiceItem;
                        var pdfFile = GetInvoicePDF(item, viewModel);
                        models.MarkPrintedLog(item, profile);

                        zip.CreateEntryFromFile(pdfFile, Path.GetFileName(pdfFile));
                    }
                }
            }

            var result = new FilePathResult(outFile, "application/octet-stream");
            result.FileDownloadName = "發票列印下載.zip";
            return result;
        }

        public ActionResult GetCustomerAllowancePDF(RenderStyleViewModel viewModel, bool? ackDel, bool? html)
        {
            if (viewModel.KeyID != null)
            {
                viewModel.DocID = viewModel.DecryptKeyValue();
            }

            if (ackDel == true)
            {
                models.ExecuteCommand("delete DocumentSubscriptionQueue where DocID = {0}", viewModel.DocID);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else if (html == true)
            {
                models.ExecuteCommand("delete DocumentSubscriptionQueue where DocID = {0}", viewModel.DocID);
            }

            var item = models.GetTable<InvoiceAllowance>().Where(i => i.AllowanceID == viewModel.DocID).FirstOrDefault();

            if (item != null)
            {

                models.ExecuteCommand(@"INSERT INTO [proc].DataProcessLog
                                                            (DocID, LogDate, Status, StepID)
                                                            VALUES          ({0},{1},{2},{3})",
                        item.AllowanceID, DateTime.Now, (int)Naming.DataProcessStatus.Done,
                        (int)Naming.InvoiceStepDefinition.PDF待傳輸);

                if (html == true)
                {
                    String[] useThermalPOSArgs;
                    return View(getAllowanceViewPath(item, out useThermalPOSArgs), item);
                }
                else
                {
                    //String outputFile = Path.Combine(Logger.LogPath.GetDateStylePath(item.AllowanceDate.Value), $"{item.AllowanceNumber}.pdf");
                    //if (System.IO.File.Exists(outputFile))
                    //{
                    //    return File(outputFile, "application/pdf");
                    //}
                    //else
                    //{
                    //    String[] useThermalPOSArgs;
                    //    String pdfFile = this.CreateContentAsPDF(getAllowanceViewPath(item, out useThermalPOSArgs), item, Session.Timeout, useThermalPOSArgs);
                    //    if (pdfFile != null)
                    //    {
                    //        System.IO.File.Move(pdfFile, outputFile);
                    //        return File(outputFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
                    //    }
                    //}
                    String[] useThermalPOSArgs;
                    String pdfFile = this.CreateContentAsPDF(getAllowanceViewPath(item, out useThermalPOSArgs), item, Session.Timeout, useThermalPOSArgs);
                    if (pdfFile != null)
                    {
                        Response.Clear();
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.AddHeader("Cache-control", "max-age=1");
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition", $"attachment;filename={DateTime.Today:yyyy-MM-dd}.pdf");

                        using (FileStream fs = System.IO.File.OpenRead(pdfFile))
                        {
                            fs.CopyTo(Response.OutputStream);
                            fs.Close();
                        }
                        System.IO.File.Delete(pdfFile);
                    }
                }
            }

            return new EmptyResult { };

        }

        public ActionResult ShowInvoicePageView(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.DocID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceItem>().Where(a => a.InvoiceID == viewModel.DocID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }

            return View(item);
        }

        public ActionResult ShowInvoice(RenderStyleViewModel viewModel)
        {
            ViewResult result = (ViewResult)ShowInvoicePageView(viewModel);

            InvoiceItem item = result.Model as InvoiceItem;
            if (item == null)
                return result;
            String[] useThermalPOSArgs;
            return View(getInvoiceViewPath(item, out useThermalPOSArgs, viewModel.PaperStyle), item);
        }

        public ActionResult ShowInvoiceContent(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ShowInvoicePageView(viewModel);

            InvoiceItem item = result.Model as InvoiceItem;
            if (item == null)
                return result;

            return View("~/Views/DataView/Module/InvoiceContent.cshtml", item);
        }

        [Authorize]
        public ActionResult PrintA0401()
        {
            var profile = HttpContext.GetUser();

            var items = models.GetTable<DocumentPrintQueue>()
                .Where(i => i.UID == profile.UID)
                .Join(models.GetTable<CDS_Document>()
                        .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice), 
                    i => i.DocID, d => d.DocID, (i, d) => i);

            return View(items);
        }

        [Authorize]
        public ActionResult PrintB0401()
        {
            var profile = HttpContext.GetUser();

            var items = models.GetTable<DocumentPrintQueue>()
                .Where(i => i.UID == profile.UID)
                .Join(models.GetTable<CDS_Document>()
                        .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance),
                    i => i.DocID, d => d.DocID, (i, d) => i);

            return View(items);
        }

        public ActionResult PrintA0401AsPDF()
        {
            ViewResult result = (ViewResult)PrintA0401();
            IQueryable<DocumentPrintQueue> items = result.Model as IQueryable<DocumentPrintQueue>;
            String pdfFile = this.CreateContentAsPDF("~/Views/DataView/PrintA0401.aspx", items, Session.Timeout);

            if (pdfFile != null)
            {
                return File(pdfFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
            }
            else
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
        }

        public ActionResult PrintB0401AsPDF()
        {
            ViewResult result = (ViewResult)PrintB0401();
            IQueryable<DocumentPrintQueue> items = result.Model as IQueryable<DocumentPrintQueue>;
            String pdfFile = this.CreateContentAsPDF("~/Views/DataView/PrintB0401.aspx", items, Session.Timeout);

            if (pdfFile != null)
            {
                return File(pdfFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
            }
            else
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
        }

        [Authorize]
        public ActionResult PrintC0401(RenderStyleViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            var items = models.GetTable<DocumentPrintQueue>()
                .Where(i => i.UID == profile.UID)
                .Join(models.GetTable<CDS_Document>()
                    .Where(d => !d.ProcessType.HasValue
                        || d.ProcessType == (int)Naming.InvoiceProcessType.C0401)
                    .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice),
                    i => i.DocID, d => d.DocID, (i, d) => i);

            if (viewModel.PaperStyle == "A4")
                return View("PrintC0401A4", items);
            else
                return View("~/Views/DataView/PrintC0401POS.cshtml", items);
        }

        [Authorize]
        public ActionResult PrintD0401()
        {
            var profile = HttpContext.GetUser();

            var items = models.GetTable<DocumentPrintQueue>()
                .Where(i => i.UID == profile.UID)
                .Join(models.GetTable<CDS_Document>()
                        .Where(d => !d.ProcessType.HasValue
                            || d.ProcessType == (int)Naming.InvoiceProcessType.D0401)
                        .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance),
                    i => i.DocID, d => d.DocID, (i, d) => i);

            return View("~/Views/DataView/PrintD0401.cshtml", items);
        }

        public static readonly String[] ThermalPOSPaper = new String[] { Settings.Default.ThermalPOS };

        public ActionResult PrintC0401AsPDF(RenderStyleViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            this.TempData["viewModel"] = viewModel;
            ViewResult result = (ViewResult)PrintC0401(viewModel);
            IQueryable<DocumentPrintQueue> items = result.Model as IQueryable<DocumentPrintQueue>;
            String pdfFile = viewModel.PaperStyle == "A4"
                    ? this.CreateContentAsPDF("~/Views/DataView/PrintC0401A4.aspx", items, Session.Timeout)
                    : this.CreateContentAsPDF("~/Views/DataView/PrintC0401POS.cshtml", items, Session.Timeout, ThermalPOSPaper);

            if (pdfFile != null)
            {
                return File(pdfFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
            }
            else
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
        }

        public ActionResult PrintD0401AsPDF()
        {
            ViewResult result = (ViewResult)PrintD0401();
            IQueryable<DocumentPrintQueue> items = result.Model as IQueryable<DocumentPrintQueue>;
            String pdfFile = this.CreateContentAsPDF("~/Views/DataView/PrintD0401.cshtml", items, Session.Timeout, ThermalPOSPaper);

            if (pdfFile != null)
            {
                return File(pdfFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
            }
            else
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
        }

        public ActionResult GetLeftQRCode(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            IQueryable<InvoiceItem> items = models.GetTable<InvoiceItem>();
            if (viewModel.InvoiceID.HasValue)
            {
                items = items.Where(i => i.InvoiceID == viewModel.InvoiceID);
            }
            viewModel.InvoiceNo = viewModel.InvoiceNo.GetEfficientString();
            if (viewModel.InvoiceNo != null && viewModel.InvoiceNo.Length == 10)
            {
                items = items.Where(i => i.TrackCode == viewModel.InvoiceNo.Substring(0, 2) && i.No == viewModel.InvoiceNo.Substring(2));
            }

            var item = items.FirstOrDefault();
            if (item != null)
            {
                bool retry = false;
                String qrContent = null;
                try
                {
                    qrContent = item.GetQRCodeContent();
                    using (Bitmap qrcode = qrContent.CreateQRCode(width: 180, height: 180,qrVersion: 10))
                    {
                        Response.Clear();
                        Response.ContentType = "image/jpeg";
                        qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
                    }
                }
                catch(Exception ex)
                {
                    retry = true;
                    Logger.Error(ex);
                    Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceID},\r\n{qrContent}\r\n{ex}");
                }

                if(retry)
                {
                    try
                    {
                        qrContent = $"{qrContent.Substring(0, 88)}:1:0:1:品項過長，詳列於發票明細:1:1:";
                        using (Bitmap qrcode = qrContent.CreateQRCode(width: 180, height: 180, qrVersion: 10))
                        {
                            Response.Clear();
                            Response.ContentType = "image/jpeg";
                            qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceID},\r\n{qrContent}\r\n{ex}");
                    }
                }
            }

            return new EmptyResult();

        }

        public ActionResult GetRightQRCode(InquireInvoiceViewModel viewModel)
        {

            using (Bitmap qrcode = "**".CreateQRCode(width: 180, height: 180, qrVersion: 10))
            {
                Response.Clear();
                Response.ContentType = "image/jpeg";
                qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
            }

            return new EmptyResult();

        }

        public ActionResult PrintSingleAllowanceAsPDF(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ShowAllowance(viewModel);
            InvoiceAllowance item = result.Model as InvoiceAllowance;
            if (item == null)
            {
                return new EmptyResult { };
            }

            String[] useThermalPOSArgs;
            String pdfFile = this.CreateContentAsPDF(getAllowanceViewPath(item, out useThermalPOSArgs), item, Session.Timeout, useThermalPOSArgs);

            if (pdfFile != null)
            {
                if (viewModel.NameOnly == true)
                {
                    String outputFile = Path.Combine(Logger.LogDailyPath, Path.GetFileName(pdfFile));
                    System.IO.File.Move(pdfFile, outputFile);

                    return Content(outputFile);
                }
                else
                {
                    return File(pdfFile, "application/pdf", $"{DateTime.Today:yyyy-MM-dd}.pdf");
                }
            }

            return new EmptyResult { };

        }

        public ActionResult ConvertDataToAllowance()
        {
            XmlDocument uploadData = new XmlDocument();
            uploadData.Load(Request.InputStream);
            AllowanceRoot allowance = uploadData.TrimAll().ConvertTo<AllowanceRoot>();
            return View(allowance);
        }

        public ActionResult ZipInvoicePackagePDF(RenderStyleViewModel viewModel, String jsonData)
        {
            var items = JsonConvert.DeserializeObject<MailTrackingCsvViewModel[]>(jsonData);
            if (items == null || items.Length == 0)
            {
                ViewBag.CloseWindow = true;
                ViewBag.Message = "請選擇郵寄項目!!";
                return View("~/Views/Shared/JsAlert.cshtml");
            }

            ViewBag.ViewModel = viewModel;
            Response.AppendCookie(new HttpCookie("FileDownloadToken", viewModel.FileDownloadToken));

            String outFile = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".zip");
            using (var zipOut = System.IO.File.Create(outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    int packageIdx = 1;
                    foreach (var g in items)
                    {
                        int idx = 1;
                        foreach (var v in g.InvoiceID)
                        {
                            InvoiceItem item = models.GetTable<InvoiceItem>().Where(i => i.InvoiceID == v).FirstOrDefault();
                            if (item == null)
                                continue;
                            var pdfFile = GetInvoicePDF(item, viewModel);
                            zip.CreateEntryFromFile(pdfFile, $"{packageIdx:000000}-{idx++:000}-{Path.GetFileName(pdfFile)}");

                            foreach (var attach in item.CDS_Document.Attachment)
                            {
                                if (System.IO.File.Exists(attach.StoredPath))
                                {
                                    zip.CreateEntryFromFile(attach.StoredPath, $"{packageIdx:000000}-{idx++:000}-{Path.GetFileName(attach.StoredPath)}");
                                }
                            }
                        }
                        packageIdx++;
                    }
                }
            }

            var result = new FilePathResult(outFile, "application/octet-stream");
            result.FileDownloadName = "發票列印下載.zip";
            return result;
        }



    }
}