using Model.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExtension.Notification
{
    public static class PortalNotification
    {
        public static Action<UserProfile> NotifyToResetPassword { get; set; }
        public static Action<UserProfile> NotifyToActivate { get; set; }
    }
}
