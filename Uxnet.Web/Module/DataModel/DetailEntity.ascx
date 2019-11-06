<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailEntity.ascx.cs"
    Inherits="Uxnet.Web.Module.DataModel.DetailEntity" %>
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" DataKeyNames="BankCode"
    OnRowCancelingEdit="gvEntity_RowCancelingEdit" OnRowDeleting="gvEntity_RowDeleting"
    OnRowEditing="gvEntity_RowEditing" OnRowUpdating="gvEntity_RowUpdating" AllowPaging="True"
    AllowSorting="True" OnSorting="gvEntity_Sorting" OnPreRender="gvEntity_PreRender"
    ShowFooter="True" OnRowCommand="gvEntity_RowCommand" 
    onselectedindexchanging="gvEntity_SelectedIndexChanging">
    <Columns>
        <asp:TemplateField ShowHeader="False">
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
