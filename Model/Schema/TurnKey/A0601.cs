﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace Model.Schema.TurnKey.A0601 {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:GEINV:eInvoiceMessage:A0601:3.2")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:GEINV:eInvoiceMessage:A0601:3.2", IsNullable=false)]
    public partial class RejectInvoice {
        
        /// <remarks/>
        public string RejectInvoiceNumber;
        
        /// <remarks/>
        public string InvoiceDate;
        
        /// <remarks/>
        public string BuyerId;
        
        /// <remarks/>
        public string SellerId;
        
        /// <remarks/>
        public string RejectDate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="time")]
        public System.DateTime RejectTime;
        
        /// <remarks/>
        public string RejectReason;
        
        /// <remarks/>
        public string Remark;
    }
}
