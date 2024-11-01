using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Business.Helper;
using eIVOGo.Helper;
using Model.Models.ViewModel;
using eIVOGo.Models.ViewModel;

using eIVOGo.Properties;
using eIVOGo.Helper.Security.Authorization;
using Model.DataEntity;
using Model.DocumentManagement;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO.B2B;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Google.Authenticator;

namespace eIVOGo.Controllers
{
    public class AccountController : SampleController<InvoiceItem>
    {
        // GET: Account
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CbsLogin(CbsLoginViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ModelState = this.ModelState;

            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/CbsLogin.cshtml");
            }

            if(Settings.Default.UseGoogleAuthenticator)
            {
                var item = UserProfileFactory.CreateInstance(viewModel.PID, viewModel.Password)?.Profile;
                if (item == null)
                {
                    ModelState.AddModelError("PID", "login failed !!");
                    return View("~/Views/Account/CbsLogin.cshtml");
                }

                item = item.LoadInstance(models).PrepareTwoFactorKey(models);

                return View("~/Views/Account/TwoFactorLogin.cshtml", item);

            }

            LoginHandler login = new LoginHandler();
            String msg;
            UserProfileMember member;
            if (!login.ProcessLogin(viewModel.PID, viewModel.Password, out msg,out member))
            {
                ModelState.AddModelError("PID", msg);
                return View("~/Views/Account/CbsLogin.cshtml");
            }

            if (member.Profile.Expiration < DateTime.Today)
            {
                return View("~/Views/Account/ChangePassword.cshtml", member.Profile);
            }

            viewModel.ReturnUrl = viewModel.ReturnUrl.GetEfficientString();
            return Redirect(viewModel.ReturnUrl ?? msg ?? "~/Account/CbsLogin");

        }

        [AllowAnonymous]
        public ActionResult CbsLogin()
        {
            //UserProfile profile = HttpContext.GetUser();
            //if (profile == null)
            //    return View();
            //else
            //    return processLogin(profile);
            this.HttpContext.Logout();
            Session.Abandon();

            return View("~/Views/Account/CbsLogin.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorAuth(TwoFactorViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                ModelState.AddModelError("CodeDigit", "login failed !!");
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            viewModel.CodeDigit = viewModel.CodeDigit.GetEfficientString()?.Replace(" ", "");
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            if (!TwoFacAuth.ValidateTwoFactorPIN(Encoding.Default.GetBytes(item.UserProfileExtension.TwoFactorKey), viewModel.CodeDigit))
            {
                ModelState.AddModelError("CodeDigit", "login failed !!");
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            LoginHandler login = new LoginHandler();
            String msg;
            if (!login.ProcessLogin(item.PID, out msg))
            {
                ModelState.AddModelError("CodeDigit", "login failed !!");
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            return JavaScript($"window.location.href = '{VirtualPathUtility.ToAbsolute(msg ?? "~/Account/CbsLogin")}';");

        }


        // GET: Account
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var result = await CbsLogin(viewModel);
            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/Login.cshtml");
            }
            else
            {
                return result;
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.ViewModel = new LoginViewModel { };
            ViewResult result = (ViewResult)CbsLogin();
            if (Settings.Default.EIVO_Service != 0)
                result.ViewName = "~/Views/Account/Login.cshtml";
            return result;
        }

        public ActionResult ForgetPassword()
        {
            return View("~/Views/Account/ForgetPassword.cshtml");
        }

        public ActionResult CommitToResetPass(LoginViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ModelState = this.ModelState;

            if (!ModelState.IsValid)
            {
                return View("ForgetPassword");
            }

            var item = models.GetTable<UserProfile>().Where(u => u.PID == viewModel.PID).FirstOrDefault();
            if (item == null)
            {
                ModelState.AddModelError("PID", "帳號錯誤!!");
            }

            if (!ModelState.IsValid)
            {
                return View("ForgetPassword");
            }

            item.NotifyToResetPassword();
            return View("ForgetPassword", model: "重設密碼信件已送出至您的信箱!!");

        }



        public ActionResult CaptchaImg(String code)
        {

            string captcha = Encoding.Default.GetString(AppResource.Instance.Decrypt(Convert.FromBase64String(code)));

            Response.Clear();
            Response.ContentType = "image/Png";
            using (Bitmap bmp = new Bitmap(120, 30))
            {
                int x1 = 0;
                int y1 = 0;
                int x2 = 0;
                int y2 = 0;
                int x3 = 0;
                int y3 = 0;
                int intNoiseWidth = 25;
                int intNoiseHeight = 15;
                Random rdn = new Random();
                using (Graphics g = Graphics.FromImage(bmp))
                {

                    //設定字型
                    using (Font font = new Font("Courier New", 16, FontStyle.Bold))
                    {

                        //設定圖片背景
                        g.Clear(Color.CadetBlue);

                        //產生雜點
                        for (int i = 0; i < 100; i++)
                        {
                            x1 = rdn.Next(0, bmp.Width);
                            y1 = rdn.Next(0, bmp.Height);
                            bmp.SetPixel(x1, y1, Color.DarkGreen);
                        }

                        using (Pen pen = new Pen(Brushes.Gray))
                        {
                            //產生擾亂弧線
                            for (int i = 0; i < 15; i++)
                            {
                                x1 = rdn.Next(bmp.Width - intNoiseWidth);
                                y1 = rdn.Next(bmp.Height - intNoiseHeight);
                                x2 = rdn.Next(1, intNoiseWidth);
                                y2 = rdn.Next(1, intNoiseHeight);
                                x3 = rdn.Next(0, 45);
                                y3 = rdn.Next(-270, 270);
                                g.DrawArc(pen, x1, y1, x2, y2, x3, y3);
                            }
                        }

                        //把GenPassword()方法換成你自己的密碼產生器，記得把產生出來的密碼存起來日後才能與user的輸入做比較。

                        g.DrawString(captcha, font, Brushes.Black, 3, 3);

                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, ImageFormat.Png);
                        byte[] bmpBytes = ms.GetBuffer();
                        bmp.Dispose();
                        ms.Close();
                        Response.BinaryWrite(bmpBytes);
                        //context.Response.End();
                    }
                }
            }

            return new EmptyResult();
        }

        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS,Naming.RoleID.ROLE_SELLER })]
        public ActionResult AccountIndex(UserAccountQueryViewModel viewModel, bool? showTab)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ShowTab = showTab;

            var profile = HttpContext.GetUser();
            if(profile.IsSystemAdmin())
            {

            }
            else
            {
                if (!viewModel.SellerID.HasValue)
                {
                    viewModel.SellerID = profile.CurrentUserRole.OrganizationCategory.CompanyID;
                }
            }

            return View("~/Views/Account/AccountIndex.cshtml");
        }

        [AuthorizedSysAdmin()]
        public ActionResult SystemAccountIndex(UserAccountQueryViewModel viewModel, bool? showTab)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ShowTab = showTab;

            viewModel.QueryAction = "InquireSystemAccount";

            return View("~/Views/Account/AccountIndex.cshtml");
        }

        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS, Naming.RoleID.ROLE_SELLER })]
        public ActionResult Inquire(UserAccountQueryViewModel viewModel)
        {

            var profile = WebPageUtility.UserProfile;

            ViewBag.ViewModel = viewModel;
            IQueryable<UserProfile> items = models.GetTable<UserProfile>();

            if (viewModel.SellerID.HasValue)
            {
                items = items.FilterByOrganization(models, viewModel.SellerID.Value);
            }
            else if (!profile.IsSystemAdmin())
            {
                items = items.FilterByOrganization(models, profile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID!=null)
            {
                items = items.Where(u => u.PID.StartsWith(viewModel.PID));
            }
            viewModel.UserName = viewModel.UserName.GetEfficientString();
            if (viewModel.UserName!=null)
            {
                items = items.Where(u => u.UserName.Contains(viewModel.UserName));
            }
            if (viewModel.RoleID.HasValue)
            {
                items = (new UserProfileManager(models)).GetUserByUserRole(items, (int)viewModel.RoleID);
            }

            if (viewModel.LevelID.HasValue)
            {
                items = items.Where(u => u.UserProfileStatus.CurrentLevel == viewModel.LevelID);
            }

            viewModel.PageSize = viewModel.PageSize.HasValue && viewModel.PageSize > 0 
                ? viewModel.PageSize.Value 
                : Uxnet.Web.Properties.Settings.Default.PageSize;

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex -= 1;
                return View("~/Views/Account/Module/ItemList.cshtml", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/Account/Module/QueryResult.cshtml", items);
            }
        }

        [AuthorizedSysAdmin()]
        public ActionResult InquireSystemAccount(UserAccountQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)Inquire(viewModel);
            IQueryable<UserProfile> items = (IQueryable<UserProfile>)result.Model;
            items = items.Where(u => u.UserRole.Any(r => r.RoleID == (int)Naming.RoleID.ROLE_SYS));

            return View(result.ViewName, items);
        }

        public ActionResult DataItem(int? id)
        {
            var item = models.GetTable<UserProfile>().Where(d => d.UID == id).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "帳號資料錯誤!!");
            }

            return View("~/Views/Account/Module/DataItem.ascx", item);

        }

        public ActionResult SendConfirmation(int? id)
        {
            var item = models.GetTable<UserProfile>().Where(u => u.UID == id).FirstOrDefault();
            if (item != null)
            {
                item.NotifyToActivate();
                return View("~/Views/Shared/AlertMessage.cshtml", model: "確認信已送出!!");
            }

            return View("~/Views/Shared/AlertMessage.cshtml", model: "帳號資料錯誤!!");
        }

        public ActionResult Deactivate(int? id)
        {
            ViewResult result = (ViewResult)DataItem(id);
            var item = result.Model as UserProfile;
            if (item != null)
            {
                item.LevelID = (int)Naming.MemberStatusDefinition.Mark_To_Delete;
                models.SubmitChanges();
            }

            return result;
        }

        public ActionResult Activate(int? id)
        {
            ViewResult result = (ViewResult)DataItem(id);
            var item = result.Model as UserProfile;
            if (item != null)
            {
                item.LevelID = (int)Naming.MemberStatusDefinition.Checked;
                models.SubmitChanges();
            }

            return result;
        }

        public ActionResult DeleteItem(int? id)
        {
            var item = models.GetTable<UserProfile>().Where(d => d.UID == id).FirstOrDefault();

            if (item == null)
            {
                return Json(new { result = false, message = "帳號資料錯誤!!" });
            }

            var profile = WebPageUtility.UserProfile;
            if (!profile.IsSystemAdmin())
            {
                if (!models.GetTable<UserRole>().Any(r => r.UID == item.UID && r.OrgaCateID == profile.CurrentUserRole.OrgaCateID))
                {
                    return Json(new { result = false, message = "帳號非所屬會員使用者!!" });
                }
            }

            try
            {
                models.GetTable<UserProfile>().DeleteOnSubmit(item);
                models.SubmitChanges();
            }
            catch(Exception ex)
            {
                return Json(new { result = false, message = ex.Message });
            }

            return Json(new { result = true });

        }

        [AllowAnonymous]
        public ActionResult CBESignUp(OrganizationViewModel viewModel)
        {
            Organization item = null;
            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
                item = models.GetTable<Organization>().Where(u => u.CompanyID == viewModel.CompanyID).FirstOrDefault();
            }

            viewModel.ApplyFromModel(item);

            ViewBag.ViewModel = viewModel;
            return View("~/Views/Organization/CBE_SignUp.cshtml", item);
        }

        public ActionResult CommitCBESignUp(OrganizationViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
            }

            viewModel.SettingInvoiceType = Naming.InvoiceTypeDefinition.一般稅額計算之電子發票;
            Organization item = viewModel.CommitOrganizationViewModel(models, ModelState);

            if (item == null)
            {
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            return Json(new { result = true });

        }

        public ActionResult ChangeLanguage(String lang)
        {
            var cLang = lang.GetEfficientString() ?? Settings.Default.DefaultUILanguage;
            Response.SetCookie(new HttpCookie("cLang", cLang));
            return Json(new { result = true, message = System.Globalization.CultureInfo.CurrentCulture.Name }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult CommitPassword(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var profile = HttpContext.GetUser();
            if (profile == null)
            {
                return View("~/Views/Shared/AlertMessageDialog.cshtml", "資料錯誤，請重新登入!!");
            }

            UserProfile item = profile.Profile.LoadInstance(models);
            viewModel.PID = item?.PID;

            viewModel.UserProfileValueCheck(profile, ModelState);

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            item.UpdatePassword(viewModel);
            models.SubmitChanges();

            return JavaScript($"alert('密碼變更完成!!'); window.location.href = '{VirtualPathUtility.ToAbsolute("~/Home/MainPage")}';");

        }


    }
}