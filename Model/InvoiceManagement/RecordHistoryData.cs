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

        public enum TableName
        {
            RecordInvoice,
            RecordAllowance,
            RecordAllowanceResponse
        }
        ISql msSql;
        aBase abase;
        string strDB, strDBName;

        public RecordHistoryData(string DB, string DB_Name)
        {
            strDB = DB;
            strDBName = DB_Name;

            msSql = new aMsSql(strDB, strDBName, "eivo", "eivoeivo");
            abase = new aBase(nameof(RecordHistoryData) + DateTime.Now.ToString("HHmmssfff"));
        }

        public void RecData(string strFilePath, TableName strTableName)
        {

            XDocument x = XDocument.Load(strFilePath);

            abase.WriteLog("V2.21.11.05", aBase.LogType.Record, nameof(RecData));
            abase.WriteLog(string.Format("FilePath:{0}", strFilePath), aBase.LogType.Record, nameof(RecData));

            switch (strTableName)
            {
                case TableName.RecordInvoice:
                    List<DataRecordInvoice> recInvoiceList = ReadInvoiceRequest(x, strFilePath);
                    if (recInvoiceList.Count() > 0)
                    {
                        abase.WriteLog(string.Format("recInvoiceList count:{0}    FilePath:{1}", strFilePath, recInvoiceList.Count()), aBase.LogType.Record, nameof(RecData));

                        SaveData(recInvoiceList, TableName.RecordInvoice, strFilePath);
                    }
                    break;
                case TableName.RecordAllowance:
                    List<DataRecordAllowance> recAllowanceList = ReadAllowanceRequest(x, strFilePath);
                    if (recAllowanceList.Count() > 0)
                    {
                        abase.WriteLog(string.Format("recAllowanceList count:{0}    FilePath:{1}", strFilePath, recAllowanceList.Count()), aBase.LogType.Record, nameof(RecData));
                        SaveData(recAllowanceList, TableName.RecordAllowance, strFilePath);
                    }

                    break;
                case TableName.RecordAllowanceResponse:
                    List<DataRecordAllowanceResponse> recAllowanceResponseList = ReadAllowanceResponse(x, strFilePath);
                    if (recAllowanceResponseList.Count() > 0)
                    {
                        abase.WriteLog(string.Format("recAllowanceResponseList count:{0}    FilePath:{1}", strFilePath, recAllowanceResponseList.Count()), aBase.LogType.Record, nameof(RecData));
                        SaveData(recAllowanceResponseList, TableName.RecordAllowanceResponse, strFilePath);
                    }
                    break;
                default:
                    return;
            }
        }



        private void SaveData(object recList, TableName strTableName, string strFilePath)
        {
            DataTable dt;
            msSql.BeginTransaction();
            try
            {
                switch (strTableName)
                {
                    case TableName.RecordInvoice:
                        dt = ConvertToDataTable(((List<DataRecordInvoice>)recList).ToList());
                        msSql.InsData(dt, "RecordInvoice");
                        abase.WriteLog(string.Format("{0} recInvoiceList Insert Complete", strFilePath), aBase.LogType.Record, nameof(RecData));
                        break;
                    case TableName.RecordAllowance:
                        dt = ConvertToDataTable(((List<DataRecordAllowance>)recList).ToList());
                        msSql.InsData(dt, "RecordAllowance");
                        abase.WriteLog(string.Format("{0} recAllowanceList Insert Complete", strFilePath), aBase.LogType.Record, nameof(RecData));
                        break;
                    case TableName.RecordAllowanceResponse:
                        dt = ConvertToDataTable(((List<DataRecordAllowanceResponse>)recList).ToList());
                        msSql.InsData(dt, "RecordAllowanceResponse");
                        abase.WriteLog(string.Format("{0} recAllowanceResponseList Insert Complete", strFilePath), aBase.LogType.Record, nameof(RecData));
                        break;
                    default:
                        return;
                }
                msSql.Commit();
            }
            catch (Exception e)
            {
                msSql.Rollback();
                abase.WriteLog(e, aBase.LogType.Wrong, "msSql");
            }
        }
        private List<DataRecordAllowanceResponse> ReadAllowanceResponse(XDocument x, string strFilePath)
        {
            List<DataRecordAllowanceResponse> recAllowanceList = new List<DataRecordAllowanceResponse>();
            //foreach (XElement AllowanceResponse in x.Elements("Item"))
            foreach (XElement AllowanceResponse in x.Elements().Elements("Item"))
            {
                DataRecordAllowanceResponse rec = new DataRecordAllowanceResponse();
                rec.FileName = strFilePath == "" ? "Empty" : Path.GetFileName(strFilePath);
                rec.ResponseStatus = AllowanceResponse.Element("Status").Value;
                rec.ZipFileName = "";
                rec.PdfFileName = "";
                foreach (XElement AllowanceItem in AllowanceResponse.Elements("Allowance"))
                {
                    rec.AllowanceNumber = AllowanceItem.Element("AllowanceNumber").Value;
                }
                recAllowanceList.Add(rec);
            }
            return recAllowanceList;
        }
        private List<DataRecordAllowance> ReadAllowanceRequest(XDocument x, string strFilePath)
        {
            List<DataRecordAllowance> recAllowanceList = new List<DataRecordAllowance>();
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

                //abase.WriteLog("recAllowanceList add 1 item", aBase.LogType.Record, nameof(RecData));
            }
            return recAllowanceList;
        }

        public Boolean RecAllowancePdf(string zipName, string fileName)
        {
            string strSql = "SELECT * FROM RecordAllowanceResponse where AllowanceNumber=@AllowanceNumber";
            DataTable dt = msSql.GetDataTable(strSql, "RecordAllowanceResponse", "@AllowanceNumber", fileName.Replace("taiwan_uxb2b_scanned_sac_pdf_", "").Replace(".pdf", ""));

            abase.WriteLog(string.Format("zipName:{0}    fileName:{1}", zipName, fileName), aBase.LogType.Record, nameof(RecAllowancePdf));

            abase.WriteLog(string.Format("AllowanceNumber:{0}", fileName.Replace("taiwan_uxb2b_scanned_sac_pdf_", "").Replace(".pdf", "")), aBase.LogType.Record, nameof(RecAllowancePdf));

            if (dt.Rows.Count < 1)
            {
                abase.WriteLog(string.Format("RecordAllowanceResponse查無對應的資料 zipName：{0}  fileName：{1}", zipName, fileName), aBase.LogType.Wrong, nameof(RecAllowancePdf));
                return true;
            }
            else
            {
                foreach (DataRow dtrw in dt.Rows)
                {
                    dtrw["ZipFileName"] = zipName;
                    dtrw["PdfFileName"] = fileName;
                    dtrw["ResponseStatus"] = "3";
                }
            }

            msSql.BeginTransaction();
            try
            {
                msSql.UpdateData(dt, "RecordAllowanceResponse", " update RecordAllowanceResponse set ZipFileName=@ZipFileName ,PdfFileName=@PdfFileName, ResponseStatus=@ResponseStatus where FileName=@FileName and AllowanceNumber=@AllowanceNumber");
                msSql.Commit();
                return true;
            }
            catch (Exception e)
            {
                msSql.Rollback();
                abase.WriteLog(e, aBase.LogType.Wrong, "msSql");
                return false;
            }


        }

        private List<DataRecordInvoice> ReadInvoiceRequest(XDocument x, string strFilePath)
        {
            List<DataRecordInvoice> recInvoiceList = new List<DataRecordInvoice>();

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
            }

            return recInvoiceList;
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
