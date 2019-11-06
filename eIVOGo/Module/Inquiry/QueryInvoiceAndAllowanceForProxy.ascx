<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryInvoiceAndAllowanceForProxy.ascx.cs" 
    Inherits="eIVOGo.Module.Inquiry.QueryInvoiceAndAllowanceForProxy" %>
<%@ Register src="../Common/CalendarInputDatePicker.ascx" tagname="ROCCalendarInput" tagprefix="uc1" %>
<%@ Register src="../Common/PagingControl.ascx" tagname="PagingControl" tagprefix="uc2" %>
<%@ Register src="../Common/PrintingButton2.ascx" tagname="PrintingButton2" tagprefix="uc3" %>
<%@ Register src="../UI/InvoiceQuerySellerSelector.ascx" tagname="InvoiceSellerSelector" tagprefix="uc4" %>

<%@ Register src="../UI/PageAction.ascx" tagname="PageAction" tagprefix="uc5" %>
<%@ Register src="../UI/FunctionTitleBar.ascx" tagname="FunctionTitleBar" tagprefix="uc6" %>
<%@ Register src="../Common/EnumSelector.ascx" tagname="EnumSelector" tagprefix="uc7" %>
<%@ Register Src="~/Module/UI/InvoiceAgentSelector.ascx" TagName="InvoiceAgentSelector" TagPrefix="uc8" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" />

<uc5:PageAction ID="PageAction1" runat="server" ItemName="首頁 > 查詢發票/折讓" />        
<!--交易畫面標題-->
<uc6:FunctionTitleBar ID="FunctionTitleBar1" runat="server" ItemName="查詢發票/折讓" />
<div id="border_gray">
<!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th colspan="2" class="Head_style_a">查詢條件</th>
        </tr>
        <tr>
            <th>查詢項目</th>
            <td class="tdleft">
                <asp:RadioButtonList ID="rdbSearchItem" RepeatColumns="5" RepeatDirection="Horizontal"
                    runat="server" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rdbSearchItem_SelectedIndexChanged">
                    <asp:ListItem Value="1" Selected="True">電子發票</asp:ListItem>
                    <asp:ListItem Value="2">電子折讓單</asp:ListItem>
                    <asp:ListItem Value="3">作廢電子發票</asp:ListItem>
                    <asp:ListItem Value="4">作廢電子折讓單</asp:ListItem>
                    <asp:ListItem Value="5">中獎發票</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr id="divReceiptNo" visible="false" runat="server">
            <th>統編</th>
            <td class="tdleft">
                <uc4:InvoiceSellerSelector ID="SellerID" runat="server" SelectorIndication="全部" Postback="true" OnSelectedIndexChanged="ddlReceiptNo_SelectedIndexChanged" />
            </td>
        </tr>
        <tr id="divGoogleID" visible="false" runat="server">
            <th><asp:Label ID="lblName" runat="server" /></th>
            <td class="tdleft"><asp:TextBox ID="txtGoogleID" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <th width="20%">日期區間</th>
            <td class="tdleft">
            自&nbsp;<uc1:ROCCalendarInput ID="DateFrom" runat="server" />
                &nbsp;至&nbsp;<uc1:ROCCalendarInput ID="DateTo" runat="server" /></td>
        </tr>
        <tr>
            <th>
                發票／折讓單號碼
            </th>
            <td class="tdleft">
                <asp:TextBox ID="invoiceNo" runat="server"></asp:TextBox>
            </td>
        </tr>
        <%--<tr>
            <th>
                發票代理商家
            </th>
            <td class="tdleft">
                <uc8:InvoiceAgentSelector ID="proxySeller" runat="server"  SelectAll="true"/>
            </td>
        </tr>--%>
    <tr id="QuerySMS" runat="server">
    <th>簡訊通知</th>
        <td class="tdleft" id="SMS">
            <asp:DropDownList ID="ddlSMSState" runat="server">
                <asp:ListItem Value="">全部</asp:ListItem>
                <asp:ListItem Value="1">是</asp:ListItem>
                <asp:ListItem Value="0">否</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
<!--表格 結束-->
</div>
<!--按鈕-->
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn">
            <asp:Button ID="btnSearch" CssClass="btn" runat="server" Text="查詢" onclick="btnSearch_Click" />
        </td>
    </tr>
</table>

<div id="divResult" visible="false" runat="server">
    <uc6:FunctionTitleBar ID="FunctionTitleBar2" runat="server" ItemName="查詢結果" />
    <!--表格 開始-->
    <div id="border_gray">
        <div runat="server" id="ResultTitle">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
                <tr>
                    <td class="Head_style_a">若為當期發票或為折讓單，因尚未開獎或無法兌獎，故於「是否中獎」欄位呈現「N/A」</td>
                </tr>
            </table>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="plResult" EnableViewState="false" runat="server"></asp:PlaceHolder>
                <center>
                    <asp:Label ID="lblError" Visible="false" ForeColor="Red" Font-Size="Larger" runat="server"
                        Text="查無資料!!" EnableViewState="false"></asp:Label></center>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!--表格 結束-->
    </div>            
    <!--按鈕-->
</div>

<cc1:InvoiceDataSource ID="dsEntity" runat="server" />