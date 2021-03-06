﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="B2CAllowancePrintView.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.B2CAllowancePrintView" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>

<% if (_item != null)
    {
        var details = _item.InvoiceAllowanceDetails.Select(d => d.InvoiceAllowanceItem).First();
            %>
<div style="page-break-after: always; width: 5.5cm; margin-left: 0cm; margin-right: 0cm">
    <div class="container" style="border-top: 0px; border-bottom: 0px; font-size:small;">
        <p style="text-align:center">營業人銷貨退回、進貨退出或<Br />折讓證明單證明聯</p>
        <p style="text-align:center"><%= String.Format("{0:yyyy-MM-dd}", _item.AllowanceDate) %></p>
        <p>賣方統編：<%= _item.InvoiceAllowanceSeller.ReceiptNo %><br />
           賣方名稱：<%= _item.InvoiceAllowanceSeller.CustomerName %><br />
           發票開立日期：<%= String.Format("{0:yyyy-MM-dd}", details.InvoiceDate) %><br />
            <%= details.InvoiceNo.Substring(0,2) %>-<%= details.InvoiceNo.Substring(2) %><br />
           買方統編：<%= !_item.InvoiceAllowanceBuyer.IsB2C() ? _item.InvoiceAllowanceBuyer.ReceiptNo : null %><br />
           買方名稱：<%= !_item.InvoiceAllowanceBuyer.IsB2C() ? _item.InvoiceAllowanceBuyer.CustomerName : null %><br />
        </p>
        <p></p>
        <p>
        <%  
            var items = _item.InvoiceAllowanceDetails.Select(d => d.InvoiceAllowanceItem);
            foreach (var d in items)
            { %>
            品名：<%= d.OriginalDescription %><br />
            數量：<%= String.Format("{0:##,###,###,##0.##}", d.Piece) %><br />
            單價：<%= String.Format("{0:##,###,###,##0.##}", d.UnitCost) %><br />
            金額(不含稅)：<%= String.Format("{0:##,###,###,##0.##}", d.Amount) %><br />
            課稅別：<%= (d.TaxType == (byte)2 || d.TaxType == (byte)3) ? "TZ" : "TX"%>
        <%  } %>
        </p>
        <p>
            營業稅額合計：<%= String.Format("{0:##,###,###,##0.##}", _item.TaxAmount) %><br />
            金額(不含稅)合計：<%= String.Format("{0:##,###,###,##0.##}", _item.TotalAmount) %><br />
        </p>
        <p></p>
        <p>
            簽收人：
        </p>
    </div>
</div>
<%      
    } %>
<cc1:InvoiceDataSource ID="dsEntity" runat="server">
</cc1:InvoiceDataSource>

