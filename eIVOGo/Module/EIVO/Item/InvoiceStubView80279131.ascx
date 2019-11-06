<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.Item.InvoiceReceiptView" %>
<%@ Register Src="InvoiceProductPrintView80279131.ascx" TagName="InvoiceProductPrintView"
    TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
  <style type="text/css" media="print">
        div.fspace
        {
            height: 8.8cm;
        }
        div.bspace
        {
            height: 9.5cm;
        }
       
    </style>
    
<div >
<table width="95%" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" class="title">
                         <%# _item.InvoiceSeller.CustomerName  %>
                    </td>
                </tr>
            </table>
            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td width="67%">
                        <table width="100%" border="0" cellpadding="1" cellspacing="0" class="tablenone-size">
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    發票號碼：
                                </td>
                                <td class="f_black">
                                     <%# _item.TrackCode + _item.No %>
                                </td>
                                <td rowspan="4" align="right" valign="top">
                                    <table border="0" cellpadding="0" cellspacing="0" class="tablenone">
                                        <tr>
                                            <td align="center" nowrap="nowrap" class="title3"> 
                                            電子發票</td>
                                        </tr>
                                        <tr>
                                              <td align="center">
                                                中華民國&nbsp;<span class="f_black"><%# _item.InvoiceDate.Value.Year-1911 %></span>&nbsp;年&nbsp;<span
                                                    class="f_black"><%# String.Format("{0:00}",_item.InvoiceDate.Value.Month) %></span>&nbsp;月&nbsp;<span
                                                        class="f_black"><%# String.Format("{0:00}",_item.InvoiceDate.Value.Day)%></span>&nbsp;日
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" nowrap="nowrap">
                                    檢查號碼：
                                </td>
                                <td class="f_black">
                                      <%# _item.CheckNo %>
                                </td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    買 受 人：
                                </td>
                                <td class="f_black">
                                      <%# _buyer.IsB2C() ? null : _buyer.CustomerName%>
                                </td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    統一編號：
                                </td>
                                <td class="f_black">
                                    <%# _buyer.IsB2C()?null:_buyer.ReceiptNo %>
                                </td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    地　　址：
                                </td>
                                <td colspan="2" class="f_black">
                                    <%--<%# _buyerOrg != null ? _buyerOrg.Addr : _buyer != null ? _buyer.IsB2C() ? null : _buyer.Address : null%>--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td align="right">
                        <table border="0" cellpadding="0" cellspacing="0" class="table-bk" style="margin-bottom: 3px;">
                            <tr>
                                <td colspan="3" align="center">
                                    買受人註記欄
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    區 分
                                </td>
                                <td align="center">
                                    進貨及費用
                                </td>
                                <td align="center">
                                    固定資產
                                </td>
                            </tr>
                            <tr>
                                <td align="center" nowrap="nowrap">
                                    得 扣 抵
                                </td>
                                <td align="center" class="f_black">
                                    <%# _item.BuyerRemark==1?"V":"&nbsp;" %>
                                </td>
                                <td align="center" class="f_black">
                                    <%# _item.BuyerRemark==2?"V":"&nbsp;" %>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    不得扣抵
                                </td>
                               <td align="center" class="f_black">
                                    <%# _item.BuyerRemark==3?"V":"&nbsp;" %>
                                </td>
                                <td align="center" class="f_black">
                                    <%# _item.BuyerRemark==4?"V":"&nbsp;" %>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td valign="top" style="position:relative;overflow:hidden;">
            <div style="position: absolute; top: 0px; left: 42%; z-index: -1; border-right: 1px solid #000000;
                width: 1px; height: 100%;">&nbsp;</div>
            <div style="position: absolute; top: 0px; left: 51%; z-index: -1; border-right: 1px solid #000000;
                width: 1px; height: 100%;">&nbsp;</div>
            <div style="position: absolute; top: 0px; left: 62%; z-index: -1; border-right: 1px solid #000000;
                width: 1px; height: 100%;">&nbsp;</div>
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="bkcube">
                <tr>
                    <td valign="top" style="position: relative; border-right: 1px solid #000000;">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="item_bk_noright"
                        style="margin-bottom: 100px;">
                      <tr>
                          <th width="55%" height="20">品&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;名</th>
                          <th width="10%">數&nbsp;&nbsp;&nbsp;量</th>
                          <th width="15%">單&nbsp;&nbsp;&nbsp;價</th>
                          <th>金&nbsp;&nbsp;&nbsp;額</th>
                      </tr>
                        <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <uc1:InvoiceProductPrintView ID="productView" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table_bk_noborder"
                            style="border-top: 1px solid #000000;">
                            <tr>
                                <td colspan="7" align="center" class="bk_noboder_right">銷 售 額 合 計</td>
                                <td width="120" align="right" class="f_black "> <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.SalesAmount) %></td>
                            </tr>
                            <tr>
                                <td width="100" align="center" nowrap="nowrap" class="bk_right_line bk_bg">營業稅</td>
                                <td width="80" align="center" nowrap="nowrap" class="bk_right_line bk_bg">應稅</td>
                                <td width="25" align="center" nowrap="nowrap" class="f_black bk_right_line bk_bg">  <%# _item.InvoiceAmountType.TaxType == (byte)1 ? "V" : "&nbsp;"%></td>
                                <td width="80" align="center" nowrap="nowrap" class="bk_right_line bk_bg">零稅率</td>
                                <td width="25" align="center" nowrap="nowrap" class="f_black bk_right_line bk_bg"> <%# _item.InvoiceAmountType.TaxType == (byte)2 ? "V" : "&nbsp;"%></td>
                                <td width="80" align="center" nowrap="nowrap" class="bk_right_line bk_bg">免稅</td>
                                <td width="25" align="center" nowrap="nowrap" class="f_black bk_bg"> <%# _item.InvoiceAmountType.TaxType == (byte)3 ? "V" : "&nbsp;"%></td>
                                <td width="120" align="right" class="f_black">  <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TaxAmount) %></td>
                            </tr>
                            <tr>
                                <td colspan="7" align="center" class="bk_noboder_right">總 計</td>
                                <td width="120" align="right" class="f_black"> <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TotalAmount) %></td>
                            </tr>
                            <tr>
                                <td height="15" colspan="8" class="bk_noboder_right" style="border-bottom-width: 0px;">
                                    總計新台幣： <span class="f_black">
                                          <%# _totalAmtChar[7] %></span> 仟 <span class="f_black">
                                            <%# _totalAmtChar[6] %></span> 佰 <span class="f_black">
                                                <%# _totalAmtChar[5] %></span> 拾 <span class="f_black">
                                                    <%# _totalAmtChar[4] %></span> 萬 <span class="f_black">
                                                        <%# _totalAmtChar[3] %></span> 仟 <span class="f_black">
                                                            <%# _totalAmtChar[2] %></span> 佰 <span class="f_black">
                                                                <%# _totalAmtChar[1] %></span> 拾 <span class="f_black">
                                                                    <%# _totalAmtChar[0] %></span> 元 <span class="f_black">零</span>
                                    角 <span class="f_black">零</span> 分 元整
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="165" valign="top" style="border-left: 1px solid #000000; position: relative;">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table-bk" style="border-width: 0px;
                            margin-bottom: 135px;">
                            <tr>
                                <th height="20" style="border-right-width: 0px;">備&nbsp;&nbsp;&nbsp;註</th>
                            </tr>
                             <tr>
                                 <td height="15" valign="top" class="f_black" style="border-width:0px;">
                                格式代號 : <%#PrintFormat((int)_item.InvoiceType)%>
                                </td>
                            </tr>
                            <tr>
                                <td height="40" valign="top" class="f_black" style="border-width:0px;">
                                  <%# String.Format("{0}{1}",_item.Remark, (_item.InvoiceDetails.Count >0 ? String.Join("<br/>",_item.InvoiceDetails.Select(d=>String.Join("<br/>",d.InvoiceProduct.InvoiceProductItem.Select(p=>p.Remark)))) : null)) %><br />
                                    <span class="f_black" style="border-right-width: 0px;">
                                        <%--<%# _buyer.IsB2C() ? String.Format("個人識別碼:{0}", _buyer.Name) : null%>--%></span>
                                    <span class="f_black" style="font-weight: bolder;">更正發票請於5日內辦理逾期恕不受理</span>
                                </td>
                            </tr>
                         </table>
                         <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table-bk" style="border-left-width: 0px;
                             position: absolute; bottom: 0px;">
                            <tr>
                                <th height="20" align="center" style="border-right-width: 0px;">營業人蓋用統一發票專用章</th>
                            </tr>
                            <tr>
                                <td height="105" align="center" style="border-bottom-width: 0px; border-right-width: 0px;">
                                    <div class="eivo_stamp">
                                         <%# _item.InvoiceSeller.CustomerName %><br />
                                        統一編號<br />
                                        <div class="notitle">
                                            <%# _item.InvoiceSeller.ReceiptNo %></div>
                                        電話:<%# _item.InvoiceSeller.Phone%><br />
                                        <%# _item.InvoiceSeller.Address %><br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tablenone">
                <tr>
                    <td align="center">
                         第一聯：存根聯
                    </td>
                </tr>
                <tr>
                    <td>
                        ※應稅、零稅率、免稅之銷售額應分別開立統一發票，並應於各該欄打「<span class="contant">V</span>」。<br />
                        本發票依臺北市國稅局中正分局99年12月28日財北國稅中正營業字第0990031712號函核准使用。<br />
                        買受人註記欄之註記方法：<br />
                        營業人購進貨物或勞務應先按其用途區分為「進貨及費用」與「固定資產」，其進項稅額，除營業稅法第19條第1項屬不可扣抵外，其餘均得扣抵，並在各該適當欄內打「<span
                            class="contant">V</span>」符號。
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</div>
