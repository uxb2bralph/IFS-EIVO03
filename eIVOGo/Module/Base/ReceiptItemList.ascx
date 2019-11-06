<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReceiptItemList.ascx.cs"
    Inherits="eIVOGo.Module.Base.ReceiptItemList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
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
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).ReceiptItem.Seller.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.Seller.CompanyName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收收據營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).ReceiptItem.Buyer.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.Buyer.CompanyName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據號碼">
                <ItemTemplate>
                    <a href="#" onclick="<%# getInvoiceItemView((CDS_Document)Container.DataItem) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.No %></a>
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
                    <asp:Label ID="status" runat="server" Text="<%# checkStatus((Naming.B2BInvoiceStepDefinition)((CDS_Document)Container.DataItem).ReceiptItem.CDS_Document.CurrentStep) %>"
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
<uc1:ActionHandler ID="doDisplayReceipt" runat="server" />
<uc1:ActionHandler ID="doDisplayCompany" runat="server" />
<uc6:ReceiptItemCommonView  ID="printReceipt" runat="server" />
<uc3:OrganizationItem ID="orgView" runat="server" Visible="false" />
<script runat="server">

    private Model.Security.MembershipManagement.UserProfileMember _userProfile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        doDisplayReceipt.DoAction = arg =>
        {
            printReceipt.QueryExpr = i => i.ReceiptID == int.Parse(arg);
            printReceipt.BindData();
        };

        doDisplayCompany.DoAction = arg =>
            {
                orgView.QueryExpr = o => o.CompanyID == int.Parse(arg);
                orgView.BindData();
            };

        _userProfile = Business.Helper.WebPageUtility.UserProfile;
    }

    private String getInvoiceItemView(CDS_Document dataItem)
    {
        return dataItem.CurrentStep != (int)Naming.B2BInvoiceStepDefinition.待開立 ?
                                                            doDisplayReceipt.GetPostBackEventReference(dataItem.DocID.ToString())
                                        : "alert('尚未開立!!');return false;";
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
                return "已接收";
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
