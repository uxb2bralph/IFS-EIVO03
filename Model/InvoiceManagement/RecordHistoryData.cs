using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Euthenia;
using Model.DataEntity;

namespace Model.InvoiceManagement
{
    public class RecordHistoryData
    {
        ISql msSql;
        public void RecData(string strFilePath)
        {
            msSql = new aMsSql("localhost", "EIVO03", "eivo", "eivoeivo");
            aBase abase = new aBase(nameof(RecordHistoryData));

            XDocument x = XDocument.Load(strFilePath);
            List<DataRecordInvoice> recInvoiceList = new List<DataRecordInvoice>();
            List<DataRecordAllowance> recAllowanceList = new List<DataRecordAllowance>();
            abase.WriteLog("V2.21.11.05", aBase.LogType.Record, nameof(RecData));
            abase.WriteLog(string.Format("FilePath:{0}", strFilePath), aBase.LogType.Record, nameof(RecData));

            foreach (XElement Invoice in x.Elements("InvoiceRoot").Elements("Invoice"))
            {
                DataRecordInvoice rec = new DataRecordInvoice();

                rec.FileName = strFilePath == "" ? "Empty" : Path.GetFileName(strFilePath);
                rec.DataNumber = Invoice.Element("DataNumber").Value;
                rec.DataDate = Invoice.Element("DataDate").Value;
                rec.GoogleId = Invoice.Element("GoogleId").Value ?? "";
                rec.SellerId = Invoice.Element("SellerId").Value ?? "";
                rec.BuyerName = Invoice.Element("BuyerName").Value ?? "";
                rec.BuyerId = Invoice.Element("BuyerId").Value ?? "";
                rec.InvoiceType = Invoice.Element("InvoiceType").Value ?? "";
                rec.DonateMark = Invoice.Element("DonateMark").Value ?? "";
                rec.PrintMark = Invoice.Element("PrintMark").Value ?? "";
                foreach (XElement InvoiceItem in Invoice.Elements("InvoiceItem"))
                {
                    rec.Description = InvoiceItem.Element("Description").Value ?? "";
                    rec.Quantity = int.Parse(InvoiceItem.Element("Quantity").Value ?? "0");
                    rec.UnitPrice = Double.Parse(InvoiceItem.Element("UnitPrice").Value ?? "0");
                    rec.Amount = Double.Parse(InvoiceItem.Element("Amount").Value ?? "0");
                    rec.SequenceNumber = int.Parse(InvoiceItem.Element("SequenceNumber").Value ?? "0");
                }
                rec.SalesAmount = Double.Parse(Invoice.Element("SalesAmount").Value ?? "0");
                rec.FreeTaxSalesAmount = Double.Parse(Invoice.Element("FreeTaxSalesAmount").Value ?? "0");
                rec.ZeroTaxSalesAmount = Double.Parse(Invoice.Element("ZeroTaxSalesAmount").Value ?? "0");
                rec.TaxType = Invoice.Element("TaxType").Value ?? "";
                rec.TaxRate = Double.Parse(Invoice.Element("TaxRate").Value ?? "0");
                rec.TaxAmount = Double.Parse(Invoice.Element("TaxAmount").Value ?? "0");
                rec.TotalAmount = Double.Parse(Invoice.Element("TotalAmount").Value ?? "0");
                foreach (XElement Contact in Invoice.Elements("Contact"))
                {
                    rec.Name = Contact.Element("Name").Value ?? "";
                    rec.Address = (Contact.Element("Address").Value ?? "").Replace("'", "''");
                    rec.Email = Contact.Element("Email").Value ?? "";
                }
                rec.Currency = Invoice.Element("Currency").Value ?? "";

                rec.CarrierType = Invoice.Element("CarrierType") == null ? "" : Invoice.Element("CarrierType").Value;
                rec.CarrierId1 = Invoice.Element("CarrierId1") == null ? "" : Invoice.Element("CarrierId1").Value;
                rec.CarrierId2 = Invoice.Element("CarrierId2") == null ? "" : Invoice.Element("CarrierId2").Value;

                recInvoiceList.Add(rec);

                abase.WriteLog("recInvoiceList add 1 item", aBase.LogType.Record, nameof(RecData));
            }

            foreach (XElement Allowance in x.Elements("AllowanceRoot").Elements("Allowance"))
            {
                DataRecordAllowance rec = new DataRecordAllowance();

                rec.FileName = strFilePath == "" ? "Empty" : Path.GetFileName(strFilePath);
                rec.AllowanceNumber = Allowance.Element("AllowanceNumber").Value;
                rec.AllowanceDate = Allowance.Element("AllowanceDate").Value;
                rec.GoogleId = Allowance.Element("GoogleId").Value;
                rec.SellerId = Allowance.Element("SellerId").Value;
                rec.BuyerName = Allowance.Element("BuyerName").Value;
                rec.BuyerId = Allowance.Element("BuyerId").Value;
                rec.AllowanceType = Allowance.Element("AllowanceType").Value;
                foreach (XElement AllowanceItem in Allowance.Elements("AllowanceItem"))
                {
                    rec.OriginalDescription = AllowanceItem.Element("OriginalDescription").Value ?? "";
                    rec.Quantity = int.Parse(AllowanceItem.Element("Quantity").Value);
                    rec.UnitPrice = Double.Parse(AllowanceItem.Element("UnitPrice").Value);
                    rec.Amount = Double.Parse(AllowanceItem.Element("Amount").Value);
                    rec.Tax = Double.Parse(AllowanceItem.Element("Tax").Value);
                    rec.AllowanceSequenceNumber = int.Parse(AllowanceItem.Element("AllowanceSequenceNumber").Value);
                    rec.TaxType = int.Parse(AllowanceItem.Element("TaxType").Value);
                }
                rec.TaxAmount = Double.Parse(Allowance.Element("TaxAmount").Value);
                rec.TotalAmount = Double.Parse(Allowance.Element("TotalAmount").Value);
                rec.Currency = Allowance.Element("Currency").Value;

                recAllowanceList.Add(rec);

                abase.WriteLog("recAllowanceList add 1 item", aBase.LogType.Record, nameof(RecData));
            }

            if (recAllowanceList.Count() > 0)
            {
                abase.WriteLog(string.Format("recAllowanceList count {0}:", recAllowanceList.Count()), aBase.LogType.Record, nameof(RecData));
            }
            if (recInvoiceList.Count() > 0)
            {
                abase.WriteLog(string.Format("recInvoiceList count {0}:", recInvoiceList.Count()), aBase.LogType.Record, nameof(RecData));
            }

            msSql.BeginTransaction();
            try
            {
                if (recAllowanceList.Count() > 0)
                {
                    msSql.InsData(ConvertToDataTable(recAllowanceList.ToList()), "RecordAllowance");
                }
                if (recInvoiceList.Count() > 0)
                {
                    msSql.InsData(ConvertToDataTable(recInvoiceList.ToList()), "RecordInvoice");
                }

                msSql.Commit();

                if (recInvoiceList.Count() > 0)
                {
                    abase.WriteLog("recInvoiceList Insert Complete", aBase.LogType.Record, nameof(RecData));
                }
                if (recAllowanceList.Count() > 0)
                {
                    abase.WriteLog("recAllowanceList Insert Complete", aBase.LogType.Record, nameof(RecData));
                }

            }
            catch (Exception e)
            {
                msSql.Rollback();
                abase.WriteLog(e, aBase.LogType.Wrong, "msSql");
            }


        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
