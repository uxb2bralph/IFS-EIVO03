﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
  <tr>
    <td width="30"><img id="img1" runat="server" enableviewstate="false" src="~/images/path_left.gif" alt="" width="30" height="29" /></td>
    <td bgcolor="#ecedd5">
        <asp:Literal ID="litItemName" runat="server" EnableViewState="false"></asp:Literal></td>
    <td width="18"><img id="img3" runat="server" enableviewstate="false" src="~/images/path_right.gif" alt="" width="18" height="29" /></td>
  </tr>
</table>
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