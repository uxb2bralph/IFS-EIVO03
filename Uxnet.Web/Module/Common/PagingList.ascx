<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagingList.ascx.cs" Inherits="Uxnet.Web.Module.Common.PagingList" %>
<asp:LinkButton id="lbnPrev" runat="server" Visible="False" onclick="lbnPrev_Click">上頁</asp:LinkButton>
<asp:DropDownList id="dlPage" runat="server" CssClass="inputText" AutoPostBack="True" onselectedindexchanged="dlPage_SelectedIndexChanged"></asp:DropDownList>
<asp:LinkButton id="lbnNext" runat="server" Visible="False" onclick="lbnNext_Click">下頁</asp:LinkButton>
<asp:Label id="lblSummary" runat="server" Visible="False"></asp:Label>
