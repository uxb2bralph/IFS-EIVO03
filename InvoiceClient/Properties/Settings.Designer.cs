﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvoiceClient.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("UXSigner")]
        public string SignerSubjectName {
            get {
                return ((string)(this["SignerSubjectName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SignerCspName {
            get {
                return ((string)(this["SignerCspName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SignerKeyPassword {
            get {
                return ((string)(this["SignerKeyPassword"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Invoice")]
        public string UploadInvoiceFolder {
            get {
                return ((string)(this["UploadInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CancelInvoice")]
        public string UploadInvoiceCancellationFolder {
            get {
                return ((string)(this["UploadInvoiceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int AutoWelfareInterval {
            get {
                return ((int)(this["AutoWelfareInterval"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SWA")]
        public string WelfareInfoFolder {
            get {
                return ((string)(this["WelfareInfoFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsAutoWelfare {
            get {
                return ((bool)(this["IsAutoWelfare"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("utf-8")]
        public string OutputEncoding {
            get {
                return ((string)(this["OutputEncoding"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("傳送大平台資料")]
        public string DownloadDataFolder {
            get {
                return ((string)(this["DownloadDataFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsAutoInvService {
            get {
                return ((bool)(this["IsAutoInvService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int AutoInvServiceInterval {
            get {
                return ((int)(this["AutoInvServiceInterval"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Allowance")]
        public string UploadAllowanceFolder {
            get {
                return ((string)(this["UploadAllowanceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CancelAllowance")]
        public string UploadAllowanceCancellationFolder {
            get {
                return ((string)(this["UploadAllowanceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("傳送大平台資料\\發票")]
        public string DownloadInvoiceFolder {
            get {
                return ((string)(this["DownloadInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("傳送大平台資料\\作廢發票")]
        public string DownloadInvoiceCancellationFolder {
            get {
                return ((string)(this["DownloadInvoiceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("傳送大平台資料\\折讓")]
        public string DownloadAllowanceFolder {
            get {
                return ((string)(this["DownloadAllowanceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("傳送大平台資料\\作廢折讓")]
        public string DownloadAllowanceCancellationFolder {
            get {
                return ((string)(this["DownloadAllowanceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BonusInvoice")]
        public string DownloadWinningFolder {
            get {
                return ((string)(this["DownloadWinningFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OutputEncodingWithoutBOM {
            get {
                return ((bool)(this["OutputEncodingWithoutBOM"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DownloadDataInAbsolutePath {
            get {
                return ((bool)(this["DownloadDataInAbsolutePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Electronic Invoice Transmission Service-Store Client")]
        public string AppTitle {
            get {
                return ((string)(this["AppTitle"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Invoice_csv")]
        public string UploadCsvInvoiceFolder {
            get {
                return ((string)(this["UploadCsvInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CancelInvoice_csv")]
        public string UploadCsvInvoiceCancellationFolder {
            get {
                return ((string)(this["UploadCsvInvoiceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoiceMap")]
        public string DownloadInvoiceMapping {
            get {
                return ((string)(this["DownloadInvoiceMapping"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Big5")]
        public string CsvEncoding {
            get {
                return ((string)(this["CsvEncoding"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("UXB2B Certificate Center.cer")]
        public string RootCA {
            get {
                return ((string)(this["RootCA"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoiceNoAssignment")]
        public string UploadBranchTrackFolder {
            get {
                return ((string)(this["UploadBranchTrackFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SellerInvoice")]
        public string UploadSellerInvoiceFolder {
            get {
                return ((string)(this["UploadSellerInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PreInvoice")]
        public string UploadPreInvoiceFolder {
            get {
                return ((string)(this["UploadPreInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Attachment")]
        public string UploadAttachmentFolder {
            get {
                return ((string)(this["UploadAttachmentFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoicePDF")]
        public string DownloadSaleInvoiceFolder {
            get {
                return ((string)(this["DownloadSaleInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoiceMailTracking")]
        public string DownloadInvoiceMailTracking {
            get {
                return ((string)(this["DownloadInvoiceMailTracking"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ReturnedMail")]
        public string DownloadInvoiceReturnedMail {
            get {
                return ((string)(this["DownloadInvoiceReturnedMail"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SellerReceiptNo {
            get {
                return ((string)(this["SellerReceiptNo"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\UXB2B_EIVO")]
        public string InvoiceTxnPath {
            get {
                return ((string)(this["InvoiceTxnPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n                    InvoiceClient.MainContent.SystemConfigTab, InvoiceClient, V" +
            "ersion=1.0.0.0, Culture=neutral, PublicKeyToken=null;\r\n                ")]
        public string MainTabs {
            get {
                return ((string)(this["MainTabs"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Enterprise")]
        public string UploadInvoiceEnterprise {
            get {
                return ((string)(this["UploadInvoiceEnterprise"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoiceTrackCode")]
        public string TrackCodeFolder {
            get {
                return ((string)(this["TrackCodeFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("VacantInvoiceNo")]
        public string VacantInvoiceNoFolder {
            get {
                return ((string)(this["VacantInvoiceNoFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EIVO03ClientService(Product)")]
        public string ServiceName {
            get {
                return ((string)(this["ServiceName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ResponseUpload {
            get {
                return ((bool)(this["ResponseUpload"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30000")]
        public int PGPWaitingForExitInMilliSeconds {
            get {
                return ((int)(this["PGPWaitingForExitInMilliSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ClientID {
            get {
                return ((string)(this["ClientID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2B相對營業人_csv")]
        public string UploadCsvBuyerFolder {
            get {
                return ((string)(this["UploadCsvBuyerFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://eceivo.uxifs.com/eivo03/Published/UploadAttachmentForGoogle.ashx")]
        public string UploadAttachment {
            get {
                return ((string)(this["UploadAttachment"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ActivationKey {
            get {
                return ((string)(this["ActivationKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Allowance_csv")]
        public string UploadCsvAllowanceFolder {
            get {
                return ((string)(this["UploadCsvAllowanceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CancelAllowance_csv")]
        public string UploadCsvAllowanceCancellationFolder {
            get {
                return ((string)(this["UploadCsvAllowanceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoRetry {
            get {
                return ((bool)(this["AutoRetry"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("300000")]
        public int WS_TimeoutInMilliSeconds {
            get {
                return ((int)(this["WS_TimeoutInMilliSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableFailedUploadAlert {
            get {
                return ((bool)(this["EnableFailedUploadAlert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("InvoiceFull_csv")]
        public string UploadCustomerCsvInvoiceFolder {
            get {
                return ((string)(this["UploadCustomerCsvInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2BSellerInvoice")]
        public string B2BUploadInvoiceFolder {
            get {
                return ((string)(this["B2BUploadInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BuyerInvoice")]
        public string B2BUploadBuyerInvoiceFolder {
            get {
                return ((string)(this["B2BUploadBuyerInvoiceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2BAllowance")]
        public string B2BUploadAllowanceFolder {
            get {
                return ((string)(this["B2BUploadAllowanceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2BCancelInvoice")]
        public string B2BUploadInvoiceCancellationFolder {
            get {
                return ((string)(this["B2BUploadInvoiceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2BCancelAllowance")]
        public string B2BUploadAllowanceCancellationFolder {
            get {
                return ((string)(this["B2BUploadAllowanceCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B2B相對營業人")]
        public string B2BCounterpartBusinessFolder {
            get {
                return ((string)(this["B2BCounterpartBusinessFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Receipt")]
        public string B2BUploadReceiptFolder {
            get {
                return ((string)(this["B2BUploadReceiptFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CancelReceipt")]
        public string B2BUploadReceiptCancellationFolder {
            get {
                return ((string)(this["B2BUploadReceiptCancellationFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableB2BAutoReceiving {
            get {
                return ((bool)(this["EnableB2BAutoReceiving"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ProcessCount {
            get {
                return ((int)(this["ProcessCount"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ProcessArrayCount {
            get {
                return ((int)(this["ProcessArrayCount"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ProcessArrayIndex {
            get {
                return ((int)(this["ProcessArrayIndex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RetryOnConnectException {
            get {
                return ((bool)(this["RetryOnConnectException"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int FileWatcherProcessCount {
            get {
                return ((int)(this["FileWatcherProcessCount"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RetryCancellationWhenInvoiceNotFound {
            get {
                return ((bool)(this["RetryCancellationWhenInvoiceNotFound"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("電子發票用戶端傳輸服務(EIVO03)")]
        public string DisplayName {
            get {
                return ((string)(this["DisplayName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("taiwan_uxb2b_scanned_gui_pdf_")]
        public string InvoicePDFZipPrefix {
            get {
                return ((string)(this["InvoicePDFZipPrefix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ZipInvoice")]
        public string ZipInvoice {
            get {
                return ((string)(this["ZipInvoice"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("E:\\UXB2B_EIVO_GOOGLE\\ZipInvoice")]
        public string PDFGeneratorOutput {
            get {
                return ((string)(this["PDFGeneratorOutput"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ClearTxnPath {
            get {
                return ((bool)(this["ClearTxnPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
                  InvoiceClient.Agent.VacantInvoiceNoInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.InvoiceMappingInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.B2BInvoiceInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.InvoiceMailTrackingInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.InvoicePDFInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.AllowancePDFGenerator, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                  InvoiceClient.Agent.InvoiceServerInspector, InvoiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;
                ")]
        public string ServerInspector {
            get {
                return ((string)(this["ServerInspector"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int PackerCycleDelayInSeconds {
            get {
                return ((int)(this["PackerCycleDelayInSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:2598/DataView/ConvertDataToAllowance")]
        public string ConvertDataToAllowance {
            get {
                return ((string)(this["ConvertDataToAllowance"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1024")]
        public int MaxFileCountInPDFZip {
            get {
                return ((int)(this["MaxFileCountInPDFZip"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("300")]
        public int WaitForInvoicePDFInSeconds {
            get {
                return ((int)(this["WaitForInvoicePDFInSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int MaxWaitingTurns {
            get {
                return ((int)(this["MaxWaitingTurns"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en-US")]
        public string AppCulture {
            get {
                return ((string)(this["AppCulture"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:2598/Published/eInvoiceService.asmx")]
        public string InvoiceClient_WS_Invoice_eInvoiceService {
            get {
                return ((string)(this["InvoiceClient_WS_Invoice_eInvoiceService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n                InvoiceClient.TransferManagement.XlsxInvoiceTransferManager, In" +
            "voiceClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;\r\n           " +
            "   ")]
        public string TransferManager {
            get {
                return ((string)(this["TransferManager"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int WatcherProcessDelayInSeconds {
            get {
                return ((int)(this["WatcherProcessDelayInSeconds"]));
            }
        }
    }
}
