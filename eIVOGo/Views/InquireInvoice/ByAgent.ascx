<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Security.MembershipManagement" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%  if(_profile.CurrentUserRole.OrganizationCategory.CategoryID==(int)Naming.CategoryID.COMP_SYS)
    { %>
<tr>
    <th>代理業者統編
    </th>
    <td class="tdleft">
        <select name="agentID">
            <option value="">全部</option>
            <%  foreach(var a in _model)
                {   %>
            <option value="<%= a.CompanyID %>"><%= $"({a.ReceiptNo}) {a.CompanyName}" %></option>
            <%  }                %>
        </select>
        <% if(Request["agentID"]!=null) { %>
        <script>
            $(function () {
                $('select[name="agentID"]').val('<%= Request["agentID"] %>');
            });
        </script>
        <% } %>
    </td>
</tr>
<%  } %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<Organization> _model;
    UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<Organization>)this.Model;
        _profile = Context.GetUser();
    }

</script>


