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
using Uxnet.Com.DataAccessLayer;
using ModelExtension.Helper;
using Newtonsoft.Json;

namespace eIVOGo.Controllers.Merchandise
{
    [Authorize]
    public class ProductCatalogController : SampleController<InvoiceItem>
    {
        // GET: ProductCatalog
        public ActionResult QueryIndex(ProductCatalogQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/ProductCatalog/Module/ProductCatalogQuery.cshtml");
        }

        public ActionResult InquireProduct(ProductCatalogQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            if (viewModel.KeyID != null)
            {
                viewModel.ProductID = viewModel.DecryptKeyValue();
            }

            IQueryable<ProductCatalog> items = models.GetDataContext().FilterProductCatalogByRole(profile, models.GetTable<ProductCatalog>());

            if(viewModel.SupplierID.HasValue)
            {
                items = items.Where(p => p.ProductSupplier.Any(s => s.SupplierID == viewModel.SupplierID));
            }
                
            if (viewModel.ProductID.HasValue)
            {
                items = items.Where(p => p.ProductID == viewModel.ProductID);
            }

            viewModel.ProductName = viewModel.ProductName.GetEfficientString();
            if (viewModel.ProductName != null && viewModel.ProductName != "*")
            {
                items = items.Where(p => p.ProductName.Contains(viewModel.ProductName));
            }

            viewModel.Barcode = viewModel.Barcode.GetEfficientString();
            if (viewModel.Barcode != null)
            {
                items = items.Where(p => p.Barcode == viewModel.Barcode);
            }

            viewModel.Spec = viewModel.Spec.GetEfficientString();
            if (viewModel.Spec != null)
            {
                items = items.Where(p => p.Spec.Contains(viewModel.Spec));
            }

            viewModel.RecordCount = items.Count();
            ViewBag.CreateNew = new ProductCatalog { };

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/ProductCatalog/Module/ProductCatalogTable.cshtml", items);
            }
            else
            {
                viewModel.ResultView = "~/Views/ProductCatalog/Module/ProductCatalogTable.cshtml";
                return View("~/Views/Common/Module/QueryResult.cshtml", items);
            }

        }

        public ActionResult ProcessDataItem(ProductCatalogQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireProduct(viewModel);
            result.ViewName = "~/Views/ProductCatalog/Module/ProductCatalogTable.cshtml";
            if (viewModel.DisplayType == Naming.FieldDisplayType.DataItem)
            {
                IQueryable<ProductCatalog> items = (IQueryable<ProductCatalog>)result.Model;
                if (items.Count() == 0)
                {
                    viewModel.DisplayType = Naming.FieldDisplayType.Create;
                }
            }
            ViewBag.DisplayType = viewModel.DisplayType;

            return result;
        }

        public ActionResult CommitItem(ProductCatalogQueryViewModel viewModel)
        {
            UserProfileMember profile = HttpContext.GetUser();
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.ProductID = viewModel.DecryptKeyValue();
            }

            viewModel.ProductName = viewModel.ProductName.GetEfficientString();
            if (viewModel.ProductName == null)
            {
                ModelState.AddModelError("ProductName", "請輸入品名");
            }

            viewModel.Barcode = viewModel.Barcode.GetEfficientString();
            viewModel.Spec = viewModel.Spec.GetEfficientString();
            viewModel.Remark = viewModel.Remark.GetEfficientString();
            viewModel.PieceUnit = viewModel.PieceUnit.GetEfficientString();

            if (!viewModel.SalePrice.HasValue)
            {
                ModelState.AddModelError("SalePrice", "請輸入單價");
            }

            if (!viewModel.SupplierID.HasValue)
            {
                ModelState.AddModelError("SupplierID", "請選擇營業人");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            ProductCatalog item = models.GetDataContext().FilterProductCatalogByRole(profile, models.GetTable<ProductCatalog>())
                .Where(p => p.ProductID == viewModel.ProductID)
                .FirstOrDefault();

            bool isNew = false;
            if (item == null)
            {
                isNew = true;
                item = new ProductCatalog
                {

                };
                item.ProductSupplier.Add(new ProductSupplier
                {
                    SupplierID = viewModel.SupplierID.Value
                });
                models.GetTable<ProductCatalog>().InsertOnSubmit(item);
            }

            item.Barcode = viewModel.Barcode;
            item.ProductName = viewModel.ProductName;
            item.SalePrice = viewModel.SalePrice.Value;
            item.Spec = viewModel.Spec;
            item.Remark = viewModel.Remark;
            item.PieceUnit = viewModel.PieceUnit;

            models.SubmitChanges();

            return Json(new { result = true, item.ProductID, keyID = item.ProductID.EncryptKey(), isNew }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteItem(ProductCatalogQueryViewModel viewModel)
        {
            UserProfileMember profile = HttpContext.GetUser();
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.ProductID = viewModel.DecryptKeyValue();
            }

            ProductCatalog item = models.GetDataContext().FilterProductCatalogByRole(profile, models.GetTable<ProductCatalog>())
                .Where(p => p.ProductID == viewModel.ProductID)
                .FirstOrDefault();

            if (item == null)
            {
                return Json(new { result = false, message = "資料錯誤" }, JsonRequestBehavior.AllowGet);
            }
            models.GetTable<ProductCatalog>().DeleteOnSubmit(item);
            models.SubmitChanges();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult QuickSearch(ProductCatalogQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireProduct(viewModel);
            IQueryable<ProductCatalog> items = (IQueryable<ProductCatalog>)result.Model;

            return Content(JsonConvert.SerializeObject(items.ToArray()), "application/json");

        }


    }
}