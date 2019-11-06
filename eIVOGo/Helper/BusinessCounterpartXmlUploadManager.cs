using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

using eIVOGo.Module.Common;
using Model.DataEntity;
using Model.Locale;
using Model.UploadManagement;
using Utility;
using eIVOGo.Properties;
using DataAccessLayer.basis;

namespace eIVOGo.Helper
{
    public class BusinessCounterpartXmlUploadManager : XmlUploadManager<EIVOEntityDataContext, Organization>
    {
        public BusinessCounterpartXmlUploadManager() : base()
        {
        }
        public BusinessCounterpartXmlUploadManager(GenericManager<EIVOEntityDataContext> manager)
            : base(manager)
        {
        }

        public Naming.InvoiceCenterBusinessType BusinessType { get; set; }

        private int? _masterID;
        private List<UserProfile> _userList;
        public int[] MasterGroup { get; set; }

        public int? MasterID
        {
            get
            {
                return _masterID;
            }
            set
            {
                _masterID = value;
            }
        }

        public override void ParseData(Model.Security.MembershipManagement.UserProfileMember userProfile, string fileName, System.Text.Encoding encoding)
        {
            _userProfile = userProfile;
            if (!_masterID.HasValue)
                _masterID = _userProfile.CurrentUserRole.OrganizationCategory.CompanyID;
            base.ParseData(userProfile, fileName, encoding);
        }

        protected override void initialize()
        {
            _userList = new List<UserProfile>();
        }

        protected override void doSave()
        {
            this.SubmitChanges();
            finalProcess();
        }

        protected override void finalProcess()
        {
            var enterprise = this.GetTable<Organization>().Where(o => o.CompanyID == _masterID)
                .FirstOrDefault().EnterpriseGroupMember.FirstOrDefault();

            String subject = (enterprise != null ? enterprise.EnterpriseGroup.EnterpriseName : "") + " 會員啟用認證信";

            ThreadPool.QueueUserWorkItem(p =>
            {
                foreach (var u in _userList)
                {
                    try
                    {
                        u.NotifyToActivate();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("［" + subject + "］傳送失敗,原因 => " + ex.Message);
                        Logger.Error(ex);
                    }
                }
            });
        }

        protected override bool validate(ItemUpload<Organization> item)
        {
            bool bResult = true;
            if (string.IsNullOrEmpty(item.Entity.CompanyName))
            {
                item.Status = String.Join("、", item.Status, "營業人名稱格式錯誤");
                bResult = false;
            }

            if (String.IsNullOrEmpty(item.Entity.ReceiptNo) ||  item.Entity.ReceiptNo.Length != 8 || !ValueValidity.ValidateString(item.Entity.ReceiptNo, 20))
            {
                item.Status = String.Join("、", item.Status, "統編格式錯誤");
                bResult = false;
            }

            if (string.IsNullOrEmpty(item.Entity.ContactEmail) || !ValueValidity.ValidateString(item.Entity.ContactEmail, 16))
            {
                item.Status = String.Join("、", item.Status, "聯絡人電子郵件格式錯誤");
                bResult = false;
            }

            if (string.IsNullOrEmpty(item.Entity.Addr))
            {
                item.Status = String.Join("、", item.Status, "地址格式錯誤");
                bResult = false;
            }

            if (string.IsNullOrEmpty(item.Entity.Phone))
            {
                item.Status = String.Join("、", item.Status, "電話格式錯誤");
                bResult = false;
            }

            if (bResult)
            {

                var currentItem = this.EntityList.Where(o => o.ReceiptNo == item.Entity.ReceiptNo).FirstOrDefault();

                if (currentItem == null)
                {
                    var newItem =_userList.Where(u=>u.PID== item.Entity.ReceiptNo).Any() ?
                        _items.Where(i => i.Entity.ReceiptNo == item.Entity.ReceiptNo).Select(i => i.Entity).FirstOrDefault():null;
                    if (newItem == null)
                    {

                        if (item.Entity.OrganizationStatus == null)
                        {
                            item.Entity.OrganizationStatus = new OrganizationStatus
                                {
                                };
                        }

                        item.Entity.OrganizationStatus.CurrentLevel = (int)Naming.MemberStatusDefinition.Checked;

                        new BusinessRelationship
                        {
                            Counterpart = item.Entity,
                            BusinessID = (int)BusinessType,
                            MasterID = _masterID.Value,
                            CurrentLevel = (int)Naming.MemberStatusDefinition.Checked
                        };

                        if (MasterGroup != null && MasterGroup.Length > 0)
                        {
                            foreach (var masterID in MasterGroup)
                            {
                                if (masterID == _masterID)
                                    continue;

                                new BusinessRelationship
                                {
                                    Counterpart = item.Entity,
                                    BusinessID = (int)BusinessType,
                                    MasterID = masterID,
                                    CurrentLevel = (int)Naming.MemberStatusDefinition.Checked
                                };
                            }
                        }

                        var orgaCate = new OrganizationCategory
                        {
                            Organization = item.Entity,
                            CategoryID = (int)Naming.CategoryID.COMP_ENTERPRISE_GROUP
                        };

                        this.EntityList.InsertOnSubmit(item.Entity);

                        var userProfile = new UserProfile
                        {
                            PID = item.Entity.ReceiptNo,
                            Phone = item.Entity.Phone,
                            EMail = item.Entity.ContactEmail,
                            Address = item.Entity.Addr,
                            UserProfileExtension = new UserProfileExtension
                            {
                                IDNo = item.Entity.ReceiptNo
                            },
                            UserProfileStatus = new UserProfileStatus
                            {
                                CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                            }
                        };

                        _userList.Add(userProfile);

                        this.GetTable<UserRole>().InsertOnSubmit(new UserRole
                        {
                            RoleID = (int)Naming.RoleID.相對營業人,
                            UserProfile = userProfile,
                            OrganizationCategory = orgaCate
                        });

                    }
                    else
                    {
                        newItem.CompanyName = item.Entity.CompanyName;
                        newItem.ReceiptNo = item.Entity.ReceiptNo;
                        newItem.ContactEmail = item.Entity.ContactEmail;
                        newItem.Addr = item.Entity.Addr;
                        newItem.Phone = item.Entity.Phone;
                        newItem.OrganizationStatus.Entrusting = item.Entity.OrganizationStatus.Entrusting == true;
                        newItem.OrganizationStatus.EntrustToPrint = item.Entity.OrganizationStatus.EntrustToPrint == true;
                    }
                }
                else
                {
                    if (!currentItem.RelativeRelation.Any(b => b.MasterID == _masterID && b.BusinessID == (int)BusinessType))
                    {
                        currentItem.RelativeRelation.Add(
                            new BusinessRelationship
                            {
                                Counterpart = currentItem,
                                BusinessID = (int)BusinessType,
                                MasterID = _masterID.Value
                            });
                    }

                    if (MasterGroup != null && MasterGroup.Length > 0)
                    {
                        foreach (var masterID in MasterGroup)
                        {
                            if (masterID == _masterID)
                                continue;

                            if (!currentItem.RelativeRelation.Any(b => b.MasterID == masterID && b.BusinessID == (int)BusinessType))
                            {
                                currentItem.RelativeRelation.Add(
                                    new BusinessRelationship
                                    {
                                        Counterpart = currentItem,
                                        BusinessID = (int)BusinessType,
                                        MasterID = masterID
                                    });
                            }
                        }
                    }

                    currentItem.CompanyName = item.Entity.CompanyName;
                    currentItem.ReceiptNo = item.Entity.ReceiptNo;
                    currentItem.ContactEmail = item.Entity.ContactEmail;
                    currentItem.Addr = item.Entity.Addr;
                    currentItem.Phone = item.Entity.Phone;
                    currentItem.OrganizationStatus.Entrusting = item.Entity.OrganizationStatus.Entrusting == true;
                    currentItem.OrganizationStatus.EntrustToPrint = item.Entity.OrganizationStatus.EntrustToPrint == true;

                    var currentUser = this.GetTable<UserProfile>().Where(u => u.PID == item.Entity.ReceiptNo).FirstOrDefault();
                    if (currentUser != null)
                    {
                        currentUser.Phone = item.Entity.Phone;
                        currentUser.EMail = item.Entity.ContactEmail;
                        currentUser.Address = item.Entity.Addr;
                    }
                }
            }

            return bResult;
        }
    }
}