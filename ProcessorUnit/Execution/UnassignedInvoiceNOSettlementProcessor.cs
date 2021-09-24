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
using System.Net;
using Uxnet.Com.Helper;
using ProcessorUnit.Properties;
using Model.Models.ViewModel;

namespace ProcessorUnit.Execution
{
    public class UnassignedInvoiceNOSettlementProcessor : ExecutorForeverBase
    {
        public UnassignedInvoiceNOSettlementProcessor()
        {

        }

        protected override void DoSomething()
        {
            InquireNoIntervalViewModel viewModel = PersistenceExtensions.Popup<InquireNoIntervalViewModel>(null, SettingsHelper.Instance.PersistenceModelPath.StoreTargetPath());
            if (viewModel != null)
            {
                using (TrackNoIntervalManager manager = new TrackNoIntervalManager(models))
                {
                    if (viewModel.SellerID.HasValue)
                    {
                        manager.SettleUnassignedInvoiceNOPeriodically(viewModel.Year.Value, viewModel.PeriodNo.Value, viewModel.SellerID);
                        manager.Context.SettlementInvoiceNo(viewModel.SellerID, viewModel.Year.Value, viewModel.PeriodNo.Value);
                        if (viewModel.BranchRelation == true && viewModel.SellerID.HasValue)
                        {
                            foreach (var orgItem in manager.GetQueryByAgent(viewModel.SellerID.Value)
                                                    .Select(o => o.CompanyID))
                            {
                                manager.SettleUnassignedInvoiceNOPeriodically(viewModel.Year.Value, viewModel.PeriodNo.Value, orgItem);
                                manager.Context.SettlementInvoiceNo(orgItem, viewModel.Year.Value, viewModel.PeriodNo.Value);
                            }
                        }
                    }
                    else 
                    {
                        var items = manager.GetTable<InvoiceTrackCode>()
                            .Where(t => t.Year == viewModel.Year && t.PeriodNo == viewModel.PeriodNo)
                            .Join(manager.GetTable<InvoiceTrackCodeAssignment>(), t => t.TrackID, a => a.TrackID, (t, a) => a)
                            .Select(a => a.SellerID);
                        foreach (var orgItem in items)
                        {
                            manager.SettleUnassignedInvoiceNOPeriodically(viewModel.Year.Value, viewModel.PeriodNo.Value, orgItem);
                            manager.Context.SettlementInvoiceNo(orgItem, viewModel.Year.Value, viewModel.PeriodNo.Value);
                        }
                    }
                }
            }

            base.DoSomething();
        }

    }
}
