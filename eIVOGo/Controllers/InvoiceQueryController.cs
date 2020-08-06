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
using System.Web.Script.Serialization;
using System.IO;
using System.IO.Compression;
using Utility;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using System.Data.Linq;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using ModelExtension.Helper;
using Uxnet.Com.DataAccessLayer;
using res = eIVOGo.Resource.Controllers.InvoiceQuery;

namespace eIVOGo.Controllers
{
    public class InvoiceQueryController : SampleController<InvoiceItem>
    {
        protected UserProfileMember _userProfile;

        protected ModelSourceInquiry<InvoiceItem> createModelInquiry()
        {
            _userProfile = WebPageUtility.UserProfile;

            var inquireConsumption = new InquireInvoiceConsumption { ControllerName = "InquireInvoice", ActionName = "ByConsumption", CurrentController = this };
            //inquireConsumption.Append(new InquireInvoiceConsumptionExtensionToPrint { CurrentController = this });
            return (ModelSourceInquiry<InvoiceItem>)(new InquireEffectiveInvoice { CurrentController = this })
                .Append(new InquireInvoiceByRole(_userProfile) { CurrentController = this })
                .Append(inquireConsumption)
                .Append(new InquireInvoiceSeller { ControllerName = "InquireInvoice", ActionName = "BySeller", /*QueryRequired = true, AlertMessage = "請選擇公司名稱!!",*/ CurrentController = this })
                .Append(new InquireInvoiceBuyer { ControllerName = "InquireInvoice", ActionName = "ByBuyer", CurrentController = this })
                .Append(new InquireInvoiceBuyerByName { ControllerName = "InquireInvoice", ActionName = "ByBuyerName", CurrentController = this })
                .Append(new InquireInvoiceDate { ControllerName = "InquireInvoice", ActionName = "ByInvoiceDate", CurrentController = this })
                .Append(new InquireInvoiceAttachment { ControllerName = "InquireInvoice", ActionName = "ByAttachment", CurrentController = this })
                .Append(new InquireInvoiceNo { CurrentController = this })
                .Append(new InquireInvoiceAgent { ControllerName = "InquireInvoice", ActionName = "ByAgent", CurrentController = this })
                .Append(new InquireWinningInvoice { CurrentController = this });
        }

        //[Amy]
        public ActionResult InvoiceReport(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = false;
            ViewBag.QueryAction = "Inquire";
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            return View(models.Inquiry);
        }


        public ActionResult InvoiceMediaReport(TaxMediaQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (!viewModel.BusinessBorder.HasValue)
            {
                viewModel.BusinessBorder = Naming.B2CCategoryID.店家;
            }
            ViewBag.QueryAction = "InquireInvoiceMedia";

            return View("~/Views/InvoiceQuery/InvoiceMediaReport.cshtml");
        }

        public ActionResult InquireInvoiceMedia(TaxMediaQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.BusinessBorder == Naming.B2CCategoryID.店家)
            {
                viewModel.TaxNo = viewModel.TaxNo.GetEfficientString();
                if (viewModel.TaxNo == null || viewModel.TaxNo.Length != 9)
                {
                    ModelState.AddModelError("TaxNo", res.請輸入9位數稅籍編號__);
                }
            }

            if (!viewModel.Year.HasValue)
            {
                ModelState.AddModelError("Year", res.請選擇年度__);
            }

            if (!viewModel.PeriodNo.HasValue)
            {
                ModelState.AddModelError("PeriodNo", res.請選擇期別__);
            }

            if (!viewModel.SellerID.HasValue)
            {
                ModelState.AddModelError("SellerID", res.請選擇發票開立人__);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/InvoiceQuery/InvoiceMediaReport.cshtml");
            }

            if (viewModel.BusinessBorder == Naming.B2CCategoryID.境外電商)
            {
                return View("~/Views/InvoiceQuery/InvoiceMediaReportCBM.cshtml");
            }

            DateTime startDate = new DateTime(viewModel.Year.Value, viewModel.PeriodNo.Value * 2 - 1, 1);
            DataLoadOptions ops = new DataLoadOptions();
            ops.LoadWith<InvoiceItem>(i => i.InvoiceBuyer);
            ops.LoadWith<InvoiceItem>(i => i.InvoiceCancellation);
            ops.LoadWith<InvoiceItem>(i => i.InvoiceAmountType);
            ops.LoadWith<InvoiceItem>(i => i.InvoiceSeller);
            models.GetDataContext().LoadOptions = ops;

            var items = models.GetTable<InvoiceItem>().Where(i => i.SellerID == viewModel.SellerID
                    && i.InvoiceDate >= startDate
                    && i.InvoiceDate < startDate.AddMonths(2))
                .OrderBy(i => i.InvoiceID);

            var allowance = models.GetTable<InvoiceAllowance>()
                    .Where(a => a.InvoiceAllowanceCancellation == null)
                    .Where(a => a.InvoiceAllowanceSeller.SellerID == viewModel.SellerID)
                    .Where(a => a.AllowanceDate >= startDate && a.AllowanceDate < startDate.AddMonths(2));
            ViewBag.AllowanceItems = allowance;


            if (items.Count() > 0 || allowance.Count() > 0)
            {
                ViewBag.FileName = String.Format("{0:d4}{1:d2}({2}).txt", viewModel.Year, viewModel.PeriodNo, items.First().Organization.ReceiptNo);
                var orgItem = models.GetTable<Organization>().Where(o => o.CompanyID == viewModel.SellerID).First();
                if (orgItem.OrganizationExtension == null)
                    orgItem.OrganizationExtension = new OrganizationExtension { };
                if (orgItem.OrganizationExtension.TaxNo != viewModel.TaxNo)
                {
                    orgItem.OrganizationExtension.TaxNo = viewModel.TaxNo;
                    models.SubmitChanges();
                }
            }
            else
            {
                ViewBag.Message = res.資料不存在__;
                return View("InvoiceMediaReport");
            }

            return View(items);
        }


        public ActionResult Inquire(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.PrintAction = "PrintResult";
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            return View("InquiryResult", models.Inquiry);
        }

        public ActionResult InvoiceQueryIndex(InquireInvoiceViewModel viewModel)
        {
            return View("~/Views/InvoiceQuery/InvoiceQueryIndex.cshtml"); 
        }
        public ActionResult InvoiceQuery_Inquire(InquireInvoiceViewModel viewModel, int? pageIndex)
        {
            ViewBag.ViewModel = viewModel;
            ModelSource<InvoiceItem> tmpModels = new ModelSource<InvoiceItem>(models);
            tmpModels.Items = tmpModels.Items;
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            models.InquiryHasError = tmpModels.InquiryHasError;
            if (pageIndex.HasValue)
            {
                ViewBag.PageIndex = pageIndex - 1;
                return View("~/Views/InvoiceQuery/Module/InvoiceQueryItemList.cshtml", tmpModels.Items);
            }
            else
            {
                ViewBag.PageIndex = 0;
                return View("~/Views/InvoiceQuery/Module/InvoiceQueryItemResult.cshtml", tmpModels.Items);
            }

        }

        public ActionResult InvoiceQuery_DownloadCSV(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ModelSource<InvoiceItem> tmpModels = new ModelSource<InvoiceItem>(models);
            tmpModels.Items = tmpModels.Items;
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            Response.ContentEncoding = Encoding.GetEncoding(950);
            //TODO:要做多國語系
            return View(tmpModels.Items);
        }
        public ActionResult InvoiceQuery_CreateXlsx(InquireInvoiceViewModel viewModel)
        {
            //TODO:要做多國語系
            ViewBag.ViewModel = viewModel;
            ModelSource<InvoiceItem> tmpModels = new ModelSource<InvoiceItem>(models);
            tmpModels.Items = tmpModels.Items;
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            _userProfile["modelSource"] = tmpModels;

            var items = tmpModels.Items.OrderBy(i => i.InvoiceID)
            .Select(i => new
            {
                發票號碼 = i.TrackCode + i.No,
                發票日期 = i.InvoiceDate,
                附件檔名 = i.CDS_Document.Attachment.Count > 0 ? i.CDS_Document.Attachment.First().KeyName : null,
                客戶ID = i.InvoiceBuyer.CustomerID,
                序號 = i.InvoicePurchaseOrder != null ? i.InvoicePurchaseOrder.OrderNo : null,
                發票開立人 = i.InvoiceSeller.CustomerName,
                開立人統編 = i.InvoiceSeller.ReceiptNo,
                未稅金額 = i.InvoiceAmountType.SalesAmount,
                稅額 = i.InvoiceAmountType.TaxAmount,
                含稅金額 = i.InvoiceAmountType.TotalAmount,
                買受人名稱 = i.InvoiceBuyer.CustomerName,
                買受人統編 = i.InvoiceBuyer.ReceiptNo,
                連絡人名稱 = i.InvoiceBuyer.ContactName,
                連絡人地址 = i.InvoiceBuyer.Address,
                買受人EMail = i.InvoiceBuyer.EMail,
                愛心碼 = i.InvoiceDonation.AgencyCode,
                是否中獎 = i.InvoiceWinningNumber.UniformInvoiceWinningNumber.PrizeType,
                載具類別 = i.InvoiceCarrier.CarrierType,
                載具號碼 = i.InvoiceCarrier.CarrierNo,
                //備註 = String.Join("", i.InvoiceDetails.Select(t => t.InvoiceProduct.InvoiceProductItem.FirstOrDefault())
                //    .Select(p => p.Remark))
            });

            var details = items
                .Select(item => new
                {
                    發票號碼 = item.發票號碼,
                    發票日期 = item.發票日期,
                    附件檔名 = item.附件檔名,
                    客戶ID = item.客戶ID,
                    序號 = item.序號,
                    發票開立人 = item.發票開立人,
                    開立人統編 = item.開立人統編,
                    未稅金額 = item.未稅金額,
                    稅額 = item.稅額,
                    含稅金額 = item.含稅金額,
                    買受人名稱 = item.買受人名稱,
                    買受人統編 = item.買受人統編,
                    連絡人名稱 = item.連絡人名稱,
                    連絡人地址 = item.連絡人地址,
                    買受人EMail = item.買受人EMail,
                    愛心碼 = item.愛心碼,
                    是否中獎 = item.是否中獎,
                    載具類別 = item.載具類別,
                    載具號碼 = item.載具號碼,
                });
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "message/rfc822";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("發票資料明細.xlsx")));

            using (DataSet ds = new DataSet())
            {
                DataTable table = details.ToDataTable();
                table.TableName = "發票資料明細";
                ds.Tables.Add(table);
                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }
            return new EmptyResult();

        }
        public ActionResult InvoiceQuery_CreateMonthlyReportXlsx(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!viewModel.SellerID.HasValue)
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: res.請選擇開立人__);
            }

            ModelSource<InvoiceItem> tmpModels = new ModelSource<InvoiceItem>(models);
            tmpModels.Items = tmpModels.Items;
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            var items = tmpModels.Items;

            if (items.Count() <= 0)
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: res.查無資料__);
            }

            using (DataSet ds = new DataSet())
            {
                foreach (var yy in items.GroupBy(i => i.InvoiceDate.Value.Year))
                {
                    foreach (var mm in yy.GroupBy(i => i.InvoiceDate.Value.Month))
                    {
                        DataTable table = new DataTable();
                        table.Columns.Add(new DataColumn(res.日期, typeof(String)));
                        table.Columns.Add(new DataColumn(res.未作廢總筆數, typeof(int)));
                        table.Columns.Add(new DataColumn(res.未作廢總金額, typeof(decimal)));
                        table.Columns.Add(new DataColumn(res.已作廢總筆數, typeof(int)));
                        table.Columns.Add(new DataColumn(res.已作廢總金額, typeof(decimal)));
                        table.TableName = yy.Key + "-" + mm.Key;

                        ds.Tables.Add(table);

                        IEnumerable<InvoiceItem> v0, v1;
                        DataRow r;

                        foreach (var item in mm.GroupBy(i => i.InvoiceDate.Value.Day).OrderBy(g => g.Key))
                        {
                            r = table.NewRow();
                            r[0] = item.Key.ToString();
                            v0 = item.Where(i => i.InvoiceCancellation == null);
                            v1 = item.Where(i => i.InvoiceCancellation != null);
                            r[1] = v0.Count();
                            r[2] = v0.Sum(i => i.InvoiceAmountType.TotalAmount);
                            r[3] = v1.Count();
                            r[4] = v1.Sum(i => i.InvoiceAmountType.TotalAmount);
                            table.Rows.Add(r);
                        }

                        v0 = mm.Where(i => i.InvoiceCancellation == null);
                        v1 = mm.Where(i => i.InvoiceCancellation != null);
                        r = table.NewRow();
                        r[0] = res.總計;
                        r[1] = v0.Count();
                        r[2] = v0.Sum(i => i.InvoiceAmountType.TotalAmount);
                        r[3] = v1.Count();
                        r[4] = v1.Sum(i => i.InvoiceAmountType.TotalAmount);
                        table.Rows.Add(r);

                    }
                }

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-control", "max-age=1");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename=({0}){1}.xlsx", items.First().Organization.ReceiptNo, HttpUtility.UrlEncode(res.開立發票月報表)));

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }

            }

            return new EmptyResult();
        }

        public ActionResult InvoiceSummary(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = false;
            ViewBag.QueryAction = "InquireSummary";
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            return View("~/Views/InvoiceQuery/InvoiceReport.cshtml", models.Inquiry);
        }
        public ActionResult InquireSummary(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.PrintAction = "PrintInvoiceSummary";
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            return View("~/Views/InvoiceQuery/InvoiceSummaryResult.cshtml", models.Inquiry);
        }

        public ActionResult InvoiceAttachment(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = false;
            ViewBag.QueryAction = "InquireAttachment";
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            return View("~/Views/InvoiceQuery/InvoiceAttachment.cshtml", models.Inquiry);
        }

        public ActionResult InquireAttachment(InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            return View("~/Views/InvoiceQuery/AttachmentResult.cshtml", models.Inquiry);
        }

        public ActionResult AttachmentGridPage(int index, int size, InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            if (index > 0)
                index--;
            else
                index = 0;

            return View(models.Items.OrderByDescending(d => d.InvoiceID)
                .Skip(index * size).Take(size));
        }


        public ActionResult GridPage(int index, int size, InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            if (index > 0)
                index--;
            else
                index = 0;

            return View(models.Items.OrderByDescending(d => d.InvoiceID)
                .Skip(index * size).Take(size));
        }

        public ActionResult DownloadCSV(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            Response.ContentEncoding = Encoding.GetEncoding(950);

            return View(models.Items);
        }

        public ActionResult CreateXlsx(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ModelSource<InvoiceItem> tmpModels = new ModelSource<InvoiceItem>(models);
            tmpModels.Items = tmpModels.Items;
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            _userProfile["modelSource"] = tmpModels;

            var items = tmpModels.Items.OrderBy(i => i.InvoiceID)
            .Select(i => new
            {
                發票號碼 = i.TrackCode + i.No,
                發票日期 = i.InvoiceDate,
                附件檔名 = i.CDS_Document.Attachment.Count > 0 ? i.CDS_Document.Attachment.First().KeyName : null,
                客戶ID = i.InvoiceBuyer.CustomerID,
                序號 = i.InvoicePurchaseOrder != null ? i.InvoicePurchaseOrder.OrderNo : null,
                發票開立人 = i.InvoiceSeller.CustomerName,
                開立人統編 = i.InvoiceSeller.ReceiptNo,
                未稅金額 = i.InvoiceAmountType.SalesAmount,
                稅額 = i.InvoiceAmountType.TaxAmount,
                含稅金額 = i.InvoiceAmountType.TotalAmount,
                買受人名稱 = i.InvoiceBuyer.CustomerName,
                買受人統編 = i.InvoiceBuyer.ReceiptNo,
                連絡人名稱 = i.InvoiceBuyer.ContactName,
                連絡人地址 = i.InvoiceBuyer.Address,
                買受人EMail = i.InvoiceBuyer.EMail,
                愛心碼 = i.InvoiceDonation.AgencyCode,
                是否中獎 = i.InvoiceWinningNumber.UniformInvoiceWinningNumber.PrizeType,
                載具類別 = i.InvoiceCarrier.CarrierType,
                載具號碼 = i.InvoiceCarrier.CarrierNo,
                //備註 = String.Join("", i.InvoiceDetails.Select(t => t.InvoiceProduct.InvoiceProductItem.FirstOrDefault())
                //    .Select(p => p.Remark))
            });

            var details = items
                .Select(item => new
                {
                    發票號碼 = item.發票號碼,
                    發票日期 = item.發票日期,
                    附件檔名 = item.附件檔名,
                    客戶ID = item.客戶ID,
                    序號 = item.序號,
                    發票開立人 = item.發票開立人,
                    開立人統編 = item.開立人統編,
                    未稅金額 = item.未稅金額,
                    稅額 = item.稅額,
                    含稅金額 = item.含稅金額,
                    買受人名稱 = item.買受人名稱,
                    買受人統編 = item.買受人統編,
                    連絡人名稱 = item.連絡人名稱,
                    連絡人地址 = item.連絡人地址,
                    買受人EMail = item.買受人EMail,
                    愛心碼 = item.愛心碼,
                    是否中獎 = item.是否中獎,
                    載具類別 = item.載具類別,
                    載具號碼 = item.載具號碼,
                });
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "message/rfc822";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("發票資料明細.xlsx")));

            using (DataSet ds = new DataSet())
            {
                DataTable table = details.ToDataTable();
                table.TableName = "發票資料明細";
                ds.Tables.Add(table);
                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }
            return new EmptyResult();
        }

        public ActionResult AssignDownload(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            String resultFile = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".xlsx");
            _userProfile["assignDownload"] = resultFile;

            ThreadPool.QueueUserWorkItem(stateInfo =>
            {
                try
                {
                    SqlCommand sqlCmd = (SqlCommand)models.GetCommand(models.Items);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            using (XLWorkbook xls = new XLWorkbook())
                            {
                                xls.Worksheets.Add(ds);
                                xls.SaveAs(resultFile);
                            }
                        }
                    }
                    models.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });

            return Content(res.下載資料請求已送出__);
        }



        public ActionResult PrintResult(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            ((ModelSource<InvoiceItem>)models).ResultModel = Naming.DataResultMode.Print;

            return View(models.Inquiry);
        }

        public ActionResult DownloadAttachment()
        {
            String jsonData = Request["data"];
            if (!String.IsNullOrEmpty(jsonData))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                int[] invoiceID = serializer.Deserialize<int[]>(jsonData);
                using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
                {
                    return zipAttachment(models.EntityList.Where(i => invoiceID.Contains(i.InvoiceID)));
                }
            }
            return Content(jsonData);
        }

        public ActionResult DownloadAll(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            return zipAttachment(models.Items);
        }

        private ActionResult zipAttachment(IEnumerable<InvoiceItem> items)
        {
            String temp = Server.MapPath("~/temp");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            String outFile = Path.Combine(temp, Guid.NewGuid().ToString() + ".zip");
            using (var zipOut = System.IO.File.Create(outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    foreach (var item in items)
                    {
                        if (item.CDS_Document.Attachment.Count > 0)
                        {
                            for (int i = 0; i < item.CDS_Document.Attachment.Count; i++)
                            {
                                var attach = item.CDS_Document.Attachment[i];
                                if (System.IO.File.Exists(attach.StoredPath))
                                {
                                    ZipArchiveEntry entry = zip.CreateEntry(i == 0 ? item.TrackCode + item.No + ".pdf" : item.TrackCode + item.No + "-" + i + ".pdf");
                                    using (Stream outStream = entry.Open())
                                    {
                                        using (var inStream = System.IO.File.Open(attach.StoredPath, FileMode.Open))
                                        {
                                            inStream.CopyTo(outStream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var result = new FilePathResult(outFile, "message/rfc822");
            result.FileDownloadName = res.發票附件+".zip";
            return result;

        }

        public ActionResult CreateMonthlyReportXlsx(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!viewModel.SellerID.HasValue)
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: res.請選擇開立人__);
            }

            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            var items = models.Items;

            if (items.Count() <= 0)
            {
                ViewBag.CloseWindow = true;
                return View("~/Views/Shared/JsAlert.cshtml", model: res.查無資料__);
            }

            using (DataSet ds = new DataSet())
            {
                foreach (var yy in items.GroupBy(i => i.InvoiceDate.Value.Year))
                {
                    foreach (var mm in yy.GroupBy(i => i.InvoiceDate.Value.Month))
                    {
                        DataTable table = new DataTable();
                        table.Columns.Add(new DataColumn(res.日期, typeof(String)));
                        table.Columns.Add(new DataColumn(res.未作廢總筆數, typeof(int)));
                        table.Columns.Add(new DataColumn(res.未作廢總金額, typeof(decimal)));
                        table.Columns.Add(new DataColumn(res.已作廢總筆數, typeof(int)));
                        table.Columns.Add(new DataColumn(res.已作廢總金額, typeof(decimal)));
                        table.TableName = yy.Key + "-" + mm.Key;

                        ds.Tables.Add(table);

                        IEnumerable<InvoiceItem> v0, v1;
                        DataRow r;

                        foreach (var item in mm.GroupBy(i => i.InvoiceDate.Value.Day).OrderBy(g => g.Key))
                        {
                            r = table.NewRow();
                            r[0] = item.Key.ToString();
                            v0 = item.Where(i => i.InvoiceCancellation == null);
                            v1 = item.Where(i => i.InvoiceCancellation != null);
                            r[1] = v0.Count();
                            r[2] = v0.Sum(i => i.InvoiceAmountType.TotalAmount);
                            r[3] = v1.Count();
                            r[4] = v1.Sum(i => i.InvoiceAmountType.TotalAmount);
                            table.Rows.Add(r);
                        }

                        v0 = mm.Where(i => i.InvoiceCancellation == null);
                        v1 = mm.Where(i => i.InvoiceCancellation != null);
                        r = table.NewRow();
                        r[0] = "總計";
                        r[1] = v0.Count();
                        r[2] = v0.Sum(i => i.InvoiceAmountType.TotalAmount);
                        r[3] = v1.Count();
                        r[4] = v1.Sum(i => i.InvoiceAmountType.TotalAmount);
                        table.Rows.Add(r);

                    }
                }

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-control", "max-age=1");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename=({0}){1}.xlsx", items.First().Organization.ReceiptNo, HttpUtility.UrlEncode(res.開立發票月報表)));

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }

            }

            return new EmptyResult();
        }

  
        public ActionResult InvoiceSummaryGridPage(int index, int size, InquireInvoiceViewModel viewModel)
        {
            //ViewBag.HasQuery = true;
            ViewBag.ViewModel = viewModel;
            models.Inquiry = createModelInquiry();
            models.BuildQuery();

            if (index > 0)
                index--;
            else
                index = 0;

            ViewBag.PageIndex = index;
            ViewBag.PageSize = size;

            return View(models.Items);
        }

        public ActionResult CreateInvoiceSummaryXlsx(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            var items = models.Items.GroupBy(i => i.SellerID)
               .Join(models.GetTable<Organization>(), i => i.Key, o => o.CompanyID, (i, o) => new
               {
                   Seller = o,
                   Items = i
               });
            var details = items
                .Select(item => new
                {
                    開立發票營業人 = item.Seller.CompanyName,
                    統編 = item.Seller.ReceiptNo,
                    上線日期 = item.Seller.InvoiceItems.OrderBy(i => i.InvoiceDate).First().InvoiceDate,
                    發票筆數 = item.Items.Count(),
                });

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("發票資料統計.xlsx")));

            using (DataSet ds = new DataSet())
            {
                DataTable table = details.ToDataTable();
                table.TableName = "發票資料統計";
                ds.Tables.Add(table);

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }
            return new EmptyResult();
        }

        public ActionResult PrintInvoiceSummary(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            models.Inquiry = createModelInquiry();
            models.BuildQuery();
            ((ModelSource<InvoiceItem>)models).ResultModel = Naming.DataResultMode.Print;
            return View(models.Inquiry);
        }



        // POST: InvoiceQuery/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: InvoiceQuery/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: InvoiceQuery/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: InvoiceQuery/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: InvoiceQuery/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
