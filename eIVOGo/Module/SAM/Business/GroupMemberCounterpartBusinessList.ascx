<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupMemberCounterpartBusinessList.ascx.cs"
    Inherits="eIVOGo.Module.SAM.Business.GroupMemberCounterpartBusinessList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True" 
        DataKeyNames="MasterID,RelativeID,BusinessID"  >
        <Columns>
            <asp:TemplateField HeaderText="集團成員" SortExpression="Fax">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).BusinessMaster.CompanyName %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="相對營業人名稱" SortExpression="CompanyName">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.CompanyName%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="統一編號" SortExpression="ReceiptNo">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).Counterpart.ReceiptNo%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="類別" SortExpression="">
                <ItemTemplate>
                    <%# ((BusinessRelationship)Container.DataItem).BusinessType.Business  %>
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
                    <%# ((BusinessRelationship)Container.DataItem).CurrentLevel.HasValue ? ((BusinessRelationship)Container.DataItem).LevelExpression.Expression : "已啟用" %>
                    <br />
                    <%# ((BusinessRelationship)Container.DataItem).BusinessID == (int)Naming.InvoiceCenterBusinessType.銷項 && ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.Entrusting==true ? "自動接收" : null %> 
                    <br />
                    <%--((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus.HasValue ? ((Naming.MemberStatusDefinition)((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus).ToString():--%>
                    <%#((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint.HasValue ? (((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint == true ? "主動列印":"停用列印"):"未設定" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" HeaderText="管理">
                <ItemTemplate>
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                        Visible="<%# ((BusinessRelationship)Container.DataItem).CurrentLevel != (int)Naming.MemberStatusDefinition.Mark_To_Delete%>"
                        Text="停用" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doDelete.GetConfirmedPostBackEventReference("確認停用此筆資料?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />
                    <asp:Button ID="btnActivate" runat="server" CausesValidation="False" CommandName="Activate"
                        Visible="<%# ((BusinessRelationship)Container.DataItem).CurrentLevel == (int)Naming.MemberStatusDefinition.Mark_To_Delete%>"
                        Text="啟用" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doActivate.GetConfirmedPostBackEventReference("確認啟用此筆資料?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />
                    <br />
                    <asp:Button ID="btnDisableEntrusting" runat="server" CausesValidation="False" CommandName="DisableEntrusting"
                        Visible="<%# ((BusinessRelationship)Container.DataItem).BusinessID == (int)Naming.InvoiceCenterBusinessType.銷項 && ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.Entrusting==true %>"
                        Text="停用自動接收" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doDisableEntrusting.GetConfirmedPostBackEventReference("確認停用此營業人自動接收?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />
                    <asp:Button ID="btnEntrust" runat="server" CausesValidation="False" CommandName="Entrust"
                        Visible="<%# ((BusinessRelationship)Container.DataItem).BusinessID == (int)Naming.InvoiceCenterBusinessType.銷項 && ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.Entrusting!=true %>"
                        Text="設定自動接收" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doEntrust.GetConfirmedPostBackEventReference("確認啟用此營業人自動接收?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />
                    <br />

<%--                    ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus.HasValue ? ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus != (int)Naming.MemberStatusDefinition.停用列印 
                                   : --%>
                    <asp:Button ID="btnPrintOff" runat="server" CausesValidation="False" CommandName="PrintOff"
                        Visible="<%#((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint.HasValue ? (((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint == true ? true : false ): false %>"
                        Text="停用列印" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doPrintOff.GetConfirmedPostBackEventReference("確認停用列印?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />

<%--                    ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus.HasValue ? ((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.PrintStatus == (int)Naming.MemberStatusDefinition.停用列印 
                                  : --%>
                    <asp:Button ID="btnPrintOn" runat="server" CausesValidation="False" CommandName="PrintOn"
                        Visible="<%#((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint.HasValue ? (((BusinessRelationship)Container.DataItem).Counterpart.OrganizationStatus.EntrustToPrint == true ? false : true ): true %>"
                        Text="啟用列印" CommandArgument='<%# String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2])) %>'
                        OnClientClick='<%# doPrintOn.GetConfirmedPostBackEventReference("確認啟用主動列印?", String.Format("{0},{1},{2}",Eval(gvEntity.DataKeyNames[0]),Eval(gvEntity.DataKeyNames[1]),Eval(gvEntity.DataKeyNames[2]))) %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
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

<cc1:BusinessRelationshipDataSource ID="dsEntity" runat="server">
</cc1:BusinessRelationshipDataSource>

<uc1:ActionHandler ID="doDelete" runat="server" />
<uc1:ActionHandler ID="doActivate" runat="server" />
<uc1:ActionHandler ID="doEntrust" runat="server" />
<uc1:ActionHandler ID="doDisableEntrusting" runat="server" />
<uc1:ActionHandler ID="doPrintOff" runat="server" />
<uc1:ActionHandler ID="doPrintOn" runat="server" />
