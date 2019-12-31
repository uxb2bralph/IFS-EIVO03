<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="System.Web.Optimization" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title>電子發票系統</title>
    <%  Html.RenderPartial("~/Views/Shared/CommonScriptInclude.cshtml"); %>
</head>
<body>
</body>
</html>
<% Html.RenderPartial("~/Views/Shared/GlobalScript.cshtml"); %>
<% Html.RenderPartial("~/Views/Shared/AlertMessage.cshtml"); %>
<script runat="server">

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

</script>
