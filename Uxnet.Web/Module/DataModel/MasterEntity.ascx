<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MasterEntity.ascx.cs"
    Inherits="Uxnet.Web.Module.DataModel.MasterEntity" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="DetailEntity.ascx" TagName="DetailEntity" TagPrefix="uc1" %>
<style type="text/css">
    .modalBackground
    {
        background-color: Gray;
        filter: alpha(opacity=70);
        opacity: 0.7;
    }
    .modalPopup
    {
        background-color: #ffffdd;
        border-width: 3px;
        border-style: solid;
        border-color: Gray;
        padding: 3px;
        width: 640px;
    }
</style>
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" 
    AllowPaging="True" AllowSorting="True" OnSorting="gvEntity_Sorting"
    OnRowDeleting="gvEntity_RowDeleting" OnRowCommand="gvEntity_RowCommand" OnPreRender="gvEntity_PreRender"
    OnRowCancelingEdit="gvEntity_RowCancelingEdit" OnRowEditing="gvEntity_RowEditing"
    OnRowUpdating="gvEntity_RowUpdating" ShowFooter="True">
    <Columns>
        <asp:CommandField ButtonType="Button" HeaderText="Show Details" SelectText="檢視" 
            ShowCancelButton="False" ShowSelectButton="True" />
        <asp:TemplateField ShowHeader="False">
            <HeaderTemplate>
                代碼或名稱:<asp:TextBox runat="server" ID="myTextBox" autocomplete="off" />
                <asp:Button ID="btnSearch" runat="server" CausesValidation="False" CommandName="Search"
                    Text="查尋" />
                <cc2:AutoCompleteExtender runat="server" BehaviorID="AutoCompleteEx" ID="autoComplete1"
                    TargetControlID="myTextBox" ServicePath="~/test/AutoComplete.asmx" ServiceMethod="GetCompletionList"
                    MinimumPrefixLength="2" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="autocomplete_listItem"
                    CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem" DelimiterCharacters=";, :"
                    ShowOnlyCurrentWordInCompletionListItem="true">
                    <Animations>
                    <OnShow>
                        <Sequence>
                            <%-- Make the completion list transparent and then show it --%>
                            <OpacityAction Opacity="0" />
                            <HideAction Visible="true" />
                            
                            <%--Cache the original size of the completion list the first time
                                the animation is played and then set it to zero --%>
                            <ScriptAction Script="
                                // Cache the size and setup the initial size
                                var behavior = $find('AutoCompleteEx');
                                if (!behavior._height) {
                                    var target = behavior.get_completionList();
                                    behavior._height = target.offsetHeight - 2;
                                    target.style.height = '0px';
                                }" />
                            
                            <%-- Expand from 0px to the appropriate size while fading in --%>
                            <Parallel Duration=".4">
                                <FadeIn />
                                <Length PropertyKey="height" StartValue="0" EndValueScript="$find('AutoCompleteEx')._height" />
                            </Parallel>
                        </Sequence>
                    </OnShow>
                    <OnHide>
                        <%-- Collapse down to 0px and fade out --%>
                        <Parallel Duration=".4">
                            <FadeOut />
                            <Length PropertyKey="height" StartValueScript="$find('AutoCompleteEx')._height" EndValue="0" />
                        </Parallel>
                    </OnHide>
                    </Animations>
                </cc2:AutoCompleteExtender>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Button ID="Button1" runat="server" CausesValidation="False" CommandName="Edit"
                    Text="編輯" />
                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="Delete"
                    OnClientClick="return confirm('確定刪除此資料項?');" Text="刪除" />
                &nbsp;<asp:Button ID="Button3" runat="server" CausesValidation="False" CommandName="New"
                    CommandArgument='<%# Container.DataItemIndex %>' Text="新增" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Button ID="Button1" runat="server" CausesValidation="True" CommandName="Update"
                    Text='<%# _insertMode ? "加入" : "更新"%>' />
                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="Cancel"
                    Text="取消" />
            </EditItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <asp:Button ID="btnCreate" runat="server" CausesValidation="False" CommandName="Create"
            Text="新增" />
    </EmptyDataTemplate>
</asp:GridView>
<asp:Button ID="btnPopup" runat="server" Text="Popup" Style="display: none" />
<cc2:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="Panel1"
    TargetControlID="btnPopup" DropShadow="true" BackgroundCssClass="modalBackground"
    PopupDragHandleControlID="winTitle">
</cc2:ModalPopupExtender>
<asp:Panel ID="Panel1" runat="server" Style="display: none;" CssClass="modalPopup">
    <table width="100%" bgcolor="#CCCCFF" id="winTitle" runat="server">
        <tr>
            <td align="right">
                <asp:LinkButton ID="lbClose" runat="server" Style="border: 2px solid #000000; text-decoration: none;">×</asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="overflow: scroll; height: 350px">
                <uc1:DetailEntity ID="details" runat="server" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
