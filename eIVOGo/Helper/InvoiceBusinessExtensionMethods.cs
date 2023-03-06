using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Models.ViewModel;
using Model.Security;
using Model.Security.MembershipManagement;
using ModelExtension.Service;
using Utility;

namespace eIVOGo.Helper
{
    public static class InvoiceBusinessExtensionMethods
    {
        public static void MarkPrintedLog<TEntity>(this GenericManager<EIVOEntityDataContext,TEntity> models,InvoiceItem item,UserProfileMember profile)
            where TEntity : class, new()
        {
            models.MarkPrintedLog(item, profile.UID);
        }

        public static void MarkPrintedLog<TEntity>(this GenericManager<EIVOEntityDataContext, TEntity> models, InvoiceAllowance item, UserProfileMember profile)
            where TEntity : class, new()
        {
            models.MarkPrintedLog(item, profile.UID);
        }


        public static IQueryable<InvoiceItem> FilterInvoiceByRole(this GenericManager<EIVOEntityDataContext> models, UserProfileMember profile, IQueryable<InvoiceItem> items)
        {
            return models.DataContext.FilterInvoiceByRole(profile, items);
        }

        public static IQueryable<InvoiceItem> FilterInvoiceByRole(this EIVOEntityDataContext models, UserProfileMember profile, IQueryable<InvoiceItem> items)
        {
            switch ((Naming.CategoryID)profile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                case Naming.CategoryID.COMP_WELFARE:
                    return items;

                case Naming.CategoryID.COMP_INVOICE_AGENT:
                    return models.GetInvoiceByAgent(items, profile.CurrentUserRole.OrganizationCategory.CompanyID);

                case Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW:
                    return items.Where(i => i.SellerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

                case Naming.CategoryID.COMP_E_INVOICE_B2C_BUYER:
                    return items.Where(i => i.InvoiceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

                default:
                    return items.Where(i => i.SellerID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                        || i.InvoiceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

            }

        }

        public static IQueryable<Organization> FilterOrganizationByRole(this EIVOEntityDataContext models, UserProfileMember profile, IQueryable<Organization> items = null)
        {
            if(items == null)
            {
                items = models.GetTable<Organization>();
            }

            switch ((Naming.CategoryID)profile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                    return items;

                case Naming.CategoryID.COMP_INVOICE_AGENT:
                    var issuers = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == profile.CurrentUserRole.OrganizationCategory.CompanyID);
                    return  items.Where(i => i.CompanyID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                        || issuers.Any(a => a.IssuerID == i.CompanyID));

                case Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW:
                case Naming.CategoryID.COMP_E_INVOICE_B2C_BUYER:
                case Naming.CategoryID.COMP_WELFARE:
                default:
                    return items.Where(i => i.CompanyID == profile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

        }

        public static IQueryable<InvoiceAllowance> FilterAllowanceByRole(this GenericManager<EIVOEntityDataContext> models, UserProfileMember profile, IQueryable<InvoiceAllowance> items)
        {
            return models.DataContext.FilterAllowanceByRole(profile, items);
        }

        public static IQueryable<InvoiceAllowance> FilterAllowanceByRole(this EIVOEntityDataContext models, UserProfileMember profile, IQueryable<InvoiceAllowance> items)
        {
            switch ((Naming.CategoryID)profile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                case Naming.CategoryID.COMP_WELFARE:
                    return items;

                case Naming.CategoryID.COMP_INVOICE_AGENT:
                case Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW:
                    return models.GetAllowanceByAgent(items, profile.CurrentUserRole.OrganizationCategory.CompanyID);

                case Naming.CategoryID.COMP_E_INVOICE_B2C_BUYER:
                    return items.Where(i => i.InvoiceAllowanceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

                default:
                    return items.Where(i => i.InvoiceAllowanceSeller.SellerID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                        || i.InvoiceAllowanceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

            }

        }

        public static IQueryable<ProcessRequest> FilterProcessRequestByRole(this EIVOEntityDataContext models, UserProfileMember profile, IQueryable<ProcessRequest> items)
        {
            switch ((Naming.CategoryID)profile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                    return items;

                default:
                    var agentID = profile.CurrentUserRole.OrganizationCategory.CompanyID;
                    var issuers = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == agentID)
                        .Select(a => a.IssuerID);
                    return items.Where(i => i.AgentID == agentID
                                    || issuers.Any(a => i.AgentID == a));

            }

        }

        public static IQueryable<ProductCatalog> FilterProductCatalogByRole(this EIVOEntityDataContext models, UserProfileMember profile, IQueryable<ProductCatalog> items)
        {
            switch ((Naming.CategoryID)profile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                    return items;

                default:
                    var agentID = profile.CurrentUserRole.OrganizationCategory.CompanyID;
                    return items.Where(i => i.ProductSupplier.Any(s => s.SupplierID == agentID));

            }

        }

        public static List<InvoiceNoAllocation> AllocateInvoiceNo(this GenericManager<EIVOEntityDataContext> models, POSDeviceViewModel viewModel)
        {
            List<InvoiceNoAllocation> items = new List<InvoiceNoAllocation>();

            //receiptNo = receiptNo.GetEfficientString();
            var seller = models.GetTable<Organization>().Where(c => c.ReceiptNo == viewModel.company_id).FirstOrDefault();
            bool auth = true;
            if (seller != null)
            {
                if (viewModel.Seed != null && viewModel.Authorization != null)
                {
                    auth = models.CheckAuthToken(seller, viewModel) != null;
                }

                if (auth)
                {
                    try
                    {
                        using (TrackNoManager mgr = new TrackNoManager(models, seller.CompanyID))
                        {
                            for (int i = 0; i < viewModel.quantity; i++)
                            {
                                var item = mgr.AllocateInvoiceNo();
                                if (item == null)
                                    break;

                                item.RandomNo = String.Format("{0:0000}", (DateTime.Now.Ticks % 10000));
                                item.EncryptedContent = String.Concat(item.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode,
                                        String.Format("{0:00000000}", item.InvoiceNo),
                                        item.RandomNo).EncryptContent();
                                models.SubmitChanges();

                                items.Add(item);
                            }
                            mgr.Close();
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }

            return items;
        }

        public static bool CheckAvailableInterval(this GenericManager<EIVOEntityDataContext> models, POSDeviceViewModel viewModel,out String reason)
        {
            reason = null;
            var seller = models.GetTable<Organization>().Where(c => c.ReceiptNo == viewModel.company_id).FirstOrDefault();
            bool auth = true;
            if (seller != null)
            {
                if (viewModel.Seed != null && viewModel.Authorization != null)
                {
                    auth = models.CheckAuthToken(seller, viewModel) != null;
                }

                if (auth)
                {
                    try
                    {
                        using (TrackNoManager mgr = new TrackNoManager(models, seller.CompanyID))
                        {
                            if(!mgr.PeekInvoiceNo().HasValue)
                            {
                                auth = false;
                                reason = "inovice no not available!";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                else
                {
                    reason = "auth failed!";
                }
            }

            return auth;
        }

    }
}