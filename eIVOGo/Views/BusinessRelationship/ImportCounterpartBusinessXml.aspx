﻿<%@ Page Title="" Language="C#" MasterPageFile="~/template/MvcMainPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
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

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <%  Html.RenderPartial("~/Views/BusinessRelationship/Module/ImportCounterpartBusinessXml.ascx"); %>
</asp:Content>
<script runat="server">

    ModelSource<InvoiceItem> models;
    ModelStateDictionary _modelState;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        ViewBag.ActionName = "首頁 > 匯入商務關係相對營業人";
    }

</script>