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
        public string ApplyFileBase { get; set; }
        public string GetApplyFileBase()
        {
            return Path.Combine(AppSettings.AppRoot, ApplyFileBase);
        }
        public string ApplyFileBackupFolder { get; set; }
        public string WordTemplateFolder { get; set; }
        public string GetWordTemplateFilePath(string wordFileName) {
            return Path.Combine(AppSettings.AppRoot, WordTemplateFolder, wordFileName);
        }

        public string GetApplyFileBackupFolder() { return Path.Combine(GetApplyFileBase(), ApplyFileBackupFolder); }
        public string ApplyFileNameFormat { get; set; }
        public string GetApplyFileName(string businessId) { return string.Format(ApplyFileNameFormat, businessId); }
        public string GetApplyFilePath(string businessId) { return Path.Combine(GetApplyFileBase(), GetApplyFileName(businessId)); }

        public string ZipFileNameFormat { get; set; }
        public string GetZipFileName(string businessId) { return string.Format(ZipFileNameFormat, businessId); }
        public string GetZipFilePath(string businessId) { return Path.Combine(GetApplyFileBase(), GetZipFileName(businessId)); }
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