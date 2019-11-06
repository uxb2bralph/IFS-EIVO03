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

namespace eIVOGo.Module.Inquiry.ForOP
{
    public partial class InquireResendInvoiceItem : InquireEntity
    {
        protected UserProfileMember _userProfile;
        protected EntityItemList<EIVOEntityDataContext, CDS_Document> itemList;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            //btnPrint.PrintControls.Add(itemList);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
           // btnPrint.BeforeClick += new EventHandler(btnPrint_BeforeClick);
          
        }
 
        //void btnPrint_BeforeClick(object sender, EventArgs e)
        //{
        //    itemList.AllowPaging = false;
        //    buildQueryItem();
        //}
        
        protected override UserControl _itemList
        {
            get { return itemList; }
        }

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceItem, bool>> queryExpr = i => i.InvoiceCancellation == null;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            String no = this.txtInvoiceNO.Text.Trim();
            if (!String.IsNullOrEmpty(no))
            {
                if (no.Length == 10)
                {
                    String trackCode = no.Substring(0, 2);
                    no = no.Substring(2);
                    queryExpr = queryExpr.And(i => i.TrackCode == trackCode && i.No == no);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.No == no);
                }
            }

            if (!String.IsNullOrEmpty(this.txtReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.InvoiceBuyer.ReceiptNo.Equals(this.txtReceiptNo.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddlAttach.SelectedValue))
            {
                if (this.ddlAttach.SelectedValue.Equals("0"))
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.Attachment.Count() > 0);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.Attachment.Count() <= 0);
                }
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
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID&&r.BusinessMaster.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue))
                                    .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                            else
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                            //if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            //    invoices = invoices.Where(i => i.InvoiceBuyer.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));

                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                                invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));
                        }
                    }
                    else
                    {
                        invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i)
                            .Concat(invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i));
                        if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue)||
                                i.InvoiceBuyer.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue ));
                        //不嚴謹需修改
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r =>  r.BusinessMaster.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue))
                                .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                              }
                            else
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                            //if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            //    invoices = invoices.Where(i => i.InvoiceBuyer.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));

                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                                invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));

                        }
                    }
                    else
                    {

                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                                invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));

                        
                    }
                }
                
                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID);
                }
                else
                {
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice)
                        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID); ;
                }
            };

            if (itemList.Select().Count() > 0)
            {
                itemList.FindControl("tblAction").Visible = true;
            }

        }

        protected void rbChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Server.Transfer(rbChange.SelectedValue);
        }

    }    
}
