using Model.DataEntity;
using Model.Locale;
using Model.Helper;
using ProcessorUnit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Utility;
using Model.InvoiceManagement;
using ClosedXML.Excel;

namespace ProcessorUnit.Execution
{
    public class InvoiceExcelRequestForVACProcessor : InvoiceExcelRequestProcessor
    {
        public InvoiceExcelRequestForVACProcessor() : base()
        {
            appliedProcessType = Naming.InvoiceProcessType.C0401_Xlsx_Allocation_ByVAC;
            processDataSet = (ds, agent) =>
            {
                using (InvoiceDataSetManager manager = new InvoiceDataSetManager(models))
                {
                    return manager.SaveUploadInvoiceAutoTrackNoForVAC(ds, agent);
                }
            };
        }
    }
}
