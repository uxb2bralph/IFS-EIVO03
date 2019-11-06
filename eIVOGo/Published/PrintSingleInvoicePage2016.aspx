<%@ Page Language="C#" AutoEventWireup="true" StylesheetTheme="Ver2016" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="~/Module/EIVO/InvoicePrintView2016.ascx" TagPrefix="uc1" TagName="InvoicePrintView2016" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body style="font-family:KaiTi">
    <form id="theForm" runat="server">
        <uc1:InvoicePrintView2016 runat="server" ID="InvoicePrintView2016" />
    </form>
</body>
</html>
