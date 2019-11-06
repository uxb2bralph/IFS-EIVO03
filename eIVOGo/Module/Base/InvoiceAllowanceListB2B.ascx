<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceAllowanceListB2B.ascx.cs"
    Inherits="eIVOGo.Module.Base.InvoiceAllowanceListB2B" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/EIVO/Action/AllowanceItemCommonView.ascx" TagName="AllowanceItemCommonView"
    TagPrefix="uc6" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Entity/OrganizationItem.ascx" TagName="OrganizationItem"
    TagPrefix="uc3" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True">
        <Columns>
            <asp:TemplateField HeaderText="日期">
                <ItemTemplate>
                    <%# ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).InvoiceAllowance.AllowanceDate)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="集團成員">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceSeller.Organization.EnterpriseGroupMember.Count > 0 ? ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceSeller.CustomerName : null%> <%# ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceBuyer.Organization.EnterpriseGroupMember.Count > 0 ? ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceBuyer.CustomerName : null%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="開立發票折讓營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceBuyer.CustomerName%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收發票折讓營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceSeller.SellerID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).InvoiceAllowance.InvoiceAllowanceSeller.CustomerName%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="折讓單號碼">
                <ItemTemplate>
                    <a onclick="<%# doDisplayInvoice.GetPostBackEventReference(String.Format("{0}",((CDS_Document)Container.DataItem).DocID)) %>"
                        href="#">
                        <%# ((CDS_Document)Container.DataItem).InvoiceAllowance.AllowanceNumber%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="金額" FooterText="總計金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).InvoiceAllowance.TotalAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="折讓狀態">
                <ItemTemplate>
                    <asp:Label ID="status" runat="server" Text="<%# checkStatus((Naming.B2BInvoiceStepDefinition)((CDS_Document)Container.DataItem).InvoiceAllowance.CDS_Document.CurrentStep) %>"                    
                        EnableViewState="false" ></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    <%# String.Format("{0:##,###,###,##0.##}", _subtotal) %></FooterTemplate>
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
<uc1:ActionHandler ID="doDisplayInvoice" runat="server" />
<uc6:AllowanceItemCommonView ID="printAllowance" runat="server" />
<uc1:ActionHandler ID="doDisplayCompany" runat="server" />
<uc3:OrganizationItem ID="orgView" runat="server" Visible="false" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        doDisplayInvoice.DoAction = arg =>
        {
            printAllowance.QueryExpr = i => i.AllowanceID == int.Parse(arg);
            printAllowance.BindData();
        };

        doDisplayCompany.DoAction = arg =>
        {
            orgView.QueryExpr = o => o.CompanyID == int.Parse(arg);
            orgView.BindData();
        };
    }

    private string checkStatus(Naming.B2BInvoiceStepDefinition step)
    {
        switch (step)
        {
            case Naming.B2BInvoiceStepDefinition.待接收:
                return (Naming.B2BInvoiceStepDefinition.待接收).ToString();
            case Naming.B2BInvoiceStepDefinition.待開立:
                return (Naming.B2BInvoiceStepDefinition.待開立).ToString();
            case Naming.B2BInvoiceStepDefinition.待傳送:
                return "待接收";
            case Naming.B2BInvoiceStepDefinition.已接收:
                return (Naming.B2BInvoiceStepDefinition.已接收).ToString();
            case Naming.B2BInvoiceStepDefinition.已開立:
                return (Naming.B2BInvoiceStepDefinition.已開立).ToString();
            case Naming.B2BInvoiceStepDefinition.已傳送:
                return "已接收";
            default:
                return "";
        }
    }
</script>


