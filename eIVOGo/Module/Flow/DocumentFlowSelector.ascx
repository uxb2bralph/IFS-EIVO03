<%@ Control Language="C#" AutoEventWireup="true" Inherits="Uxnet.Web.Module.DataModel.ItemSelector" %>
<%@ Register assembly="Model" namespace="Model.DocumentFlowManagement" tagprefix="cc1" %>
<%@ Import Namespace="Model.DocumentFlowManagement" %>
<asp:DropDownList ID="selector" runat="server" DataTextField="FlowName" DataSourceID="dsEntity"
    DataValueField="FlowID" EnableViewState="false" OnDataBound="selector_DataBound">
</asp:DropDownList>
<cc1:DocumentFlowDataSource ID="dsEntity" runat="server">
</cc1:DocumentFlowDataSource>

