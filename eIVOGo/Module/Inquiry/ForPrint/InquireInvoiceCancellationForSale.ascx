<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InquireInvoiceCancellationForSale.ascx.cs"
    Inherits="eIVOGo.Module.Inquiry.ForPrint.InquireInvoiceCancellationForSale" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/CalendarInputDatePicker.ascx" TagName="CalendarInputDatePicker"
    TagPrefix="uc3" %>
<%@ Register src="~/Module/Base/InvoiceCancellationCheckList.ascx" tagname="InvoiceCancellationCheckList" tagprefix="uc4" %>
<%@ Register src="~/Module/EIVO/Business/MasterBusinessSelector.ascx" tagname="MasterBusinessSelector" tagprefix="uc5" %>
<div class="border_gray">
    <!--表格 開始-->
    <table class="left_title" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr>
                <th class="Head_style_a" colspan="2">
                    查詢條件
                </th>
            </tr>
            <tr>
                <th>
                    查詢項目
                </th>
                <td class="tdleft">
                    <asp:RadioButtonList ID="rbChange" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                        AutoPostBack="True" OnSelectedIndexChanged="rbChange_SelectedIndexChanged">
                        <asp:ListItem Value="~/EIVO/PrintInvoiceForSale.aspx">電子發票&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                        <asp:ListItem Value="~/EIVO/PrintInvoiceAllowanceForSale.aspx">電子折讓單&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                        <asp:ListItem Selected="True" Value="~/EIVO/PrintInvoiceCancellationForSale.aspx">作廢電子發票&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                        <asp:ListItem Value="~/EIVO/PrintAllowanceCancellationForSale.aspx">作廢電子折讓單</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th>
                    相對營業人
                </th>
                <td class="tdleft">
                    <uc5:MasterBusinessSelector ID="MasterID" runat="server" SelectorIndication="全部" />
                </td>
            </tr>
            <tr>
                <th width="20%">
                    日期區間
                </th>
                <td class="tdleft">
                    自&nbsp;
                    <uc3:CalendarInputDatePicker ID="DateFrom" runat="server" />
                    &nbsp;&nbsp; 至&nbsp;<uc3:CalendarInputDatePicker ID="DateTo" runat="server" />
                    &nbsp;&nbsp;
                </td>
            </tr>
        </tbody>
    </table>
    <!--表格 結束-->
</div>
<table border="0" cellspacing="0" cellpadding="0" width="100%">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <asp:Button ID="btnQuery" runat="server" Text="查詢" class="btn" OnClick="btnQuery_Click" />
            </td>
        </tr>
    </tbody>
</table>
<uc2:FunctionTitleBar ID="resultTitle" runat="server" ItemName="查詢結果" Visible="false" />
<uc4:InvoiceCancellationCheckList ID="itemList" runat="server" Visible="false" />

