﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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
<%@ Import Namespace="Model.Schema.EIVO.B2B" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%  if (_model.Count() > 0)
    {%>
<div>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th class="Head_style_a">發票
            </th>
        </tr>
    </table>
    <table class="table01 itemList">
        <thead>
            <tr>
                <th>發票號碼</th>
                <th>開立日期</th>
                <th>開立人統編</th>
                <th>時間</th>
                <th>錯誤訊息</th>
            </tr>
        </thead>
        <tbody>
            <%  foreach (var item in _model)
                {
                    InvoiceRootInvoice invoice = null;
                    SellerInvoiceRootInvoice b2bInvoice = null;%>
            <tr>
                <td>
                    <a href='<%= $"{eIVOGo.Properties.Settings.Default.WebApDomain}{VirtualPathUtility.ToAbsolute("~/Published/DumpExceptionLog.ashx")}?logID={item.LogID}" %>'
                    target="_blank">
                    <%= item.IsCSV == true 
                            ? item.DataContent 
                            : (invoice=getInvoiceContent(item.DataContent))?.InvoiceNumber ?? (b2bInvoice=getSellerInvoiceContent(item.DataContent))?.InvoiceNumber %></a>
                </td>
                <td>
                    <%= invoice?.DataDate ?? invoice?.InvoiceDate ?? b2bInvoice?.InvoiceDate %>
                </td>
                <td><%= invoice?.SellerId ?? b2bInvoice?.SellerId %></td>
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

    protected InvoiceRootInvoice getInvoiceContent(String data)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return doc.ConvertTo<InvoiceRootInvoice>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }

    protected SellerInvoiceRootInvoice getSellerInvoiceContent(String data)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return doc.ConvertTo<SellerInvoiceRootInvoice>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }


</script>
