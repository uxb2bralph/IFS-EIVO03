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

namespace eIVOGo.Module.EIVO
{
    public partial class InvoiceItemPrintList : InvoiceItemCheckList
    {
        private UserProfileMember _userProfile;

        protected void btnShow_Click(object sender, EventArgs e)
        {
            String[] ar = GetItemSelection();
            if (ar!=null && ar.Count() > 0)
            {
                //_userProfile.EnqueueInvoicePrint(dsEntity.CreateDataManager(), ar.Select(a => int.Parse(a)));
                _userProfile.EnqueueDocumentPrint(dsEntity.CreateDataManager(), ar.Select(a => int.Parse(a)));
                //使用XPS列印
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
                    String.Format("window.open('{0}?printBack={1}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/PrintInvoicePage.aspx"), Request["printBack"])
                    , true);
                ////使用PDF列印
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
                //    String.Format("window.open('{0}?printBack={1}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/PrintInvoiceAsPDF.aspx"), Request["printBack"])
                //    , true);
                gvEntity.DataBind();
            }
            else
            {
                this.AjaxAlert("請選擇列印資料!!");
            }
        }

        protected void btnCSV_Click(object sender, EventArgs e)
        {
            String[] ar = GetItemSelection();
            if (ar != null && ar.Count() > 0)
            {
                string printData = string.Join(",", ar);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
                String.Format("window.open('{0}?printData={1}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/CreateCSV.aspx"), printData)
                , true);
                gvEntity.DataBind();
            }
            else
            {
                this.AjaxAlert("請選擇匯出資料!!");
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
            btnCSV.Visible = dsEntity.CurrentView.LastSelectArguments.TotalRowCount > 0;
        }
    }    
}