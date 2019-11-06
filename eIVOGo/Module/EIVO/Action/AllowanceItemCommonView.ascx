<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AllowanceItemCommonView.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.Action.AllowanceItemCommonView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc5" %>
<%@ Register assembly="Model" namespace="Model.DataEntity" tagprefix="cc1" %>
<%@ Import Namespace="Utility" %>
<%@ Register src="~/Module/EIVO/Action/AllowanceProductItemList.ascx" tagname="AllowanceProductItemList" tagprefix="uc3" %>
<%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
<!--路徑名稱-->
<!--交易畫面標題-->
<!--按鈕-->
<asp:Button ID="btnPopup" runat="server" Text="Button" Style="display: none" />
<asp:Panel ID="Panel1" runat="server" Style="display: none; width: 750px; background-color: #ffffdd;
    border-width: 3px; border-style: solid; border-color: Gray; padding: 3px;">
    <asp:Panel ID="Panel3" runat="server" Style="cursor: move; background-color: #DDDDDD;
        border: solid 1px Gray; color: Black">
        <%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        --%>
        <!--路徑名稱-->
        <!--交易畫面標題-->
                <uc1:FunctionTitleBar ID="FunctionTitleBar2" runat="server" ItemName="折讓" />
                <div id="border_gray">
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title" >
                    <tr>
                        <th width="100" nowrap="nowrap">折讓日期</th>
                        <td class="tdleft"><%# ValueValidity.ConvertChineseDateString(_entity.AllowanceDate)  %></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
                    <tr>
                        <th width="100" colspan="4" class="Head_style_a">原開立銷貨發票單位</th>
                    </tr>
                    <tr>
                        <th width="100" nowrap="nowrap">統一編號</th>
                        <td width="30%" class="tdleft"><%#_entity.InvoiceAllowanceSeller.Organization.ReceiptNo%></td>
                        <th width="100" nowrap="nowrap">名　　稱</th>
                        <td class="tdleft"><%#_entity.InvoiceAllowanceSeller.Organization.CompanyName%></td>
                    </tr>
                    <tr>
                        <th width="100">營業所在地址</th>
                        <td colspan="3" class="tdleft"><%#_entity.InvoiceAllowanceSeller.Organization.Addr%></td>
                    </tr>
                </table>
                <br />
                <uc3:AllowanceProductItemList ID="AllowanceProductItems" runat="server" />
        </div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td class="Bargain_btn" align="center">
                    <asp:Button ID="btnCancel" runat="server" Text="關閉" />
                    
                </td>
            </tr>
        </table>
        <%--            </ContentTemplate>
        </asp:UpdatePanel>
        --%>
    </asp:Panel>
</asp:Panel>
<asp:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="btnPopup"
    CancelControlID="btnCancel" PopupControlID="Panel1" BackgroundCssClass="modalBackground"
    DropShadow="true" PopupDragHandleControlID="Panel3" />
<uc5:ActionHandler ID="doCancel" runat="server" />
<uc5:ActionHandler ID="doConfirm" runat="server" />
<cc1:AllowanceDataSource ID="dsEntity" runat="server">
</cc1:AllowanceDataSource>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        btnCancel.OnClientClick = doCancel.GetPostBackEventReference(null);
    }
</script>
