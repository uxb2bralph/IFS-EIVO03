using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Agent;
using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.DataEntity;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using Newtonsoft.Json;
using Utility;

namespace InvoiceClient.TransferManagement
{

    public abstract class ServerInspector
    {
        protected bool _isRunning;
        public abstract void StartUp();
        public abstract Type UIConfigType { get; }
        public ServerInspector ChainedInspector
        { get; set; }
        public virtual void ExecutiveService(List<String> pathInfo)
        {
            if (ChainedInspector != null)
            {
                ChainedInspector.ExecutiveService(pathInfo);
            }
        }

        public static Organization GetRegisterdMember()
        {
            using (WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService())
            {

                try
                {
                    Root token = invSvc.CreateMessageToken("讀取用戶端資料");
                    XmlNode doc = invSvc.GetRegisteredMember(token.ConvertToXml().Sign());
                    if (doc != null)
                    {
                        return doc.DeserializeDataContract<Model.DataEntity.Organization>();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return null;
        }

        public static ServiceInfo @ServiceInfo
        {
            get;
            private set;
        }
        public static void PrepareServiceInfo()
        {
            using (WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService())
            {

                try
                {
                    Root token = invSvc.CreateMessageToken("讀取系統服務資訊");
                    String result = invSvc.GetServiceInfo(token.ConvertToXml().Sign());
                    if (result != null)
                    {
                        Logger.Info("ServerInfo:" + result);
                        ServiceInfo = JsonConvert.DeserializeObject<ServiceInfo>(result);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }


        public static void AcknowledgeServer()
        {
            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();

            try
            {
                Root token = invSvc.CreateMessageToken("用戶端系統正在執行中");
                invSvc.AcknowledgeLivingReport(token.ConvertToXml().Sign());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
