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
using System.Threading.Tasks;

namespace InvoiceClient.Agent
{
    public class InvoicePDFWatcherForZip : InvoiceWatcher
    {
        private String _outFile;
        private List<String> _files;
        private int _waitCycle;

        public InvoicePDFWatcherForZip(String fullPath) : base(fullPath)
        {
            _files = new List<string>(Settings.Default.MaxFileCountInPDFZip);
        }

        protected override void prepareStorePath(string fullPath)
        {

        }

        protected override void processBatchFiles(string[] files)
        {
            if (files.Length < Settings.Default.MaxFileCountInPDFZip)
            {
                if (_waitCycle < Settings.Default.MaxWaitingTurns)
                {
                    _waitCycle++;
                    Console.WriteLine($"fewer count:{files.Length}");
                    Task.Delay(Settings.Default.WaitForInvoicePDFInSeconds * 1000).Wait();
                    return;
                }

            }

            _waitCycle = 0;
            _files.Clear();

            _outFile = Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}.zip");
            using (var zipOut = System.IO.File.Create(_outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    for (int i = 0, count = 0; count < Settings.Default.MaxFileCountInPDFZip && i < files.Length; i++)
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

            if (_files.Count > 0)
            {
                String zipName = Path.Combine(_ResponsedPath, $"{Settings.Default.InvoicePDFZipPrefix}{DateTime.Now:yyyyMMddHHmmssffff}-{_files.Count}.zip");
                File.Move(_outFile, zipName);

                foreach (var item in _files)
                {
                    //File.Delete(item);
                    storeFile(item, Path.Combine(Logger.LogDailyPath, Path.GetFileName(item)));
                }
                
                //if(Settings.Default.PackerCycleDelayInSeconds>0 && _files.Count< __MaxFileCount)
                //{
                //    Task.Delay(Settings.Default.PackerCycleDelayInSeconds * 1000)
                //        .Wait();
                //}
            }

        }

        protected override void processComplete()
        {

        }

    }
}
