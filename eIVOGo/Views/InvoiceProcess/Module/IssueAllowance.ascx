<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Module.Common" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<div id="<%= _tabID %>">
    <%  ViewBag.ViewModel = _viewModel;
        Html.RenderPartial("~/Views/Forms/SimpleAllowance.ascx"); %>
</div>
<script>
    $(function () {
        $global.createTab('issueAllowance', '開立折讓證明單', $('#<%= _tabID %>'), true);
    });
</script>


<script runat="server">

    ModelSource<InvoiceItem> models;
    ModelStateDictionary _modelState;
    IQueryable<InvoiceItem> _model;
    String _tabID = $"tab{DateTime.Now.Ticks}";
    AllowanceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        _model = (IQueryable<InvoiceItem>)this.Model;

        if (_model.Count() > 0)
        {
            var item = _model.First();
            var prodItem = _model.SelectMany(i => i.InvoiceDetails)
                    .SelectMany(d => d.InvoiceProduct.InvoiceProductItem).ToArray();

            _viewModel = new AllowanceViewModel
            {
                SellerID = item.SellerID,
                BuyerReceiptNo = item.InvoiceBuyer.ReceiptNo,
                BuyerName = item.InvoiceBuyer.Name,
                AllowanceDate = DateTime.Now,
                AllowanceType = Naming.AllowanceTypeDefinition.賣方開立,
                TaxAmount = _model.Sum(i => i.InvoiceAmountType.TaxAmount),
                TotalAmount = _model.Sum(i => i.InvoiceAmountType.SalesAmount),
                ProcessType = item.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.C0401
                    ? Naming.InvoiceProcessType.D0401
                    : Naming.InvoiceProcessType.B0401,
                No = Enumerable.Range(1, prodItem.Length).Select(n => (short)n).ToArray(),
                InvoiceDate = prodItem.Select(p => p.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoiceDate.Value).ToArray(),
                InvoiceNo = prodItem.Select(p => p.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoiceNo()).ToArray(),
                OriginalDescription = prodItem.Select(p => p.InvoiceProduct.Brief).ToArray(),
                OriginalSequenceNo = prodItem.Select(p => p.No).ToArray(),
                Piece = prodItem.Select(p => p.Piece).ToArray(),
                PieceUnit = prodItem.Select(p => p.PieceUnit).ToArray(),
                UnitCost = prodItem.Select(p => p.UnitCost).ToArray(),
                Amount = prodItem.Select(p => p.CostAmount).ToArray(),
                Tax = prodItem.Select(p => Math.Round(p.CostAmount.Value * 0.05m)).ToArray(),
                TaxType = prodItem.Select(p => (Naming.TaxTypeDefinition)p.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoiceAmountType.TaxType).ToArray(),
            };
        }
    }
</script>
