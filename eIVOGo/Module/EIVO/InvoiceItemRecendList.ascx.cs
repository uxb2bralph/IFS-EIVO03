using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using eIVOGo.Module.Base;
using eIVOGo.Helper;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using Business.Helper;
using System.Text;
using eIVOGo.Module.Common;

namespace eIVOGo.Module.EIVO
{
    public partial class InvoiceItemRecendList : InvoiceItemCheckList
    {
        private UserProfileMember _userProfile;

        protected void btnShow_Click(object sender, EventArgs e)
        {
            String[] ar = GetItemSelection();
            if (ar!=null && ar.Count() > 0)
            {
                List<int> forGoogle = new List<int>();
                List<int> NonGoogle = new List<int>();

                foreach (var id in ar.Select(a => int.Parse(a)))                
                {
                    var mgr = dsEntity.CreateDataManager();
                    var item = mgr.GetTable<CDS_Document>().Where(i => i.DocID == id).FirstOrDefault();
                    var Category = mgr.GetTable<OrganizationCategory>().Where(oc => oc.CompanyID == item.InvoiceItem.InvoiceSeller.SellerID).FirstOrDefault().CategoryID;
                    if (Category == (int)Naming.CategoryID.COMP_E_INVOICE_GOOGLE_TW)
                    {
                        forGoogle.Add(item.InvoiceItem.InvoiceID);
                    }
                    else
                    {
                        NonGoogle.Add(item.InvoiceItem.InvoiceID);
                    }
                    
                }
                if (forGoogle.Count > 0) forGoogle.SendGoogleInvoiceMail();
                if (NonGoogle.Count > 0) NonGoogle.SendB2CInvoiceMail("電子發票開立郵件通知");
                this.AjaxAlert("Email通知已重送!!");
                gvEntity.DataBind();
            }
            else
            {
                this.AjaxAlert("請選擇重送資料!!");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(InvoiceItemPrintList_PreRender);
            this.Load += new EventHandler(InvoiceItemPrintList_Load);
        }

        void InvoiceItemPrintList_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        void InvoiceItemPrintList_PreRender(object sender, EventArgs e)
        {
            btnShow.Visible = dsEntity.CurrentView.LastSelectArguments.TotalRowCount > 0;
        }
    }    
}