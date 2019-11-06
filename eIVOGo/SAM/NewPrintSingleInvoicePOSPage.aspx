<%@ Page Language="C#" AutoEventWireup="true" Theme="POSPrint" %>
<%@ Register Src="~/Module/EIVO/NewInvoicePOSPrintView.ascx" TagPrefix="uc1" TagName="NewInvoicePrintView" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <style type="text/css">
        div.fspace
        {
            height: 8.8cm;
        }
        div.bspace
        {
            height: 8.9cm;
        }
        
        body, td, th
        {
            font-family: Verdana, Arial, Helvetica, sans-serif, "細明體" , "新細明體";
        }

    </style>
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body style="margin-left:0cm; margin-right:0cm;">
    <script type="text/javascript" src="../Scripts/css3-multi-column.js"></script>
    <form id="theForm" runat="server">
        <uc1:NewInvoicePrintView runat="server" ID="finalView" IsFinal="true" />
    </form>
</body>
</html>
<script>
    (function () {
        var beforePrint = function () {
        };

        var afterPrint = function () {
            window.close();
        };

        if (window.matchMedia) {
            var mediaQueryList = window.matchMedia('print');
            mediaQueryList.addListener(function (mql) {
                if (mql.matches) {
                    beforePrint();
                } else {
                    afterPrint();
                }
            });
        }

        window.onbeforeprint = beforePrint;
        window.onafterprint = afterPrint;

        window.onload = function () {
            self.focus();
            self.print();
            //window.close();
        };
    })();
</script>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        int invoiceID;
        if (!String.IsNullOrEmpty(Request["invoiceID"]) && int.TryParse(Request["invoiceID"], out invoiceID))
        {
            finalView.InvoiceID = invoiceID;
        }
        else if (Request["invoiceNo"] != null)
        {
            finalView.InvoiceNo = Request["invoiceNo"];
        }
    }
</script>