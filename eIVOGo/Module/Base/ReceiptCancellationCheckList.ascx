<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReceiptItemCheckList.ascx.cs"
    Inherits="eIVOGo.Module.Base.ReceiptItemList" %>
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
            <asp:TemplateField>
                <HeaderTemplate>
                    全選<input id="chkAll" name="chkAll" type="checkbox" onclick="$('input[id$=\'chkItem\']').attr('checked',$('input[id$=\'chkAll\']').is(':checked'));" />
                </HeaderTemplate>
                <ItemTemplate>
                    <input id="chkItem" name="chkItem" type="checkbox" value='<%# ((CDS_Document)Container.DataItem).DocID  %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="日期">
                <ItemTemplate>
                    <%# ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.ReceiptDate)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="開立收據營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.Seller.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.Seller.CompanyName%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收收據營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.Buyer.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.Buyer.CompanyName%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據號碼">
                <ItemTemplate>
                    <a href="#" onclick="<%# getInvoiceItemView((CDS_Document)Container.DataItem) %>">
                        <%# ((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.No%></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    共<%# _totalRecordCount %>筆</FooterTemplate>
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="對帳單">
                <ItemTemplate>
                    <a href="#" onclick="<%# getStatement((CDS_Document)Container.DataItem) %>">
                        <asp:Image ID="Image1" BorderStyle="None" ImageUrl="~/images/icon_ca.gif" runat="server" Visible="<%# ((CDS_Document)Container.DataItem).Attachment.Count<=0 ? false : true %>" />
                    </a>
                </ItemTemplate>                
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="金額" FooterText="總計金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).DerivedDocument.ParentDocument.ReceiptItem.TotalAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="作廢狀態">
                <ItemTemplate>
                    <asp:Label ID="status" runat="server" Text="<%# checkStatus((Naming.B2BInvoiceStepDefinition)((CDS_Document)Container.DataItem).CurrentStep) %>"
                        EnableViewState="false" ></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    <%# String.Format("{0:##,###,###,##0.##}", this.Select().Sum(d=>d.DerivedDocument.ParentDocument.ReceiptItem.TotalAmount)) %></FooterTemplate>
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
<uc1:ActionHandler ID="doDisplayStatement" runat="server" />
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

        doDisplayStatement.DoAction = arg =>
        {
            //Response.WriteFileAsDownload(arg, String.Format("{0:yyyy-MM-dd}.pdf", DateTime.Today), false);
            Response.WriteFileAsDownload(arg);
        };

        _userProfile = Business.Helper.WebPageUtility.UserProfile;
    }

    private String getInvoiceItemView(CDS_Document dataItem)
    {
        //return dataItem.CurrentStep != (int)Naming.B2BInvoiceStepDefinition.待開立 ?
        //                                                    doDisplayReceipt.GetPostBackEventReference(dataItem.DerivedDocument.SourceID.ToString())
        //                                : "alert('尚未開立!!');return false;";
        return doDisplayReceipt.GetPostBackEventReference(dataItem.DerivedDocument.SourceID.ToString());
    }

    private String getStatement(CDS_Document dataItem)
    {
        return dataItem.Attachment.Count <= 0 ? "return false;"
            : File.Exists(dataItem.Attachment.FirstOrDefault().StoredPath) ? doDisplayStatement.GetPostBackEventReference(dataItem.Attachment.FirstOrDefault().StoredPath) : "alert('對帳單檔案遺失!!');return false;";
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
