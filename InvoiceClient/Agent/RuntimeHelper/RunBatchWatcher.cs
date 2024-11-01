using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading;
using System.Xml;
using System.Reflection;
using System.Web.Mvc;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Model.Helper;
using Newtonsoft.Json;
using Utility;
using Model.Models.ViewModel;
using ModelExtension.Helper;
using ModelExtension.Notification;
using System.Diagnostics;
using System.Windows.Forms;

namespace InvoiceClient.Agent.RuntimeHelper
{
    public class RunBatchWatcher : InvoiceWatcher
    {
        public RunBatchWatcher(String fullPath)
            : base(fullPath)
        {
            //_ResponsedPath = fullPath + "(Response)";
            //_ResponsedPath.CheckStoredPath();
        }

        protected override void processFile(String invFile)
        {
            if (!File.Exists(invFile))
                return;

            String fileName = Path.GetFileName(invFile);
            String backupPath = Path.Combine(Logger.LogDailyPath, fileName);

            try
            {
                if(File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                File.Move(invFile, backupPath);

                if (backupPath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessStartInfo info = new ProcessStartInfo(backupPath)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false
                    };

                    Process.Start(info);
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"while processing move {invFile} => {backupPath}\r\n{ex}");
                return;
            }
        }

    }
}
