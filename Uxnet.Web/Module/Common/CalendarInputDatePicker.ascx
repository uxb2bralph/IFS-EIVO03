<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarInputDatePicker.ascx.cs"
    Inherits="Uxnet.Web.Module.Common.CalendarInputDatePicker" %>
<asp:TextBox ID="txtDate" CssClass="textfield" runat="server" Columns="10" ReadOnly="True" /><asp:Label
    ID="errorMsg" runat="server" Text="請選擇日期!!" Visible="false" EnableViewState="false"
    ForeColor="Red"></asp:Label>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.Load += new EventHandler(module_common_calendarinputdatepicker_ascx_Load);
    }

    void module_common_calendarinputdatepicker_ascx_Load(object sender, EventArgs e)
    {

        if (Page.Header.FindControl("uiCSS") == null)
        {
            HtmlLink uiCSS = new HtmlLink();
            uiCSS.ID = "uiCSS";
            uiCSS.Href = "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css";
            uiCSS.Attributes["rel"] = "stylesheet";
            uiCSS.Attributes["type"] = "text/css";
            Page.Header.Controls.Add(uiCSS);
        }
        
        Page.ClientScript.RegisterClientScriptInclude("jQuery", "http://ajax.googleapis.com/ajax/libs/jquery/1.5/jquery.min.js");
        Page.ClientScript.RegisterClientScriptInclude("jQueryUI","http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js");
        Page.ClientScript.RegisterClientScriptInclude("locale","http://jquery-ui.googlecode.com/svn/trunk/ui/i18n/jquery.ui.datepicker-zh-TW.js");

        StringBuilder sb = new StringBuilder();
        sb.Append(@"$(document).ready(function () {
          $.datepicker.setDefaults($.datepicker.regional['zh-tw']);");
        sb.Append(@"
          $(""#").Append(txtDate.ClientID).Append("\").datepicker(); });");

        Page.ClientScript.RegisterStartupScript(this.GetType(), txtDate.ClientID, sb.ToString(), true);
            
    }
</script>