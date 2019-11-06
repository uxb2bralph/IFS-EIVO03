<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

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

<% if(_model!=null) { %>
<div style="text-align:center; position:absolute;margin-left:0.3cm;height:7cm;margin-top:2cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
<div style="text-align:center; position:absolute;margin-left:19.4cm;height:7cm;margin-top:2cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
<div style="page-break-after: always;margin-left:0.5cm;margin-right:0.5cm;margin-bottom:0cm;margin-top:0.5cm;">
    <div class="fspace">
        <div class="company" style="padding-top: 0.5cm;padding-left: 1.0cm;">
            <span class="title"><%= _model.InvoiceSeller.CustomerName  %></span><br />
            <p>
                <%= _model.InvoiceSeller.Address  %><br />
                <%= _model.Organization.OrganizationStatus.SetToOutsourcingCS == true ? "委外客服電話：0800-010-626" : ""%>
            </p>
        </div>
        <div class="customer">
            <p style="margin-left:-30px;margin-top:-30px;width:12cm;">
                <%= _buyer.PostCode %><br />
                <%= _buyer.Address %><br />
                <%=_model.InvoiceSeller.ReceiptNo == "27934855" && _buyer.CustomerName != _buyer.ContactName
                        ? _buyer.CustomerName + "  " + _buyer.ContactName 
                        : _buyer.ContactName    %><%--<%= _buyer.ContactName %> --%>
            鈞啟
            </p>
        </div>
    </div>
    <div class="container"  style="border-bottom-style:none;border-top-style:none;margin-left:0.2cm" >
        <div style="width: 7.2cm; height: 9cm; display: block; overflow: hidden; float: left;"></div>
        <div class="cutfield" style="margin-left: -0.6cm;margin-top: -0.6cm;">
            <%  if (_model.Organization.LogoURL != null)
                { %>
                    <img id="logo" style="width: 200px; height: auto;" src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_model.Organization.LogoURL) %>' />
            <%  }
                else
                { %>
                    <h3 style="width:4.8cm;"><%= _model.InvoiceSeller.CustomerName  %></h3>
            <%  }
                if (_isDuplicatedPrint)
                { %>
            <h2 style="font-size: larger;">電子發票證明聯補印</h2>
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
                隨機碼 <%= _model.RandomNo %>&nbsp;&nbsp;&nbsp;&nbsp;總計 <%= String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TotalAmount) %><br />
                賣方<%= _model.Organization.ReceiptNo%> <%= _buyer.IsB2C() ? null : String.Format("買方{0}", _buyer.ReceiptNo)%>
            </p>
            <div class="code1">
                <%  String data = $"{_model.InvoiceDate.Value.Year - 1911:000}{_model.InvoiceDate.Value.Month:00}{_model.TrackCode}{_model.No}{_model.RandomNo}";  %>
                <img id="barcode" alt="" width="160" height="22" src="<%= data.GetBarcode39ImageSrc() %>" />
<%--                <img id="barcode" alt="" width="160" height="22" src="<%= String.Format("{0}{1}?{2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/Published/GetBarCode39.ashx"), data) %>" />--%>
            </div>
            <div class="code2">
                <img id="qrcode1" alt="" width="80" height="80" src="<%= _model.GetLeftQRCodeImageSrc() %>" />
                <img id="qrcode2" alt="" width="80" height="80" src="<%= "**".CreateQRCodeImageSrc(width:180,height:180,qrVersion:10) %>" />
<%--                <img id="qrcode1" alt="" width="80" height="80" src="<%= String.Format("{0}{1}?invoiceID={2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/DataView/GetLeftQRCode"), _model.InvoiceID) %>" />
                <img id="qrcode2" alt="" width="80" height="80" src="<%= String.Format("{0}{1}?invoiceID={2}", Uxnet.Web.Properties.Settings.Default.HostUrl, VirtualPathUtility.ToAbsolute("~/DataView/GetRightQRCode"), _model.InvoiceID) %>" />--%>
            </div>
        </div>
    </div>
    <div class="listfield" style="padding-left: 0.5cm;">
        <div class="content_box">
            <p class="productname">品名</p>
            <p class="quantity">數量</p>
            <p class="price">單價</p>
            <p class="totalPrice">小計</p>
        </div>
        <% int _itemIdx;
           for (_itemIdx = 0; _productItem!=null &&  _itemIdx < Math.Min(_productItem.Length, _ItemPagingCount);_itemIdx++)
           {
               var item = _productItem[_itemIdx];
            %>
        <div class="content_box">
            <p class="productname"><%= item.InvoiceProduct.Brief%></p>
            <p class="quantity"><%= String.Format("{0:##,###,###,##0.##}", item.Piece)%></p>
            <p class="price"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></p>
            <p class="totalPrice"><%= String.Format("{0:##,###,###,##0.##}", item.CostAmount)%></p>
        </div>
        <% } %>
        <% if (_productItem != null && _productItem.Length <= _FirstCheckCount)
           { %>
        <div class="content_box">
            <p style="border-top: 1px dotted #808080;margin-right:1.6cm;">
                <span style="font-size: 12pt; font-weight: bold;">
                    總計：<%=_model.InvoiceDetails.Count%>項&nbsp;&nbsp;
                    金額：<%= String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TotalAmount)%></span><br />
                    課稅別：<%= (_model.InvoiceAmountType.TaxType == (byte)2 || _model.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;
                    <%  if (!_buyer.IsB2C())
                        {   %>
                    應稅銷售額：<%=String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TaxType == 1 || _model.InvoiceAmountType.TaxType == 9 
                                                                        ? _model.InvoiceAmountType.SalesAmount 
                                                                        : _model.InvoiceAmountType.TaxType == 4
                                                                            ? _model.InvoiceAmountType.TotalAmount
                                                                            : 0) %>  &nbsp;&nbsp;
                    零稅率銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)2 ? _model.InvoiceAmountType.SalesAmount : 0) %>  &nbsp;&nbsp;
                    免稅銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)3 ? _model.InvoiceAmountType.SalesAmount : 0) %> &nbsp;&nbsp;
                    稅額：<%= String.Format("{0:##,###,###,##0.##} ",_model.InvoiceBuyer.BuyerMark==3 || _model.InvoiceBuyer.BuyerMark==4 ? 0 : _model.InvoiceAmountType.TaxAmount) %>&nbsp;&nbsp;
                    <%  } %>
                    備註：<%= String.Join(";", _model.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)).Replace("\r\n","<br/>").Replace("\n","<br/>") %><br />
                    退貨請憑電子發票證明聯辦理
                <%  if (_model.InvoiceWinningNumber != null && _model.InvoiceCarrier != null && _model.InvoiceCarrier.CarrierType == "3J0002")
                    { %>
                        (貴用戶如已使用手機條碼設定為自動匯款，請勿重複領獎)
                <%  } %>
            </p>
        </div>
        <% } %>
    </div>
</div>
<%--<p runat="server" style="page-break-after: always" visible="<%= printBack ? true : (!IsFinal ? true : false) %>"></p>--%>
    <% if(_viewModel?.PrintBack==true || (_productItem!=null && _productItem.Length>_FirstCheckCount))  { %>
<div class="br" id="backPage" style="page-break-after: always;margin-left:0.5cm;margin-right:0.5cm;border-bottom-style:none;">
    <div class="bspace listfield" style="padding-left: 0.5cm;">
        <% for (; _itemIdx < Math.Min(_productItem.Length, _ItemPagingCount*2);_itemIdx++)
           {
               var item = _productItem[_itemIdx];
        %>
        <div class="content_box">
            <p class="productname"><%= item.InvoiceProduct.Brief%></p>
            <p class="quantity"><%= String.Format("{0:##,###,###,##0.##}", item.Piece)%></p>
            <p class="price"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></p>
            <p class="totalPrice"><%= String.Format("{0:##,###,###,##0.##}", item.CostAmount)%></p>
        </div>
        <% } %>
        <% if (_productItem.Length>_FirstCheckCount && _productItem.Length <= _SecondCheckCount)
           { %>
        <div class="content_box">
            <p style="border-top: 1px dotted #808080;">
                <span style="font-size: 12pt; font-weight: bold;">總計：<%=_model.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_model.InvoiceAmountType.TaxType == (byte)2 || _model.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;
                <%  if (!_buyer.IsB2C())
                    {   %>
                應稅銷售額：<%=String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TaxType == 1 || _model.InvoiceAmountType.TaxType == 9 
                                                                        ? _model.InvoiceAmountType.SalesAmount 
                                                                        : _model.InvoiceAmountType.TaxType == 4
                                                                            ? _model.InvoiceAmountType.TotalAmount
                                                                            : 0) %>  &nbsp;&nbsp;
                零稅率銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)2 ? _model.InvoiceAmountType.SalesAmount : 0) %>  &nbsp;&nbsp;
                免稅銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)3 ? _model.InvoiceAmountType.SalesAmount : 0) %> &nbsp;&nbsp;
                稅額：<%= String.Format("{0:##,###,###,##0.##} ",_model.InvoiceBuyer.BuyerMark==3 || _model.InvoiceBuyer.BuyerMark==4 ? 0 : _model.InvoiceAmountType.TaxAmount) %>&nbsp;&nbsp;
                <%  } %>
                備註：<%= String.Join(";", _model.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)).Replace("\r\n","<br/>").Replace("\n","<br/>") %><br />
                退貨請憑電子發票證明聯辦理
            </p>
        </div>
        <% } %>
    </div>
    <% if(_viewModel?.PrintBack==true) { %>
<div style="text-align:center; position:absolute;margin-left:-0.3cm;margin-top:12cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
<div style="text-align:center; position:absolute;margin-left:19cm;margin-top:12cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
    <div class="container" style="border-bottom-style:none;border-top-style:none;margin-left:18px;margin-right:0px;padding-top:0.2cm;height:17cm;" >
        <div style="width: 7.2cm; height: 8.8cm; display: block; overflow: hidden; float: right;"></div>
        <div class="cutfield" style="float: right;">
            <h3 class="notop">領獎收據</h3>
            <p class="sign">
                發票年期別：<br />
                發票字軌號碼：<br />
                金額：新台幣<br />
                中獎人：<br />
                身分證字號：<br />
                地址：<br />
                電話：
            </p>
            <h3 class="notop">紙本電子發票注意事項</h3>
            <p class="rule1">中獎人請於領獎期間內攜帶國民身分證及本收執聯向下列郵局兌領，逾期不得領獎，影本不得進行領獎。</p>
            <p class="rule">(一) 特別獎、特獎及頭獎為各直轄市及各縣、市之指定郵局。</p>
            <p class="rule">(二) 二獎、三獎、四獎、五獎及六獎為各地郵局。</p>

        </div>
    </div>
    <% } %>
</div>
    <% } %>
    <% if(_productItem!=null && _productItem.Length>_SecondCheckCount)  { %>
<div class="br" style="page-break-after: always">
    <div class="listfield">
        <% for (; _itemIdx < _productItem.Length;_itemIdx++)
           {
               var item = _productItem[_itemIdx];
        %>
        <div class="content_box">
            <p class="productname"><%= item.InvoiceProduct.Brief%></p>
            <p class="quantity"><%= String.Format("{0:##,###,###,##0.##}", item.Piece)%></p>
            <p class="price"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></p>
            <p class="totalPrice"><%= String.Format("{0:##,###,###,##0.##}", item.CostAmount)%></p>
        </div>
        <% } %>
        <div class="content_box">
            <p style="border-top: 1px dotted #808080;">
                <span style="font-size: 12pt; font-weight: bold;">總計：<%=_model.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_model.InvoiceAmountType.TaxType == (byte)2 || _model.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;
                <%  if (!_buyer.IsB2C())
                    {   %>
                應稅銷售額：<%=String.Format("{0:##,###,###,##0.##}", _model.InvoiceAmountType.TaxType == 1 || _model.InvoiceAmountType.TaxType == 9 
                                                                        ? _model.InvoiceAmountType.SalesAmount 
                                                                        : _model.InvoiceAmountType.TaxType == 4
                                                                            ? _model.InvoiceAmountType.TotalAmount
                                                                            : 0) %>  &nbsp;&nbsp;
                零稅率銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)2 ? _model.InvoiceAmountType.SalesAmount : 0) %>  &nbsp;&nbsp;
                免稅銷售額：<%=String.Format("{0:##,###,###,##0.##}",_model.InvoiceAmountType.TaxType == (byte)3 ? _model.InvoiceAmountType.SalesAmount : 0) %> &nbsp;&nbsp;
                稅額：<%= String.Format("{0:##,###,###,##0.##} ",_model.InvoiceBuyer.BuyerMark==3 || _model.InvoiceBuyer.BuyerMark==4 ? 0 : _model.InvoiceAmountType.TaxAmount) %>&nbsp;&nbsp;
                <%  } %>
                備註：<%= String.Join(";", _model.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)).Replace("\r\n","<br/>").Replace("\n","<br/>") %><br />
                退貨請憑電子發票證明聯辦理
            </p>
        </div>
    </div>
</div>
        <% if((_productItem.Length-_SecondCheckCount) / 43 %2 == 0) { %>
<div class="br" style="page-break-after: always">&nbsp;</div>
        <% } %>
    <% } %>
<% } %>
<%--<p runat="server" style="page-break-after: always" visible="<%= printBack && !IsFinal %>"></p>--%>

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
