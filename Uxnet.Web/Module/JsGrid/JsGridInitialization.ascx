<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JsGridInitialization.ascx.cs" Inherits="Uxnet.Web.Module.JsGrid.JsGridInitialization" %>
<%@ Import Namespace="Uxnet.Web.Module.JsGrid" %>
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
            setTimeout(function() {self.print();},1000);
        };
        var $colIdx = $('input[name="colIdx"]');
        $colIdx.prop('checked',false);
        $([<%= Request["colIdx"]%>]).each(function() {
            $colIdx.eq(this).prop('checked',true);
        });

        grid.gridConfig.fields = $eivo.resetJsGridFields(f);

        <% } %>
        grid.jsGrid.jsGrid(grid.gridConfig);
    });
</script>
