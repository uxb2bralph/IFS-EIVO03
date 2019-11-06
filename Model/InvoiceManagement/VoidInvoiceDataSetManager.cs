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
    public class VoidInvoiceDataSetManager : InvoiceManagerV3
    {
        public VoidInvoiceDataSetManager() : base() { }
        public VoidInvoiceDataSetManager(GenericManager<EIVOEntityDataContext> mgr) : base(mgr) { }


        public enum ResultField
        {
            InvoiceNo = 0,
            SellerID,
            StatusCode,
            Description,
        }
        public DataTable InitializeVoidInvoiceResponseTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Invoice No", typeof(String)));
            table.Columns.Add(new DataColumn("Seller ID", typeof(String)));
            table.Columns.Add(new DataColumn("Status Code", typeof(int)));
            table.Columns.Add(new DataColumn("Description", typeof(String)));
            table.TableName = "Process Result";
            return table;
        }

        private void ReportError(DataTable result, Exception ex)
        {
            DataRow row = result.NewRow();
            row[(int)ResultField.Description] = ex.Message;
            row[(int)ResultField.StatusCode] = 0;

            result.Rows.Add(row);
        }

        private void ReportSuccess(DataTable result, InvoiceCancellation target)
        {
            InvoiceItem invoice = target.InvoiceItem;
            DataRow row = result.NewRow();
            row[(int)ResultField.SellerID] = invoice.InvoiceSeller.ReceiptNo;
            row[(int)ResultField.InvoiceNo] = $"{invoice.TrackCode}{invoice.No}";
            row[(int)ResultField.StatusCode] = 1;
            result.Rows.Add(row);
        }

        public DataTable SaveUploadInvoiceCancellation(DataSet item, Organization owner)
        {
            DataTable result = InitializeVoidInvoiceResponseTable();
            IEnumerable<DataRow> items = item.Tables[0].Rows.Cast<DataRow>();

            if (items.Count() > 0)
            {
                EventItems = null;
                EventItems_Allowance = null;
                List<InvoiceItem> eventItems = new List<InvoiceItem>();
                int invSeq = 0;
                for (int idx = 0; idx < items.Count(); idx++, invSeq++)
                {
                    var row = items.ElementAt(idx);
                    try
                    {
                        Exception ex;
                        InvoiceCancellation voidItem = null;
                        DerivedDocument p = null;

                        if ((ex = row.VoidInvoice(this, owner, ref voidItem, ref p)) != null)
                        {
                            ReportError(result, ex);
                            continue;
                        }

                        eventItems.Add(voidItem.InvoiceItem);
                        ReportSuccess(result, voidItem);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        ReportError(result, ex);
                    }
                }

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
