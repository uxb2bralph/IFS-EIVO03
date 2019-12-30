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
    public class ExecutorForeverBase
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

        public ExecutorForeverBase ChainedExecutor { get; set; }

        protected GenericManager<EIVOEntityDataContext> models;
        
        protected virtual void DoSomething()
        {
            if (ChainedExecutor != null)
            {
                ChainedExecutor.DoSomething();
            }
        }

    }
}
