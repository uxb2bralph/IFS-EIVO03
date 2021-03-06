﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.SAM.EditCompany" %>
<%@ Register Src="../UI/RegisterMessage.ascx" TagName="RegisterMessage" TagPrefix="uc2" %>
<%@ Register Src="../UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc3" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register src="EditSimpleOrganization.ascx" tagname="EditOrganization" tagprefix="uc1" %>
<%@ Register src="../Common/DataModelCache.ascx" tagname="DataModelCache" tagprefix="uc4" %>

<%@ Import Namespace="Uxnet.Web.WebUI" %>
<!--路徑名稱-->


<asp:UpdatePanel ID="Updatepanel1" runat="server">
    <ContentTemplate>
        <div id="mainpage" runat="server">
            <uc3:PageAction ID="actionItem" runat="server" ItemName="修改相對營業人資料" />
            <!--交易畫面標題-->
            <uc1:EditOrganization ID="EditItem" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += new EventHandler(module_sam_editcompany_ascx_PreRender);
    }

    void module_sam_editcompany_ascx_PreRender(object sender, EventArgs e)
    {
        EditItem.BindData();
    }

    protected override void EditItem_Done(object sender, EventArgs e)
    {
        this.AjaxAlert("修改完成!!");
    }

</script>


