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
using System.Data.SqlClient;
using Uxnet.Web.Helper;
using System.Data.Linq;

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

            var status = 0;

            if (File.Exists(gpgName))
            {
                storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));

                status = 1;
                
            }
            else
            {
                storeFile(fullPath, Path.Combine(_requestPath, fileName));

                status = 0;
            }

            using (var db = new DataContext(DbConnection.LocalDb.InvoiceClient))
            {
                var sqlCommand = $@"INSERT INTO [dbo].PGPEncryptLog 
                                         (SourceFilePath, PGPFileName, Status) 
                                         VALUES('{fullPath}','{Path.GetFileName(gpgName)}',{status})";
                
                var value = db.ExecuteQuery<int>(sqlCommand);
            }
        }

    }
}
