<%@ Page Title="" Language="C#" MasterPageFile="~/template/main_page.Master" AutoEventWireup="true"
    Inherits="eIVOGo.template.base_page" StylesheetTheme="Visitor" %>
<%@ Register src="../Module/EIVO/Action/ReceiveInvoiceCancellation.ascx" tagname="ReceiveInvoiceCancellation" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <uc1:ReceiveInvoiceCancellation ID="ReceiveInvoiceCancellation1" 
        runat="server" />
</asp:Content>
