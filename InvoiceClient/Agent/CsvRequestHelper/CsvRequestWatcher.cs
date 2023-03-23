﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.Locale;
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Utility;
using CsvHelper;

namespace InvoiceClient.Agent.CsvRequestHelper
{
    public class CsvRequestWatcher : InvoiceWatcher
    {
        public CsvRequestWatcher(String fullPath)
            : base(fullPath)
        {

        }

        protected override void prepareStorePath(string fullPath)
        {
            _failedTxnPath = fullPath + "(Failure)";
            _failedTxnPath.CheckStoredPath();

            if (__FailedTxnPath != null)
            {
                __FailedTxnPath.Add(_failedTxnPath);
            }

            _inProgressPath = Path.Combine(Logger.LogPath, Path.GetFileName(fullPath), $"{Process.GetCurrentProcess().Id}");
            _inProgressPath.CheckStoredPath();
        }

        protected List<String[]> DataItems { get; set; }

        protected List<String[]> ParseCsv(String csvFile)
        {
            List<String[]> items = new List<String[]>();
            using (StreamReader sr = new StreamReader(csvFile, Encoding.GetEncoding(Settings.Default.CsvEncoding)))
            {

                sr.ReadLine();
                int lineIdx = 0;
                using (CsvParser parser = new CsvParser(sr, new CsvHelper.Configuration.Configuration() { TrimOptions = CsvHelper.Configuration.TrimOptions.Trim }, leaveOpen: false))
                {
                    String[] column;
                    while ((column = parser.Read()) != null)
                    {
                        lineIdx++;
                        items.Add(column);
                    }
                }
            }
            return items;
        }

        protected override void processFile(string invFile)
        {
            DataItems = null;
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
                Logger.Error(ex);
                return;
            }

            try
            {
                DataItems = ParseCsv(fullPath);
                storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
            }
        }


    }
}
