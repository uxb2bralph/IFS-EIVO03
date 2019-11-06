<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AllowancePrintView.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.AllowancePrintViewB2B" %>
<%@ Register Src="Item/AllowancePrintViewPart1.ascx" TagName="AllowancePrintViewPart1"
    TagPrefix="uc1" %>
<%@ Register Src="Item/AllowancePrintViewPart2.ascx" TagName="AllowancePrintViewPart2"
    TagPrefix="uc2" %>
<%@ Register Src="Item/AllowancePrintViewPart3.ascx" TagName="AllowancePrintViewPart3"
    TagPrefix="uc3" %>
<%@ Register Src="Item/AllowancePrintViewPart4.ascx" TagName="AllowancePrintViewPart4"
    TagPrefix="uc4" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<div class="bk" style="page-break-after: always;">
    <uc3:AllowancePrintViewPart3 ID="part3" runat="server" />
</div>
<div class="bk">
    <uc4:AllowancePrintViewPart4 ID="part4" runat="server" />
</div>
<div id="newPage" runat="server" enableviewstate="false" visible="<%# IsFinal!=true %>"
    style="page-break-after: always;">
</div>
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
