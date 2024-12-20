﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.DataEntity" %>

    <td>
                <input name="chkItem" type="checkbox" value="<%= _model.InvoiceID %>" />
    </td>
    <td><%= String.Format("{0:yyyy/MM/dd}",_model.InvoiceDate) %></td>
    <td><%= _model.InvoiceBuyer.CustomerID %></td>
    <td><%= _model.InvoicePurchaseOrder!=null ? _model.InvoicePurchaseOrder.OrderNo : null %></td>
    <td><%= _model.InvoiceSeller.CustomerName %></td>
    <td><%= _model.InvoiceSeller.ReceiptNo %></td>
    <td><a onclick="showInvoiceModal(<%= _model.InvoiceID %>);"><%= _model.TrackCode %><%= _model.No %></a></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.SalesAmount) %></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxAmount) %></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TotalAmount) %></td>
    <td><%= _model.InvoiceWinningNumber!=null ? _model.InvoiceWinningNumber.PrizeType : "N/A" %></td>
    <td><%= _model.InvoiceBuyer.IsB2C() ? "" : _model.InvoiceBuyer.ReceiptNo %></td>
    <td><%= _model.InvoiceBuyer.CustomerName %></td>
    <td><%= _model.InvoiceBuyer.ContactName %></td>
    <td><%= _model.InvoiceBuyer.Address %></td>
    <td><%= _model.InvoiceBuyer.EMail %></td>
    <td><%= String.Join("", _model.InvoiceDetails.Select(t => t.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)) %></td>
    <td><%= _model.CDS_Document.SMSNotificationLogs.Any() ? "是" : "否" %></td>
    <td><%= _model.InvoiceCarrier!=null ? _model.InvoiceCarrier.CarrierNo : null %></td>
    <td>
        <%= ((Naming.VoidActionMode)_model.AuthorizeToVoid.VoidMode).ToString() %>
    </td>


<script runat="server">

    InvoiceItem _model;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (InvoiceItem)this.Model;
    }

</script>
