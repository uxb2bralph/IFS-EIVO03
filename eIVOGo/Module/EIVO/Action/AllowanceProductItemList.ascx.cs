using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.DataEntity;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class AllowanceProductItemList : EntityItemList<EIVOEntityDataContext, InvoiceAllowanceDetail>
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected InvoiceItem loadItem(string InvoiceNO)
        {
            var mgr = dsEntity.CreateDataManager();
            return mgr.GetTable<InvoiceItem>().Where(i => (i.TrackCode.Trim() + i.No.Trim()).Equals(InvoiceNO.Trim())).FirstOrDefault();
        }
    }
}