<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "查詢結果"); %>

<div class="border_gray" style="overflow-x: auto;">
    <%  var recordCount = _model.Count();
        if(recordCount>0)
        { %>
            <%  Html.RenderPartial("~/Views/BusinessRelationship/Module/ItemList.ascx",_model); %>
            <nav aria-label="Page navigation">
                <ul class="pagination" id="businessPagination"></ul>
            </nav>
    <span>總相對營業人數：<%= recordCount %>，總分店數：<%= _model.SelectMany(m=>m.Counterpart.OrganizationBranch).Count() %></span>
            <script>
                $(function () {
                    var obj = $('#businessPagination').twbsPagination({
                                totalPages: <%= (recordCount+_viewModel.PageSize-1) / _viewModel.PageSize %>,
                                totalRecordCount: <%= recordCount %>,
                                visiblePages: 10,
                                first: '最前',
                                prev: '上頁',
                                next: '下頁',
                                last: '最後',
                                initiateStartPageClick: false,
                                onPageClick: function (event, page) {
                                    uiInquireBusiness.inquireBusiness(page,function(data){
                                        var $node = $('.businessList').next();
                                        $('.businessList').remove();
                                        $node.before(data);
                                    });
                                }
                            });
                        });
            </script>
    <%  }
        else
        { %>
            <font color="red">查無資料!!</font>
            <%  Html.RenderPartial("~/Views/BusinessRelationship/Module/ItemList.ascx",_model); %>
    <%  } %>
</div>

<script runat="server">

    IQueryable<BusinessRelationship> _model;
    BusinessRelationshipQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (IQueryable<BusinessRelationship>)this.Model;
        _viewModel = (BusinessRelationshipQueryViewModel)ViewBag.ViewModel;

    }

</script>

