﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Register Src="~/Module/jQuery/EnumSelector.ascx" TagPrefix="uc1" TagName="EnumSelector" %>

<tr id="<%= _trID %>">
    <td><%  Html.RenderPartial("~/Views/BusinessRelationship/Module/GroupMemberSelector.ascx",
                    new InputViewModel
                    {
                        Name = "CompanyID"
                    }); %></td>
    <td>
        <input type="text" name="CompanyName" placeholder="請輸入相對營業人名稱" value="" />
    </td>
    <td style="white-space: nowrap;">
        <input type="text" name="ReceiptNo" placeholder="請輸入統一編號" value="" />
        <a href="javascript:loadCompany();"><i class="fa fa-question-circle" aria-hidden="true"></i></a>
        <script>
            function loadCompany() {
                showLoading();
                $.post('<%= Url.Action("GetCompany", "Home") %>', { 'term': $('#<%= _trID %> input[name="ReceiptNo"]').val() }, function (data) {
                    hideLoading();
                    if ($.isPlainObject(data) && data) {
                        $('#<%= _trID %> input[name="ReceiptNo"]').val(data.ReceiptNo);
                        $('#<%= _trID %> input[name="CompanyName"]').val(data.CompanyName);
                        $('#<%= _trID %> input[name="Addr"]').val(data.Addr);
                        $('#<%= _trID %> input[name="Phone"]').val(data.Phone);
                        $('#<%= _trID %> input[name="ContactEmail"]').val(data.ContactEmail);
                    } 
                });
            }
        </script>
    </td>
    <td>
        <uc1:EnumSelector runat="server" ID="businessSelector" FieldName="BusinessType" TypeName="Model.Locale.Naming+InvoiceCenterBusinessType, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
    </td>
    <td>
        <input type="text" name="ContactEmail" placeholder="請輸入聯絡人電子郵件" value="" /></td>
    <td>
        <input type="text" name="Addr" placeholder="請輸入地址" value="" /></td>
    <td>
        <input type="text" name="Phone" placeholder="請輸入電話" value="" /></td>
    <td>
        <%--<input type="text" name="CustomerNo" placeholder="請輸入店號" value="" />--%>
        新增完相對營業人後，請再從編輯功能維護分店資料。
    </td>
    <td>
        <%--<uc1:EnumSelector runat="server" ID="levelSelector" FieldName="CompanyStatus" TypeName="Model.Locale.Naming+BusinessRelationshipStatus, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />--%>
        <input type="hidden" name="CompanyStatus" value="1103" />
    </td>
    <td>
        <a class="btn" onclick="uiInquireBusiness.commitItem();">新增相對營業人</a>
    </td>
</tr>


<script runat="server">

    ModelSource<InvoiceItem> models;
    String _trID = $"tr{DateTime.Now.Ticks}";

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
    }

</script>
