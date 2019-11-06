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
<%@ Import Namespace="Model.Security.MembershipManagement" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Business.Helper" %>

<input type="text" size="40" id="<%= _contentID %>" class="form-control" value="" placeholder="請輸入檢索關鍵字" />
<input type="hidden" name="<%= _fieldName %>" />
<script>
    $(function () {
        debugger;
        var $input = $('#<%= _contentID %>');
        $input.on('change', function (evt) {
            if ($input.val() == '') {
                $('input:hidden[name="<%= _fieldName %>"]').val('');
                $('input[name="BuyerReceiptNo"]').val('');
            } else {
                $('input[name="BuyerReceiptNo"]').val($input.val().split(' ')[0]);
            }
        });
        $input.autocomplete({
            source: '<%= Url.Action("SearchCounterpart", "Home") %>',
            select: function (event, ui) {
                $input.val(ui.item.label);
                $('input:hidden[name="<%= _fieldName %>"]').val(ui.item.value);
                event.preventDefault();
                if ($global.onBusiness) {
                    $global.onBusiness(ui.item.value);
                }
            },
            close: function (event, ui) {
                if ($input.val() == '') {
                    $('input:hidden[name="<%= _fieldName %>"]').val('');
                }
            },
        });
    });

    <%  if (Request[_fieldName] != null)
        { %>
        $(function () {
            $('input:hidden[name="<%= _fieldName %>"]').val('<%: Request[_fieldName] %>');
                });
    <%  } %>
</script>


<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    IEnumerable<Organization> _model;
    String _fieldName;
    String _contentID;
    UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IEnumerable<Organization>)this.Model;
        _fieldName = (String)ViewBag.FieldName ?? "BuyerID";
        _contentID = (String)ViewBag.FieldID ?? ("id" + DateTime.Now.Ticks);
        _profile = Context.GetUser();
    }

</script>
