using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Locale;
using Model.Security.MembershipManagement;
using Model.InvoiceManagement;
using Business.Helper;
using Model.DataEntity;
using Uxnet.Web.Module.Common;
using System.Linq.Expressions;
using Utility;

namespace eIVOGo.Module.Base
{
    public partial class InvoiceAllowanceCheckList : InvoiceAllowanceList
    {

        public String[] GetItemSelection()
        {
            return Request.Form.GetValues("chkItem");
        }

    }    
}