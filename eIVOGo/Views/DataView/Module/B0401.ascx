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

<% if(_model!=null)
   { %>
<div class="Invo_page">
    <!-- 列印頁面 -->
    <div class="pageOne">
        <%--<div class="title">
            <img src="images/chc_logo.gif" width="403" height="51"></div>--%>
        <div style="width: 60%; float: right; text-align: center;">
        <%  if (_model.InvoiceAllowanceSeller.Organization.LogoURL != null)
                { %>
                    <img id="logo" style="width: 200px; height: auto;" src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_model.InvoiceAllowanceSeller.Organization.LogoURL) %>' />
            <%  }   %>
            <h3>營業人銷貨退回、進貨退出或折讓證明單證明聯</h3>
            <p>開立日期：<%= String.Format("{0:yyyy-MM-dd}",_model.AllowanceDate) %></p>
        </div>
        <table width="40%" border="0" cellpadding="0" cellspacing="0" class="tball">
            <tr>
                <td width="40" rowspan="3" nowrap style="text-align: center; border-bottom-width: 0px;"><span style="text-align: left;">原發<br/>
                    開票<br/>
                    立單<br/>
                    銷位<br/>
                    貨&nbsp;&nbsp;&nbsp;</span></td>
                <td width="70" align="center" nowrap>營利事業<br/>
                    統一編號</td>
                <td><span class="contant-m"><%= _model.InvoiceAllowanceSeller.ReceiptNo %></span></td>
            </tr>
            <tr>
                <td width="70" align="center">名　　稱</td>
                <td><span class="contant-m"><%= _model.InvoiceAllowanceSeller.CustomerName %></span></td>
            </tr>
            <tr>
                <td width="70" align="center" style="border-bottom-width: 0px;">營業所在<br/>
                    地　　址</td>
                <td style="border-bottom-width: 0px;"><span class="contant-m"><%= _model.InvoiceAllowanceSeller.Address %></span></td>
            </tr>
        </table>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tball_list" height="100%">
            <tr>
                <th colspan="7" align="center"><span class="contant-m">開 立 發 票</span></th>
                <th colspan="4" align="center"><span class="contant-m">退　貨　或　折　讓　內　容</span></th>
                <th colspan="3" align="center"><span class="contant-m">課稅別<br/>
                    （<span class="contant">V</span>）</span></th>
            </tr>
            <tr>
                <th align="center">一<br/>
                    般<br/>
                    /<br/>
                    特<br/>
                    種</th>
                <th align="center">年</th>
                <th align="center">月</th>
                <th align="center">日</th>
                <th align="center">字<br/>
                    軌</th>
                <th align="center">號 碼</th>
                <th align="center">品　　名</th>
                <th align="center">數　量</th>
                <th align="center">單　價</th>
                <th align="center" nowrap>金　額<br/>
                    (不含稅之進貨額)</th>
                <th align="center" nowrap>營業稅額</th>
                <th align="center">應<br/>
                    <br/>
                    稅</th>
                <th align="center">零<br/>
                    稅<br/>
                    率</th>
                <th align="center">免<br/>
                    <br/>
                    稅</th>
            </tr>
            <% for (_itemIdx = 0; _itemIdx < 20 && _itemIdx < _products.Length; _itemIdx++)
               {
                    var prodItem = _products[_itemIdx];
                     %>
            <tr>
                <td align="center">一</td>
                <td align="center"><%= prodItem.InvoiceDate.Value.Year-1911 %></td>
                <td align="center"><%= prodItem.InvoiceDate.Value.Month %></td>
                <td align="center"><%= prodItem.InvoiceDate.Value.Day %></td>
                <td align="center"><%= prodItem.InvoiceNo.Substring(0,2) %></td>
                <td align="center"><%= prodItem.InvoiceNo.Substring(2) %></td>
                <td align="center"><%= prodItem.OriginalDescription %></td>
                <td align="center"><%= String.Format("{0:##,###,###,##0.##}", prodItem.Piece) %></td>
                <td align="center"><%= String.Format("{0:##,###,###,##0.##}", prodItem.UnitCost) %></td>
                <td align="center" nowrap><%= String.Format("{0:##,###,###,##0.##}", prodItem.Amount) %></td>
                <td align="center" nowrap><%= String.Format("{0:##,###,###,##0.##}", prodItem.Tax) %></td>
                <td align="center"><%= prodItem.TaxType==(byte)1?"V":null %></td>
                <td align="center"><%= prodItem.TaxType==(byte)2?"V":null %></td>
                <td align="center"><%= prodItem.TaxType==(byte)3?"V":null %></td>
            </tr>
            <% } %>
<%--            <tr>
                <td align="center"></td>
                <td align="center"></td>
                <td align="center"></td>
                <td align="center"></td>
                <td align="center"></td>
                <td align="center"></td>
                <td></td>
                <td align="right"></td>
                <td align="right"></td>
                <td align="right"></td>
                <td align="right"></td>
                <td align="center"></td>
                <td align="center"></td>
                <td align="center"></td>
            </tr>--%>
            <tr>
                <td colspan="9" align="center"><span class="contant-m">合　　　　　　　　　　　計</span></td>
                <td align="right"><%= String.Format("{0:##,###,###,##0.##}", _model.TotalAmount ) %></td>
                <td align="right"><%= String.Format("{0:##,###,###,##0.##}", _model.TaxAmount ) %></td>
                <td colspan="3" align="center"></td>
            </tr>
        </table>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" style="margin-top: 10px;">
            <tr>
                <td colspan="8">
                    <p style="margin-bottom: 10px;">本證明單所列進貨退出或折讓，確屬事實，特此證明。</p>
                    <p>
                        <% if(!_model.InvoiceAllowanceBuyer.IsB2C())
                           {  %>
                        原進貨營業人(或原買受人)名稱：<%= _model.InvoiceAllowanceBuyer.CustomerName %><br/>
                        營利事業統一編號：<%= _model.InvoiceAllowanceBuyer.ReceiptNo %><br/>
                        地址：<%= _model.InvoiceAllowanceBuyer.Address %><br/>
                        備註：<br/>
                        <% } %>
<%--                        <span class="txtnote">發貨通知單編號：APD15091148</span><br/>
                        <span class="txtnote">檢查號碼：01234</span><br/>
                        <span class="txtnote">訂單編號：P12346A</span><br/>
                        <span class="txtnote">信用狀編號：</span><br/>
                        <span class="txtnote">開立日期：2015-10-02</span>--%>
                    </p>
                </td>
                <td width="40%" align="center" valign="top">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tball">
                        <tr>
                            <td align="center">進貨營業人(或原買受人)<br/>
                                蓋統一發票專用章</td>
                        </tr>
                        <tr>
                            <td height="200" align="center">
                                <%  if(!_model.InvoiceAllowanceBuyer.IsB2C())
                                    {  %>
                                <div class="eivo_stamp">
                                    <%= _model.InvoiceAllowanceBuyer.CustomerName %><br/>
                                    統一編號<br/>
                                    <div class="notitle"><%= _model.InvoiceAllowanceBuyer.ReceiptNo %></div>
                                    <%= _model.InvoiceAllowanceBuyer.Address %><br/>
                                </div>
                                <%  } %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <!-- 列印頁面 -->
</div>
<% } %>

<script runat="server">


    ModelSource<InvoiceItem> models;
    InvoiceAllowance _model;
    protected InvoiceAllowanceItem[] _products;
    protected int _itemIdx;
    protected int _pageCount;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceAllowance)this.Model;

        _products = models.GetTable<InvoiceAllowanceDetail>().Where(d => d.AllowanceID == _model.AllowanceID)
            .Join(models.GetTable<InvoiceAllowanceItem>(), d => d.ItemID, i => i.ItemID, (p, i) => i)
            .OrderBy(i => i.ItemID).ToArray();
        _pageCount = (_products.Length + 19) / 20;
    }



</script>