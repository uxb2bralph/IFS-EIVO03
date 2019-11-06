<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.SAM.Business.GroupMemberCounterpartBusinessList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc4" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True" DataKeyNames="MasterID,RelativeID,BusinessID">
        <Columns>
            <asp:TemplateField HeaderText="集團成員" SortExpression="Fax">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).BusinessMaster.CompanyName %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="類別" SortExpression="">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).BusinessType.Business  %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="營業人名稱" SortExpression="CompanyName">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.CompanyName%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="統一編號" SortExpression="ReceiptNo">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.ReceiptNo%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="聯絡人電子郵件" SortExpression="ContactEmail">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.ContactEmail%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="地址" SortExpression="Addr">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.Addr%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="電話" SortExpression="Phone">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.Phone%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="狀態" SortExpression="">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.LevelExpression.Expression %>
                </ItemTemplate>
            </asp:TemplateField>
<%--            <asp:TemplateField HeaderText="" SortExpression="">
                <ItemTemplate>
                    <asp:Button ID="btnSignature" runat="server" CausesValidation="False" Visible="<%# ((BusinessRelationship)Container.DataItem).BusinessType.BusinessID == (int)Naming.InvoiceCenterBusinessType.進項 %>"
                        CommandName="Signature" Text="設定發票章" CommandArgument='<%# String.Format("{0}",Eval(gvEntity.DataKeyNames[0])) %>'
                        OnClientClick='<%# doInvoiceSignature.GetPostBackEventReference(String.Format("{0}",((BusinessRelationship)Container.DataItem).Counterpart.CompanyID)) %>' />
                </ItemTemplate>
            </asp:TemplateField>
--%>        </Columns>
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
<cc1:BusinessRelationshipDataSource ID="dsEntity" runat="server">
</cc1:BusinessRelationshipDataSource>
<uc4:DataModelCache ID="modelItem" runat="server" KeyName="CompanyID" />
<uc1:ActionHandler ID="doDelete" runat="server" />
<uc1:ActionHandler ID="doActivate" runat="server" />
<uc1:ActionHandler ID="doInvoiceSignature" runat="server" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        doInvoiceSignature.DoAction = arg =>
        {
            modelItem.DataItem = int.Parse(arg);
            Server.Transfer("~/SAM/ImportInvoiceSignature.aspx");
        };
    }
</script>
