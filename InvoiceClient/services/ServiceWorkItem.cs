using InvoiceClient.Properties;
using Model.InvoiceManagement;
using System;
using System.Collections.Generic;

using System.Data.Linq;
using System.IO;
using System.Linq;
using Uxnet.Com.Helper;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utility;
using Uxnet.Web.Helper;
using System.Xml.Linq;
using Model.Models;
using Model.DataEntity;

namespace InvoiceClient.services
{
    public static class ServiceWorkItem
    {
        static int _AssertionDay = 1;
        static TimeSpan _AssertionTime = new TimeSpan(0, 2, 0);
        static string _path;
        static int _ScheduleToNextTurn = int.Parse(Settings.Default.ScheduleToNextTurn);

        static ServiceWorkItem()
        {
            var jobList = JobScheduler.JobList;

            if (Settings.Default.EnableJobScheduler)
            {

                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(GooglePlayGeneratorPDFLogToDB).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(GooglePlayGeneratorPDFLogToDB).AssemblyQualifiedName,
                        Description = "GooglePlay產製PDFlog寫入DB",
                        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, _AssertionDay).Add(_AssertionTime)
                    });
                }
                if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(InvoicePDFWatcherForZipLogToDB).AssemblyQualifiedName))
                {
                    JobScheduler.AddJob(new JobItem
                    {
                        AssemblyQualifiedName = typeof(InvoicePDFWatcherForZipLogToDB).AssemblyQualifiedName,
                        Description = "壓縮打包PDFlog寫入DB",
                        Schedule = new DateTime(DateTime.Today.Year, DateTime.Today.Month, _AssertionDay).Add(_AssertionTime)
                    });
                }
            }
        }

        public class GooglePlayGeneratorPDFLogToDB : IJob
        {
            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                return DateTime.Now.AddMinutes(_ScheduleToNextTurn);
            }

            public void DoJob()
            {

                PlayGeneratorPDFLogToDB();
            }

            public void Dispose()
            {

            }
        }

        public class InvoicePDFWatcherForZipLogToDB : IJob
        {
            public DateTime GetScheduleToNextTurn(DateTime current)
            {
                return DateTime.Now.AddMinutes(_ScheduleToNextTurn);
            }

            public void DoJob()
            {

                PDFForZipLogToDB();
            }

            public void Dispose()
            {

            }
        }

        private static void PlayGeneratorPDFLogToDB()
        {
            _path = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "logs");

            //取得當前日期減一日的daily資料夾
            string filePath = ValueValidity.GetDateStylePath(_path, DateTime.Now.AddDays(-1));

            filePath = $"{filePath}\\InvoicePDFGeneratorForGooglePlay.xml";

            if (File.Exists(filePath))
            {
                XDocument xmlDoc = XDocument.Load(filePath);                

                using (InvoiceClientEntityManager<InvoicePDFGeneratorForGooglePlayLog> mgr = new InvoiceClientEntityManager<InvoicePDFGeneratorForGooglePlayLog>())
                {
                    var records = from list in xmlDoc.Descendants("InvoicePDFGeneratorForGooglePlayModel")
                                  select list;

                    var isExist = false;

                    Logger.Info("InvoicePDFGeneratorForGooglePlayLog ConnectionString:" + mgr.DataContext.Connection.ConnectionString);

                    foreach (var item in records)
                    {
                        if (!(item.Element("Date") == null || item.Element("Path") == null || item.Element("OrderNo") == null))
                        {
                            var sqlCommand = $@"INSERT INTO [InvoiceClient].[dbo].[InvoicePDFGeneratorForGooglePlayLog] 
                                         (OrderNo, FileName, CreateDate) 
                                         VALUES('{item.Element("OrderNo").Value}','{item.Element("Path").Value}','{item.Element("Date").Value}')";

                            var message = string.Empty;

                            try
                            {
                                mgr.ExecuteCommand(sqlCommand);
                            }
                            catch (Exception ex)
                            {
                                message = ex.ToString() + "error OrderNo:" + item.Element("OrderNo").Value;
                                Logger.Error(message);
                            }

                            if (message.IndexOf("PRIMARY KEY") > -1 || message.Equals(string.Empty))
                            {

                                isExist = true;

                                item.RemoveAll();

                                xmlDoc.Save(filePath);
                            }
                        }
                    }

                    if (!isExist)
                    {
                        File.Delete(filePath);
                    }
                }
            };
        }

        private static void PDFForZipLogToDB()
        {
            _path = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "logs");

            //取得當前日期減一日的daily資料夾
            string filePath = ValueValidity.GetDateStylePath(_path, DateTime.Now.AddDays(-1));

            filePath = $"{filePath}\\InvoicePDFWatcherForZip.xml";

            if (File.Exists(filePath))
            {
                XDocument xmlDoc = XDocument.Load(filePath);

                using (InvoiceClientEntityManager<InvoicePDFWatcherForZipLog> mgr = new InvoiceClientEntityManager<InvoicePDFWatcherForZipLog>())
                {
                    var records = from list in xmlDoc.Descendants("InvoicePDFWatcherForZipModel")
                                  select list;

                    var isExist = false;

                    Logger.Info("InvoicePDFWatcherForZipLog ConnectionString:" + mgr.DataContext.Connection.ConnectionString);

                    foreach (var item in records)
                    {
                        if (!(item.Element("Date") == null || item.Element("FileName") == null || item.Element("OrderNo") == null))
                        {                            
                            var sqlCommand = $@"INSERT INTO [InvoiceClient].[dbo].[InvoicePDFWatcherForZipLog] 
                                         (OrderNo, ZipFileName, Status, CreateDate) 
                                         VALUES('{item.Element("OrderNo").Value}','{item.Element("FileName").Value}','{item.Element("Status").Value}','{item.Element("Date").Value}')";
                            
                            var message = string.Empty;

                            try
                            {
                                mgr.ExecuteCommand(sqlCommand);
                            }
                            catch (Exception ex)
                            {
                                message = ex.ToString() + "error OrderNo:" + item.Element("OrderNo").Value;
                                Logger.Error(message);
                            }
                                                        
                            if (message.IndexOf("PRIMARY KEY") > -1 || message.Equals(string.Empty))
                            {

                                isExist = true;

                                item.RemoveAll();

                                xmlDoc.Save(filePath);
                            }
                        }
                    }

                    if (!isExist)
                    {
                        File.Delete(filePath);
                    }
                }
            };

        }
    }
}

