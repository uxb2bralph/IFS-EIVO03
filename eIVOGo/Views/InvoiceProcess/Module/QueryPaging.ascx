<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
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

<%  var recordCount = _model.Count();
    if (recordCount > 0)
    { %>
    <%  Html.RenderPartial("~/Views/InvoiceProcess/Module/ItemTable.ascx", _model); %>
    <input type="hidden" name="pageIndex" />
    <nav aria-label="Page navigation">
        <ul class="pagination" id="itemPagination"></ul>
    </nav>
    <script>
        $(function () {
            var obj = $('#itemPagination').twbsPagination({
                totalPages: <%= (recordCount + _viewModel.PageSize - 1) / _viewModel.PageSize %>,
                        totalRecordCount: <%= recordCount %>,
                        visiblePages: 10,
                        first: '最前',
                        prev: '上頁',
                        next: '下頁',
                        last: '最後',
                        initiateStartPageClick: false,
                        onPageClick: function (event, page) {
                            uiInvoiceQuery.inquire(page);
                        }
                    });
                });
    </script>
<%  } %>

<script runat="server">


    ModelSource<InvoiceItem> models;
    IQueryable<InvoiceItem> _model;
    InquireInvoiceViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

        _model = (IQueryable<InvoiceItem>)this.Model;
    }

</script>

