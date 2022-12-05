using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eIVOGo.Properties
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
            get
            {
                return _default;
            }
        }

        public MonthlyBilling Billing { get; set; } = new MonthlyBilling { };

    }

    public class MonthlyBilling
    {
        public int BillingDay { get; set; } = 5;
    }


}