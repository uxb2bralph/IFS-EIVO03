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
using InvoiceClient.Agent.MIGHelper;
using InvoiceClient.Agent.CsvRequestHelper;

namespace InvoiceClient.TransferManagement
{
    public class POSInvoiceTransferManager : ITransferManager
    {
        private InvoiceWatcher _SellerInvoiceWatcher;
        private InvoiceWatcher _B2BSellerInvoiceWatcher;
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
        private InvoiceWatcher _C0401Watcher;
        private InvoiceWatcher _C0501Watcher;
        private InvoiceWatcher _A0401Watcher;
        private InvoiceWatcher _A0501Watcher;



        public void EnableAll(String fullPath)
        {
            _SellerInvoiceWatcher = new InvoiceWatcherV2(POSReady._Settings.SellerInvoice)
            {
                PreferredProcessType = Model.Locale.Naming.InvoiceProcessType.C0401,
            };
            _SellerInvoiceWatcher.StartUp();

            _B2BSellerInvoiceWatcher = new InvoiceWatcherV2(POSReady._Settings.B2BSellerInvoice)
            {
                PreferredProcessType = Model.Locale.Naming.InvoiceProcessType.A0401,
            };
            _B2BSellerInvoiceWatcher.StartUp();

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

            _C0401Watcher = new CsvC0401RequestWatcher(Path.Combine(fullPath, POSReady._Settings.C0401))
            {
                ResponsePath = POSReady._Settings.SellerInvoice,
                PreparedPrintPath = POSReady._Settings.PreparedInvoice,
            };
            _C0401Watcher.StartUp();

            _A0401Watcher = new CsvInvoiceRequestWatcher(Path.Combine(fullPath, POSReady._Settings.A0401))
            {
                ResponsePath = POSReady._Settings.B2BSellerInvoice,
                PreparedPrintPath = POSReady._Settings.PreparedInvoice,
            };
            _A0401Watcher.StartUp();

            _C0501Watcher = new CsvInvoiceCancellationRequestWatcher(Path.Combine(fullPath, POSReady._Settings.C0501));
            _C0501Watcher.StartUp();

            _A0501Watcher = new CsvInvoiceCancellationRequestWatcher(Path.Combine(fullPath, POSReady._Settings.A0501));
            _A0501Watcher.StartUp();

            _SellerInvoiceWatcher.InitializeDependency(_PreInvoiceWatcher);
            _CancellationWatcher.InitializeDependency(_SellerInvoiceWatcher);
            _AllowanceCancellationWatcher.InitializeDependency( _AllowanceWatcher);
            _C0501Watcher.InitializeDependency(_C0401Watcher);
            _A0501Watcher.InitializeDependency(_A0401Watcher);
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
            _B2BSellerInvoiceWatcher?.Dispose();
            _CancellationWatcher?.Dispose();
            _PreInvoiceWatcher?.Dispose();
            _AllowanceWatcher?.Dispose();
            _AllowanceCancellationWatcher?.Dispose();
            _C0401Watcher?.Dispose();
            _A0401Watcher?.Dispose();
            _C0501Watcher?.Dispose();
            _A0501Watcher?.Dispose();
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
            if (_B2BSellerInvoiceWatcher != null)
                sb.Append(_B2BSellerInvoiceWatcher.ReportError());
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
            if (_C0401Watcher != null)
                sb.Append(_C0401Watcher.ReportError());
            if (_A0401Watcher != null)
                sb.Append(_A0401Watcher.ReportError());
            if (_C0501Watcher != null)
                sb.Append(_C0501Watcher.ReportError());
            if (_A0501Watcher != null)
                sb.Append(_A0501Watcher.ReportError());
            return sb.ToString();

        }

        public void SetRetry()
        {
            _PreparedInvoiceWatcher.Retry();
            _BlindReturnWatcher.Retry();
            _ZeroAmountWatcher.Retry();
            _ReplacementWatcher.Retry();
            _SellerInvoiceWatcher.Retry();
            _B2BSellerInvoiceWatcher.Retry();
            _CancellationWatcher.Retry();
            _PreInvoiceWatcher.Retry();
            _AllowanceWatcher.Retry();
            _AllowanceCancellationWatcher.Retry();
            _PrintInvoiceWatcher?.Retry();
            _PreparedAllowanceWatcher.Retry();
            _C0401Watcher.Retry();
            _A0401Watcher.Retry();
            _C0501Watcher.Retry();
            _A0501Watcher.Retry();
        }



        public Type UIConfigType
        {
            get { return typeof(InvoiceClient.MainContent.POSChannelConfig); }
        }
    }
}
