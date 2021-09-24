using Model.Helper;
using Model.Schema.EIVO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Utility;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Kiosk.Helper
{
    public static class ExtensionMethods
    {
        public static String GetLeftQRCodeImageSrc(this InvoiceRootInvoice item, int qrVersion = 10)
        {
            bool retry = false;
            String qrContent = null;
            try
            {
                qrContent = item.GetQRCodeContent();
                return qrContent.CreateQRCodeImageSrc(width: 180, height: 180, qrVersion: qrVersion);
            }
            catch (Exception ex)
            {
                retry = true;
                Logger.Error(ex);
                Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceNumber},\r\n{qrContent}\r\n{ex}");
            }

            if (retry)
            {
                try
                {
                    qrContent = $"{qrContent.Substring(0, 88)}:1:1:1:品項過長，詳列於發票明細:1:1:";
                    return qrContent.CreateQRCodeImageSrc(width: 180, height: 180, qrVersion: 10);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceNumber},\r\n{qrContent}\r\n{ex}");
                }
            }

            return null;

        }

        public static String GetBarcode39ImageSrc(this String content, int width = 320, int height = 80, int margin = 0, int? wide = null, int? narrow = null)
        {
            using (Bitmap img = content.GetCode39(false, wide, narrow, height, margin))
            {
                return CreateBase64Content(img, 0);
            }
        }

        public static string CreateBase64Content(Bitmap img, float dpi = 600f)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                if (dpi > 0)
                {
                    img.SetResolution(dpi, dpi);
                }
                img.Save(buffer, ImageFormat.Gif);
                StringBuilder sb = new StringBuilder("data:image/gif;base64,");
                sb.Append(Convert.ToBase64String(buffer.ToArray()));
                return sb.ToString();
            }
        }

        public static String CreateQRCodeImageSrc(this String content, int width = 160, int height = 160, int margin = 0, int qrVersion = 10)
        {
            using (Bitmap img = content.CreateQRCode(width, height, margin, qrVersion))
            {
                return CreateBase64Content(img);
            }
        }

        public static Bitmap CreateQRCode(this String content, int width = 160, int height = 160, int margin = 0, int qrVersion = 10)
        {
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }

            IBarcodeWriter writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions()
                {
                    ErrorCorrection = ErrorCorrectionLevel.L,
                    Margin = margin,
                    Width = width,
                    Height = height,
                    QrVersion = qrVersion,
                    CharacterSet = "UTF-8"   // 少了這一行中文就亂碼了
                }
            };

            Bitmap picCode = writer.Write(content);
            picCode.SetResolution(600f, 600f);
            return picCode;

        }


        public static String GetQRCodeContent(this InvoiceRootInvoice item)
        {
            string finalEncryData = item.CustomerDefined?.ProjectNo;

            StringBuilder sb = new StringBuilder();
            DateTime invoiceDate;
            if (DateTime.TryParseExact(String.Format("{0} {1}", item.InvoiceDate, item.InvoiceTime), "yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out invoiceDate))
            {
                sb.Append(item.InvoiceNumber);
                sb.Append(String.Format("{0:000}{1:00}{2:00}", invoiceDate.Year - 1911, invoiceDate.Month, invoiceDate.Day));
                sb.Append(item.InvoiceNumber);
                sb.Append(String.Format("{0:X8}", item.SalesAmount > 0
                    ? (int)item.SalesAmount
                    : item.ZeroTaxSalesAmount > 0
                        ? (int)item.ZeroTaxSalesAmount
                            : item.FreeTaxSalesAmount > 0
                                ? (int)item.FreeTaxSalesAmount
                                : 0));
                sb.Append(String.Format("{0:X8}", (int)item.TotalAmount));
                sb.Append(item.BuyerId=="0000000000" ? "00000000" : item.BuyerId);
                sb.Append(item.SellerId);
                sb.Append(finalEncryData);
                sb.Append(":");
                sb.Append("**********");
                sb.Append(":");
                sb.Append(item.InvoiceItem.Length);
                sb.Append(":");
                sb.Append(item.InvoiceItem.Length);
                sb.Append(":");
                sb.Append(2);
                sb.Append(":");
                StringBuilder details = new StringBuilder();
                foreach (var p in item.InvoiceItem)
                {
                    if (p.Quantity>0)
                    {
                        details.Append(p.Description);
                        details.Append(":");
                        details.Append(String.Format("{0:#0}", p.Quantity));
                        details.Append(":");
                        details.Append(String.Format("{0:#0}", p.UnitPrice));
                        details.Append(":");
                    }
                }
                sb.Append(Convert.ToBase64String(Encoding.Default.GetBytes(details.ToString())));
            }

            return sb.ToString();
        }

        public static String ReadToEnd(this HttpRequestBase request)
        {
            using(StreamReader reader  = new StreamReader(request.InputStream,request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}