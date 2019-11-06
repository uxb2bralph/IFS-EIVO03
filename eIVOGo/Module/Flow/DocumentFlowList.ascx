<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentFlowList.ascx.cs"
    Inherits="eIVOGo.Module.Flow.DocumentFlowList" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DocumentFlowManagement" TagPrefix="cc1" %>
<%@ Register Src="DocumentFlowItem.ascx" TagName="DocumentFlowItem" TagPrefix="uc3" %>
<%@ Register src="../Common/PageAnchor.ascx" tagname="PageAnchor" tagprefix="uc4" %>
<%@ Register src="../Common/DataModelCache.ascx" tagname="DataModelCache" tagprefix="uc5" %>
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
    GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
    DataSourceID="dsEntity" ShowFooter="True" DataKeyNames="FlowID">
    <Columns>
        <asp:TemplateField HeaderText="流程名稱" SortExpression="FlowName">
            <ItemTemplate>
                <%# Eval("FlowName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:Button ID="btnModify" runat="server" CausesValidation="False" CommandName="Modify"
                    Text="修改" CommandArgument='<%# String.Format("{0}",Eval(gvEntity.DataKeyNames[0])) %>'
                    OnClientClick='<%# doEdit.GetPostBackEventReference(String.Format("{0}",Eval(gvEntity.DataKeyNames[0]))) %>' />
                &nbsp;
                <asp:Button ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                    Text="刪除" CommandArgument='<%# String.Format("{0}",Eval(gvEntity.DataKeyNames[0])) %>'
                    OnClientClick='<%# doDelete.GetConfirmedPostBackEventReference("確認刪除此筆資料?", String.Format("{0}",Eval(gvEntity.DataKeyNames[0]))) %>' />
                &nbsp;<asp:Button ID="btnApply" runat="server" CausesValidation="False" CommandName="Modify"
                    Text="設定流程步驟" CommandArgument='<%# String.Format("{0}",Eval(gvEntity.DataKeyNames[0])) %>'
                    OnClientClick='<%# doApply.GetPostBackEventReference(String.Format("{0}",Eval(gvEntity.DataKeyNames[0]))) %>' />
            </ItemTemplate>
            <FooterTemplate>
                <asp:Button ID="btnCreate" runat="server" CausesValidation="False" CommandName="Create"
                    Text="新增流程" OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' /></FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <FooterStyle CssClass="total-count" />
    <EmptyDataTemplate>
        <asp:Button ID="btnCreate" runat="server" CausesValidation="False" Text="新增流程" OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' />
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
<cc1:DocumentFlowDataSource ID="dsEntity" runat="server">
</cc1:DocumentFlowDataSource>
<uc1:ActionHandler ID="doCreate" runat="server" />
<uc1:ActionHandler ID="doEdit" runat="server" />
<uc1:ActionHandler ID="doDelete" runat="server" />
<uc1:ActionHandler ID="doApply" runat="server" />
<uc3:DocumentFlowItem ID="editItem" runat="server" />
<uc4:PageAnchor ID="ApplyFlowControl" runat="server" TransferTo="~/SYS/ApplyFlowControl.aspx" />
<uc5:DataModelCache ID="modelItem" runat="server" KeyName="FlowID" />


