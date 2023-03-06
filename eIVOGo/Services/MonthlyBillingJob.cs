using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Model.InvoiceManagement;
using eIVOGo.Properties;
using Utility;

using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using Business.Helper.BillingSettlement;
using Uxnet.Com.Helper;

namespace eIVOGo.Services
{
    public class MonthlyBillingJob : IJob
    {
        private bool disposedValue;

        public void DoJob()
        {
            Task.Run(() =>
            {
                try
                {
                    using (ModelSource<EIVOEntityDataContext> models = new ModelSource<EIVOEntityDataContext>())
                    {
                        var settlement = DateTime.Now.AddMonths(-1)
                                .AssertMonthlyBillingSettlement(models);
                        settlement.DoMonthlyBillingSettlement(models);
                        models.DoSubmitBill();
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        public DateTime GetScheduleToNextTurn(DateTime current)
        {
            DateTime nextPeriod = new DateTime(current.Year, current.Month, AppSettings.Default.Billing.BillingDay).AddMonths(1);
            return nextPeriod;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MonthlyBillingJob()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}