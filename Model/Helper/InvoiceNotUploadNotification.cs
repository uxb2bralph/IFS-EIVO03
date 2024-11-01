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
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using Model.Locale;

namespace Model.Helper
{
    public static class InvoiceNotUploadNotification
    {
        private static QueuedProcessHandler __Handler;

        static InvoiceNotUploadNotification()
        {
            lock (typeof(InvoiceNotUploadNotification))
            {
                if (__Handler == null)
                {
                    __Handler = new QueuedProcessHandler
                    {
                        MaxWaitingCount = 1,
                        Process = () =>
                        {
                            using (InvoiceManager mgr = new InvoiceManager())
                            {
                                var checkDate = DateTime.Today.AddDays(-1);

                                var eligibleInvoiceCountZeroSellersWeatherSettingAlertOrNot = 
                                    GetInvoiceUploadZeroWeatherSettingAlertOrNotList(
                                        mgr, 
                                        checkDate);

                                var eligibleInvoiceCountZeroSellers =
                                    eligibleInvoiceCountZeroSellersWeatherSettingAlertOrNot
                                        .Where(x => x.OrganizationSettings.Any(s => s.Settings == "InvoiceNotUploadedAlert"));

                                foreach (var seller in eligibleInvoiceCountZeroSellers)
                                {
                                    /* 2023.09.19 會議討論暫時不納入
                                    var mailing = mgr.GetUserListByCompanyID(item.SellerId)
                                        .Select(u => u.EMail)
                                        .Take(0)
                                        .ToList();
                                    item.NotifyEmails.AddRange(mailing);
                                    */

                                    OrganizationViewModel orgModel = new OrganizationViewModel { };
                                    orgModel.CompanyID = seller.CompanyID;
                                    EIVOPlatformFactory.NotifyInvoiceNotUpload(orgModel);
                                }
                            }
                        }
                    };
                }
            }
        }

        public static IQueryable<Organization> GetInvoiceUploadZeroWeatherSettingAlertOrNotList(
            InvoiceManager mgr, 
            DateTime checkDate)
        {
            var eligibleSellers =
                 mgr.GetTable<Organization>()
                     .Where(x => (!x.OrganizationExtension.ExpirationDate.HasValue)
                         || (x.OrganizationExtension.ExpirationDate >= checkDate))
                     .Where(x => x.OrganizationExtension.GoLiveDate <= checkDate)
                     .Where(x => x.OrganizationStatus.CurrentLevel != (int)Naming.MemberStatusDefinition.Mark_To_Delete)
                     .Where(x => x.OrganizationCategory
                                    .Where(y=> y.CategoryID == (int)CategoryDefinition.CategoryEnum.發票開立營業人
                                        || y.CategoryID == (int)CategoryDefinition.CategoryEnum.經銷商)
                                    .Any());

            //var eligibleInvoiceCountNotZeroSellers =
            //    mgr.GetTable<InvoiceItem>()
            //        .Where(x => x.InvoiceDate.Value.Year == checkDate.Year)
            //        .Where(x => x.InvoiceDate.Value.Month == checkDate.Month)
            //        .Where(x => x.InvoiceDate.Value.Day == checkDate.Day)
            //        .Where(x => eligibleSellers.Contains((int)x.SellerID))
            //        .GroupBy(x => x.SellerID)
            //        .Select(y => y.Key ?? 0);

            //var eligibleInvoiceCountZeroSellers
            //    = eligibleSellers.Except(eligibleInvoiceCountNotZeroSellers).ToList();
            var invoiceItems = mgr.GetTable<InvoiceItem>()
                .Where(i => i.InvoiceDate >= checkDate)
                .Where(i => i.InvoiceDate < checkDate.AddDays(1));
            var eligibleInvoiceCountZeroSellers
                = eligibleSellers.Where(c => !invoiceItems.Any(i => i.SellerID == c.CompanyID));

            return eligibleInvoiceCountZeroSellers;
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

    public class NotifyInvoiceNotUpload
    {
        public NotifyInvoiceNotUpload() { }
        public int SellerId { get; set; }
        public string ContactEmails { get; set; }
        public string CompanyName { get; set; }
        public string ReceiptNo { get; set; }
        public List<string> UserProfileEmails { get; set; } = new List<string>(); 
        public int Count { get; set; }
        public string GetNotifyEmailsString 
            => String.Join(",",UserProfileEmails.Where(m => m != null));

        public override string ToString()
        {
            return $"SellerId-{SellerId}" +
                $", 發票{Count}筆" +
                $", ReceiptNo-{ReceiptNo}" +
                $", CompanyName-{CompanyName}" +
                $", NotifyEmailsTo-{GetNotifyEmailsString}";
                //$", SmtpSettings-{SmtpSettings.UrlAction}";
        }
    }
}
