using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using DataAccessLayer.basis;
using Uxnet.Com.DataAccessLayer;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Model.Models.ViewModel;
using Utility;
using Model.Helper;

namespace eIVOGo.Helper
{
    public static class QueryExtensionMethods
    {

        public static IQueryable<Organization> InitializeOrganizationQuery(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr)
        {
            switch ((Naming.CategoryID)userProfile.CurrentUserRole.OrganizationCategory.CategoryID)
            {
                case Naming.CategoryID.COMP_SYS:
                    return mgr.GetTable<Organization>().Where(
                        o => o.OrganizationCategory.Any(
                            c => c.CategoryID == (int)Naming.CategoryID.COMP_E_INVOICE_B2C_SELLER
                                || c.CategoryID == (int)Naming.CategoryID.COMP_VIRTUAL_CHANNEL
                                || c.CategoryID == (int)Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW
                                || c.CategoryID == (int)Naming.CategoryID.COMP_INVOICE_AGENT));

                case Naming.CategoryID.COMP_INVOICE_AGENT:
                case Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW:
                    return mgr.GetQueryByAgent(userProfile.CurrentUserRole.OrganizationCategory.CompanyID);

                case Naming.CategoryID.COMP_E_INVOICE_B2C_SELLER:
                case Naming.CategoryID.COMP_VIRTUAL_CHANNEL:
                case Naming.CategoryID.COMP_CROSS_BORDER_MURCHANT:
                    return mgr.GetTable<Organization>().Where(
                        o => o.CompanyID == userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                default:
                    break;
            }

            return mgr.GetTable<Organization>().Where(o => false);
        }

        public static IQueryable<UserProfile> FilterByOrganization(this IQueryable<UserProfile> items, GenericManager<EIVOEntityDataContext> models, int companyID)
        {
            return items.Join(models.GetTable<UserRole>()
                            .Join(models.GetTable<OrganizationCategory>().Where(c => c.CompanyID == companyID),
                                r => r.OrgaCateID, c => c.OrgaCateID, (r, c) => r),
                        u => u.UID, r => r.UID, (u, r) => u);
        }

        public static int GetAttachedPdfPageCount(this int docID, GenericManager<EIVOEntityDataContext> models)
        {
            var attachment = models.GetTable<Attachment>().Where(a => a.DocID == docID).FirstOrDefault();
            return GetAttachedPdfPageCount(attachment);
        }

        public static int GetAttachedPdfPageCount(this Attachment attachment)
        {
            if (attachment == null || !File.Exists(attachment.StoredPath))
            {
                return 0;
            }

            var content = File.ReadAllText(attachment.StoredPath);
            var match = Regex.Match(content, "/Count(?([^\\r\\n])\\s)\\d+");
            if (match.Value != String.Empty)
            {
                return int.Parse(match.Value.Substring(7));
            }
            return 0;
        }

        public static IQueryable<BusinessRelationship> PromptBusinessRelationship(this BusinessRelationshipQueryViewModel viewModel, GenericManager<EIVOEntityDataContext> models,out IQueryable<BusinessRelationship> items,out IQueryable<Organization> masterItems, out IQueryable<Organization> relativeItems)
        {
            if (viewModel.KeyID != null)
            {
                viewModel = JsonConvert.DeserializeObject<BusinessRelationshipQueryViewModel>(viewModel.KeyID.DecryptData());
            }

            relativeItems = models.GetTable<Organization>();
            masterItems = models.GetTable<Organization>();
            items = models.GetTable<BusinessRelationship>();

            if (viewModel.CompanyID.HasValue)
            {
                items = items.Where(r => r.MasterID == viewModel.CompanyID);
            }

            if (viewModel.RelativeID.HasValue)
            {
                items = items.Where(r => r.RelativeID == viewModel.RelativeID);
            }

            if (viewModel.BusinessID.HasValue)
            {
                items = items.Where(r => r.BusinessID == (int)viewModel.BusinessID);
            }

            viewModel.ReceiptNo = viewModel.ReceiptNo.GetEfficientString();
            if (viewModel.ReceiptNo != null)
            {
                relativeItems = relativeItems.Where(i => i.ReceiptNo == viewModel.ReceiptNo);
            }

            viewModel.MasterNo = viewModel.MasterNo.GetEfficientString();
            if (viewModel.MasterNo != null)
            {
                masterItems = masterItems.Where(i => i.ReceiptNo == viewModel.MasterNo);
            }

            return items = items
                .Join(masterItems, m => m.MasterID, o => o.CompanyID, (m, o) => m)
                .Join(relativeItems, m => m.RelativeID, o => o.CompanyID, (m, o) => m);
        }

    }
}