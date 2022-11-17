using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.InvoiceManagement.ErrorHandle;
using Model.Models.ViewModel;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using ModelExtension.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Utility;
using Uxnet.Com.Security.UseCrypto;

namespace Business.Helper.ReportProcessor
{
    public static class InvoiceDataReportExtensions
    {
        public static void CreateReport(this MonthlyReportQueryViewModel viewModel)
        {
            if(!viewModel.DateFrom.HasValue)
            {
                viewModel.DateFrom = DateTime.Today;
            }

            if(!viewModel.DateTo.HasValue)
            {
                viewModel.DateTo = viewModel.DateFrom.Value.AddMonths(1);
            }
            else
            {
                viewModel.DateTo = viewModel.DateTo.Value.AddMonths(1);
            }

            viewModel.DateFrom = new DateTime(viewModel.DateFrom.Value.Year, viewModel.DateFrom.Value.Month, 1);
            viewModel.DateTo = new DateTime(viewModel.DateTo.Value.Year, viewModel.DateTo.Value.Month, 1);

            using (GenericManager<EIVOEntityDataContext> models = new GenericManager<EIVOEntityDataContext>())
            {
                ProcessRequest taskItem = models.GetTable<ProcessRequest>()
                                            .Where(p => p.TaskID == viewModel.TaskID)
                                            .FirstOrDefault();
                if (taskItem == null)
                    return;

                if (taskItem.ResponsePath == null)
                {
                    taskItem.ResponsePath = Path.Combine(Logger.LogDailyPath, Guid.NewGuid().ToString() + ".xlsx");
                }

                IQueryable<InvoiceItem> GetInvoice(int sellerID,DateTime dateFrom,DateTime dateTo)
                {
                    var invoice = models.GetTable<InvoiceItem>()
                                        .Where(i => i.SellerID == sellerID)
                                        .Where(i => i.InvoiceDate >= dateFrom)
                                        .Where(i => i.InvoiceDate < dateTo);
                    return invoice;
                }

                IQueryable<InvoiceCancellation> GetInvoiceCancellation(int sellerID, DateTime dateFrom, DateTime dateTo)
                {
                    var cancelInvoice = models.GetTable<InvoiceCancellation>()
                                        .Where(i => i.CancelDate >= dateFrom)
                                        .Where(i => i.CancelDate < dateTo)
                                        .Join(models.GetTable<InvoiceItem>().Where(i => i.SellerID == sellerID),
                                            c => c.InvoiceID, i => i.InvoiceID, (c, i) => c);

                    return cancelInvoice;
                }


                IQueryable<InvoiceAllowance> GetAllowance(int sellerID, DateTime dateFrom, DateTime dateTo)
                {
                    var allowance = models.GetTable<InvoiceAllowance>()
                                        .Where(i => i.AllowanceDate >= viewModel.DateFrom)
                                        .Where(i => i.AllowanceDate < viewModel.DateTo.Value.AddDays(1))
                                        .Join(models.GetTable<InvoiceAllowanceSeller>()
                                            .Where(i => i.SellerID == sellerID),
                                a => a.AllowanceID, s => s.AllowanceID, (a, s) => a);
                    return allowance;
                }

                IQueryable<InvoiceAllowanceCancellation> GetAllowanceCancellation(int sellerID, DateTime dateFrom, DateTime dateTo)
                {
                    var cancelAllowance = models.GetTable<InvoiceAllowanceCancellation>()
                                        .Where(i => i.CancelDate >= dateFrom)
                                        .Where(i => i.CancelDate < dateTo)
                                        .Join(models.GetTable<InvoiceAllowance>()
                                            .Join(models.GetTable<InvoiceAllowanceSeller>()
                                                    .Where(i => i.SellerID == sellerID),
                                                a => a.AllowanceID, s => s.AllowanceID, (a, s) => a),
                                        c => c.AllowanceID, a => a.AllowanceID, (c, a) => c);

                    return cancelAllowance;
                }

                var issuers = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == viewModel.AgentID);
                var sellers = models.GetTable<Organization>()
                    .Where(c => c.CompanyID == viewModel.SellerID
                        || issuers.Any(i => i.IssuerID == c.CompanyID));

                Exception exception = null;

                try
                {

                    using (DataSet ds = new DataSet())
                    {
                        DataTable table = new DataTable("發票資料明細");
                        ds.Tables.Add(table);
                        table.Columns.Add("營業人名稱");
                        table.Columns.Add("統一編號");
                        table.Columns.Add("發票", typeof(int));
                        table.Columns.Add("作廢發票", typeof(int));
                        table.Columns.Add("折讓", typeof(int));
                        table.Columns.Add("作廢折讓", typeof(int));
                        table.Columns.Add("月份");
                        table.Columns.Add("上線日期", typeof(String));
                        table.Columns.Add("註記停用日期", typeof(String));


                        foreach (var issuer in sellers)
                        {
                            for (DateTime idx = viewModel.DateFrom.Value; idx < viewModel.DateTo; )
                            {
                                var end = idx.AddMonths(1);
                                var r = table.NewRow();

                                var seller = issuer;
                                r[0] = seller.CompanyName;
                                r[1] = seller.ReceiptNo;
                                r[2] = GetInvoice(seller.CompanyID, idx, end).Count();
                                r[3] = GetInvoiceCancellation(seller.CompanyID, idx, end).Count();
                                r[4] = GetAllowance(seller.CompanyID, idx, end).Count();
                                r[5] = GetAllowanceCancellation(seller.CompanyID, idx, end).Count();
                                r[6] = $"{idx:yyyyMM}";
                                r[7] = $"{seller.OrganizationExtension?.GoLiveDate:yyyy/MM/dd}";
                                r[8] = $"{seller.OrganizationExtension?.ExpirationDate:yyyy/MM/dd}";

                                table.Rows.Add(r);
                                idx = end;
                            }
                        }

                        using (var xls = ds.ConvertToExcel())
                        {
                            xls.SaveAs(taskItem.ResponsePath);
                        }
                    }

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
