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

    <input type="hidden" name="pageIndex" />
    <nav aria-label="Page navigation">
        <ul class="pagination" id="<%= _paginationID %>"></ul>
    </nav>
    <script>
        $(function () {
            var obj = $('#<%= _paginationID %>').twbsPagination({
                totalPages: <%= (_viewModel.RecordCount + _viewModel.PageSize - 1) / _viewModel.PageSize %>,
                        totalRecordCount: <%= _viewModel.RecordCount %>,
                        visiblePages: 10,
                        first: '最前',
                        prev: '上頁',
                        next: '下頁',
                        last: '最後',
                        initiateStartPageClick: false,
                        onPageClick: function (event, page) {
                            <%= _viewModel.OnPageCallScript %>;
                        }
                    });
                });
    </script>

<script runat="server">

    ModelSource<InvoiceItem> models;
    QueryViewModel _viewModel;
    String _paginationID = "itemPagination" + DateTime.Now.Ticks;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (QueryViewModel)ViewBag.ViewModel;
    }

</script>

