using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using InvoiceClient.TransferManagement;
using Model.Locale;
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Newtonsoft.Json;
using Utility;
using Uxnet.Com.Helper;

namespace InvoiceClient.Agent
{
    public class ProcessRequestWatcher : InvoiceWatcher
    {

        public ProcessRequestWatcher(String fullPath)
            : base(fullPath)
        {

        }

        public Naming.InvoiceProcessType? ResponsibleProcessType { get; set; }

        protected override void prepareStorePath(string fullPath)
        {
            _ResponsedPath = fullPath + "(Response)";
            _ResponsedPath.CheckStoredPath();

            _failedTxnPath = fullPath + "(Failure)";
            _failedTxnPath.CheckStoredPath();

            if (__FailedTxnPath != null)
            {
                __FailedTxnPath.Add(_failedTxnPath);
            }

            _inProgressPath = Path.Combine(Logger.LogPath, Path.GetFileName(fullPath), $"{Process.GetCurrentProcess().Id}");
            _inProgressPath.CheckStoredPath();
        }

        protected override void processFile(string invFile)
        {
            if (!File.Exists(invFile))
                return;

            String fileName = Path.GetFileName(invFile);
            String fullPath = Path.Combine(_inProgressPath, fileName);
            try
            {
                File.Move(invFile, fullPath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }

            try
            {
                var result = processUpload(fullPath);

                if (result.result != true)
                {
                    storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
                }
                else
                {
                    storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
            }
        }

        protected JsonResult UploadTo(String requestFile, String url, DynamicQueryStringParameter queryParams = null)
        {
            using (WebClientEx client = new WebClientEx())
            {
                client.Timeout = 43200000;

                if (queryParams != null)
                {
                    url += queryParams.ToQueryString();
                }

                byte[] data = client.UploadFile(url, requestFile);
                return JsonConvert.DeserializeObject<JsonResult>(client.Encoding.GetString(data));

                //using (var write = client.OpenWrite(url))
                //{
                //    using (var file = File.OpenRead(requestFile))
                //    {
                //        file.CopyTo(write);
                //    }

                //    var response = client.Response;
                //    using (StreamReader reader = new StreamReader(response.GetResponseStream(), client.Encoding))
                //    {
                //        return JsonConvert.DeserializeObject<JsonResult>(reader.ReadToEnd());
                //    }
                //}
            }
        }

        protected virtual JsonResult processUpload(String requestFile)
        {
            return UploadTo(requestFile, $"{ServerInspector.ServiceInfo.TaskCenterUrl}/InvoiceData/UploadProcessRequest?keyID={HttpUtility.UrlEncode(ServerInspector.ServiceInfo.AgentToken)}&sender={ServerInspector.ServiceInfo.AgentUID}&processType={(int?)ResponsibleProcessType}");
        }

    }

    public class JsonResult
    {
        public bool? result { get; set; }
        public String message { get; set; }
    }
}
