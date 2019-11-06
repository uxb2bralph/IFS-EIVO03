<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Import Namespace="Business.Helper" %>

<%  int idx = 0;
    foreach (var item in _items)
    {
        idx++;  %>
<tr>
    <%  Html.RenderPartial(_dataItemView, item);  %>
</tr>
<%  }
%>

<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<InvoiceItem> _items;
    IQueryable<InvoiceItem> _model;
    IOrderedQueryable<InvoiceItem> _order;
    int[] _sort;
    String _dataItemView;
    InquireInvoiceViewModel _viewModel;
    Dictionary<int, LambdaExpression> _sortExpr;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<InvoiceItem>)this.Model;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

        _dataItemView = ViewBag.DataItemView ?? "~/Views/InvoiceProcess/Module/DataItem.ascx";

        _sort = _viewModel.SortIndex();

        if (ViewBag.SortExpression == null)
        {
            ViewBag.SortExpression = _sortExpr = new Dictionary<int, LambdaExpression>();
        }
        else
        {
            _sortExpr = (Dictionary<int, LambdaExpression>)ViewBag.SortExpression;
        }

        if (_sort != null && _sort.Length > 0)
        {
            sorting();
            sortBy();
        }
        else
        {
            _model = _model.OrderByDescending(i => i.InvoiceID);
        }

        _items = _model
            .Skip((int)_viewModel.PageIndex * (int)_viewModel.PageSize)
            .Take((int)_viewModel.PageSize);
    }

    void sorting()
    {
        _sortExpr.Add(1, (Expression<Func<InvoiceItem, DateTime?>>)(p => p.InvoiceDate));
        _sortExpr.Add(2, (Expression<Func<InvoiceItem, String>>)(i => i.InvoiceBuyer.CustomerID));
        _sortExpr.Add(3, (Expression<Func<InvoiceItem, String>>)(i => i.InvoicePurchaseOrder.OrderNo));
        _sortExpr.Add(4, (Expression<Func<InvoiceItem, String>>)(i => i.Organization.CompanyName));
        _sortExpr.Add(5, (Expression<Func<InvoiceItem, String>>)(i => i.Organization.ReceiptNo));
        _sortExpr.Add(7, (Expression<Func<InvoiceItem, decimal?>>)(i => i.InvoiceAmountType.SalesAmount));
        _sortExpr.Add(8, (Expression<Func<InvoiceItem, decimal?>>)(i => i.InvoiceAmountType.TaxAmount));
        _sortExpr.Add(9, (Expression<Func<InvoiceItem, decimal?>>)(i => i.InvoiceAmountType.TotalAmount));
        //_sortExpr.Add(10, (Expression<Func<InvoiceItem, int>>)(i => i.InvoiceWinningNumber != null ? i.InvoiceWinningNumber.UniformInvoiceWinningNumber.Rank : 0));
        _sortExpr.Add(10, (Expression<Func<InvoiceItem, int?>>)(i => (int?)i.InvoiceWinningNumber.UniformInvoiceWinningNumber.Rank));
        _sortExpr.Add(11, (Expression<Func<InvoiceItem, String>>)(i => i.InvoiceBuyer.ReceiptNo));
        _sortExpr.Add(6, (Expression<Func<InvoiceItem, String>>)(i => i.TrackCode + i.No));
    }

    void sortBy()
    {
        Expression sort;
        int idx = _sort[0];
        if (idx > 0)
        {
            sort = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { typeof(InvoiceItem), _sortExpr[idx].Body.Type }, _model.Expression, _sortExpr[idx]);
        }
        else
        {
            sort = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { typeof(InvoiceItem), _sortExpr[-idx].Body.Type }, _model.Expression, _sortExpr[-idx]);
        }
        _model = _model.Provider.CreateQuery<InvoiceItem>(sort);


        for (int i = 1; i < _sort.Length; i++)
        {
            idx = _sort[i];
            if (idx > 0)
            {
                sort = Expression.Call(typeof(Queryable), "ThenBy", new Type[] { typeof(InvoiceItem), _sortExpr[idx].Body.Type }, _model.Expression, _sortExpr[idx]);
            }
            else
            {
                sort = Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { typeof(InvoiceItem), _sortExpr[-idx].Body.Type }, _model.Expression, _sortExpr[-idx]);
            }
            _model = _model.Provider.CreateQuery<InvoiceItem>(sort);
        }
    }




</script>
