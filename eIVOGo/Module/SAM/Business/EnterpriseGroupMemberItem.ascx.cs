using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;
using eIVOGo.Properties;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using eIVOGo.Module.Common;
using Model.DataEntity;
using Model.Locale;
using Utility;
using Uxnet.Web.Module.DataModel;
using Uxnet.Web.WebUI;
using Model.InvoiceManagement;

namespace eIVOGo.Module.SAM.Business
{
    public partial class EnterpriseGroupMemberItem : EditEntityItemBase<EIVOEntityDataContext, EnterpriseGroupMember>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_entity != null)
                this.EnterpriseID.Visible = false;

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.QueryExpr = m => m.CompanyID == (int?)modelItem.DataItem;
        }

        protected override bool saveEntity()
        {
            var mgr = dsEntity.CreateDataManager();

            loadEntity();

            String receiptNo = ReceiptNo.Text.Trim();
            if (_entity == null || _entity.Organization.ReceiptNo != receiptNo)
            {
                if (mgr.GetTable<Organization>().Any(o => o.ReceiptNo == receiptNo))
                {
                    this.AjaxAlert("相同的企業統編已存在!!");
                    return false;
                }
            }

            bool isNewItem = false;
            OrganizationCategory orgaCate = null;
            if (_entity == null)
            {
                if (String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                {
                    this.AjaxAlert("未指定集團加值中心!!");
                    return false;
                }

                _entity = new EnterpriseGroupMember
                {
                    EnterpriseID = int.Parse(EnterpriseID.SelectedValue),
                    Organization = new Organization
                    {
                        OrganizationStatus = new OrganizationStatus
                        {
                            CurrentLevel = (int)Naming.MemberStatusDefinition.Checked
                        }
                    }
                };

                orgaCate = new OrganizationCategory
                {
                    Organization = _entity.Organization,
                    CategoryID = (int)Naming.CategoryID.COMP_ENTERPRISE_GROUP
                };

                mgr.EntityList.InsertOnSubmit(_entity);
                isNewItem = true;
            }

            _entity.Organization.ReceiptNo = ReceiptNo.Text.Trim();
            _entity.Organization.CompanyName = CompanyName.Text.Trim();
            _entity.Organization.Addr = Addr.Text.Trim();
            _entity.Organization.Phone = Phone.Text.Trim();
            _entity.Organization.Fax = Fax.Text.Trim();
            _entity.Organization.UndertakerName = UndertakerName.Text.Trim();
            _entity.Organization.ContactName = ContactName.Text.Trim();
            _entity.Organization.ContactTitle = ContactTitle.Text.Trim();
            _entity.Organization.ContactPhone = ContactPhone.Text.Trim();
            _entity.Organization.ContactMobilePhone = ContactMobilePhone.Text.Trim();
            _entity.Organization.ContactEmail = ContactEmail.Text.Trim();
            _entity.Organization.OrganizationStatus.SetToPrintInvoice = SetToPrintInvoice.Checked;
            _entity.Organization.OrganizationStatus.InvoicePrintView = _entity.Organization.OrganizationStatus.SetToPrintInvoice.Value ? InvoicePrintView.Text : null;
            _entity.Organization.OrganizationStatus.IronSteelIndustry = IronSteelIndustry.Checked;
            _entity.Organization.OrganizationStatus.Entrusting = Entrusting.Checked;
            if (!Entrusting.Checked)
            {
                _entity.Organization.OrganizationStatus.TokenID = null;
            }

            if (!_entity.Organization.OrganizationValueCheck(this))
            {
                return false;
            }

            mgr.SubmitChanges();

            if (isNewItem)
            {
                createDefaultUser(mgr, orgaCate);
            }

            if (Entrusting.Checked)
            {
                (new InvoiceManager(mgr)).Context.UpdateGroupMemberUserRole(_entity.CompanyID,(int)Naming.RoleID.集團成員, (int)Naming.RoleID.分店相對營業人);
            }
            else
            {
                (new InvoiceManager(mgr)).Context.UpdateGroupMemberUserRole(_entity.CompanyID, (int)Naming.RoleID.分店相對營業人, (int)Naming.RoleID.集團成員);
            }

            return true;
        }

        private void createDefaultUser(GenericManager<EIVOEntityDataContext, EnterpriseGroupMember> mgr, OrganizationCategory orgaCate)
        {
            var userProfile = new UserProfile
            {
                PID = _entity.Organization.ReceiptNo,
                Phone = _entity.Organization.Phone,
                EMail = _entity.Organization.ContactEmail,
                Address = _entity.Organization.Addr,
                UserProfileExtension = new UserProfileExtension
                {
                    IDNo = _entity.Organization.ReceiptNo
                },
                UserProfileStatus = new UserProfileStatus
                {
                    CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                }
            };

            mgr.GetTable<UserRole>().InsertOnSubmit(new UserRole
            {
                RoleID = (int)Naming.RoleID.集團成員,
                UserProfile = userProfile,
                OrganizationCategory = orgaCate
            });

            mgr.SubmitChanges();

            userProfile.NotifyToActivate();

        }
    }
}