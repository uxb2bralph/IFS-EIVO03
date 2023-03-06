﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>

    <table id="tblAction" width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td >
                <button type="button" class="btn" name="btnVoid" onclick="uiInvoiceQuery.desireToVoid(true);">核准</button>&nbsp;&nbsp;
                <button type="button" class="btn" name="btnVoidCancellation" onclick="uiInvoiceQuery.desireToVoid(false);">退回</button>
            </td>
        </tr>
    </table>


<script runat="server">


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

</script>

