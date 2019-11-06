<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
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

<table id="<%= _listID %>" class="table01 itemList">
    <thead>
        <tr>
            <th aria-sort="other">
                <input name="chkAll" type="checkbox" /></th>
            <th data-sort="1">日期</th>
            <th data-sort="2">Customer ID</th>
            <th data-sort="3">開立發票營業人</th>
            <th data-sort="4">統編</th>
            <th data-sort="5">折讓號碼</th>
            <th data-sort="6">未稅金額</th>
            <th data-sort="7">稅額</th>
            <th aria-sort="other">含稅金額</th>
            <th data-sort="9">買受人統編</th>
            <th aria-sort="other">備註</th>
        </tr>
    </thead>
    <tbody>
        <%  Html.RenderPartial("~/Views/AllowanceProcess/Module/ItemList.ascx", _model); %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="11">
                未稅金額總計：&nbsp;<%= String.Format("{0:##,###,###,##0.##}",_model.Sum(i=>i.TotalAmount)) %>，
                稅額總計：&nbsp;<%= String.Format("{0:##,###,###,##0.##}",_model.Sum(i=>i.TaxAmount)) %>

                <script>
                    debugger;
                    $(function () {

                        var $table = $('#<%= _listID %>');

                        uiAllowanceQuery.paging = function (data) {
                            //uiAllowanceQuery.pagePlacement.html(data);
                            var $tbody = $table.find('tbody');
                            $tbody.empty();
                            var $data = $(data);
                            $data.appendTo($('body')).appendTo($tbody);
                        };

                        uiAllowanceQuery.sort = (function () {
                            var orderBy = [];

                            initSortable($table, uiAllowanceQuery.inquire, uiAllowanceQuery.pageIndex, orderBy);

                            this.getSort = function () {
                                return orderBy;
                            };

                            return this;
                        })();

                        var chkBox = $(".itemList input[name='chkAll']");
                        var chkItem = $(".itemList input[name='chkItem']");
                        chkBox.click(function () {
                            chkItem.prop('checked', chkBox.is(':checked'));
                        });

                        chkItem.click(function (e) {
                            if (!$(this).is(':checked')) {
                                chkBox.prop('checked', false);
                            }
                        });

                    });
                </script>
            </td>
        </tr>
    </tfoot>
</table>

<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<InvoiceAllowance> _model;
    String _listID = "itemList" + DateTime.Now.Ticks;
    InquireInvoiceViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<InvoiceAllowance>)this.Model;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

    }

</script>
