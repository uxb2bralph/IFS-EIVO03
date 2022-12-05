﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Locale;
using Newtonsoft.Json;
using Utility;

namespace Model.DataEntity
{
    public partial class DataDefinition
    {
    }

    public partial class InvoiceDeliveryTracking
    {
        public int? DuplicateCount { get; set; }
        public bool MergedItem { get; set; }
    }

    public partial class InvoiceItem
    {
        public int? PackageID { get; set; }
    }


    public class NotifyToProcessID
    {
        public int? MailToID { get; set; }
        public Organization Seller { get; set; }
        public String itemNO { get; set; }
        public int? DocID { get; set; }
        public String Subject { get; set; }
        public String MailTo { get; set; }
        public bool? AppendAttachment { get; set; }
    }

    public class NotifyMailInfo
    {
        public bool? isMail { get; set; }
        public InvoiceItem InvoiceItem { get; set; }
    }

    public class InvoiceEntity
    {
        public InvoiceItem MainItem { get; set; }
        public List<InvoiceProduct> ItemDetails { get; set; }
        public Naming.UploadStatusDefinition? Status { get; set; }
        public String Reason { get; set; }
    }

    public partial class InvoiceAmountType
    {
        public String TaxTypeString => TaxType == (byte)Naming.TaxTypeDefinition.免稅
                ? ""
                : TaxType == (byte)Naming.TaxTypeDefinition.零稅率
                    ? "TZ"
                    : "TX";
    }

    public partial class InvoiceAllowanceItem
    {
        public String TaxTypeString => TaxType == (byte)Naming.TaxTypeDefinition.免稅
                ? ""
                : TaxType == (byte)Naming.TaxTypeDefinition.零稅率
                    ? "TZ"
                    : "TX";
    }

    public partial class CustomSmtpHost
    {
        public enum StatusType
        {
            Disabled = 0,
            Enabled = 1,
        }
    }

    public partial class OrganizationCustomSetting
    {
        private OrganizationCustomSettingsModel _settings;
        public OrganizationCustomSettingsModel Settings
        {
            get
            {
                if (_settings == null)
                {
                    if (SettingData != null)
                    {
                        _settings = JsonConvert.DeserializeObject<OrganizationCustomSettingsModel>(SettingData);
                    }
                    else
                    {
                        _settings = new OrganizationCustomSettingsModel { };
                        Accept();
                    }
                }
                return _settings;
            }
        }

        public void Accept()
        {
            SettingData = _settings?.JsonStringify();
        }
    }

    public class OrganizationCustomSettingsModel
    {
        public String C0401POSView { get; set; }
    }

    public partial class CategoryDefinition
    {
        public enum CategoryEnum
        {
            發票開立營業人 = 15,	                                //	2	賣方	     sketch_seller.gif
            相對營業人 = 16,	            //	3	買方	     sketch_buyer.gif
            GoogleTaiwan = 17,                            //  4  
            集團成員 = 18,
            營業人發票自動配號 = 19,
            經銷商 = 20,
            境外電商 = 23,
            主機構 = 24,
        }
    }

    public partial class ExtraBillingItem 
    {
        public enum BillingTypeEnum
        {
            PayOnce = 1,
            PayContinuous = 2,
        }
    }

    public partial class InvoiceIssuerAgent
    {
        public enum Relationship
        {
            MasterBranch = 1,
        }
    }

    public partial class Organization
    {
        public IEnumerable<InvoiceIssuerAgent> BranchRelation 
        {
            get => this.InvoiceIssuerAgent.Where(a => a.RelationType == (int)Model.DataEntity.InvoiceIssuerAgent.Relationship.MasterBranch);
        } 
    }
}
