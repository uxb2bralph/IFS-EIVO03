using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uxnet.ToolAdapter.Properties;

namespace Uxnet.Com.Properties
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
    }

}