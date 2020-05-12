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
using DataContructor.Models;

namespace InvoiceClient.Agent
{
    public class PGPEncryptWatcherForGoogle : InvoiceWatcher
    {

        public PGPEncryptWatcherForGoogle(String fullPath) : base(fullPath)
        {

        }

        public String AddedStore { get; set; }

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
            int status = 0;
            if (File.Exists(gpgName))
            {
                storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                if (AddedStore != null)
                {
                    try
                    {
                        AddedStore.CheckStoredPath();
                        String storeTo = Path.Combine(AddedStore, Path.GetFileName(gpgName));
                        File.Copy(gpgName, storeTo);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                status = 1;
            }
            else
            {
                storeFile(fullPath, Path.Combine(_requestPath, fileName));
            }

            String filePath = Path.Combine(Logger.LogDailyPath, "PGPEncryptWatcherForGoogle.csv");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                using (CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(writer, true))
                {
                    csv.WriteRecord<PGPEncryptWatcherModel>(new PGPEncryptWatcherModel
                    {
                        FileName = gpgName,
                        Status = status,
                    });
                    writer.WriteLine();
                }
            }
        }

    }
}
