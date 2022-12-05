using Model.DataEntity;
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
using Model.Models.ViewModel;
using Business.Helper.ReportProcessor;
using System.Diagnostics;

namespace ProcessorUnit.Execution
{
    public class AutoUpdater : ExecutorForeverBase
    {
        const String UpdateBatch = "AutoUpdate.bat";
        public AutoUpdater()
        {

        }

        protected override void DoSomething()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateBatch)))
            {
                RunBatch(UpdateBatch,null);
                Program.Terminate();
            }

            base.DoSomething();
        }

        public static Process RunBatch(String batchFileName, String args)
        {
            Logger.Info($"{batchFileName} {args}");
            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, batchFileName), args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            return Process.Start(info);
        }
    }
}
