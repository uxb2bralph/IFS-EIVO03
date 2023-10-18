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
                                var checkDate = DateTime.Now.Date.AddDays(-1);

                                List<int> eligibleInvoiceCountZeroSellersWeatherSettingAlertOrNot = 
                                    GetInvoiceUploadZeroWeatherSettingAlertOrNotList(
                                        mgr, 
                                        checkDate);

                                var eligibleInvoiceCountZeroSellers = 
                                    mgr.GetTable<Organization>()
                                    .Where(x=> eligibleInvoiceCountZeroSellersWeatherSettingAlertOrNot.Contains(x.CompanyID))
                                    .Where(x=>x.OrganizationExtension.InvoiceNotUploadedAlert == true)
                                    .Select(y=>y.CompanyID) 
                                    .ToList();

                                foreach (var sellerID in eligibleInvoiceCountZeroSellers)
                                {
                                    /* 2023.09.19 會議討論暫時不納入
                                    var mailing = mgr.GetUserListByCompanyID(item.SellerId)
                                        .Select(u => u.EMail)
                                        .Take(0)
                                        .ToList();
                                    item.NotifyEmails.AddRange(mailing);
                                    */

                                    OrganizationViewModel orgModel = new OrganizationViewModel { };
                                    orgModel.CompanyID = sellerID;
                                    EIVOPlatformFactory.NotifyInvoiceNotUpload(orgModel);
                                }
                            }
                        }
                    };
                }
            }
        }

        public static List<int> GetInvoiceUploadZeroWeatherSettingAlertOrNotList(
            InvoiceManager mgr, 
            DateTime checkDate)
        {
            var eligibleSellers =
                 mgr.GetTable<Organization>()
                     .Where(x => (x.OrganizationExtension.ExpirationDate == null)
                         || (x.OrganizationExtension.ExpirationDate.Value.CompareTo(checkDate) >= 0))
                     .Where(x => x.OrganizationExtension.GoLiveDate.Value.CompareTo(checkDate) <= 0)
                     //.Where(x => x.OrganizationExtension.InvoiceNotUploadedAlert == true)
                     .Where(x => x.OrganizationStatus.CurrentLevel != (int)Naming.MemberStatusDefinition.Mark_To_Delete)
                     .Join(mgr.GetTable<OrganizationCategory>(),
                        i => i.CompanyID, d => d.CompanyID, (i, d) => d)
                     .Where(y =>
                         y.CategoryID == (int)CategoryDefinition.CategoryEnum.發票開立營業人
                         || y.CategoryID == (int)CategoryDefinition.CategoryEnum.經銷商)
                     .Select(z => z.CompanyID)
                     .ToList();

            var eligibleInvoiceCountNotZeroSellers =
                mgr.GetTable<InvoiceItem>()
                    .Where(x => x.InvoiceDate.Value.Year == checkDate.Year)
                    .Where(x => x.InvoiceDate.Value.Month == checkDate.Month)
                    .Where(x => x.InvoiceDate.Value.Day == checkDate.Day)
                    .Where(x => eligibleSellers.Contains((int)x.SellerID))
                    .GroupBy(x => x.SellerID)
                    .Select(y => y.Key ?? 0);

            var eligibleInvoiceCountZeroSellers
                = eligibleSellers.Except(eligibleInvoiceCountNotZeroSellers).ToList();

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
