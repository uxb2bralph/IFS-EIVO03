﻿<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Model.DataEntity" %>
<%# ((CDS_Document)((GridViewRow)Container).DataItem).InvoiceItem.InvoicePurchaseOrder == null ? "" : ((CDS_Document)((GridViewRow)Container).DataItem).InvoiceItem.InvoicePurchaseOrder.OrderNo%>
