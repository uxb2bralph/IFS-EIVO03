<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Register src="DropdownMenu.ascx" tagname="DropdownMenu" tagprefix="uc1" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Utility" %>

<uc1:DropdownMenu ID="mainMenu" runat="server" MenuBrand="功能選單" StaticLevel="1" />
<script runat="server">

    Model.Security.MembershipManagement.UserProfileMember _userProfile; 

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += module_ui_sidebarmenu_ascx_PreRender;

        _userProfile = Business.Helper.WebPageUtility.UserProfile;
        
    }

    void module_ui_sidebarmenu_ascx_PreRender(object sender, EventArgs e)
    {
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.Load(Path.Combine(Server.MapPath("~/resource"),_userProfile.CurrentSiteMenu));
        var menuData = (Uxnet.Web.Module.SiteAction.SiteMenuItem)doc.ConvertTo<Uxnet.Web.Module.SiteAction.SiteMenuItem>();
        mainMenu.BindData(menuData);
    }
</script>

