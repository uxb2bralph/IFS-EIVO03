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
    public partial class InvoiceItemCommonView : EditEntityItemModal<EIVOEntityDataContext, InvoiceItem>
    {
        protected override void loadEntity()
        {
            base.loadEntity();
            if (_entity != null)
            {
                productItems.BuildQuery = table =>
                {
                    return table.Join(table.Context.GetTable<InvoiceProduct>()
                        .Join(table.Context.GetTable<InvoiceDetail>().Where(d => d.InvoiceID == _entity.InvoiceID),
                            p => p.ProductID, d => d.ProductID, (p, d) => p),
                        i => i.ProductID, p => p.ProductID, (i, p) => i);
                };
            }
        }
    }
}