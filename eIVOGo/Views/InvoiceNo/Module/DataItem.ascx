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
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Business.Helper" %>

<tr>
    <td><%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year %></td>
    <td><%= String.Format("{0:00}-{1:00}月",_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2-1,_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo*2) %></td>
    <td><%= _model.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode %>(<%= $"{(Naming.InvoiceTypeDefinition?)_model.InvoiceTrackCodeAssignment.InvoiceTrackCode.InvoiceType}" %>)</td>
    <td><%= String.Format("{0:00000000}",_model.StartNo) %></td>
    <td><%= String.Format("{0:00000000}",_model.EndNo) %></td>
    <td><%
            var current = _model.InvoiceNoAssignments.OrderByDescending(a => a.InvoiceID).FirstOrDefault();
        %>
        <%= _model.EndNo-_model.StartNo+1 %>／<%= current!=null ?_model.EndNo-current.InvoiceNo : _model.EndNo-_model.StartNo+1 %></td>
    <td><%  
            if (current != null)
            {
                Writer.Write(String.Format("{0:00000000}", current.InvoiceNo));
            }
        %></td>
    <td>
        <div class="btn-group dropdown" data-toggle="dropdown">
            <button class="btn bg-color-blueLight" data-toggle="dropdown" aria-expanded="false">請選擇功能</button>
            <button class="btn bg-color-blueLight dropdown-toggle" data-toggle="dropdown" aria-expanded="true"><span class="caret"></span></button>
            <ul class="dropdown-menu">
        <%  if (_model.InvoiceNoAssignments.Count == 0)
            { %>

                <li><a class="btn" onclick="uiTrackCodeNo.editItem('<%= _model.IntervalID.EncryptKey() %>');">修改</a></li>
                <li><a class="btn" onclick="uiTrackCodeNo.deleteItem('<%= _model.IntervalID.EncryptKey() %>');">刪除</a></li>
                <%  if (_model.InvoiceTrackCodeAssignment.Organization.POSDevice.Count > 0)
                    { %>
                <li><a class="btn" onclick="uiTrackCodeNo.assignPOSBooklets('<%= _model.IntervalID.EncryptKey() %>');">POS本組數配置</a></li>
                <%  } %>
                <li><a class="btn" onclick="uiTrackCodeNo.allotInterval('<%= _model.IntervalID.EncryptKey() %>');">本組數均分</a></li>
        <%  }
            else
            {
                var remained = _model.EndNo - current.InvoiceNo;
                if (remained >= 150)
                {   %>
                <li><a class="btn" onclick="uiTrackCodeNo.splitItem('<%= _model.IntervalID.EncryptKey() %>');">分割</a></li>
            <%  }
            }%>
            </ul>
        </div>
</td>
</tr>


<script runat="server">

    InvoiceNoInterval _model;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (InvoiceNoInterval)this.Model;
    }

</script>
