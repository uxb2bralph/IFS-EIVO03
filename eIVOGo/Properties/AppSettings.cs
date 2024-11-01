﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Utility;
using Uxnet.ToolAdapter.Properties;

namespace eIVOGo.Properties
{
    public partial class AppSettings : AppSettingsBase
    {
        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings() : base()
        {

        }

        static AppSettings _default;
        public static AppSettings Default
        {
            get
            {
                return _default;
            }
        }

        public static void Reload()
        {
            Reload<AppSettings>(ref _default, typeof(AppSettings).Namespace);
        }

        public MonthlyBilling Billing { get; set; } = new MonthlyBilling { };
        public String InvoiceCarrierProviderID { get; set; } = "70762419";
        public String GovCarrierVerrificationUrl { get; set; } = "https://wwwtest.einvoice.nat.gov.tw/APCONSUMER/BTC101I/";
        public String GovCarrierLocalVerrificationUrl { get; set; } = "https://wwwtest.einvoice.nat.gov.tw/APCONSUMER/BTC103I/";
        public String GovApiKeyBase64 { get; set; } = "99Xzi1nA0nn7TdTBYsylZg==";
        public decimal Zoom { get; set; } = 100;
        public InvoiceNumberApplySetting InvoiceNumberApplySetting = new InvoiceNumberApplySetting();
        public IEnumerable<InvoiceNumberApplyWordSetting> InvoiceNumberApplyWordSetting { get; set; }
    }

    public class InvoiceNumberApplySetting
    {
        public string ApplyFileBaseFolder { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNumberApply").CheckStoredPath();
        public string WordTemplateFolder { get; set; } = Path.Combine(AppSettings.AppRoot, "resource", "InvoiceNumberApply");
        public string GetWordTemplateFilePath(string wordFileName)
        {
            return Path.Combine(AppSettings.AppRoot, WordTemplateFolder, wordFileName);
        }

        public string ApplyFileBackupFolder { get; set; } = Path.Combine(Logger.LogPath, "history").CheckStoredPath();
        public string ApplyFileNameFormat { get; set; } = "{0}.json";
        public string GetApplyFileName(string businessId) { return string.Format(ApplyFileNameFormat, businessId); }
        public string GetApplyFilePath(string businessId) { return Path.Combine(ApplyFileBaseFolder, GetApplyFileName(businessId)); }

        public string ZipFileNameFormat { get; set; } = "{0}.zip";
        public string GetZipFileName(string businessId) { return string.Format(ZipFileNameFormat, businessId); }
        public string GetZipFilePath(string businessId) { return Path.Combine(ApplyFileBaseFolder, GetZipFileName(businessId)); }
        public string ApplyFileNameFormatReg { get; set; }
        public bool NotifyEnable { get; set; }
        public IEnumerable<InvoiceNumberApplySysSupplier> SysSupplier { get; set; }
        public IEnumerable<InvoiceNumberApplyPaperTestSet> PaperTestSet { get; set; }
    }

    public class InvoiceNumberApplyWordSetting
    {
        public string ID { get; set; }
        public string OutputName { get; set; }
        public string TemplateFileName { get; set; }

        public override string ToString()
        {
            return
                "ID=" + ID +
                "OutputName=" + OutputName +
                "TemplateFileName=" + TemplateFileName;
        }
    }

    public class InvoiceNumberApplyPaperTestSet
    {
        public string ID { get; set; }
        public string TestBusinessName { get; set; }
        public string SubmitBusinessName { get; set; }
        public string ReportNo { get; set; }
        public string PaperName { get; set; }
        public string PaperNo { get; set; }

        public string ReportDate { get; set; }
        public override string ToString()
        {
            return
                "ID=" + ID +
                "TestBusinessName=" + TestBusinessName +
                "SubmitBusinessName=" + SubmitBusinessName +
                "ReportNo=" + ReportNo +
                "PaperName=" + PaperName +
                 "PaperNo=" + PaperNo +
                 "ReportDate=" + ReportDate;
        }
    }

    public class InvoiceNumberApplySysSupplier
    {
        public string ID { get; set; }
        public string BusinessID { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            return
                "ID=" + ID +
                "BusinessID=" + BusinessID +
                "No=" + No +
                "Name=" + Name +
                "Version=" + Version;
        }
    }

    public class MonthlyBilling
    {
        public int BillingDay { get; set; } = 5;
    }


}