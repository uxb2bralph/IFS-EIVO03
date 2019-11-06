<%@ Control Language="C#" AutoEventWireup="true" Inherits="Uxnet.Web.Module.DataModel.ItemSelector" %>
<%@ Register assembly="Model" namespace="Model.DocumentFlowManagement" tagprefix="cc1" %>
<%@ Import Namespace="Model.DocumentFlowManagement" %>
<asp:DropDownList ID="selector" runat="server" DataTextField="Expression"
    DataValueField="CompanyID" EnableViewState="false" OnDataBound="selector_DataBound">
</asp:DropDownList>
<cc1:DocumentTypeFlowDataSource ID="dsEntity" runat="server">
</cc1:DocumentTypeFlowDataSource>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        var mgr = ((DocumentTypeFlowDataSource)dsEntity).CreateDataManager();
        selector.DataSource = mgr.GetTable<Organization>()
            .Join(mgr.GetTable<EnterpriseGroupMember>().GroupBy(m => m.CompanyID), c => c.CompanyID, b => b.Key, (c, b) => c)
            .Select(c => new
            {
                Expression = String.Format("{0}({1})", c.CompanyName, c.ReceiptNo),
                c.CompanyID
            });

        this.PreRender += new EventHandler(module_flow_documenttypeflowcandidateselector_ascx_PreRender);
    }

    void module_flow_documenttypeflowcandidateselector_ascx_PreRender(object sender, EventArgs e)
    {
        if (!_isBound)
        {
            selector.DataBind();
        }
    }
</script>


