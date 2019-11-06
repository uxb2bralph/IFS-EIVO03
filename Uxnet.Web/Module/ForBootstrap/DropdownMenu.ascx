<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownMenu.ascx.cs" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownMenu" %>
<%@ Register src="DropdownMainMenuBlock.ascx" tagname="DropdownMenuBlock" tagprefix="uc1" %>
<div class="dropdown">
<%--    <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown">
        Dropdown
                    <span class="caret"></span>
    </button>--%>
    <uc1:DropdownMenuBlock ID="menuBlock" runat="server" />
</div>
