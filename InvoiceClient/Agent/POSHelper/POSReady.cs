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
using System.Net;
using Uxnet.Com.Helper;

namespace InvoiceClient.Agent.POSHelper
{

    public class POSReady : InvoiceServerInspector
    {
        internal static readonly LocalSettings _Settings;

        static POSReady()
        {
            String filePath = Path.Combine(Logger.LogPath, "InvoiceNoInspector.json");
            if (File.Exists(filePath))
            {
                _Settings = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(filePath));
            }
            else
            {
                _Settings = new LocalSettings { };
            }

            File.WriteAllText(filePath, _Settings.JsonStringify());

            _Settings.InvoiceNoPreload.CheckStoredPath();
            _Settings.PreparedInvoice.CheckStoredPath();
            _Settings.PreparedAllowance.CheckStoredPath();
            _Settings.SellerInvoice.CheckStoredPath();
            _Settings.PrintInvoice.CheckStoredPath();
            _Settings.MIGResponse.CheckStoredPath();
        }

        public static LocalSettings Settings => _Settings;

        public new class LocalSettings : InvoiceServerInspector.LocalSettings
        {
            public String InvoiceNoPreload { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "InvoiceNo");
            public int LowVolumeAlert { get; set; } = 500;
            public int Booklet { get; set; } = 10;
            public String LoadInvoiceNoUrl { get; set; } = "/POSDevice/AllocateInvoiceNo";
            public String PreparedInvoice { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "PreparedInvoice");
            public String BlindReturn { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "BlindReturn");
            public String ZeroAmount { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "ZeroAmountReceipt");
            public String Replacement { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "Replacement");
            public String SellerInvoice { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "SellerInvoice");
            public String PrintInvoice { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "PrintInvoice");
            public String PrintC0401 { get; set; } = "http://localhost:10800/FrontEnd/PrintC0401";
            public String PrintBlindReturn { get; set; } = "http://localhost:10800/FrontEnd/BlindReturn";
            public String PrintZeroAmount { get; set; } = "http://localhost:10800/FrontEnd/PrintZeroAmount";
            public String PrintReplacement { get; set; } = "http://localhost:10800/FrontEnd/Replacement";
            public String PrintD0401 { get; set; } = "http://localhost:10800/FrontEnd/PrintD0401";
            public String VerifyAllowance { get; set; } = "/POSDevice/VerifyAllowance";
            public String PreparedAllowance { get; set; } = Path.Combine(Logger.LogPath, "InvoiceNoInspector", "PreparedAllowance");
            public String MIGResponse { get; set; } = Path.Combine(Logger.LogPath, "MIGResponse");
            public bool UserPOSPrinter { get; set; } = false;
        }

        public POSReady()
        {

        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class InvoiceIssue
    {
        public string sn { get; set; }
        public string random { get; set; }
        public string aesbase64 { get; set; }
    }

    public class InvoiceNoRoot
    {
        public int? SellerID { get; set; }
        public int? Year { get; set; }
        public int? PeriodNo { get; set; }
        public List<InvoiceIssue> invoice_issue { get; set; }
    }


}
