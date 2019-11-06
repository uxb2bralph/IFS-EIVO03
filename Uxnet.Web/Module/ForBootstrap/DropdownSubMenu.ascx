<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownSubMenu.ascx.cs" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownSubMenu" %>
<li class="dropdown-submenu">
    <a tabindex="-1" href="#"><%# _dataItem.value %></a>
    <asp:PlaceHolder ID="subMenu" runat="server" EnableViewState="false"></asp:PlaceHolder>
</li>