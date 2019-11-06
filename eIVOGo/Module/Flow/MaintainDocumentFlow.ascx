<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.template.ContentControlTemplate" %>
<%@ Register src="~/Module/UI/PageAction.ascx" tagname="PageAction" tagprefix="uc1" %>
<%@ Register src="~/Module/UI/FunctionTitleBar.ascx" tagname="FunctionTitleBar" tagprefix="uc2" %>

<%@ Register src="DocumentFlowControlList.ascx" tagname="DocumentFlowControlList" tagprefix="uc3" %>

<%@ Register src="DocumentFlowList.ascx" tagname="DocumentFlowList" tagprefix="uc4" %>



<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 建立發票傳送流程" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="建立發票傳送流程" />
<uc4:DocumentFlowList ID="flowList" runat="server" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        flowList.QueryExpr = f => true;
    }
</script>



