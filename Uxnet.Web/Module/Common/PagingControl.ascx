﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagingControl.ascx.cs"
    Inherits="Uxnet.Web.Module.Common.PagingControl" %>
<table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" id="table-count">
    <tr>
        <td>
            <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
            <ItemTemplate>|
                <%# renderPageIndex((int)Container.DataItem)%>
            </ItemTemplate>
            </asp:Repeater>
            |&nbsp;
            <asp:LinkButton ID="lbnPrev" runat="server" Visible="False" href="#" EnableViewState="false">上頁</asp:LinkButton>
            |
            <asp:LinkButton ID="lbnNext" runat="server" Visible="False" href="#" EnableViewState="false">下頁</asp:LinkButton>
            <asp:TextBox ID="PageNum" runat="server" Columns="4" CssClass="textfield" EnableViewState="false"></asp:TextBox>
&nbsp;<input type="submit" value="頁數" class="btn" id="btnPost" />
&nbsp;</td>
        <td align="right" nowrap="nowrap">
            <asp:Label ID="lblSummary" runat="server" Visible="False" EnableViewState="false"></asp:Label>
        </td>
    </tr>
</table>
