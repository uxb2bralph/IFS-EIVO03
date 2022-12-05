using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Text;
using Model.InvoiceManagement;
using eIVOGo.Properties;
using Utility;

using Model.DataEntity;
using System.Diagnostics;
using Model.Locale;
using Uxnet.Com.Helper;
using System.Threading.Tasks;
using eIVOGo.Module.Common;
using Model.Helper;

namespace eIVOGo.Services
{
    public static class ServiceWorkItem
    {
        static ServiceWorkItem()
        {
            var jobList = JobScheduler.JobList;

            if (Settings.Default.EnableJobScheduler)
            {

                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(UnassignNOCheckSchedule).AssemblyQualifiedName))
                {
                    DateTime initDate = DateTime.Today.AddMonths(2);

                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(UnassignNOCheckSchedule).AssemblyQualifiedName,
                        Description = "計算上期空白發票",
                        Schedule = new DateTime(initDate.Year, (initDate.Month - 1) / 2 * 2 + 1, 1)
                    });
                }
                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(DailyCheckSchedule).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(DailyCheckSchedule).AssemblyQualifiedName,
                        Description = "每日自動檢查",
                        Schedule = DateTime.Today.Date
                    });
                }
                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(MonthlyBillingJob).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(MonthlyBillingJob).AssemblyQualifiedName,
                        Description = "每月應收帳款",
                        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, AppSettings.Default.Billing.BillingDay),
                    });
                }

                //if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(TurnKeyCheckSchedule).AssemblyQualifiedName))
                //{
                //    JobScheduler.AddJob(new JobItem
                //    {
                //        AssemblyQualifiedName = typeof(TurnKeyCheckSchedule).AssemblyQualifiedName,
                //        Description = "每日未上傳大平台統計",
                //        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).Add(_AssertionTime)
                //    });
                //}
            }

        }

        private static void doDailyCheck()
        {
            //try
            //{
            //    double credit;
            //    if (ModelExtension.MessageManagement.SMSManager.AlertToLowerCredit(out credit))
            //    {
            //        String.Format("簡訊儲值點數即將用盡!!剩餘點數:{0}", credit).SendMailMessage(Settings.Default.WebMaster, "簡訊儲值點數不足");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex);
            //}

            InvoiceNoSafetyStockNotification.Notify();
            DailyJobs.Notify();
        }


        //private static void doDailyTurnKeyCheck()
        //{
        //    try
        //    {
        //        eIVOGo.Published.AlertAdmTurnKeyInfo Info = new eIVOGo.Published.AlertAdmTurnKeyInfo();

        //        if (Info.totalRecordCount() > 0)
        //        {
        //            try
        //            {
        //                MailMessage message = new MailMessage();
        //                message.ReplyToList.Add(Settings.Default.ReplyTo);
        //                message.From = new MailAddress(Settings.Default.WebMaster);

        //                message.To.Add(Settings.Default.WebMaster);
        //                message.Subject = "電子發票系統 未上傳至TurnKey通知";
        //                message.IsBodyHtml = true;
        //                using (WebClient wc = new WebClient())
        //                {
        //                    wc.Encoding = Encoding.UTF8;
        //                    message.Body = wc.DownloadString(String.Format("{0}{1}",
        //                        Uxnet.Web.Properties.Settings.Default.HostUrl,
        //                        VirtualPathUtility.ToAbsolute("~/Published/AlertAdmTurnKeyInfo.aspx")));
        //                }

        //                SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
        //                smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
        //                smtpclient.Send(message);

        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Error(ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //    }
        //}

        public static void StartUp()
        {

        }

        public class UnassignNOCheckSchedule : IJob
        {

            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                DateTime nextPeriod = new DateTime(current.Year, (current.Month - 1) / 2 * 2 + 1, 1).AddMonths(2);
                if (current.Day >= 10)
                {
                    return nextPeriod;
                }
                else
                {
                    if (current.Month % 2 == 1)
                    {
                        return current.AddDays(1);
                    }
                    else
                    {
                        return nextPeriod;
                    }
                }
            }

            public void DoJob()
            {
                Task.Run(() =>
                {
                    DateTime calcPeriod = DateTime.Today.AddMonths(-2);
                    int year = calcPeriod.Year;
                    int periodNo = (calcPeriod.Month + 1) / 2;

                    using (TrackNoIntervalManager models = new TrackNoIntervalManager())
                    {
                        foreach (var item in models.PromptTrackCodeAssignment(year, periodNo).ToList())
                        {
                            if (item.Organization.OrganizationExtension?.AutoBlankTrack == true)
                            {
                                models.SettleUnassignedInvoiceNO(item);
                            }
                        }
                    }
                });
            }

            public void Dispose()
            {

            }
        }
        public class DailyCheckSchedule : IJob
        {

            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                return current.AddDays(1);
            }

            public void DoJob()
            {
                doDailyCheck();
            }

            public void Dispose()
            {

            }
        }
        //public class TurnKeyCheckSchedule : IJob
        //{

        //    public DateTime GetScheduleToNextTurn(DateTime current)
        //    {
        //        return current.AddDays(1);
        //    }

        //    public void DoJob()
        //    {
        //        doDailyTurnKeyCheck();
        //    }

        //    public void Dispose()
        //    {

        //    }
        //}
        
        
    }
}