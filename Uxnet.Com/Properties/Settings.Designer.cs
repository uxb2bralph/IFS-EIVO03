﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uxnet.Com.Properties
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Microsoft XPS Document Writer")]
        public string XpsPrinterName
        {
            get
            {
                return ((string)(this["XpsPrinterName"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("列印")]
        public string WndPrnTitle
        {
            get
            {
                return ((string)(this["WndPrnTitle"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("列印(&P)")]
        public string BtnPrnTitle
        {
            get
            {
                return ((string)(this["BtnPrnTitle"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("另存檔案為")]
        public string WndSaveAsTitle
        {
            get
            {
                return ((string)(this["WndSaveAsTitle"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("儲存(&S)")]
        public string BtnSaveTitle
        {
            get
            {
                return ((string)(this["BtnSaveTitle"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Bullzip PDF Printer")]
        public string PdfPrinterName
        {
            get
            {
                return ((string)(this["PdfPrinterName"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SqlLog
        {
            get
            {
                return ((bool)(this["SqlLog"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://hinet01:8880/DocumentService/DocumentCreator.asmx")]
        public string Uxnet_Com_WS_DocumentService_DocumentCreator
        {
            get
            {
                return ((string)(this["Uxnet_Com_WS_DocumentService_DocumentCreator"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseMessageCenter
        {
            get
            {
                return ((bool)(this["UseMessageCenter"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://tpit-matrix/CDSAR/published/UploadFile.ashx")]
        public string MessageUploadUrl
        {
            get
            {
                return ((string)(this["MessageUploadUrl"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://tpit-matrix/CDSAR/published/SendMessage.asmx")]
        public string Uxnet_Com_WS_MessageService_SendMessage
        {
            get
            {
                return ((string)(this["Uxnet_Com_WS_MessageService_SendMessage"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LogPath
        {
            get
            {
                return ((string)(this["LogPath"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("masterDB")]
        public string DBConnectionStringName
        {
            get
            {
                return ((string)(this["DBConnectionStringName"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\UXB2B_EIVO\\PDFQueue")]
        public string PDFWorkingQueue
        {
            get
            {
                return ((string)(this["PDFWorkingQueue"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\Ralph\\AppData\\Local\\Temp\\PdfPrinter.pdf")]
        public string PdfOutput
        {
            get
            {
                return ((string)(this["PdfOutput"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30000")]
        public int MaxWaitingInterval
        {
            get
            {
                return ((int)(this["MaxWaitingInterval"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UsePDFPrinterService
        {
            get
            {
                return ((bool)(this["UsePDFPrinterService"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IgnoreCertificateRevoked
        {
            get
            {
                return ((bool)(this["IgnoreCertificateRevoked"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Uxnet.Com.Security.UseCrypto.dsPKCS7,Uxnet.Com, Version=1.0.0.0, Culture=neutral," +
            " PublicKeyToken=null")]
        public string PKCS7LogType
        {
            get
            {
                return ((string)(this["PKCS7LogType"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Uxnet.Com.Helper.DefaultTools.PdfUtility,Uxnet.Com, Version=1.0.0.0, Culture=neut" +
            "ral, PublicKeyToken=null")]
        public string IPdfUtilityImpl
        {
            get
            {
                return ((string)(this["IPdfUtilityImpl"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("JobScheduler.xml")]
        public string JobSchedulerFile
        {
            get
            {
                return ((string)(this["JobSchedulerFile"]));
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SqlLogIgnoreSelect
        {
            get
            {
                return ((bool)(this["SqlLogIgnoreSelect"]));
            }
        }
    }
}
