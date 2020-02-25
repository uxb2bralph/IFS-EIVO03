using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Locale;

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


    public class DataRecordInvoice
    {
        /*
    <DataNumber>GIZTGMJSHEYDMNRWL4YQ0000</DataNumber>
    <DataDate>2019/04/17</DataDate>
    <GoogleId>295189970125107</GoogleId>
    <SellerId>42523557</SellerId>
    <BuyerName>0181</BuyerName>
    <BuyerId>0000000000</BuyerId>
    <InvoiceType>07</InvoiceType>
    <DonateMark>0</DonateMark>
    <PrintMark>N</PrintMark>
    <InvoiceItem>
        <Description>Google One</Description>
        <Quantity>1</Quantity>
        <UnitPrice>62</UnitPrice>
        <Amount>62</Amount>
        <SequenceNumber>1</SequenceNumber>
    </InvoiceItem>
    <SalesAmount>62</SalesAmount>
    <FreeTaxSalesAmount>0</FreeTaxSalesAmount>
    <ZeroTaxSalesAmount>0</ZeroTaxSalesAmount>
    <TaxType>1</TaxType>
    <TaxRate>0.05</TaxRate>
    <TaxAmount>3</TaxAmount>
    <TotalAmount>65</TotalAmount>
    <Contact>
        <Name>測試員一</Name>
        <Address>100台灣台北市南海路10號</Address>
        <Email>test1@gmail.com</Email>
    </Contact>
    <Currency>TWD</Currency>
    <CarrierType>5G0001</CarrierType>
    <CarrierId1>oxygen0615@gmail.com</CarrierId1>
    <CarrierId2>oxygen0615@gmail.com</CarrierId2>
   */
        public string FileName { get; set; }
        public string DataNumber { get; set; }
        public string DataDate { get; set; }
        public string GoogleId { get; set; }
        public string SellerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerId { get; set; }
        public string InvoiceType { get; set; }
        public string DonateMark { get; set; }
        public string PrintMark { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public Double UnitPrice { get; set; }
        public Double Amount { get; set; }
        public int SequenceNumber { get; set; }
        public Double SalesAmount { get; set; }
        public Double FreeTaxSalesAmount { get; set; }
        public Double ZeroTaxSalesAmount { get; set; }
        public string TaxType { get; set; }
        public double TaxRate { get; set; }
        public Double TaxAmount { get; set; }
        public Double TotalAmount { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Currency { get; set; }
        public string CarrierType { get; set; }
        public string CarrierId1 { get; set; }
        public string CarrierId2 { get; set; }
    }

    public class DataRecordAllowance
    {
        //<AllowanceNumber>GI2DSNZSHAYTQMBTL4YQ0000</AllowanceNumber>
        //<AllowanceDate>2019/07/01</AllowanceDate>
        //<GoogleId>294972066587515</GoogleId>
        //<SellerId>42523557</SellerId>
        //<BuyerName>3415</BuyerName>
        //<BuyerId>0000000000</BuyerId>
        //<AllowanceType>1</AllowanceType>
        //<OriginalDescription>Google Play 應用程式</OriginalDescription>
        //<Quantity>1</Quantity>
        //<UnitPrice>229</UnitPrice>
        //<Amount>229</Amount>
        //<Tax>11</Tax>
        //<AllowanceSequenceNumber>1</AllowanceSequenceNumber>
        //<TaxType>1</TaxType>
        //<TaxAmount>11</TaxAmount>
        //<TotalAmount>240</TotalAmount>
        //<Currency>TWD</Currency>

        public string FileName { get; set; }
        public string AllowanceNumber { get; set; }
        public string AllowanceDate { get; set; }
        public string GoogleId{ get; set; }
        public string SellerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerId { get; set; }
        public string AllowanceType{ get; set; }
        public string OriginalDescription { get; set; }
        public int Quantity { get; set; }
        public Double UnitPrice { get; set; }
        public Double Amount { get; set; }
        public Double Tax { get; set; }
        public int AllowanceSequenceNumber { get; set; }
        public int TaxType { get; set; }
        public Double TaxAmount { get; set; }
        public Double TotalAmount { get; set; }
        public string Currency { get; set; }
    }
}
