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
using Business.Helper.ReportProcessor;

namespace ProcessorUnit.Execution
{
    public class DataReportProcessor : ExecutorForeverBase
    {
        public DataReportProcessor()
        {

        }

        protected override void DoSomething()
        {
            MonthlyReportQueryViewModel viewModel = PersistenceExtensions.Popup<MonthlyReportQueryViewModel>(null, SettingsHelper.Instance.PersistenceModelPath.StoreTargetPath());
            if (viewModel != null)
            {
                viewModel.CreateReport();
            }

            base.DoSomething();
        }

    }
}
