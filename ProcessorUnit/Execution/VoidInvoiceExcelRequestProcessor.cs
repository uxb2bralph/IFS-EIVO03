using Model.InvoiceManagement;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit.Execution
{
    public class VoidInvoiceExcelRequestProcessor : InvoiceExcelRequestProcessor
    {
        public VoidInvoiceExcelRequestProcessor()
        {
            appliedProcessType = Naming.InvoiceProcessType.C0501_Xlsx;
            processDataSet = (ds, requestItem) =>
            {
                using (VoidInvoiceDataSetManager manager = new VoidInvoiceDataSetManager(models))
                {
                    return manager.SaveUploadInvoiceCancellation(ds, requestItem);
                }
            };
        }
    }
}
