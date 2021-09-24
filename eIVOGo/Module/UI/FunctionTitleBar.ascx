<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<h1><img id="img2" runat="server" enableviewstate="false" src="~/images/icon_search.gif" width="29" height="28" border="0" align="absmiddle" /><asp:Literal
        ID="litItemName" EnableViewState="false" runat="server"></asp:Literal></h1>
<script runat="server">
        [System.ComponentModel.Bindable(true)]
        public String ItemName
        {
            get
            {
                return litItemName.Text;
            }
            set
            {
                litItemName.Text = value;
            }
        }
</script>