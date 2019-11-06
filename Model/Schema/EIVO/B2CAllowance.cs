﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此原始程式碼由 xsd 版本=4.7.3081.0 自動產生。
// 
namespace Model.Schema.EIVO {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class AllowanceRoot {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Allowance")]
        public AllowanceRootAllowance[] Allowance;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class AllowanceRootAllowance {
        
        /// <remarks/>
        public string AllowanceNumber;
        
        /// <remarks/>
        public string AllowanceDate;
        
        /// <remarks/>
        public string GoogleId;
        
        /// <remarks/>
        public string SellerId;
        
        /// <remarks/>
        public string SellerName;
        
        /// <remarks/>
        public string BuyerId;
        
        /// <remarks/>
        public string BuyerName;
        
        /// <remarks/>
        public byte AllowanceType;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AllowanceItem")]
        public AllowanceRootAllowanceAllowanceItem[] AllowanceItem;
        
        /// <remarks/>
        public decimal TaxAmount;
        
        /// <remarks/>
        public decimal TotalAmount;
        
        /// <remarks/>
        public string Currency;
        
        /// <remarks/>
        public AllowanceRootAllowanceContact Contact;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> LineNo;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LineNoSpecified;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class AllowanceRootAllowanceAllowanceItem {
        
        /// <remarks/>
        public string OriginalInvoiceDate;
        
        /// <remarks/>
        public string OriginalInvoiceNumber;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<short> OriginalSequenceNumber;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OriginalSequenceNumberSpecified;
        
        /// <remarks/>
        public string Item;
        
        /// <remarks/>
        public string OriginalDescription;
        
        /// <remarks/>
        public decimal Quantity;
        
        /// <remarks/>
        public string Unit;
        
        /// <remarks/>
        public decimal UnitPrice;
        
        /// <remarks/>
        public decimal Amount;
        
        /// <remarks/>
        public decimal Tax;
        
        /// <remarks/>
        public short AllowanceSequenceNumber;
        
        /// <remarks/>
        public byte TaxType;
        
        /// <remarks/>
        public string Remark;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class AllowanceRootAllowanceContact {
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public string Address;
        
        /// <remarks/>
        public string TEL;
        
        /// <remarks/>
        public string Email;
    }
}
