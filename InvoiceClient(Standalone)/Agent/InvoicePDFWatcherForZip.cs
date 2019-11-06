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
using System.IO.Compression;

namespace InvoiceClient.Agent
{
    public class InvoicePDFWatcherForZip : InvoiceWatcher
    {
        private String _outFile;
        private List<String> _files;
        private const int __MaxFileCount = 1024;
        private string _storedPath;

        public InvoicePDFWatcherForZip(String fullPath) : base(fullPath)
        {
            _files = new List<string>(__MaxFileCount);
            _storedPath = Settings.Default.DownloadDataInAbsolutePath ? Settings.Default.DownloadSaleInvoiceFolder : Path.Combine(Settings.Default.InvoiceTxnPath, Settings.Default.DownloadSaleInvoiceFolder);
        }

        protected override void prepareStorePath(string fullPath)
        {

        }

        protected override void processBatchFiles(string[] files)
        {
            _files.Clear();

            _outFile = $"{DateTime.Now.Ticks}.zip";
            using (var zipOut = System.IO.File.Create(Path.Combine(Logger.LogDailyPath, _outFile)))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    for (int i = 0, count = 0; count < __MaxFileCount && i < files.Length; i++)
                    {
                        try
                        {
                            var item = files[i];
                            zip.CreateEntryFromFile(item, Path.GetFileName(item));
                            _files.Add(item);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
            }

            foreach (var item in _files)
            {
                File.Delete(item);
            }

            String args = $"{_outFile} {Logger.LogDailyPath} {_storedPath} {Settings.Default.InvoicePDFZipPrefix}{DateTime.Now:yyyyMMddHHmmssffff}-{_files.Count}";
            Process proc = Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipOutput.bat"), args);

        }

        protected override void processComplete()
        {

        }

    }
}
