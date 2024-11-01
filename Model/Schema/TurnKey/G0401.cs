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
// 此原始程式碼由 xsd 版本=4.8.3928.0 自動產生。
// 
namespace Model.Schema.TurnKey.G0401 {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0", IsNullable=false)]
    public partial class Allowance {
        
        /// <remarks/>
        public Main Main;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ProductItem", IsNullable=false)]
        public DetailsProductItem[] Details;
        
        /// <remarks/>
        public Amount Amount;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public partial class Main {
        
        /// <remarks/>
        public string AllowanceNumber;
        
        /// <remarks/>
        public string AllowanceDate;
        
        /// <remarks/>
        public MainSeller Seller;
        
        /// <remarks/>
        public MainBuyer Buyer;
        
        /// <remarks/>
        public AllowanceTypeEnum AllowanceType;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public partial class MainSeller {
        
        /// <remarks/>
        public string Identifier;
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public string Address;
        
        /// <remarks/>
        public string PersonInCharge;
        
        /// <remarks/>
        public string TelephoneNumber;
        
        /// <remarks/>
        public string FacsimileNumber;
        
        /// <remarks/>
        public string EmailAddress;
        
        /// <remarks/>
        public string CustomerNumber;
        
        /// <remarks/>
        public string RoleRemark;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public partial class Amount {
        
        /// <remarks/>
        public decimal TaxAmount;
        
        /// <remarks/>
        public decimal TotalAmount;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public partial class MainBuyer {
        
        /// <remarks/>
        public string Identifier;
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public string Address;
        
        /// <remarks/>
        public string PersonInCharge;
        
        /// <remarks/>
        public string TelephoneNumber;
        
        /// <remarks/>
        public string FacsimileNumber;
        
        /// <remarks/>
        public string EmailAddress;
        
        /// <remarks/>
        public string CustomerNumber;
        
        /// <remarks/>
        public string RoleRemark;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public enum AllowanceTypeEnum {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public partial class DetailsProductItem {
        
        /// <remarks/>
        public string OriginalInvoiceDate;
        
        /// <remarks/>
        public string OriginalInvoiceNumber;
        
        /// <remarks/>
        public string OriginalSequenceNumber;
        
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
        public string AllowanceSequenceNumber;
        
        /// <remarks/>
        public TaxTypeEnum TaxType;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:GEINV:eInvoiceMessage:G0401:4.0")]
    public enum TaxTypeEnum {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1 = 1,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Item4,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Item9 = 9,
    }
}
