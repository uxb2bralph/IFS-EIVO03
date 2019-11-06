<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintReceiptPage.aspx.cs" Inherits="eIVOGo.SAM.PrintReceiptPage" StylesheetTheme="Receipt" %>

<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css" media="print">
div.fspace { height: 9.3cm; }
div.bspace { height: 10cm; }
</style>
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body onload='javascript:self.print();self.close();'>
    <form id="theForm" runat="server">

    <cc1:ReceiptDataSource ID="dsEntity" runat="server">
    </cc1:ReceiptDataSource>

    </form>
</body>
</html>
