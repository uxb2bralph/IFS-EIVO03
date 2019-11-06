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
using Uxnet.Web.WebUI;
using eIVOGo.Helper;

namespace eIVOGo.Module.Inquiry.ForOP
{
    public partial class InquireInvoiceItemDownloadLog : InquireEntity
    {
        protected UserProfileMember _userProfile;
        protected EntityItemList<EIVOEntityDataContext, CDS_Document> itemList;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            
           
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
     
        }

    

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
            else
            {
                this.AjaxAlert("請選擇起始日期!");
                ResetQuery();
                return;
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            String Startno = this.txtInvoiceNO.Text.Trim();
            if (!String.IsNullOrEmpty(Startno))
            {
                if (Startno.Length == 10)
                {
                    String trackCode = Startno.Substring(0, 2);
                    int no = Convert.ToInt32(Startno.Substring(2));
                    queryExpr = queryExpr.And(i => i.TrackCode == trackCode && Convert.ToInt32(i.No) >= no);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.No == Startno);
                }
            }
            String Endno = this.txtInvoiceNOEnd.Text.Trim();
            if (!String.IsNullOrEmpty(Endno))
            {
                if (Endno.Length == 10)
                {
                    String trackCode = Endno.Substring(0, 2);
                    int no = Convert.ToInt32(Endno.Substring(2));
                    queryExpr = queryExpr.And(i => i.TrackCode == trackCode && Convert.ToInt32(i.No) <= no);
                }
                //else
                //{
                //    queryExpr = queryExpr.And(i => i.No == Endno);
                //}
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

            if (!String.IsNullOrEmpty(this.ddDownload.SelectedValue))
            {
                if (this.ddDownload.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.DocumentDownloadLogs.Any());
                }
                else
                {
                    queryExpr = queryExpr.And(i => !i.CDS_Document.DocumentDownloadLogs.Any());
                }
            }
            //if (!String.IsNullOrEmpty(this.EntrustToPrint.SelectedValue))
            //{
            //    if (this.EntrustToPrint.SelectedValue.Equals("1"))
            //    {
      queryExpr = queryExpr.And(i => (bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint);
            //    }
            //    else
            //    {
            //        queryExpr = queryExpr.And(i => !(bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint || !i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint.HasValue);
            //    }
            //}
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
                            invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
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
                            invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                    }
                }

                //if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                //{
                //    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                //        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID);
                //}
                //else
                //{
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice)
                        .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID); ;
                //}
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
                this.DownloadButton.Attributes.Clear();
                //this.DownloadButton.Attributes.Remove("onclick");
                this.DownloadButton.Attributes.Add("onclick", string.Format("{0}{1}{2}{3}", "event.returnValue=confirm('", "是否重送", itemList.Select().Count(), "筆資料?');"));
            }

        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            buildQueryItem();

            var mgr = dsEntity.CreateDataManager();
            foreach (var item in itemList.Select())
            {
                mgr.PrepareToDownload(item.InvoiceItem,false);
            }

            this.AjaxAlert("重送成功");
        }



    }    
}
