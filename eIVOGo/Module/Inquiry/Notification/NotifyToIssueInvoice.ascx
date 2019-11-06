<%@ Control Language="C#" AutoEventWireup="true"  %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register src="InvoiceItemList.ascx" tagname="InvoiceItemList" tagprefix="uc3" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Register src="InvoiceCancellationList.ascx" tagname="InvoiceCancellationList" tagprefix="uc4" %>
<%@ Register src="InvoiceAllowanceList.ascx" tagname="InvoiceAllowanceList" tagprefix="uc5" %>
<%@ Register src="AllowanceCancellationList.ascx" tagname="AllowanceCancellationList" tagprefix="uc6" %>
<%@ Register src="ReceiptItemList.ascx" tagname="ReceiptItemList" tagprefix="uc7" %>
<%@ Register src="ReceiptCancellationList.ascx" tagname="ReceiptCancellationList" tagprefix="uc8" %>
<uc1:PageAction ID="actionItem" runat="server" ItemName="下列資料已傳送至集團加值中心，請登入平台執行開立。" EnableIcon="False" />
<uc2:FunctionTitleBar ID="titleBar" runat="server" ItemName="電子發票" EnableIcon="False" />
<uc3:InvoiceItemList ID="itemList" runat="server" AllowPaging="False" />
<uc2:FunctionTitleBar ID="titleAllowance" runat="server" ItemName="發票折讓" EnableIcon="False" />
<uc5:InvoiceAllowanceList ID="allowanceList" runat="server" AllowPaging="False"/>
<uc2:FunctionTitleBar ID="titleCancellation" runat="server" ItemName="作廢發票" EnableIcon="False" />
<uc4:InvoiceCancellationList ID="invoiceCancelList" runat="server" AllowPaging="False"/>
<uc2:FunctionTitleBar ID="titleAllowanceCancellation" runat="server" ItemName="作廢折讓" EnableIcon="False" />
<uc6:AllowanceCancellationList ID="allowanceCancelList" runat="server" AllowPaging="False"/>
<uc2:FunctionTitleBar ID="titleReceipt" runat="server" ItemName="收據" 
    EnableIcon="False" />
<uc7:ReceiptItemList ID="receiptList" runat="server" AllowPaging="false" />
<uc2:FunctionTitleBar ID="titleCancelReceipt" runat="server" ItemName="作廢收據" 
    EnableIcon="False" />
<uc8:ReceiptCancellationList ID="receiptCancelList" runat="server" AllowPaging="false" />


<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += new EventHandler(module_inquiry_notification_notifytoissueinvoice_ascx_PreRender);
        
        int businessID;
        if (!String.IsNullOrEmpty(Request["businessID"]) && int.TryParse(Request["businessID"], out businessID))
        {
            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.電子發票
                    && d.DocumentDispatches.Where(r => r.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.電子發票).Count() > 0
                    && d.InvoiceItem.InvoiceSeller.SellerID == businessID);
            };

            invoiceCancelList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢發票
                    && d.DocumentDispatches.Where(r => r.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢發票).Count() > 0
                    && d.DerivedDocument.ParentDocument.InvoiceItem.InvoiceSeller.SellerID == businessID);
            };

            allowanceList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.發票折讓
                    && d.DocumentDispatches.Where(s => s.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.發票折讓).Count() > 0
                    && d.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID == businessID);
            };

            allowanceCancelList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢折讓
                    && d.DocumentDispatches.Where(s => s.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢折讓).Count() > 0
                    && d.DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID == businessID);
            };

            receiptList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.收據
                    && d.DocumentDispatches.Where(r => r.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.收據).Count() > 0
                    && d.ReceiptItem.SellerID == businessID);
            };

            receiptCancelList.BuildQuery = table =>
            {
                return table.Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立 && d.DocType == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢收據
                    && d.DocumentDispatches.Where(r => r.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢收據).Count() > 0
                    && d.DerivedDocument.ParentDocument.ReceiptItem.SellerID == businessID);
            };     
            
        }
    }

    void module_inquiry_notification_notifytoissueinvoice_ascx_PreRender(object sender, EventArgs e)
    {
        itemList.Visible = itemList.Select().Count() > 0;
        titleBar.Visible = itemList.Visible;
        invoiceCancelList.Visible = invoiceCancelList.Select().Count() > 0;
        titleCancellation.Visible = invoiceCancelList.Visible;
        allowanceList.Visible = allowanceList.Select().Count() > 0;
        titleAllowance.Visible = allowanceList.Visible;
        allowanceCancelList.Visible = allowanceCancelList.Select().Count() > 0;
        titleAllowanceCancellation.Visible = allowanceCancelList.Visible;
        titleReceipt.Visible = receiptList.Select().Count() > 0;
        titleCancelReceipt.Visible = receiptCancelList.Select().Count() > 0;
    }

</script>
