<%@ Control Language="C#" AutoEventWireup="true" Inherits="Uxnet.Web.Module.DataModel.ItemSelector" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<asp:DropDownList ID="selector" runat="server" DataTextField="EnterpriseName"
    DataValueField="EnterpriseID" EnableViewState="false" OnDataBound="selector_DataBound">
</asp:DropDownList>
<cc1:EnterpriseGroupDataSource ID="dsEntity" runat="server">
</cc1:EnterpriseGroupDataSource>

<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        //this.PreRender += new EventHandler(ItemSelector_PreRender);
        var mgr = ((EnterpriseGroupDataSource)dsEntity).CreateDataManager();
        //selector.DataSource = mgr.GetTable<InvoiceItem>()
        //    .Select(i=>i.SellerID).Distinct()
        //    .Join(mgr.EntityList, b => b, o => o.CompanyID, (b, o) => o)
        //    .OrderBy(o => o.ReceiptNo).Select(o => new { Expression = String.Format("{0} {1}", o.ReceiptNo, o.CompanyName), o.CompanyID });
        if (SelectAll)
            selector.Items.Add(new ListItem("全部", ""));
        selector.Items.AddRange(mgr.GetTable<EnterpriseGroup>().Select(t => new ListItem(t.EnterpriseName, t.EnterpriseID.ToString())).ToArray());

    }
    //void ItemSelector_PreRender(object sender, EventArgs e)
    //{

    //}
        [System.ComponentModel.Bindable(true)]
    public bool SelectAll
    {
        get;
        set;
    }
</script>
