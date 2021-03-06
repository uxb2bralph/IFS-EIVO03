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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "POS本組數配置"); %>
<form id="editItem">
    <div class="panel panel-default">
        <div class="panel-heading">
            本組數配置
        </div>
        <div class="panel-body">
            <div class="table-responsive">
                <!--表格 開始-->
                <table class="table table-striped table-bordered table-hover">
                    <tr>
                        <th>(<%= _model.InvoiceTrackCodeAssignment.Organization.ReceiptNo %>)<%= _model.InvoiceTrackCodeAssignment.Organization.CompanyName %>
                        </th>
                        <td>
                            <%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year %>年<%= String.Format("{0:00}-{1:00}月", _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo * 2 - 1, _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo * 2) %>
                        </td>
                        <th>
                            <%= _model.StartNo %> ~ <%= _model.EndNo %>
                        </th>
                        <td>共<%= (_model.EndNo-_model.StartNo+1) /50 %>本
                        </td>
                    </tr>
                    <tr>
                        <th colspan="1">POS機號
                        </th>
                        <td colspan="3">本組數
                        </td>
                    </tr>
                    <%  foreach (var item in _model.InvoiceTrackCodeAssignment.Organization.POSDevice)
                        { %>
                    <tr>
                        <th colspan="1">
                            <%= item.POSNo %>
                        </th>
                        <td colspan="3">
                            <input type="text" class="form-control" name="Booklets" maxlength="6" value='' />
                        </td>
                    </tr>
                    <%  } %>
                </table>
            </div>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="table-responsive">
                <!--表格 開始-->
                <table class="table table-striped table-bordered table-hover">
                    <tr>
                        <td class="Bargain_btn" align="center">
                            <button type="button" onclick="$global.commitOrganization();" name="UpdateButton" class="btn">確定</button>
                            &nbsp;
            <input name="Reset" type="reset" class="btn" value="重填" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</form>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    InvoiceNoInterval _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceNoInterval)this.Model;
    }

</script>
