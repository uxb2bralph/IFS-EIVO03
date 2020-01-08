<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

    <td><input name="chkItem" type="checkbox" value="<%= _model.InvoiceID %>" /></td>
    <td><%= String.Format("{0:yyyy/MM/dd}",_model.InvoiceDate) %></td>
    <td><%= _model.InvoiceBuyer.CustomerID %></td>
    <td><%= _model.InvoicePurchaseOrder!=null ? _model.InvoicePurchaseOrder.OrderNo : null %></td>
    <td><%= _model.InvoiceSeller?.CustomerName %></td>
    <td><%= _model.InvoiceSeller?.ReceiptNo %></td>
    <td><a onclick="showInvoiceModal(<%= _model.InvoiceID %>);"><%= _model.TrackCode %><%= _model.No %></a></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.SalesAmount) %></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxAmount) %></td>
    <td align="right"><%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TotalAmount) %></td>
    <td><%= _model.InvoiceAmountType.CurrencyType?.AbbrevName %></td>
    <td><%= _model.InvoiceWinningNumber!=null ? _model.InvoiceWinningNumber.UniformInvoiceWinningNumber.PrizeType : "N/A" %></td>
    <td><%= _model.InvoiceBuyer.IsB2C() ? "" : _model.InvoiceBuyer.ReceiptNo %></td>
    <td><%= _model.InvoiceCancellation==null ? "" : $"已作廢({_model.InvoiceCancellation.CancelDate:yyyy/MM/dd})" %></td>
    <td><%= _model.InvoiceBuyer.CustomerName %></td>
    <td><%= _model.InvoiceBuyer.ContactName %></td>
    <td><%= _model.InvoiceBuyer.Address %></td>
    <td><%= _model.InvoiceBuyer.EMail %></td>
    <td><%= String.Join("", _model.InvoiceDetails.Select(t => t.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)) %></td>
    <td><%= _model.CDS_Document.IssuingNotice!=null ? String.Format("{0:yyyy/MM/dd HH:mm:ss}",_model.CDS_Document.IssuingNotice.IssueDate) : "未通知" %></td>
    <td><%= _model.InvoiceCarrier!=null ? _model.InvoiceCarrier.CarrierNo : null %></td>
    <td><%= _model.PrintMark %></td>
    <td style="white-space: nowrap;">
        <%  var attachment = _model.CDS_Document.Attachment.FirstOrDefault();
            if (attachment != null)
            {   %>
        <a href="<%= $"{VirtualPathUtility.ToAbsolute("~/Helper/DownloadAttachment.ashx")}?keyName={attachment.KeyName}" %>"><%= attachment.KeyName %></a>
        <%  } %>
    </td>
    <td>
        <% if (attachment != null)
            {   %>
        <%= attachment.GetAttachedPdfPageCount() %>
        <%  } %>
    </td>
    <td style="white-space: nowrap;">
        <%  if (_viewModel.ResultAction == "Process")
            {
                Html.RenderPartial("~/Views/InvoiceProcess/DataAction/DataProcess.cshtml", _model);
            } %>
    </td>


<script runat="server">

    InvoiceItem _model;
    InquireInvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (InvoiceItem)this.Model;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

    }

</script>
