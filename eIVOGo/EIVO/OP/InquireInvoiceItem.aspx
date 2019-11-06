<%@ Page Title="" Language="C#" MasterPageFile="~/template/main_page.Master" AutoEventWireup="true" Inherits="eIVOGo.template.base_page" StylesheetTheme="Visitor" %>
<%@ Register src="../../Module/Inquiry/ForOP/InquireInvoiceItem.ascx" tagname="InquireInvoiceItem" tagprefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    <uc1:InquireInvoiceItem ID="InquireInvoiceItem1" runat="server" />
</asp:Content>
