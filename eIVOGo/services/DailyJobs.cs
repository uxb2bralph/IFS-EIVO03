using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Net;

using Utility;
using Model.Schema.TXN;
using System.Diagnostics;
using System.Threading.Tasks;
using Uxnet.Com.Helper;
using Model.DataEntity;
using Model.Models.ViewModel;
using Model.InvoiceManagement;

namespace eIVOGo.Services
{
    public static class DailyJobs
    {
        private static QueuedProcessHandler __Handler;

        static DailyJobs()
        {
            lock (typeof(DailyJobs))
            {
                if (__Handler == null)
                {
                    __Handler = new QueuedProcessHandler
                    {
                        Process = () =>
                        {
                            // check and update GoLiveDate of members
                            using (InvoiceManager models = new InvoiceManager())
                            {
                                //models.SettleVacantInvoiceNo(year, period);
                                var items =
                                    models.GetTable<Organization>()
                                        .Where(o => models.GetTable<InvoiceItem>().Any(i => i.SellerID == o.CompanyID))
                                        .Where(o => o.OrganizationExtension == null || !o.OrganizationExtension.GoLiveDate.HasValue);

                                foreach (var item in items.ToList())
                                {
                                    var invoice = models.GetTable<InvoiceItem>().Where(i => i.SellerID == item.CompanyID)
                                                        .OrderBy(i => i.InvoiceID).FirstOrDefault();
                                    if (invoice != null)
                                    {
                                        if (item.OrganizationExtension == null)
                                        {
                                            item.OrganizationExtension = new OrganizationExtension
                                            {

                                            };
                                        }
                                        item.OrganizationExtension.GoLiveDate = invoice.CDS_Document.DocDate;
                                        models.SubmitChanges();
                                    }
                                }
                            }
                        }
                    };
                }
            }
        }

        public static int ResetBusyCount()
        {
            return __Handler.ResetBusyCount();
        }

        public static void Notify()
        {
            __Handler.Notify();
        }
    }
}
