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

    <% foreach (var item in _items.OrderBy(o => o.ReceiptNo))
        { %>
    <option value="<%= item.CompanyID %>"><%= String.Format("{0} {1}", item.ReceiptNo, item.CompanyName) %></option>
    <%  } %>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    IQueryable<Organization> _items;
    Model.Security.MembershipManagement.UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _profile = Business.Helper.WebPageUtility.UserProfile;

        _items = models.GetTable<Organization>()
            .Where(o => models.GetTable<EnterpriseGroupMember>().Any(g => g.CompanyID == o.CompanyID));

        if (_profile.IsSystemAdmin())
        {

        }
        else
        {
            _items = _items.Where(o => o.CompanyID == _profile.CurrentUserRole.OrganizationCategory.CompanyID);
        }
    }

</script>

