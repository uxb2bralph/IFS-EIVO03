using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.Locale;

namespace eIVOGo.Module.EIVO
{
    public partial class AllowancePrintViewB2B : eIVOGo.Module.EIVO.AllowancePrintView
    {

        protected override void AllowancePrintView_PreRender(object sender, EventArgs e)
        {
            if (AllowanceID.HasValue)
            {
                var mgr = dsEntity.CreateDataManager();
                _item = mgr.GetTable<InvoiceAllowance>().Where(i => i.AllowanceID == AllowanceID).First();

                part3.Item = _item;
                part4.Item = _item;

                this.DataBind();
            }
        }
    }
}