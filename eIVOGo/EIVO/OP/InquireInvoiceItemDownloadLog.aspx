<%@ Page Title="" Language="C#" MasterPageFile="~/template/main_page.Master" AutoEventWireup="true" Inherits="eIVOGo.template.base_page" StylesheetTheme="Visitor" %>
<%@ Register Src="../../Module/Inquiry/ForOP/InquireInvoiceItemDownloadLog.ascx"
    TagName="InquireInvoiceItemDownloadLog" TagPrefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    <uc1:InquireInvoiceItemDownloadLog ID="InquireInvoiceItemDownloadLog" runat="server" />
</asp:Content>
