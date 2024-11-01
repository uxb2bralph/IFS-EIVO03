using System.Web.Mvc;
using System.Web.Routing;

namespace Uxnet.Web.Helper.MvcExtension
{

    public static class ControllerContextExtensions
    {
        public static bool IsTransferAction(this ControllerContext context)
        {
            RouteData routeData = context.RouteData;
            if (routeData == null)
            {
                return false;
            }

            return context.HttpContext.Request.QueryString[TransferActionOnlyAttribute.IsTransferActionMarker] != null;
        }
    }
}