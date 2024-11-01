using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Utility;
using Uxnet.ToolAdapter.Properties;

namespace Kiosk.Properties
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

        public String InvoiceClientLogPath { get; set; } = "C:\\UXB2B_GW\\logs";
        public String PrintInvoice { get; set; } = Path.Combine("C:\\UXB2B_GW\\logs", "InvoiceNoInspector", "PrintInvoice");
        public String Margin { get; set; } = "-0cm";
        public decimal Zoom { get; set; } = 100;
        public String PreparedInvoice { get; set; } = Path.Combine("C:\\UXB2B_GW\\logs", "InvoiceNoInspector", "PreparedInvoice");
        public PrintMode @PrintMode { get; set; } = PrintMode.ForPOS;

    }

    public enum PrintMode
    {
        ForPOS = 0,
        ForHeadquarter = 1,
    }

}