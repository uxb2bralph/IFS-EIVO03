using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using InvoiceClient.Properties;
using Model.Schema.EIVO;
using InvoiceClient.Agent;
using Utility;
using Newtonsoft.Json;

namespace InvoiceClient.TransferManagement
{
    public class XlsxInvoiceTransferManager : ITransferManager
    {
        private InvoiceWatcher _InvoiceWatcher;
        private InvoiceWatcher _CancellationWatcher;
        private InvoiceWatcher _AllowanceWatcher;
        private InvoiceWatcher _AllowanceCancellationWatcher;

        LocalSettings _Settings;
        class LocalSettings
        {
            public String InvoiceRequestPath { get; set; } = "Invoice_Xlsx";
            public String VoidInvoiceRequestPath { get; set; } = "CancelInvoice_Xlsx";
            public String AllowanceRequestPath { get; set; } = "Allowance_Xlsx";
            public String VoidAllowanceRequestPath { get; set; } = "CancelAllowance_Xlsx";
        }

        public XlsxInvoiceTransferManager()
        {
            String filePath = Path.Combine(Logger.LogPath, "XlsxInvoiceTransferManager.json");
            if (File.Exists(filePath))
            {
                _Settings = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(filePath));
            }
            else
            {
                _Settings = new LocalSettings { };
                File.WriteAllText(filePath, JsonConvert.SerializeObject(_Settings));
            }
        }

        public void EnableAll(String fullPath)
        {
            _InvoiceWatcher = new XlsxInvoiceWatcher(Path.Combine(fullPath, _Settings.InvoiceRequestPath));
            _InvoiceWatcher.StartUp();

            _CancellationWatcher = new XlsxVoidInvoiceWatcher(Path.Combine(fullPath, _Settings.VoidInvoiceRequestPath));
            _CancellationWatcher.StartUp();

            _AllowanceWatcher = new XlsxAllowanceWatcher(Path.Combine(fullPath, _Settings.AllowanceRequestPath));
            _AllowanceWatcher.StartUp();

            _AllowanceCancellationWatcher = new XlsxVoidAllowanceWatcher(Path.Combine(fullPath, _Settings.VoidAllowanceRequestPath));
            _AllowanceCancellationWatcher.StartUp();

        }

        public void PauseAll()
        {
            if (_InvoiceWatcher != null)
            {
                _InvoiceWatcher.Dispose();
            }

            if (_CancellationWatcher != null)
            {
                _CancellationWatcher.Dispose();
            }
            if (_AllowanceWatcher != null)
            {
                _AllowanceWatcher.Dispose();
            }
            if (_AllowanceCancellationWatcher != null)
            {
                _AllowanceCancellationWatcher.Dispose();
            }
        }

        public String ReportError()
        {
            StringBuilder sb = new StringBuilder();
            if (_InvoiceWatcher != null)
                sb.Append(_InvoiceWatcher.ReportError());
            if (_CancellationWatcher != null)
                sb.Append(_CancellationWatcher.ReportError());
            if (_AllowanceWatcher != null)
                sb.Append(_AllowanceWatcher.ReportError());
            if (_AllowanceCancellationWatcher != null)
                sb.Append(_AllowanceCancellationWatcher.ReportError());

            return sb.ToString();

        }

        public void SetRetry()
        {
            _InvoiceWatcher.Retry();
            _CancellationWatcher.Retry();
            _AllowanceWatcher.Retry();
            _AllowanceCancellationWatcher.Retry();
        }



        public Type UIConfigType
        {
            get { return typeof(InvoiceClient.MainContent.XlsxInvoiceCenterConfig); }
        }
    }
}
