using Model.DataEntity;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.InvoiceManagement.InvoiceProcess
{
    public static class InvoiceProcessExtensions
    {
        public static Naming.InvoiceStepDefinition StepReadyToAllowanceMIG(this Organization seller)
        {
            return seller.OrganizationSettings.Any(s => s.Settings == "SendAllowanceMIGManually")
                ? Naming.InvoiceStepDefinition.待批次傳送
                : Naming.InvoiceStepDefinition.已開立;
        }
    }
}
