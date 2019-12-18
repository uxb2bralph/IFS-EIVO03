using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

using Utility;
using eIVOGo.Helper;
using Business.Helper;
using global::Model.DataEntity;
using global::Model.Locale;
using eIVOGo.Models.ViewModel;

namespace eIVOGo.Helper.Security.Authorization
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //用父類別的驗證，判斷是否在角色內
                if (!AuthorizeCore(filterContext.HttpContext))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                                { "controller", "Account" },
                                { "action", "CbsLogin" },
                                { "id", UrlParameter.Optional }
                        });
                }
            }
            else
            {
                // 未登入，轉至登入頁面
                string rtURL = "";
                rtURL = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "CbsLogin" },
                    { "ReturnUrl", rtURL }
                });
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                                { "controller", "Account" },
                                { "action", "CbsLogin" },
                                { "id", UrlParameter.Optional }
                });
        }
    }

    public class AuthorizedSysAdminAttribute : UserAuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            return httpContext.GetUser().IsSystemAdmin();

        }
    }

    public class RoleAuthorizeAttribute : UserAuthorizeAttribute
    {
        public Naming.RoleID[] RoleID { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            return httpContext.GetUser().IsAuthorized(RoleID);

        }
    }

}