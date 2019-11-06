<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnnouncementItem.ascx.cs"
    Inherits="eIVOGo.Module.SYS.Item.AnnouncementItem"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc1" %>
<%@ Register Src="~/Module/Common/CalendarInputDatePicker.ascx" TagName="CalendarInputDatePicker" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Button ID="btnPopup" runat="server" Text="Button" Style="display: none" />
<asp:Panel ID="Panel1" runat="server" Style="display: none; width: 650px; background-color: #ffffdd;
    border-width: 3px; border-style: solid; border-color: Gray; padding: 3px;z-index:100000 !important; ">
    <asp:Panel ID="Panel3" runat="server" Style="cursor: move; background-color: #DDDDDD;
        border: solid 1px Gray; color: Black">
        <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
        <!--路徑名稱-->
        <!--交易畫面標題-->
        <uc1:FunctionTitleBar ID="titleBar" runat="server" ItemName="跑馬燈訊息維護" />
        <!--按鈕-->
        <div class="border_gray">
         <table width="100%" border="0" cellpadding="0" cellspacing="0" id="left_title">
           <tr style="display:none">
            <th width="20%">
               AnnID
            </th>
            <td colspan="2" class="tdleft">
                <asp:TextBox ID="txtAnnID" runat="server" Text="<%# DataItem.AnnID %>" ></asp:TextBox>                
              </td>
        </tr>
        <tr>
            <th width="20%">
                 訊息內容
            </th>
            <td colspan="2" class="tdleft">
                <asp:TextBox ID="txtAnnMessage" runat="server"  TextMode="MultiLine" 
                Width="95%" Text="<%# DataItem.AnnMessage %>" ></asp:TextBox>                
               
            </td>
        </tr>
        <tr>
            <th width="20%">
                起始時間
            </th>
            <td colspan="2" class="tdleft">
                  <uc2:CalendarInputDatePicker ID="DateFrom" runat="server" />
            </td>
        </tr>
         <tr>
            <th width="20%">
                結束時間
            </th>
            <td colspan="2" class="tdleft">
                  <uc2:CalendarInputDatePicker ID="EndDate" runat="server" />
            </td>
        </tr>
          <tr>
            <th width="20%">
                永久顯示
            </th>
            <td colspan="2" class="tdleft">
                 <asp:CheckBox  ID="AlwaysShow" runat="server" Checked="<%# DataItem.AlwaysShow %>" />是
            </td>
        </tr></table>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td class="Bargain_btn" align="center">
          <asp:Button ID="btnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                Text="確定" OnClick="btnUpdate_Click"/>
                            &nbsp;<asp:Button ID="btnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="取消" OnClick="btnCancel_Click"/>
</td>
                </tr>
            </table>
        </div>
        <%-- </ContentTemplate>
        </asp:UpdatePanel>--%>
    </asp:Panel>
</asp:Panel>
<asp:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="btnPopup"
    PopupControlID="Panel1" BackgroundCssClass="modalBackground" DropShadow="true"
    PopupDragHandleControlID="Panel3" />
<cc1:AnnouncementRECDataSource ID="dsEntity" runat="server" Isolated="true">
</cc1:AnnouncementRECDataSource>
