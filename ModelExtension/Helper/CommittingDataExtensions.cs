﻿using System;
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

using ClosedXML.Excel;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Model.Helper;
using DataAccessLayer.basis;
using ModelExtension.Notification;
using Newtonsoft.Json;

namespace ModelExtension.Helper
{
    public static class CommittingDataExtensions
    {
        public static Organization CommitOrganizationViewModel(this OrganizationViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ModelStateDictionary modelState)
        {
            viewModel.OrganizationValueCheck(modelState);

            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
            }
            Organization item = models.GetTable<Organization>().Where(u => u.CompanyID == viewModel.CompanyID).FirstOrDefault();

            if (viewModel.ReceiptNo != null)
            {
                if (item == null || item.ReceiptNo != viewModel.ReceiptNo)
                {
                    if (models.GetTable<Organization>().Any(o => o.ReceiptNo == viewModel.ReceiptNo))
                    {
                        modelState.AddModelError("ReceiptNo", "相同的企業統編已存在!!");
                    }
                }
            }

            if (!viewModel.CategoryID.HasValue)
            {
                modelState.AddModelError("CategoryID", "請設定公司類別!!");
            }

            if (!viewModel.SettingInvoiceType.HasValue)
            {
                modelState.AddModelError("SettingInvoiceType", "請設定發票類別!!");
            }

            if (!modelState.IsValid)
            {
                return null;
            }

            OrganizationCategory orgaCate = null;
            if (item == null)
            {
                item = new Organization
                {
                };

                models.GetTable<Organization>().InsertOnSubmit(item);
            }

            if (item.OrganizationStatus == null)
            {
                item.OrganizationStatus = new OrganizationStatus
                {
                    CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                };
            }

            if (item.OrganizationExtension == null)
            {
                item.OrganizationExtension = new OrganizationExtension { };
            }

            orgaCate = item.OrganizationCategory.FirstOrDefault();
            if (orgaCate == null)
            {
                orgaCate = new OrganizationCategory
                {
                    Organization = item
                };

            }

            orgaCate.CategoryID = (int)viewModel.CategoryID;

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
            item.OrganizationExtension.CustomerNo = viewModel.CustomerNo.GetEfficientString();
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
            item.OrganizationStatus.DisableIssuingNotice = viewModel.DisableIssuingNotice ?? true;
            item.OrganizationStatus.InvoiceNoticeSetting = viewModel.NoticeStatus != null && viewModel.NoticeStatus.Length > 0 ? viewModel.NoticeStatus.Sum() : (int?)null;
            item.OrganizationStatus.DisableWinningNotice = (item.OrganizationStatus.InvoiceNoticeSetting & (int)Naming.InvoiceNoticeStatus.Winning) == 0;
            item.OrganizationStatus.EntrustToPrint = viewModel.EntrustToPrint == true;
            item.OrganizationStatus.EnableTrackCodeInvoiceNoValidation = viewModel.EnableTrackCodeInvoiceNoValidation;
            item.OrganizationStatus.IgnoreDuplicatedDataNumber = viewModel.IgnoreDuplicatedDataNumber;

            models.SubmitChanges();

            //if (isNewItem)
            //{
            //    models.CreateDefaultUser(item, orgaCate);
            //}

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

        public static UserProfile CommitUserProfileViewModel(this UserProfileViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ModelStateDictionary modelState, UserProfileMember creator)
        {
            UserProfile item = null;

            if (!String.IsNullOrEmpty(viewModel.KeyID))
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();

            viewModel.UserProfileValueCheck(creator, modelState);
            if (viewModel.PID != null)
            {
                if ((item != null && models.GetTable<UserProfile>().Any(u => u.UID != item.UID && u.PID == viewModel.PID))
                   || (item == null && models.GetTable<UserProfile>().Any(u => u.PID == viewModel.PID)))
                {
                    modelState.AddModelError("PID", "這個帳號已被使用，請更換申請帳號!!");
                }
            }

            if (item == null)
            {
                ///新增帳號
                ///
                if (viewModel.Password == null)
                {
                    modelState.AddModelError("PassWord", "密碼不可為空白!!");
                }
            }

            if (!modelState.IsValid)
            {
                return null;
            }

            bool isNew = false;
            if (item == null)
            {
                item = new UserProfile
                {
                    UserProfileStatus = new UserProfileStatus
                    {
                        CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                    },
                    Creator = creator.UID,
                };

                models.GetTable<UserProfile>().InsertOnSubmit(item);
                isNew = true;
            }

            item.PID = viewModel.PID;
            item.UserName = viewModel.UserName;
            if (!String.IsNullOrEmpty(viewModel.Password))
            {
                item.Password2 = Utility.ValueValidity.MakePassword(viewModel.Password);
                if (viewModel.WaitForCheck == true)
                    item.UserProfileStatus.CurrentLevel = (int)Naming.MemberStatusDefinition.Checked;
            }
            item.EMail = viewModel.EMail;
            item.Address = viewModel.Address;
            item.Phone = viewModel.Phone;
            item.MobilePhone = viewModel.MobilePhone;
            item.Phone2 = viewModel.Phone2;

            models.SubmitChanges();

            models.ExecuteCommand("delete ResetUserPassword where UID = {0}", item.UID);

            if(isNew)
            {
                PortalNotification.NotifyToActivate(item);
                viewModel.UID = item.UID;
            }

            viewModel.KeyID = null;
            viewModel.CommitUserRoleViewModel(models, modelState, creator);

            return item;
        }

        public static UserRole CommitUserRoleViewModel(this UserRoleViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ModelStateDictionary modelState, UserProfileMember creator)
        {
            UserRoleViewModel tmp = viewModel;
            if (!String.IsNullOrEmpty(viewModel.KeyID))
            {
                tmp = JsonConvert.DeserializeObject<UserRoleViewModel>(viewModel.KeyID.DecryptData());
            }

            viewModel.UserRoleValueCheck(creator, modelState);

            int? orgaCateID = models.GetTable<OrganizationCategory>().Where(c => c.CompanyID == viewModel.SellerID)
                    .Select(c => c.OrgaCateID).FirstOrDefault();

            if (!orgaCateID.HasValue)
            {
                modelState.AddModelError("SellerID", "請選擇所屬會員!!");
            }

            if (!modelState.IsValid)
            {
                return null;
            }

            models.ExecuteCommand("delete UserRole where UID = {0}", viewModel.UID);

            UserRole item = new UserRole
            {
                OrgaCateID = orgaCateID.Value,
                RoleID = ((int?)viewModel.RoleID) ?? (int)Naming.EIVOUserRoleID.會員,
                UID = viewModel.UID.Value,
            };

            models.GetTable<UserRole>().InsertOnSubmit(item);
            models.SubmitChanges();

            return item;
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

            return userProfile;
        }

        public static BusinessRelationship CommitBusinessRelationshipViewModel(this BusinessRelationshipViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ModelStateDictionary modelState)
        {
            BusinessRelationship item = null;
            Organization orgItem = models.GetTable<Organization>().Where(o => o.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault();

            if (orgItem == null)
            {
                orgItem = CommitOrganizationViewModel(viewModel, models, new ModelStateDictionary { });
            }

            Organization masterItem = models.GetTable<Organization>().Where(m => m.CompanyID == viewModel.MasterID).FirstOrDefault();
            if (masterItem == null)
            {
                masterItem = models.GetTable<Organization>().Where(m => m.ReceiptNo == viewModel.MasterNo).FirstOrDefault();
            }

            if (masterItem == null)
            {
                masterItem = (new OrganizationViewModel 
                {
                    ReceiptNo = viewModel.MasterNo,
                    CompanyName = viewModel.MasterName,
                }).CommitOrganizationViewModel(models, new ModelStateDictionary { });
            }

            if(!viewModel.BusinessID.HasValue)
            {
                viewModel.BusinessID = Naming.InvoiceCenterBusinessType.銷項;
            }

            item = models.GetTable<BusinessRelationship>()
                .Where(b => b.MasterID == masterItem.CompanyID)
                .Where(b => b.RelativeID == orgItem.CompanyID)
                .Where(b => b.BusinessID == (int)viewModel.BusinessID)
                .FirstOrDefault();

            if (item == null)
            {
                item = new BusinessRelationship
                {
                    RelativeID = orgItem.CompanyID,
                    BusinessID = (int)viewModel.BusinessID,
                    MasterID = masterItem.CompanyID,
                    CurrentLevel = viewModel.CompanyStatus
                };
                models.GetTable<BusinessRelationship>().InsertOnSubmit(item);
            }

            item.CompanyName = viewModel.CompanyName.GetEfficientString();
            item.ContactEmail = viewModel.ContactEmail.GetEfficientString();
            item.Addr = viewModel.Addr.GetEfficientString();
            item.Phone = viewModel.Phone.GetEfficientString();
            item.CustomerNo = viewModel.CustomerNo.GetEfficientString();

            models.SubmitChanges();

            return item;
        }

    }
}