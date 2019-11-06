<%@ Control Language="c#" Inherits="Uxnet.Web.Module.Common.ReturnButton" CodeBehind="ReturnButton.ascx.cs" %>
<asp:Button ID="btnGoBack" runat="server" Text="¦^¤W­¶" EnableViewState="false" />
<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        if (UseReferrer)
        {
            if (!String.IsNullOrEmpty(GoBackUrl))
            {
                btnGoBack.OnClientClick = String.Format("window.location.href = '{0}';return false;", VirtualPathUtility.ToAbsolute(GoBackUrl));
            }
            else if (null != Request.UrlReferrer)
            {
                btnGoBack.OnClientClick = String.Format("window.location.href = '{0}';return false;", Request.UrlReferrer.AbsolutePath);
            }
            else
            {
                btnGoBack.OnClientClick = "window.history.go(-1);";
            }
        }
        else
        {
            btnGoBack.OnClientClick = "window.history.go(-1);";
        }        
    }
</script>
