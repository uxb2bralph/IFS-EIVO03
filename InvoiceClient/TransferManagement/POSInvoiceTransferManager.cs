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
        private InvoiceWatcher _PreparedInvoiceWatcher;
        private InvoiceWatcher _PreInvoiceWatcher;
        private InvoiceWatcher _PrintInvoiceWatcher;
        private InvoiceCancellationWatcher _CancellationWatcher;
        private AllowanceWatcher _AllowanceWatcher;
        private AllowanceCancellationWatcher _AllowanceCancellationWatcher;
        private InvoiceWatcher _BlindReturnWatcher;
        private InvoiceWatcher _PreparedAllowanceWatcher;
        private InvoiceWatcher _ReplacementWatcher;
        private InvoiceWatcher _ZeroAmountWatcher;


        public void EnableAll(String fullPath)
        {
            _SellerInvoiceWatcher = new InvoiceWatcherV2(POSReady._Settings.SellerInvoice);
            _SellerInvoiceWatcher.StartUp();

            _PreparedInvoiceWatcher = new PreparedInvoiceWatcher(POSReady._Settings.PreparedInvoice);
            _PreparedInvoiceWatcher.StartUp();

            _BlindReturnWatcher = new PreparedInvoiceWatcher(POSReady._Settings.BlindReturn)
            {
                ConvertPrintFormUrl = POSReady._Settings.PrintBlindReturn,
            };
            _BlindReturnWatcher.StartUp();

            _ReplacementWatcher = new PreparedInvoiceWatcher(POSReady._Settings.Replacement)
            {
                ConvertPrintFormUrl = POSReady._Settings.PrintReplacement,
            };
            _ReplacementWatcher.StartUp();

            _ZeroAmountWatcher = new PreparedInvoiceWatcher(POSReady._Settings.ZeroAmount)
            {
                ConvertPrintFormUrl = POSReady._Settings.PrintZeroAmount,
            };
            _ZeroAmountWatcher.StartUp();

            _PreparedAllowanceWatcher = new PreparedAllowanceWatcher(POSReady._Settings.PreparedAllowance)
            {
                ConvertPrintFormUrl = POSReady._Settings.PrintD0401,
            };
            _PreparedAllowanceWatcher.StartUp();

            if (POSReady._Settings.UserPOSPrinter)
            {
                _PrintInvoiceWatcher = new POSPrintWatcher(POSReady._Settings.PrintInvoice);
                _PrintInvoiceWatcher.StartUp();
            }

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
            _PreparedInvoiceWatcher?.Dispose();
            _BlindReturnWatcher?.Dispose();
            _ZeroAmountWatcher?.Dispose();
            _ReplacementWatcher?.Dispose();
            _PreparedAllowanceWatcher?.Dispose();
            _PrintInvoiceWatcher?.Dispose();
            _SellerInvoiceWatcher?.Dispose();
            _CancellationWatcher?.Dispose();
            _PreInvoiceWatcher?.Dispose();
            _AllowanceWatcher?.Dispose();
            _AllowanceCancellationWatcher?.Dispose();

        }

        public String ReportError()
        {
            StringBuilder sb = new StringBuilder();
            if (_PreparedInvoiceWatcher != null)
                sb.Append(_PreparedInvoiceWatcher.ReportError());
            if (_BlindReturnWatcher != null)
                sb.Append(_BlindReturnWatcher.ReportError());
            if (_ZeroAmountWatcher != null)
                sb.Append(_ZeroAmountWatcher.ReportError());
            if (_ReplacementWatcher != null)
                sb.Append(_ReplacementWatcher.ReportError());
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
            if (_PrintInvoiceWatcher != null)
                sb.Append(_PrintInvoiceWatcher.ReportError());
            if (_PreparedAllowanceWatcher != null)
                sb.Append(_PreparedAllowanceWatcher.ReportError());

            return sb.ToString();

        }

        public void SetRetry()
        {
            _PreparedInvoiceWatcher.Retry();
            _BlindReturnWatcher.Retry();
            _ZeroAmountWatcher.Retry();
            _ReplacementWatcher.Retry();
            _SellerInvoiceWatcher.Retry();
            _CancellationWatcher.Retry();
            _PreInvoiceWatcher.Retry();
            _AllowanceWatcher.Retry();
            _AllowanceCancellationWatcher.Retry();
            _PrintInvoiceWatcher?.Retry();
            _PreparedAllowanceWatcher.Retry();
        }



        public Type UIConfigType
        {
            get { return typeof(InvoiceClient.MainContent.POSChannelConfig); }
        }
    }
}
