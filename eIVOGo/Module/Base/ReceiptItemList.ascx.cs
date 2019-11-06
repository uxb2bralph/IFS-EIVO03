using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.DataEntity;

namespace eIVOGo.Module.Base
{
    public partial class ReceiptItemList : EntityItemList<EIVOEntityDataContext, CDS_Document>
    {
        protected int? _totalRecordCount;
        protected decimal? _subtotal;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void dsEntity_Select(object sender, DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<CDS_Document> e)
        {
            base.dsEntity_Select(sender, e);
            _totalRecordCount = e.Query.Count();
            _subtotal = e.Query.Select(d => d.ReceiptItem).Sum(i => i.TotalAmount);
        }
    }
}