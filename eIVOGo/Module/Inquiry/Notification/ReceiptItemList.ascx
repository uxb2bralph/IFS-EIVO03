<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.Base.ReceiptItemList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/EIVO/Action/ReceiptItemCommonView.ascx" TagName="ReceiptItemCommonView" TagPrefix="uc6" %>
<%@ Register Src="~/Module/Entity/OrganizationItem.ascx" TagName="OrganizationItem" TagPrefix="uc3" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True">
        <Columns>
            <asp:TemplateField HeaderText="日期">
                <ItemTemplate>
                    <%# ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).ReceiptItem.ReceiptDate)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="開立收據營業人">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).ReceiptItem.Seller.CompanyName %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收收據營業人">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).ReceiptItem.Buyer.CompanyName %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據號碼">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).ReceiptItem.No %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    共<%# _totalRecordCount %>筆</FooterTemplate>
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="金額" FooterText="總計金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).ReceiptItem.TotalAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據狀態">
                <ItemTemplate>
                    <%# (Naming.B2BInvoiceStepDefinition)((CDS_Document)Container.DataItem).CurrentStep %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    <%# String.Format("{0:##,###,###,##0.##}", this.Select().Sum(d => d.ReceiptItem.TotalAmount))%></FooterTemplate>
                <FooterStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
        <FooterStyle CssClass="total-count" />
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
</div>
<cc1:DocumentDataSource ID="dsEntity" runat="server">
</cc1:DocumentDataSource>
