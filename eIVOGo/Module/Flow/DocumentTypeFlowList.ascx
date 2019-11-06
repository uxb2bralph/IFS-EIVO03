<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentTypeFlowList.ascx.cs"
    Inherits="eIVOGo.Module.Flow.DocumentTypeFlowList" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/PageAnchor.ascx" TagName="PageAnchor" TagPrefix="uc5" %>
<%@ Register Assembly="Model" Namespace="Model.DocumentFlowManagement" TagPrefix="cc1" %>
<%@ Import Namespace="Model.DocumentFlowManagement" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Register Src="DocumentTypeFlowItem.ascx" TagName="DocumentTypeFlowItem" TagPrefix="uc3" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True" 
        DataKeyNames="TypeID,FlowID,CompanyID,BusinessID">
        <Columns>
            <asp:TemplateField HeaderText="資料類型" SortExpression="TypeID">
                <ItemTemplate>
                    <%# ((Naming.B2BInvoiceDocumentTypeDefinition) ((DocumentTypeFlow)Container.DataItem).TypeID).ToString() %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="屬性" SortExpression="OrgaCateID">
                <ItemTemplate>
                    <%# ((DocumentTypeFlow)Container.DataItem).BusinessType.Business %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="公司名稱" SortExpression="OrgaCateID">
                <ItemTemplate>
                    <%# ((DocumentTypeFlow)Container.DataItem).Organization.CompanyName %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="適用流程" SortExpression="FlowID">
                <ItemTemplate>
                    <%# ((DocumentTypeFlow)Container.DataItem).DocumentFlow.FlowName %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" HeaderText="管理">
                <ItemTemplate>
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                        Text="刪除" CommandArgument='<%# String.Format("{0},{1},{2},{3}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]),Eval(gvEntity.DataKeyNames[3])) %>'
                        OnClientClick='<%# doDelete.GetConfirmedPostBackEventReference("確認刪除此筆資料?", String.Format("{0},{1},{2},{3}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]),Eval(gvEntity.DataKeyNames[3]))) %>' />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Button ID="btnCreate" runat="server" CausesValidation="False" CommandName="Create"
                        Text="新增套用流程" OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' /></FooterTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
        <FooterStyle CssClass="total-count" />
        <EmptyDataTemplate>
            <asp:Button ID="btnCreate" runat="server" CausesValidation="False" CommandName="Create"
                Text="新增套用流程" OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' />
        </EmptyDataTemplate>
        <EmptyDataRowStyle CssClass="noborder" />
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
<cc1:DocumentTypeFlowDataSource ID="dsEntity" runat="server">
</cc1:DocumentTypeFlowDataSource>
<uc1:ActionHandler ID="doCreate" runat="server" />
<uc1:ActionHandler ID="doDelete" runat="server" />
<uc4:DataModelCache ID="modelItem" runat="server" KeyName="DocumentTypeFlow" />
<uc3:DocumentTypeFlowItem ID="editItem" runat="server" Visible="false" />
