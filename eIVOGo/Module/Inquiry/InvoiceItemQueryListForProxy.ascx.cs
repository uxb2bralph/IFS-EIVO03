using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Module.Common;
using Business.Helper;

namespace eIVOGo.Module.Inquiry
{
    public partial class InvoiceItemQueryListForProxy : InvoiceItemList
    {
        private UserProfileMember _userProfile;

        string idtype="";
        public String IDType
        {
            set { idtype = value; }
        }

        Boolean setdayrange=false;
        public Boolean hasDayRange
        {
            set { setdayrange = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(InvoiceItemQueryList_Load);
            this.PreRender += new EventHandler(InvoiceItemList_PreRender);
            this.PrintingButton21.BeforeClick += new EventHandler(PrintingButton21_BeforeClick);
            this.SaveAsExcelButton1.BeforeClick += new EventHandler(SaveAsExcelButton1_BeforeClick);
        }

        void SaveAsExcelButton1_BeforeClick(object sender, EventArgs e)
        {
            if (!setdayrange)
                QueryExpr = QueryExpr.And(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5));
            this.gvEntity.AllowPaging = false;            
        }

        void InvoiceItemQueryList_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            if (_userProfile.CurrentUserRole.OrganizationCategory.CategoryID == (int)Naming.B2CCategoryID.Google台灣)
            {
                this.gvEntity.Columns[1].Visible = true;
            }
            else if (_userProfile.CurrentUserRole.OrganizationCategory.CategoryID == (int)Naming.B2CCategoryID.商家發票自動配號)
            {
                this.gvEntity.Columns[1].Visible = true;
                this.gvEntity.Columns[1].HeaderText = "客戶ID";
            }
            else if (_userProfile.CurrentUserRole.OrganizationCategory.CategoryID == (int)Naming.B2CCategoryID.商家)
            {
                this.gvEntity.Columns[1].Visible = true;
                this.gvEntity.Columns[1].HeaderText = "客戶ID";
            }
            else if (_userProfile.CurrentUserRole.RoleID == ((int)Naming.RoleID.ROLE_SYS))
            {
                this.gvEntity.Columns[1].Visible = true;
                if (this.idtype.Equals("GoogleID"))
                    this.gvEntity.Columns[1].HeaderText = "GoogleID";
                else if (this.idtype.Equals("客戶ID"))
                    this.gvEntity.Columns[1].HeaderText = "客戶ID";
                else
                    this.gvEntity.Columns[1].HeaderText = "GoogleID/客戶ID";
            }

            this.PrintingButton21.btnPrint.Text = "資料列印";
            this.PrintingButton21.btnPrint.CssClass = "btn";
            this.PrintingButton21.PrintControls.Add(this.gvEntity);
            this.SaveAsExcelButton1.OutputFileName = "InvoiceReport.xls";
            this.SaveAsExcelButton1.DownloadControls.Add(this.gvEntity);
        }

        void InvoiceItemList_PreRender(object sender, EventArgs e)
        {
            PrintingButton21.Visible = dsEntity.CurrentView.LastSelectArguments.TotalRowCount > 0;
            SaveAsExcelButton1.Visible = dsEntity.CurrentView.LastSelectArguments.TotalRowCount > 0;            
        }

        void PrintingButton21_BeforeClick(object sender, EventArgs e)
        {
            this.gvEntity.AllowPaging = false;
        }
    }    
}