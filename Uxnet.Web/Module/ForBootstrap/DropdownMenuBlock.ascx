<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownMenuBlock.ascx.cs" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownMenuBlock" %>
<%@ Import Namespace="Uxnet.Web.Module.SiteAction" %>
<%@ Import Namespace="Utility" %>
<ul class="dropdown-menu">
    <asp:Repeater ID="rpItem" runat="server" EnableViewState="false">
        <ItemTemplate>
        </ItemTemplate>
    </asp:Repeater>
</ul>
