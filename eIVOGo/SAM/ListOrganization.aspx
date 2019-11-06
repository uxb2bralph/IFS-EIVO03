<%@ Page Title="" Language="C#" EnableViewState="false" EnableEventValidation="false" %>

<%@ Register Src="~/Module/SAM/Business/OrganizationList.ascx" TagPrefix="uc1" TagName="OrganizationList" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagPrefix="uc1" TagName="FunctionTitleBar" %>
<%@ Register Src="~/Module/SAM/OrganizationManager.ascx" TagPrefix="uc1" TagName="OrganizationManager" %>

<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility"  %>
<form runat="server" id="theForm">
    <div id="result">
        <uc1:FunctionTitleBar runat="server" ID="resultTitle" ItemName="查詢結果" />
        <uc1:OrganizationList runat="server" ID="itemList" />
    </div>
</form>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ASP.module_sam_organizationmanager_ascx.BuildQuery(itemList);
    }
</script>
