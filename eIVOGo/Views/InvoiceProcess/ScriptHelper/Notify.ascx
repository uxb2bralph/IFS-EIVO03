﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<script>
    $(function () {
        var $postForm;
        if (!window.uiInvoiceQuery) {
            window.uiInvoiceQuery = {};
        }
        window.uiInvoiceQuery.notify = function () {
            if (!$('input[name="chkItem"]').is(':checked')) {
                alert('請選擇重送資料!!');
                return false;
            }

            var $formData = $('input[name="chkItem"]:checked').serializeObject();
            $formData.cancellation = $('#queryArea select[name="Cancelled"]').val();
            $formData.processType = $('#queryArea select[name="ProcessType"]').val();

            showLoading();
            $.post('<%= Url.Action("IssueInvoiceNotice", "InvoiceProcess") %>', $formData, function (data) {
                hideLoading();
                if ($.isPlainObject(data)) {
                    alert(data.message);
                } else {
                    $(data).appendTo($('body')).remove();
                }
            });

        };
        window.uiInvoiceQuery.authorize = function () {
            if (!$('input[name="chkItem"]').is(':checked')) {
                alert('請選擇核准重印資料!!');
                return false;
            }

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("AuthorizeToPrint","InvoiceProcess") %>",
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        var $data = $(data);
                        $('body').append($data);
                        $data.remove();
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

        window.uiInvoiceQuery.voidInvoice = function (mode) {
            if (!$('input[name="chkItem"]').is(':checked')) {
                alert('請選擇註銷資料!!');
                return false;
            }

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("VoidInvoice","InvoiceProcess") %>" + '?mode=' + mode,
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        var $data = $(data);
                        $('body').append($data);
                        $data.remove();
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

        window.uiInvoiceQuery.desireToVoid = function (allow) {
            if (!$('input[name="chkItem"]').is(':checked')) {
                alert('請選擇核准註銷資料!!');
                return false;
            }

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("DesireToVoidInvoice","InvoiceProcess") %>" + '?allow=' + allow,
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        var $data = $(data);
                        $('body').append($data);
                        $data.remove();
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

    });
</script>
<script runat="server">

    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

    }
</script>

