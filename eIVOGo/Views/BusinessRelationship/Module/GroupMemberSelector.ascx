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

<select name="<%= _model.Name %>" id="<%= _model.Id %>">
    <%  if (ViewBag.SelectAll == true)
        { %>
    <option value="">全部</option>
    <%  } %>
    <%  if (ViewBag.SelectIndication != null)
        {
            Writer.WriteLine(ViewBag.SelectIndication);
        } %>
    <% foreach (var item in _items)
        { %>
    <option value="<%= item.CompanyID %>"><%= String.Format("{0} {1}", item.ReceiptNo, item.CompanyName) %></option>
    <%  } %>
</select>
<%  if(_model.DefaultValue!=null)
    { %>
        <script>
            $(function () {
                $('select[name="<%= _model.Name%>"]').val('<%= _model.DefaultValue %>');
            });
        </script>
<%  } %>
<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    InputViewModel _model;
    IEnumerable<Organization> _items;
    Model.Security.MembershipManagement.UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (InputViewModel)this.Model;
        if(_model==null)
        {
            _model = new InputViewModel
            {
                Name = "CompanyID",
                Id = "CompanyID_" + DateTime.Now.Ticks
            };
        }
        var models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _profile = Business.Helper.WebPageUtility.UserProfile;

        if (_profile.IsSystemAdmin())
        {
            _items = models.GetTable<EnterpriseGroupMember>().Select(o => o.CompanyID)
                    .Distinct().Join(models.GetTable<Organization>(), d => d, o => o.CompanyID, (d, o) => o).OrderBy(o => o.ReceiptNo);

        }
        else
        {
            _items = models.GetTable<EnterpriseGroupMember>()
                    .Where(o => o.CompanyID == _profile.CurrentUserRole.OrganizationCategory.CompanyID)
                    .Select(o => o.CompanyID)
                    .Join(models.GetTable<Organization>(), d => d, o => o.CompanyID, (d, o) => o).OrderBy(o => o.ReceiptNo);
        }
    }

</script>

