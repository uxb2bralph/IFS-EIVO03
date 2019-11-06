﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.Item.InvoiceProductPrintView" %>
<%@ Import Namespace="Model.DataEntity" %>
<%--<%# !String.IsNullOrEmpty(Item.Brief)? String.Format("<tr><td height=\"15\">{0}</td></tr>",Item.Brief) :""  %>--%>
<asp:Repeater ID="rpList" runat="server" EnableViewState="false">
    <ItemTemplate>
        <tr>
            <td height="15" valign="top">
                <%# ((InvoiceProductItem)Container.DataItem).No+1 %>.<%# Item.Brief %><%--<%# !String.IsNullOrEmpty(((InvoiceProductItem)Container.DataItem).Remark) ? "(備註:" + ((InvoiceProductItem)Container.DataItem).Remark + ")" : null %>--%>
            </td>
            <td align="right" valign="top">
                <%# String.Format("{0:#}",((InvoiceProductItem)Container.DataItem).Piece) %>
            </td>
            <td align="right" valign="top">
                <%# String.Format("{0:##,###,###,##0.##}", ((InvoiceProductItem)Container.DataItem).UnitCost) %>
            </td>
            <td align="right" valign="top">
                <%# String.Format("{0:##,###,###,##0.##}",((InvoiceProductItem)Container.DataItem).CostAmount) %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
