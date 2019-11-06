<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Schema.EIVO" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%  if (_model.Count() > 0)
    {%>
<div>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th class="Head_style_a">作廢發票
            </th>
        </tr>
    </table>
    <table class="table01 itemList">
        <thead>
            <tr>
                <th>作廢發票號碼</th>
                <th>作廢日期</th>
                <th>開立人統編</th>
                <th>時間</th>
                <th>錯誤訊息</th>
            </tr>
        </thead>
        <tbody>
            <%  foreach (var item in _model)
                {
                    CancelInvoiceRootCancelInvoice dataItem = null;%>
            <tr>
                <td>
                    <a href='<%= $"{eIVOGo.Properties.Settings.Default.WebApDomain}{VirtualPathUtility.ToAbsolute("~/Published/DumpExceptionLog.ashx")}?logID={item.LogID}" %>'
                    target="_blank">
                    <%= item.IsCSV == true ? item.DataContent : (dataItem=getInvoiceCancellationContent(item.DataContent))?.CancelInvoiceNumber%></a>
                </td>
                <td>
                    <%= dataItem?.CancelDate %>
                </td>
                <td><%= dataItem?.SellerId %></td>
                <td><%= $"{item.LogTime:yyyy/MM/dd HH:mm:ss}" %></td>
                <td><%= item.Message %></td>
            </tr>
            <%  } %>
        </tbody>
    </table>
</div>
<%  } %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<ExceptionLog> _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<ExceptionLog>)this.Model;

    }

    protected CancelInvoiceRootCancelInvoice getInvoiceCancellationContent(String data)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return doc.ConvertTo<CancelInvoiceRootCancelInvoice>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }


</script>
