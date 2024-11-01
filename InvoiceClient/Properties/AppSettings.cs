using Model.Locale;
using Model.Schema.TXN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uxnet.ToolAdapter.Properties;

namespace InvoiceClient.Properties
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

        public bool WatchSubDirectories { get; set; } = false;
        public ServiceInfo @ServiceInfo { get; set; }
        public bool InstalledService { get; set; } = false;
        public bool UseMainForm { get; set; }= true;

    }
}
