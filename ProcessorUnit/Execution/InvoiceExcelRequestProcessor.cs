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
    public class InvoiceExcelRequestProcessor : ExecutorForever
    {
        public InvoiceExcelRequestProcessor()
        {
            appliedProcessType = Naming.InvoiceProcessType.C0401_Xlsx;
            processDataSet = (ds, requestItem) => 
            {
                using (InvoiceDataSetManager manager = new InvoiceDataSetManager(models))
                {
                    return manager.SaveUploadInvoiceAutoTrackNo(ds, requestItem);
                }
            };
        }

        protected Func<DataSet, ProcessRequest, DataTable> processDataSet;

        protected override void ProcessRequestItem()
        {
            ProcessRequest requestItem = queueItem.ProcessRequest;
            String requestFile = requestItem.RequestPath.StoreTargetPath();
            if(File.Exists(requestFile))
            {
                Organization agent = requestItem.Organization;
                requestItem.ProcessStart = DateTime.Now;
                models.SubmitChanges();

                using (DataSet ds = requestFile.ImportExcelXLS())
                {
                    int idx = 1;
                    foreach(var t in ds.Tables.Cast<DataTable>().ToArray())
                    {
                        if (t.TableName.Contains("Details"))
                        {
                            t.TableName = "Details";
                        }
                        else if (t.TableName.Contains("Void_Invoice"))
                        {
                            t.TableName = "Void_Invoice";
                        }
                        else if (t.TableName.Contains("Invoice"))
                        {
                            t.TableName = "Invoice";
                        }
                        else if (t.TableName.Contains("Void_Allowance"))
                        {
                            t.TableName = "Void_Allowance";
                        }
                        else if (t.TableName.Contains("Allowance"))
                        {
                            t.TableName = "Allowance";
                        }
                        else 
                        {
                            t.TableName = $"unused_{idx++}";
                        }
                    }
                    var result = processDataSet(ds, requestItem);
                    result.TableName = "Process Result";
                    ds.Tables.Add(result);

                    using (XLWorkbook xls = new XLWorkbook())
                    {
                        xls.Worksheets.Add(ds);

                        String responseName = $"{Path.GetFileNameWithoutExtension(requestFile)}_Response.xlsx";
                        String responsePath = Path.Combine(Path.GetDirectoryName(requestFile), responseName);

                        if (SettingsHelper.Instance.ResponsePath != null)
                        {
                            responsePath = responsePath.Replace(StorePathExtensions.AppRoot, SettingsHelper.Instance.ResponsePath);
                        }

                        xls.SaveAs(responsePath);
                        requestItem.ProcessComplete = DateTime.Now;
                        requestItem.ResponsePath = responsePath;
                        if (requestItem.ProcessCompletionNotification == null)
                            requestItem.ProcessCompletionNotification = new ProcessCompletionNotification { };
                        models.DeleteAnyOnSubmit<ProcessRequestQueue>(d => d.TaskID == queueItem.TaskID);
                        models.SubmitChanges();

                    }
                }
            }
        }

    }
}
