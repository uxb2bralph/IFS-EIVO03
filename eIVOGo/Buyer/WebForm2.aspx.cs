using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.Module.Common;

namespace MyInvoice.Buyer
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnPrint.PrintControls.Add(table01);
            btnPrint.PrintControls.Add(GridView1);
            btnPrint.PrintControls.Add(btnPrint);
            btnPrint.PrintControls.Add(new Calendar());
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dsInv.Select += new EventHandler<DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<Model.DataEntity.InvoiceItem>>(dsInv_Select);
            this.PreRender += new EventHandler(WebForm2_PreRender);
            btnPrint.BeforeClick += new EventHandler(btnPrint_BeforeClick);
        }

        void btnPrint_BeforeClick(object sender, EventArgs e)
        {
            GridView1.AllowPaging = false;
        }

        void WebForm2_PreRender(object sender, EventArgs e)
        {
            PagingControl paging = (PagingControl)GridView1.BottomPagerRow.Cells[0].FindControl("pagingList");
            paging.RecordCount = dsInv.CurrentView.LastSelectArguments.TotalRowCount;
            paging.CurrentPageIndex = GridView1.PageIndex;
        }

        void dsInv_Select(object sender, DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<Model.DataEntity.InvoiceItem> e)
        {

        }

        protected void PageIndexChanged(object source, PageChangedEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.DataBind();
        }
       
    }
}