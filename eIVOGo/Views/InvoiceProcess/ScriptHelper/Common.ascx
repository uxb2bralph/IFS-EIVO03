﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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

<script>
    var uiInvoiceQuery;
    $(function () {
        var $postForm;
        uiInvoiceQuery = {
            inquire: function (pageNum, onPaging) {

                var $this = uiInvoiceQuery;
                var $placement = $('button:contains("查詢")').closest('table');
                var $formData = $('#queryArea').find('input,select,textarea').serializeObject();

                if (isNaN(pageNum)) {
                    $formData.startQuery = true;
                    uiInvoiceQuery.pageIndex = 1;
                }
                else {
                    $formData.pageIndex = pageNum;
                    $this.pageIndex = pageNum;
                }

                if ($this.sort) {
                    $formData.Sort = $this.sort.getSort();
                }

                $formData.actionTitle = '<%= _viewModel.ActionTitle %>';

                showLoading();
                $.post('<%= Url.Action("Inquire","InvoiceProcess",new { resultAction = ViewBag.ResultAction }) %>', $formData, function (data) {

                    hideLoading();
                    if ($.isPlainObject(data)) {
                        alert(data.message);
                    } else {
                        if ($this.initQuery) {

                            if (uiInvoiceQuery.$result)
                                uiInvoiceQuery.$result.remove();
                            uiInvoiceQuery.$result = $(data).insertAfter($placement);
                            $this.initQuery = false;
                            //$this.showHeader();
                            //$this.checkColumns();
                        } else {
                            $this.paging(data);
                            //$this.checkColumns();
                        }
                    }
                });
            },
            print: function (style,processType) {
                if (!$('input[name="chkItem"]').is(':checked')) {
                    alert('請選擇列印資料!!');
                    return false;
                }

                var $formData = $('#theForm').serializeObject();
                $formData.paperStyle = style;
                //$formData.processType = processType;

                showLoading();
                $.post('<%= Url.Action("Print","InvoiceProcess") %>', $formData, function (data) {
                    hideLoading();
                    if ($.isPlainObject(data)) {
                        alert(data.message);
                    } else {
                        if (data) {
                            var $data = $(data);
                            $data.dialog();
                            $data.find('a').on('click', function (evt) {
                                $data.dialog('close');
                            });
                        }
                    }
                });
            },
            download: function () {
                if ($postForm) {
                    $postForm.remove();
                }

                $postForm = $('<form method="post" />').prop('action', '<%= Url.Action("CreateXlsx","InvoiceProcess") %>')
                    .css('display', 'none').appendTo($('body'));

                $('#theForm').serializeArray().forEach(function (item, index) {
                    $('<input type="hidden">')
                        .prop('name', item.name).prop('value', item.value)
                        .appendTo($postForm);
                });
                $postForm.submit();
                //showLoading();
            },
            saveBuyer: function () {
                if ($postForm) {
                    $postForm.remove();
                }

                $postForm = $('<form method="post" />').prop('action', '<%= Url.Action("ExportInvoiceBuyer","InvoiceProcess") %>')
                    .css('display', 'none').appendTo($('body'));

                $('#theForm').serializeArray().forEach(function (item, index) {
                    $('<input type="hidden">')
                        .prop('name', item.name).prop('value', item.value)
                        .appendTo($postForm);
                });
                $postForm.submit();
                //showLoading();
            },
            editBuyer: function (keyID) {
                var event = event || window.event;
                var $tr = $(event.target).closest('tr');

                uiInvoiceQuery.refreshDataRow = function () {
                    $tr.empty();
                    showLoading();
                    $.post('<%= Url.Action("LoadInvoiceItem", "InvoiceProcess",new { _viewModel.ResultAction }) %>', { 'keyID': keyID }, function (data) {
                        hideLoading();
                        if ($.isPlainObject(data)) {
                            alert(data.message);
                        } else {
                            $(data).appendTo($('body')).appendTo($tr);
                        }
                    });
                };

                showLoading();
                $.post('<%= Url.Action("EditInvoiceBuyer", "InvoiceProcess") %>', {'keyID':keyID}, function (data) {
                    hideLoading();
                    if ($.isPlainObject(data)) {
                        alert(data.message);
                    } else {
                        $(data).appendTo($('body'));
                    }
                });
            },
        };
    });
</script>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InquireInvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

    }
</script>

