using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Properties
{
    public partial class AppSettings : Uxnet.Com.Properties.AppSettings
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

        public String StoreRoot { get; set; } = "WebStore";
        public String AttachmentTempStore { get; set; } = "TempAttachment";

    }
}
