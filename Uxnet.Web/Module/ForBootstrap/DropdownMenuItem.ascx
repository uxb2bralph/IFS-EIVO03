<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownMenuItem.ascx.cs" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownMenuItem" %>
<%@ Import Namespace="Utility" %>
<li><a href='<%# String.IsNullOrEmpty(_dataItem.url) ? "#" : VirtualPathUtility.ToAbsolute(_dataItem.url) %>'><%# _dataItem.value %></a></li>