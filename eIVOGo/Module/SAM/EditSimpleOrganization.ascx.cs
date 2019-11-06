using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using eIVOGo.Module.Common;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using DataAccessLayer.basis;
using eIVOGo.Module.UI;

namespace eIVOGo.Module.SAM
{  
   
    public partial class EditSimpleOrganization : EditOrganization
    {
        protected override void AddMember_PreRender(object sender, EventArgs e)
        {
            if (_entity != null)
            {
                modelItem.DataItem = _entity.CompanyID;
            }
        }

        protected override bool saveEntity()
        {
            var mgr = dsEntity.CreateDataManager();

            loadEntity();

            String receiptNo = ReceiptNo.Text.Trim();
            //if(receiptNo!="0000000000" && !receiptNo.CheckRegno())
            //{
            //    this.AjaxAlert("統編資料錯誤!!");
            //    return false;
            //}
            if (_entity == null || _entity.ReceiptNo != receiptNo)
            {
                if (mgr.GetTable<Organization>().Any(o => o.ReceiptNo == receiptNo))
                {
                    this.AjaxAlert("相同的企業統編已存在!!");
                    return false;
                }
            }

            String[] branchNo = null;
            var branchNoItems = Request.Form.GetValues("BranchNo");
            if (branchNoItems != null)
            {
                if (String.IsNullOrEmpty(Request["addItem"]))
                {
                    if(branchNoItems.Length>1)
                    {
                        branchNo = new String[branchNoItems.Length - 1];
                        Array.Copy(branchNoItems, branchNo, branchNoItems.Length - 1);
                    }
                }
                else
                {
                    branchNo = branchNoItems;
                }
            }
            if (branchNo != null)
            {
                for (int i = 0; i < branchNo.Length; i++)
                {
                    branchNo[i] = branchNo[i].GetEfficientString();
                    if (branchNo[i] == null)
                    {
                        this.AjaxAlert($"請輸入第{i + 1}家分店代號!!");
                        return false;
                    }
                }
                var duplicated = branchNo.GroupBy(b => b).Where(g => g.Count() > 1);
                if (duplicated.Count()>0)
                {
                    this.AjaxAlert("分店代號重複輸入:" + String.Join("、", duplicated.Select(g => g.Key)));
                    return false;
                }

                int companyID = _entity != null ? _entity.CompanyID : -1;
                var branchTable = mgr.GetTable<OrganizationBranch>();
                var exists = branchNo.Where(b => branchTable.Any(t => t.BranchNo == b && t.CompanyID != companyID)).ToList();
                if (exists.Count() > 0)
                {
                    this.AjaxAlert("分店代號已被使用:" + String.Join("、", exists));
                    return false;
                }
            }

            //if (String.IsNullOrEmpty(CategoryID.SelectedValue))
            //{
            //    this.AjaxAlert("請設定公司類別!!");
            //    return false;
            //}

            //if (String.IsNullOrEmpty(InvoiceType.SelectedValue))
            //{
            //    this.AjaxAlert("請設定發票類別!!");
            //    return false;
            //}

            bool isNewItem = false;
            OrganizationCategory orgaCate = null;
            if (_entity == null)
            {
                _entity = new Organization
                    {
                        OrganizationStatus = new OrganizationStatus
                        {
                            CurrentLevel = (int)Naming.MemberStatusDefinition.Checked
                        }

                    };

                orgaCate = new OrganizationCategory
                {
                    Organization = _entity,
                    CategoryID = (int)Naming.MemberCategoryID.相對營業人
                };

                mgr.EntityList.InsertOnSubmit(_entity);
                isNewItem = true;
            }
            else
            {
                orgaCate = _entity.OrganizationCategory.First();
            }

            //orgaCate.CategoryID = int.Parse(CategoryID.SelectedValue);

            _entity.ReceiptNo = receiptNo;
            _entity.CompanyName = CompanyName.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.Addr = Addr.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.Phone = Phone.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.Fax = Fax.Text.Trim().InsteadOfNullOrEmpty(null);
            //_entity.UndertakerName = UndertakerName.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.ContactName = ContactName.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.ContactTitle = ContactTitle.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.ContactPhone = ContactPhone.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.ContactMobilePhone = ContactMobilePhone.Text.Trim().InsteadOfNullOrEmpty(null);
            _entity.ContactEmail = ContactEmail.Text.Trim().InsteadOfNullOrEmpty(null);
            //_entity.OrganizationStatus.SetToPrintInvoice = SetToPrintInvoice.Checked;
            //_entity.OrganizationStatus.SetToOutsourcingCS = SetToOutsourcingCS.Checked;
            //_entity.OrganizationStatus.InvoicePrintView = _entity.OrganizationStatus.SetToPrintInvoice.Value ? InvoicePrintView.Text : null;
            //_entity.OrganizationStatus.AllowancePrintView = _entity.OrganizationStatus.SetToPrintInvoice.Value ? AllowancePrintView.Text : null;
            //_entity.OrganizationStatus.AuthorizationNo = AuthorizationNo.Text.Trim().InsteadOfNullOrEmpty(null);
            //_entity.OrganizationStatus.SetToNotifyCounterpartBySMS = SetToNotifyCounterpartBySMS.Checked;
            //_entity.OrganizationStatus.DownloadDataNumber = DownloadDataNumber.Checked;
            //_entity.OrganizationStatus.UploadBranchTrackBlank = UploadBranchTrackBlank.Checked;
            //_entity.OrganizationStatus.PrintAll = PrintAll.Checked;
            //_entity.OrganizationStatus.SettingInvoiceType = int.Parse(InvoiceType.SelectedValue);
            //_entity.OrganizationStatus.SubscribeB2BInvoicePDF = SubscribeB2BInvoicePDF.Checked;
            //_entity.OrganizationStatus.UseB2BStandalone = UseB2BStandalone.Checked;
            //_entity.OrganizationStatus.DisableWinningNotice = !SetWinningNotice.Checked;
            //_entity.OrganizationStatus.EntrustToPrint = EntrustToPrint.Checked;
            //_entity.OrganizationStatus.EnableTrackCodeInvoiceNoValidation = EnableTrackCodeInvoiceNoValidation.Checked;
            if (!_entity.OrganizationValueCheck(this))
            {
                return false;
            }

            String[] branchID = Request.Form.GetValues("branchID");
            String[] branchName = Request.Form.GetValues("BranchName");
            String[] branchContactEmail = Request.Form.GetValues("BranchContactEmail");
            String[] branchPhone = Request.Form.GetValues("BranchPhone");
            String[] branchAddr = Request.Form.GetValues("BranchAddr");
            int idx = 0;
            if(branchID!=null && branchID.Length>0)
            {
                int[] id = branchID.Select(i => int.Parse(i)).ToArray();
                for (; idx<id.Length;idx++ )
                {
                    var item = _entity.OrganizationBranch.Where(b => b.BranchID == id[idx]).FirstOrDefault();
                    if (item == null)
                        continue;
                    item.Addr = branchAddr[idx].GetEfficientString();
                    item.BranchName = branchName[idx].GetEfficientString();
                    item.ContactEmail = branchContactEmail[idx].GetEfficientString();
                    item.Phone = branchPhone[idx].GetEfficientString();
                    item.BranchNo = branchNo[idx].GetEfficientString();
                }
            }

            UserProfile userProfile = null;
            if(!String.IsNullOrEmpty(Request["addItem"]) && branchNo[idx].GetEfficientString()!=null)
            {
                var item = new OrganizationBranch { };
                _entity.OrganizationBranch.Add(item);
                item.Addr = branchAddr[idx].GetEfficientString();
                item.BranchName = branchName[idx].GetEfficientString();
                item.ContactEmail = branchContactEmail[idx].GetEfficientString();
                item.Phone = branchPhone[idx].GetEfficientString();
                item.BranchNo = branchNo[idx].GetEfficientString();

                userProfile = mgr.GetTable<UserProfile>().Where(u => u.PID == item.BranchNo).FirstOrDefault();
                if (userProfile == null)
                {
                    userProfile = new UserProfile
                    {
                        PID = item.BranchNo,
                        Phone = item.Phone,
                        EMail = item.ContactEmail,
                        Address = item.Addr,
                        Password2 = ValueValidity.MakePassword(_entity.ReceiptNo),
                        UserProfileExtension = new UserProfileExtension
                        {
                            IDNo = item.BranchNo
                        },
                        UserProfileStatus = new UserProfileStatus
                        {
                            CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                        }
                    };

                    mgr.GetTable<UserRole>().InsertOnSubmit(new UserRole
                    {
                        RoleID = (int)Naming.RoleID.分店相對營業人,
                        UserProfile = userProfile,
                        OrganizationCategory = orgaCate
                    });
                }
            }

            var deleteID = Request.Form.GetValues("deleteID");
            if(deleteID!=null && deleteID.Length>0)
            {
                var id = deleteID.Select(i => int.Parse(i));
                mgr.DeleteAllOnSubmit<OrganizationBranch>(b => id.Contains(b.BranchID));
            }

            mgr.SubmitChanges();

            if (userProfile != null)
            {
                userProfile.SendActivationNotice();
            }
            else if (isNewItem)
            {
                createDefaultUser(mgr, orgaCate);
            }

            return true;
        }

    }
}