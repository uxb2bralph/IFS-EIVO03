<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComposedFunctionItem.ascx.cs"
    Inherits="Uxnet.Web.Module.SiteAction.ComposedFunctionItem" %>
<%@ Register Src="FunctionItem.ascx" TagName="FunctionItem" TagPrefix="uc1" %>
<input type="checkbox" id="cbFunction" runat="server" /><asp:Literal ID="litItemName" runat="server"></asp:Literal>
<ul>
    <li>
        <uc1:FunctionItem ID="childItem" runat="server" />
    </li>
</ul>
