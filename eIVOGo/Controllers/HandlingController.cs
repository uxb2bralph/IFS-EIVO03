using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Newtonsoft.Json;
using Utility;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class HandlingController : SampleController<InvoiceItem>
    {
        protected UserProfileMember _userProfile;

        public HandlingController() : base()
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Hello,World!!";
            return View();
        }

        // GET: Handling
        public ActionResult DisableCompany(int companyID)
        {
            updateCompanyStatus(companyID, Naming.MemberStatusDefinition.Mark_To_Delete);
            return View("Index");
        }

        private void updateCompanyStatus(int companyID, Naming.MemberStatusDefinition status)
        {
            if (!_userProfile.CheckSystemCompany())
            {
                ViewBag.Message = "使用者非系統管理公司!!";
                return;
            }

            ViewBag.Message = "資料不存在!!";

            using (ModelSource<Organization> models = new ModelSource<Organization>())
            {
                OrganizationStatus item = models.GetTable<Organization>().Where(o => o.CompanyID == companyID)
                    .Select(o => o.OrganizationStatus).FirstOrDefault();
                if (item != null)
                {
                    item.CurrentLevel = (int)status;
                    models.SubmitChanges();
                    ViewBag.Message = "資料已更新!!";
                }
            }
        }

        public ActionResult EnableCompany(int companyID)
        {
            updateCompanyStatus(companyID, Naming.MemberStatusDefinition.Checked);
            return View("Index");
        }

        public ActionResult ApplyRelationship(int companyID)
        {
            var item = models.GetTable<Organization>().Where(o => o.CompanyID == companyID).FirstOrDefault();
            if (item == null)
            {
                ViewBag.Message = "資料不存在!!";
            }
            else
            {
                if (!item.IsEnterpriseGroupMember())
                {
                    models.GetTable<EnterpriseGroupMember>().InsertOnSubmit(
                        new EnterpriseGroupMember
                        {
                            EnterpriseID = (int)Naming.EnterpriseGroup.網際優勢股份有限公司,
                            CompanyID = item.CompanyID
                        });
                    models.SubmitChanges();
                    ViewBag.Message = "設定完成!!";
                }
                else
                {
                    ViewBag.Message = "該開立人已是B2B營業人!!";
                }
            }

            return View("Index");
        }

        public ActionResult MailTracking(MailTrackingViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/Handling/InvoiceMailTracking.cshtml");
        }

        public ActionResult InquireToTrackMail(MailTrackingViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            viewModel.StartNo = viewModel.StartNo.GetEfficientString();
            viewModel.EndNo = viewModel.EndNo.GetEfficientString();
            if (viewModel.StartNo == null || viewModel.StartNo.Length != 10)
            {
                ModelState.AddModelError("StartNo", "發票起號由2碼英文+8碼數字組成，共10碼。");
            }
            else if (viewModel.EndNo == null || viewModel.EndNo.Length != 10)
            {
                ModelState.AddModelError("EndNo", "發票迄號由2碼英文+8碼數字組成，共10碼。");
            }
            else if(viewModel.StartNo.Substring(0, 2) != viewModel.EndNo.Substring(0, 2))
            {
                ModelState.AddModelError("StartNo", "發票起、迄號字軌不相同!!");
                ModelState.AddModelError("EndNo", "發票起、迄號字軌不相同!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            String startNo = viewModel.StartNo.Substring(2, 8);
            String endNo = viewModel.EndNo.Substring(2, 8);
            String trackCode = viewModel.StartNo.Substring(0, 2);

            IQueryable<InvoiceItem> items = models.GetTable<InvoiceItem>();
            IQueryable<InvoiceItem> exclusive = models.GetTable<InvoiceItem>();
            bool hasExclusive = false;
            if (viewModel.DateFrom.HasValue)
            {
                hasExclusive = true;
                exclusive = exclusive.Where(i => i.InvoiceDate >= viewModel.DateFrom);
            }

            if (viewModel.DateTo.HasValue)
            {
                hasExclusive = true;
                exclusive = exclusive.Where(i => i.InvoiceDate < viewModel.DateTo.Value.AddDays(1));
            }

            if(hasExclusive)
            {
                items = items.Where(i => !exclusive.Any(x => x.InvoiceID == i.InvoiceID));
            }

            if (viewModel.ChannelID.HasValue)
            {
                items = items.Join(models.GetTable<CDS_Document>().Where(d => d.ChannelID == (int?)viewModel.ChannelID), 
                    i => i.InvoiceID, d => d.DocID, (i, d) => i);
            }

            if (viewModel.Attachment == 1)
            {
                items = items.Join(models.GetTable<CDS_Document>().Where(d => d.Attachment.Any()),
                    i => i.InvoiceID, d => d.DocID, (i, d) => i);
            }
            else if (viewModel.Attachment == 0)
            {
                items = items.Join(models.GetTable<CDS_Document>().Where(d => !d.Attachment.Any()),
                    i => i.InvoiceID, d => d.DocID, (i, d) => i);
            }

            items = items.Where(i => i.TrackCode == trackCode
                && String.Compare(i.No, startNo) >= 0
                && String.Compare(i.No, endNo) <= 0
                && i.InvoiceCancellation == null);

            //var resultItems = items
            //    .Where(i => i.InvoiceBuyer.Address != null && i.InvoiceBuyer.ReceiptNo == "0000000000")
            //    .OrderBy(i => i.TrackCode).ThenBy(i => i.No)
            //    .Union(items
            //        .Where(i => i.InvoiceBuyer.Address != null && i.InvoiceBuyer.ReceiptNo != "0000000000")
            //        .OrderBy(i => i.TrackCode).ThenBy(i => i.No));

            var resultItems = items.Join(models.GetTable<InvoiceBuyer>().Where(i => i.Address != null),
                    i => i.InvoiceID, b => b.InvoiceID, (i, b) => i);
                //.OrderBy(i => i.InvoiceBuyer.ReceiptNo)
                //.ThenBy(i => i.InvoiceBuyer.Address)
                //.ThenBy(i => i.TrackCode)
                //.ThenBy(i => i.No);

            return View("~/Views/Handling/MailTracking/QueryResult.cshtml", resultItems);
        }

        public ActionResult InvoiceMailItems(bool? showTable, int[] id, int[] packageID)
        {
            List<InvoiceItem> items = new List<InvoiceItem>();
            if (id != null && id.Length > 0)
            {
                for (int idx = 0; idx < id.Length; idx++)
                {
                    var item = models.GetTable<InvoiceItem>().Where(i => i.InvoiceID == id[idx]).FirstOrDefault();
                    if (item != null)
                    {
                        item.PackageID = packageID[idx];
                    }
                    items.Add(item);
                }
            }
            else
            {
                items = models.GetTable<InvoiceItem>().Where(i => false).ToList();
            }

            if (showTable == true)
            {
                return View("~/Views/Handling/MailTracking/TableList.cshtml", items);
            }
            else
            {
                return View("~/Views/Handling/MailTracking/ItemList.cshtml", items);
            }
        }

        public ActionResult PackInvoice(int[] id)
        {
            IQueryable<InvoiceItem> items;
            if (id != null && id.Length > 0)
            {
                items = models.GetTable<InvoiceItem>().Where(i => id.Contains(i.InvoiceID));
            }
            else
            {
                items = models.GetTable<InvoiceItem>().Where(i => false);
            }

            if (items.Count() == 0)
            {
                ViewBag.Message = "發票資料錯誤!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }
            else
            {
                return View("~/Views/Handling/MailTracking/PackedItemList.cshtml", items);
            }
        }

        public ActionResult CommitMailTracking(String jsonData, int? deliveryStatus)
        {
            var items = JsonConvert.DeserializeObject<MailTrackingCsvViewModel[]>(jsonData);
            if (items == null || items.Length == 0)
            {
                ViewBag.Message = "請選擇郵寄項目!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            if (!deliveryStatus.HasValue)
            {
                ViewBag.Message = "請選擇郵寄過程!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            bool result = true;
            var table = models.GetTable<InvoiceDeliveryTracking>();
            foreach (var m in items)
            {
                foreach (var p in m.InvoiceID)
                {
                    if (m.DeliveryDate.HasValue)
                    {
                        var mailNo = m.MailNo1.GetEfficientString();
                        if (mailNo == null)
                            continue;

                        var item = new InvoiceDeliveryTracking
                        {
                            DeliveryDate = m.DeliveryDate.Value,
                            DeliveryStatus = deliveryStatus.Value,
                            InvoiceID = p.Value,
                            TrackingNo1 = mailNo,
                            TrackingNo2 = m.MailNo2
                        };
                        table.InsertOnSubmit(item);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            try
            {
                if (result)
                {
                    models.SubmitChanges();
                    return Json(new { result = true });
                }
                else
                {
                    return Json(new { result = false, message = "資料錯誤!!" });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message });
            }

        }

        public ActionResult DownloadXlsx(String jsonData)
        {
            var items = JsonConvert.DeserializeObject<MailTrackingCsvViewModel[]>(jsonData);
            if (items == null || items.Length == 0)
            {
                ViewBag.CloseWindow = true;
                ViewBag.Message = "請選擇郵寄項目!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "message/rfc822";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("掛號郵件號碼明細.xlsx")));


            using (DataSet ds = new DataSet())
            {
                DataTable table = new DataTable();
                ds.Tables.Add(table);
                table.Columns.Add("掛號號碼");
                table.Columns.Add("姓名");
                table.Columns.Add("寄件地名或地址");
                table.Columns.Add("備考");
                table.Columns.Add("附件檔頁數");
                table.Columns.Add("合計");
                //table.Columns.Add("作業方式");
                table.Columns.Add("遞件日期");
                table.Columns.Add("Google_id");
                table.Columns.Add("發票號碼");
                table.Columns.Add("發票日期");

                foreach (var m in items)
                {
                    var item = models.GetTable<InvoiceItem>().Where(i => i.InvoiceID == m.PackageID).FirstOrDefault();
                    if (item == null)
                        continue;
                    var mailNo = m.MailNo1.GetEfficientString();
                    if (mailNo == null)
                        continue;
                    var row = table.NewRow();
                    table.Rows.Add(row);

                    row["掛號號碼"] = mailNo;
                    row["遞件日期"] = $"{m.DeliveryDate:yyyy/MM/dd}";
                    row["姓名"] = item.InvoiceBuyer.ContactName;
                    row["寄件地名或地址"] = item.InvoiceBuyer.Address;
                    int p1 = m.InvoiceID != null && m.InvoiceID.Length > 1 ? m.InvoiceID.Length : 0;
                    row["備考"] = $"{p1:###}";
                    int p2 = m.InvoiceID != null && m.InvoiceID.Length > 0 ? m.InvoiceID.Sum(i => i.Value.GetAttachedPdfPageCount(models)) : 0;
                    row["附件檔頁數"] = $"{p2:###}";
                    row["合計"] = $"{p1 + p2:###}";
                    //row["作業方式"] = ((Naming.ChannelIDType?)item.CDS_Document.ChannelID).ToString();
                    row["Google_id"] = item.InvoiceBuyer.CustomerID;
                    row["發票號碼"] = $"{item.TrackCode}{item.No}";
                    row["發票日期"] = $"{item.InvoiceDate:yyyy/MM/dd}";
                }

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            return new EmptyResult();
        }

        // GET: Handling/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Handling/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Handling/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Handling/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
