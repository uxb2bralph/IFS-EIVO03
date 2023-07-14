using ApplicationResource;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace eIVOGo.Properties
{
    public partial class AppSettings : Uxnet.Com.Properties.AppSettings
    {
        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings() : base()
        {

        }

        static AppSettings _default;
        public new static AppSettings Default
        {
            get
            {
                return _default;
            }
        }

        public MonthlyBilling Billing { get; set; } = new MonthlyBilling { };
        public InvoiceNumberApplySetting InvoiceNumberApplySetting = new InvoiceNumberApplySetting();
        public IEnumerable<InvoiceNumberApplyWordSetting> InvoiceNumberApplyWordSetting { get; set; } = new List<InvoiceNumberApplyWordSetting>() { };
    }

    public class InvoiceNumberApplySetting
    {
        public string BasePath { get; set; }
        public string BackupFolder { get; set; }
        public string WordTemplateFolder { get; set; }
        public string GetWordTemplateFolder(string wordFileName) {
            return Path.Combine(AppSettings.AppRoot, WordTemplateFolder, wordFileName);
        }

        public string GetBackupPath() { return Path.Combine(BasePath, BackupFolder); }
        public string JsonFileNameFormat { get; set; }
        public string GetJsonFileName(string businessId) { return string.Format(JsonFileNameFormat, businessId); }
        public string GetJsonFileFullPath(string businessId) { return Path.Combine(BasePath, GetJsonFileName(businessId)); }

        public string ZipFileNameFormat { get; set; }
        public string GetZipFileName(string businessId) { return string.Format(ZipFileNameFormat, businessId); }
        public string GetZipFileFullPath(string businessId) { return Path.Combine(BasePath, GetZipFileName(businessId)); }
        public string JsonFileNameFormatReg { get; set; }
        public bool NotifyEnable { get; set; }
        public string NotifyMailTo { get; set; }
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