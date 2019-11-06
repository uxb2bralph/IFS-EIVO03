<%@ Control Language="c#" Inherits="Uxnet.Web.Module.Common.MailButton" Codebehind="MailButton.ascx.cs" %>
<asp:button id="_btnDownload" runat="server" Text="傳送至電子信箱" CssClass="inputText" onclick="_btnDownload_Click"></asp:button>
<input type="hidden" id="mailButtonAddr" name="mailButtonAddr">
<script language="javascript">
<!--
	function sendMail()
	{
		var mailButtonAddr = document.all('mailButtonAddr');
		mailButtonAddr.value = prompt("請輸入收件人電子信箱,如有多位收件者,請用逗號','分開","");
		if(mailButtonAddr.value!='null' && mailButtonAddr.value.length>0)
			return true;
		
		return false;
	}
//-->
</script>
