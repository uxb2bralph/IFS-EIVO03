﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
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

<table class="table01 businessList">
    <thead>
        <tr>
            <th style="min-width: 200px">集團成員</th>
            <th style="min-width: 200px">相對營業人名稱</th>
            <th style="min-width: 80px">統一編號</th>
            <th style="min-width: 50px">類別</th>
            <th style="min-width: 120px">聯絡人電子郵件</th>
            <th style="min-width: 200px">地址</th>
            <th style="min-width: 120px">電話</th>
            <th style="min-width: 120px">店號</th>
            <th style="min-width: 60px">狀態</th>
            <th style="min-width: 150px">管理</th>
        </tr>
    </thead>
    <tbody>
        <%  int idx = 0;
                foreach (var item in _items)
                {
                    idx++;
                    Html.RenderPartial("~/Views/BusinessRelationship/Module/DataItem.ascx", item);
                }
                Html.RenderPartial("~/Views/BusinessRelationship/Module/AddItem.ascx");
        %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="10">&nbsp;</td>
        </tr>
    </tfoot>
</table>
<%--<script>
    $(function () {
        $('.businessList').DataTable({
            "scrollX": true,
            "paging": false,
            "ordering": false,
            "info": false,
            "searching": false,
            "responsive": true
        });
    });
</script>--%>



<script runat="server">

    IQueryable<BusinessRelationship> _items;
    BusinessRelationshipQueryViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (BusinessRelationshipQueryViewModel)ViewBag.ViewModel;
        _items = ((IQueryable<BusinessRelationship>)this.Model).Skip(_viewModel.PageIndex.Value * _viewModel.PageSize.Value)
            .Take(_viewModel.PageSize.Value);
    }

</script>
