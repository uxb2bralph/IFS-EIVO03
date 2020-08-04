<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JsGridInitialization.ascx.cs" Inherits="eIVOGo.Module.JsGrid.JsGridInitialization" %>
<%@ Import Namespace="eIVOGo.Module.JsGrid" %>
<%@ Import Namespace="Utility" %>

<script>
    $(function () {

        var f = <%= FieldName!=null ? FieldName : JsGridHelper.JsGridFieldArray(FieldObject) %>;

        var grid = $eivo.preparJsGrid('<%= JsGridSelector %>', f, '<%= FieldsContainerSelector %>');
<%  int size;
    if (Request.GetRequestValue("queryPageSize", out size)) { %>
        grid.gridConfig.pageSize = <%= size %>;
<%  }  %>
        grid.gridConfig.controller = {
            loadData: function (filter) {

                var d = $.Deferred();

                $.post('<%= DataSourceUrl %>?q=1&index=' + filter.pageIndex + '&size=' + filter.pageSize, $('form').serialize())
                .done(function (response) {
                    d.resolve(response);
                });

                return d.promise();
            }
        };

        <% if(!AllowPaging) { %>
        grid.gridConfig.paging = false;
        grid.gridConfig.pageSize = <%= GetRecordCount() %>;
        <% } %>

        <% if(PrintMode) { %>
        grid.gridConfig.onDataLoaded = function(args) {
            setTimeout(function() {self.focus();self.print();},1000);
        };
        var $colIdx = $('input:checkbox[name="colIdx"]');
        $colIdx.prop('checked',false);
        $([<%= Request["colIdx"]%>]).each(function() {
            $colIdx.eq(this).prop('checked',true);
        });

        grid.gridConfig.fields = $eivo.resetJsGridFields(f);

        <% } %>
        grid.jsGrid.jsGrid(grid.gridConfig);

        $("<%= JsGridSelector %>").jsGrid({
            noDataContent: "<%= eIVOGo.Resource.Views.Common.Page.查無資料%>",
            pagerFormat: "{first} | {prev} | {pages} | {next} | {last} &nbsp;&nbsp; {pageIndex} / {pageCount} &nbsp;&nbsp;&nbsp;&nbsp; " + "<%=eIVOGo.Resource.Views.Common.Page.總筆數_%>"+" {itemCount}",
            pageNextText: "<%=eIVOGo.Resource.Views.Common.Page.下一頁%>",
            pagePrevText: "<%=eIVOGo.Resource.Views.Common.Page.上一頁%>",
            pageFirstText: "<%=eIVOGo.Resource.Views.Common.Page.首頁%>",
            pageLastText: "<%=eIVOGo.Resource.Views.Common.Page.末頁%>",
            pageNavigatorNextText: "<%=eIVOGo.Resource.Views.Common.Page.下10頁%>",
            pageNavigatorPrevText: "<%=eIVOGo.Resource.Views.Common.Page.上10頁%>",
        });
    });
    
</script>
