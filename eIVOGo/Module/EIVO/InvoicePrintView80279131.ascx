<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.InvoicePrintView" %>
<%--<%@ Register Src="Item/InvoiceProductPrintView.ascx" TagName="InvoiceProductPrintView"
    TagPrefix="uc1" %>--%>
<%@ Register Src="Item/InvoiceStubView80279131.ascx" TagName="InvoiceStubView" TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="Item/InvoiceReceiptView80279131.ascx" TagName="InvoiceReceiptView"
    TagPrefix="uc2" %>
<%@ Register Src="Item/InvoiceBalanceView80279131.ascx" TagName="InvoiceBalanceView"
    TagPrefix="uc3" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<div id="divFrist" runat="server" class="bk" style="margin-top: 5px;" visible="<%#showFrist%>"
    enableviewstate="false">
    <uc1:InvoiceStubView ID="InvoiceStubView" runat="server" Item="<%# _item %>" />
</div>
<div id="Div1" runat="server" enableviewstate="false" visible="<%#showFrist&(showThird&showFourth)==true %>"
    style="page-break-after: always;">
</div>
<div class="Khaki" runat="server"   visible="<%#showThird%>"  enableviewstate="false"
 style="margin-top: 5px;page-break-after: always;">
    <uc2:InvoiceReceiptView ID="receiptView" runat="server" Item="<%# _item %>" />
</div>
<%--<p style="border-bottom: 1px dotted #000; margin-top: 13px; margin-bottom: 10px;">
</p>--%>
<div class="or" id="divFourth" runat="server" visible="<%#showFourth%>" enableviewstate="false"
style="margin-top: 5px;">
    <uc3:InvoiceBalanceView ID="balanceView" runat="server" Item="<%# _item %>" />
</div>
<div id="newPage" runat="server" enableviewstate="false" visible="<%# IsFinal!=true %>" style="page-break-after:always;">
    &nbsp;
</div>
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
<script runat="server">
    bool isPrintSingle;
    bool showThird = true, showFourth = true,showFrist = true;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!String.IsNullOrEmpty(Request["printAll"]))
        {
            if (!Request["printAll"].ToString().Equals("1"))
            {
                showFrist = false;
            }
            else
            {
                showThird = false;
                showFourth = false;
            }

        }
        else
        {
            showFrist = false;
        }
        
        //if (!String.IsNullOrEmpty(Request["isPrintSingle"]))
        //{
        //    bool.TryParse(Request["isPrintSingle"], out isPrintSingle);
        //}
    }
</script>
