<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageUploader.ascx.cs"
    Inherits="Uxnet.Web.Module.Common.ImageUploader" %>
請選擇圖檔：<asp:FileUpload ID="imgFile" runat="server" CssClass="inputText" />
<br />
<asp:Image ID="imgInvoice" runat="server" Width="200px" Height="200px"></asp:Image>
<asp:Button ID="btnPreview" runat="server" Text="檢視" />
<div id="newSign" style="VISIBILITY: hidden;" onmouseout="this.style.visibility='hidden';" runat="server"></div>
