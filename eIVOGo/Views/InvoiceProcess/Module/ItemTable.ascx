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

<table id="<%= _listID %>" class="table01 itemList">
    <thead>
        <tr>
            <th aria-sort="other">
                <input name="chkAll" type="checkbox" /></th>
            <th data-sort="1">日期</th>
            <th data-sort="2">客戶 ID</th>
            <th data-sort="3">序號</th>
            <th data-sort="4">開立發票營業人</th>
            <th data-sort="5">統編</th>
            <th data-sort="6">發票號碼</th>
            <th data-sort="7">未稅金額</th>
            <th data-sort="8">稅額</th>
            <th data-sort="9">含稅金額</th>
            <th aria-sort="other">幣別</th>
            <th data-sort="10">是否中獎</th>
            <th data-sort="11">買受人統編</th>
            <th aria-sort="other">發票狀態</th>
            <th aria-sort="other">名稱</th>
            <th aria-sort="other">連絡人名稱</th>
            <th aria-sort="other">地址</th>
            <th aria-sort="other">email</th>
            <th aria-sort="other">備註</th>
            <th aria-sort="other">開立通知</th>
            <th aria-sort="other">載具資訊</th>
            <th aria-sort="other">列印註記</th>
            <th aria-sort="other">附件檔</th>
            <th aria-sort="other">附件檔頁數</th>
            <th aria-sort="other" style="min-width: 140px;">管理</th>
        </tr>
    </thead>
    <tbody>
        <%  Html.RenderPartial("~/Views/InvoiceProcess/Module/ItemList.ascx", _model); %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="25">
                <%
                    foreach(var g in _model.GroupBy(i=>i.InvoiceAmountType.CurrencyID))
                    {
                        var currency = g.Key.HasValue ? models.GetTable<CurrencyType>().Where(c => c.CurrencyID == g.Key).FirstOrDefault()?.AbbrevName : "TWD"; %>
                含稅金額總計(<%= currency %>)：&nbsp;<%= String.Format("{0:##,###,###,##0.##}",g.Sum(i=>i.InvoiceAmountType.TotalAmount)) %>，
                未稅金額總計(<%= currency %>)：&nbsp;<%= String.Format("{0:##,###,###,##0.##}",g.Sum(i=>i.InvoiceAmountType.SalesAmount)) %>，
                稅額總計(<%= currency %>)：&nbsp;<%= String.Format("{0:##,###,###,##0.##}",g.Sum(i=>i.InvoiceAmountType.TaxAmount)) %><br />
                <%  }
                    %>

                <script>
                    debugger;
                    $(function () {

                        var $table = $('#<%= _listID %>');

                        uiInvoiceQuery.paging = function (data) {
                            //uiInvoiceQuery.pagePlacement.html(data);
                            var $tbody = $table.find('tbody');
                            $tbody.empty();
                            var $data = $(data);
                            $data.appendTo($('body')).appendTo($tbody);

                            $(".itemList input[name='chkAll']").prop('checked', false);
                            $(".itemList input[name='chkItem']").click(function (e) {
                                if (!$(this).is(':checked')) {
                                    $(".itemList input[name='chkAll']").prop('checked', false);
                                }
                            });
                        };

                        uiInvoiceQuery.sort = (function () {
                            var orderBy = [];

                            initSortable($table, uiInvoiceQuery.inquire, uiInvoiceQuery.pageIndex, orderBy);

                            this.getSort = function () {
                                return orderBy;
                            };

                            return this;
                        })();

                        $(".itemList input[name='chkAll']").click(function () {
                            $(".itemList input[name='chkItem']")
                                .prop('checked', $(this).is(':checked'));
                        });

                        $(".itemList input[name='chkItem']").click(function (e) {
                            if (!$(this).is(':checked')) {
                                $(".itemList input[name='chkAll']").prop('checked', false);
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
    IQueryable<InvoiceItem> _model;
    String _listID = "itemList" + DateTime.Now.Ticks;
    InquireInvoiceViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<InvoiceItem>)this.Model;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

    }

</script>
