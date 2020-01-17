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

            var textContents = new List<string>();

            using (var zipOut = System.IO.File.Create(_outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    textContents.Add("Order No:");

                    for (int i = 0, count = 0; count < Settings.Default.MaxFileCountInPDFZip && i < files.Length; i++)
                    {
                        var item = files[i];
                        var fileName = Path.GetFileName(item);

                        var pdfName = fileName.Split('.').ToArray()[0].Split('_');

                        if (pdfName.Length > 1)
                        {
                            textContents.Add($"{pdfName[pdfName.Length - 2]}");
                        }

                        try
                        {
                            zip.CreateEntryFromFile(item, fileName);
                            _files.Add(item);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);

                            textContents.Add("Failed to add archive");
                        }
                    }
                }
            }

            if (_files.Count > 0)
            {
                var zipName = $"{Settings.Default.InvoicePDFZipPrefix}{DateTime.Now:yyyyMMddHHmmssffff}-{_files.Count}.zip";

                String moveFileName = Path.Combine(_ResponsedPath, zipName);

                File.Move(_outFile, moveFileName);

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
                textContents.Add($"Zip File Name:{zipName}");
            }

            if (textContents.Count > 0)
            {
                //The pdf file is packed into a compressed file and written to the log 
                foreach (var item in textContents)
                    Logger.PdfToZip(item);
            }
        }

        protected override void processComplete()
        {

        }

    }
}
