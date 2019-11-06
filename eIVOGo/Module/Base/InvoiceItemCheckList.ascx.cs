using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Module.Common;

namespace eIVOGo.Module.Base
{
    public partial class InvoiceItemCheckList : InvoiceItemList
    {
        public String[] GetItemSelection()
        {
            return Request.Form.GetValues("chkItem");
        }

        public int setPageSize
        {
            get { return this.gvEntity.PageSize; }
            set 
            { 
                this.gvEntity.PageSize = value;
            }
        }
    }    
}