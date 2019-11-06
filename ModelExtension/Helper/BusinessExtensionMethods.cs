using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;

namespace ModelExtension.Helper
{
    public static class BusinessExtensionMethods
    {
        public static BusinessRelationship ApplyCounterpartBusiness<TEntity>(this BusinessRelationshipViewModel viewModel, ModelSource<TEntity> models, out UserProfile userProfile)
            where TEntity : class, new()
        {
            BusinessRelationship model = null;
            Organization orgItem = models.GetTable<Organization>().Where(o => o.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault();
            userProfile = null;

            if (orgItem == null)
            {
                orgItem = new Organization
                {
                    OrganizationStatus = new OrganizationStatus
                    {
                        CurrentLevel = (int)Naming.MemberStatusDefinition.Checked,
                        Entrusting = viewModel.Entrusting,
                        EntrustToPrint = viewModel.EntrustToPrint,
                    },
                    OrganizationExtension = new OrganizationExtension { }
                };

                model = new BusinessRelationship
                {
                    Counterpart = orgItem,
                    BusinessID = viewModel.BusinessType.Value,
                    MasterID = viewModel.CompanyID.Value,
                    CurrentLevel = viewModel.CompanyStatus
                };

                var orgaCate = new OrganizationCategory
                {
                    Organization = orgItem,
                    CategoryID = (int)Naming.CategoryID.COMP_E_INVOICE_B2C_BUYER
                };

                if (!String.IsNullOrEmpty(viewModel.CustomerNo))
                {
                    orgItem.OrganizationBranch.Add(new OrganizationBranch
                    {
                        BranchNo = viewModel.CustomerNo,
                        BranchName = viewModel.CompanyName,
                        Addr = viewModel.Addr,
                        ContactEmail = viewModel.ContactEmail,
                        Phone = viewModel.Phone
                    });
                }

                models.GetTable<Organization>().InsertOnSubmit(orgItem);

                String pid = !String.IsNullOrEmpty(viewModel.CustomerNo) ? viewModel.CustomerNo : viewModel.ReceiptNo;

                if (!models.GetTable<UserProfile>().Any(u => u.PID == pid))
                {
                    userProfile = new UserProfile
                    {
                        PID = pid,
                        Phone = viewModel.Phone,
                        EMail = viewModel.ContactEmail,
                        Address = viewModel.Addr,
                        UserProfileExtension = new UserProfileExtension
                        {
                            IDNo = viewModel.ReceiptNo
                        },
                        UserProfileStatus = new UserProfileStatus
                        {
                            CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                        }
                    };

                    models.GetTable<UserProfile>().InsertOnSubmit(userProfile);
                    models.GetTable<UserRole>().InsertOnSubmit(new UserRole
                    {
                        RoleID = (int)Naming.RoleID.ROLE_BUYER,
                        UserProfile = userProfile,
                        OrganizationCategory = orgaCate
                    });
                }
            }
            else
            {
                model = models.GetTable<BusinessRelationship>().Where(r => r.MasterID == viewModel.CompanyID && r.BusinessID == viewModel.BusinessType && r.RelativeID == orgItem.CompanyID).FirstOrDefault();
                if (model == null)
                {
                    model = new BusinessRelationship
                    {
                        Counterpart = orgItem,
                        BusinessID = viewModel.BusinessType.Value,
                        MasterID = viewModel.CompanyID.Value
                    };
                }

                if(orgItem.OrganizationExtension==null)
                {
                    orgItem.OrganizationExtension = new OrganizationExtension { };
                }

                if(orgItem.OrganizationStatus==null)
                {
                    orgItem.OrganizationStatus = new OrganizationStatus { };
                }

                //var currentUser = models.GetTable<UserProfile>().Where(u => u.PID == viewModel.ReceiptNo).FirstOrDefault();
                //if (currentUser != null)
                //{
                //    currentUser.Phone = viewModel.Phone;
                //    currentUser.EMail = viewModel.ContactEmail;
                //    currentUser.Address = viewModel.Addr;
                //}
            }

            orgItem.CompanyName = viewModel.CompanyName.Trim();
            orgItem.ReceiptNo = viewModel.ReceiptNo;
            orgItem.ContactEmail = viewModel.ContactEmail;
            orgItem.Addr = viewModel.Addr;
            orgItem.Phone = viewModel.Phone;
            orgItem.OrganizationExtension.CustomerNo = viewModel.CustomerNo;

            models.SubmitChanges();

            return model;
        }


    }
}
