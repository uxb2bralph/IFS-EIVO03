<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" StylesheetTheme="Login" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="~/Module/Common/CommonScriptInclude.ascx" TagPrefix="uc2" TagName="CommonScriptInclude" %>


<!DOCTYPE html >
<html>
<head runat="server">
    <title>電子發票系統</title>
    <uc2:CommonScriptInclude runat="server" ID="CommonScriptInclude" />
    <script type="text/javascript" language="javascript">
<!--
    //顯示年份//
    function show_date() {
        var time = new Date(); //宣告日期物件，儲存目前系統時間
        t_year = time.getFullYear(); //取得今年年分
        if (t_year > 2011) {
            document.write(" - " + t_year);
        }
    }
    -->
    </script>
</head>
<body>
    <div class="get_pw">
        <form class="getmail" id="form1" runat="server">
            <table border="0" cellspacing="0" cellpadding="0" class="sign_in">
                <tr>
                    <td>
                        <span>帳號：</span><br />
                        <input name="PID" type="text" placeholder="請輸入帳號!!" required />
                    </td>
                    <td>
                        <a onclick="javascript:login();">
                            <img onmouseout="this.src='<%= VirtualPathUtility.ToAbsolute("~/images/getpw_button_up.gif") %>'" onmouseover="this.src='<%= VirtualPathUtility.ToAbsolute("~/images/getpw_button_over.gif") %>'" src="<%= VirtualPathUtility.ToAbsolute("~/images/getpw_button_up.gif") %>" /></a>
                    </td>
                </tr>
            </table>
            <table border="0" cellspacing="0" cellpadding="0" class="verifyno">
                <tr>
                    <td>
                        <%  Html.RenderPartial("~/Views/Shared/CaptchaImg.ascx"); %>
                    </td>
                </tr>
            </table>
            <table border="0" cellspacing="0" cellpadding="0" class="err01">
                <tr>
                    <td>
                        <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div class="copyright" style="position: absolute; top: 290px; left: 50%;">
        &copy; 2011
        <script type="text/javascript" language="javascript">            show_date();</script>
        UXB2B. All rights reserved. [ <a href="<%= Url.Action("Login","Account") %>">回登入頁</a> ]
    </div>
</body>
</html>
<%  Html.RenderPartial("~/Views/Shared/ReportInputError.ascx"); %>
<%  Html.RenderPartial("~/Views/Shared/JsAlert.ascx"); %>
<script>
    $(function () {
        $('input:text').addClass('form-control textfield');
    });
    function login() {
        var event = event || window.event;
        $('form').prop('action','<%= Url.Action("CommitToResetPass","Account") %>').submit();
    }
</script>
