using System.Data;
using System.Linq;
using Business.Helper;
using eIVOGo.Helper;
using CommittingResource = eIVOGo.Resource.Models.ViewModel.CommittingDataExtensions;
using eIVOGo.Resource.Controllers;
using Organization = Model.DataEntity.Organization;
using Model.Locale;
using Model.Models.ViewModel;
using Utility;
using Model.DataEntity;

namespace eIVOGo.Models.ViewModel
{
    public static class CommittingDataExtensions
    {
        public static Organization CommitViewModel<TEntity>(this OrganizationViewModel viewModel, eIVOGo.Controllers.SampleController<TEntity> controller)
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
                        ModelState.AddModelError("ReceiptNo", CommittingResource.相同的企業統編已存在__);
                    }
                }
            }

            if (!viewModel.CategoryID.HasValue)
            {
                ModelState.AddModelError("CategoryID", CommittingResource.請設定公司類別__);
            }

            if (!viewModel.SettingInvoiceType.HasValue)
            {
                ModelState.AddModelError("SettingInvoiceType", CommittingResource.請設定發票類別__);
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
            //yuki加入客服專線 start
            item.CustomerServiceHotline = viewModel.CustomerServiceHotline;
            item.CustomerServiceEmail = viewModel.CustomerServiceEmail;
            //yuki加入客服專線 end
            //營業人資訊
            item.BusinessPersonInformation = @viewModel.BusinessPersonInformation;
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