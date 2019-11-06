using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.Locale;
using Model.Security.MembershipManagement;
using Uxnet.Web.Module.DataModel;
using Uxnet.Web.WebUI;
using Model.DataEntity;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class AllowanceItemCommonView : EditEntityItemModal<EIVOEntityDataContext, InvoiceAllowance>
    {
        protected override void loadEntity()
        {
            base.loadEntity();
            if (_entity != null)
            {
                AllowanceProductItems.BuildQuery = table =>
                {
                    return table.Where(d=>d.AllowanceID==_entity.AllowanceID).Join(table.Context.GetTable<InvoiceAllowanceItem>(),
                        i => i.ItemID, p => p.ItemID, (i, p) => i);
                };
            }
        }
    }
}