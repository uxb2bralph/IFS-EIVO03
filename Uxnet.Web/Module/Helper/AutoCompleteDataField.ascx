<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoCompleteDataField.ascx.cs"
    Inherits="Uxnet.Web.Module.Helper.AutoCompleteDataField" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:TextBox ID="TextInfo" runat="server" Columns="40"></asp:TextBox>
<asp:AutoCompleteExtender ID="TextInfo_AutoCompleteExtender" runat="server" DelimiterCharacters=";, :"
    ShowOnlyCurrentWordInCompletionListItem="true" Enabled="True" TargetControlID="TextInfo"
    ServicePath="~/published/AutoComplete.asmx" ServiceMethod="GetCompanyList"
    MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"></asp:AutoCompleteExtender>
