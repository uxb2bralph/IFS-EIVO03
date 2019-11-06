<%@ Control Language="c#" Inherits="Uxnet.Web.Module.Common.SignXmlContext" CodeBehind="SignXmlContext.ascx.cs" %>
<object id="signable" classid="<%= String.Format("{0}#Uxnet.Distribute.Signable",VirtualPathUtility.ToAbsolute("~/Published/Uxnet.Distribute.dll")) %>">
</object>
<script type="text/javascript">
<!--

    function signContext(arg, context) {
        var signObj = CreateSignable(); //new ActiveXObject("Uxnet.Distribute.Signable"); //document.all('signable');
        if (signObj != null) {
            if (signObj.SignXml(arg)) {
                var form = document.forms[0];

                var element = document.getElementById('dataSignature');
                if (element == null) {
                    element = document.createElement('textArea');
                    element.name = "dataSignature";
                    element.style.display = "none";
                    element.style.visibility = "hidden";
                    form.appendChild(element);
                }
                element.value = signObj.SignedMessage;

                afterSigned();
            }
            else {
                alert(signObj.ErrorMessage);
            }
        }
    }

    function signContextSilently(arg, context) {
        var signObj = CreateSignable(); //new ActiveXObject("Uxnet.Distribute.Signable"); //document.all('signable');
        if (signObj != null) {
            if (signObj.SignXmlSilently(arg)) {
                var form = document.forms[0];

                var element = document.getElementById('dataSignature');
                if (element == null) {
                    element = document.createElement('textArea');
                    element.name = "dataSignature";
                    element.style.display = "none";
                    element.style.visibility = "hidden"; 
                    form.appendChild(element);
                }
                element.value = signObj.SignedMessage;

                afterSigned();
            }
            else {
                alert(signObj.ErrorMessage);
            }
        }
    }
    

    function signMessage(form, arg) {
        if (form != null) {
            var signObj = CreateSignable(); //new ActiveXObject("Uxnet.Distribute.Signable"); //document.all('signable');
            if (signObj != null) {
                if (signObj.SignXml(arg)) {

                    var element = document.getElementById('dataSignature');
                    if (element == null) {
                        element = document.createElement('textArea');
                        element.name = "dataSignature";
                        element.style.display = "none";
                        element.style.visibility = "hidden"; 
                        form.appendChild(element);
                    }
                    element.value = signObj.SignedMessage;

                    return true;
                }
                else {
                    alert(signObj.ErrorMessage);
                }
            }
        }
        return false;
    }
    

    function ProcessCallBackError(arg, context) {
        alert(arg);
    }

    function CreateSignable() {
        var tmpDOM;

        try {
            tmpDOM = document.all('signable'); // new ActiveXObject("Uxnet.Distribute.Signable");
        } catch (e) {
            tmpDOM = null;
        }


        if (tmpDOM == null) {
            if (confirm("您尚未安裝網際優勢用戶端電子簽章元件,是否立即下載安裝?")) {
                window.location.href = signableObj;
            }
        }
        return tmpDOM;
    }
    
    
	
//-->
</script>

