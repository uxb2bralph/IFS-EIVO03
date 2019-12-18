using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.TaskManagement
{
    public class TaskManager : EIVOEntityManager<ProcessRequest>
    {       
        public ProcessRequest SaveUploadTask(GenericManager<EIVOEntityDataContext> mgr, String requestPath,Naming.InvoiceProcessType processType)
        {

            var processItem = new ProcessRequest
            {                  
                SubmitDate = DateTime.Now,
                ProcessStart=DateTime.Now,
                ProcessComplete=DateTime.Now,
                RequestPath = requestPath,
                ProcessType = (int)processType,
                ProcessRequestQueue = new ProcessRequestQueue
                {

                }
            };
            mgr.GetTable<ProcessRequest>().InsertOnSubmit(processItem);

            mgr.SubmitChanges();            

            return processItem;
        }
    }
}
