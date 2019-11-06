<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChooseMultiple.ascx.cs" Inherits="Uxnet.Web.Module.Common.ChooseMultiple" %>
<table align="center" border="1" cellspacing="0" class="text01" rules="all"
    border-collapse: collapse; background-color: #999999">
    <tr>
        <td align="center" style="background-color: #dce0e7">
            可用
        </td>
        <td align="center" style="background-color: #dce0e7">
            選擇
        </td>
        <td align="center" style="background-color: #dce0e7">
            已設定
        </td>
    </tr>
    <tr>
        <td align="center" style="background-color: #cccc99">
            <asp:ListBox ID="applicableList" runat="server" CssClass="inputText" 
                Rows="10" SelectionMode="Multiple"></asp:ListBox>
        </td>
        <td align="center" style="background-color: #cccc99">
            <asp:Button ID="btnSelect" runat="server" CssClass="inputText" Text="=&gt;" 
                onclick="btnSelect_Click" />
            <br />
            <br />
            <asp:Button ID="btnRemove" runat="server" CssClass="inputText" Text="&lt;=" 
                onclick="btnRemove_Click" />
        </td>
        <td align="center" style="background-color: #cccc99">
            <asp:ListBox ID="selectedList" runat="server" CssClass="inputText" 
                Rows="10" SelectionMode="Multiple"></asp:ListBox>
        </td>
    </tr>
    <tr>
        <td align="center" style="background-color: #dce0e7" align="center" colspan="3">
            <asp:Button ID="btnConfirm" runat="server" CssClass="inputText" Text="確定" 
                onclick="btnConfirm_Click" />
            &nbsp;
            <asp:Button ID="btnCancel" runat="server" CssClass="inputText" Text="取消" 
                onclick="btnCancel_Click" />
        </td>
        </tr>
    
</table>
