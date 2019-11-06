<%@ WebHandler Language="C#" Class="eIVOCenter.SAM.Helper.ResetGroup" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Model.DataEntity;
using Model.InvoiceManagement;
using Utility;

namespace eIVOCenter.SAM.Helper
{
    /// <summary>
    /// Summary description for ResetGroup
    /// </summary>
    public class ResetGroup : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var Request = context.Request;
            var Response = context.Response;

            Response.ContentType = "application/json";
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {

                int companyID;
                if (Request.GetRequestValue("keyValue", out companyID))
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        var org = mgr.GetTable<Organization>().Where(o => o.CompanyID == companyID).FirstOrDefault();
                        if (org != null)
                        {
                            if (org.EnterpriseGroupMember.Count > 0)    //已經是集團成員身分：取消集團成員
                            {
                                mgr.DeleteAll<EnterpriseGroupMember>(g => g.CompanyID == companyID);
                                mgr.ExecuteCommand(
                                    @"UPDATE UserRole
                                SET     RoleID = 62
                                FROM  OrganizationCategory INNER JOIN
                                          UserRole ON OrganizationCategory.OrgaCateID = UserRole.OrgaCateID
                                WHERE (UserRole.RoleID = 63 OR UserRole.RoleID = 61) AND (OrganizationCategory.CompanyID = {0}) ", org.CompanyID);

                                Response.Write(serializer.Serialize(new { result = true, message = "開立發票集團成員已停用!!" }));
                                return;
                            }
                            else
                            {   //已經是相對營業人身分：建立集團成員
                                int enterpriseID;
                                if (Request.GetRequestValue("enterprise", out enterpriseID))
                                {
                                    org.EnterpriseGroupMember.Add(new EnterpriseGroupMember
                                    {
                                        EnterpriseID = enterpriseID
                                    });

                                    mgr.SubmitChanges();
                                    mgr.ExecuteCommand(
                                        @"UPDATE UserRole
                                SET     RoleID = 63
                                FROM  OrganizationCategory INNER JOIN
                                          UserRole ON OrganizationCategory.OrgaCateID = UserRole.OrgaCateID
                                WHERE (UserRole.RoleID = 62) AND (OrganizationCategory.CompanyID = {0}) ", org.CompanyID);

                                    Response.Write(serializer.Serialize(new { result = true, message = "開立發票集團成員已啟用!!" }));
                                    return;
                                }
                            }
                        }
                    }
                }

                Response.Write(serializer.Serialize(new { result = false, message = "資料錯誤!!" }));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Response.Write(serializer.Serialize(new { result = false, message = ex.Message }));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
