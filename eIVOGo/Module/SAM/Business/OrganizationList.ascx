<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationList.ascx.cs"
    Inherits="eIVOGo.Module.SAM.Business.OrganizationList" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Ajax/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Module/SAM/Business/EnterpriseGroupSelector.ascx" TagPrefix="uc1" TagName="EnterpriseGroupSelector" %>


<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True" DataKeyNames="CompanyID" ItemType="Model.DataEntity.Organization">
        <Columns>
            <asp:TemplateField HeaderText="企業名稱" SortExpression="CompanyName">
                <ItemTemplate>
                    <%# Item.CompanyName %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="統一編號" SortExpression="ReceiptNo">
                <ItemTemplate>
                    <%# Item.ReceiptNo  %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="負責人姓名" SortExpression="UndertakerName">
                <ItemTemplate>
                    <%# Item.UndertakerName%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="電子郵件" SortExpression="ContactEmail">
                <ItemTemplate>
                    <%# Item.ContactEmail%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" HeaderText="管理">
                <ItemTemplate>
                    <input type="button" onclick="resetGroup(<%# Item.CompanyID%>,<%# Item.EnterpriseGroupMember.Count>0 ? 1 : 0 %>);" value="<%# Item.EnterpriseGroupMember.Count>0 ? "停用集團成員" : "啟用集團成員" %>" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
        <FooterStyle CssClass="total-count" />
        <PagerStyle HorizontalAlign="Center" />
        <SelectedRowStyle />
        <HeaderStyle />
        <AlternatingRowStyle CssClass="OldLace" />
        <PagerTemplate>
        </PagerTemplate>
        <RowStyle />
        <EditRowStyle />
    </asp:GridView>
    <uc1:PagingControl runat="server" ID="pagingList" />
</div>
<div id="groups" style="display:none">
    請選擇電子發票加值中心：<br />
    <uc1:EnterpriseGroupSelector runat="server" ID="EnterpriseGroupSelector" />
</div>
<cc1:OrganizationDataSource ID="dsEntity" runat="server">
</cc1:OrganizationDataSource>
<script>
    function resetGroup(keyValue,status) {
        var $this = $(event.target);
        if(confirm('確定'+$this.val()+'?')) {
            if(status==1) {
                $.post('<%= VirtualPathUtility.ToAbsolute("~/SAM/Helper/ResetGroup.ashx")%>', {'keyValue' : keyValue}, function (data) {
                    $('<div>' + data.message + '</div>').dialog(/*{ width: 640 }*/);
                    $this.val('已'+$this.val());
                    $this.prop('disabled',true);
                });
            } else {
                $('#groups').dialog({
                    buttons: [{
                        text: "確定",
                        icons: {
                            primary: "ui-icon-heart"
                        },
                        click: function() {
                            $( this ).dialog( "close" );
                            $.post('<%= VirtualPathUtility.ToAbsolute("~/SAM/Helper/ResetGroup.ashx")%>', 
                                {
                                    'keyValue' : keyValue,
                                    'enterprise' : $('#groups [name$="selector"]').val()
                                }, 
                                function (data) {
                                    $('<div>' + data.message + '</div>').dialog(/*{ width: 640 }*/);
                                    $this.val('已'+$this.val());
                                    $this.prop('disabled',true);
                            });
                        }
                    },
                    {
                        text: "取消",
                        icons: {
                            primary: "ui-icon-heart"
                        },
                        click: function() {
                            $( this ).dialog( "close" );
                        }
                    }]
                });
            }
        }
    }

</script>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.Load += module_sam_business_organizationlist_ascx_Load;
        this.PreRender += module_sam_business_organizationlist_ascx_PreRender;
    }

    void module_sam_business_organizationlist_ascx_Load(object sender, EventArgs e)
    {
    }

    void module_sam_business_organizationlist_ascx_PreRender(object sender, EventArgs e)
    {
        pagingList.RecordCount = this.Select().Count();
        gvEntity.PageIndex = pagingList.CurrentPageIndex;
    }

    protected override void gvEntity_DataBound(object sender, EventArgs e)
    {
        
    }
</script>