﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoicePaperView.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.InvoicePaperView" %>
<%@ Register Src="Item/InvoiceProductPrintView.ascx" TagName="InvoiceProductPrintView"
    TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="Item/InvoiceReceiptView53147117.ascx" TagName="InvoiceReceiptView"
    TagPrefix="uc2" %>
<%@ Register Src="Item/InvoiceBalanceView53147117.ascx" TagName="InvoiceBalanceView"
    TagPrefix="uc3" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<div class="fspace">
    <div class="company">
        <span class="title">晨創有限公司</span><br />
        <p>
            新北市板橋區中山路二段50之2號5樓<br />
            客服電話：02-89538100</p>
    </div>
    <div class="customer">
        <p>
            <%# _buyer.PostCode %><br />
            <%# _buyer.Address %><br />
            <%# _buyer.ContactName %>
            鈞啟</p>
    </div>
</div>
<p style="border-bottom: 1px dotted #000; margin-bottom: 15px">
</p>
<div class="Khaki">
    <uc2:InvoiceReceiptView ID="receiptView" runat="server" Item="<%# _item %>" />
</div>
<p style="border-bottom: 1px dotted #000; margin-top: 13px; margin-bottom: 10px;">
</p>
<div class="or">
    <uc3:InvoiceBalanceView ID="balanceView" runat="server" Item="<%# _item %>" />
</div>
<p style="page-break-after: always">
</p>
<div class="br">
    <div class="bspace">
    </div>
    <table width="95%" border="0" align="center" cellpadding="3" cellspacing="0">
        <tr>
            <td>
                <table width="100%" border="0" cellpadding="2" cellspacing="0">
                    <tr>
                        <td height="30" colspan="2" align="center" class="title">
                            統一發票領獎注意事項
                        </td>
                    </tr>
                    <tr>
                        <td width="50%" valign="top">
                            <div class="notice" style="float: right;">
                                <p>
                                    一、統一發票之給獎，依統一發票給獎辦法之規定辦理。</p>
                                <p>
                                    二、各期統一發票開獎日期及領獎期間如下，領獎末日如遇假日順延至次一上班日：</p>
                                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table-br" style="margin-bottom: 5px;">
                                    <tr>
                                        <td align="center">
                                            期 別
                                        </td>
                                        <td align="center">
                                            開獎日期
                                        </td>
                                        <td align="center">
                                            領獎期間
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" nowrap="nowrap">
                                            1～2月
                                        </td>
                                        <td align="center">
                                            3/25
                                        </td>
                                        <td align="center">
                                            4/6～7/5
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            3～4月
                                        </td>
                                        <td align="center">
                                            5/25
                                        </td>
                                        <td align="center">
                                            6/6～9/5
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            5～6月
                                        </td>
                                        <td align="center">
                                            7/25
                                        </td>
                                        <td align="center">
                                            8/6～11/5
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            7～8月
                                        </td>
                                        <td align="center">
                                            9/25
                                        </td>
                                        <td align="center">
                                            10/6～1/5
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            9～10月
                                        </td>
                                        <td align="center">
                                            11/25
                                        </td>
                                        <td align="center">
                                            12/6～3/5
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            11～12月
                                        </td>
                                        <td align="center">
                                            1/25
                                        </td>
                                        <td align="center">
                                            2/6～5/5
                                        </td>
                                    </tr>
                                </table>
                                <p>
                                    三、中獎人請於領獎期間內攜帶國民身分證(非本國國籍人士得以護照、居留證等文件替代)及中獎統一發票收執聯，向下列郵局兌領，並應於公告之兌獎領獎時間內為之，逾期則不得領獎。</p>
                                <p class="sublist">
                                    (一)特別獎、特獎、頭獎為各直轄市及各縣、市之指定郵局。</p>
                                <p class="sublist">
                                    (二)二獎、三獎、四獎、五獎及六獎為各地郵局。</p>
                                <p>
                                    四、對獎若有疑義，請洽郵局服務專線<br />
                                    電話：(02)2396-1651。</p>
                            </div>
                        </td>
                        <td align="left" valign="top">
                            <div class="notice">
                                <table style="margin-bottom: 5px;" class="table-br" border="0" cellspacing="0" cellpadding="0"
                                    width="100%">
                                    <tbody>
                                        <tr>
                                            <td colspan="11" align="center">
                                                領 獎 收 據
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="11" align="center">
                                                貼 用 印 花
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%" nowrap="nowrap" align="center">
                                                金 額
                                            </td>
                                            <td colspan="10" align="right">
                                                新台幣<br />
                                                元 整
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%" nowrap="nowrap" align="center">
                                                中 獎 人
                                            </td>
                                            <td style="padding: 0px;" colspan="10" align="center">
                                                <div style="margin: 0px auto; width: 30px; padding-top: 2px; border-right-color: rgb(51, 51, 121);
                                                    border-left-color: rgb(51, 51, 121); border-right-width: 1px; border-left-width: 1px;
                                                    border-right-style: solid; border-left-style: solid;">
                                                    簽名<br />
                                                    或<br />
                                                    蓋章</div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%" nowrap="nowrap" align="center">
                                                國民身分證<br />
                                                統 一 編 號
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                            <td align="center">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%" nowrap="nowrap" align="center">
                                                戶 籍<br />
                                                地 址
                                            </td>
                                            <td colspan="10" align="center">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="35%" nowrap="nowrap" align="center">
                                                電 話
                                            </td>
                                            <td colspan="10" align="center">
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <div class="tel">
                                    檢舉不法逃漏稅，請寫真實姓名地址，寄<br />
                                    營業人所在地稽徵機關。<br />
                                    國稅局全國免費服務專線：<br />
                                    0800-000-321<br />
                                    檢舉貪瀆不法信箱：台北郵政5-75號信箱</div>
                                <div class="urllink">
                                    查詢中獎號碼網址：<br />
                                    http://www.dot.gov.tw</div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
<p style="page-break-after: always">
</p>
<cc1:InvoiceDataSource ID="dsEntity" runat="server" Isolated="true">
</cc1:InvoiceDataSource>
