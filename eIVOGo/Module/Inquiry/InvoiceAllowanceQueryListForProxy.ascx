﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceAllowanceQueryListForProxy.ascx.cs"
    Inherits="eIVOGo.Module.Inquiry.InvoiceAllowanceQueryListForProxy" %>
<%@ Register Src="~/Module/Common/SaveAsExcelButton.ascx" TagName="SaveAsExcelButton" TagPrefix="uc1" %>
<%@ Register Src="../Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register src="../Common/PrintingButton2.ascx" tagname="PrintingButton2" tagprefix="uc3" %>
<%@ Register Src="../EIVO/PNewInvalidInvoicePreview.ascx" TagName="PNewInvalidInvoicePreview"
    TagPrefix="uc4" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Utility" %>
<!--表格 開始-->
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
    GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" ClientIDMode="Static"
    EnableViewState="False" ShowFooter="True" DataSourceID="dsEntity">
    <Columns>
        <asp:TemplateField HeaderText="日期">
            <ItemTemplate>
                <%# ValueValidity.ConvertChineseDateString(((InvoiceAllowance)Container.DataItem).AllowanceDate)%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="GoogleID" Visible="false" > 
            <ItemTemplate><%#((InvoiceAllowance)Container.DataItem).InvoiceAllowanceBuyer.CustomerID%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
<%--        <asp:TemplateField HeaderText="序號"> 
            <ItemTemplate><%# ((InvoiceItem)Container.DataItem).InvoicePurchaseOrder == null ? "" : ((InvoiceItem)Container.DataItem).InvoicePurchaseOrder.OrderNo%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>--%>
        <asp:TemplateField HeaderText="代理發票開立人">
            <ItemTemplate>
                <%# "("+((InvoiceAllowance)Container.DataItem).CDS_Document.DocumentOwner.Organization.ReceiptNo +")" +
                ((InvoiceAllowance)Container.DataItem).CDS_Document.DocumentOwner.Organization.CompanyName%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="開立發票營業人">
            <ItemTemplate>
                <%# ((InvoiceAllowance)Container.DataItem).InvoiceAllowanceSeller.Organization.CompanyName%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="統編">
            <ItemTemplate>
                <%# ((InvoiceAllowance)Container.DataItem).InvoiceAllowanceSeller.Organization.ReceiptNo%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="折讓號碼">
            <ItemTemplate>
                <asp:LinkButton ID="lbtn" runat="server" Text='<%# ((InvoiceAllowance)Container.DataItem).AllowanceNumber %>'
                    CausesValidation="false" CommandName="Edit" OnClientClick='<%# Page.ClientScript.GetPostBackEventReference(this, String.Format("S:{0}",((InvoiceAllowance)Container.DataItem).AllowanceID)) + "; return false;" %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="未稅金額" > 
            <ItemTemplate><%#String.Format("{0:0,0.00}", (((InvoiceAllowance)Container.DataItem).TotalAmount - ((InvoiceAllowance)Container.DataItem).TaxAmount))%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField> 
        <asp:TemplateField HeaderText="稅額" > 
            <ItemTemplate><%#String.Format("{0:0,0.00}", ((InvoiceAllowance)Container.DataItem).TaxAmount)%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center" />
            <FooterTemplate>
                共<%# _totalRecordCount %>筆</FooterTemplate>
        </asp:TemplateField> 
        <asp:TemplateField HeaderText="含稅金額"  FooterText="總計金額："> 
            <ItemTemplate><%#String.Format("{0:0,0.00}", ((InvoiceAllowance)Container.DataItem).TotalAmount)%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center" />
            <FooterStyle HorizontalAlign="Right" />
        </asp:TemplateField> 
        <asp:TemplateField HeaderText="買受人統編" > 
            <ItemTemplate><%# ((InvoiceAllowance)Container.DataItem).InvoiceAllowanceBuyer.ReceiptNo.Equals("0000000000") ? "N/A" : ((InvoiceAllowance)Container.DataItem).InvoiceAllowanceBuyer.ReceiptNo%></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center"  />
            <FooterTemplate>
                <%# String.Format("{0:##,###,###,##0}", _subtotal) %></FooterTemplate>
        </asp:TemplateField>               
        <asp:TemplateField HeaderText="備註" > 
            <ItemTemplate></ItemTemplate>  
            <ItemStyle HorizontalAlign="Center"  />
        </asp:TemplateField>
    </Columns>
    <FooterStyle />
    <EmptyDataTemplate>
        <%# doEmptyDataHandler() %>
    </EmptyDataTemplate>
    <PagerStyle HorizontalAlign="Center" />
    <SelectedRowStyle />
    <HeaderStyle />
    <AlternatingRowStyle CssClass="OldLace" />
    <PagerTemplate>
        <uc2:PagingControl ID="pagingList" runat="server" OnPageIndexChanged="PageIndexChanged" />
    </PagerTemplate>
    <RowStyle />
    <EditRowStyle />
</asp:GridView>
<center>
    <asp:Label ID="lblError" Visible="false" ForeColor="Red" Font-Size="Larger" runat="server" Text="查無資料!!"
        EnableViewState="false"></asp:Label>
</center>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn">
            <uc3:PrintingButton2 ID="PrintingButton21" runat="server" Visible="false" />&nbsp;&nbsp;
            <uc1:SaveAsExcelButton ID="SaveAsExcelButton1" runat="server" Visible="false" clientClick="return confirm('預設報表區間為前五個月!!\n若有其他條件請於查詢條件設定!!\n是否繼續下載??');" />
        </td>
    </tr>
</table>
<cc1:AllowanceDataSource ID="dsEntity" runat="server">
</cc1:AllowanceDataSource>
<uc4:PNewInvalidInvoicePreview ID="PNewInvalidInvoicePreview1" runat="server" />
