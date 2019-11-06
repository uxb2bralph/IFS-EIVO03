﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
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
<%@ Import Namespace="Business.Helper" %>

<div id="<%= _dialogID %>">

    <div class="border_gray">
        <table class="table01 itemList" cellspacing="0" cellpadding="0" style="width: 100%;">
            <thead>
                <tr>
                    <th><%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year %>/<%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2-1 %>~<%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2 %></th>
                    <th><%=_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode %><%= _model.StartNo %>~<%= _model.EndNo %></th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th>平均分配本數</th>
                    <td>
                        <input type="text" name="Parts" value="<%: _viewModel.Parts %>" />
                    </td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="2" style="text-align:center;">
                        <input type="button" value="確定" class="btn"/>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>

    <script>
        $('#<%= _dialogID %>').dialog({
            //autoOpen: false,
            width: 'auto',
            resizable: true,
            //modal: true,
            title: "<div class='modal-title'><h4><i class='fa fa-clock-o'></i> 配號區間分配本數</h4></div>",
            buttons: [
            ],
            close: function (event, ui) {
                $('#<%= _dialogID %>').remove();
            }
        });

        $(function () {
            $('#<%= _dialogID %> input:button[value="確定"]').on('click', function () {

                var event = event || window.event;
                var $formData = $('#<%= _dialogID %>').find('input,select,textarea').serializeObject();
                clearErrors();

                showLoading();
                $.post('<%= Url.Action("CommitAllotment", "InvoiceNo", new { keyID = _model.IntervalID.EncryptKey() }) %>', $formData, function (data) {
                    hideLoading();
                    if ($.isPlainObject(data)) {
                        if (data.result) {
                            alert('作業完成!!');
                            $('#<%= _dialogID %>').dialog("close");
                            if (uiTrackCodeNo.inquire) {
                                uiTrackCodeNo.inquire();
                            }
                        } else {
                            alert(data.message);
                        }
                    } else {
                        $(data).appendTo($('body'));
                    }
                });
            });

            $('#<%= _dialogID %> input:button[value="關閉"]').on('click', function () {
                $('#<%= _dialogID %>').dialog("close");
            });

            $('#<%= _dialogID %>').find('input,select,textarea').attr('data-role', '<%= _dialogID %>');
        });
    </script>

</div>


<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    InvoiceNoInterval _model;
    InvoiceNoIntervalViewModel _viewModel;
    String _dialogID = "allot" + DateTime.Now.Ticks;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = this.Model as InvoiceNoInterval;
        _viewModel = (InvoiceNoIntervalViewModel)ViewBag.ViewModel;

    }

</script>