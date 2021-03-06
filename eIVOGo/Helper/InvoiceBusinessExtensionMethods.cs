﻿using System;
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
using Model.Security.MembershipManagement;
using Utility;

namespace eIVOGo.Helper
{
    public static class InvoiceBusinessExtensionMethods
    {
        public static void MarkPrintedLog<TEntity>(this GenericManager<EIVOEntityDataContext,TEntity> models,InvoiceItem item,UserProfileMember profile)
            where TEntity : class, new()
        {
            if (!item.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice))
            {
                item.CDS_Document.DocumentPrintLog.Add(new DocumentPrintLog
                {
                    PrintDate = DateTime.Now,
                    UID = profile.UID,
                    TypeID = (int)Naming.DocumentTypeDefinition.E_Invoice
                });
            }

            models.DeleteAnyOnSubmit<DocumentPrintQueue>(d => d.DocID == item.InvoiceID);
            models.DeleteAnyOnSubmit<DocumentAuthorization>(d => d.DocID == item.InvoiceID);
            models.SubmitChanges();
        }

        public static void MarkPrintedLog<TEntity>(this GenericManager<EIVOEntityDataContext, TEntity> models, InvoiceAllowance item, UserProfileMember profile)
            where TEntity : class, new()
        {
            if (!item.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Allowance))
            {
                item.CDS_Document.DocumentPrintLog.Add(new DocumentPrintLog
                {
                    PrintDate = DateTime.Now,
                    UID = profile.UID,
                    TypeID = (int)Naming.DocumentTypeDefinition.E_Allowance
                });
            }

            models.DeleteAnyOnSubmit<DocumentPrintQueue>(d => d.DocID == item.AllowanceID);
            models.SubmitChanges();
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
                case Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW:
                    return models.GetInvoiceByAgent(items, profile.CurrentUserRole.OrganizationCategory.CompanyID);

                case Naming.CategoryID.COMP_E_INVOICE_B2C_BUYER:
                    return items.Where(i => i.InvoiceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

                default:
                    return items.Where(i => i.SellerID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                        || i.InvoiceBuyer.BuyerID == profile.CurrentUserRole.OrganizationCategory.CompanyID);

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

        static bool checkAuth(SHA256 hash,OrganizationToken token, AuthTokenViewModel viewModel)
        {
            String computedAuth = Convert.ToBase64String(
                    Encoding.Default.GetBytes(
                        String.Concat(
                            hash.ComputeHash(
                                Encoding.Default.GetBytes($"{token.Organization.ReceiptNo}{token.KeyID}{viewModel.Seed}")
                            ).Select(b => b.ToString("x2"))
                        )
                    )
                );

            return viewModel.Authorization == computedAuth;
        }

        public static OrganizationToken CheckAuthToken(this GenericManager<EIVOEntityDataContext> models, Organization seller, AuthTokenViewModel viewModel)
        {
            SHA256 hash = SHA256.Create();

            var agents = models.GetTable<InvoiceIssuerAgent>().Where(i => i.IssuerID == seller.CompanyID)
                            .Select(a => a.AgentID);

            var tokenItems = models.GetTable<Organization>().Where(o => o.CompanyID == seller.CompanyID || agents.Contains(o.CompanyID))
                        .Join(models.GetTable<OrganizationToken>(), o => o.CompanyID, t => t.CompanyID, (o, t) => t)
                        .ToArray();

            foreach (var token in tokenItems)
            {
                if (checkAuth(hash, token, viewModel))
                    return token;
            }

            return null;
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