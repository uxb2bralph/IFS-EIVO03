<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignXmlPage.aspx.cs" Inherits="eIVOGo.Published.SignXmlPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div>
    
        Find Certificate by Subject :
        <asp:TextBox ID="Subject" runat="server" Columns="64"></asp:TextBox>
        <br />
        Certificate Store Name :
        <asp:DropDownList ID="CertStoreName" runat="server">
        </asp:DropDownList>
        <br />
        Certificate Store Location :
        <asp:DropDownList ID="CertStoreLocation" runat="server">
        </asp:DropDownList>
        <br />
        Certificate List :
        <select name="CertName">
            <%  if (_certs != null && _certs.Count() > 0)
                {
                    foreach (var item in _certs)
                    {%>
                        <option><%= item.Subject %></option>
            <%      }
                } %>
        </select>
        <asp:Button ID="btnListCert" runat="server" Text="List Certificates" OnClick="btnListCert_Click" />
        <br />

        Certificate Store Password :
        <asp:TextBox ID="StorePass" runat="server" Columns="32" EnableViewState="False" 
            TextMode="Password"></asp:TextBox>
        <br />
        CSP Name :
        <asp:TextBox ID="CspName" runat="server" Columns="32"></asp:TextBox>
        <br />
        Xml Context Upload :         <asp:FileUpload ID="XmlFile" runat="server" />
&nbsp;
        <asp:Button ID="btnUpload" runat="server" onclick="btnUpload_Click" 
            Text="Upload" />
        <br />
        Xml Context Upload :         <asp:FileUpload ID="XmlSigFile" runat="server" />
&nbsp;
        <asp:Button ID="btnVerify" runat="server" onclick="btnVerify_Click" 
            Text="Verify" />
        <br />
        <asp:Label ID="lblMsg" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
    
        <asp:LinkButton ID="lbViewCert" runat="server" onclick="lbViewCert_Click" 
            Visible="False">詳細憑證內容</asp:LinkButton>
    
    </div>
    </form>
</body>
</html>
