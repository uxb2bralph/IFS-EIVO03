<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.Item.InvoiceProductPrintView" %>
<%@ Import Namespace="Model.DataEntity" %>
<%--<%# !String.IsNullOrEmpty(Item.Brief)? String.Format("<tr><td height=\"15\">{0}</td></tr>",Item.Brief) :""  %>--%>
<asp:Repeater ID="rpList" runat="server" EnableViewState="false">
    <ItemTemplate>
        <tr valign="top">
            <td align="center">
                <%# ((InvoiceProductItem)Container.DataItem).ItemNo %>
            </td>
            <td width="172">
                <%# Item.Brief %><%# ((InvoiceProductItem)Container.DataItem).Spec %>
            </td>
            <td width="52" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).Piece %>
            </td>
            <td width="66" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).Weight %>
            </td>
            <td width="61" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).UnitFreight %>
            </td>
            <td width="77" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).UnitCost %>
            </td>
            <td width="88" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).FreightAmount %>
            </td>
            <td width="101" align="right">
                <%# ((InvoiceProductItem)Container.DataItem).CostAmount %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
