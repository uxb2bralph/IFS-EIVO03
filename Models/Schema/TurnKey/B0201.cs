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
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace Model.Schema.TurnKey.B0201 {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:B0201:3.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:GEINV:eInvoiceMessage:B0201:3.1", IsNullable=false)]
    public partial class CancelAllowance {
        
        /// <remarks/>
        public string CancelAllowanceNumber;
        
        /// <remarks/>
        public string AllowanceDate;
        
        /// <remarks/>
        public string BuyerId;
        
        /// <remarks/>
        public string SellerId;
        
        /// <remarks/>
        public string CancelDate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="time")]
        public System.DateTime CancelTime;
        
        /// <remarks/>
        public string CancelReason;
        
        /// <remarks/>
        public string Remark;
    }
}
