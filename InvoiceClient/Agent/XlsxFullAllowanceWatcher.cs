using System;
using System.Collections.Generic;
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
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Newtonsoft.Json;
using Utility;

namespace InvoiceClient.Agent
{
    public class XlsxFullAllowanceWatcher : ProcessRequestWatcher
    {

        public XlsxFullAllowanceWatcher(String fullPath)
            : base(fullPath)
        {

        }

        protected override JsonResult processUpload(String requestFile)
        {
            return UploadTo(requestFile, $"{ServerInspector.ServiceInfo.TaskCenterUrl}/InvoiceData/UploadFullAllowanceRequestXlsx?keyID={HttpUtility.UrlEncode(ServerInspector.ServiceInfo.AgentToken)}&sender={ServerInspector.ServiceInfo.AgentUID}");
        }

    }

}
