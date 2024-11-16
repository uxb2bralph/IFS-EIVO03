using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uxnet.ToolAdapter.Properties;

namespace Model.Properties
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
            get => _default;
        }

        public static void Reload()
        {
            Reload<AppSettings>(ref _default, typeof(AppSettings).Namespace);
        }

        public String StoreRoot { get; set; } = "WebStore";
        public String AttachmentTempStore { get; set; } = "TempAttachment";
        public String EINVTurnkeyRoot { get; set; } = "C:\\EINVTurnkey";
        public String SystemKeyName { get; set; } = "SystemKey.new.json";
        public bool UseMIG40 { get; set; } = false;
        public int UserPasswordValidDays { get; set; } = 180;

    }
}
