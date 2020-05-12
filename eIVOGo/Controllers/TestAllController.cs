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
using Model.Security.MembershipManagement;
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
            return new EmptyResult();
        }

        public ActionResult NotifyGovPlatform(bool? reset)
        {
            if (reset == true)
            {
                GovPlatformFactory.ResetBusyCount();
            }
            GovPlatformFactory.Notify();
            return Content(DateTime.Now.ToString());
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
            if (!String.IsNullOrEmpty(data))
            {
                using (Bitmap qrcode = data.CreateQRCode(width ?? 160, height ?? 160, margin ?? 0))
                {
                    Response.Clear();
                    Response.ContentType = "image/jpeg";
                    qrcode.Save(Response.OutputStream, ImageFormat.Jpeg);
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


    }
}