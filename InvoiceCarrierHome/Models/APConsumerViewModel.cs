using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceCarrierHome.Models.ViewModel
{
    public class APConsumerViewModel
    {
        /// <summary>
        /// 發票號碼
        /// </summary>
        public String InvoiceNumber { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public String InvoiceDate { get; set; }
        /// <summary>
        /// 個人識別碼
        /// </summary>
        public String UserCode { get; set; }
        /// <summary>
        /// 4位隨機碼
        /// </summary>
        public String RandomCode { get; set; }
        /// <summary>
        /// 圖形驗證碼
        /// </summary>
        public String ImageCode { get; set; }

        public InvoiceItemData Result { get; set; }
    }

    public class InvoiceItemData
    {
        /// <summary>
        /// 發票號碼
        /// </summary>
        public String InvoiceNumber { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public String InvoiceDate { get; set; }
        /// <summary>
        /// 金額
        /// </summary>
        public String TotalAmount { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public String Currency { get; set; }
        /// <summary>
        /// 發票狀態
        /// </summary>
        public String InvoiceStatus { get; set; }
        /// <summary>
        /// 發票開立公司名稱
        /// </summary>
        public String CompanyName { get; set; }
        /// <summary>
        /// 賣方統編
        /// </summary>
        public String ReceiptNo { get; set; }
        /// <summary>
        /// 賣方地址
        /// </summary>
        public String Address { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public String Remarks { get; set; }

        public List<InvoiceDetailData> Results { get; set; }

    }
    public class InvoiceDetailData
    {
        /// <summary>
        /// 產品名稱
        /// </summary>
        public String ProductName { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public String Quantity { get; set; }
        /// <summary>
        /// 單價
        /// </summary>
        public String UnitPrice { get; set; }
        /// <summary>
        /// 小計
        /// </summary>
        public String Subtotal { get; set; }
    }
}