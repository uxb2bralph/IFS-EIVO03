using Model.InvoiceManagement;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorUnit.Execution
{
    public class VoidAllowanceExcelRequestProcessor : InvoiceExcelRequestProcessor
    {
        public VoidAllowanceExcelRequestProcessor()
        {
            appliedProcessType = Naming.InvoiceProcessType.D0501_Xlsx;
            processDataSet = (ds, agent) =>
            {
                using (VoidAllowanceDataSetManager manager = new VoidAllowanceDataSetManager(models))
                {
                    return manager.SaveUploadAllowanceCancellation(ds, agent);
                }
            };
        }
    }
}
