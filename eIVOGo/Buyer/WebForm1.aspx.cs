using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.Module.Common;
using Model.InvoiceManagement;
using Uxnet.Web.WebUI;

namespace MyInvoice.Buyer
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litMsg.Text = DateTime.Now.ToString();
            btnPrint.PrintControls.Add(GridView1);
            signContext.Launcher = btnCA;
        }

        protected override void OnInit(EventArgs e)
        {
            GridView1.PreRender += new EventHandler(GridView1_PreRender);
            btnPrint.BeforeClick += new EventHandler(btnPrint_BeforeClick);
            signContext.BeforeSign += new EventHandler(signContext_BeforeSign);
        }

        void signContext_BeforeSign(object sender, EventArgs e)
        {
            signContext.DataToSign = dataToSign.Text;
        }

        void btnPrint_BeforeClick(object sender, EventArgs e)
        {
            GridView1.AllowPaging = false;
        }

        protected void PageIndexChanged(object source, PageChangedEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
        }

        private void bindData()
        {
            using (InvoiceManager mgr = new InvoiceManager())
            {
                if (GridView1.AllowPaging)
                {
                    GridView1.PageIndex = PagingControl.GetCurrentPageIndex(GridView1, 0);
                    GridView1.DataSource = mgr.EntityList;
                    GridView1.DataBind();

                    PagingControl paging = (PagingControl)GridView1.BottomPagerRow.Cells[0].FindControl("pagingList");
                    paging.RecordCount = mgr.EntityList.Count();
                    paging.CurrentPageIndex = GridView1.PageIndex;
                }
                else
                {
                    GridView1.DataSource = mgr.EntityList;
                    GridView1.DataBind();
                }
            }
        }


        void GridView1_PreRender(object sender, EventArgs e)
        {
            bindData();
        }

        protected void btnHello_Click(object sender, EventArgs e)
        {
            btnHello.AjaxAlert("Hello!!");
        }

        protected void btnCA_Click(object sender, EventArgs e)
        {
            this.AjaxAlert(signContext.Verify().ToString());
        }
    }
}