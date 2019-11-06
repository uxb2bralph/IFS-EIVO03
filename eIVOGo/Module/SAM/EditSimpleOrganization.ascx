﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditSimpleOrganization.ascx.cs"
    Inherits="eIVOGo.Module.SAM.EditSimpleOrganization" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="../Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc1" %>
<%@ Register Src="../Common/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc2" %>
<%@ Register Src="../Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc3" %>
<div class="border_gray">
    <!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <td colspan="4" class="Head_style_a">營業人基本資料
            </td>
        </tr>
        <tr>
            <th nowrap="nowrap" width="150">
                <span style="color: red">*</span>統一編號
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ReceiptNo" runat="server" Text='<%# _entity.ReceiptNo %>' />
            </td>
            <th width="150">
                <span style="color: red">*</span>名稱
            </th>
            <td class="tdleft">
                <asp:TextBox ID="CompanyName" runat="server" Text='<%# _entity.CompanyName %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>地址
            </th>
            <td colspan="3" class="tdleft">
                <asp:TextBox ID="Addr" Columns="68" runat="server" Text='<%# _entity.Addr %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="Phone" runat="server" Text='<%# _entity.Phone %>' />
            </td>
            <th width="150">傳真
            </th>
            <td class="tdleft">
                <asp:TextBox ID="Fax" runat="server" Text='<%# _entity.Fax %>' />
            </td>
        </tr>
<%--        <tr>
            <th width="150">公司負責人
            </th>
            <td class="tdleft">
                <asp:TextBox ID="UndertakerName" runat="server" Text='<%# _entity.UndertakerName %>' />
            </td>
            <th width="150">
                <span style="color: red">*</span>類別
            </th>
            <td class="tdleft">
                <uc2:EnumSelector ID="CategoryID" runat="server" TypeName="Model.Locale.Naming+MemberCategoryID, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                    SelectorIndication="請選擇" SelectedValue="<%# _entity.OrganizationCategory[0].CategoryID %>" />
            </td>
        </tr>--%>
        <tr>
            <td colspan="4" class="Head_style_a">聯絡方式
            </td>
        </tr>
        <tr>
            <th width="150">聯絡人姓名
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactName" runat="server" Text='<%# _entity.ContactName %>' />
            </td>
            <th width="150">聯絡人職稱
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactTitle" runat="server" Text='<%# _entity.ContactTitle %>' />
            </td>
        </tr>
        <tr>
            <th width="150">聯絡人電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactPhone" runat="server" Text='<%# _entity.ContactPhone %>' />
            </td>
            <th width="150">聯絡人行動電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactMobilePhone" runat="server" Text='<%# _entity.ContactMobilePhone %>' />
                &nbsp;
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>聯絡人電子郵件
            </th>
            <td class="tdleft" colspan="3">
                <asp:TextBox ID="ContactEmail" runat="server" Columns="68" Text='<%# _entity.ContactEmail %>' />
            </td>
        </tr>
        <tr>
            <th style="vertical-align: top;">分店</th>
            <td colspan="3" style="text-align:left">
        <%  if(_entity.OrganizationBranch.Count>0)
            { %>
                    <%  foreach (var item in _entity.OrganizationBranch.OrderBy(b=>b.BranchID))
                        { %>
                    <input type="checkbox" name="deleteID" value="<%= item.BranchID %>" />刪除 
                <div class="border_gray">
                    <input type="hidden" name="BranchID" value="<%= item.BranchID %>" />
                    分店名稱：<input name="BranchName" type="text" placeholder="" value="<%= item.BranchName%>" />
                    店號：<input name="BranchNo" type="text" placeholder="" value="<%= item.BranchNo%>" /><br />
                    電話：<input name="BranchPhone" type="text" placeholder="" value="<%= item.Phone%>" /><br />
                    email：<input name="BranchContactEmail" type="text" size="60" placeholder="" value="<%= item.ContactEmail%>" /><br />
                    地址：<input name="BranchAddr" type="text" size="60" placeholder="" value="<%= item.Addr%>" />
                </div>
                    <%  } %>
        <%  } %>
                    <input type="checkbox" name="addItem" value="true" />新增 
                <div class="border_gray" style="display:none;">
                    分店名稱：<input name="BranchName" type="text" placeholder="" value="" /><br />
                    店號：<input name="BranchNo" type="text" placeholder="" value="" /><br />
                    電話：<input name="BranchPhone" type="text" placeholder="" value="" /><br />
                    email：<input name="BranchContactEmail" type="text" size="60" placeholder="" value="" /><br />
                    地址：<input name="BranchAddr" type="text" size="60" placeholder="" value="" />
                </div>
                <script>
                    $(function () {
                        $('input:checkbox[name="deleteID"]').on('click', function (evt) {
                            var $this = $(this);
                            if ($this.is(':checked')) {
                                $this.next().css('background', 'darkgrey');
                            } else {
                                $this.next().css('background', '');
                            }
                        });
                        $('input:checkbox[name="addItem"]').on('click', function (evt) {
                            var $this = $(this);
                            if ($this.is(':checked')) {
                                $this.next().css('display', 'block');
                            } else {
                                $this.next().css('display', 'none');
                            }
                        });
                    });
                </script>
            </td>
        </tr>
<%--        <tr>
            <td colspan="4" class="Head_style_a">設定
            </td>
        </tr>
        <tr>
            <th width="150">設定使用發票列印
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="SetToPrintInvoice" runat="server" Checked='<%# _entity.OrganizationStatus.SetToPrintInvoice == true  %>'
                    Text="設定使用發票列印" />
            </td>
            <th width="150">發票列印樣式<br />
                折讓單列印樣式
            </th>
            <td class="tdleft">
                <asp:TextBox ID="InvoicePrintView" runat="server" Text='<%# _entity.OrganizationStatus.InvoicePrintView %>'
                    Columns="40" /><br />
                <asp:TextBox ID="AllowancePrintView" runat="server" Text='<%# _entity.OrganizationStatus.AllowancePrintView %>'
                    Columns="40" />
            </td>
        </tr>
        <tr>
            <th width="150">電子發票核准函號
            </th>
            <td class="tdleft" >
                <asp:TextBox ID="AuthorizationNo" runat="server" Text='<%# _entity.OrganizationStatus.AuthorizationNo %>' />
            </td>
            <th>自動核准列印
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="EntrustToPrint" runat="server" Checked='<%# _entity.OrganizationStatus.EntrustToPrint == true  %>'
                    Text="B2B發票買受人可重複列印" />
            </td>
        </tr>
        <tr>
            <th width="150">設定使用委外客服
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="SetToOutsourcingCS" runat="server" Checked='<%# _entity.OrganizationStatus.SetToOutsourcingCS == true  %>'
                    Text="使用委外客服" />
            </td>
            <th>設定簡訊通知
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="SetToNotifyCounterpartBySMS" runat="server" Checked='<%# _entity.OrganizationStatus.SetToNotifyCounterpartBySMS == true  %>'
                    Text="使用簡訊通知買受人" />
            </td>
        </tr>
        <tr>
            <th width="150">設定傳送發票對應資料
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="DownloadDataNumber" runat="server" Checked='<%# _entity.OrganizationStatus.DownloadDataNumber == true  %>'
                    Text="使用傳送企業發票對應資料" />
            </td>
            <th width="150">上期發票空白號碼
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="UploadBranchTrackBlank" runat="server" Checked='<%# _entity.OrganizationStatus.UploadBranchTrackBlank == true  %>'
                    Text="用戶端自動下載" />
            </td>
        </tr>
        <tr>
            <th width="150">設定發票資料全列印
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="PrintAll" runat="server" Checked='<%# _entity.OrganizationStatus.PrintAll == true  %>'
                    Text="是" />
            </td>
            <th width="150">
                <span style="color: red">*</span>設定發票類別
            </th>
            <td class="tdleft">
                <asp:DropDownList ID="InvoiceType" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <th>設定接收B2B發票(PDF檔)
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="SubscribeB2BInvoicePDF" runat="server" Checked='<%# _entity.OrganizationStatus.SubscribeB2BInvoicePDF == true  %>'
                    Text="是" />
            </td>
            <th width="150">設定中獎通知
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="SetWinningNotice" runat="server" Checked='<%# _entity.OrganizationStatus.DisableWinningNotice == false  %>'
                    Text="寄送中獎通知" />
            </td>
        </tr>
        <tr>
            <th>開立發票字軌號碼檢核
            </th>
            <td class="tdleft" colspan="3">
                <asp:CheckBox ID="EnableTrackCodeInvoiceNoValidation" runat="server" Checked='<%# _entity.OrganizationStatus.EnableTrackCodeInvoiceNoValidation == true  %>'
                    Text="是" />
            </td>
        </tr>
        <tr>
            <th >上傳系統格式
            </th>
            <td class="tdleft">
                <select name="uploadType">
                    <option>XML</option>
                    <option>CSV</option>
                    <option>MIG</option>
                    <option>XML(含折讓單)</option>
                </select>
            </td>
            <%  if (_entity != null)
                      { %>
            <th>公司LOGO</th>
            <td>
                <img id="logo" style="width:300px;" src="<%= _entity.LogoURL != null ? VirtualPathUtility.ToAbsolute("~/" + _entity.LogoURL) : null %>" />
                <input type="file" id="fileLogo" style="display:inline;" name="fileLogo" />
            </td>
            <%  } %>
        </tr>

        <tr>
            <th>G/W發票存放資料夾
            </th>
            <td class="tdleft" colspan="3">
                <input type="text" size="68" name="txnPath" value="<%= Request["txnPath"]!=null ? Request["txnPath"] : "C:\\UXB2B_EIVO" %>" />
            </td>
        </tr>
        <tr>
            <th width="150">系統
            </th>
            <td class="tdleft">
                <select name="svcType">
                    <option value="0">測試機</option>
                    <option value="1">正式機</option>
                </select>
            </td>
            <th>B2B發票轉送EIVO05</th>
            <td class="tdleft">
                <asp:CheckBox ID="UseB2BStandalone" runat="server" Checked='<%# _entity.OrganizationStatus.UseB2BStandalone == true  %>'
                    Text="是" /></td>
        </tr>--%>
    </table>
</div>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn" align="center">
            <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CssClass="btn"
                CommandName="Update" Text="確定" />
            &nbsp;
            <input name="Reset" type="reset" class="btn" value="重填" />
            <%--<asp:Button ID="Config_btn" runat="server" CausesValidation="True" CssClass="btn"
                Text="下載設定檔" OnClick="Config_btn_Click" />--%>
<%--            <% if(_entity!=null) { %>
            <input type="button" class="btn" name="btnDownloadConfig" value="下載設定檔" />
            <% } %>--%>
        </td>
    </tr>
</table>
<script>
    <% if(_entity!=null) { %>
    $(function () {
        $('input:button[name="btnDownloadConfig"]').click(function (evt) {
            window.location.href = '<%= VirtualPathUtility.ToAbsolute("~/EIVO/DownloadAppconfig.aspx")%>?id=<%= _entity.CompanyID%>'
                + '&ut=' + $('select[name="uploadType"]').val()
                + '&st=' + $('select[name="svcType"]').val()
                + '&txn=' + encodeURI($('input[name="txnPath"]').val());
        });
    });

    $(function () {

        var fileUpload = $('#fileLogo');
        var elmt = fileUpload.parent();

        fileUpload.off('click').on('change', function () {

            $('<form method="post" id="myForm" enctype="multipart/form-data"></form>')
            .append(fileUpload).ajaxForm({
                url: "<%= VirtualPathUtility.ToAbsolute("~/Organization/UpdateLogo") %>",
                data: { 'id': '<%= _entity.CompanyID %>' },
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    elmt.append(fileUpload);
                    $(data).appendTo($('body')).remove();
                },
                error: function () {
                    elmt.append(fileUpload);
                }
            }).submit();
        });
    });

    <% } %>

    
</script>
<cc1:OrganizationDataSource ID="dsEntity" runat="server">
</cc1:OrganizationDataSource>
<uc1:DataModelCache ID="modelItem" runat="server" KeyName="CompanyID" />
<uc3:ActionHandler ID="doConfirm" runat="server" />
<uc1:DataModelCache ID="modelItem2" runat="server" KeyName="Config" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        UpdateButton.OnClientClick = doConfirm.GetPostBackEventReference(null);
    }
</script>
