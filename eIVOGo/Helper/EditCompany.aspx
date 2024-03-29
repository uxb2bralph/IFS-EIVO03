﻿<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagPrefix="uc1" TagName="DataModelCache" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Model.Helper" %>
<%  if (_profile.IsSystemAdmin())
    { %>
<iframe src="<%= VirtualPathUtility.ToAbsolute("~/Helper/RenderContent.aspx?control=~/Module/SAM/EditCompany.ascx") %>" width="100%" height="100%"></iframe>
<%  }
    else
    { %>
<iframe src="<%= VirtualPathUtility.ToAbsolute("~/Helper/RenderContent.aspx?control=~/Module/SAM/EditCounterpartBusiness.ascx") %>" width="100%" height="100%"></iframe>
<%  } %>
<uc1:DataModelCache runat="server" ID="modelItem" KeyName="CompanyID" />
<script runat="server">

    Model.Security.MembershipManagement.UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        int companyID;
        if(Request.GetRequestValue("companyID",out companyID))
        {
            modelItem.DataItem = companyID;
        }
        else
        {
            modelItem.DataItem = (int?)null;
        }
        _profile = Business.Helper.WebPageUtility.UserProfile;
    }
</script>
