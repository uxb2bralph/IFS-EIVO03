using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using eIVOGo.Helper.Security.Authorization;

namespace eIVOGo.Controllers
{
   
    public class UserProfileController : SampleController<InvoiceItem>
    {
        
        [HttpGet]
        [Authorize]
        public ActionResult EditMySelf(bool? forCheck)
        {
            var profile = HttpContext.GetUser();
            UserProfileViewModel viewModel = new UserProfileViewModel
            {
                WaitForCheck = forCheck
            };

            var item = models.GetTable<UserProfile>().Where(u => u.PID == profile.PID).FirstOrDefault();

            if (item != null)
            {
                viewModel.KeyID = item.UID.EncryptKey();
                viewModel.PID = item.PID;
                viewModel.UserName = item.UserName;
                viewModel.EMail = item.EMail;
                viewModel.Address = item.Address;
                viewModel.Phone = item.Phone;
                viewModel.MobilePhone = item.MobilePhone;
                viewModel.Phone2 = item.Phone2;
                viewModel.SellerID = item.UserRole.FirstOrDefault()?.OrganizationCategory.CompanyID;
            }

            ViewBag.ViewModel = viewModel;
            return View(viewModel);
        }

        [Authorize]
        public ActionResult EditItem(UserProfileViewModel viewModel)
        {
            var item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();

            if(item==null)
            {
                if(viewModel.ResetID.HasValue)
                {
                    item = models.GetTable<ResetUserPassword>().Where(r => r.ResetID == viewModel.ResetID)
                        .Select(r => r.UserProfile).FirstOrDefault();
                }
            }

            if (item != null)
            {
                viewModel.KeyID = item.UID.EncryptKey();
                viewModel.PID = item.PID;
                viewModel.UserName = item.UserName;
                viewModel.EMail = item.EMail;
                viewModel.Address = item.Address;
                viewModel.Phone = item.Phone;
                viewModel.MobilePhone = item.MobilePhone;
                viewModel.Phone2 = item.Phone2;
                viewModel.DefaultRoleID = (Naming.RoleID?)item.UserRole.Select(r => r.RoleID).FirstOrDefault();
            }

            ViewBag.ViewModel = viewModel;
            return View("~/Views/UserProfile/EditUserProfile.cshtml", model:item);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(UserProfileViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditItem(viewModel);
            UserProfile item = result.Model as UserProfile;
            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
               
            if((new LoginHandler()).ProcessLogin(item.PID))
            {
                viewModel.WaitForCheck = true;
                return View("EditMySelf", item);
            }
            else
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料錯誤!!");
            }
        }

        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS, Naming.RoleID.ROLE_SELLER })]
        public ActionResult Commit(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var profile = HttpContext.GetUser();
            UserProfile item = null;
            if (!String.IsNullOrEmpty(viewModel.KeyID))
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.PID))
            {
                ModelState.AddModelError("PID", "帳號不可為空白!!");
            }
            else if ((item != null && models.GetTable<UserProfile>().Any(u => u.UID != item.UID && u.PID == viewModel.PID))
                    || (item == null && models.GetTable<UserProfile>().Any(u => u.PID == viewModel.PID)))
            {
                ModelState.AddModelError("PID", "這個帳號已被使用，請更換申請帳號!!");
            }

            Regex reg = new Regex("^(?=.*\\d)(?=.*[a-zA-Z])");

            if (!String.IsNullOrEmpty(viewModel.Password))
            {
                if (viewModel.Password.Length < 6)
                {
                    //檢查密碼
                    ModelState.AddModelError("PassWord", "密碼不可少於６個字碼!!");
                }

                else if (viewModel.Password != viewModel.Password1)
                {
                    //檢查密碼
                    ModelState.AddModelError("PassWord1", "二組密碼輸入不同!!");
                }
                else
                {
                    if (profile.IsSystemAdmin())
                    {
                    }
                    else if (!reg.IsMatch(viewModel.Password))
                    {
                        //檢查密碼
                        ModelState.AddModelError("PassWord", "密碼須由英文、數字組成!!");
                    }
                }
            }
            else if (item == null)
            {
                ///新增帳號
                ///
                if (String.IsNullOrEmpty(viewModel.Password))
                {
                    ModelState.AddModelError("PassWord", "密碼不可為空白!!");
                }
            }

            int? orgaCateID = null;

            if (viewModel.SellerID.HasValue)
            {
                orgaCateID = models.GetTable<OrganizationCategory>().Where(c => c.CompanyID == viewModel.SellerID)
                    .Select(c => c.OrgaCateID).FirstOrDefault();
            }

            if(!orgaCateID.HasValue)
            {
                if (profile.IsSystemAdmin())
                {
                    ModelState.AddModelError("SellerID", "請選擇所屬會員!!");
                }
                else
                {
                    orgaCateID = profile.CurrentUserRole.OrgaCateID;
                }
            }

            if (!viewModel.DefaultRoleID.HasValue)
            {
                ModelState.AddModelError("DefaultRoleID", "請選擇身份設定!!");
            }
            else if(!profile.IsSystemAdmin())
            {
                if (!Enum.IsDefined(typeof(Naming.EIVOMemberRoleID), (int)viewModel.DefaultRoleID))
                {
                    ModelState.AddModelError("DefaultRoleID", "請選擇身份設定!!");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (item == null)
            {
                item = new UserProfile
                {
                    UserProfileStatus = new UserProfileStatus
                    {
                        CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                    }
                };
                item.UserRole.Add(new UserRole
                {
                    OrgaCateID = orgaCateID.Value,
                    RoleID = ((int?)viewModel.DefaultRoleID) ?? (int)Naming.EIVOUserRoleID.會員
                });
                models.GetTable<UserProfile>().InsertOnSubmit(item);
            }
            else
            {
                models.ExecuteCommand("delete UserRole where UID = {0}", item.UID);
                item.UserRole.Add(new UserRole
                {
                    OrgaCateID = orgaCateID.Value,
                    RoleID = (int)viewModel.DefaultRoleID,
                });
            }

            item.PID = viewModel.PID;
            item.UserName = viewModel.UserName;
            if (!String.IsNullOrEmpty(viewModel.Password))
            {
                item.Password2 = Utility.ValueValidity.MakePassword(viewModel.Password);
                if (viewModel.WaitForCheck == true)
                    item.UserProfileStatus.CurrentLevel = (int)Naming.MemberStatusDefinition.Checked;
            }
            item.EMail = viewModel.EMail;
            item.Address = viewModel.Address;
            item.Phone = viewModel.Phone;
            item.MobilePhone = viewModel.MobilePhone;
            item.Phone2 = viewModel.Phone2;

            models.SubmitChanges();

            models.ExecuteCommand("delete ResetUserPassword where UID = {0}", item.UID);

            if (viewModel.WaitForCheck == true)
            {
                return View("~/Views/UserProfile/AccountChecked.ascx");
            }
            else
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: "資料已修改!!");
            }
        }

    }
}