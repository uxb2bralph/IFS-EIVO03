<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.DataEntity" %>

<tr>
    <td><%= _model.Year %></td>
    <td><%= String.Format("{0:00}-{1:00}月",_model.PeriodNo*2-1,_model.PeriodNo*2) %></td>
    <td><%= _model.TrackCode %></td>
    <td><%= String.Format("{0:00000000}",_model.InvoiceNo) %></td>
    <td><%= String.Format("{0:00000000}",_tailItem.InvoiceNo) %></td>
    <td><%= _tailItem.InvoiceNo - _model.InvoiceNo + 1 %></td>
</tr>


<script runat="server">

    InquireVacantNoResult _model;
    InquireVacantNoResult _tailItem;
    List<InquireVacantNoResult> _items;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (InquireVacantNoResult)this.Model;
        _items = (List<InquireVacantNoResult>)ViewBag.DataItems;
        if (_model.CheckNext.HasValue)
            checkSummary();
        else
            _tailItem = _model;
    }

    long checkSummary()
    {
        var index = _items.IndexOf(_model);
        _tailItem = _items[index + 1];
        return _tailItem.InvoiceNo - _model.InvoiceNo + 1;
    }

</script>
