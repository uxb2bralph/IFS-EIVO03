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
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Models.ViewModel;
using Model.Security.MembershipManagement;
using Newtonsoft.Json.Linq;
using QRCoder;
using Utility;

namespace eIVOGo.Controllers
{
    public class TestAllController : SampleController<InvoiceItem>
    {
        // GET: TestAll
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            Logger.Info("test...");
            Services.DailyJobs.Notify();
            return new EmptyResult();
        }

        public ActionResult NotifyEIVOPlatform(bool? reset)
        {
            if (reset == true)
            {
                EIVOPlatformFactory.ResetBusyCount();
            }
            EIVOPlatformFactory.Notify();
            return Json(EIVOPlatformFactory.CurrentStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MatchAttachment()
        {
            GoogleInvoiceExtensionMethods.MatchGoogleInvoiceAttachment();
            return Content(DateTime.Now.ToString());
        }

        public ActionResult QRCode(String data, int? width, int? height, int? margin)
        {
            //if (!String.IsNullOrEmpty(data))
            //{
            //    using (Bitmap qrcode = data.CreateQRCode(width ?? 160, height ?? 160, margin ?? 0))
            //    {
            //        Response.Clear();
            //        Response.ContentType = "image/jpeg";
            //        qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
            //    }
            //}

            if(!String.IsNullOrEmpty(data))
            {
                using(QRCodeData codeData = QRCodeGenerator.GenerateQrCode(data,QRCodeGenerator.ECCLevel.L))
                {
                    using (QRCode qrCode = new QRCode(codeData))
                    {
                        using (Bitmap qrImg = qrCode.GetGraphic(width ?? 300))
                        {
                            Response.Clear();
                            Response.ContentType = "image/jpeg";
                            qrImg.Save(Response.OutputStream, ImageFormat.Jpeg);
                        }
                    }
                }

            }
            

            return new EmptyResult();

        }

        public ActionResult BarCode(String data, int? width, int? height, int? margin, int? wide, int? narrow, String format)
        {
            if (!String.IsNullOrEmpty(data))
            {
                using (Bitmap qrcode = data.GetCode39(false,wide,narrow,height,margin) /* data.CreateBarCode(width ?? 160, height ?? 160, margin ?? 0)*/)
                {
                    Response.Clear();
                    format = format.GetEfficientString();
                    format = format == null ? "bmp" : format.ToLower();
                    switch (format)
                    {
                        case "gif":
                            Response.ContentType = "image/gif";
                            qrcode.Save(Response.OutputStream, ImageFormat.Gif);
                            break;
                        case "tif":
                            Response.ContentType = "image/tiff";
                            qrcode.Save(Response.OutputStream, ImageFormat.Tiff);
                            break;
                        case "jpg":
                            Response.ContentType = "image/jpg";
                            qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
                            break;
                        case "png":
                            Response.ContentType = "image/png";
                            qrcode.Save(Response.OutputStream, ImageFormat.Png);
                            break;
                        case "bmp":
                            Response.ContentType = "image/bmp";
                            qrcode.Save(Response.OutputStream, ImageFormat.Bmp);
                            break;
                        default:
                            Response.ContentType = "application/octet-stream";
                            qrcode.Save(Response.OutputStream, ImageFormat.Bmp);
                            break;
                    }
                }
            }
            return new EmptyResult();

        }

        public ActionResult CheckInvoiceNo(POSDeviceViewModel viewModel)
        {
            //if (String.IsNullOrEmpty(Request.ContentType) && String.IsNullOrEmpty(Request.Params["Query_String"]))
            //{
            //    using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            //    {
            //        viewModel = JsonConvert.DeserializeObject<POSDeviceViewModel>(reader.ReadToEnd());
            //    }
            //}

            /**
                Http Header
                Seed: RANDOM[16]
                Authorization: Base64(ToHexString(SHA256([Vendor 統編] + [Activation Key] + [Seed])))

                {
	                "SellerID": "[商家統編]",
	                "Booklet": 1
                }
             */

            Request.SaveAs(Path.Combine(Logger.LogDailyPath, String.Format("{0}.txt", DateTime.Now.Ticks)), true);

            if (viewModel.Booklet.HasValue)
            {
                viewModel.quantity = viewModel.Booklet * 50;
            }

            viewModel.Seed = Request.Headers["Seed"].GetEfficientString();
            viewModel.Authorization = Request.Headers["Authorization"].GetEfficientString();

            String reason;
            var result = models.CheckAvailableInterval(viewModel,out reason);

            return Json(new
            {
                result = result,
                message = reason,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Echo(POSDeviceViewModel viewModel)
        {
            return Json(viewModel);
        }

        public ActionResult EditItem(DataTableQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            JObject json = JObject.Parse(RequestBody);
            Table<Organization> org = models.GetTable<Organization>();
            IQueryable<Organization> items = org;
            Expression<Func<Organization, String>> order = o => o.CompanyName;

            //string propertyName = "CompanyName"; // 要排序的屬性名稱
            //Type sourceType = typeof(Organization); // 源類型
            //Type keyType = typeof(string); // 排序鍵類型
            //var propertyExpression = CreateExpression(sourceType, keyType, propertyName);

            //items = items.OrderBy(propertyExpression);
            items = items.OrderBy(order);
            items = items.Skip(1000).Take(50);
            var sqlCmd = items.ToString();

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public static Expression<Func<TSource, TKey>> CreateExpression<TSource, TKey>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(parameter, typeof(TSource).GetProperty(propertyName));
            var lambda = Expression.Lambda<Func<TSource, TKey>>(property, parameter);
            return lambda;
        }

        public static LambdaExpression CreateExpression(Type sourceType, Type keyType, string propertyName)
        {
            var parameter = Expression.Parameter(sourceType, "x");
            var property = Expression.Property(parameter, sourceType.GetProperty(propertyName));
            var keySelector = Expression.Lambda(property, parameter);

            var resultType = typeof(Func<,>).MakeGenericType(sourceType, keyType);
            var lambda = Expression.Lambda(resultType, keySelector.Body, keySelector.Parameters);

            return lambda;
        }

        public ActionResult Dump()
        {
            Request.SaveAs(Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}.txt"), true);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

    }
}