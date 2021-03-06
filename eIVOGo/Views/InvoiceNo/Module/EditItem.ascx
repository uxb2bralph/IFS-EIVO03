﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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

<tr>
    <td><%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year %></td>
    <td><%= String.Format("{0:00}-{1:00}月",_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2-1,_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2) %></td>
    <td><%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode %></td>
    <td>
        <input type="text" name="StartNo" data-role="edit" placeholder="請輸入配號區間起始號碼" value="<%= String.Format("{0:00000000}",_model.StartNo) %>" />
    </td>
    <td>
        <input type="text" name="EndNo" data-role="edit" placeholder="請輸入配號區間結尾號碼" value="<%= String.Format("{0:00000000}",_model.EndNo) %>" />
    </td>
    <td>
        <div class="btn-group dropdown" data-toggle="dropdown">
            <button class="btn bg-color-blueLight" data-toggle="dropdown" aria-expanded="false">請選擇功能</button>
            <button class="btn bg-color-blueLight dropdown-toggle" data-toggle="dropdown" aria-expanded="true"><span class="caret"></span></button>
            <ul class="dropdown-menu">
                <li><a class="btn" onclick="uiTrackCodeNo.commitItem('<%= _model.IntervalID.EncryptKey() %>');">確定</a></li>
                <li><a class="btn" onclick="uiTrackCodeNo.showItem('<%= _model.IntervalID.EncryptKey() %>');">取消</a></li>
            </ul>
        </div>
    </td>
</tr>


<script runat="server">

    InvoiceNoIntervalViewModel _viewModel;
    InvoiceNoInterval _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (InvoiceNoIntervalViewModel)ViewBag.ViewModel;
        _model = (InvoiceNoInterval)this.Model;
    }

</script>
