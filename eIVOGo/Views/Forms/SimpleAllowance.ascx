﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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
<%@ Import Namespace="Business.Helper" %>

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "開立折讓證明單"); %>
<form id="<%= _formID %>" role="form" method="post">
    <div class="border_gray">
        <!--表格 開始-->
        <h2>營業人</h2>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
            <tr>
                <th nowrap="nowrap" width="120">發票開立人
                </th>
                <td class="tdleft">
                    <%  var seller = models.GetTable<Organization>().Where(o => o.CompanyID == _viewModel.SellerID).First();    %>
                    <%= seller.ReceiptNo %> <%= seller.CompanyName %>
                </td>
                <th nowrap="nowrap" width="120">買受人
                </th>
                <td class="tdleft">
                    <%  if (_viewModel.BuyerReceiptNo != "0000000000")
                        {   %>
                    <%= _viewModel.BuyerReceiptNo %> <%= _viewModel.BuyerName %>
                    <%  } %>
                </td>
            </tr>
            <tr>
                <th nowrap="nowrap">營業稅額合計
                </th>
                <td class="tdleft">
                    <input class="form-control" name="TaxAmount" type="text" value="" />
                </td>
                <th nowrap="nowrap">未稅金額總計
                </th>
                <td class="tdleft">
                    <input class="form-control" name="TotalAmount" type="text" value="" />
                </td>
            </tr>
        </table>
        <!--表格 結束-->
    </div>
    <div class="border_gray" style="overflow-x:auto;">
        <!--表格 開始-->
        <h2>折讓明細</h2>
        <div><input type="checkbox" name="cbTaxed" checked="checked" onchange="<%= _formID %>.calc(true);" />貨品單價含稅</div>
        <table class="table table-striped table-bordered table-hover dataTable no-footer dtr-inline left_title itemList">
            <thead>
                <tr>
                    <th style="min-width: 120px;">原發票日期</th>
                    <th style="min-width: 140px;">原發票號碼</th>
                    <%--<th>原明細排列序號</th>--%>
                    <th style="min-width: 180px;">原品名</th>
                    <th style="min-width: 60px;">數量</th>
                    <%--<th style="min-width: 60px;">單位</th>--%>
                    <th style="min-width: 100px;">單價</th>
                    <th style="min-width: 120px;">未稅金額</th>
                    <th style="min-width: 120px;">營業稅額</th>
                    <th>課稅別</th>
                </tr>
            </thead>
            <tbody>
                <%  for (int i = 0; i < _viewModel.OriginalSequenceNo.Length; i++)
                    {%>
                <tr>
                    <td>
                        <input class="form-control" name="InvoiceDate" type="text" value="<%= $"{_viewModel.InvoiceDate[i]:yyyy/MM/dd}" %>" readonly="readonly" /></td>
                    <td>
                        <input class="form-control" name="InvoiceNo" type="text" value="<%= _viewModel.InvoiceNo[i] %>" readonly="readonly" />
                        <input class="form-control" name="OriginalSequenceNo" type="hidden" value="<%= _viewModel.OriginalSequenceNo[i] %>" />
                    </td>
                    <%--<td>                    </td>--%>
                    <td>
                        <input class="form-control" name="OriginalDescription" type="text" value="<%= _viewModel.OriginalDescription[i] %>"/></td>
                    <td>
                        <input class="form-control" name="Piece" type="number" step="1" min="1" value="<%= $"{_viewModel.Piece[i]:##,###,###,##0.##}" %>" onblur="<%= _formID %>.calcSingle(<%= i %>);"/>
                        <input class="form-control" name="PieceUnit" type="hidden" value="<%= _viewModel.PieceUnit[i] %>" />
                    </td>
                    <%--<td>                    </td>--%>
                    <td>
                        <input class="form-control" name="UnitCost" type="number" value="<%= $"{_viewModel.UnitCost[i]:##,###,###,##0.##}" %>"  onblur="<%= _formID %>.calcSingle(<%= i %>);"/></td>
                    <td>
                        <input class="form-control" name="Amount" type="number" value="<%--<%= $"{_viewModel.Amount[i]:##,###,###,##0.##}" %>--%>"/></td>
                    <td>
                        <input class="form-control" name="Tax" type="number" value="<%--<%= $"{_viewModel.Tax[i]:##,###,###,##0.##}" %>--%>"/></td>
                    <td>
                        <input  name="TaxType" type="hidden" value="<%= (int)_viewModel.TaxType[i] %>"/><%= _viewModel.TaxType[i] %>
                    </td>
                </tr>
                <%  } %>
            </tbody>
        </table>
    </div>
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr>
                <td class="Bargain_btn">
                    <a class="btn" onclick="<%= _formID %>.sum();">小計</a>
                    <a class="btn" onclick="<%= _formID %>.commit();">開立</a>
                    <button type="reset" class="btn">清除</button>
                </td>
            </tr>
        </tbody>
    </table>
</form>
<script>
    var <%= _formID %>;
    $(function () {
        var $form = $('#<%= _formID %>');
        var formObject = {
            calc: function (calcAll) {
                //for (var idx = 0; idx < $form.find('input[name="Piece"]').length; idx++) {
                //    this.calcSingle(idx);
                //}

                var $tax = $form.find('input[name="Tax"]');
                var $amt = $form.find('input[name="Amount"]');

                $amt.each(function (idx) {
                    var tax = $tax.eq(idx).val();
                    var amt = $amt.eq(idx).val();
                    if (calcAll == true || amt == '' || isNaN(amt) || isNaN(tax)) {
                        formObject.calcSingle(idx);
                    }
                });

            },
            calcSingle: function (idx) {
                var piece = Number($form.find('input[name="Piece"]').eq(idx).val());
                var cost = Number($form.find('input[name="UnitCost"]').eq(idx).val());
                var taxType = Number($form.find('input[name="TaxType"]').eq(idx).val());
                var $amount = $form.find('input[name="Amount"]').eq(idx);
                var $tax = $form.find('input[name="Tax"]').eq(idx);

                if (isNaN(piece) || isNaN(cost) || isNaN(taxType) || taxType != 1) {
                    return;
                }

                var totalCost = cost * piece;
                if ($form.find('input:checkbox[name="cbTaxed"]').is(':checked')) {
                    var amount = math.round(totalCost / 1.05);
                    var tax = totalCost - amount;
                    $amount.val(amount);
                    $tax.val(tax);
                } else {
                    $amount.val(totalCost);
                    $tax.val(math.round(totalCost * 0.05));
                }
            },
            commit: function () {
                if (this.sum()) {
                    var $formData = $form.serializeObject();
                    $formData.buyerReceiptNo = '<%= _viewModel.BuyerReceiptNo %>';
                    $formData.buyerName = '<%= _viewModel.BuyerName %>';
                    showLoading();
                    $.post('<%= Url.Action("CommitAllowance","InvoiceBusiness",new { _viewModel.ProcessType,KeyID = seller.CompanyID.EncryptKey() }) %>', $formData, function (data) {
                        hideLoading();
                        if ($.isPlainObject(data)) {
                            alert(data.message);
                        } else {
                            var $data = $(data);
                            $data.find('a').on('click', function (evt) {
                                $data.dialog('close');
                            });
                            $data.appendTo($('body'));
                            $data.dialog();
                        }
                    });
                }
            },

            sum: function () {

                this.calc();

                var taxAmt = 0;
                var totalAmt = 0;
                var $tax = $form.find('input[name="Tax"]');
                var $amt = $form.find('input[name="Amount"]');

                $tax.each(function (idx) {
                    var tax = math.number($tax.eq(idx).val());
                    if (!isNaN(tax)) {
                        taxAmt += tax;
                    } 
                });

                $amt.each(function (idx) {
                    var amt = math.number($amt.eq(idx).val());
                    if (!isNaN(amt)) {
                        totalAmt += amt;
                    }
                });

                $form.find('input[name="TotalAmount"]').val(totalAmt);
                $form.find('input[name="TaxAmount"]').val(taxAmt);

                return true;
            },
        };

        <%= _formID %> = formObject;
    });
</script>

<script runat="server">

    ModelSource<InvoiceItem> models;
    ModelStateDictionary _modelState;
    AllowanceViewModel _viewModel;
    String _formID = $"form{DateTime.Now.Ticks}";

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        _viewModel = (AllowanceViewModel)ViewBag.ViewModel;
    }
</script>
