<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrintInvoiceForSale.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.Action.PrintInvoiceForSale" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Inquiry/ForPrint/InquireInvoiceItemForSale.ascx" TagName="InquireInvoiceForSale"
    TagPrefix="uc3" %>
<%@ Register Src="~/Module/Common/PageAnchor.ascx" TagName="PageAnchor" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/SignContext.ascx" TagName="SignContext" TagPrefix="uc5" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>


<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 銷項電子發票/折讓/收據查詢/列印" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="銷項電子發票/折讓/收據查詢/列印" />
<uc3:InquireInvoiceForSale ID="inquiryAction" runat="server" />
<table border="0" cellspacing="0" cellpadding="0" width="100%" runat="server" visible="false"
    enableviewstate="false" id="tblAction">
    <tbody>
      <tr>
            <td align="center">
                列印選項：<asp:RadioButton ID="rdbAll" Text="扣抵聯/收執聯" GroupName="01" runat="server" Checked="true" /><asp:RadioButton ID="rdbFirst" Text="存根聯" GroupName="01" runat="server" />
                
                </td>
        </tr>
        <tr>
            <td class="Bargain_btn">
                <asp:Button ID="btnReceive" runat="server" Text="列印" OnClick="btnReceive_Click" />
            </td>
        </tr>
    </tbody>
</table>
<uc5:SignContext ID="signContext" runat="server" Catalog="列印發票"  Entrusting="True"
    UsePfxFile="False" EmptyContentMessage="沒有資料可供列印!!" />
<uc4:PageAnchor ID="NextAction" runat="server" TransferTo="" />
<cc1:DocumentDataSource ID="dsEntity" runat="server">
</cc1:DocumentDataSource>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _successfulMsg = "下載檔案作業即將進行，請稍後...!!";
    }
</script>