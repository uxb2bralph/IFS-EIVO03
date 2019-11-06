<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.Base.InvoiceItemList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/EIVO/Action/InvoiceItemCommonView.ascx" TagName="InvoiceItemCommonView"
    TagPrefix="uc6" %>
<%@ Register Src="~/Module/Entity/OrganizationItem.ascx" TagName="OrganizationItem" TagPrefix="uc3" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True">
        <Columns>
            <asp:TemplateField HeaderText="日期">
                <ItemTemplate>
                    <%# ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).InvoiceItem.InvoiceDate)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="發票類別">
                <ItemTemplate>
                    <%# (((CDS_Document)Container.DataItem).InvoiceItem.InvoiceSeller.Organization.EnterpriseGroupMember.Count > 0 & ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceBuyer.Organization.EnterpriseGroupMember.Count > 0) ? "" : ((CDS_Document)Container.DataItem).InvoiceItem.CheckBusinessType(dsEntity.CreateDataManager(), _userProfile.CurrentUserRole.OrganizationCategory.CompanyID).ToString()%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="集團成員">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceSeller.Organization.EnterpriseGroupMember.Count>0 ? ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceSeller.CustomerName : null  %> <%# ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceBuyer.Organization.EnterpriseGroupMember.Count > 0 ? ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceBuyer.CustomerName : null%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="開立發票營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).InvoiceItem.InvoiceSeller.SellerID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceSeller.CustomerName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收發票營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).InvoiceItem.InvoiceBuyer.BuyerID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceBuyer.CustomerName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="發票號碼">
                <ItemTemplate>
                    <a href="#" onclick="<%# getInvoiceItemView((CDS_Document)Container.DataItem) %>">
                        <%# ((CDS_Document)Container.DataItem).InvoiceItem.TrackCode + ((CDS_Document)Container.DataItem).InvoiceItem.No %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="未稅金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceAmountType.SalesAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="稅 額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceAmountType.TaxAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
                <FooterTemplate>
                    共<%# _totalRecordCount %>筆</FooterTemplate>
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).InvoiceItem.InvoiceAmountType.TotalAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="對帳單" FooterText="總計金額">
                <ItemTemplate>
                    <a href="#" onclick="<%# getStatement((CDS_Document)Container.DataItem) %>">
                        <asp:Image ID="Image1" BorderStyle="None" ImageUrl="~/images/icon_ca.gif" runat="server" Visible="<%# ((CDS_Document)Container.DataItem).Attachment.Count<=0 ? false : true %>" />
                    </a>
                </ItemTemplate>                
                <ItemStyle HorizontalAlign="Center" />
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
       
            <asp:TemplateField HeaderText="下載狀態">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).DocumentDownloadLogs.Any(l => l.TypeID == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice) ? "已下載" : "未下載"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    <%# String.Format("{0:##,###,###,##0.##}", _subtotal) %></FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="下載日期">
                <ItemTemplate>
                    <%#  ((CDS_Document)Container.DataItem).DocumentDownloadLogs.Any(l => l.TypeID == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice) ? 
                    ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).DocumentDownloadLogs.OrderByDescending(d=>d.DownloadDate).Select(d=>d.DownloadDate).FirstOrDefault()):" "%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
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
<uc1:ActionHandler ID="doDisplayCompany" runat="server" />
<uc1:ActionHandler ID="doDisplayStatement" runat="server" />
<uc6:InvoiceItemCommonView ID="printInvoice" runat="server" />
<uc3:OrganizationItem ID="orgView" runat="server" Visible="false" />
<script runat="server">

    private Model.Security.MembershipManagement.UserProfileMember _userProfile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        doDisplayInvoice.DoAction = arg =>
        {
            printInvoice.QueryExpr = i => i.InvoiceID == int.Parse(arg);
            printInvoice.BindData();
        };

        doDisplayCompany.DoAction = arg =>
            {
                orgView.QueryExpr = o => o.CompanyID == int.Parse(arg);
                orgView.BindData();
            };

        doDisplayStatement.DoAction = arg =>
            {
                //Response.WriteFileAsDownload(arg, String.Format("{0:yyyy-MM-dd}.pdf", DateTime.Today), false);
                Response.WriteFileAsDownload(arg);
            };

        _userProfile = Business.Helper.WebPageUtility.UserProfile;
    }

    private String getInvoiceItemView(CDS_Document dataItem)
    {
        return dataItem.CurrentStep != (int)Naming.B2BInvoiceStepDefinition.待開立 ?
                                                            doDisplayInvoice.GetPostBackEventReference(dataItem.DocID.ToString())
                                        : "alert('尚未開立!!');return false;";
    }

    private String getStatement(CDS_Document dataItem)
    {
        return dataItem.Attachment.Count <= 0 ? "return false;"
            : File.Exists(dataItem.Attachment.FirstOrDefault().StoredPath) ? doDisplayStatement.GetPostBackEventReference(dataItem.Attachment.FirstOrDefault().StoredPath) : "alert('對帳單檔案遺失!!');return false;";
    }

    private System.Drawing.Color checkStatus(Naming.B2BInvoiceStepDefinition step)
    {
        switch (step)
        {
            case Naming.B2BInvoiceStepDefinition.待接收:
            case Naming.B2BInvoiceStepDefinition.待開立:
            case Naming.B2BInvoiceStepDefinition.待傳送:
                return System.Drawing.Color.Red;
            default:
                return System.Drawing.Color.Black;
        }
    }
</script>
