<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentTypeFlowItem.ascx.cs"
    Inherits="eIVOGo.Module.Flow.DocumentTypeFlowItem" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc4" %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagName="DataModelCache" TagPrefix="uc2" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc5" %>
<%@ Register Assembly="Model" Namespace="Model.DocumentFlowManagement" TagPrefix="cc1" %>
<%@ Register Src="DocumentTypeFlowCandidateSelector.ascx" TagName="DocumentTypeFlowCandidateSelector"
    TagPrefix="uc3" %>
<%@ Register Src="DocumentFlowSelector.ascx" TagName="DocumentFlowSelector" TagPrefix="uc6" %>
<%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
<!--路徑名稱-->
<!--交易畫面標題-->
<!--按鈕-->
<asp:Button ID="btnPopup" runat="server" Text="Button" Style="display: none" />
<asp:Panel ID="Panel1" runat="server" Style="display: none; width: 650px; background-color: #ffffdd;
    border-width: 3px; border-style: solid; border-color: Gray; padding: 3px;">
    <asp:Panel ID="Panel3" runat="server" Style="cursor: move; background-color: #DDDDDD;
        border: solid 1px Gray; color: Black">
        <%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        --%>
        <!--路徑名稱-->
        <!--交易畫面標題-->
        <uc1:FunctionTitleBar ID="titleBar" runat="server" ItemName="修改發票號碼" />
        <!--按鈕-->
        <div class="border_gray" id="holder" runat="server">
            <table class="left_title" border="0" cellspacing="0" cellpadding="0" width="100%">
                <tbody>
                    <tr>
                        <th class="Head_style_a" colspan="2">
                            套用文件處理流程
                        </th>
                    </tr>
                    <tr>
                        <th nowrap>
                            <span class="red">*</span> 文件類型
                        </th>
                        <td class="tdleft">
                            <uc4:EnumSelector ID="TypeID" runat="server" TypeName="Model.Locale.Naming+B2BInvoiceDocumentTypeDefinition, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                                SelectorIndication="請選擇" />
                        </td>
                    </tr>
                    <tr>
                        <th nowrap>
                            <span class="red">*</span> 發票屬性
                        </th>
                        <td class="tdleft">
                            <uc4:EnumSelector ID="BusinessID" runat="server" TypeName="Model.Locale.Naming+InvoiceCenterBusinessType, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                                SelectorIndication="請選擇" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span class="red">*</span> 所屬公司
                        </th>
                        <td class="tdleft">
                            <uc3:DocumentTypeFlowCandidateSelector ID="CompanyID" SelectorIndication="請選擇" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span class="red">*</span> 適用流程
                        </th>
                        <td class="tdleft">
                            <uc6:DocumentFlowSelector ID="FlowID" runat="server" SelectorIndication="請選擇" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td class="Bargain_btn" align="center">
                    <asp:Button ID="btnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                        Text="確定" />
                    &nbsp;
                    <input type="reset" value="重填" class="btn" />
                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="關閉" />
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
<cc1:DocumentTypeFlowDataSource ID="dsEntity" runat="server">
</cc1:DocumentTypeFlowDataSource>
<uc2:DataModelCache ID="modelItem" runat="server" KeyName="DocumentTypeFlow" />
<uc5:ActionHandler ID="doConfirm" runat="server" />
<uc5:ActionHandler ID="doCancel" runat="server" />
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        btnUpdate.OnClientClick = doConfirm.GetPostBackEventReference(null);
        btnCancel.OnClientClick = doCancel.GetPostBackEventReference(null);
    }
</script>
