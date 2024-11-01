using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Utility;
using Uxnet.ToolAdapter.Properties;

namespace ModelExtension.Properties
{
    public partial class AppSettings : AppSettingsBase
    {
        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings() : base()
        {

        }

        static AppSettings _default;
        public static AppSettings Default
        {
            get
            {
                return _default;
            }
        }

        public static void Reload()
        {
            Reload<AppSettings>(ref _default, typeof(AppSettings).Namespace);
        }

        public String LineToken { get; set; } = "Hp41egh8G3O4rDKXL3TxwrB4gWtuGh87ZNfYTxdr1Ls";
        public String LineNotify { get; set; } = "https://notify-api.line.me/api/notify";
    }
}