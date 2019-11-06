<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeleverInvoice.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.RelativeBuyer.DeleverInvoice" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="InquireInvoiceForDelevering.ascx" TagName="InquireInvoiceForDelevering"
    TagPrefix="uc3" %>
<%@ Register Src="~/Module/Common/PageAnchor.ascx" TagName="PageAnchor" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/SignContext.ascx" TagName="SignContext" TagPrefix="uc5" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>


<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 銷項電子發票/折讓/收據查詢/開立/接收" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="銷項電子發票/折讓/收據查詢/開立/接收" />
<uc3:InquireInvoiceForDelevering ID="inquiryAction" runat="server" />
<table border="0" cellspacing="0" cellpadding="0" width="100%" runat="server" visible="false"
    enableviewstate="false" id="tblAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <asp:Button ID="btnReceive" runat="server" Text="接收" OnClick="btnReceive_Click" />
            </td>
        </tr>
    </tbody>
</table>
<uc5:SignContext ID="signContext" runat="server" Catalog="接收發票" 
    UsePfxFile="False" EmptyContentMessage="沒有資料可供接收!!" />
<uc4:PageAnchor ID="NextAction" runat="server" TransferTo="" />
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
