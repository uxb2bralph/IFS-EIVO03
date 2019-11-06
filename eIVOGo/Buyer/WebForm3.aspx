﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="MyInvoice.Buyer.WebForm3" %>

<%@ Register assembly="Uxnet.Com.Net4" namespace="DataAccessLayer.basis" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="btnClick" runat="server" onclick="btnClick_Click" 
            Text="Click me" />
&nbsp;
        <asp:Label ID="lbClick" runat="server" Text="Click me"></asp:Label>
    
    </div>
    <asp:FormView ID="FormView1" runat="server" AllowPaging="True" 
        DataSourceID="LinqDataSource1">
        <EditItemTemplate>
            ContactName:
            <asp:TextBox ID="ContactNameTextBox" runat="server" 
                Text='<%# Bind("ContactName") %>' />
            <br />
            Fax:
            <asp:TextBox ID="FaxTextBox" runat="server" Text='<%# Bind("Fax") %>' />
            <br />
            CompanyName:
            <asp:TextBox ID="CompanyNameTextBox" runat="server" 
                Text='<%# Bind("CompanyName") %>' />
            <br />
            ReceiptNo:
            <asp:TextBox ID="ReceiptNoTextBox" runat="server" 
                Text='<%# Bind("ReceiptNo") %>' />
            <br />
            Phone:
            <asp:TextBox ID="PhoneTextBox" runat="server" Text='<%# Bind("Phone") %>' />
            <br />
            Addr:
            <asp:TextBox ID="AddrTextBox" runat="server" Text='<%# Bind("Addr") %>' />
            <br />
            <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" 
                CommandName="Update" Text="Update" />
            &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" 
                CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </EditItemTemplate>
        <InsertItemTemplate>
            ContactName:
            <asp:TextBox ID="ContactNameTextBox" runat="server" 
                Text='<%# Bind("ContactName") %>' />
            <br />
            Fax:
            <asp:TextBox ID="FaxTextBox" runat="server" Text='<%# Bind("Fax") %>' />
            <br />
            CompanyName:
            <asp:TextBox ID="CompanyNameTextBox" runat="server" 
                Text='<%# Bind("CompanyName") %>' />
            <br />
            ReceiptNo:
            <asp:TextBox ID="ReceiptNoTextBox" runat="server" 
                Text='<%# Bind("ReceiptNo") %>' />
            <br />
            Phone:
            <asp:TextBox ID="PhoneTextBox" runat="server" Text='<%# Bind("Phone") %>' />
            <br />
            Addr:
            <asp:TextBox ID="AddrTextBox" runat="server" Text='<%# Bind("Addr") %>' />
            <br />
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                CommandName="Insert" Text="Insert" />
            &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" 
                CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </InsertItemTemplate>
        <ItemTemplate>
            ContactName:
            <asp:Label ID="ContactNameLabel" runat="server" 
                Text='<%# Bind("ContactName") %>' />
            <br />
            Fax:
            <asp:Label ID="FaxLabel" runat="server" Text='<%# Bind("Fax") %>' />
            <br />
            CompanyName:
            <asp:Label ID="CompanyNameLabel" runat="server" 
                Text='<%# Bind("CompanyName") %>' />
            <br />
            ReceiptNo:
            <asp:Label ID="ReceiptNoLabel" runat="server" Text='<%# Bind("ReceiptNo") %>' />
            <br />
            Phone:
            <asp:Label ID="PhoneLabel" runat="server" Text='<%# Bind("Phone") %>' />
            <br />
            Addr:
            <asp:Label ID="AddrLabel" runat="server" Text='<%# Bind("Addr") %>' />
            <br />

        </ItemTemplate>
    </asp:FormView>
    <cc1:LinqDataSource ID="LinqDataSource1" runat="server" 
        ContextTypeName="Model.DataEntity.EIVOEntityDataContext" EntityTypeName="" 
        Select="new (ContactName, Fax, CompanyName, ReceiptNo, Phone, Addr)" 
        TableName="Organization" oncontextcreated="LinqDataSource1_ContextCreated" 
        oncontextcreating="LinqDataSource1_ContextCreating" 
        onquerycreated="LinqDataSource1_QueryCreated" 
        onselected="LinqDataSource1_Selected" onselecting="LinqDataSource1_Selecting">
    </cc1:LinqDataSource>
    </form>
</body>
</html>
