using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Locale;
using ProcessorUnit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ProcessorUnit.Execution
{
    public class ExecutorForever
    {
        public int TaskDelayInMilliseconds { get; set; } = 5000;
        public void ReadyToGo()
        {
            var t = Task.Run(() =>
            {
                DoSomething();
            });

            t = t.ContinueWith(ts =>
            {
                if(ts.IsFaulted)
                {
                    Logger.Error(ts.Exception);
                }

                Task.Delay(TaskDelayInMilliseconds).ContinueWith(ts1 =>
                {
                    ReadyToGo();
                });
            });
        }

        public ExecutorForever ChainedExecutor { get; set; }

        protected GenericManager<EIVOEntityDataContext> models;
        protected ProcessRequestQueue queueItem;
        protected Naming.InvoiceProcessType? appliedProcessType;
        protected virtual void DoSomething()
        {
            int? taskID = null;
            using (models = new ModelSource<InvoiceItem>())
            {
                try
                {

                    _ = models.DataContext.ApplyProcessRequest(SettingsHelper.Instance.ProcessorID, (int?)appliedProcessType, ref taskID);

                    if (taskID.HasValue)
                    {
                        queueItem = models.GetTable<ProcessRequestQueue>().Where(q => q.TaskID == taskID).First();
                        ProcessRequestItem();
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    queueItem.ProcessRequest.ExceptionLog = new ExceptionLog
                    {
                        DataContent = ex.Message
                    };
                    queueItem.ProcessRequest.ProcessComplete = DateTime.Now;
                    models.GetTable<ProcessRequestQueue>().DeleteOnSubmit(queueItem);
                    models.SubmitChanges();
                }
            }

            if (ChainedExecutor != null)
            {
                ChainedExecutor.DoSomething();
            }

            //else
            //{
            //    Console.WriteLine($"{DateTime.Now}: Do something...");
            //}
        }

        protected virtual void ProcessRequestItem()
        {

        }
    }
}
