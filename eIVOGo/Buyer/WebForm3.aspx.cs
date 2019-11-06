using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.WebUI;

namespace MyInvoice.Buyer
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!this.IsPostBack)
            {
                lbClick.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(btnClick, ""));
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LinqDataSource1.DataSourceView.Selecting += new EventHandler<LinqDataSourceSelectEventArgs>(DataSourceView_Selecting);
        }

        void DataSourceView_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

        }

        //void LinqDataSource1_DataSourceViewCreated(object sender, DataAccessLayer.basis.LinqDataSourceViewEventArgs e)
        //{
        //    LinqDataSourceView view = e.DataSourceView;
        //}

        protected void btnClick_Click(object sender, EventArgs e)
        {
            WebMessageBox.Alert(Page, "Hello,World!!");
        }

        protected void LinqDataSource1_ContextCreating(object sender, LinqDataSourceContextEventArgs e)
        {

        }

        protected void LinqDataSource1_ContextCreated(object sender, LinqDataSourceStatusEventArgs e)
        {

        }

        protected void LinqDataSource1_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {

        }

        protected void LinqDataSource1_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

        }

        protected void LinqDataSource1_QueryCreated(object sender, QueryCreatedEventArgs e)
        {

        }
    }
}