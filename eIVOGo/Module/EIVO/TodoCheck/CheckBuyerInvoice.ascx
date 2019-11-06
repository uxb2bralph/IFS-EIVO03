<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckBuyerInvoice.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.TodoCheck.CheckBuyerInvoice" %>
<%@ Register src="~/Module/Common/ActionHandler.ascx" tagname="ActionHandler" tagprefix="uc1" %>
<%@ Register src="../../Common/PageAnchor.ascx" tagname="PageAnchor" tagprefix="uc2" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<tr>
    <td>
        <img runat="server" id="img1" enableviewstate="false" border="0" align="middle" src="~/images/arrow_02.gif"
            width="15" height="15" />
        <a href="#" onclick="<%# doLink.GetPostBackEventReference(null) %>">進項電子發票待接收<%# _count %>筆</a>
    </td>
</tr>
<uc1:ActionHandler ID="doLink" runat="server" />
<uc2:PageAnchor ID="LinkAction" runat="server" RedirectTo="~/EIVO/ReceiveInvoice.aspx?query=All" />
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
<script runat="server">
    int _count;
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var mgr = dsEntity.CreateDataManager();

        var orgItem = mgr.GetTable<Organization>().Where(o => o.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID).First();
        if (orgItem.EnterpriseGroupMember.Count > 0)
        {
            this.Visible = false;
        }
        else
        {
            _count = mgr.GetTable<CDS_Document>().Where(d => d.CurrentStep == (int)Model.Locale.Naming.B2BInvoiceStepDefinition.待接收 && d.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice)
                .Join(mgr.EntityList.Where(i => i.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID), d => d.DocID, i => i.InvoiceID, (d, i) => d).Count();

            if (_count > 0)
            {
                this.DataBind();
            }
            else
            {
                this.Visible = false;
            }
        }
    }
</script>


