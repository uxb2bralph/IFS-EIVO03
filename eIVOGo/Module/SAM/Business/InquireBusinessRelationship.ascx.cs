using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.Security.MembershipManagement;
using Business.Helper;
using System.Linq.Expressions;
using Model.DataEntity;
using Utility;

namespace eIVOGo.Module.SAM.Business
{
    public partial class InquireBusinessRelationship : InquireEntity
    {
        protected UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        protected override UserControl _itemList
        {
            get { return itemList; }
        }

        protected override void buildQueryItem()
        {
            Expression<Func<Organization, bool>> queryExpr = i => true;
            int? companyID = (int?)null;

            if (!String.IsNullOrEmpty(ReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.ReceiptNo == ReceiptNo.Text);
            }
            if (!String.IsNullOrEmpty(CompanyName.Text))
            {
                queryExpr = queryExpr.And(i => i.CompanyName == CompanyName.Text);
            }
            if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
            {
                companyID = int.Parse(CompanyID.SelectedValue);
            }
            if (!String.IsNullOrEmpty(CompanyStatus.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.OrganizationStatus.CurrentLevel == int.Parse(CompanyStatus.SelectedValue));
            }

            itemList.BuildQuery = table =>
            {
                var org = table.Context.GetTable<Organization>();
                if (!String.IsNullOrEmpty(BusinessType.SelectedValue))
                {
                    return companyID.HasValue ? table.Where(b => b.MasterID == companyID.Value && b.BusinessID==int.Parse(BusinessType.SelectedValue)).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                        : table.Where(b => b.BusinessID == int.Parse(BusinessType.SelectedValue)).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
                }
                else
                {
                    return companyID.HasValue ? table.Where(b => b.MasterID == companyID.Value).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                        : table.Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
                }
            };

            itemList.BindData();
            base.buildQueryItem();

        }
    }
}