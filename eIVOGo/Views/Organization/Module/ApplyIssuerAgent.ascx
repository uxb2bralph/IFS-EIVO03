<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "設定發票代傳營業人"); %>
<form>
    <div class="panel panel-default">
    <div class="panel-heading">
        發票代傳營業人
    </div>
    <div class="panel-body">
        <%
            var items = models.GetTable<Organization>().Where(o => o.OrganizationCategory.Any(c => c.CategoryID == (int)Naming.CategoryID.COMP_INVOICE_AGENT));
            foreach (var item in items)
            { %>
                <label class="radio">
                <input type="checkbox" name="agentID" value="<%= item.CompanyID %>" <%= item.InvoiceInsurerAgent.Any(i=>i.IssuerID==_model.CompanyID) ? "checked" : null %>  />
                <i></i> <%= String.Concat("(",item.ReceiptNo,")",item.CompanyName) %>
                </label>
    <%      }   %>
    </div>
</div>
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="table-responsive">
                <!--表格 開始-->
                <table class="table table-striped table-bordered table-hover">
                    <tr>
                        <td class="Bargain_btn" align="center">
                            <button type="button" onclick="$global.commitIssuerAgent();" name="UpdateButton" class="btn">確定</button>
                        &nbsp;
            <input name="Reset" type="reset" class="btn" value="重填" />
                            <%--<asp:Button ID="Config_btn" runat="server" CausesValidation="True" CssClass="btn"
                Text="下載設定檔" OnClick="Config_btn_Click" />--%>
                            <%--<% if (_viewModel.CompanyID.HasValue)
                                { %>
                            <input type="button" class="btn" name="btnDownloadConfig" value="下載設定檔" />
                            <% } %>--%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</form>
<script>
    $(function () {
        $global.commitIssuerAgent = function () {
            var event = event || window.event;
            $form = $(event.target).closest('form');
            showLoading();
            $.post('<%= Url.Action("CommitIssuerAgent","Organization",new { _viewModel.KeyID }) %>', $form.serializeObject(), function (data) {
                hideLoading();
                if ($.isPlainObject(data)) {
                    if (data.result) {
                        if (confirm('資料已儲存，是否關閉？')) {
                            $global.removeTab('applyIssuerAgent');
                        }
                    } else {
                        alert(data.message);
                    }
                } else {
                    $(data).appendTo($('body'));
                }
            });
        };
    });
</script>
<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    OrganizationViewModel _viewModel;
    Organization _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (OrganizationViewModel)ViewBag.ViewModel;
        _model = (Organization)this.Model;
    }

</script>
