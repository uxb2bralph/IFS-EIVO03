using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Module.Common;

namespace eIVOGo.Module.Inquiry
{
    public partial class InquireInvoiceItem : InquireEntity
    {
        protected UserProfileMember _userProfile;
        protected EntityItemList<EIVOEntityDataContext, CDS_Document> itemList;
        public Boolean setdayrange = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            var user_Roler = _userProfile.UserRoleTable.Where(u=>u.RoleID == 62).FirstOrDefault();
            if (user_Roler != null)
            {
                if (this.BuyerQueryItem!=null)
                this.BuyerQueryItem.Visible = false;
            }
            btnPrint.PrintControls.Add(itemList);
            this.SaveAsExcelButton1.OutputFileName = "InvoiceReport.xls";
            this.SaveAsExcelButton1.DownloadControls.Add(this.itemList.FindControl("gvEntity"));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnPrint.BeforeClick += new EventHandler(btnPrint_BeforeClick);
            this.SaveAsExcelButton1.BeforeClick += new EventHandler(SaveAsExcelButton1_BeforeClick);
        }
        void SaveAsExcelButton1_BeforeClick(object sender, EventArgs e)
        {
            this.itemList.AllowPaging = false;
            setdayrange = false;
            buildQueryItem();


        }
        void btnPrint_BeforeClick(object sender, EventArgs e)
        {
            itemList.AllowPaging = false;
            buildQueryItem();
        }

        protected override UserControl _itemList
        {
            get { return itemList; }
        }

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceItem, bool>> queryExpr = i => i.InvoiceCancellation == null;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.InvoiceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtInvoiceNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => (i.TrackCode.Trim() + i.No.Trim()).Equals(this.txtInvoiceNO.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddPrint.SelectedValue))
            {
                if (this.ddPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.DocumentPrintLog.Any());
                }
                else
                {
                    queryExpr = queryExpr.And(i => !i.CDS_Document.DocumentPrintLog.Any());
                }
            }
            if (!String.IsNullOrEmpty(this.EntrustToPrint.SelectedValue))
            {
                if (this.EntrustToPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => (bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint || !i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }
            if(this.BuyerQueryItem.Visible == true & !String.IsNullOrEmpty(this.txtBuyerReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.InvoiceBuyer.ReceiptNo == this.txtBuyerReceiptNo.Text);
            }
            itemList.BuildQuery = table =>
            {
                var invoices = table.Context.GetTable<InvoiceItem>().OrderByDescending(i => i.InvoiceID).Where(queryExpr);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(s => s.SellerID == companyID)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(b => b.BuyerID == companyID)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                    }
                    else
                    {
                        invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i)
                            .Concat(invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i));
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            invoices = invoices.Where(i => i.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                        else
                        {
                            invoices = invoices.Where(i => i.InvoiceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                       .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5))
                       , d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(i => i.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(i=>i.DocID);
                }
                else
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice)
                       .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5)), d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(i => i.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice)
                        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(i=>i.DocID);
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }

        }

        protected void rbChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Server.Transfer(rbChange.SelectedValue);
        }

    }    
}
