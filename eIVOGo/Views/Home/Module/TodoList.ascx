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

<ul>
    <%  var count = models.GetTable<AuthorizeToVoid>().Count();
        if ( count> 0)
        {             %>
            <li>
                <a href="<%= Url.Action("AllowToVoid","InvoiceProcess") %>"><i class="fa fa-hand-o-right" aria-hidden="true"></i>營業人已送出<%=count %>筆註銷發票待核准</a>
            </li>
    <%  } %>
</ul>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
    }

</script>
