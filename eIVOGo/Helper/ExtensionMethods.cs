using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;
using eIVOGo.Module.Common;
using eIVOGo.Module.UI;
using eIVOGo.Properties;
using eIVOGo.template;
using MessagingToolkit.QRCode.Codec;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace eIVOGo.Helper
{
    public static partial class ExtensionMethods
    {
        public static int[] GetKeyValue(this String keyValue)
        {
            return keyValue.Split(',').Select(s => int.Parse(s)).ToArray();
        }

        public static String[] GetItemSelection(this HttpRequest request)
        {
            return request.Form.GetValues("chkItem");
        }

        public static void SetTransferMessage(this Page page, String message)
        {
            page.Items[base_page.PAGE_ALERT_ITEM_KEY] = message;
        }

        public static PopupModalMessage AttachWaitingMessage(this Button button, String message, bool autoAttach)
        {
            PopupModalMessage modal = button.Page.Items["waitingMsg"] as PopupModalMessage;
            if (modal == null)
            {
                modal = (PopupModalMessage)button.Page.LoadControl("~/Module/UI/PopupModalMessage.ascx");
                modal.ID = "waitingMsg";
                modal.InitializeAsUserControl(button.Page);
                button.Parent.Controls.Add(modal);
                button.Page.Items["waitingMsg"] = modal;
                modal.Message = message;
            }

            if (autoAttach)
            {
                button.Attributes["onclick"] = modal.GetClientTriggerScript();
            }

            return modal;
        }

        public static bool OrganizationValueCheck(this Organization dataItem, Control control)
        {
            if (String.IsNullOrEmpty(dataItem.CompanyName))
            {
                //檢查名稱
                WebMessageBox.AjaxAlert(control, "請輸入公司名稱!!");
                return false;
            }
            if (String.IsNullOrEmpty(dataItem.ReceiptNo))
            {
                //檢查名稱
                WebMessageBox.AjaxAlert(control, "請輸入公司統編!!");
                return false;
            }
            if (String.IsNullOrEmpty(dataItem.Addr))
            {
                //檢查名稱
                WebMessageBox.AjaxAlert(control, "請輸入公司地址!!");
                return false;
            }
            if (String.IsNullOrEmpty(dataItem.Phone))
            {
                //檢查名稱
                WebMessageBox.AjaxAlert(control, "請輸入公司電話!!");
                return false;
            }

            Regex reg = new Regex("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");

            if (String.IsNullOrEmpty(dataItem.ContactEmail) || !reg.IsMatch(dataItem.ContactEmail))
            {
                //檢查email
                WebMessageBox.AjaxAlert(control, "電子信箱尚未輸入或輸入錯誤!!");
                return false;
            }
            return true;
        }

        public static void EnqueueInvoicePrint(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, IEnumerable<int> docID)
        {
            foreach (var id in docID)
            {
                var item = mgr.GetTable<CDS_Document>().Where(i => i.DocID == id).FirstOrDefault();
                if (item != null && item.DocumentPrintQueue == null)
                {
                    item.DocumentPrintQueue = new DocumentPrintQueue
                    {
                        SubmitDate = DateTime.Now,
                        UID = userProfile.UID
                    };
                    mgr.SubmitChanges();
                }
            }
        }

        public static bool EnqueueDocumentPrint(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, IEnumerable<int> docID)
        {
            bool result = false;
            foreach (var id in docID)
            {
                var item = mgr.GetTable<CDS_Document>().Where(i => i.DocID == id).FirstOrDefault();
                if (item != null && item.DocumentPrintQueue == null
                    && (item.InvoiceItem == null || item.IsPrintableInvoice()))
                {
                    item.DocumentPrintQueue = new DocumentPrintQueue
                    {
                        SubmitDate = DateTime.Now,
                        UID = userProfile.UID
                    };
                    result = true;
                }
            }
            if (result)
                mgr.SubmitChanges();
            return result;
        }

        public static bool EnqueueInvoicePrint(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, IEnumerable<int> docID, out String reason)
        {
            bool result = false;
            reason = null;
            foreach (var id in docID)
            {
                var item = mgr.GetTable<CDS_Document>().Where(i => i.DocID == id).FirstOrDefault();
                if (item == null || item.InvoiceItem == null)
                    continue;
                if (item.InvoiceItem.Organization.OrganizationStatus.EntrustToPrint == false && item.DocumentPrintLog.Any() && item.DocumentAuthorization == null)
                {
                    reason = $"發票已列印({item.InvoiceItem.TrackCode}{item.InvoiceItem.No})，請取得授權列印!!";
                    return false;
                }
                if (item.DocumentPrintQueue == null)
                {
                    if (item.IsPrintableInvoice())
                    {
                        item.DocumentPrintQueue = new DocumentPrintQueue
                        {
                            SubmitDate = DateTime.Now,
                            UID = userProfile.UID
                        };
                        result = true;
                    }
                    else
                    {
                        reason = $"個人發票({item.InvoiceItem.TrackCode}{item.InvoiceItem.No})不提供自行列印，如有需要紙本請與管理部承辦人員連絡，謝謝配合。";
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            if (result)
                mgr.SubmitChanges();
            return result;
        }

        public static String CreateContentAsPDF(this HttpServerUtility Server, String relativePath, double timeOutInMinute, String[] args = null)
        {
            String path = Server.MapPath("~/temp");
            path.CheckStoredPath();

            Guid uniqueID = Guid.NewGuid();
            String saveTo = Path.Combine(path, String.Format("{0}.htm", uniqueID));
            String pdfFile = Path.Combine(path, String.Format("{0}.pdf", uniqueID));
            String tempHtml = Path.Combine(Logger.LogDailyPath, String.Format("{0}.htm", uniqueID));

            using (StreamWriter sw = new StreamWriter(tempHtml))
            {
                Server.Execute(relativePath, sw, true);
                sw.Flush();
                sw.Close();
            }
            File.Move(tempHtml, saveTo);

            //convertHtmlToPDF(saveTo, pdfFile, timeOutInMinute);

            saveTo.ConvertHtmlToPDF(pdfFile, timeOutInMinute, args);

            if (File.Exists(pdfFile))
            {
                File.Delete(saveTo);

                bool checking = true;
                while (checking)
                {
                    try
                    {
                        using (var fs = File.OpenRead(pdfFile))
                        {
                            fs.Close();
                            checking = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                return pdfFile;
            }

            return null;
        }

        public static byte[] EncryptString(this String EncryptContent, byte[] key)
        {
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
            aesCSP.Key = key;
            aesCSP.GenerateIV();

            byte[] inBlock = Encoding.Default.GetBytes(EncryptContent);
            ICryptoTransform xfrm = aesCSP.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

            return outBlock;
        }

        public static bool ParseDate(this String dateStr, out DateTime? dateValue)
        {
            dateValue = null;
            if (!String.IsNullOrEmpty(dateStr))
            {
                DateTime dateVal;
                if (DateTime.TryParse(dateStr, out dateVal))
                {
                    dateValue = dateVal;
                    return true;
                }
            }
            return false;
        }

        public static bool IsPrintableInvoice(this CDS_Document item)
        {
            return item.InvoiceItem == null
                ? false
                : item.InvoiceItem.InvoiceBuyer.IsB2C()
                    ? item.InvoiceItem.PrintMark == "Y"
                        || (item.InvoiceItem.PrintMark == "N"
                                && item.InvoiceItem.InvoiceWinningNumber != null
                                && item.InvoiceItem.InvoiceDonation == null
                                && item.InvoiceItem.InvoiceCarrier != null
                                && item.InvoiceItem.InvoiceCarrier.CarrierType == Settings.Default.DefaultUserCarrierType)
                        ? /*item.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice)
                            ? item.DocumentAuthorization != null
                                ? true : false
                            :*/ true
                        : false
                    : /*item.InvoiceItem.Organization.OrganizationStatus.EntrustToPrint == false
                        ? item.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice)
                            ? item.DocumentAuthorization != null ? true : false
                            : true
                        :*/ true;
        }

        public static void SetModelSource<TEntity>(this TempDataDictionary tempData, ModelSource<TEntity> models)
            where TEntity : class, new()
        {
            tempData["modelSource"] = models;
        }

        public static ModelSource<TEntity> GetModelSource<TEntity>(this TempDataDictionary tempData)
            where TEntity : class, new()
        {
            return (ModelSource<TEntity>)tempData["modelSource"];
        }

        public static GenericManager<EIVOEntityDataContext> GetGenericModelSource(this TempDataDictionary tempData)
        {
            return (GenericManager<EIVOEntityDataContext>)tempData["modelSource"];
        }


        public static ModelSource<TEntity> InvokeModelSource<TEntity>(this TempDataDictionary tempData)
            where TEntity : class, new()
        {
            GenericManager<EIVOEntityDataContext> models = tempData.GetGenericModelSource();
            if (models == null)
            {
                models = new ModelSource<TEntity>();
                tempData.SetModelSource<TEntity>((ModelSource<TEntity>)models);
                return (ModelSource<TEntity>)models;
            }
            else if (models is ModelSource<TEntity>)
            {
                return (ModelSource<TEntity>)models;
            }
            else
            {
                return new ModelSource<TEntity>(models);
            }
        }


        public static void DownloadCsv<TEntity>(this ViewUserControl control, Func<IEnumerable<TEntity>, IEnumerable<object>> getCsvResult)
            where TEntity : class, new()
        {
            var models = control.TempData.GetModelSource<TEntity>();
            //control.Response.CsvDownload(getCsvResult(models.Items), null, Encoding.GetEncoding(950), "text/csv");
            control.Response.CsvDownload(getCsvResult(models.Items), null, "text/csv");
        }

        public static String CreateContentAsPDF<T>(this Controller controller, String viewPath, T model, double timeOutInMinute, String[] args = null)
        {
            String path = controller.Server.MapPath("~/temp");
            path.CheckStoredPath();

            Guid uniqueID = Guid.NewGuid();
            String saveTo = Path.Combine(path, String.Format("{0}.htm", uniqueID));
            String pdfFile = Path.Combine(path, String.Format("{0}.pdf", uniqueID));
            String tempHtml = Path.Combine(Logger.LogDailyPath, String.Format("{0}.htm", uniqueID));

            using (StreamWriter sw = new StreamWriter(tempHtml))
            {
                sw.Write(controller.RenderViewToString(viewPath, model));
                sw.Flush();
                sw.Close();
            }
            File.Move(tempHtml, saveTo);

            saveTo.ConvertHtmlToPDF(pdfFile, timeOutInMinute, args);

            if (File.Exists(pdfFile))
            {
                File.Delete(saveTo);

                bool checking = true;
                while (checking)
                {
                    try
                    {
                        using (var fs = File.OpenRead(pdfFile))
                        {
                            fs.Close();
                            checking = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                return pdfFile;
            }

            return null;
        }


        public static string RenderViewToString<T>(this Controller controller, string viewPath, T model)
        {
            controller.ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                IView view;
                if (viewPath.EndsWith("html", StringComparison.OrdinalIgnoreCase))
                {
                    view = new RazorView(controller.ControllerContext, viewPath, null, false, null);
                }
                else
                {
                    view = new WebFormView(controller.ControllerContext, viewPath);
                }
                var dataDict = new ViewDataDictionary<T>(model);
                var tempDict = new TempDataDictionary();
                var viewContext = new ViewContext(controller.ControllerContext, view, dataDict,
                                            controller.TempData /*new TempDataDictionary()*/, writer);
                viewContext.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static bool CheckSystemCompany(this UserProfileMember _userProfile)
        {
            return _userProfile.CurrentUserRole.OrganizationCategory.CategoryID == (int)Naming.CategoryID.COMP_SYS;
        }

        public static bool IsSystemAdmin(this UserProfileMember profile)
        {
            return profile != null && profile.CurrentUserRole.RoleID == (int)Naming.RoleID.ROLE_SYS;
        }

        public static bool IsAuthorized(this UserProfileMember profile, Naming.RoleID[] roleID)
        {
            return profile != null && roleID.Contains((Naming.RoleID)profile.CurrentUserRole.RoleID);
        }

        public static UserProfile CreateDefaultUser(this GenericManager<EIVOEntityDataContext> models, Organization item, OrganizationCategory orgaCate)
        {
            var userProfile = new UserProfile
            {
                PID = item.ReceiptNo,
                Phone = item.Phone,
                EMail = item.ContactEmail,
                Address = item.Addr,
                UserProfileExtension = new UserProfileExtension
                {
                    IDNo = item.ReceiptNo
                },
                UserProfileStatus = new UserProfileStatus
                {
                    CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                }
            };

            models.GetTable<UserRole>().InsertOnSubmit(new UserRole
            {
                RoleID = (int)Naming.RoleID.ROLE_SELLER,
                UserProfile = userProfile,
                OrganizationCategory = orgaCate
            });

            models.SubmitChanges();
            userProfile.SendActivationNotice();

            return userProfile;
        }

        public static void SendActivationNotice(this UserProfile userProfile)
        {
            userProfile.NotifyToActivate();
        }

        public static Bitmap CreateQRCodeOld(String content, QRCodeEncoder.ENCODE_MODE encoding, int scale, int version, QRCodeEncoder.ERROR_CORRECTION errorCorrect)
        {
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            qrCodeEncoder.QRCodeScale = scale;
            qrCodeEncoder.QRCodeVersion = version;
            qrCodeEncoder.QRCodeErrorCorrect = errorCorrect;
            qrCodeEncoder.CharacterSet = "UTF-8";

            return qrCodeEncoder.Encode(content);

        }

        public static String GetLeftQRCodeImageSrc(this InvoiceItem item,int qrVersion = 10)
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
                Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceID},\r\n{qrContent}\r\n{ex}");
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
                    Logger.Warn($"產生發票QR Code失敗 => {item.InvoiceID},\r\n{qrContent}\r\n{ex}");
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

        public static string CreateBase64Content(Bitmap img,float dpi = 600f)
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

            //QRCodeWriter qrCode = new QRCodeWriter();
            //Dictionary<EncodeHintType, object> options = new Dictionary<EncodeHintType, object>();
            //options.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            //options.Add(EncodeHintType.ERROR_CORRECTION, "L");
            //options.Add(EncodeHintType.MARGIN, 0);

            //var result = qrCode.encode(content, BarcodeFormat.QR_CODE, 160, 160);
            //BarcodeWriter writer = new BarcodeWriter();
            //return writer.Write(result);

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

            //QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            //qrCodeEncoder.QRCodeEncodeMode = encoding;
            //qrCodeEncoder.QRCodeScale = scale;
            //qrCodeEncoder.QRCodeVersion = version;
            //qrCodeEncoder.QRCodeErrorCorrect = errorCorrect;
            //qrCodeEncoder.CharacterSet = "UTF-8";

            //return qrCodeEncoder.Encode(content);

        }

        public static Bitmap CreateBarCode(this String content, int width = 320, int height = 80, int margin = 0)
        {
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }

            IBarcodeWriter writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.CODE_39,
                Options = new EncodingOptions()
                {
                    PureBarcode = true,
                    Margin = margin,
                    //Width = width,
                    Height = height,
                }
            };

            Bitmap picCode = writer.Write(content);
            //picCode.SetResolution(600f, 600f);
            return picCode;

            //QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            //qrCodeEncoder.QRCodeEncodeMode = encoding;
            //qrCodeEncoder.QRCodeScale = scale;
            //qrCodeEncoder.QRCodeVersion = version;
            //qrCodeEncoder.QRCodeErrorCorrect = errorCorrect;
            //qrCodeEncoder.CharacterSet = "UTF-8";

            //return qrCodeEncoder.Encode(content);

        }

        //public static String CreateQRCodeImageSrc(this String content, QRCodeEncoder.ENCODE_MODE encoding = QRCodeEncoder.ENCODE_MODE.BYTE, int scale = 4, int version = 8, QRCodeEncoder.ERROR_CORRECTION errorCorrect = QRCodeEncoder.ERROR_CORRECTION.L, float dpi = 600f)
        //{
        //    using (Bitmap img = content.CreateQRCode())
        //    {
        //        using (MemoryStream buffer = new MemoryStream())
        //        {
        //            img.SetResolution(dpi, dpi);
        //            img.Save(buffer, ImageFormat.Png);
        //            StringBuilder sb = new StringBuilder("data:image/png;base64,");
        //            sb.Append(Convert.ToBase64String(buffer.ToArray()));
        //            return sb.ToString();
        //        }
        //    }
        //}

        public static String GetQRCodeContent(this InvoiceItem item)
        {
            var buyer = item.InvoiceBuyer;

            string finalEncryData = BuildEncryptedData(item);

            StringBuilder sb = new StringBuilder();
            sb.Append(item.TrackCode + item.No);
            sb.Append(String.Format("{0:000}{1:00}{2:00}", item.InvoiceDate.Value.Year - 1911, item.InvoiceDate.Value.Month, item.InvoiceDate.Value.Day));
            sb.Append(item.RandomNo);
            sb.Append(String.Format("{0:X8}", (int)item.InvoiceAmountType.SalesAmount.Value));
            sb.Append(String.Format("{0:X8}", (int)item.InvoiceAmountType.TotalAmount.Value));
            sb.Append(buyer.IsB2C() ? "00000000" : buyer.ReceiptNo);
            sb.Append(item.InvoiceSeller != null ? item.InvoiceSeller.ReceiptNo : item.Organization.ReceiptNo);
            sb.Append(finalEncryData);
            sb.Append(":");
            sb.Append("**********");
            sb.Append(":");
            sb.Append(item.InvoiceDetails.Count);
            sb.Append(":");
            sb.Append(item.InvoiceDetails.Count);
            sb.Append(":");
            sb.Append(2);
            sb.Append(":");
            StringBuilder details = new StringBuilder();
            foreach (var p in item.InvoiceDetails)
            {
                //sb.Append(p.InvoiceProduct.Brief);
                //sb.Append(":");
                foreach (var pd in p.InvoiceProduct.InvoiceProductItem)
                {
                    if (!pd.Piece.Value.Equals(0))
                    {
                        details.Append(p.InvoiceProduct.Brief);
                        details.Append(":");
                        details.Append(String.Format("{0:#0}", pd.Piece));
                        details.Append(":");
                        details.Append(String.Format("{0:#0}", pd.UnitCost));
                        details.Append(":");
                    }
                }
            }
            sb.Append(Convert.ToBase64String(Encoding.Default.GetBytes(details.ToString())));

            return sb.ToString();
        }

        public static string BuildEncryptedData(this InvoiceItem item)
        {
            String key = File.ReadAllText(Path.Combine(Logger.LogPath, "ORCodeKey.txt"));
            String EncryptContent = item.TrackCode + item.No + item.RandomNo;
            com.tradevan.qrutil.QREncrypter qrencrypter = new com.tradevan.qrutil.QREncrypter();
            String finalEncryData = qrencrypter.AESEncrypt(EncryptContent, key);
            return finalEncryData;
        }

        public static int InvoiceTypeToFormat(this int type)
        {
            switch (type)
            {
                //case (int)Naming.InvoiceTypeDefinition.三聯式:
                //    return (int)Naming.InvoiceTypeFormat.三聯式;
                //case (int)Naming.InvoiceTypeDefinition.二聯式:
                //    return (int)Naming.InvoiceTypeFormat.二聯式;
                //case (int)Naming.InvoiceTypeDefinition.二聯式收銀機:
                //    return (int)Naming.InvoiceTypeFormat.二聯式收銀機;
                //case (int)Naming.InvoiceTypeDefinition.特種稅額:
                //    return (int)Naming.InvoiceTypeFormat.特種稅額;
                //case (int)Naming.InvoiceTypeDefinition.電子計算機:
                //    return (int)Naming.InvoiceTypeFormat.電子計算機;
                //case (int)Naming.InvoiceTypeDefinition.三聯式收銀機:
                //    return (int)Naming.InvoiceTypeFormat.三聯式收銀機;
                case (int)Naming.InvoiceTypeDefinition.一般稅額計算之電子發票:
                    return (int)Naming.InvoiceTypeFormat.一般稅額計算之電子發票;
                case (int)Naming.InvoiceTypeDefinition.特種稅額計算之電子發票:
                    return (int)Naming.InvoiceTypeFormat.特種稅額;
                default:
                    return (int)Naming.InvoiceTypeFormat.一般稅額計算之電子發票;
            }

        }
    }

    public enum QueryType
    {
        電子發票,
        電子折讓單,
        作廢電子發票,
        作廢電子折讓單,
        中獎發票
    }
}