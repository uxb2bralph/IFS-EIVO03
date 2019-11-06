using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;
using System.ComponentModel;
using Model.Locale;

namespace eIVOGo.Module.EIVO
{
    public partial class B2CAllowancePrintView : AllowancePrintView
    {
        protected override void AllowancePrintView_PreRender(object sender, EventArgs e)
        {
            if (AllowanceID.HasValue)
            {
                var mgr = dsEntity.CreateDataManager();
                _item = mgr.GetTable<InvoiceAllowance>().Where(i => i.AllowanceID == AllowanceID).FirstOrDefault();
                this.DataBind();
            }

        }

    }
}