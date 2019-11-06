using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.DataEntity;
using Model.Locale;
using System.ComponentModel;
using System.Linq.Expressions;

namespace eIVOGo.Module.UI
{
    public partial class SellerSelector : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (SelectAll)
                    _Selector.Items.Add(new ListItem("全部", ""));
                var mgr = dsInv.CreateDataManager();
                IQueryable<Organization> orgItems = Filter != null ? mgr.GetTable<Organization>().Where(Filter) : mgr.GetTable<Organization>();
                _Selector.Items.AddRange(orgItems.Where(
                    o => o.OrganizationCategory.Any(
                        c => c.CategoryID == (int)Naming.CategoryID.COMP_E_INVOICE_B2C_SELLER || c.CategoryID == (int)Naming.CategoryID.COMP_VIRTUAL_CHANNEL))
                        .Select(o =>
                            new ListItem(String.Format("{0} {1}", o.ReceiptNo, o.CompanyName), o.CompanyID.ToString())).ToArray());
            }
        }

        [Bindable(true)]
        public bool SelectAll
        {
            get;
            set;
        }

        [Bindable(true)]
        public Expression<Func<Organization, bool>> Filter
        {
            get;set;
        }

        public DropDownList Selector
        {
            get
            {
                return _Selector;
            }
        }
    }
}