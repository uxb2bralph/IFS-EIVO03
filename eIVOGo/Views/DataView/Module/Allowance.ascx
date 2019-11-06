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

<div>
    <%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "發票折讓證明"); %>
    <div class="border_gray">
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
            <tbody>
                <tr>
                    <th width="100" nowrap="nowrap">折讓日期</th>
                    <td class="tdleft"><%= _model.AllowanceDate.Value.Year-1911 %>年<%= _model.AllowanceDate.Value.Month %>月<%= _model.AllowanceDate.Value.Day %>日
                    </td>
                </tr>
            </tbody>
        </table>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
            <tbody>
                <tr>
                    <th width="100" colspan="4" class="Head_style_a">原開立銷貨發票單位</th>
                </tr>
                <tr>
                    <th width="100" nowrap="nowrap">統一編號</th>
                    <td width="30%" class="tdleft"><%= _model.InvoiceAllowanceSeller.ReceiptNo %>
                    </td>
                    <th width="100" nowrap="nowrap">名　　稱</th>
                    <td class="tdleft"><%= _model.InvoiceAllowanceSeller.CustomerName %>
                    </td>
                </tr>
                <tr>
                    <th width="100">營業所在地址</th>
                    <td colspan="3" class="tdleft"><%= _model.InvoiceAllowanceSeller.Address %>
                    </td>
                </tr>
            </tbody>
        </table>
        <br>
        <div>
            <table class="table01" cellspacing="0" cellpadding="0" id="gvEntity" style="width: 100%; border-collapse: collapse;">
                <tbody>
                    <tr>
                        <td align="center" colspan="6" style="color: #FFFFFF; background-color: #C99040;">開立發票</td>
                        <td align="center" colspan="5" style="color: #FFFFFF; background-color: #C99040;">退貨或折讓內容</td>
                    </tr>
                    <tr>
                        <th style="white-space:nowrap;">聯式</th>
                        <th style="white-space:nowrap;">年</th>
                        <th style="white-space:nowrap;">月</th>
                        <th style="white-space:nowrap;">日</th>
                        <th style="white-space:nowrap;">字軌</th>
                        <th style="white-space:nowrap;">號碼</th>
                        <th style="white-space:nowrap;">品名</th>
                        <th style="white-space:nowrap;">數量</th>
                        <th style="white-space:nowrap;">單價</th>
                        <th style="white-space:nowrap;">金額<br/>
                            (不含稅之進貨額)</th>
                        <th style="white-space:nowrap;">營業稅額</th>
                    </tr>
                    <%  foreach (var d in _model.InvoiceAllowanceDetails)
                        {
                            var item = d.InvoiceAllowanceItem;  %>
                    <tr>
                        <td></td>
                        <td><%= item.InvoiceDate.Value.Year%></td>
                        <td><%= item.InvoiceDate.Value.Month%></td>
                        <td><%= item.InvoiceDate.Value.Day%></td>
                        <td><%= item.InvoiceNo?.Substring(0,2)%></td>
                        <td><%= item.InvoiceNo?.Substring(2)%></td>
                        <td><%= item.OriginalDescription%></td>
                        <td style="text-align:right;"><%= $"{item.Piece:.}"%></td>
                        <td style="text-align:right;"><%= String.Format("{0:##,###,###,##0.##}", item.UnitCost) %></td>
                        <td style="text-align:right;"><%= String.Format("{0:##,###,###,##0.##}", item.Amount)%></td>
                        <td style="text-align:right;"><%= String.Format("{0:##,###,###,##0.##}", item.Tax) %></td>
                    </tr>
                    <%  } %>
                </tbody>
            </table>
        </div>
        <!--表格 結束-->
    </div>
</div>

<script runat="server">

    ModelSource<InvoiceItem> models;
    InvoiceAllowance _model;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceAllowance)this.Model;
    }




</script>
