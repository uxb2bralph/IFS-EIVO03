<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.NewInvoicePrintView" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>

<% if(_item!=null) { %>
<div style="text-align:center; position:absolute;margin-left:0cm;height:7cm;margin-top:2cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
<div style="text-align:center; position:absolute;margin-left:19.6cm;height:7cm;margin-top:2cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
<div style="page-break-after: always;margin-left:0.5cm;margin-right:0.5cm;margin-bottom:0cm;margin-top:0.5cm;">
    <div class="fspace" style="margin-left:0.2cm;margin-right:0.2cm;border-bottom-style:none;height: 8.6cm;">
        <div class="company" style="padding-left:1cm">
            <span class="title"><%= _item.InvoiceSeller.CustomerName  %></span><br />
            <p>
                <%= _item.InvoiceSeller.Address  %><br />
                <%= _item.Organization.OrganizationStatus.SetToOutsourcingCS == true ? "委外客服電話：0800-010-626" : ""%>
            </p>
        </div>
        <div style="margin-left:160px;margin-top:110px;font-size:medium">
            <p>
                <%= _buyer.PostCode %><br />
                <%= _buyer.Address %><br />
                <%=_item.InvoiceSeller.ReceiptNo == "27934855" ? _buyer.CustomerName + "  " + _buyer.ContactName : _buyer.ContactName%><%--<%= _buyer.ContactName %> --%>
            鈞啟
            </p>
        </div>
    </div>
    <div class="container"  style="border-bottom-style:none;border-top-style:none;margin-left:0.2cm" >
        <div style="width: 7.2cm; height: 9cm; display: block; overflow: hidden; float: left;"></div>
        <div class="cutfield" style="margin-left:-30px;padding-top:0cm;">
            <%  if (_item.Organization.LogoURL != null)
                { %>
                    <img id="logo" style="width: 200px; height: auto;" src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_item.Organization.LogoURL) %>' />
            <%  }
                else
                { %>
                    <h3><%= _item.InvoiceSeller.CustomerName  %></h3>
            <%  } %>
            <h2>電子發票證明聯</h2>
            <h2><%= _item.InvoiceDate.Value.Year-1911 %>年<%= (_item.InvoiceDate.Value.Month % 2).Equals(0) ? String.Format("{0:00}-{1:00}", _item.InvoiceDate.Value.Month - 1, _item.InvoiceDate.Value.Month) : String.Format("{0:00}-{1:00}", _item.InvoiceDate.Value.Month, _item.InvoiceDate.Value.Month+1)%>月 </h2>
            <h2><%= _item.TrackCode + "-" + _item.No %></h2>
            <p>
                <%= String.Format("{0:yyyy-MM-dd HH:mm:ss}", _item.InvoiceDate.Value)%> <%= _buyer.IsB2C()
                                                                                                ? ""
                                                                                                : _item.InvoiceBuyer.BuyerMark==3 || _item.InvoiceBuyer.BuyerMark==4
                                                                                                    ? "不得扣抵" 
                                                                                                    : String.Format("格式 {0}", ((int)_item.InvoiceType.Value).InvoiceTypeToFormat()) %><br />
                隨機碼 <%= _item.RandomNo %>&nbsp;&nbsp;&nbsp;&nbsp;總計 <%= String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TotalAmount) %><br />
                賣方<%= _item.Organization.ReceiptNo%> <%= _buyer.IsB2C() ? null : String.Format("買方{0}", _buyer.ReceiptNo)%>
            </p>
            <div class="code1">
                <img id="barcode" alt="" runat="server" width="160" height="22" src="" />
            </div>
            <div class="code2">
                <img id="qrcode1" alt="" runat="server" width="80" height="80" src="" /><img id="qrcode2" alt="" runat="server" width="80" height="80" src="" />
            </div>
        </div>
    </div>
    <div class="listfield" style="margin-left:1.1cm;margin-right:0cm;width:700px">
        <div class="content_box">
            <p class="productname" >品名</p>
            <p class="quantity" style="width:50px;">數量</p>
            <p class="price" style="width:140px;">單價</p>
            <p class="totalPrice">小計</p>
        </div>
        <% int _itemIdx;
           for (_itemIdx = 0; _productItem!=null &&  _itemIdx < Math.Min(_productItem.Length, _ItemPagingCount);_itemIdx++)
           {
               var item = _productItem[_itemIdx];
            %>
        <div class="content_box">
            <p class="productname"><%= item.InvoiceProduct.Brief%></p>
            <p class="quantity" style="width:50px;"><%= String.Format("{0:##,###,###,##0.##}", item.Piece)%></p>
            <p class="price" style="width:140px;"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></p>
            <p class="totalPrice"><%= String.Format("{0:##,###,###,##0.##}", item.CostAmount)%></p>
        </div>
        <% } %>
        <% if (_productItem != null && _productItem.Length <= _FirstCheckCount)
           { %>
        <div class="content_box" >
            <p style="border-top: 1px dotted #808080;margin-right:1.6cm;">
                <span style="font-size: 12pt; font-weight: bold;">總計：<%=_item.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_item.InvoiceAmountType.TaxType == (byte)2 || _item.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("應稅銷售額：{0:##,###,###,##0.##}  零稅率銷售額：{1:##,###,###,##0.##}  免稅銷售額：{2:##,###,###,##0.##}", ((_item.InvoiceAmountType.TaxType != (byte)2 && _item.InvoiceAmountType.TaxType != (byte)3) ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)2 ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)3 ? _item.InvoiceAmountType.SalesAmount : 0)) : ""%>
            &nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("稅額：{0:##,###,###,##0.##} ", _item.InvoiceAmountType.TaxAmount) : ""%>&nbsp;&nbsp;備註：<%= String.Join(";", _item.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark))%><br />
                退貨請憑電子發票證明聯辦理
                <%  if (_item.InvoiceWinningNumber != null && _item.InvoiceCarrier != null && _item.InvoiceCarrier.CarrierType == "3J0002")
                    { %>
                        (貴用戶如已使用手機條碼設定為自動匯款，請勿重複領獎)
                <%  } %>
            </p>
        </div>
        <% } %>
    </div>
</div>
<%--<p runat="server" style="page-break-after: always" visible="<%= printBack ? true : (!IsFinal ? true : false) %>"></p>--%>
    <% if(printBack || (_productItem!=null && _productItem.Length>_FirstCheckCount))  { %>
<div class="br" id="backPage" style="page-break-after: always;margin-left:0.5cm;margin-right:0.5cm;border-bottom-style:none;">
    <div class="bspace listfield">
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
                <span style="font-size: 12pt; font-weight: bold;">總計：<%=_item.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_item.InvoiceAmountType.TaxType == (byte)2 || _item.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("應稅銷售額：{0:##,###,###,##0.##}  零稅率銷售額：{1:##,###,###,##0.##}  免稅銷售額：{2:##,###,###,##0.##}", ((_item.InvoiceAmountType.TaxType != (byte)2 && _item.InvoiceAmountType.TaxType != (byte)3) ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)2 ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)3 ? _item.InvoiceAmountType.SalesAmount : 0)) : ""%>
            &nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("稅額：{0:##,###,###,##0.##} ", _item.InvoiceAmountType.TaxAmount) : ""%>&nbsp;&nbsp;備註：<%= String.Join(";", _item.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark))%><br />
                退貨請憑電子發票證明聯辦理
            </p>
        </div>
        <% } %>
    </div>
    <% if(printBack) { %>
<div style="text-align:center; position:absolute;margin-left:-0.6cm;margin-top:12cm;font-size:small" >請<br />沿<br />虛<br />線<br />先<br />摺<br />再<br />撕</div>
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
                <span style="font-size: 12pt; font-weight: bold;">總計：<%=_item.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_item.InvoiceAmountType.TaxType == (byte)2 || _item.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("應稅銷售額：{0:##,###,###,##0.##}  零稅率銷售額：{1:##,###,###,##0.##}  免稅銷售額：{2:##,###,###,##0.##}", ((_item.InvoiceAmountType.TaxType != (byte)2 && _item.InvoiceAmountType.TaxType != (byte)3) ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)2 ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)3 ? _item.InvoiceAmountType.SalesAmount : 0)) : ""%>
            &nbsp;&nbsp;<%= !_buyer.IsB2C() ? String.Format("稅額：{0:##,###,###,##0.##} ", _item.InvoiceAmountType.TaxAmount) : ""%>&nbsp;&nbsp;備註：<%= String.Join(";", _item.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark))%><br />
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
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>
<script runat="server">
    bool printBack = false;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!String.IsNullOrEmpty(Request["printBack"]))
        {
            bool.TryParse(Request["printBack"], out printBack);
        }
    }
    
    [System.ComponentModel.Bindable(true)]
    public String InvoiceNo
    {
        get
        {
            return _item != null ? _item.TrackCode + _item.No : null;
        }
        set
        {
            if (value != null && value.Length == 10)
            {
                var item = dsEntity.CreateDataManager().EntityList.Where(i => i.TrackCode == value.Substring(0, 2) && i.No == value.Substring(2)).FirstOrDefault();
                if (item != null)
                {
                    this.InvoiceID = item.InvoiceID;
                }
            }
        }
    }
    
</script>
