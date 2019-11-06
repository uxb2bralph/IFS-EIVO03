<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateSystemCertificate.ascx.cs"
    Inherits="eIVOGo.Module.SYS.CreateSystemCertificate" %>
<%@ Register Src="../UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/SignContext.ascx" TagName="SignContext" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc5" %>
<uc2:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 建立系統簽章憑證" />
<div class="border_gray">
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th class="Head_style_a" align="left">
                PKCS12(PFX)憑證檔:<br />
                <asp:FileUpload ID="PfxFile" runat="server" />
            </th>
            <td>
                PIN Code:<asp:TextBox ID="PIN" runat="server" TextMode="Password"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnConfirm" runat="server" Text="上載" OnClick="btnConfirm_Click" />
            </td>
        </tr>
    </table>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title"
        id="tblAction" runat="server" visible="false" enableviewstate="false">
        <tr>
            <th class="Head_style_a">
                憑證明細:
            </th>
            <td>
                <asp:Literal ID="certMsg" runat="server" EnableViewState="false"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th class="Head_style_a">
                設定電子簽章憑證:
            </th>
            <td>
                <asp:Button ID="btnUpload" runat="server" Text="建立憑證資訊" CommandName="Upload" 
                    OnClick="btnUpload_Click" />
                &nbsp;
                <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" Text="重選憑證" />
            </td>
        </tr>
    </table>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th align="left">
                系統憑證資訊
            </th>
            <td class="tdleft">
                <font color="red">
                    <asp:Literal ID="msg" runat="server" EnableViewState="false" Text="尚未建立"></asp:Literal></font>
            </td>
        </tr>
        <tr>
            <th align="left">
                簽章測試
            </th>
            <td class="tdleft">
                <asp:FileUpload ID="XmlFile" runat="server" />
                &nbsp;
                <asp:Button ID="btnSign" runat="server" OnClick="btnSign_Click" Text="Upload" />
            </td>
        </tr>
        <tr>
            <th align="left">
                驗簽測試
            </th>
            <td class="tdleft">
                <asp:FileUpload ID="XmlSigFile" runat="server" />
                &nbsp;
                <asp:Button ID="btnVerify" runat="server" OnClick="btnVerify_Click" Text="Verify" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
                <asp:LinkButton ID="lbViewCert" runat="server" OnClick="lbViewCert_Click" Visible="False">詳細憑證內容</asp:LinkButton>
            </td>
        </tr>
    </table>
    <!--表格 開始-->
    <!--表格 結束-->
</div>
<cc1:OrganizationDataSource ID="dsEntity" runat="server">
</cc1:OrganizationDataSource>
<uc1:SignContext ID="signContext" runat="server" UsePfxFile="False" />
<uc5:ActionHandler ID="doConfirm" runat="server" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _userProfile = Business.Helper.WebPageUtility.UserProfile;
        this.QueryExpr = o => o.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID;
    }
</script>
