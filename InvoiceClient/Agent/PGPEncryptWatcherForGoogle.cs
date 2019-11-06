using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InvoiceClient.Properties;
using System.Xml;
using Utility;
using InvoiceClient.Helper;
using Model.Schema.EIVO;
using System.Threading;
using Model.Schema.TXN;
using System.Diagnostics;
using System.Globalization;
using Model.Locale;

namespace InvoiceClient.Agent
{
    public class PGPEncryptWatcherForGoogle : InvoiceWatcher
    {

        public PGPEncryptWatcherForGoogle(String fullPath) : base(fullPath)
        {

        }

        protected override void processFile(String invFile)
        {
            if (!File.Exists(invFile))
                return;

            String fileName = Path.GetFileName(invFile);
            String fullPath = Path.Combine(_inProgressPath, fileName);

            try
            {
                File.Move(invFile, fullPath);
            }
            catch (Exception ex)
            {
                Logger.Warn("move file error: " + invFile);
                Logger.Error(ex);
                return;
            }

            String gpgName = fullPath.EncryptFileTo(_ResponsedPath);

            if (File.Exists(gpgName))
            {
                storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
            }
            else
            {
                storeFile(fullPath, Path.Combine(_requestPath, fileName));
            }
        }

    }
}
