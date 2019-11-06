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

namespace eIVOGo.Models.ViewModel
{
    public static class CommittingDataExtensions
    {
        public static Organization CommitViewModel<TEntity>(this OrganizationViewModel viewModel,eIVOGo.Controllers.SampleController<TEntity> controller)
            where TEntity : class, new()
        {
            dynamic ViewBag = controller.ViewBag;
            var models = controller.DataSource;
            var ModelState = controller.ModelState;

            ViewBag.ViewModel = viewModel;

            viewModel.OrganizationValueCheck(controller);

            Organization item = null;
            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
                item = models.GetTable<Organization>().Where(u => u.CompanyID == viewModel.CompanyID).FirstOrDefault();
            }

            if (viewModel.ReceiptNo != null)
            {
                if (item == null || item.ReceiptNo != viewModel.ReceiptNo)
                {
                    if (models.GetTable<Organization>().Any(o => o.ReceiptNo == viewModel.ReceiptNo))
                    {
                        ModelState.AddModelError("ReceiptNo", "相同的企業統編已存在!!");
                    }
                }
            }

            if (!viewModel.CategoryID.HasValue)
            {
                ModelState.AddModelError("CategoryID", "請設定公司類別!!");
            }

            if (!viewModel.SettingInvoiceType.HasValue)
            {
                ModelState.AddModelError("SettingInvoiceType", "請設定發票類別!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return null;
            }

            bool isNewItem = false;
            OrganizationCategory orgaCate = null;
            if (item == null)
            {
                item = new Organization
                {
                    OrganizationStatus = new OrganizationStatus
                    {
                        CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                    }

                };

                models.GetTable<Organization>().InsertOnSubmit(item);
                isNewItem = true;
            }

            orgaCate = item.OrganizationCategory.FirstOrDefault();
            if (orgaCate == null)
            {
                orgaCate = new OrganizationCategory
                {
                    Organization = item
                };

            }

            orgaCate.CategoryID = viewModel.CategoryID.Value;

            item.ReceiptNo = viewModel.ReceiptNo;
            item.CompanyName = viewModel.CompanyName;
            item.Addr = viewModel.Addr;
            item.Phone = viewModel.Phone;
            item.Fax = viewModel.Fax;
            item.UndertakerName = viewModel.UndertakerName;
            item.ContactName = viewModel.ContactName;
            item.ContactTitle = viewModel.ContactTitle;
            item.ContactPhone = viewModel.ContactPhone;
            item.ContactMobilePhone = viewModel.ContactMobilePhone;
            item.ContactEmail = viewModel.ContactEmail;
            item.OrganizationStatus.SetToPrintInvoice = viewModel.SetToPrintInvoice;
            item.OrganizationStatus.SetToOutsourcingCS = viewModel.SetToOutsourcingCS;
            item.OrganizationStatus.InvoicePrintView = viewModel.SetToPrintInvoice == true ? viewModel.InvoicePrintView.GetEfficientString() : null;
            item.OrganizationStatus.AllowancePrintView = viewModel.SetToPrintInvoice == true ? viewModel.AllowancePrintView.GetEfficientString() : null;
            item.OrganizationStatus.AuthorizationNo = viewModel.AuthorizationNo.GetEfficientString();
            item.OrganizationStatus.SetToNotifyCounterpartBySMS = viewModel.SetToNotifyCounterpartBySMS;
            item.OrganizationStatus.DownloadDataNumber = viewModel.DownloadDataNumber;
            item.OrganizationStatus.UploadBranchTrackBlank = viewModel.UploadBranchTrackBlank;
            item.OrganizationStatus.PrintAll = viewModel.PrintAll;
            item.OrganizationStatus.SettingInvoiceType = (int?)viewModel.SettingInvoiceType;
            item.OrganizationStatus.SubscribeB2BInvoicePDF = viewModel.SubscribeB2BInvoicePDF;
            item.OrganizationStatus.UseB2BStandalone = viewModel.UseB2BStandalone;
            item.OrganizationStatus.DisableWinningNotice = viewModel.DisableWinningNotice ?? true;
            item.OrganizationStatus.DisableIssuingNotice = viewModel.DisableIssuingNotice ?? true;
            item.OrganizationStatus.InvoiceNoticeSetting = viewModel.NoticeStatus != null && viewModel.NoticeStatus.Length > 0 ? viewModel.NoticeStatus.Sum() : (int?)null;
            item.OrganizationStatus.EntrustToPrint = viewModel.EntrustToPrint == true;
            item.OrganizationStatus.EnableTrackCodeInvoiceNoValidation = viewModel.EnableTrackCodeInvoiceNoValidation;
            item.OrganizationStatus.IgnoreDuplicatedDataNumber = viewModel.IgnoreDuplicatedDataNumber;

            models.SubmitChanges();

            if (isNewItem)
            {
                models.CreateDefaultUser(item, orgaCate);
            }

            if (orgaCate.CategoryID == (int)Naming.B2CCategoryID.開立發票店家代理)
            {
                if (!models.GetTable<InvoiceIssuerAgent>().Any(a => a.IssuerID == item.CompanyID && a.AgentID == item.CompanyID))
                {
                    models.ExecuteCommand(
                            @"INSERT INTO InvoiceIssuerAgent
                                (AgentID, IssuerID)
                                VALUES ({0},{0})", item.CompanyID);
                }
            }
            else
            {
                models.ExecuteCommand(
                    @"DELETE FROM InvoiceIssuerAgent
                        WHERE (AgentID = {0}) AND (IssuerID = {0})", item.CompanyID);
            }

            return item;
        }

    }
}