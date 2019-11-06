<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupMemberSelector.ascx.cs" Inherits="eIVOGo.Module.SAM.Business.GroupMemberSelector" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<asp:DropDownList ID="selector" runat="server" EnableViewState="False" DataTextField="Expression" DataValueField="CompanyID"
    ondatabound="selector_DataBound">
</asp:DropDownList>
<cc1:EnterpriseGroupMemberDataSource ID="dsEntity" runat="server">
</cc1:EnterpriseGroupMemberDataSource>


