<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SiteMenuBar.ascx.cs"
    Inherits="Uxnet.Web.Module.SiteAction.SiteMenuBar" %>
<asp:Menu ID="mainMenu" runat="server" Orientation="Horizontal" StaticDisplayLevels="2"
    DataSourceID="dsHandler" Font-Size="XX-Small" DynamicHorizontalOffset="2" StaticSubMenuIndent="10px"
    EnableViewState="False">
    <DataBindings>
        <asp:MenuItemBinding DataMember="menuItem" NavigateUrlField="url" TextField="value" />
    </DataBindings>
    <StaticHoverStyle BackColor="#F4F6FC" ForeColor="#6C5E94" />
    <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" ForeColor="#990000" />
    <DynamicHoverStyle BackColor="#FFFFCC" ForeColor="#CC6600" />
    <DynamicMenuStyle BackColor="#CCCCCC" />
    <StaticSelectedStyle BackColor="#F4F6FC" />
    <DynamicSelectedStyle BackColor="#F4F6FC" />
    <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" ForeColor="#003399" />
</asp:Menu>
<asp:XmlDataSource ID="dsHandler" runat="server" EnableViewState="False" TransformFile="~/resource/MenuTransformer.xsl"
    DataFile="~/resource/DeveloperMenu.xml" EnableCaching="False"></asp:XmlDataSource>
