﻿<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Model.DataEntity" %>
<%#String.Format("{0:##,###,###,##0.##}", ((CDS_Document)((GridViewRow)Container).DataItem).InvoiceItem.InvoiceAmountType.TotalAmount)%>

