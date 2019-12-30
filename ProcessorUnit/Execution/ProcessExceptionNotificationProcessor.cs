﻿using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using ProcessorUnit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Utility;
using Model.InvoiceManagement;
using ClosedXML.Excel;
using System.Net;
using Uxnet.Com.Helper;
using ProcessorUnit.Properties;

namespace ProcessorUnit.Execution
{
    public class ProcessExceptionNotificationProcessor : ExecutorForeverBase
    {
        private const int __MAX_PROCESS_COUNT = 1024;

        public ProcessExceptionNotificationProcessor()
        {

        }

        protected override void DoSomething()
        {
            using (models = new ModelSource<InvoiceItem>())
            {
                var items = models.GetTable<ProcessExceptionNotification>().Where(p => !p.BookingTime.HasValue)
                        .GroupBy(p => p.TaskID)
                        .Select(g=>g.Key)
                        .Take(__MAX_PROCESS_COUNT).ToList();
                if(items.Count>0)
                {
                    using (WebClientEx client = new WebClientEx())
                    {
                        client.Timeout = 43200000;

                        foreach (var item in items)
                        {
                            try
                            {
                                var result = models.ExecuteCommand("update [proc].ProcessExceptionNotification set BookingTime = GETDATE() where TaskID = {0}", item);
                                if (result > 0)
                                {
                                    client.DownloadString($"{Settings.Default.EIVOPortal}/Notification/NotifyProcessException?taskID={item}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }
                    }
                }
            }

            base.DoSomething();
        }

    }
}
