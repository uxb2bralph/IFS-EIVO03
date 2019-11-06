using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utility;
using System.IO;
using Model.Properties;
using Model.Helper;

namespace Model.InvoiceManagement
{
    public class GovPlatformFactory
    {
        private static int __InvoiceBusyCount = 0;

        public static EventHandler SendNotification
        {
            get;
            set;
        }

        public static void Notify()
        {
            //if (Interlocked.Increment(ref __InvoiceBusyCount) == 1)
            //{
            //    ThreadPool.QueueUserWorkItem(t =>
            //    {
            //        try
            //        {
            //            do
            //            {
            //                notifyToProcess(t);
            //            } while (Interlocked.Decrement(ref __InvoiceBusyCount) > 0);

            //            checkResponse(t);
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.Error(ex);
            //        }
            //    });
            //}
        }

        public static int ResetBusyCount()
        {
            Interlocked.Exchange(ref __InvoiceBusyCount, 0);
            return __InvoiceBusyCount;
        }


        private static void notifyToProcess(object stateInfo)
        {
            try
            {
                InvoiceNotification.ProcessMessage();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                using (GovPlatformManager mgr = new GovPlatformManager())
                {
                    mgr.TransmitInvoice();
                    mgr.ExecuteCommand(@"
                        INSERT INTO DocumentDispatch (DocID, TypeID)
                        SELECT          DocID, DocType
                        FROM              CDS_Document AS c
                        WHERE          (CurrentStep IS NULL) AND (DocDate >= {0}) 
                            AND (NOT EXISTS
                                    (SELECT NULL
                                        FROM DocumentDispatch
                                        WHERE (DocID = c.DocID) AND (TypeID = c.DocType)))",DateTime.Today.AddDays(-1));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static void checkResponse(object stateInfo)
        {
            try
            {
                using (GovPlatformManager mgr = new GovPlatformManager())
                {
                    mgr.CheckResponse();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

    }
}
