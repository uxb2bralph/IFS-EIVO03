<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.Item.InvoiceReceiptView" %>
<%@ Register Src="SogoInvoiceProductPrintView.ascx" TagName="SogoInvoiceProductPrintView"
    TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<table width="99%" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td>&nbsp;</td>
        <td>
            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td colspan="2" align="center">
                        <table border="0" cellpadding="1" cellspacing="0">
                            <tr>
                                <td align="center" nowrap="nowrap" class="title">
                                    <img id="logo" runat="server" src="~/images/sogo.gif" alt="" name="" width="32" height="32" align="absmiddle" />
                                    <%# _item.InvoiceSeller.CustomerName  %>
                                 </td>
                            </tr>
                            <tr>
                                <td align="center" nowrap="nowrap" class="title"> 
                                    電子發票<%-- (
                                    <%# _item.CDS_Document.DocumentPrintLog.Any(l=>l.TypeID==(int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice)?"副本":"正本" %>
                                    )--%>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    中華民國&nbsp;<span class="f_black"><%# _item.InvoiceDate.Value.Year-1911 %></span>&nbsp;年&nbsp;<span class="f_black"><%# String.Format("{0:00}",_item.InvoiceDate.Value.Month) %></span>&nbsp;月&nbsp;<span
                                        class="f_black"><%# String.Format("{0:00}",_item.InvoiceDate.Value.Day)%></span>&nbsp;日
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="63%">
                        <table width="100%" border="0" cellpadding="1" cellspacing="0" class="tablenone-size">
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    發票號碼：
                                </td>
                                <td class="f_black">
                                    <%# _item.TrackCode%><%# _item.No %>
                                </td>
                                <td>
                                    <%--檢查號碼：--%>
                                </td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    買 受 人：
                                </td>
                                <td class="f_black">
                                    <%# _buyer.IsB2C() ? null : _buyer.CustomerName%>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    統一編號：
                                </td>
                                <td class="f_black">
                                    <%# _buyer.IsB2C()?null:_buyer.ReceiptNo %>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="12%" align="right" nowrap="nowrap">
                                    地　　址：
                                </td>
                                <td colspan="2" class="f_black">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td align="right">
                        <table border="0" cellpadding="0" cellspacing="0" class="table-or" style="margin-bottom: 3px;">
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
        <td align="right" valign="top">
            <table width="0%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="top">                    
                        <p style="margin:0px;">可<br/>扣<br/>抵<br/>外<br/>，<br/>其<br/>餘<br/>均<br/>得<br/>扣<br/>抵<br/>，<br/>並<br/>在<br/>各<br/>該<br/>適<br/>當<br/>欄<br/>內<br/>打<br/>﹁<br/>ｖ<br/>﹂<br/>符<br/>號<br/>。</p></td>
                    <td valign="top">
                        <p style="margin:0px;">﹁<br/>固<br/>定<br/>資<br/>產<br/>﹂<br/>，<br/>其<br/>進<br/>項<br/>稅<br/>額<br/>，<br/>除<br/>營<br/>業<br/>稅<br/>法<br/>第<br/>十<br/>九<br/>條<br/>第<br/>一<br/>項<br/>屬<br/>不</p></td>
                    <td valign="top">
                        <p style="margin:0px;">營<br/>業<br/>人<br/>購<br/>進<br/>貨<br/>物<br/>或<br/>勞<br/>務<br/>應<br/>先<br/>按<br/>其<br/>用<br/>途<br/>區<br/>分<br/>為<br/>﹁<br/>進<br/>貨<br/>及<br/>費<br/>用<br/>﹂<br/>與</p>
                    </td>
                    <td valign="top">買<br/>受<br/>人<br/>註<br/>記<br/>欄<br/>之<br/>註<br/>記<br/>方<br/>法<br/>：</td>
                    <td valign="top">第<br/>二<br/>聯<br/>：<br/>扣<br/>抵<br/>聯</td>
                </tr>
            </table>
        </td>
        <td valign="top" style="position:relative;overflow:hidden;">
            <div style="position:absolute;top:0px;left:38%;z-index:-1;border-right:1px solid #c76c42;width:1px;height:100%;">&nbsp;</div>
            <div style="position:absolute;top:0px;left:48%;z-index:-1;border-right:1px solid #c76c42;width:1px;height:100%;">&nbsp;</div>
            <div style="position:absolute;top:0px;left:64%;z-index:-1;border-right:1px solid #c76c42;width:1px;height:100%;">&nbsp;</div>
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="orcube">
                <tr>
                    <td width="80%" valign="top" style="position:relative;border-right:1px solid #c76c42;">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="item_or_noright" style="margin-bottom:100px;">
                            <tr>
                                <th width="48%" height="20">
                                    品　　　　名
                                </th>
                                <th width="12%" height="20">
                                    數　　量
                                </th>
                                <th width="20%" height="20">
                                    單　　價
                                </th>
                                <th height="20">
                                    金　　額
                                </th>
                            </tr>
                            <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <uc1:SogoInvoiceProductPrintView ID="productView" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>   
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table_or_noborder">
                            <tr>
                                <td height="20" colspan="4" align="center" class="or_noboder_right">
                                    銷　售　額　合　計
                                </td>
                                <td width="20%" height="20" align="right" class="f_black or_noboder_right or_nobgcolor">
                                    <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.SalesAmount) %>
                                </td>
                            </tr>
                            <tr>
                                <td rowspan="2" align="center" nowrap="nowrap">
                                    營　業　稅
                                </td>
                                <td width="20%" height="20" align="center" nowrap="nowrap">
                                    應稅
                                </td>
                                <td width="20%" height="20" align="center" nowrap="nowrap">
                                    零稅率
                                </td>
                                <td width="20%" height="20" align="center" nowrap="nowrap" class="or_noboder_right">
                                    免稅
                                </td>
                                <td width="20%" rowspan="2" align="right" class="f_black or_noboder_right or_nobgcolor">
                                    <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TaxAmount) %>
                                </td>
                            </tr>
                            <tr>
                                <td width="20%" height="18" align="center" nowrap="nowrap">
                                    <span class="f_black"><%# _item.InvoiceAmountType.TaxType == (byte)1 ? "V" : "&nbsp;"%></span>
                                </td>
                                <td width="20%" height="18" align="center" nowrap="nowrap">
                                    <%# _item.InvoiceAmountType.TaxType == (byte)2 ? "V" : "&nbsp;"%>
                                </td>
                                <td width="20%" height="18" align="center" nowrap="nowrap" class="or_noboder_right">
                                    <%# _item.InvoiceAmountType.TaxType == (byte)3 ? "V" : "&nbsp;"%>
                                </td>
                            </tr>
                            <tr>
                                <td height="20" colspan="4" align="center" class="or_noboder_right">
                                    總　　　　　　　計
                                </td>
                                <td width="20%" height="20" align="right" class="f_black or_noboder_right or_nobgcolor">
                                    <%# String.Format("{0:##,###,###,##0.##}",_item.InvoiceAmountType.TotalAmount) %>
                                </td>
                            </tr>
                            <tr>
                                <td height="20" colspan="5" style="border-bottom-width:0px;" class="or_noboder_right">
                                　　總計新台幣： <span class="f_black">
                                    <%# _totalAmtChar[7] %></span> 仟 <span class="f_black">
                                    <%# _totalAmtChar[6] %></span> 佰 <span class="f_black">
                                    <%# _totalAmtChar[5] %></span> 拾 <span class="f_black">
                                    <%# _totalAmtChar[4] %></span> 萬 <span class="f_black">
                                    <%# _totalAmtChar[3] %></span> 仟 <span class="f_black">
                                    <%# _totalAmtChar[2] %></span> 佰 <span class="f_black">
                                    <%# _totalAmtChar[1] %></span> 拾 <span class="f_black">
                                    <%# _totalAmtChar[0] %></span> 元整
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td height="310" valign="top" style="position:relative;">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table-or" style="border-width: 0px;">
                            <tr>
                                <th height="20" style="border-right-width: 0px;">
                                    備　　註
                                </th>
                            </tr>
                             <tr>
                              <td height="15" valign="top" class="f_black" style="border-width:0px;">
                                格式代號 : <%#PrintFormat((int)_item.InvoiceType)%>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" class="f_black" style="border-width: 0px;">
                                    <pre><%# String.Format("{0}{1}",_item.Remark, (_item.InvoiceDetails.Count >0 ? _item.InvoiceDetails[0].InvoiceProduct.InvoiceProductItem[0].Remark : null)) %></pre><br />
                                    <span class="f_black" style="border-right-width: 0px;">
                                        <%--<%# _buyer.IsB2C() ? String.Format("個人識別碼:{0}", _buyer.Name) : null%>--%></span>&nbsp;
                                </td>
                            </tr>
                           
                            </table>
                            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table-or" style="border-left-width: 0px;position:absolute;bottom:0px;left:0px;">
                            <tr>
                                <th height="20" align="center" style="border-right-width: 0px;">
                                    營業人蓋用統一發票專用章
                                </th>
                            </tr>
                            <tr>
                                <td height="105" align="center" style="border-bottom-width: 0px; border-right-width: 0px;">
                                   <div class="eivo_stamp">
                                        <%# _item.InvoiceSeller.CustomerName %><br />
                                        統一編號<br />
                                    <div class="notitle">
                                        <%# _item.InvoiceSeller.ReceiptNo %>
                                    </div>
                                        電話:<%# _item.InvoiceSeller.Phone%><br />
                                        <%# _item.InvoiceSeller.Address %><br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table width="100%" border="0" cellpadding="1" cellspacing="0" style="background-color:#FFF;">
                <tr>
                    <td>
                        ※應稅、零稅率、免稅之銷售額應分別開立統一發票，並應於各該欄打「<span class="contant">V</span>」。<br/>
                        本發票依財北國稅中正營業字第0990031712號函核准使用。<br/>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>