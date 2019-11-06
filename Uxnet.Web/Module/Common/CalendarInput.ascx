<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarInput.ascx.cs"
    Inherits="Uxnet.Web.Module.Common.CalendarInput" %>
<asp:TextBox ID="txtDate" CssClass="textfield" runat="server" Columns="10" ReadOnly="True" />
<img runat="server" id="imgCalendar" src="~/Module/Common/calendar.gif" width="24"
    height="24" align="absMiddle" style="cursor: pointer" />
<asp:Label ID="errorMsg" runat="server" Text="請選擇日期!!" Visible="false" EnableViewState="false"
    ForeColor="Red"></asp:Label>
