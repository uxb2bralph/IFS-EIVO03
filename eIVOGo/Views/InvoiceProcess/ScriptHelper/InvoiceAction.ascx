<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
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
        window.uiInvoiceQuery.commitAction = function () {
            if (!$('input[name="chkItem"]').is(':checked')) {
                alert('<%= $"請選擇{_viewModel.ActionTitle}" %>');
                return false;
            }

            $('#theForm').ajaxForm({
                url: "<%= Url.Action(_viewModel.CommitAction,"InvoiceProcess") %>",
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if ($.isPlainObject(data)) {
                        alert(data.message);
                    } else {
                        var $data = $(data);
                        $('body').append($data);
                        //$data.remove();
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
    InquireInvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;
    }
</script>

