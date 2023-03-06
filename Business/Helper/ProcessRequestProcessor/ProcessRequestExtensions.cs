using Business.Helper.ReportProcessor;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.InvoiceManagement.ErrorHandle;
using Model.Locale;
using Model.Models.ViewModel;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using ModelExtension.Helper;
using ModelExtension.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Utility;
using Uxnet.Com.Security.UseCrypto;

namespace Business.Helper.ProcessRequestProcessor
{
    public static class ProcessRequestExtensions
    {
        public static void ZipInvoicePDF(this int taskID)
        {
            using (GenericManager<EIVOEntityDataContext> models = new GenericManager<EIVOEntityDataContext>())
            {
                ProcessRequest taskItem = models.GetTable<ProcessRequest>()
                                            .Where(p => p.TaskID == taskID)
                                            .FirstOrDefault();
                if (taskItem?.ViewModel == null)
                    return;

                RenderStyleViewModel viewModel = JsonConvert.DeserializeObject<RenderStyleViewModel>(taskItem.ViewModel); 

                if (taskItem.ResponsePath == null)
                {
                    taskItem.ResponsePath = Path.Combine(Logger.LogDailyPath, $"{Guid.NewGuid()}.zip");
                }

                Exception exception = null;

                try
                {
                    String outFile = Path.Combine(Logger.LogDailyPath, $"{Guid.NewGuid()}.zip");

                    var items = models.GetTable<DocumentPrintQueue>()
                        .Where(i => i.UID == taskItem.Sender)
                        .Join(models.GetTable<InvoiceItem>(),
                            i => i.DocID, d => d.InvoiceID, (i, d) => i);

                    using (var zipOut = System.IO.File.Create(outFile))
                    {
                        using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                        {
                            foreach (var doc in items)
                            {
                                viewModel.DocID = doc.DocID;
                                var pdfFile = PdfDocumentGenerator.CreateInvoicePdf(viewModel);
                                var item = doc.CDS_Document.InvoiceItem;
                                models.MarkPrintedLog(item, taskItem.Sender.Value);
                                zip.CreateEntryFromFile(pdfFile, $"{item.TrackCode}{item.No}.pdf");
                                File.Delete(pdfFile);
                            }
                        }
                    }

                    File.Move(outFile, taskItem.ResponsePath);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    exception = ex;
                    Logger.Error(ex);
                }

                if (exception != null)
                {
                    taskItem.ExceptionLog = new ExceptionLog
                    {
                        DataContent = exception.Message
                    };
                }

                taskItem.ProcessComplete = DateTime.Now;
                models.SubmitChanges();


            }

        }

    }
}
