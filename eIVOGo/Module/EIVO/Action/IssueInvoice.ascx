<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IssueInvoice.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.Action.IssueInvoice" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Inquiry/InquireInvoiceForIssuing.ascx" TagName="InquireInvoiceForIssuing"
    TagPrefix="uc3" %>
<%@ Register Src="~/Module/Common/PageAnchor.ascx" TagName="PageAnchor" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/SignContext.ascx" TagName="SignContext" TagPrefix="uc5" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>


<uc1:PageAction ID="actionItem" runat="server" ItemName="首頁 &gt; 銷項電子發票/折讓/收據查詢/開立/接收" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="銷項電子發票/折讓/收據查詢/開立/接收" />
<uc3:InquireInvoiceForIssuing ID="inquiryAction" runat="server" />
<table border="0" cellspacing="0" cellpadding="0" width="100%" runat="server" visible="false"
    enableviewstate="false" id="tblAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <asp:Button ID="btnReceive" runat="server" Text="開立" OnClick="btnReceive_Click" />
            </td>
        </tr>
    </tbody>
</table>
<uc5:SignContext ID="signContext" runat="server" Catalog="開立發票" 
    UsePfxFile="False" EmptyContentMessage="沒有資料可供開立!!" />
<uc4:PageAnchor ID="NextAction" runat="server" TransferTo="" />
<cc1:DocumentDataSource ID="dsEntity" runat="server">
</cc1:DocumentDataSource>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += new EventHandler(module_eivo_action_issueinvoice_ascx_PreRender);
        inquiryAction.DoDefaultQuery = !this.IsPostBack && !String.IsNullOrEmpty(Request["query"]);
    }

    void module_eivo_action_issueinvoice_ascx_PreRender(object sender, EventArgs e)
    {
        //var mgr = dsEntity.CreateDataManager();
        //var item = mgr.GetTable<Organization>().Where(d => d.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID).First();

        //if (String.IsNullOrEmpty(item.InvoiceSignature))
        //{
        //    this.AjaxAlertAndRedirect("貴公司統一發票專用章尚未設定,請連絡系統管理員!!", VirtualPathUtility.ToAbsolute("~/EIVO/CheckTodoList.aspx"));
        //}
        
    }
</script>