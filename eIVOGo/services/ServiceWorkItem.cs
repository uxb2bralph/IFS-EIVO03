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

namespace eIVOGo.services
{
    public static class ServiceWorkItem
    {
        private static DateTime __DailyCheck = DateTime.Today;

        static int _AssertionDay = 10;
        static TimeSpan _AssertionTime = new TimeSpan(5, 0, 0);
        static TimeSpan _CheckTime = new TimeSpan(9, 0, 0);

        static ServiceWorkItem()
        {
            var jobList = JobScheduler.JobList;

            if (Settings.Default.EnableJobScheduler)
            {

                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(UnassignNOCheckSchedule).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(UnassignNOCheckSchedule).AssemblyQualifiedName,
                        Description = "計算上期空白發票",
                        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).Add(_AssertionTime)
                    });
                }
                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(DailyCheckSchedule).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(DailyCheckSchedule).AssemblyQualifiedName,
                        Description = "簡訊儲值通知",
                        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, _AssertionDay).Add(_AssertionTime)
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
            if (__DailyCheck < DateTime.Now)
            {
                __DailyCheck = DateTime.Today.AddDays(1);
                try
                {
                    double credit;
                    if (ModelExtension.MessageManagement.SMSManager.AlertToLowerCredit(out credit))
                    {
                        String.Format("簡訊儲值點數即將用盡!!剩餘點數:{0}", credit).SendMailMessage(Settings.Default.WebMaster, "簡訊儲值點數不足");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public static void doUnassignNOCheck(int sellerID, int trackID)
        {
            using(TrackNoIntervalManager models = new TrackNoIntervalManager())
            {
                models.SettleUnassignedInvoiceNO(sellerID, trackID);
            }
        }

        private static void doDailyTurnKeyCheck()
        {
            if (__DailyCheck < DateTime.Now)
            {
                __DailyCheck = DateTime.Today.AddDays(1);
                try
                {
                    eIVOGo.Published.AlertAdmTurnKeyInfo Info= new eIVOGo.Published.AlertAdmTurnKeyInfo();
                    
                    if (Info.totalRecordCount() > 0)
                    {
                        try
                        {
                            MailMessage message = new MailMessage();
                            message.ReplyToList.Add(Settings.Default.ReplyTo);
                            message.From = new MailAddress(Settings.Default.WebMaster);

                            message.To.Add(Settings.Default.WebMaster);
                            message.Subject = "電子發票系統 未上傳至TurnKey通知";
                            message.IsBodyHtml = true;
                            using (WebClient wc = new WebClient())
                            {
                                wc.Encoding = Encoding.UTF8;
                                message.Body = wc.DownloadString(String.Format("{0}{1}",
                                    Uxnet.Web.Properties.Settings.Default.HostUrl,
                                    VirtualPathUtility.ToAbsolute("~/Published/AlertAdmTurnKeyInfo.aspx")));
                            }

                            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
                            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
                            smtpclient.Send(message);

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }    
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public static bool ThreadSafeCheckEnable(ref bool token)
        {
            var bRun = false;
            lock (typeof(ServiceWorkItem))
            {
                if (!token)
                {
                    bRun = true;
                    token = true;
                }
            }
            return bRun;
        }

        public class UnassignNOCheckSchedule : IJob
        {

            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                return current.AddMonths(2 - ((current.Month / 2 + 1) % 2));
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
                        models.SettleUnassignedInvoiceNO(year, periodNo);
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
                return current.AddMonths(1);
            }

            public void DoJob()
            {
                doDailyCheck();
            }

            public void Dispose()
            {

            }
        }
        public class TurnKeyCheckSchedule : IJob
        {

            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                return current.AddDays(1);
            }

            public void DoJob()
            {
                doDailyTurnKeyCheck();
            }

            public void Dispose()
            {

            }
        }
        
        
    }
}