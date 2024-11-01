using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using DataAccessLayer.basis;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Helper;
using Model.Resource;
using Model.Security.MembershipManagement;
using Utility;
using Model.InvoiceManagement.InvoiceProcess;
using DataAccessLayer;
using ModelExtension.Helper;
using eIVOGo.Helper.Security.Authorization;

namespace eIVOGo.Controllers
{
    public class InvoiceBusinessController : SampleController<InvoiceItem>
    {
        // GET: InvoiceBusiness
        public ActionResult ApplyPOSDevice(int? id)
        {
            var item = models.GetTable<Organization>().Where(o => o.CompanyID == id).FirstOrDefault();
            if (item == null)
                return Content("營業人資料錯誤!!");

            return View("POSDeviceList", item);
        }

        public ActionResult CommitPOS(int? id, int? deviceID, String POSNo)
        {
            var orgItem = models.GetTable<Organization>().Where(o => o.CompanyID == id).FirstOrDefault();
            if (orgItem == null)
                return View("~/Views/Shared/AlertMessage.cshtml", model: "營業人資料錯誤!!");

            POSNo = POSNo.GetEfficientString();
            if (POSNo == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "POS機編號錯誤!!");
            }

            if (orgItem.POSDevice.Any(p => p.POSNo == POSNo && p.DeviceID != deviceID))
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "已存在相同的POS機編號!!");
            }

            var item = models.GetTable<POSDevice>().Where(p => p.CompanyID == id && p.DeviceID == deviceID).FirstOrDefault();
            if (item == null)
            {
                item = new POSDevice
                {
                    CompanyID = orgItem.CompanyID
                };
                orgItem.POSDevice.Add(item);
            }
            item.POSNo = POSNo;

            models.SubmitChanges();

            return View("~/Views/InvoiceBusiness/POSDevice/DataItem.ascx", item);

        }

        public ActionResult DeletePOS(int? id, int deviceID)
        {
            var item = models.DeleteAny<POSDevice>(d => d.CompanyID == id && d.DeviceID == deviceID);

            if (item == null)
            {
                return Json(new { result = false, message = "POS機編號錯誤!!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EditPOS(int? id, int deviceID)
        {
            var item = models.GetTable<POSDevice>().Where(d => d.CompanyID == id && d.DeviceID == deviceID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "POS機編號錯誤!!");
            }

            return View("~/Views/InvoiceBusiness/POSDevice/EditItem.ascx", item);

        }

        public ActionResult DataItem(int? id, int deviceID)
        {
            var item = models.GetTable<POSDevice>().Where(d => d.CompanyID == id && d.DeviceID == deviceID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "POS機編號錯誤!!");
            }

            return View("~/Views/InvoiceBusiness/POSDevice/DataItem.ascx", item);
        }

        public ActionResult GenerateGUID()
        {
            return Content(Guid.NewGuid().ToString());
        }

        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS, Naming.RoleID.ROLE_SELLER })]
        public ActionResult CreateInvoice(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InvoiceBusiness/CreateInvoice.cshtml");
        }

        public ActionResult UploadData(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InvoiceBusiness/UploadData.cshtml");
        }


        public ActionResult PreviewInvoice(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var seller = models.GetTable<Organization>().Where(o => o.CompanyID == viewModel.SellerID).FirstOrDefault();
            if (seller == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "發票開立人錯誤!!");
            }

            viewModel.SellerName = seller.CompanyName;
            viewModel.SellerReceiptNo = seller.ReceiptNo;

            //using (TrackNoManager mgr = new TrackNoManager(new GenericManager<EIVOEntityDataContext>(models.GetDataContext()), seller.CompanyID))
            //{
            //    if (!mgr.ApplyInvoiceDate(viewModel.InvoiceDate.Value))
            //    {
            //        return View("~/Views/Shared/AlertMessage.cshtml", model: String.Format(MessageResources.AlertNullTrackNoInterval, seller.ReceiptNo));
            //    }

            //    viewModel.TrackCode = mgr.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode;
            //    viewModel.No = String.Format("{0:00000000}", mgr.PeekInvoiceNo());
            //}

            InvoiceViewModelValidator<InvoiceItem> validator = new InvoiceViewModelValidator<InvoiceItem>(this.DataSource, seller);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: exception.Message);
            }

            InvoiceItem newItem = validator.InvoiceItem;
            viewModel.TrackCode = newItem.TrackCode;
            viewModel.No = newItem.No;
            viewModel.BuyerName = newItem.InvoiceBuyer.Name;
            viewModel.BuyerReceiptNo = newItem.InvoiceBuyer.ReceiptNo;

            ViewBag.Seller = seller;
            return View("PrintInvoice");
        }

        public ActionResult InitializeCommittingInvoice(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var seller = models.GetTable<Organization>().Where(o => o.CompanyID == viewModel.SellerID).FirstOrDefault();
            if (seller == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "發票開立人錯誤!!");
            }

            return View(seller);
        }


        public ActionResult CommitInvoice(InvoiceViewModel viewModel)
        {
            ViewResult result = (ViewResult)InitializeCommittingInvoice(viewModel);
            Organization seller = result.Model as Organization;
            if (seller == null)
            {
                return result;
            }

            viewModel.SellerName = seller.CompanyName;
            viewModel.SellerReceiptNo = seller.ReceiptNo;

            InvoiceViewModelValidator<InvoiceItem> validator = new InvoiceViewModelValidator<InvoiceItem>(this.DataSource, seller);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return Json(new { result = false, message = exception.Message });
            }

            try
            {
                InvoiceItem newItem = validator.InvoiceItem;
                newItem.CDS_Document.ProcessType = (int?)viewModel.InvoiceProcessType;

                if (viewModel.ForPreview == true)
                {
                    return View("~/Views/DataView/Module/InvoiceContent.cshtml", newItem);
                }

                models.GetTable<InvoiceItem>().InsertOnSubmit(newItem);
                C0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                if (viewModel.Counterpart == true || !String.IsNullOrEmpty(viewModel.BuyerReceiptNo) || !String.IsNullOrEmpty(viewModel.EMail))
                {
                    C0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
                }
                models.SubmitChanges();

                EIVOPlatformFactory.Notify();

                viewModel.TrackCode = newItem.TrackCode;
                viewModel.No = newItem.No;


                return View("~/Views/InvoiceBusiness/Module/InvoiceCreated.ascx", newItem);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message });
            }


        }

        public ActionResult CommitA0401(InvoiceViewModel viewModel)
        {
            ViewResult result = (ViewResult)InitializeCommittingInvoice(viewModel);
            Organization seller = result.Model as Organization;
            if (seller == null)
            {
                return result;
            }

            viewModel.SellerName = seller.CompanyName;
            viewModel.SellerReceiptNo = seller.ReceiptNo;

            A0401ViewModelValidator<InvoiceItem> validator = new A0401ViewModelValidator<InvoiceItem>(this.DataSource, seller);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return Json(new { result = false, message = exception.Message });
            }

            InvoiceItem newItem = validator.InvoiceItem;
            newItem.CDS_Document.ProcessType = (int?)viewModel.InvoiceProcessType;
            if (viewModel.ForPreview == true)
            {
                return View("~/Views/DataView/Module/InvoiceContent.cshtml", newItem);
            }

            models.GetTable<InvoiceItem>().InsertOnSubmit(newItem);
            A0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            A0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
            models.SubmitChanges();

            EIVOPlatformFactory.Notify();

            viewModel.TrackCode = newItem.TrackCode;
            viewModel.No = newItem.No;

            return View("~/Views/InvoiceBusiness/Module/A0401Created.ascx", newItem);

        }

        public ActionResult CommitA0101(InvoiceViewModel viewModel)
        {
            ViewResult result = (ViewResult)InitializeCommittingInvoice(viewModel);
            Organization seller = result.Model as Organization;
            if (seller == null)
            {
                return result;
            }

            viewModel.SellerName = seller.CompanyName;
            viewModel.SellerReceiptNo = seller.ReceiptNo;

            A0101ViewModelValidator<InvoiceItem> validator = new A0101ViewModelValidator<InvoiceItem>(this.DataSource, seller);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return Json(new { result = false, message = exception.Message });
            }

            InvoiceItem newItem = validator.InvoiceItem;
            newItem.CDS_Document.ProcessType = (int?)viewModel.InvoiceProcessType;
            if (viewModel.ForPreview == true)
            {
                return View("~/Views/DataView/Module/InvoiceContent.cshtml", newItem);
            }

            models.GetTable<InvoiceItem>().InsertOnSubmit(newItem);
            A0101Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.待傳送);
            models.SubmitChanges();

            EIVOPlatformFactory.Notify();

            viewModel.TrackCode = newItem.TrackCode;
            viewModel.No = newItem.No;

            return View("~/Views/InvoiceBusiness/Module/A0401Created.ascx", newItem);

        }

        public ActionResult CommitAllowance(AllowanceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.SellerID = viewModel.DecryptKeyValue();
            }

            AllowanceViewModelValidator<InvoiceItem> validator = new AllowanceViewModelValidator<InvoiceItem>(this.DataSource, null);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return Json(new { result = false, message = exception.Message });
            }

            InvoiceAllowance newItem = validator.Allowance;
            //newItem.CDS_Document.ProcessType = (int)viewModel.ProcessType;
            models.GetTable<InvoiceAllowance>().InsertOnSubmit(newItem);
            if (newItem.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.D0401)
            {
                D0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
                D0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, validator.Seller.StepReadyToAllowanceMIG());
            }
            else
            {
                B0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
                B0401Handler.PushStepQueueOnSubmit(models, newItem.CDS_Document, validator.Seller.StepReadyToAllowanceMIG());
            }
            models.SubmitChanges();

            return View("~/Views/InvoiceBusiness/Module/AllowanceCreated.ascx", newItem);

        }

        public ActionResult EncryptContent(String content, String key)
        {
            com.tradevan.qrutil.QREncrypter qrencrypter = new com.tradevan.qrutil.QREncrypter();
            return Content(qrencrypter.AESEncrypt(content, key));
        }

        public ActionResult GetInvoiceRequestSample(InvoiceRequestViewModel viewModel)
        {

            var items = models.GetTable<InvoiceItem>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("InvoiceRequestSample.xlsx")));

            DataSet ds;
            switch (viewModel.ProcessType)
            {
                case Naming.InvoiceProcessType.A0401_Xlsx_Allocation_ByIssuer:
                case Naming.InvoiceProcessType.C0401_Xlsx_Allocation_ByIssuer:
                    ds = items.GetInvoiceDataForIssuer(models);
                    break;

                case Naming.InvoiceProcessType.C0401_Xlsx_Allocation_ByVAC:
                    ds = items.GetInvoiceDataForVAC(models);
                    break;

                case Naming.InvoiceProcessType.C0401_Xlsx_CBE:
                    ds = items.GetInvoiceDataForCBE(models);
                    break;

                case Naming.InvoiceProcessType.C0401_Xlsx:
                default:
                    ds = items.GetInvoiceData(models);
                    break;

            }

            using (var mgr = new InvoiceDataSetManager(models))
            {
                ds.Tables.Add(mgr.InitializeInvoiceResponseTable());
            }

            using (var xls = ds.ConvertToExcel())
            {
                xls.SaveAs(Response.OutputStream);
            }

            ds.Dispose();

            Response.End();

            return new EmptyResult();
        }

        public ActionResult GetAllowanceRequestSample()
        {

            var items = models.GetTable<InvoiceAllowance>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("AllowanceRequestSample.xlsx")));

            using (DataSet ds = items.GetAllowanceData(models))
            {
                using (var mgr = new AllowanceDataSetManager(models))
                {
                    ds.Tables.Add(mgr.InitializeAllowanceResponseTable());
                }
                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            Response.End();

            return new EmptyResult();
        }

        public ActionResult GetVoidInvoiceRequestSample()
        {

            var items = models.GetTable<InvoiceCancellation>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("VoidInvoiceRequestSample.xlsx")));

            using (DataSet ds = items.GetVoidInvoiceData(models))
            {
                using (var mgr = new VoidInvoiceDataSetManager(models))
                {
                    ds.Tables.Add(mgr.InitializeVoidInvoiceResponseTable());
                }

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            Response.End();

            return new EmptyResult();
        }


        public ActionResult GetVoidAllowanceRequestSample()
        {

            var items = models.GetTable<InvoiceAllowanceCancellation>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("VoidAllowanceRequestSample.xlsx")));

            using (DataSet ds = items.GetVoidAllowanceData(models))
            {
                using (var mgr = new VoidAllowanceDataSetManager(models))
                {
                    ds.Tables.Add(mgr.InitializeVoidAllowanceResponseTable());
                }

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            Response.End();

            return new EmptyResult();
        }

        public ActionResult GetFullAllowanceRequestSample()
        {

            var items = models.GetTable<InvoiceItem>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("FullAllowanceRequestSample.xlsx")));

            using (DataSet ds = items.GetInvoiceDataForFullAllowance(models))
            {
                using (var mgr = new AllowanceDataSetManager(models))
                {
                    ds.Tables.Add(mgr.InitializeAllowanceResponseTable());
                }
                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            Response.End();

            return new EmptyResult();
        }

    }

}