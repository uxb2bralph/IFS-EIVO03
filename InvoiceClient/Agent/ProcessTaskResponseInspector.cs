using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Properties;
using Utility;
using Model.Schema.TXN;
using InvoiceClient.Helper;
using InvoiceClient.TransferManagement;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace InvoiceClient.Agent
{

    public class ProcessTaskResponseInspector : InvoiceServerInspector
    {
        public ProcessTaskResponseInspector()
        {
            
        }

        public override void StartUp()
        {
            InvokeService(ReceiveProcessResponse);
        }

        private void ReceiveProcessResponse()
        {
            String url = $"{ServerInspector.ServiceInfo.TaskCenterUrl}/InvoiceData/NotifyRequestCompletion?keyID={HttpUtility.UrlEncode(ServerInspector.ServiceInfo.AgentToken)}";
            using (WebClientEx client = new WebClientEx())
            {
                client.Timeout = 43200000;

                String result = client.DownloadString(url);
                try
                {
                    dynamic jsonResult = JsonConvert.DeserializeObject(result);

                    if (jsonResult.result == true)
                    {
                        JArray items = (JArray)jsonResult.data;
                        foreach (dynamic item in items)
                        {
                            RetrieveResponse((int?)item.TaskID, (String)item.ChannelName, (String)item.ChannelResponse, (String)item.ResponseName);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }

        private void RetrieveResponse(int? taskID, String channelName, String channelResponse, String responseName)
        {
            String url = $"{ServerInspector.ServiceInfo.TaskCenterUrl}/InvoiceData/CommitProcessResponse?keyID={HttpUtility.UrlEncode(ServerInspector.ServiceInfo.AgentToken)}&taskID={taskID}";
            Logger.Debug("response url:" + url);
            using (WebClientEx client = new WebClientEx())
            {
                client.Timeout = 43200000;

                String filePath = Path.Combine(Settings.Default.InvoiceTxnPath, channelResponse);
                filePath.CheckStoredPath();
                filePath = Path.Combine(filePath, responseName.Substring(responseName.IndexOf('_') + 1));
                client.DownloadFile(url, filePath);
            }
        }

        public override Type UIConfigType
        {
            get { return null; }
        }

    }
}
