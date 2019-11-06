﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="MyInvoice.Buyer.WebForm2" %>
<%@ Register src="../Module/Common/PagingControl.ascx" tagname="PagingControl" tagprefix="uc1" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>

<%@ Register src="../Module/Common/PrintingButton2.ascx" tagname="PrintingButton2" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table runat="server" width="100%" border="0" cellpadding="0" cellspacing="0" id="table01">
    <tr>
      <td colspan="8" class="Head_style_a">若為當期發票或為折讓單，因尚未開獎或無法兌獎，故於「是否中獎」欄位呈現「N/A」</td>
    </tr>
    </table>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AutoGenerateColumns="False" DataKeyNames="InvoiceID" EnableViewState="false"
            DataSourceID="dsInv">
            <Columns>
                <asp:BoundField DataField="InvoiceID" HeaderText="InvoiceID" ReadOnly="True" 
                    SortExpression="InvoiceID" />
                <asp:BoundField DataField="No" HeaderText="No" SortExpression="No" />
                <asp:BoundField DataField="InvoiceDate" HeaderText="InvoiceDate" 
                    SortExpression="InvoiceDate" />
                <asp:BoundField DataField="CheckNo" HeaderText="CheckNo" 
                    SortExpression="CheckNo" />
                <asp:BoundField DataField="Remark" HeaderText="Remark" 
                    SortExpression="Remark" />
                <asp:BoundField DataField="BuyerRemark" HeaderText="BuyerRemark" 
                    SortExpression="BuyerRemark" />
                <asp:BoundField DataField="CustomsClearanceMark" 
                    HeaderText="CustomsClearanceMark" SortExpression="CustomsClearanceMark" />
                <asp:BoundField DataField="TaxCenter" HeaderText="TaxCenter" 
                    SortExpression="TaxCenter" />
                <asp:BoundField DataField="PermitDate" HeaderText="PermitDate" 
                    SortExpression="PermitDate" />
                <asp:BoundField DataField="PermitWord" HeaderText="PermitWord" 
                    SortExpression="PermitWord" />
                <asp:BoundField DataField="PermitNumber" HeaderText="PermitNumber" 
                    SortExpression="PermitNumber" />
                <asp:BoundField DataField="Category" HeaderText="Category" 
                    SortExpression="Category" />
                <asp:BoundField DataField="RelateNumber" HeaderText="RelateNumber" 
                    SortExpression="RelateNumber" />
                <asp:BoundField DataField="InvoiceType" HeaderText="InvoiceType" 
                    SortExpression="InvoiceType" />
                <asp:BoundField DataField="GroupMark" HeaderText="GroupMark" 
                    SortExpression="GroupMark" />
                <asp:BoundField DataField="DonateMark" HeaderText="DonateMark" 
                    SortExpression="DonateMark" />
                <asp:BoundField DataField="SellerID" HeaderText="SellerID" 
                    SortExpression="SellerID" />
                <asp:BoundField DataField="DonationID" HeaderText="DonationID" 
                    SortExpression="DonationID" />
                <asp:BoundField DataField="RandomNo" HeaderText="RandomNo" 
                    SortExpression="RandomNo" />
                <asp:BoundField DataField="TrackCode" HeaderText="TrackCode" 
                    SortExpression="TrackCode" />
            </Columns>
            <PagerTemplate>
                <uc1:PagingControl ID="pagingList" runat="server" OnPageIndexChanged="PageIndexChanged" />
            </PagerTemplate>
        </asp:GridView>
    
    </div>
    <cc1:InvoiceDataSource ID="dsInv" runat="server">
    </cc1:InvoiceDataSource>
    
    <uc2:PrintingButton2 ID="btnPrint" runat="server" />
    
    </form>
</body>
</html>
