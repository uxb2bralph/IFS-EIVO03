<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownMenuBlock.ascx.cs" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownMenuBlock" %>
<%@ Import Namespace="Uxnet.Web.Module.SiteAction" %>
<%@ Import Namespace="Utility" %>
<ul class="dropdown-menu" role="menu" aria-labelledby="dropdownMenu" style="margin-bottom: 5px; display: block; position: static;">
    <asp:Repeater ID="rpItem" runat="server" EnableViewState="false">
        <ItemTemplate>
        </ItemTemplate>
    </asp:Repeater>
</ul>