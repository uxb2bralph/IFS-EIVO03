<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%# String.Format("{0:##,###,###,##0.##}",((CDS_Document)((GridViewRow)Container).DataItem).InvoiceAllowance.TaxAmount)%>
