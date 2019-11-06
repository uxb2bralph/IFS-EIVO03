<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>

<div class="border_gray">
    選擇電子發票型式：
    <input type="radio" name="InvoiceProcessType" value="<%= (int)Naming.InvoiceProcessType.C0401 %>" <%= _viewModel.InvoiceProcessType == Naming.InvoiceProcessType.C0401 ? "checked" : null %> />熱感紙證明聯(B2C、一般B2B)&nbsp;&nbsp;
    <input type="radio" name="InvoiceProcessType" value="<%= (int)Naming.InvoiceProcessType.A0401 %>" <%= _viewModel.InvoiceProcessType == Naming.InvoiceProcessType.A0401 ? "checked" : null %> />B2B存證&nbsp;&nbsp;
    <input type="radio" name="InvoiceProcessType" value="<%= (int)Naming.InvoiceProcessType.A0101 %>" <%= _viewModel.InvoiceProcessType == Naming.InvoiceProcessType.A0101 ? "checked" : null %> />B2B交換
    <script>
        debugger;
        $(function () {
            var $input = $('input:radio[name="InvoiceProcessType"]');
            <%  if(_viewModel.InvoiceProcessType.HasValue)
                {   %>
            $('input:radio[name="InvoiceProcessType"][value="<%= (int?)_viewModel.InvoiceProcessType %>"]').prop('checked', true);
            <%  }   %>
            $input.on('click', function (event) {
                var event = event || window.event;
                $('').launchDownload('<%= Url.Action("CreateInvoice","InvoiceBusiness") %>', { 'invoiceProcessType': $(this).val() });
            });
        });
    </script>
</div>

<script runat="server">

    InvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ViewBag.ActionName = "首頁 > 電子發票開立";
        _viewModel = (InvoiceViewModel)ViewBag.ViewModel;
    }
</script>
