<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.EIVO.Item.AllowanceBalanceView" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Model.DataEntity" %>
<table border="0" cellspacing="0" cellpadding="0" width="100%">
    <tbody>
        <tr>
            <td width="50%">
                <table class="table-bk" border="0" cellspacing="0" cellpadding="0" width="100%">
                    <tbody>
                        <tr>
                            <td rowspan="3" width="40">
                                <table style="font-weight: 700" class="noborder">
                                    <tr>
                                        <td rowspan="4" valign="top">
                                            原<br />
                                            開<br />
                                            立<br />
                                            銷<br />
                                            貨
                                        </td>
                                        <td>
                                            發
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            票
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            單
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            位
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td width="80" align="center">
                                營利事業<br />
                                統一編號
                            </td>
                            <td>
                                <span class="contant-m">
                                    <%# _item.InvoiceAllowanceSeller.ReceiptNo %></span>
                            </td>
                        </tr>
                        <tr>
                            <td width="80" align="center">
                                名 稱
                            </td>
                            <td>
                                <span class="contant-m">
                                    <%# _item.InvoiceAllowanceSeller.CustomerName %></span>
                            </td>
                        </tr>
                        <tr>
                            <td width="80" align="center">
                                營業所在<br />
                                地 址
                            </td>
                            <td>
                                <span class="contant-m">
                                    <%# _item.InvoiceAllowanceSeller.Address %></span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
            <td width="50%" align="center">
                <table border="0" cellspacing="0" cellpadding="0" width="379" height="111">
                    <!-- MSTableType="layout" -->
                    <tbody>
                        <tr>
                            <td height="70" align="center">
                                <table style="font-weight: 700; font-size: x-large">
                                    <tr>
                                        <td rowspan="2">
                                            營業人
                                        </td>
                                        <td>
                                            銷貨退回
                                        </td>
                                        <td rowspan="2">
                                            或折讓證明單
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            進貨退出
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="title3" width="70%" align="center">
                                中華民國 ：<%# _item.AllowanceDate.Value.Year-1911 %>年<%# String.Format("{0:00}", _item.AllowanceDate.Value.Month)%>月<%# String.Format("{0:00}", _item.AllowanceDate.Value.Day)%>日
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<table class="table-bk" border="0" cellspacing="0" cellpadding="0" width="100%">
    <tbody>
        <tr>
            <th colspan="6" align="center">
                <span class="contant-m">開 立 發 票</span>
            </th>
            <th colspan="5" align="center">
                <span class="contant-m">退 貨 或 折 讓 內 容</span>
            </th>
            <th colspan="3" align="center">
                <span class="contant-m">課稅別(V)</span>
            </th>
        </tr>
        <tr>
            <th align="center">
                聯<br />
                式
            </th>
            <th align="center">
                年
            </th>
            <th align="center">
                月
            </th>
            <th align="center">
                日
            </th>
            <th align="center">
                字<br />
                軌
            </th>
            <th align="center">
                號 碼
            </th>
            <th align="center">
                品 名
            </th>
            <th align="center">
                數 量
            </th>
            <th align="center">
                單 價
            </th>
            <th nowrap align="center">
                金 額<br />
                (不含稅之進貨額)
            </th>
            <th nowrap align="center">
                營業稅額
            </th>
            <th align="center">
                應<br />
                <br />
                稅
            </th>
            <th align="center">
                零<br />
                稅<br />
                率
            </th>
            <th align="center">
                免<br />
                <br />
                稅
            </th>
        </tr>
        <asp:Repeater ID="rpList" runat="server" EnableViewState="false">
            <ItemTemplate>
                <tr>
                    <td align="center">
                        三
                    </td>
                    <td align="center">
                        <%# ((InvoiceAllowanceItem)Container.DataItem).InvoiceDate.Value.Year-1911 %>
                    </td>
                    <td align="center">
                        <%# String.Format("{0:00}",((InvoiceAllowanceItem)Container.DataItem).InvoiceDate.Value.Month) %>
                    </td>
                    <td align="center">
                        <%# String.Format("{0:00}", ((InvoiceAllowanceItem)Container.DataItem).InvoiceDate.Value.Day) %>
                    </td>
                    <td align="center">
                        <%# ((InvoiceAllowanceItem)Container.DataItem).InvoiceNo.Substring(0,2) %>
                    </td>
                    <td>
                        <%# ((InvoiceAllowanceItem)Container.DataItem).InvoiceNo.Substring(2) %>
                    </td>
                    <td>
                        <%# ((InvoiceAllowanceItem)Container.DataItem).OriginalDescription %>
                    </td>
                    <td align="right">
                        <%# String.Format("{0:##,###,###,##0.##}", ((InvoiceAllowanceItem)Container.DataItem).Piece) %>
                    </td>
                    <td align="right">
                        <%# String.Format("{0:##,###,###,##0.##}", ((InvoiceAllowanceItem)Container.DataItem).UnitCost) %>
                    </td>
                    <td align="right">
                        <%# String.Format("{0:##,###,###,##0.##}", ((InvoiceAllowanceItem)Container.DataItem).Amount) %>
                    </td>
                    <td align="right">
                        <%# String.Format("{0:##,###,###,##0.##}", ((InvoiceAllowanceItem)Container.DataItem).Tax) %>
                    </td>
                    <td align="center">
                        <span class="contant">
                            <%# ((InvoiceAllowanceItem)Container.DataItem).TaxType == (byte)1 ? "V" : "&nbsp;"%></span>
                    </td>
                    <td align="center">
                        <span class="contant">
                            <%# ((InvoiceAllowanceItem)Container.DataItem).TaxType == (byte)2 ? "V" : "&nbsp;"%></span>
                    </td>
                    <td align="center">
                        <span class="contant">
                            <%# ((InvoiceAllowanceItem)Container.DataItem).TaxType == (byte)3 ? "V" : "&nbsp;"%></span>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rpBlank" runat="server" EnableViewState="false">
            <ItemTemplate>
                <tr>
                    <td align="center">&nbsp;
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                    <td>
                    </td>
                    <td align="right">
                    </td>
                    <td align="right">
                    </td>
                    <td align="right">
                    </td>
                    <td align="right">
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                    <td align="center">
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td colspan="9" align="center">
                <span class="contant-m">合 計</span>
            </td>
            <td align="right">
                <%# String.Format("{0:##,###,###,##0.##}", _item.TotalAmount) %>
            </td>
            <td align="right">
                <%# String.Format("{0:##,###,###,##0.##}", _item.TaxAmount) %>
            </td>
            <td colspan="3" align="center">
            </td>
        </tr>
    </tbody>
</table>
<table class="tablenone" border="0" cellspacing="0" cellpadding="4" width="100%">
    <tbody>
        <tr>
            <td class="contant" valign="top" width="80%">
                <br />
                第三聯：由進貨人作為進項稅額之扣減憑證。<br />
                本證明單所列進貨退出或折讓，確屬事實，特此證明。<br />
                原進貨營業人（或原買受人）名稱：<%# _item.InvoiceAllowanceBuyer.CustomerName %><br />
                營利事業統一編號：<%# _item.InvoiceAllowanceBuyer.ReceiptNo %><br />
                地址：<%# _item.InvoiceAllowanceBuyer.Address %>
            </td>
            <td valign="top" align="center">
                <table class="table-bk" border="0" cellspacing="0" cellpadding="4" width="100%">
                    <tbody>
                        <tr>
                            <td nowrap>
                                營業人蓋用統一發票專用章
                            </td>
                        </tr>
                        <tr>
                            <td height="100" align="center">
                                <div class="eivo_stamp">
                                    <%# _item.InvoiceAllowanceBuyer.CustomerName %><br/>
                                    統一編號<br/>
                                    <div class="notitle">
                                        <%# _item.InvoiceAllowanceBuyer.ReceiptNo %></div>
                                    電話:<%# _item.InvoiceAllowanceBuyer.Phone %><br />
                                    <%# _item.InvoiceAllowanceBuyer.Address %><br/>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        rpList.DataBinding += new EventHandler(rpList_DataBinding);
    }

    void rpList_DataBinding(object sender, EventArgs e)
    {
        var count = _item.InvoiceAllowanceDetails.Count;
        if (count < 10)
        {
            rpBlank.DataSource = ((int)0).GenerateArray(10 - count);
            rpBlank.DataBind();
        }
    }
</script>
