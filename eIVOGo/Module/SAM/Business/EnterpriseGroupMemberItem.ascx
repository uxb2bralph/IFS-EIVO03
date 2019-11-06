<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseGroupMemberItem.ascx.cs"
    Inherits="eIVOGo.Module.SAM.Business.EnterpriseGroupMemberItem" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="EnterpriseGroupSelector.ascx" TagName="EnterpriseGroupSelector"
    TagPrefix="uc3" %>
<%@ Register src="~/Module/Common/ActionHandler.ascx" tagname="ActionHandler" tagprefix="uc5" %>
<%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
<!--路徑名稱-->
<!--交易畫面標題-->
<!--按鈕-->
<div class="border_gray">
    <!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <td colspan="4" class="Head_style_a">
                企業基本資料
            </td>
        </tr>
        <tr>
            <th nowrap="nowrap" width="150">
                <span style="color: red">*</span>集團加值中心
            </th>
            <td class="tdleft" colspan="3">
            <%# _entity!=null?_entity.EnterpriseGroup.EnterpriseName:""%>
                <uc3:EnterpriseGroupSelector ID="EnterpriseID" runat="server" />
            </td>
        </tr>
        <tr>
            <th nowrap="nowrap" width="150">
                <span style="color: red">*</span>公司統一編號
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ReceiptNo" runat="server" Text='<%# _entity.Organization.ReceiptNo %>' />
            </td>
            <th width="150">
                <span style="color: red">*</span>名稱
            </th>
            <td class="tdleft">
                <asp:TextBox ID="CompanyName" runat="server" Text='<%# _entity.Organization.CompanyName %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>地址
            </th>
            <td colspan="3" class="tdleft">
                <asp:TextBox ID="Addr" Columns="68" runat="server" Text='<%# _entity.Organization.Addr %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="Phone" runat="server" Text='<%# _entity.Organization.Phone %>' />
            </td>
            <th width="150">
                傳真
            </th>
            <td class="tdleft">
                <asp:TextBox ID="Fax" runat="server" Text='<%# _entity.Organization.Fax %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                公司負責人
            </th>
            <td class="tdleft" colspan="3">
                <asp:TextBox ID="UndertakerName" runat="server" Text='<%# _entity.Organization.UndertakerName %>' />
            </td>
        </tr>
        <tr>
            <td colspan="4" class="Head_style_a">
                聯絡方式
            </td>
        </tr>
        <tr>
            <th width="150">
                聯絡人姓名
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactName" runat="server" Text='<%# _entity.Organization.ContactName %>' />
            </td>
            <th width="150">
                聯絡人職稱
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactTitle" runat="server" Text='<%# _entity.Organization.ContactTitle %>' />
            </td>
        </tr>
        <tr>
            <th width="150">
                聯絡人電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactPhone" runat="server" Text='<%# _entity.Organization.ContactPhone %>' />
            </td>
            <th width="150">
                聯絡人行動電話
            </th>
            <td class="tdleft">
                <asp:TextBox ID="ContactMobilePhone" runat="server" Text='<%# _entity.Organization.ContactMobilePhone %>' />
                &nbsp;
            </td>
        </tr>
        <tr>
            <th width="150">
                <span style="color: red">*</span>聯絡人電子郵件
            </th>
            <td colspan="3" class="tdleft">
                <asp:TextBox ID="ContactEmail" runat="server" Columns="68" Text='<%# _entity.Organization.ContactEmail %>' />
            </td>
        </tr>
        <tr>
                    <th width="150">
                        設定使用發票列印
                    </th>
                    <td class="tdleft">
                        <asp:CheckBox ID="SetToPrintInvoice" runat="server" Checked='<%# _entity.Organization.OrganizationStatus.SetToPrintInvoice == true %>'
                            Text="設定使用發票列印" />
                    </td>
                    <th width="150">
                        發票列印樣式
                    </th>
                    <td class="tdleft">
                        <asp:TextBox ID="InvoicePrintView" runat="server" 
                            Text='<%# _entity.Organization.OrganizationStatus.InvoicePrintView %>' Columns="40" />
                    </td>
                </tr>
        <tr>
            <th width="150">
                開立電子發票格式
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="IronSteelIndustry" runat="server" Checked='<%# _entity.Organization.OrganizationStatus.IronSteelIndustry == true %>'
                            Text="鋼鐵工業" />
            </td>
            <th width="150">
                發票作業模式
            </th>
            <td class="tdleft">
                <asp:CheckBox ID="Entrusting" runat="server" Checked='<%# _entity.Organization.OrganizationStatus.Entrusting == true %>'
                            Text="自動開立＼接收" />
            </td>
        </tr>

    </table>
</div>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn" align="center">
            <asp:Button ID="btnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                Text="確定" />
            &nbsp;
            <input type="reset" value="重填" class="btn" />
        </td>
    </tr>
</table>
<cc1:EnterpriseGroupMemberDataSource ID="dsEntity" runat="server">
</cc1:EnterpriseGroupMemberDataSource>
<uc2:DataModelCache ID="modelItem" runat="server" KeyName="CompanyID" />
<uc5:ActionHandler ID="doConfirm" runat="server" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        btnUpdate.OnClientClick = doConfirm.GetPostBackEventReference(null);
    }
</script>
