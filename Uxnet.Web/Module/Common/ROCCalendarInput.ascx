<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ROCCalendarInput.ascx.cs" Inherits="Uxnet.Web.Module.Common.ROCCalendarInput" %>
<asp:TextBox id="txtDate" CssClass="textfield" runat="server" Columns="10" />
<span class="contant"><img runat="server" id="imgCalendar" src="~/Module/Common/calendar.gif" width="24" height="24"
		align="absMiddle" style="CURSOR:pointer"/>
    <asp:Label ID="errorMsg" runat="server" Text="請選擇日期!!" Visible="false" EnableViewState="false" ForeColor="Red"></asp:Label>
