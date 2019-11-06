<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentFlowControlList.ascx.cs"
    Inherits="eIVOGo.Module.Flow.DocumentFlowControlList" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DocumentFlowManagement" TagPrefix="cc1" %>
<%@ Register Src="DocumentFlowControlItem.ascx" TagName="DocumentFlowControlItem" TagPrefix="uc3" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Register src="../Common/DataModelCache.ascx" tagname="DataModelCache" tagprefix="uc6" %>
<div>流程名稱:<%# _item.FlowName %></div>
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
    GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
    DataSourceID="dsEntity" ShowFooter="True" DataKeyNames="StepID">
    <Columns>
        <asp:TemplateField HeaderText="起始步驟" SortExpression="PrevStep">
            <ItemTemplate>
                <%# _item != null && _item.InitialStep == ((DocumentFlowControl)Container.DataItem).StepID ? "◎" : ""%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="上一步" SortExpression="PrevStep">
            <ItemTemplate>
                <%# ((DocumentFlowControl)Container.DataItem).PrevStep.HasValue ? ((DocumentFlowControl)Container.DataItem).PrevStepItem.LevelExpression.Expression : null%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="流程狀態" SortExpression="LevelID">
            <EditItemTemplate>
            </EditItemTemplate>
            <ItemTemplate>
                <%# ((Naming.B2BInvoiceStepDefinition)Eval("LevelID")).ToString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="下一步" SortExpression="NextStep">
            <ItemTemplate>
                <%# ((DocumentFlowControl)Container.DataItem).NextStep.HasValue ? ((DocumentFlowControl)Container.DataItem).NextStepItem.LevelExpression.Expression : null%>
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
                &nbsp;
                <asp:Button ID="btnBranch" runat="server" CausesValidation="False" CommandName="Modify"
                    Text="設定Branch" CommandArgument='<%# String.Format("{0}",Eval(gvEntity.DataKeyNames[0])) %>'
                    OnClientClick='<%# doBranch.GetPostBackEventReference(String.Format("{0}",Eval(gvEntity.DataKeyNames[0]))) %>' />
            </ItemTemplate>
            <FooterTemplate>
                <asp:Button ID="btnCreate" runat="server" CausesValidation="False" CommandName="Create"
                    Text="建立流程步驟" OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' /></FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <FooterStyle CssClass="total-count" />
    <EmptyDataTemplate>
        <asp:Button ID="btnCreate" runat="server" CausesValidation="False" Text="建立流程步驟"
            OnClientClick='<%# doCreate.GetPostBackEventReference(null) %>' />
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
<cc1:DocumentFlowControlDataSource ID="dsEntity" runat="server">
</cc1:DocumentFlowControlDataSource>
<uc1:ActionHandler ID="doCreate" runat="server" />
<uc1:ActionHandler ID="doEdit" runat="server" />
<uc1:ActionHandler ID="doDelete" runat="server" />
<uc1:ActionHandler ID="doBranch" runat="server" />
<uc3:DocumentFlowControlItem ID="editItem" runat="server" />
<uc6:DataModelCache ID="modelItem" runat="server" KeyName="FlowID" />
<uc6:DataModelCache ID="StepID" runat="server" KeyName="StepID" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += new EventHandler(module_flow_documentflowcontrollist_ascx_PreRender);
        doBranch.DoAction = arg =>
            {
                StepID.DataItem = int.Parse(arg);
                Server.Transfer("ApplyFlowBranch.aspx");
            };
    }

    void module_flow_documentflowcontrollist_ascx_PreRender(object sender, EventArgs e)
    {
        if (_item != null)
        {
            this.DataBind();
        }
    }
</script>
