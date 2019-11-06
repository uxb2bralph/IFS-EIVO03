<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InquireBusinessRelationship.ascx.cs"
    Inherits="eIVOGo.Module.SAM.Business.InquireBusinessRelationship" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/CalendarInputDatePicker.ascx" TagName="CalendarInputDatePicker"
    TagPrefix="uc3" %>
<%@ Register Src="~/Module/SAM/Business/EnterpriseCounterpartBusinessList.ascx" TagName="EnterpriseCounterpartBusinessList"
    TagPrefix="uc4" %>
<%@ Register Src="GroupMemberSelector.ascx" TagName="GroupMemberSelector" TagPrefix="uc5" %>
<%@ Register Src="../../Common/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc6" %>
<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 集團營業人資料查詢" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="集團營業人資料查詢" />
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
                <th width="120">
                    營業人統編
                </th>
                <td class="tdleft">
                    <asp:TextBox ID="ReceiptNo" runat="server"></asp:TextBox>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <th width="120">
                    <span class="tdleft">營業人名稱</span>
                </th>
                <td class="tdleft">
                    <asp:TextBox ID="CompanyName" runat="server"></asp:TextBox>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <th>
                    集團成員
                </th>
                <td class="tdleft">
                    <uc5:GroupMemberSelector ID="CompanyID" runat="server" SelectorIndication="全部" />
                </td>
            </tr>
            <tr>
                <th width="120">
                    營業人狀態
                </th>
                <td class="tdleft">
                    <asp:DropDownList ID="CompanyStatus" runat="server">
                        <asp:ListItem Value="">全部</asp:ListItem>
                        <asp:ListItem Value="1103">已啟用</asp:ListItem>
                        <asp:ListItem Value="1101">已停用</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th width="120">
                    營業人類別
                </th>
                <td class="tdleft">
                    <uc6:EnumSelector ID="BusinessType" runat="server" SelectorIndication="全部" TypeName="Model.Locale.Naming+InvoiceCenterBusinessQueryType, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
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
<uc4:EnterpriseCounterpartBusinessList ID="itemList" runat="server" Visible="false" />
