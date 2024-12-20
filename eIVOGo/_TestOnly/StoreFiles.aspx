﻿<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.IO" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="theForm" runat="server">
        <asp:Repeater ID="rpItems" runat="server" EnableViewState="false">
            <ItemTemplate>
                <input type="hidden" name='<%# Eval("Key") %>' value='<%# Eval("Value") %>' />
            </ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
<script runat="server">

    List<KeyValuePair<String, String>> _items;
    String _returnTo = null;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += _testonly_storefiles_aspx_PreRender;
        
        _items = new List<KeyValuePair<string, string>>();
        foreach (var key in Request.Form.AllKeys)
        {
            foreach (var v in Request.Form.GetValues(key))
            {
                _items.Add(new KeyValuePair<string, string>(key, v));
            }
        }

        if (Request.Files.Count > 0)
        {
            String storePath = Path.Combine(Request.PhysicalApplicationPath, DateTime.Today.ToString("yyyy"), DateTime.Today.ToString("MM"), DateTime.Today.ToString("dd"));
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            for (int idx = 0; idx < Request.Files.Count;idx++ )
            {
                var file = Request.Files[idx];
                String fullName = Path.Combine(storePath, file.FileName);
                file.SaveAs(fullName);
                _items.Add(new KeyValuePair<string, string>("file" + idx.ToString(), fullName));
            }
        }
        
        if(Request["returnTo"]!=null)
        {
            _returnTo = Request["returnTo"];
            if (!String.IsNullOrEmpty(Request.Params["QUERY_STRING"]))
            {
                _returnTo = _returnTo + "?" + Request.Params["QUERY_STRING"];
            }
        }

        if (_returnTo != null)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "document.forms[0].submit();", true);
        }
        
    }

    void _testonly_storefiles_aspx_PreRender(object sender, EventArgs e)
    {
        Page.Form.Action = _returnTo;
        rpItems.DataSource = _items;
        rpItems.DataBind();
    }
    
</script>
