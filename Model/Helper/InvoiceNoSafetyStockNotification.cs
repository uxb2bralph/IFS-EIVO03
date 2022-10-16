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

namespace Model.Helper
{
    public static class InvoiceNoSafetyStockNotification
    {
        private static QueuedProcessHandler __Handler;

        static InvoiceNoSafetyStockNotification()
        {
            lock (typeof(InvoiceNoSafetyStockNotification))
            {
                if (__Handler == null)
                {
                    __Handler = new QueuedProcessHandler
                    {
                        MaxWaitingCount = 1,
                        Process = () =>
                        {

                            InquireNoIntervalViewModel viewModel = new InquireNoIntervalViewModel
                            {
                                Year = DateTime.Today.Year,
                                PeriodNo = (DateTime.Today.Month + 1) / 2
                            };
                            
                            using (TrackNoIntervalManager models = new TrackNoIntervalManager())
                            {
                                //models.SettleVacantInvoiceNo(year, period);
                                var assignments =
                                    models.PromptTrackCodeAssignment(viewModel.Year.Value, viewModel.PeriodNo.Value)
                                        .Where(a => a.Organization.OrganizationExtension.InvoiceNoSafetyStock.HasValue);

                                OrganizationViewModel orgModel = new OrganizationViewModel { };

                                foreach (var item in assignments.ToList())
                                {
                                    viewModel.SellerID = item.SellerID;
                                    var items = viewModel.InquireInvoiceNoInterval(models);
                                    if (items.Any())
                                    {
                                        var invoiceNoStock = items.ToList().Sum(i => i.EndNo + 1 - i.CurrentAllocatingNo());
                                        if (invoiceNoStock < item.Organization.OrganizationExtension.InvoiceNoSafetyStock)
                                        {
                                            orgModel.CompanyID = item.SellerID;
                                            EIVOPlatformFactory.NotifyLowerInvoiceNoStock(orgModel);
                                        }
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
