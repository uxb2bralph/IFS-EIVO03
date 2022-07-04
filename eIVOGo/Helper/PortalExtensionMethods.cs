using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;


using eIVOGo.Properties;

using MessagingToolkit.QRCode.Codec;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace eIVOGo.Helper
{
    public static class PortalExtensionMethods
    {
        public static void NotifyToResetPassword(this UserProfile profile)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadString($"{Uxnet.Web.Properties.Settings.Default.HostUrl}{VirtualPathUtility.ToAbsolute("~/Notification/ActivateUser")}?uid={profile.UID}&resetPass={true}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });

        }

        public static void NotifyTwoFactorSettings(this UserProfile profile)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadString($"{Uxnet.Web.Properties.Settings.Default.HostUrl}{VirtualPathUtility.ToAbsolute("~/Notification/ActivateUser")}?uid={profile.UID}&resetPass={true}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });

        }

        public static void NotifyToActivate(this UserProfile profile)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadString($"{Uxnet.Web.Properties.Settings.Default.HostUrl}{VirtualPathUtility.ToAbsolute("~/Notification/ActivateUser")}?uid={profile.UID}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("［網際優勢電子發票加值中心 會員啟用認證信］傳送失敗,原因 => " + ex.Message);
                    Logger.Error(ex);
                }
            });
            //ThreadPool.QueueUserWorkItem(t =>
            //{

            //});
        }

        public static void NotifyToActivate(this String pid)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadString($"{Uxnet.Web.Properties.Settings.Default.HostUrl}{VirtualPathUtility.ToAbsolute("~/Notification/ActivateUser")}?PID={HttpUtility.UrlEncode(pid)}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("［網際優勢電子發票加值中心 會員啟用認證信］傳送失敗,原因 => " + ex.Message);
                    Logger.Error(ex);
                }
            });
            //ThreadPool.QueueUserWorkItem(t =>
            //{

            //});
        }

    }
}