<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Security.MembershipManagement" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Business.Helper" %>

    <table id="tblAction" width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td class="Bargain_btn">
                <%  Html.RenderPartial("~/Views/InvoiceProcess/ResultAction/PrintAction.ascx"); %>
                <input type="button" class="btn" name="btnPrint" value="Excel下載" onclick="uiInvoiceQuery.download();" style="display:none" />
                <%  if (_profile.IsSystemAdmin())
                    { %>
                <input type="button" class="btn" name="btnBuyer" value="下載買受人資料" onclick="uiInvoiceQuery.saveBuyer();" style="display:none"/>
                <%  } %>
            </td>
        </tr>
    </table>


<script runat="server">

    UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _profile = Context.GetUser();
    }

</script>

