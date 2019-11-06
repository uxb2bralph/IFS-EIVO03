<%@ Page Title="" Language="C#" MasterPageFile="~/template/main_page.Master" AutoEventWireup="true" Inherits="eIVOGo.template.base_page" StylesheetTheme="Visitor" %>
<%@ Register src="../../Module/Inquiry/ForOP/InquireResendInvoiceCancellation.ascx" tagname="InquireInvoiceCancellation" tagprefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    <uc1:InquireInvoiceCancellation ID="InquireInvoiceCancellation1" runat="server" />
</asp:Content>
