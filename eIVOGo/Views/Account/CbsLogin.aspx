<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" StylesheetTheme="Login" %>

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
<link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" rel="stylesheet" >
<script src="//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
<script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
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
    <div class="container">
    <div class="row">
		<div class="col-md-4 col-md-offset-4">
    		<div class="panel-default">
			  	<div class="panel-body">
                    
			    	<form id="form1" runat="server" accept-charset="UTF-8" role="form">
                    <fieldset>
                        <img class="mx-auto d-block logosize" src="../images/e-GUIplatform.png" alt="">                                         
			    	  	<div class="form-group prompt">
                            <label for="user_login">ID</label>
			    		    <input class="form-control" name="PID" type="text" required autofocus>
			    		</div>
			    		<div class="form-group prompt">
                            <label>Password</label>
                            <a class ="form-sublink" href="<%= Url.Action("ForgetPassword","Account") %>">Forgot password</a>
			    			<input class="form-control" name="Password" type="password" value="" required>
			    		</div>
						<button id="btn" type="button" class="btn btn-inverted btn-block mb-4" >Sign In</button> 	
						<div class="form-group">
							  
						</div>
			    	</fieldset>
			      	</form>
					
			    </div>
			</div>
		</div>
	</div>
</div>
</body>
</html>
<%  Html.RenderPartial("~/Views/Shared/ReportInputError.cshtml"); %>
<script>
    $(function () {
        $('input:text').addClass('form-control textfield');
    });
    $('#btn').click(function() {
        var event = event || window.event;
        $('form').prop('action','<%= Url.Action("CbsLogin","Account") %>').submit(); 
        });    
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
    }
</script>
