using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using InvoiceClient.Properties;
using Model.Schema.EIVO;
using InvoiceClient.Agent;
using InvoiceClient.Agent.POSHelper;

namespace InvoiceClient.TransferManagement
{
    public class POSInvoiceTransferManager : ITransferManager
    {
        private InvoiceWatcher _SellerInvoiceWatcher;
        //private InvoiceWatcher _PreparedInvoiceWatcher;
        private InvoiceWatcher _PreInvoiceWatcher;
        private InvoiceCancellationWatcher _CancellationWatcher;
        private AllowanceWatcher _AllowanceWatcher;
        private AllowanceCancellationWatcher _AllowanceCancellationWatcher;


        public void EnableAll(String fullPath)
        {
            _SellerInvoiceWatcher = new InvoiceWatcherV2(POSReady._Settings.SellerInvoice);
            _SellerInvoiceWatcher.StartUp();

            //_PreparedInvoiceWatcher = new PreparedInvoiceWatcher(POSReady._Settings.PreparedInvoice);
            //_PreparedInvoiceWatcher.StartUp();

            _PreInvoiceWatcher = new POSInvoiceWatcher(Path.Combine(fullPath, Settings.Default.UploadPreInvoiceFolder));
            _PreInvoiceWatcher.StartUp();

            _CancellationWatcher = new InvoiceCancellationWatcherV2(Path.Combine(fullPath, Settings.Default.UploadInvoiceCancellationFolder));
            _CancellationWatcher.StartUp();

            _AllowanceWatcher = new POSAllowanceWatcher(Path.Combine(fullPath, Settings.Default.UploadAllowanceFolder));
            _AllowanceWatcher.StartUp();

            _AllowanceCancellationWatcher = new AllowanceCancellationWatcherV2(Path.Combine(fullPath, Settings.Default.UploadAllowanceCancellationFolder));
            _AllowanceCancellationWatcher.StartUp();

            _SellerInvoiceWatcher.InitializeDependency(_PreInvoiceWatcher);
            _CancellationWatcher.InitializeDependency(_SellerInvoiceWatcher);
            _AllowanceCancellationWatcher.InitializeDependency( _AllowanceWatcher);
        }

        public void PauseAll()
        {
            //if (_PreparedInvoiceWatcher != null)
            //{
            //    _PreparedInvoiceWatcher.Dispose();
            //}

            if (_SellerInvoiceWatcher != null)
            {
                _SellerInvoiceWatcher.Dispose();
            }
            if (_CancellationWatcher != null)
            {
                _CancellationWatcher.Dispose();
            }
            if (_PreInvoiceWatcher != null)
            {
                _PreInvoiceWatcher.Dispose();
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
            //if (_PreparedInvoiceWatcher != null)
            //    sb.Append(_PreparedInvoiceWatcher.ReportError());
            if (_SellerInvoiceWatcher != null)
                sb.Append(_SellerInvoiceWatcher.ReportError());
            if (_CancellationWatcher != null)
                sb.Append(_CancellationWatcher.ReportError());
            if (_PreInvoiceWatcher != null)
                sb.Append(_PreInvoiceWatcher.ReportError());
            if (_AllowanceWatcher != null)
                sb.Append(_AllowanceWatcher.ReportError());
            if (_AllowanceCancellationWatcher != null)
                sb.Append(_AllowanceCancellationWatcher.ReportError());

            return sb.ToString();

        }

        public void SetRetry()
        {
            //_PreparedInvoiceWatcher.Retry();
            _SellerInvoiceWatcher.Retry();
            _CancellationWatcher.Retry();
            _PreInvoiceWatcher.Retry();
            _AllowanceWatcher.Retry();
            _AllowanceCancellationWatcher.Retry();

        }



        public Type UIConfigType
        {
            get { return typeof(InvoiceClient.MainContent.POSChannelConfig); }
        }
    }
}
