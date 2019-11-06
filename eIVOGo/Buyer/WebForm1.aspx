﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="MyInvoice.Buyer.WebForm1" %>

<%@ Register Src="../Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc1" %>
<%@ Register Src="../Module/Common/PrintingButton2.ascx" TagName="PrintingButton2"
    TagPrefix="uc2" %>
<%@ Register Src="TestPostBackEvent.ascx" TagName="TestPostBackEvent" TagPrefix="uc3" %>
<%@ Register Src="../Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc4" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="../Module/Common/SignContext.ascx" TagName="SignContext" TagPrefix="uc5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <uc4:PageAction ID="pageAction" runat="server" ItemName="資料繫結測試" />
    <asp:Literal ID="litMsg" runat="server" Text="<%= DateTime.Now %>" EnableViewState="false"></asp:Literal>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="InvoiceID" EnableViewState="False">
                    <Columns>
                        <asp:BoundField DataField="InvoiceID" HeaderText="InvoiceID" ReadOnly="True" SortExpression="InvoiceID" />
                        <asp:TemplateField HeaderText="No" SortExpression="No">
                            <ItemTemplate>
                                <%# ((InvoiceItem)Container.DataItem).No %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="InvoiceDate" HeaderText="InvoiceDate" SortExpression="InvoiceDate" />
                        <asp:BoundField DataField="CheckNo" HeaderText="CheckNo" SortExpression="CheckNo" />
                        <asp:BoundField DataField="Remark" HeaderText="Remark" SortExpression="Remark" />
                        <asp:BoundField DataField="BuyerRemark" HeaderText="BuyerRemark" SortExpression="BuyerRemark" />
                        <asp:BoundField DataField="CustomsClearanceMark" HeaderText="CustomsClearanceMark"
                            SortExpression="CustomsClearanceMark" />
                        <asp:BoundField DataField="TaxCenter" HeaderText="TaxCenter" SortExpression="TaxCenter" />
                    </Columns>
                    <PagerTemplate>
                        <uc1:PagingControl ID="pagingList" runat="server" OnPageIndexChanged="PageIndexChanged" />
                    </PagerTemplate>
                </asp:GridView>
                <uc2:PrintingButton2 ID="btnPrint" runat="server" />
                <br />
                <asp:TextBox ID="dataToSign" runat="server" Columns="80" Rows="10" TextMode="MultiLine"></asp:TextBox>
                <asp:Button ID="btnCA" runat="server" Text="Test CA" OnClick="btnCA_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrint" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <uc3:TestPostBackEvent ID="TestPostBackEvent1" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:Button ID="btnHello" runat="server" OnClick="btnHello_Click" Text="Hello" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc5:SignContext ID="signContext" runat="server" />
    </form>
</body>
</html>
