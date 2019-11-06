<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>

<%  if (_viewModel != null)
    {%>
    <%  if (_viewModel.ProcessType == Naming.InvoiceProcessType.C0401)
        { %>
<button type="button" class="btn" name="paperStyle" value="A4" onclick="uiInvoiceQuery.print('A4');">A4格式列印</button>
(列印兌獎訊息：<input name="printBack" type="radio" value="true" />是 
                <input name="printBack" type="radio" value="false" />否)
                <input type="button" class="btn" name="btnPrint" value="熱感紙規格列印" onclick="uiInvoiceQuery.print('POS');" />
(列印買受人地址：<input name="printBuyerAddr" type="radio" value="true" />是 
                <input name="printBuyerAddr" type="radio" value="false" />否)
    <%  }
        else if (_viewModel.ProcessType == Naming.InvoiceProcessType.A0401)
        {  %>
<button type="button" class="btn" name="paperStyle" value="A4" onclick="uiInvoiceQuery.print('A4',<%= (int)Naming.InvoiceProcessType.A0401 %>);">A4格式列印</button>
    <%  } %>
<%  } %>

<script runat="server">

    InquireInvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = ViewBag.ViewModel as InquireInvoiceViewModel;
    }

</script>

