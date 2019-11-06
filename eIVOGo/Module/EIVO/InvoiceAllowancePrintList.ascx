<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceAllowancePrintList.ascx.cs"
    Inherits="eIVOGo.Module.EIVO.InvoiceAllowancePrintList" %>
<%@ Register Src="../Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Utility" %>
<!--表格 開始-->
<script type="text/javascript">
    $(document).ready(
        function () {
            var chkBox = $("input[id$='chkAll']");
            chkBox.click(
            function () {
                $("#gvEntity INPUT[type='checkbox']")
                .attr('checked', chkBox
                .is(':checked'));
            });

            // To deselect CheckAll when a GridView CheckBox        
            // is unchecked        
            $("#gvEntity INPUT[type='checkbox']").click(
            function (e) {
                if (!$(this)[0].checked) {
                    chkBox.attr("checked", false);
                }
            });
        });
</script>
<asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
    GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" ClientIDMode="Static"
    EnableViewState="False" ShowFooter="True" DataSourceID="dsEntity">
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <input id="chkAll" name="chkAll" type="checkbox" />
            </HeaderTemplate>
            <ItemTemplate>
                <input id="chkItem" name="chkItem" type="checkbox" value='<%# ((InvoiceAllowance)Container.DataItem).AllowanceID  %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="日期">
            <ItemTemplate>
                <%# ValueValidity.ConvertChineseDateString(((InvoiceAllowance)Container.DataItem).AllowanceDate)%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="開立發票營業人">
            <ItemTemplate>
                <%# ((InvoiceAllowance)Container.DataItem).CDS_Document.DocumentOwner.Organization.CompanyName%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="統編">
            <ItemTemplate>
                <%# ((InvoiceAllowance)Container.DataItem).CDS_Document.DocumentOwner.Organization.ReceiptNo%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="折讓號碼">
            <ItemTemplate>
                <asp:LinkButton ID="lbtn" runat="server" Text='<%# ((InvoiceAllowance)Container.DataItem).AllowanceNumber %>'
                    CausesValidation="false" CommandName="Edit" OnClientClick='<%# Page.ClientScript.GetPostBackEventReference(this, String.Format("S:{0}",((InvoiceAllowance)Container.DataItem).InvoiceItem.InvoiceID)) + "; return false;" %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <FooterTemplate>
                共<%# _totalRecordCount %>筆</FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="金額">
            <ItemTemplate>
                <%#String.Format("{0:##,###,###,##0.##}", ((InvoiceAllowance)Container.DataItem).TotalAmount)%></ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <FooterTemplate>
                總計金額：<%# String.Format("{0:##,###,###,##0.##}", _subtotal) %></FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <FooterStyle />
    <EmptyDataTemplate>
        <%# doEmptyDataHandler() %>
    </EmptyDataTemplate>
    <PagerStyle HorizontalAlign="Center" />
    <SelectedRowStyle />
    <HeaderStyle />
    <AlternatingRowStyle CssClass="OldLace" />
    <PagerTemplate>
        <uc2:PagingControl ID="pagingList" runat="server" OnPageIndexChanged="PageIndexChanged" />
    </PagerTemplate>
    <RowStyle />
    <EditRowStyle />
</asp:GridView>
<center>
    <asp:Label ID="lblError" Visible="false" ForeColor="Red" Font-Size="Larger" runat="server"
        Text="查無資料!!" EnableViewState="false"></asp:Label>
</center>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn">
            <asp:Button ID="btnShow" runat="server" Text="列印折讓單" onclick="btnShow_Click" />
        </td>
    </tr>
</table>
<cc1:AllowanceDataSource ID="dsEntity" runat="server">
</cc1:AllowanceDataSource>
