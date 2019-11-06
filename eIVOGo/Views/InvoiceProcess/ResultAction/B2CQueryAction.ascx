<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Security.MembershipManagement" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

    <table id="tblAction" width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td class="Bargain_btn">
                <%  if (_profile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo != "0000000000")
                    { %>
                <button type="button" class="btn" name="paperStyle" value="A4" onclick="uiInvoiceQuery.print('A4');">A4格式列印</button>
<%--                (列印兌獎訊息：<input name="printBack" type="radio" value="true" />是 
                <input name="printBack" type="radio" value="false" />否)--%>
                <%  }
                    else
                    { %>
                <input type="button" class="btn" name="btnPrint" value="熱感紙規格列印" onclick="uiInvoiceQuery.print('POS');" />
<%--                (列印買受人地址：<input name="printBuyerAddr" type="radio" value="true" />是 
                <input name="printBuyerAddr" type="radio" value="false" />否)--%>
                <%  } %>
                <input type="button" class="btn" name="btnPrint" value="Excel下載" onclick="uiInvoiceQuery.download();" />
            </td>
        </tr>
    </table>


<script runat="server">

    ModelSource<InvoiceItem> models;
    UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;

        _profile = Context.GetUser();
    }

</script>

