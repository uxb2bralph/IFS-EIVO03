﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.NewInvoicePrintView" %>
<%@ Register Src="Item/InvoiceProductPrintView.ascx" TagName="InvoiceProductPrintView"
    TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Register Src="Item/InvoiceReceiptView.ascx" TagName="InvoiceReceiptView" TagPrefix="uc2" %>
<%@ Register Src="Item/InvoiceBalanceView.ascx" TagName="InvoiceBalanceView" TagPrefix="uc3" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="Item/NewInvoicePOSProductPrintView.ascx" TagName="NewInvoicePOSProductPrintView" TagPrefix="uc4" %>

<%  if(_item!=null)
    {
        if (_item.Organization.CompanyName != null && _item.Organization.CompanyName.Length > 16)
        {   %>
<style>
    .title-small {
        font-size: small !important;
    }
</style>
    <%  } %>
<div style="page-break-after: always;width:5.5cm;margin-left: 0cm;margin-right:0cm">
    
    <div class="container" style="<%= _buyer.IsB2C() ? "page-break-after: always;" : null %>border-top:0px;border-bottom: 0px">
        <table >
            <tr>
                <td>
                    <div class="cutfield" style="/*width:5cm;*/border-top:0px;border:0px;/*font-weight:bold;*/">
                    <%  if (_item.Organization.LogoURL != null)
                        { %>
                            <img id="logo" style="width:200px;height:auto;"  src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_item.Organization.LogoURL) %>' />
                <%      }
                        else
                        { %>
                            <div style="text-align:left; width:4.4cm">
                                <h3 class="title-small" style="width:4.2cm; padding-top:0px;font-weight:bold; height: 1.3cm;"><%=_item.Organization.CompanyName %></h3>
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
                        <h2><%= _item.InvoiceDate.Value.Year-1911 %>年<%= (_item.InvoiceDate.Value.Month % 2).Equals(0) ? String.Format("{0:00}-{1:00}", _item.InvoiceDate.Value.Month - 1, _item.InvoiceDate.Value.Month) : String.Format("{0:00}-{1:00}", _item.InvoiceDate.Value.Month, _item.InvoiceDate.Value.Month+1)%>月 </h2>
                        <h2><%= _item.TrackCode + "-" + _item.No %></h2>
                        <p>
                            <%= String.Format("{0:yyyy-MM-dd HH:mm:ss}", _item.InvoiceDate.Value)%> <%= _buyer.IsB2C()
                                                                                                            ? ""
                                                                                                            : _item.InvoiceBuyer.BuyerMark==3 || _item.InvoiceBuyer.BuyerMark==4
                                                                                                                ? "不得扣抵" 
                                                                                                                : String.Format("格式 {0}", ((int)_item.InvoiceType.Value).InvoiceTypeToFormat()) %><br />
                            隨機碼 <%= _item.RandomNo %>&nbsp;&nbsp;&nbsp;&nbsp; 總計 <%= String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TotalAmount) %><br />
                            賣方<%= _item.Organization.ReceiptNo%> <%= _buyer.IsB2C() ? null : String.Format("買方{0}", _buyer.ReceiptNo)%>
                        </p>
                        <div class="code1">
                            <img id="barcode" alt="" runat="server" width="160" height="22" src="" />
                        </div>
                        <div class="code2" >
                            <img style="/*margin-left:0.3cm;*/margin-right:0.3cm;" id="qrcode1" alt="" runat="server" width="80" height="80" src="" />
							<img style="/*margin-left:0.3cm;*/margin-right:0.3cm;" id="qrcode2" alt="" runat="server" width="80" height="80" src="" />
                        </div>
                    </div>                             
                </td>
           
            </tr> </table>
        </div>
          
        <div class="listfield" style="border-top:0px;border-bottom: 0px">
         <table style="width:4.8cm;font-size:8pt;font-weight:bold;">
             <tr>
                <td colspan="3">
                            <p style="	display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">品名</p>
                </td>
             </tr>
            <tr>
                <td style="width:20%">
                    <p style="	display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">數量</p>
                    </td>
                <td style="width:40%">
                    <p style="	display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">單價</p>
                    </td>
                <td style="width:40%">
                    <p style="	display: inline-block;padding: 2px 0px;margin: 0;font-size:8pt;line-height: 1.5">小計</p>
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
                <span style="font-size: 8pt;">總計：<%=_item.InvoiceDetails.Count%>項&nbsp;&nbsp;金額：<%= String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TotalAmount)%></span><br />
                課稅別：<%= (_item.InvoiceAmountType.TaxType == (byte)2 || _item.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX"%>&nbsp;&nbsp;<br />
                <% if (!_buyer.IsB2C())
                    {   %>
                應稅銷售額：<%=String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TaxType == 1 || _item.InvoiceAmountType.TaxType == 9 
                                                                        ? _item.InvoiceAmountType.SalesAmount 
                                                                        : _item.InvoiceAmountType.TaxType == 4
                                                                            ? _item.InvoiceAmountType.TotalAmount
                                                                            : 0) %>  <br/>
                零稅率銷售額：<%=String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TaxType == (byte)2 ? _item.InvoiceAmountType.SalesAmount : 0) %>  <br/>
                免稅銷售額：<%=String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TaxType == (byte)3 ? _item.InvoiceAmountType.SalesAmount : 0) %> <br/>
                稅額：<%= String.Format("{0:##,###,###,##0.##} ",_item.InvoiceBuyer.BuyerMark==3 || _item.InvoiceBuyer.BuyerMark==4 ? 0 : _item.InvoiceAmountType.TaxAmount) %><br />
                <%  } %>
                備註：<%= String.Join(";", _item.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)).Replace("\r\n","<br/>").Replace("\n","<br/>") %><br />
                退貨請憑電子發票證明聯辦理<br />
                <%  if (_item.InvoiceWinningNumber != null && _item.InvoiceCarrier != null && _item.InvoiceCarrier.CarrierType == "3J0002")
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
                                <span style="font-size: 8pt; font-weight: bold;">總計：<%#_item.InvoiceDetails.Count %>項&nbsp;&nbsp;金額：<%# String.Format("{0:##,###,###,##0.##}", _item.InvoiceAmountType.TotalAmount)%></span><br />
                                課稅別：<%# (_item.InvoiceAmountType.TaxType == (byte)2 || _item.InvoiceAmountType.TaxType == (byte)3) ? "TZ" : "TX" %>&nbsp;&nbsp;<%# !_buyer.IsB2C() ? String.Format("應稅銷售額：{0:##,###,###,##0.##}  零稅率銷售額：{1:##,###,###,##0.##}  免稅銷售額：{2:##,###,###,##0.##}", ((_item.InvoiceAmountType.TaxType != (byte)2 && _item.InvoiceAmountType.TaxType != (byte)3) ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)2 ? _item.InvoiceAmountType.SalesAmount : 0), (_item.InvoiceAmountType.TaxType == (byte)3 ? _item.InvoiceAmountType.SalesAmount : 0)) : ""%>
            &nbsp;&nbsp;<%# !_buyer.IsB2C() ? String.Format("稅額：{0:##,###,###,##0.##} ",  _item.InvoiceAmountType.TaxAmount) : ""%>&nbsp;&nbsp;備註：<%# String.Join(";", _item.InvoiceDetails.Select(d => d.InvoiceProduct.InvoiceProductItem.FirstOrDefault().Remark)) %><br />
                                退貨請憑電子發票證明聯辦理
                            </p>
                        </div>
                    
                </td>
            </tr>--%>
        </table>
    </div>
</div>
    <% if(Request["printBuyerAddr"]=="true") { %>
<div style="page-break-after: always; width: 5.2cm; margin-left: 0cm; margin-right: 0cm; padding-top:0px;">
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.PostCode %></span><br>
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.Address %></span><br><br>
    <span style="font-size:1.2em;font-weight:bold;"><%= _buyer.ContactName %> 鈞啟</span><br>
    <%--<span style="font-size:1.2em;font-weight:bold;">(No:<%=_item.No.Substring(0,5)+"***" %>)</span>--%>
</div>
    <% } %>
<% } %>

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
