<%@ Page Language="C#" AutoEventWireup="true" StylesheetTheme="Ver2016" %>

<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<%@ Register Src="~/Module/EIVO/Item/InvoiceView2016.ascx" TagPrefix="uc1" TagName="InvoiceView2016" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.DataEntity" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body>
    <form id="theForm" runat="server">
        <cc1:InvoiceDataSource ID="dsEntity" runat="server">
        </cc1:InvoiceDataSource>
    </form>
</body>
</html>
<script runat="server">

    Model.Security.MembershipManagement.UserProfileMember _userProfile;
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _userProfile = Business.Helper.WebPageUtility.UserProfile;
        initializeData();
    }

    protected virtual void initializeData()
    {
        IEnumerable<int> items;
        var mgr = dsEntity.CreateDataManager();

        items = mgr.GetTable<DocumentPrintQueue>().Where(i => i.UID == _userProfile.UID && i.CDS_Document.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice).Select(i => i.DocID).ToList();
        if (items != null && items.Count() > 0)
        {
            foreach (var item in items)
            {
                ASP.module_eivo_item_invoiceview2016_ascx view = (ASP.module_eivo_item_invoiceview2016_ascx)this.LoadControl("~/Module/EIVO/Item/InvoiceView2016.ascx");
                view.InitializeAsUserControl(this.Page);
                view.Item = mgr.GetTable<InvoiceItem>().Where(i => i.InvoiceID == item).FirstOrDefault();
                if (view.Item != null)
                {
                    theForm.Controls.Add(view);

                    if (!mgr.GetTable<DocumentPrintLog>().Any(d => d.DocID == item && d.TypeID == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice))
                    {
                        mgr.GetTable<DocumentPrintLog>().InsertOnSubmit(new DocumentPrintLog
                            {
                                PrintDate = DateTime.Now,
                                UID = _userProfile.UID,
                                DocID = item,
                                TypeID = (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice
                            });
                    }

                    mgr.DeleteAnyOnSubmit<DocumentPrintQueue>(q => q.DocID == item);
                    mgr.SubmitChanges();
                }
            }
        }
    }

</script>