﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (_message != null)
    { %>
<script>
    alert('<%= HttpUtility.JavaScriptStringEncode(_message) %>');
    <%  if (ViewBag.CloseWindow == true)
        { %>
    window.close();
    <%  } %>
</script>
<%  } %>
<script runat="server">
    String _message;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _message = (this.Model as String) ?? (String)ViewBag.Message ?? Request["Message"];
    }

</script>
