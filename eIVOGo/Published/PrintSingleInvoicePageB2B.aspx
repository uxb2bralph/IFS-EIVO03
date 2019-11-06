<%@ Page Language="C#" AutoEventWireup="true" StylesheetTheme="Paper" %>
<%@ Import Namespace="Model.DataEntity" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css" media="print">
        body, td, th
        {
            font-family: Verdana, Arial, Helvetica, sans-serif, "細明體" , "新細明體";
        }
    </style>
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body>
    <form id="theForm" runat="server">
    </form>
</body>
</html>
<script runat="server">

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.Load += new EventHandler(sam_printsingleinvoicepage_aspx_Load);
    }

    void sam_printsingleinvoicepage_aspx_Load(object sender, EventArgs e)
    {
        initializeData();
    }

    protected virtual void initializeData()
    {
        int invoiceID;
        if (!String.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out invoiceID))
        {
            String invoicePrintView = null;
            using (Model.InvoiceManagement.InvoiceManager mgr = new Model.InvoiceManagement.InvoiceManager())
            {
                eIVOGo.Module.EIVO.InvoicePrintView finalView = null;
                var item = mgr.GetTable<InvoiceItem>().Where(i => i.InvoiceID == invoiceID).FirstOrDefault();
                if (item!=null)
                {
                    invoicePrintView = item.Organization.OrganizationStatus.InvoicePrintView;
                    if (String.IsNullOrEmpty(invoicePrintView))
                    {
                        invoicePrintView = "~/Module/EIVO/InvoicePrintView.ascx";
                    }

                    eIVOGo.Module.EIVO.InvoicePrintView view = (eIVOGo.Module.EIVO.InvoicePrintView)this.LoadControl(invoicePrintView);
                    view.InitializeAsUserControl(this.Page);
                    view.InvoiceID = item.InvoiceID;
                    theForm.Controls.Add(view);
                    finalView = view;
                }
                
                if (finalView != null)
                    finalView.IsFinal = true;
            }
            
        }
    }
    
</script>