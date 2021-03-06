﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" StylesheetTheme="Login" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="~/Module/Common/CommonScriptInclude.ascx" TagPrefix="uc2" TagName="CommonScriptInclude" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>電子發票系統</title>
    <uc2:CommonScriptInclude runat="server" ID="CommonScriptInclude" />
<script type="text/javascript" language="javascript">
<!--
//顯示年份//
function show_date(){
	var time=new Date(); //宣告日期物件，儲存目前系統時間
	t_year=time.getFullYear(); //取得今年年分
	if(t_year > 2011){
		document.write(" - " + t_year);
	}
}
-->
</script>
</head>
<body>
    <div class="login">
        <form id="form1" runat="server">
            <input name="ReturnUrl" type="hidden" value="<%= Request["ReturnUrl"] %>" />
        <table border="0" cellspacing="0" cellpadding="0" class="sign_in">
            <tr>
                <td>
                        <span>帳號：</span><br />
                        <input name="PID" type="text" placeholder="請輸入帳號!!" required />
                </td>
                <td>
                        <span>密碼：</span><br />
                        <input name="Password" type="password" placeholder="請輸入密碼!!" required />
                    
                </td>
                <td>
                        <a onclick="javascript:login();">
                            <img onmouseout="this.src='<%= VirtualPathUtility.ToAbsolute("~/images/login_button_up.gif") %>'" onmouseover="this.src='<%= VirtualPathUtility.ToAbsolute("~/images/login_button_over.gif") %>'" src="<%= VirtualPathUtility.ToAbsolute("~/images/login_button_up.gif") %>" /></a>
                </td>
                <td valign="bottom">
                    <span class="forget_pw">[ <a href="<%= Url.Action("ForgetPassword","Account") %>">忘記密碼</a> ]</span>
                </td>
            </tr>
            </table>
            <table border="0" cellspacing="0" cellpadding="0" class="verifyno" >
             <tr>
                    <td>
                        <%  Html.RenderPartial("~/Views/Shared/CaptchaImg.ascx"); %>
                    </td>
            </tr>
                <tr>
                    <td><a href="<%= VirtualPathUtility.ToAbsolute("~/Published/InvoiceClient.zip") %>">電子發票傳輸用戶端下載</a><br />
                        <a href="<%= VirtualPathUtility.ToAbsolute("~/Published/InvoiceClient(XP-Win2003).zip") %>">電子發票傳輸用戶端下載(Windows XP、Windows 2003)</a>
                    </td>
                </tr>
            </table> 
            <table border="0" cellspacing="0" cellpadding="0" class="err01">
            <tr>
                <td>
                     <asp:Label ID="lblMsg" runat="server" ></asp:Label>
                </td>
            </tr>
        </table>
        </form>
    </div>
    <div class="run">
    <!--跑馬燈開始-->
        <marquee class="notice" scrollamount="2">
            <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
                <ItemTemplate>
                    ‧系統公告：<FONT color="#434343"><%# ((SystemMessage)Container.DataItem).MessageContents %></FONT>
                </ItemTemplate>
            </asp:Repeater>
        </marquee>
    <!--跑馬燈結束-->
    </div>
    <div class="copyright">
        Powered by 
        UXB2B
    </div>
</body>
</html>
<%  Html.RenderPartial("~/Views/Shared/ReportInputError.cshtml"); %>
<script>
    $(function () {
        $('input:text').addClass('form-control textfield');
    });
    function login() {
        var event = event || window.event;
        $('form').prop('action','<%= Url.Action("Login","Account") %>').submit();
    }
</script>
<cc1:SystemMessagesDataSource ID="dsEntity" runat="server">
</cc1:SystemMessagesDataSource>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += new EventHandler(login_aspx_PreRender);
    }

    void login_aspx_PreRender(object sender, EventArgs e)
    {
        var data = dsEntity.CreateDataManager().EntityList.Where(s => s.AlwaysShow || (s.StartDate <= DateTime.Now && s.EndDate.Value.AddDays(1) > DateTime.Now));
        rpList.DataSource = data;
        rpList.DataBind();
    }
</script>
