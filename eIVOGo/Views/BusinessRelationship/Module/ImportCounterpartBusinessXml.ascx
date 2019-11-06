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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "相對營業人管理-新增或編輯相對營業人資料"); %>
<div id="queryArea" class="border_gray">
    <!--表格 開始-->
    <table class="left_title" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr class="other">
                <th width="20%" nowrap>
                    匯入檔案範本
                </th>
                <td class="tdleft">
                    <a href="<%= Url.Action("GetCounterpartBusinessSample","BusinessRelationship") %>">
                        <img border="0" src="../images/icon_ca.gif" width="27" height="28" /></a> 
                    <font color="blue">請依據檔案中各欄位名稱填入相對應內容，每一列代表唯一家相對營業人資料，若匯入資料已存在系統，系統會以編輯方式修改原存在資料</font>
                </td>
            </tr>
            <tr class="other">
                <th width="20%" nowrap>
                    所屬集團成員
                </th>
                <td class="tdleft">
                    <select name="CompanyID">
                        <%  if (_profile.IsSystemAdmin())
                            {   %>
                        <option value="">請選擇...</option>
                        <%  } %>
                    <%  Html.RenderPartial("~/Views/BusinessRelationship/Module/GroupMemberOptions.ascx");   %>
                    </select>
                </td>
            </tr>
            <tr class="other">
                <th width="20%" nowrap>
                    相對營業人類別
                </th>
                <td class="tdleft">
                    <select name="BusinessType">
                        <option value="<%= (int)Naming.InvoiceCenterBusinessType.銷項 %>"><%= Naming.InvoiceCenterBusinessType.銷項 %></option>
                        <option value="<%= (int)Naming.InvoiceCenterBusinessType.進項 %>"><%= Naming.InvoiceCenterBusinessType.進項 %></option>
                    </select>
                </td>
            </tr>
            <tr class="other">
                <th width="20%" nowrap>
                    相對營業人資料匯入
                </th>
                <td class="tdleft">
                    <input type="file" id="theFile" name="theFile" style="display:none;"/>
                    <button type="button" name="btnUpload" class="btn">傳送檔案</button>
                </td>
            </tr>
        </tbody>
    </table>
    <!--表格 結束-->
</div>
<script>
    $(function () {
        var $file = $('#theFile');
        var $btn = $('button[name="btnUpload"]');

        $btn.click(function () {
            $file.val('');
            $file.click();
        });

        $file.on('change', function (event) {
            clearErrors();
            $('.tblAction').remove();
            uploadFile($file, $('#queryArea input,select,textarea').serializeObject(),
                '<%= Url.Action("ImportXml","BusinessRelationship") %>',
                function (data) {
                    $btn.before($file);
                    $('#queryArea').after($(data));
                },
                function () {
                    $btn.before($file);
                })
        });
    });
</script>
<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    Model.Security.MembershipManagement.UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _profile = Business.Helper.WebPageUtility.UserProfile;

    }

</script>