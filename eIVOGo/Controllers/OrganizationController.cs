using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class OrganizationController : SampleController<InvoiceItem>
    {
        // GET: Organization
        public ActionResult UpdateLogo(int? id)
        {
            var item = models.GetTable<Organization>().Where(c => c.CompanyID == id).FirstOrDefault();
            if (item == null)
            {
                ViewBag.Message = "店家資料錯誤!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }
            if (Request.Files.Count == 0)
            {
                ViewBag.Message = "請設定公司LOGO標幟!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            String path = Server.MapPath("~/LOGO");
            path.CheckStoredPath();

            var file = Request.Files[0];
            String fileName = item.ReceiptNo + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            String storePath = Path.Combine(path, fileName);
            file.SaveAs(storePath);

            item.LogoURL = "LOGO/" + fileName;
            models.SubmitChanges();

            return View("~/Views/Organization/LogoUpdated.ascx", item);

        }

        public ActionResult EditItem(OrganizationViewModel viewModel)
        {
            Organization item = null;
            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = BitConverter.ToInt32(viewModel.DecryptKey(), 0);
                item = models.GetTable<Organization>().Where(u => u.CompanyID == viewModel.CompanyID).FirstOrDefault();
            }

            viewModel.ApplyFromModel(item);

            ViewBag.ViewModel = viewModel;
            return View("~/Views/Organization/Module/EditItem.cshtml", item);
        }

        public ActionResult CommitItem(OrganizationViewModel viewModel)
        {

            Organization item = viewModel.CommitViewModel(this);

            if (item == null)
            {
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            return Json(new { result = true });
        }

        public ActionResult ApplyIssuerAgent(OrganizationViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = BitConverter.ToInt32(viewModel.DecryptKey(), 0);
            }
            Organization item = null;
            item = models.GetTable<Organization>().Where(u => u.CompanyID == viewModel.CompanyID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.ascx", model: "開立人資料錯誤!!");
            }

            return View("~/Views/Organization/Module/ApplyIssuerAgent.ascx", item);
        }

        public ActionResult CommitIssuerAgent(OrganizationViewModel viewModel,int?[] agentID)
        {
            ViewResult result = (ViewResult)ApplyIssuerAgent(viewModel);
            Organization item = result.Model as Organization;

            if(item==null)
            {
                return result;
            }

            models.ExecuteCommand("delete InvoiceIssuerAgent where IssuerID = {0}", item.CompanyID);
            if (agentID != null && agentID.Length > 0)
            {
                foreach (var id in agentID)
                {
                    models.ExecuteCommand("insert InvoiceIssuerAgent (AgentID,IssuerID) values ({0},{1})", id, item.CompanyID);
                }
            }

            return Json(new { result = true });
        }

        public ActionResult GatewaySettings(OrganizationViewModel viewModel)
        {
            ViewResult result = (ViewResult)ApplyIssuerAgent(viewModel);
            Organization item = result.Model as Organization;
            if (item == null)
                return result;
            
            return View("~/Views/Organization/Module/GatewaySettings.cshtml", item);
        }

        public ActionResult CommitDefaultProcessType(OrganizationViewModel viewModel)
        {
            ViewResult result = (ViewResult)ApplyIssuerAgent(viewModel);
            Organization item = result.Model as Organization;
            if (item == null)
                return result;

            item.OrganizationStatus.InvoiceClientDefaultProcessType = (int?)viewModel.DefaultProcessType;
            models.SubmitChanges();

            return Json(new { result = true });
        }


    }
}