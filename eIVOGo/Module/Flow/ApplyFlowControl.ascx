<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.template.ContentControlTemplate" %>
<%@ Register src="~/Module/UI/PageAction.ascx" tagname="PageAction" tagprefix="uc1" %>
<%@ Register src="~/Module/UI/FunctionTitleBar.ascx" tagname="FunctionTitleBar" tagprefix="uc2" %>

<%@ Register src="DocumentFlowControlList.ascx" tagname="DocumentFlowControlList" tagprefix="uc3" %>



<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 建立流程步驟" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="建立流程步驟" />

<uc3:DocumentFlowControlList ID="DocumentFlowControlList1" runat="server" />


