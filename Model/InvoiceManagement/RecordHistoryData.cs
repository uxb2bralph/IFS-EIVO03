using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.DataEntity;
using System.Data.SqlClient;

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
        public FunMsSql msSql;

        public RecordHistoryData()
        {
            msSql = new FunMsSql();
        }

        public void RecData(string strFilePath, TableName strTableName)
        {
            XDocument x = XDocument.Load(strFilePath);

            switch (strTableName)
            {
                case TableName.RecordInvoice:
                    List<DataRecordInvoice> recInvoiceList = ReadInvoiceRequest(x, strFilePath);
                    if (recInvoiceList.Count() > 0)
                    {
                        SaveData(recInvoiceList, TableName.RecordInvoice, strFilePath);
                    }
                    break;
                case TableName.RecordAllowance:
                    List<DataRecordAllowance> recAllowanceList = ReadAllowanceRequest(x, strFilePath);
                    if (recAllowanceList.Count() > 0)
                    {
                        SaveData(recAllowanceList, TableName.RecordAllowance, strFilePath);
                    }

                    break;
                case TableName.RecordAllowanceResponse:
                    List<DataRecordAllowanceResponse> recAllowanceResponseList = ReadAllowanceResponse(x, strFilePath);
                    if (recAllowanceResponseList.Count() > 0)
                    {
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
                        break;
                    case TableName.RecordAllowance:
                        dt = ConvertToDataTable(((List<DataRecordAllowance>)recList).ToList());
                        msSql.InsData(dt, "RecordAllowance");
                        break;
                    case TableName.RecordAllowanceResponse:
                        dt = ConvertToDataTable(((List<DataRecordAllowanceResponse>)recList).ToList());
                        msSql.InsData(dt, "RecordAllowanceResponse");
                        break;
                    default:
                        return;
                }
                msSql.Commit();
            }
            catch (Exception e)
            {
                msSql.Rollback();
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
            }
            return recAllowanceList;
        }

        public Boolean RecAllowancePdf(string zipName, string fileName)
        {
            string strSql = "SELECT * FROM RecordAllowanceResponse where AllowanceNumber=@AllowanceNumber";
            DataTable dt = msSql.GetDataTable(strSql, "RecordAllowanceResponse", "@AllowanceNumber", fileName.Replace("taiwan_uxb2b_scanned_sac_pdf_", "").Replace(".pdf", ""));

            if (dt.Rows.Count < 1)
            {
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

        public int? InquireInvoiceID(int customer_ID, int total_Amount)
        {
            string strSql = "select * from viewInvoiceIDwithCustomerID where customerid=@customerid and TotalAmount >= @TotalAmount order by TotalAmount, InvoiceDate desc";
            DataTable dt = msSql.GetDataTable(strSql, "viewInvoiceIDwithCustomerID", "@customerid", customer_ID.ToString(), "@TotalAmount", total_Amount.ToString());

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return int.Parse(dt.Rows[0]["InvoiceID"].ToString());
            }
        }


    }


    public class FunMsSql
    {
        private string strConnString;
        private SqlConnection m_conn;
        private SqlTransaction m_transaction;

        public FunMsSql()
        {
            strConnString = Properties.Settings.Default.DBConnectstring;
        }
        private SqlCommand GetSqlCommand()
        {
            SqlCommand cmd = m_conn.CreateCommand();
            cmd.Connection = m_conn;
            cmd.Transaction = m_transaction;
            cmd.CommandTimeout = 300;
            return cmd;
        }

        public void BeginTransaction()
        {
            m_conn = new SqlConnection(strConnString);
            m_conn.Open();
            m_transaction = m_conn.BeginTransaction();
        }

        public void Commit()
        {
            m_transaction.Commit();
            m_conn.Close();
        }

        public void Rollback()
        {
            m_transaction.Rollback();
            m_conn.Close();
        }

        public DataTable GetDataTable(string strSql, string strDatatableName, params string[] strParameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (var conn = new SqlConnection(strConnString))
                {
                    using (var cmd = new SqlCommand(strSql, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        for (int i = 0; i <= strParameters.Length - 1; i += 2)
                        {
                            cmd.Parameters.AddWithValue(strParameters[i].ToString(), strParameters[i + 1]);
                        }
                        conn.Open();

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(ds, strDatatableName);
                        }
                    }
                }
                if (ds.Tables.Count != 1)
                {
                    return new DataTable();
                }
                return ds.Tables[strDatatableName];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InsData(DataTable dt, String strDatatableName)
        {
            int i = 0;

            InsData(dt, strDatatableName, ref i);
        }

        public void InsData(DataTable dt, string strDatatableName, ref int iResult)
        {
            SqlCommand cmd = GetSqlCommand();

            try
            {
                foreach (DataRow dtrw in dt.Rows)
                {
                    if (dtrw.RowState == DataRowState.Added)
                    {
                        String strInsSqlString = GetInsSqlString(dtrw, strDatatableName);
                        cmd.CommandText = strInsSqlString;
                        iResult += cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetInsSqlString(DataRow dtrw, string strDatatableName)
        {
            String strColName = "";
            String strColValue = "";

            DataTable dt = GetDataTable("select * from " + strDatatableName + " where 1=2", strDatatableName);

            for (int i = 0; i <= dtrw.Table.Columns.Count - 1; i++)
            {
                if (InInsertTable(dtrw.Table.Columns[i].ColumnName, dt) && dtrw[i].ToString() != "")
                {
                    if (strColName != "")
                        strColName += ", ";
                    if (strColValue != "")
                        strColValue += ", ";

                    strColName += dtrw.Table.Columns[i].ColumnName;

                    if (dtrw[i].GetType().Name == typeof(DateTime).Name)
                    {
                        strColValue += String.Format("'{0}'", ((DateTime)dtrw[i]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (dtrw[i].GetType().Name == typeof(Int32).Name)
                    {
                        strColValue += ((Int32)dtrw[i]).ToString().ToUpper();
                    }
                    else if (dtrw[i].GetType().Name == typeof(Double).Name)
                    {
                        strColValue += ((Double)dtrw[i]).ToString().ToUpper();
                    }
                    else if (dtrw[i].GetType().Name == typeof(Boolean).Name)
                    {
                        strColValue += ((Boolean)dtrw[i]).ToString().ToUpper();
                    }
                    else
                    {
                        strColValue += String.Format("'{0}'", dtrw[i].ToString());
                    }
                }
            }

            return "Insert " + strDatatableName + " (" + strColName + ") Values (" + strColValue + ")";
        }

        private bool InInsertTable(string columnName, DataTable dt)
        {
            bool bolExist = false;

            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {
                if (dt.Columns[i].ColumnName.ToLower() == columnName.ToLower())
                {
                    return true;
                }
            }
            return bolExist;
        }

        public void UpdateData(string strUpdateString)
        {
            SqlCommand cmd = GetSqlCommand();
            int iResult = 0;
            cmd.CommandText = strUpdateString;
            iResult += cmd.ExecuteNonQuery();
        }

        public void UpdateData(DataTable dt, string strDatatableName, string strUpdateString)
        {
            int i = 0;
            UpdateData(dt, strDatatableName, strUpdateString, ref i);
        }

        public void UpdateData(DataTable dt, string strDatatableName, string strUpdateSqlString, ref int iResult)
        {
            SqlCommand cmd = GetSqlCommand();
            try
            {
                foreach (DataRow dtrw in dt.Rows)
                {
                    if (dtrw.RowState == DataRowState.Modified)
                    {
                        cmd.CommandText = strUpdateSqlString;
                        cmd.Parameters.Clear();
                        for (int ii = 0; ii <= dt.Columns.Count - 1; ii++)
                        {
                            string strColName = dt.Columns[ii].ColumnName;
                            cmd.Parameters.AddWithValue("@" + strColName, dtrw[strColName]);
                        }
                        iResult += cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



    }

}
