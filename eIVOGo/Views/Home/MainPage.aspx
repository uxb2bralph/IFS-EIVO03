<%@ Page Title="" Language="C#" MasterPageFile="~/template/MvcMainPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
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
<%@ Import Namespace="Business.Helper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <%  if (_profile.IsSystemAdmin())
        {
            Html.RenderPartial("~/Views/Home/Module/TodoList.ascx");
        } %>
</asp:Content>
<script runat="server">

    Model.Security.MembershipManagement.UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ViewBag.ActionName = "首頁";
        _profile = Context.GetUser();
    }

</script>