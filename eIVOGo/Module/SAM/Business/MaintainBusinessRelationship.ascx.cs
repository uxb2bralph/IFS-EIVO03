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
    public partial class MaintainBusinessRelationship : InquireBusinessRelationship
    {

        protected override void buildQueryItem()
        {
            Expression<Func<Organization, bool>> queryExpr = i => true;
            int? companyID = (int?)null;

            if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
            {
                companyID = int.Parse(CompanyID.SelectedValue);
            }
            if (!String.IsNullOrEmpty(ReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.ReceiptNo == ReceiptNo.Text);
            }
            if (!String.IsNullOrEmpty(CompanyName.Text))
            {
                queryExpr = queryExpr.And(i => i.CompanyName == CompanyName.Text);
            }
            if (!String.IsNullOrEmpty(CompanyStatus.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.OrganizationStatus.CurrentLevel == int.Parse(CompanyStatus.SelectedValue));
            }

            //主動列印
            if (!String.IsNullOrEmpty(EntrustToPrint.SelectedValue))
            {
                if (this.EntrustToPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => (bool)i.OrganizationStatus.EntrustToPrint);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.OrganizationStatus.EntrustToPrint || !i.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }
            //自動接收
            if (!String.IsNullOrEmpty(Entrusting.SelectedValue))
            {
                if (this.Entrusting.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => (bool)i.OrganizationStatus.Entrusting);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.OrganizationStatus.Entrusting || !i.OrganizationStatus.Entrusting.HasValue);
                }
            }

            itemList.BuildQuery = table =>
            {

                var org = table.Context.GetTable<Organization>();

                if (!String.IsNullOrEmpty(BusinessType.SelectedValue))
                {
                    return companyID.HasValue ? table.Where(b => b.MasterID == companyID.Value && b.BusinessID == int.Parse(BusinessType.SelectedValue)).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                        : table.Where(b => b.BusinessID == int.Parse(BusinessType.SelectedValue)).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
                }
                else
                {
                    return companyID.HasValue ? table.Where(b => b.MasterID == companyID.Value).Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                        : table.Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
                }
            };

        }

        protected override void btnAdd_Click(object sender, EventArgs e)
        {
            Server.Transfer(ToImport.TransferTo);
        }
    }
}