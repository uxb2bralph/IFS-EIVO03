﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<%  if(_model!=null)
    { %>
<div style="page-break-after: always;width:5.5cm;margin-left: 0cm;margin-right:0cm">
    
    <div class="container" style="border-top:0px;border-bottom: 0px;<%= _buyer.IsB2C() ? "page-break-after: always;" : null %>">
        <table >
            <tr>
                <td>
                    <div class="cutfield" style="/*width:5cm;*/border-top:0px;border:0px;/*font-weight:bold;*/">
                    <%  if (_model.Organization.LogoURL != null)
                        { %>
                            <img id="logo" style="max-width:5.2cm;max-height:1.7cm;"  src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_model.Organization.LogoURL) %>' />
                <%      }
                        else
                        { %>
                            <div style="text-align:left; width:4.4cm">
                                <h3 class="title-small" style="width:4.2cm; padding-top:0px;font-weight:bold; height: 1.3cm; <%= _model.Organization.CompanyName != null && _model.Organization.CompanyName.Length > 16 ? "font-size:small;" : null %>"><%=_model.Organization.CompanyName %></h3>
                            </div>
                    <%  }
                        if (_isDuplicatedPrint)
                        { %>
                        <h2 style="font-size:larger;">電子發票證明聯補印</h2>
                    <%  }
                        else
                        { %>
                        <h2>電子發票證明聯</h2>
                    <%  } %>
                        <h2><%= _model.InvoiceDate.Value.Year-1911 %>年<%= (_model.InvoiceDate.Value.Month % 2).Equals(0) ? String.Format("{0:00}-{1:00}", _model.InvoiceDate.Value.Month - 1, _model.InvoiceDate.Value.Month) : String.Format("{0:00}-{1:00}", _model.InvoiceDate.Value.Month, _model.InvoiceDate.Value.Month+1)%>月 </h2>
                        <h2><%= _model.TrackCode + "-" + _model.No %></h2>
                        <p>
                            <%= String.Format("{0:yyyy-MM-dd HH:mm:ss}", _model.InvoiceDate.Value)%> <%= _buyer.IsB2C()
                                                                                                            ? ""
                                                                                                            : _model.InvoiceBuyer.BuyerMark==3 || _model.InvoiceBuyer.BuyerMark==4
                                                                                                                ? "不得扣抵" 
                                                                                                                : String.Format("格式 {0}", ((int)_model.InvoiceType.Value).InvoiceTypeToFormat()) %><br />
                            隨機碼 <%= _model.RandomNo %>&nbsp;&nbsp;&nbsp;&nbsp; 總計 <%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TotalAmount) %><br />
                            賣方<%= _model.Organization.ReceiptNo%> <%= _buyer.IsB2C() ? null : String.Format("買方{0}", _buyer.ReceiptNo)%>
                        </p>
                        <div class="code1">
                            <%  String data = $"{_model.InvoiceDate.Value.Year - 1911:000}{_model.InvoiceDate.Value.Month:00}{_model.TrackCode}{_model.No}{_model.RandomNo}";  %>
                            <img id="barcode" alt="" style="max-width:4.7cm;" src="<%= data.GetBarcode39ImageSrc(margin:10,height:40,wide:3,narrow:1) %>" />
<%--                            <img id="barcode" alt="" width="160" height="22" src="<%= String.Format("{0}{1}?{2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/Published/GetBarCode39.ashx"), data) %>" />--%>
                        </div>
                        <div class="code2" style="padding-left:0.3cm;" >
                            <img style="margin-right: 0.3cm;width:2.0cm;" alt="" src="<%= _model.GetLeftQRCodeImageSrc(qrVersion:8) %>" />
                            <img style="margin-right: 0.3cm;width:2.0cm;" alt="" src="<%= "**".CreateQRCodeImageSrc(width:180,height:180,qrVersion:8) %>" />
                            <%--                            <img style="/*margin-left:0.3cm;*/margin-right:0.3cm;" alt="" width="80" height="80" src="<%= String.Format("{0}{1}?invoiceID={2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/DataView/GetLeftQRCode"), _model.InvoiceID) %>" />
                            <img style="/*margin-left:0.3cm;*/margin-right:0.3cm;" alt="" width="80" height="80" src="<%= String.Format("{0}{1}?invoiceID={2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/DataView/GetRightQRCode"), _model.InvoiceID) %>" />--%>
                        </div>
                    </div>                             
                </td>
           
            </tr> </table>
        </div>
          
        <div class="listfield" style="border-top:0px;border-bottom: 0px">
         <table style="width:4.8cm;font-size:8pt;font-weight:bold;">
             <tr>
                <td colspan="3">
                            <p style="display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">品名</p>
                </td>
             </tr>
            <tr>
                <td style="width:20%">
                    <p style="display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">數量</p>
                    </td>
                <td style="width:40%">
                    <p style="display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">單價</p>
                    </td>
                <td style="width:40%">
                    <p style="display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">小計</p>
                </td>
            </tr>
              <%--<tr>
                        <td colspan="4">
                       <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
                            <ItemTemplate>
                                <uc4:NewInvoicePOSProductPrintView ID="productView" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>--%>
        <% int _itemIdx;
           for (_itemIdx = 0; _productItem != null && _itemIdx < _productItem.Length; _itemIdx++)
           {
               var item = _productItem[_itemIdx];
            %>  
        <tr>
            <td colspan="3" height="15" valign="top"><%= item.InvoiceProduct.Brief%></td>
        </tr>
        <tr>
            <td align="right" valign="top"><%= String.Format("{0:##,###,###,##0.##}", item.Piece)%></td>
            <td align="right" valign="top"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></td>
            <td align="right" valign="top"><%= String.Format("{0:##,###,###,##0.##}", item.CostAmount)%></td>
        </tr>
        <% } %>
        <% if (_productItem != null )
           { %>
             <tr><td colspan="3" style="font-size: 8pt;">
       
            <p style="border-top: 1px dotted #808080;">
                <span style="font-size: 8pt;">總計：<%=_model.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_model.InvoiceAmountType.TaxType == (byte)2 || _model.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;<br />
                <% if (!_buyer.IsB2C())
                    {   %>
                應稅銷售額：<%=String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TaxType == 1 || _model.InvoiceAmountType.TaxType == 9 
                                                                        ? _model.InvoiceAmountType.SalesAmount 
                                                                        : _model.InvoiceAmountType.TaxType == 4
                                                                            ? _model.InvoiceAmountType.TotalAmount
                                                                            : 0) %>  <br/>
                零稅率銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)2 ? _model.InvoiceAmountType.SalesAmount : 0) %>  <br/>
                免稅銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)3 ? _model.InvoiceAmountType.SalesAmount : 0) %> <br/>
                稅額：<%= String.Format("{0:##,###,###,##0.##} ",_model.InvoiceBuyer.BuyerMark==3 || _model.InvoiceBuyer.BuyerMark==4 ? 0 : _model.InvoiceAmountType.TaxAmount) %><br />
                <%  } %>
                備註：<%= String.Join(";", _model.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)).Replace("\r\n","<br/>").Replace("\n","<br/>") %><br />
                退貨請憑電子發票證明聯辦理<br />
                <%  if (_model.InvoiceWinningNumber != null && _model.InvoiceCarrier != null && _model.InvoiceCarrier.CarrierType == "3J0002")
                    { %>
                        (貴用戶如已使用手機條碼設定為自動匯款，請勿重複領獎)
                <%  } %>
            </p></td>
        </tr>
        <% } %>

                              <%-- </td>
            </tr><tr>
                <td  colspan="4">
                        <div style="font-size: 8pt;">
                            <p style="border-top: 1px dotted #808080;">
                                <span style="font-size: 8pt; font-weight: bold;">總計：<%#_model.InvoiceDetails.Count %>項&nbsp;&nbsp;金額：<%# String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TotalAmount)%></span><br />
                                課稅別：<%# (_model.InvoiceAmountType.TaxType == (byte)2 || _model.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX" %>&nbsp;&nbsp;<%# !_buyer.IsB2C() ? String.Format("應稅銷售額：{0:##,###,###,##0.##}  零稅率銷售額：{1:##,###,###,##0.##}  免稅銷售額：{2:##,###,###,##0.##}", ((_model.InvoiceAmountType.TaxType != (byte)2 && _model.InvoiceAmountType.TaxType != (byte)3) ? _model.InvoiceAmountType.SalesAmount : 0), (_model.InvoiceAmountType.TaxType == (byte)2 ? _model.InvoiceAmountType.SalesAmount : 0), (_model.InvoiceAmountType.TaxType == (byte)3 ? _model.InvoiceAmountType.SalesAmount : 0)) : ""%>
            &nbsp;&nbsp;<%# !_buyer.IsB2C() ? String.Format("稅額：{0:##,###,###,##0.##} ",  _model.InvoiceAmountType.TaxAmount) : ""%>&nbsp;&nbsp;備註：<%# String.Join(";", _model.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)) %><br />
                                退貨請憑電子發票證明聯辦理
                            </p>
                        </div>
                    
                </td>
            </tr>--%>
        </table>
    </div>
</div>
    <% if(_viewModel?.PrintBuyerAddr==true) { %>
<div style="page-break-after: always; width: 5.2cm; margin-left: 0cm; margin-right: 0cm; padding-top:0px;">
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.PostCode %></span><br>
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.Address %></span><br><br>
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.ContactName %> 鈞啟</span><br>
    <%--<span style="font-size:1.2em;font-weight:bold;">(No:<%=_model.No.Substring(0,5)+"***" %>)</span>--%>
</div>
    <% } %>
<% } %>

<script runat="server">

    RenderStyleViewModel _viewModel;

    ModelSource<InvoiceItem> models;
    InvoiceItem _model;
    InvoiceProductItem[] _productItem;
    InvoiceBuyer _buyer;
    Organization _buyerOrg;
    bool _isDuplicatedPrint = false;
    const int _FirstCheckCount = 6;
    const int _SecondCheckCount = 16;
    const int _ItemPagingCount = 10;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceItem)this.Model;
        _viewModel = ViewBag.ViewModel as RenderStyleViewModel;

        _productItem = models.GetTable<InvoiceDetail>().Where(d => d.InvoiceID == _model.InvoiceID)
            .Join(models.GetTable<InvoiceProduct>(), d => d.ProductID, p => p.ProductID, (d, p) => p)
            .Join(models.GetTable<InvoiceProductItem>(), p => p.ProductID, i => i.ProductID, (p, i) => i).ToArray();

        _buyer = _model.InvoiceBuyer;
        if (_buyer != null && _buyer.BuyerID.HasValue)
            _buyerOrg = _buyer.Organization;

        _isDuplicatedPrint = /*_buyer.IsB2C() &&*/ _model.CDS_Document.DocumentPrintLog.Count > 0 && _model.Organization.OrganizationStatus.EntrustToPrint != true;

    }



</script>
