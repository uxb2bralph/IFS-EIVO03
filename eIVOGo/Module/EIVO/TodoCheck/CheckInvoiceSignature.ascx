<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.TodoCheck.CheckBuyerInvoice" %>
<%@ Register src="~/Module/Common/ActionHandler.ascx" tagname="ActionHandler" tagprefix="uc1" %>
<%@ Register src="../../Common/PageAnchor.ascx" tagname="PageAnchor" tagprefix="uc2" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<tr>
    <td>
        <img runat="server" id="img1" enableviewstate="false" border="0" align="middle" src="~/images/arrow_02.gif"
            width="15" height="15" />
        <a href="#" onclick="<%# doLink.GetPostBackEventReference(null) %>">統一發票專用章待匯入</a>
    </td>
</tr>
<uc1:ActionHandler ID="doLink" runat="server" />
<uc2:PageAnchor ID="LinkAction" runat="server"  RedirectTo="~/SAM/ImportInvoiceSignature.aspx" />
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
<script runat="server">
    int _count;
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var mgr = dsEntity.CreateDataManager();
        var item = mgr.GetTable<Organization>().Where(d => d.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID).First();
        
        if (String.IsNullOrEmpty(item.InvoiceSignature))
        {
            this.DataBind();
        }
        else
        {
            this.Visible = false;
        }
    }
</script>


