using DataAccessLayer.basis;
using Model.DataEntity;
using Model.InvoiceManagement.enUS;
using Model.InvoiceManagement.ErrorHandle;
using Model.InvoiceManagement.InvoiceProcess;
using Model.InvoiceManagement.Validator;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Model.InvoiceManagement
{
    public class InvoiceDataSetManager : InvoiceManagerV3
    {
        public InvoiceDataSetManager() : base() { }
        public InvoiceDataSetManager(GenericManager<EIVOEntityDataContext> mgr) : base(mgr) { }

        public enum ResultField
        {
            InvoiceNo = 0,
            DataID,
            InvoiceDate,
            StatusCode,
            Description,
            SellerID,
            EncData,
        }
        public DataTable InitializeInvoiceResponseTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Invoice No", typeof(String)));
            table.Columns.Add(new DataColumn("Data ID", typeof(String)));
            table.Columns.Add(new DataColumn("Invoice Date", typeof(DateTime)));
            table.Columns.Add(new DataColumn("Status Code", typeof(int)));
            table.Columns.Add(new DataColumn("Description", typeof(String)));
            table.Columns.Add(new DataColumn("Seller ID", typeof(String)));
            table.Columns.Add(new DataColumn("EncData", typeof(String)));
            table.TableName = "Process Result";
            return table;
        }

        protected void ReportError(DataTable result, DataRow source, Exception ex, InvoiceDataSetValidator validator)
        {
            DataRow row = result.NewRow();
            row[(int)ResultField.Description] = ex.Message;
            row[(int)ResultField.StatusCode] = 0;
            if (source != null)
            {
                row[(int)ResultField.DataID] = source[validator.InvoiceField.Data_ID];
                row[(int)ResultField.SellerID] = source[validator.InvoiceField.Seller_ID];
            }
            result.Rows.Add(row);
        }

        protected void ReportSuccess(DataTable result, InvoiceItem target)
        {
            DataRow row = result.NewRow();
            row[(int)ResultField.SellerID] = target.InvoiceSeller.ReceiptNo;
            row[(int)ResultField.InvoiceNo] = target.TrackCode + target.No;
            row[(int)ResultField.DataID] = target.InvoicePurchaseOrder?.OrderNo;
            row[(int)ResultField.InvoiceDate] = target.InvoiceDate;
            row[(int)ResultField.StatusCode] = 1;
            if (EIVOPlatformFactory.BuildEncryptedData != null)
            {
                row[(int)ResultField.EncData] = EIVOPlatformFactory.BuildEncryptedData(target);
            }
            result.Rows.Add(row);
        }

        public DataTable SaveUploadInvoiceAutoTrackNoForCBE(DataSet item, Organization owner)
        {
            InvoiceDataSetValidator validator = new InvoiceDataSetValidator(this, owner, Naming.InvoiceProcessType.C0401_Xlsx_CBE);

            DataTable result = InitializeInvoiceResponseTable();

            IEnumerable<DataRow> invoiceItems = item.Tables["Invoice"].Rows.Cast<DataRow>();
            IEnumerable<DataRow> details = item.Tables["Details"].Rows.Cast<DataRow>();
            String dataID = "";
            foreach (DataRow row in details)
            {
                if (row.IsNull(validator.DetailsField.Data_ID) || String.IsNullOrEmpty((String)row[validator.DetailsField.Data_ID]))
                {
                    row[validator.DetailsField.Data_ID] = dataID;
                }
                else
                {
                    dataID = (String)row[validator.DetailsField.Data_ID];
                }
            }

            if (invoiceItems.Count() > 0)
            {
                EventItems = null;
                List<InvoiceItem> eventItems = new List<InvoiceItem>();

                int invSeq = 0;
                processAutoTrackInvoiceNo(invoiceItems, details, Naming.InvoiceTypeDefinition.一般稅額計算之電子發票, validator, ref invSeq, eventItems, result);

                if (eventItems.Count > 0)
                {
                    HasItem = true;
                }
                EventItems = eventItems;
            }

            return result;
        }

        public DataTable SaveUploadInvoiceAutoTrackNoForVAC(DataSet item, Organization owner)
        {
            return SaveUploadInvoiceAutoTrackNo(item, owner, Naming.InvoiceProcessType.C0401_Xlsx_Allocation_ByVAC);
        }

        void processAutoTrackInvoiceNo(IEnumerable<DataRow> items, IEnumerable<DataRow> details, Naming.InvoiceTypeDefinition indication, InvoiceDataSetValidator validator,ref int invSeq, List<InvoiceItem> eventItems, DataTable result)
        {
            validator.InvoiceTypeIndication = indication;
            validator.StartAutoTrackNo();
            for (int idx = 0; idx < items.Count(); idx++, invSeq++)
            {
                try
                {
                    var invItem = items.ElementAt(idx);
                    String dataID = invItem.GetString(validator.InvoiceField.Data_ID).GetEfficientString();
                    IEnumerable<DataRow> productDetails;
                    if (dataID != null)
                    {
                        productDetails = details.Where(d => d.GetString(validator.DetailsField.Data_ID) == dataID);
                    }
                    else
                    {
                        dataID = invItem.GetString(validator.InvoiceField.Invoice_No).GetEfficientString();
                        productDetails = details.Where(d => d.GetString(validator.DetailsField.Invoice_No) == dataID);
                    }

                    Exception ex;
                    if ((ex = validator.Validate(invItem, productDetails)) != null)
                    {
                        if (IgnoreDuplicateDataNumberException && (ex is DuplicateDataNumberException))
                        {
                            var testItem = ((DuplicateDataNumberException)ex).CurrentPO.InvoiceItem;
                            if (testItem != null)
                            {
                                eventItems.Add(testItem);
                                continue;
                            }
                        }

                        ReportError(result, invItem, ex, validator);
                        continue;
                    }

                    InvoiceItem newItem = validator.InvoiceItem;

                    C0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                    C0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);

                    this.EntityList.InsertOnSubmit(newItem);
                    this.SubmitChanges();

                    eventItems.Add(newItem);
                    ReportSuccess(result, newItem);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    ReportError(result, null, ex, validator);
                }
            }
            validator.EndAutoTrackNo();
        }

        public DataTable SaveUploadInvoiceAutoTrackNo(DataSet item, Organization owner, Naming.InvoiceProcessType processType = Naming.InvoiceProcessType.C0401_Xlsx)
        {
            InvoiceDataSetValidator validator = new InvoiceDataSetValidator(this, owner, processType);

            DataTable result = InitializeInvoiceResponseTable();

            IEnumerable<DataRow> invoiceItems = item.Tables["Invoice"].Rows.Cast<DataRow>();
            IEnumerable<DataRow> details = item.Tables["Details"].Rows.Cast<DataRow>();
            //String dataID = "";
            //foreach (DataRow row in details)
            //{
            //    if (row.IsNull(validator.DetailsField.Data_ID) || String.IsNullOrEmpty((String)row[validator.DetailsField.Data_ID]))
            //    {
            //        row[validator.DetailsField.Data_ID] = dataID;
            //    }
            //    else
            //    {
            //        dataID = (String)row[validator.DetailsField.Data_ID];
            //    }
            //}

            if (invoiceItems.Count() > 0)
            {
                EventItems = null;
                List<InvoiceItem> eventItems = new List<InvoiceItem>();

                int invSeq = 0;

                IEnumerable<DataRow> invoiceData = invoiceItems.Where(t => t.GetData<int>(validator.InvoiceField.Invoice_Type) == (int)Naming.InvoiceTypeDefinition.特種稅額計算之電子發票);
                if (invoiceData != null && invoiceData.Count() > 0)
                {

                    processAutoTrackInvoiceNo(invoiceData, details, Naming.InvoiceTypeDefinition.特種稅額計算之電子發票, validator, ref invSeq, eventItems, result);
                    var tmp = invoiceData;
                    invoiceData = invoiceItems.Except(tmp);
                }
                else
                {
                    invoiceData = invoiceItems;
                }

                processAutoTrackInvoiceNo(invoiceData, details, Naming.InvoiceTypeDefinition.一般稅額計算之電子發票, validator, ref invSeq, eventItems, result);

                if (eventItems.Count > 0)
                {
                    HasItem = true;
                }
                EventItems = eventItems;
            }

            return result;
        }

        public DataTable SaveUploadInvoice(DataSet item, Organization owner)
        {
            InvoiceDataSetValidator validator = new InvoiceDataSetValidator(this, owner, Naming.InvoiceProcessType.C0401_Xlsx_Allocation_ByIssuer);

            DataTable result = InitializeInvoiceResponseTable();

            IEnumerable<DataRow> invoiceItems = item.Tables["Invoice"].Rows.Cast<DataRow>();
            IEnumerable<DataRow> details = item.Tables["Details"].Rows.Cast<DataRow>();
            String invoiceNo = "";
            foreach (DataRow row in details)
            {
                if (row.IsNull(validator.DetailsField.Invoice_No) || String.IsNullOrEmpty((String)row[validator.DetailsField.Invoice_No]))
                {
                    row[validator.DetailsField.Invoice_No] = invoiceNo;
                }
                else
                {
                    invoiceNo = (String)row[validator.DetailsField.Invoice_No];
                }
            }

            if (invoiceItems.Count() > 0)
            {
                EventItems = null;
                List<InvoiceItem> eventItems = new List<InvoiceItem>();

                int invSeq = 0;
                void proc(IEnumerable<DataRow> items, Naming.InvoiceTypeDefinition indication)
                {
                    validator.InvoiceTypeIndication = indication;
                    for (int idx = 0; idx < items.Count(); idx++, invSeq++)
                    {
                        try
                        {
                            var invItem = items.ElementAt(idx);
                            invoiceNo = invItem.GetString(validator.InvoiceField.Invoice_No);
                            IEnumerable<DataRow> productDetails = details.Where(d => d.GetString(validator.DetailsField.Invoice_No) == invoiceNo);

                            Exception ex;
                            if ((ex = validator.Validate(invItem, productDetails)) != null)
                            {
                                if (IgnoreDuplicateDataNumberException && (ex is DuplicateDataNumberException))
                                {
                                    var testItem = ((DuplicateDataNumberException)ex).CurrentPO.InvoiceItem;
                                    if (testItem != null)
                                    {
                                        eventItems.Add(testItem);
                                        continue;
                                    }
                                }

                                ReportError(result, invItem, ex, validator);
                                continue;
                            }

                            InvoiceItem newItem = validator.InvoiceItem;

                            C0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                            C0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);

                            this.EntityList.InsertOnSubmit(newItem);
                            this.SubmitChanges();

                            eventItems.Add(newItem);
                            ReportSuccess(result, newItem);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            ReportError(result, null, ex, validator);
                        }
                    }
                }

                IEnumerable<DataRow> invoiceData = invoiceItems.Where(t => t.GetData<int>(validator.InvoiceField.Invoice_Type) == (int)Naming.InvoiceTypeDefinition.特種稅額計算之電子發票);
                if (invoiceData != null && invoiceData.Count() > 0)
                {

                    proc(invoiceData, Naming.InvoiceTypeDefinition.特種稅額計算之電子發票);
                    var tmp = invoiceData;
                    invoiceData = invoiceItems.Except(tmp);
                }
                else
                {
                    invoiceData = invoiceItems;
                }

                proc(invoiceData, Naming.InvoiceTypeDefinition.一般稅額計算之電子發票);

                if (eventItems.Count > 0)
                {
                    HasItem = true;
                }
                EventItems = eventItems;
            }

            return result;
        }

    }
}
