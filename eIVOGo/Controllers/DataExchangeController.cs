using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Helper;
using eIVOGo.Models.ViewModel;
using ClosedXML.Excel;
using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using Model.Security.MembershipManagement;
using ModelExtension.DataExchange;
using Utility;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class DataExchangeController : SampleController<InvoiceItem>
    {
        // GET: DataExchange
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateBuyer()
        {
            var profile = HttpContext.GetUser();

            try
            {
                var xlFile = Request.Files["InvoiceBuyer"];
                if (xlFile != null)
                {
                    using(XLWorkbook xlwb = new XLWorkbook(xlFile.InputStream))
                    {
                        InvoiceBuyerExchange exchange = new InvoiceBuyerExchange();
                        switch ((Naming.RoleID)profile.CurrentUserRole.RoleID)
                        {
                            case Naming.RoleID.ROLE_SYS:
                                exchange.ExchangeData(xlwb);
                                break;
                            case Naming.RoleID.ROLE_SELLER:
                            case Naming.RoleID.ROLE_NETWORKSELLER:
                                exchange.ExchangeData(xlwb, item =>
                                {
                                    return item.SellerID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                                        || item.CDS_Document.DocumentOwner.OwnerID == profile.CurrentUserRole.OrganizationCategory.CompanyID;
                                });
                                break;
                            default:
                                break;
                        }

                        String result = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".xslx");
                        xlwb.SaveAs(result);
                        return File(result, "message/rfc822", "修改買受人資料(回應).xlsx");
                    }
                }
                ViewBag.AlertMessage = "檔案錯誤!!";
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                ViewBag.AlertMessage = ex.ToString();
            }
            return View("Index");
        }

        public ActionResult UpdateTrackCode()
        {
            var profile = HttpContext.GetUser();

            try
            {
                var xlFile = Request.Files["TrackCode"];
                if (xlFile != null)
                {
                    using (XLWorkbook xlwb = new XLWorkbook(xlFile.InputStream))
                    {
                        TrackCodeExchange exchange = new TrackCodeExchange();
                        switch ((Naming.RoleID)profile.CurrentUserRole.RoleID)
                        {
                            case Naming.RoleID.ROLE_SYS:
                                exchange.ExchangeData(xlwb);
                                break;
                            default:
                                break;
                        }

                        String result = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".xslx");
                        xlwb.SaveAs(result);
                        return File(result, "message/rfc822", "修改發票字軌(回應).xlsx");
                    }
                }
                ViewBag.AlertMessage = "檔案錯誤!!";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ViewBag.AlertMessage = ex.ToString();
            }
            return View("Index");
        }

        public ActionResult GetResource(String path)
        {
            if (path != null)
            {
                String filePath = path.DecryptData();
                if (System.IO.File.Exists(filePath))
                {
                    return File(filePath, "application/octet-stream", Path.GetFileName(filePath));
                }
            }
            return new EmptyResult { };
        }

        public async Task<ActionResult> ImportInvoice()
        {
            var userProfile = HttpContext.GetUser();

            if (Request.Files.Count <= 0)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "請選擇匯入檔!!");
            }

            var file = Request.Files[0];
            String fileName = Path.Combine(Logger.LogDailyPath, DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName));
            ViewBag.ImportFile = fileName;
            String content;
            using (StreamReader reader = new StreamReader(Request.Files[0].InputStream))
            {
                content = reader.ReadToEnd();
            }

            InvoiceEntity[] items = await Task.Run<InvoiceEntity[]>(() =>
                {
                    InvoiceEntity[] result = null;
                    try
                    {
                        result = JsonConvert.DeserializeObject<InvoiceEntity[]>(content);
                        if (result != null && result.Length > 0)
                        {
                            Parallel.ForEach(result, item =>
                            {
                                try
                                {
                                    item.Save();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                    item.Status = Naming.UploadStatusDefinition.匯入失敗;
                                    item.Reason = ex.Message;
                                }
                            });

                            System.IO.File.WriteAllText(fileName, JsonConvert.SerializeObject(result));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                    return result;
                });

            return View("~/Views/DataExchange/Module/ImportInvoiceResult.ascx", items);

        }


    }
}