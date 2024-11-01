using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uxnet.ToolAdapter.Properties;

namespace TaskCenter.Properties
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
        public new static AppSettings Default
        {
            get => _default;
        }
    }
}