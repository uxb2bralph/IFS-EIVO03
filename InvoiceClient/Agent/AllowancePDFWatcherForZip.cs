using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceClient.Properties;
using Utility;

namespace InvoiceClient.Agent
{
    public class AllowancePDFWatcherForZip : InvoiceWatcher
    {
        private String _outFile;
        private List<String> _files;
        private int _waitCycle;

        public AllowancePDFWatcherForZip(String fullPath) : base(fullPath)
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

            _outFile = Path.Combine(Logger.LogDailyPath, $"A_{DateTime.Now.Ticks}.zip");

            using (var zipOut = System.IO.File.Create(_outFile))
            {
                using(ZipArchive zip =new ZipArchive (zipOut,ZipArchiveMode.Create))
                {
                    for (int i=0,count=0;count<Settings.Default.MaxFileCountInPDFZip && i <files.Length;i++)
                    {
                        var item = files[i];
                        var fileName = Path.GetFileName(item);

                        var pdfName = fileName.Split('.').ToArray()[0].Split('_');

                        try
                        {
                            zip.CreateEntryFromFile(item, fileName);
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
                var zipName = $"{Settings.Default.AllowancePDFZipPrefix}{DateTime.Now:yyyyMMddHHmmssffff}-{_files.Count}.zip";              

                String moveFileName = Path.Combine(_ResponsedPath, zipName);

                File.Move(_outFile, moveFileName);

                foreach (var item in _files)
                {                    
                    storeFile(item, Path.Combine(Logger.LogDailyPath, Path.GetFileName(item)));
                }     
            }

        }


    }
}
