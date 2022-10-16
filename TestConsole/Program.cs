﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Model.DataEntity;
using Model.Helper;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Data.Linq;
using Business.Helper;
using System.Net;
using TestConsole.ServiceReference1;
using ClosedXML.Excel;
using System.Data;
using Uxnet.Com.DataAccessLayer;
using Model.InvoiceManagement;
using Model.InvoiceManagement.InvoiceProcess;
using Model.Schema.EIVO;
using System.Security.Cryptography;
using Model.Schema.TXN;
using ModelExtension.Helper;
using Model.Locale;
using Model.Models.ViewModel;

namespace TestConsole
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            Logger.OutputWritter = Console.Out;
            Logger.Info($"Process start at {DateTime.Now}");

            test32();

            //test01();
            //test02();
            //test03();
            //XElement doc = XElement.Parse("<root><test>hello...</test></root>");

            //using (HttpClient client = new HttpClient())
            //{

            //}
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml("<root><book>test</book></root>");

            //test04();

            //test05();
            //Logger.OutputWritter = Console.Out;
            //AppResource.Instance.InitializeKey(true);
            //test08();
            //Logger.Info("test...");
            //test06();
            //test07();

            //test08(args);

            //Dictionary<int, String> d = new Dictionary<int, string>();
            //d.Add(0, "aaa");
            //d.Add(1, "bbb");
            //d.Add(2, "ccc");

            //test09();

            //test10();
            //test11();

            //if (args != null && args.Length > 0)
            //{
            //    XmlDocument doc = new XmlDocument();
            //    doc.Load(args[0]);
            //    String outFile = Path.Combine(Path.GetDirectoryName(args[0]), $"{Path.GetFileNameWithoutExtension(args[0])}(formatted).xml");
            //    Console.WriteLine($"Save to:{outFile}");
            //    doc.Save(outFile);
            //}

            //test12(args);

            //using (XLWorkbook xlwb = new XLWorkbook("G:\\temp\\發票資料明細.xlsx"))
            //{

            //}

            //DataSet ds = @"G:\temp\test.xlsx".ImportExcelXLS();
            //test14();
            //test15();
            //test16();

            //test17();

            //test18();
            //test19();

            //test20(args);
            //test24();
            //test21(args);
            //test22(args);
            //var result = "aaa,bbb,\"c,d\"e\"\",f,,,kkk".ParseCsvLine();
            //test23();

            //test25();

            //test26(args);

            //test27();
            //test28(args);
            //test29(args);

            //test30();

            //test31();

            //String json = File.ReadAllText("G:\\temp\\test.json");
            //InvoiceRoot invoice = JsonConvert.DeserializeObject<InvoiceRoot>(json);

            Console.ReadKey();
        }

        private static void test32()
        {
            String test = "CN=amylee, OU=70762419-RA-UXRA, OU=Universal Exchange Inc., OU=Public Certification Authority, O=\"Chunghwa Telecom Co., Ltd.\", C=TW";
            String input = "CN = amylee, OU=70762419-RA-UXRA, OU=Universal Exchange Inc., OU=Public Certification Authority, O=\"Chunghwa Telecom Co., Ltd.\", C=TW";
            String pattern = @"[^=,\s]*[\s]*?=((\""[^""]*\"")|[^,]*)";

            var all = Regex.Matches(input, pattern);
            var items = all.Cast<Match>().Select(m => m.Value.Split('='))
                  .Select(pair => new KeyValuePair<String, String>(pair[0].Trim().ToUpper(), pair[1].Trim()))
                  .ToList();
        }

        private static void test31()
        {
            Regex reg = new Regex("^/((?<!,)[A-Z0-9+-\\.](?!,)){7}$");
            String line;
            while ((line = Console.ReadLine()) != "")
            {
                Console.WriteLine($"Matched : {reg.IsMatch(line)}");
            }
        }

        private static void test30()
        {
            Organization[] items = JsonConvert.DeserializeObject<Organization[]>(File.ReadAllText("G:\\temp\\Organization.json"));
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var table = models.GetTable<Organization>();
                foreach (var item in items)
                {
                    if (!table.Any(o => o.ReceiptNo == item.ReceiptNo))
                    {
                        table.InsertOnSubmit(item);
                        models.SubmitChanges();
                    }
                }
            }
        }

        private static void test29(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
                {
                    var token = models.GetTable<Organization>().Where(c => c.ReceiptNo == args[0])
                            .FirstOrDefault()?.OrganizationToken;

                    if (token != null)
                    {
                        AuthTokenViewModel viewModel = new AuthTokenViewModel
                        {
                            SellerID = args[0],
                            Seed = $"{DateTime.Now.Ticks % 100000000:00000000}",
                        };

                        using (SHA256 hash = SHA256.Create())
                        {
                            viewModel.Authorization = token.ComputeAuthorization(hash, viewModel.Seed);
                            Console.WriteLine(viewModel.JsonStringify());
                        }
                    }
                }
            }
        }

        private static void test28(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (Directory.Exists(args[0]))
                {
                    var files = Directory.EnumerateFiles(args[0], "*.xml");
                    if (files.Any())
                    {
                        List<AutomationItem> container = new List<AutomationItem>();
                        XmlDocument docRes = new XmlDocument();
                        String storedPath = Logger.LogDailyPath.CheckStoredPath();
                        foreach (var f in files)
                        {
                            docRes.Load(f);
                            var responseItem = docRes.ConvertTo<Automation>();
                            if (responseItem.Item != null && responseItem.Item.Length > 0)
                            {
                                container.AddRange(responseItem.Item);
                            }

                            String storedFile = Path.Combine(storedPath, Path.GetFileName(f));
                            if (File.Exists(storedFile))
                            {
                                File.Delete(storedFile);
                            }
                            File.Move(f, storedFile);
                        }

                        if (container.Any())
                        {
                            using (DataSet ds = new DataSet())
                            {
                                DataTable table = null;
                                if (container[0].Invoice != null)
                                {
                                    table = container
                                    .Select(i => new
                                    {
                                        i.Invoice?.SellerId,
                                        i.Invoice?.InvoiceNumber,
                                        i.Status,
                                        i.Description,
                                    })
                                    .ToDataTable();
                                }
                                else if (container[0].CancelInvoice != null)
                                {
                                    table = container
                                    .Select(i => new
                                    {
                                        i.CancelInvoice?.SellerId,
                                        i.CancelInvoice?.CancelInvoiceNumber,
                                        i.Status,
                                        i.Description,
                                    })
                                    .ToDataTable();
                                }
                                else if (container[0].Allowance != null)
                                {
                                    table = container
                                    .Select(i => new
                                    {
                                        i.Allowance?.SellerId,
                                        i.Allowance?.AllowanceNumber,
                                        i.Status,
                                        i.Description,
                                    })
                                    .ToDataTable();
                                }
                                else if (container[0].CancelAllowance != null)
                                {
                                    table = container
                                    .Select(i => new
                                    {
                                        i.CancelAllowance?.SellerId,
                                        i.CancelAllowance?.CancelAllowanceNumber,
                                        i.Status,
                                        i.Description,
                                    })
                                    .ToDataTable();
                                }

                                if (table != null)
                                {
                                    table.TableName = "Response Data";
                                    ds.Tables.Add(table);

                                    using (var xls = ds.ConvertToExcel())
                                    {
                                        String outputName = Path.Combine(args[0].CheckStoredPath(), $"report_{DateTime.Today.AddDays(-1):yyyyMMdd}.xlsx");
                                        xls.SaveAs(outputName);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private static void test27()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var invoiceItems = models.GetTable<InvoiceItem>().OrderByDescending(i => i.InvoiceID)
                                .Take(20);

                var dataItems = invoiceItems.Select(i => i.CreateC0401(false)).ToArray();

                var jsonData = JsonConvert.SerializeObject(dataItems);
                File.WriteAllText("G:\\temp\\C0401.json", jsonData);

                var cancelItems = models.GetTable<InvoiceCancellation>()
                    .Select(c => c.InvoiceItem)
                    .OrderByDescending(i => i.InvoiceID)
                                .Take(20);

                jsonData = JsonConvert.SerializeObject(cancelItems.Select(i => i.CreateC0501(false)).ToArray());
                File.WriteAllText("G:\\temp\\C0501.json", jsonData);

                var allowanceItems = models.GetTable<InvoiceAllowance>()
                    .OrderByDescending(i => i.AllowanceID)
                                .Take(20);

                jsonData = JsonConvert.SerializeObject(allowanceItems.Select(i => i.CreateD0401(models,false)).ToArray());
                File.WriteAllText("G:\\temp\\D0401.json", jsonData);

                var cancelAllowance = models.GetTable<InvoiceAllowanceCancellation>()
                    .Select(c => c.InvoiceAllowance)
                    .OrderByDescending(i => i.AllowanceID)
                                .Take(20);

                jsonData = JsonConvert.SerializeObject(cancelAllowance.Select(i => i.CreateD0501(false)).ToArray());
                File.WriteAllText("G:\\temp\\D0501.json", jsonData);
            }
        }

        public class InvoiceData
        {
            public InvoiceItem DataItem { get; set; }
            public InvoiceAmountType Amount => DataItem.InvoiceAmountType;
            public InvoiceSeller Seller => DataItem.InvoiceSeller;
            public InvoiceBuyer Buyer => DataItem.InvoiceBuyer;
            public InvoiceCarrier Carrier => DataItem.InvoiceCarrier;
            public InvoiceDonation Donation => DataItem.InvoiceDonation;
            public ProductItem[] Products => DataItem.InvoiceDetails
                .SelectMany(d => d.InvoiceProduct.InvoiceProductItem)
                .Select(p => new ProductItem 
                {
                    Product = p,
                })
                .ToArray();
        }

        public class ProductItem
        {
            public InvoiceProductItem Product { get; set; }
            public String Brief => Product.InvoiceProduct.Brief;


        }

        private static void test26(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                CipherDecipherSrv cipher = new CipherDecipherSrv(10);
                Console.WriteLine(cipher.cipher(args[0]));
            }
        }

        private static void test25()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("G:\\temp\\123.xml");

            using (StreamWriter writer = new StreamWriter("G:\\temp\\data.xml", false, Encoding.GetEncoding("utf-8")))
            {
                doc.Save(writer);
            }
        }

        private static void test24()
        {
            String aesNo = AESEncrypt("FP10000803" + "6577", "374EC8AF1BF8F4C21C7428DDC938B107");
            Console.WriteLine(aesNo);
            aesNo = ("FP10000803").EncryptContent("6577");
        }

        public static string AESEncrypt(string plainText, string AESKey)
        {
            byte[] bytes = Encoding.Default.GetBytes(plainText);
            RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                KeySize = 128,
                Key = AESKey.HexToByteArray(),
                BlockSize = 128,
                IV = Convert.FromBase64String("Dt8lyToo17X/XkXaQvihuA==")
            };
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            var buf = memoryStream.ToArray();
            return Convert.ToBase64String(buf);
        }

        private static void test23()
        {
            using (InvoiceManager models = new InvoiceManager())
            {
                A0501Handler a0501 = new A0501Handler(models);
                a0501.WriteToTurnkey();
            }
        }

        private static void test22(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    var lines = File.ReadAllLines(args[0]);
                    if (lines != null && lines.Length > 0)
                    {
                        List<AllowanceRootAllowance> items = new List<AllowanceRootAllowance>();
                        using (InvoiceManager models = new InvoiceManager())
                        {
                            var table = models.GetTable<InvoiceItem>();
                            var dataItems = lines.Select(l => l.GetEfficientString())
                                    .Where(l => l != null)
                                    .Where(l => l.Length == 10)
                                    .Select(l => new
                                    {
                                        TrackCode = l.Substring(0, 2),
                                        No = l.Substring(2)
                                    })
                                    .Select(n => table.Where(i => i.TrackCode == n.TrackCode && i.No == n.No).FirstOrDefault())
                                    .Where(i => i != null)
                                    .Select(i => ToAllowance(i))
                                    .ToArray();

                            if (dataItems != null && dataItems.Length > 0)
                            {
                                AllowanceRoot allowance = new AllowanceRoot
                                {
                                    Allowance = dataItems,
                                };
                                String fileName = Path.Combine(Logger.LogDailyPath, $"taiwan_uxb2b_print_allowance_request_C_{DateTime.Now:yyyyMMddHHmmss_ffffff}.xml");
                                allowance.ConvertToXml().Save(fileName);
                            }

                        }
                    }
                }
            }
        }

        private static AllowanceRootAllowance ToAllowance(InvoiceItem item)
        {
            var productItems = item.InvoiceDetails.SelectMany(d => d.InvoiceProduct.InvoiceProductItem);
            short idx = 1;
            return new AllowanceRootAllowance
            {
                AllowanceNumber = item.InvoicePurchaseOrder?.OrderNo,
                AllowanceDate = DateTime.Today.ToString("yyyy/MM/dd"),
                GoogleId = item.InvoiceBuyer.CustomerID,
                SellerId = item.InvoiceSeller.ReceiptNo,
                BuyerName = item.InvoiceBuyer.Name,
                BuyerId = item.InvoiceBuyer.ReceiptNo,
                AllowanceType = 1,
                AllowanceItem = productItems.Select(p => new AllowanceRootAllowanceAllowanceItem
                {
                    OriginalDescription = p.InvoiceProduct.Brief,
                    Quantity = p.Piece ?? 1,
                    UnitPrice = p.UnitCost ?? 0,
                    Amount = p.CostAmount ?? 0,
                    Tax = (p.CostAmount ?? 0) * 0.05m,
                    AllowanceSequenceNumber = idx++,
                    TaxType = 1
                }).ToArray(),
                TaxAmount = item.InvoiceAmountType.TaxAmount ?? 0,
                TotalAmount = item.InvoiceAmountType.TotalAmount ?? 0,
                Currency = item.InvoiceAmountType.CurrencyType?.AbbrevName ?? "TWD"
            };
        }

        private static void test21(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                int.TryParse(args[1], out int wait);
                (new Uxnet.Com.Helper.DefaultTools.Program()).Print(args[0], wait);
            }
        }

        private static void test20(string[] args)
        {
            DateTime calcPeriod = DateTime.Today.AddMonths(-2);
            int year = calcPeriod.Year;
            int period = (calcPeriod.Month + 1) / 2;
            int? sellerID = null;

            if (args.Length > 1)
            {
                if (int.TryParse(args[0], out int v))
                {
                    year = v;
                }
                if (int.TryParse(args[1], out v))
                {
                    period = v;
                }

                if (args.Length > 2)
                {
                    if (int.TryParse(args[2], out v))
                    {
                        sellerID = v;
                    }
                }
            }



            using (TrackNoIntervalManager models = new TrackNoIntervalManager())
            {
                //models.SettleVacantInvoiceNo(year, period);
                models.SettleUnassignedInvoiceNOPeriodically(year, period, sellerID);
            }
        }

        private static void test19()
        {
            using (InvoiceManager models = new InvoiceManager())
            {
                C0501Handler c0501 = new C0501Handler(models);
                //c0501.NotifyIssued();
                c0501.WriteToTurnkey();
            }
        }

        private static void test18()
        {
            String jsonData = @"
{
    ""result"":true,
    ""data"":[1,3,4,5,6,2]
}
";
            dynamic json = JsonConvert.DeserializeObject(jsonData);
            _Test result = JsonConvert.DeserializeObject<_Test>(jsonData);
            Console.WriteLine(json.result);
        }

        class _Test
        {
            public bool? result { get; set; }
            public object data { get; set; }
            public int?[] asArray => (data as JArray)?.Select(i => (int?)i).ToArray();
        }

        private static void test17()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var orgItem = models.GetTable<Organization>().Where(o => o.ReceiptNo == "70762419").FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(orgItem));
            }
        }

        private static void test16()
        {
            (new Class1()).Test();
            (new Class2()).Test();
        }

        private static void test14()
        {
            var t = Task.Run(() =>
            {
                throw new Exception("test...");
            });

            t.ContinueWith(ts =>
            {
                Console.WriteLine(ts.Exception);

            });
        }

        private static void test15()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var items = models.GetTable<InvoiceAllowance>()
                                .Join(models.GetTable<InvoiceAllowanceSeller>().Where(i => i.ReceiptNo == "27934855"),
                                    a => a.AllowanceID, s => s.AllowanceID, (a, s) => a)
                                .Join(models.GetTable<InvoiceAllowanceCancellation>(),
                                    a => a.AllowanceID, c => c.AllowanceID, (a, c) => c)
                                .OrderByDescending(i => i.AllowanceID).Take(100);

                var test = (IQueryable<dynamic>)items;

                using (DataSet ds = items.GetVoidAllowanceData(models))
                {
                    using (XLWorkbook xls = new ClosedXML.Excel.XLWorkbook())
                    {
                        xls.Worksheets.Add(ds);
                        xls.SaveAs("G:\\temp\\test-3.xlsx");
                    }
                }

            }
        }

        private static void test13()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var items = models.GetTable<InvoiceItem>()
                                .Where(i => i.InvoiceSeller.ReceiptNo == "42954865")
                    .OrderByDescending(i => i.InvoiceID).Take(100);
                var test = (IQueryable<dynamic>)items;

                using (DataSet ds = items.GetInvoiceData(models))
                {
                    using (XLWorkbook xls = new ClosedXML.Excel.XLWorkbook())
                    {
                        xls.Worksheets.Add(ds);
                        xls.SaveAs("G:\\temp\\test-2.xlsx");
                    }
                }

            }
        }

        private static void test12(string[] args)
        {
            eInvoiceServiceSoapClient client = new eInvoiceServiceSoapClient("eInvoiceServiceSoap", "http://eceivo.uxifs.com/Published/eInvoiceService.asmx");
            Console.WriteLine(client.Endpoint.Address);

            if (args.Length > 1)
            {
                foreach (var f in new DirectoryInfo(args[0]).GetFiles(args[1], SearchOption.AllDirectories))
                {
                    string s = File.ReadAllText(f.FullName, Encoding.GetEncoding(950));
                    string t = File.ReadAllText(f.FullName, Encoding.UTF8);
                    if (s != t)
                    {
                        File.WriteAllText(f.FullName, s, Encoding.UTF8);
                        Console.WriteLine($"{f.FullName} converted!!");
                    }
                    else
                    {
                        Console.WriteLine($"{f.FullName} is utf-8!!");
                    }
                }
            }
        }

        private static void test11()
        {
            Logger.OutputWritter = Console.Out;
            Logger.Info("Run....");
            Logger.Info("test....");
            Logger.Info($"{DateTime.Now.Ticks}");
            Logger.Info("Run....");
            Logger.Info("test....");
            Logger.Info($"{DateTime.Now.Ticks}");

            Console.ReadKey();
        }

        private static void test10()
        {
            String xml = @"<?xml version='1.0' encoding='UTF-8' ?>
<AllowanceRoot>
	<Allowance>
		<AllowanceNumber>GU3DKNJSL4YQ0000</AllowanceNumber>
		<AllowanceDate>2019/02/22</AllowanceDate>
		<GoogleId>6101132063883093</GoogleId>
		<SellerId>42523557</SellerId>
		<BuyerName>2498</BuyerName>
		<BuyerId>0000000000</BuyerId>
		<AllowanceType>1</AllowanceType>
		<AllowanceItem>
			<OriginalDescription>Google Play 電影</OriginalDescription>
			<Quantity>1</Quantity>
			<UnitPrice>705</UnitPrice>
			<Amount>705</Amount>
			<Tax>35</Tax>
			<AllowanceSequenceNumber>1</AllowanceSequenceNumber>
			<TaxType>1</TaxType>
		</AllowanceItem>
		<TaxAmount>35</TaxAmount>
		<TotalAmount>740</TotalAmount>
	</Allowance>
</AllowanceRoot>";

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                client.Encoding = Encoding.UTF8;
                Console.WriteLine(client.UploadString("http://localhost:2598/DataView/ConvertDataToAllowance", xml));
            }
        }

        private static void test09()
        {
            TheB b = new TheB();
            Console.WriteLine(b.Say());
            Console.WriteLine(((TheA)b).Say());
            Console.WriteLine(((TheBase)b).Say());
        }

        class TheBase
        {
            public virtual String Say()
            {
                return "nothing";
            }
        }

        class TheA : TheBase
        {
            public override String Say()
            {
                return "AAA";
            }
        }

        class TheB : TheA
        {
            public override String Say()
            {
                return "BBB";
            }
        }

        private static void test08(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                (new TestC0701() { MIG_Only = true }).StartUp(args[0]);
            }
        }

        class TestC0701
        {
            public bool MIG_Only { get; set; }

            public void StartUp(String fileName)
            {
                String storedPath = Path.Combine(Logger.LogDailyPath, "MIG");
                storedPath.CheckStoredPath();
                Model.Properties.Settings.Default.C0701Outbound.CheckStoredPath();

                String invoiceNo = File.ReadAllText(fileName);

                String[] items = invoiceNo.Split(new String[] { "\r\n", ",", ";", "、" }, StringSplitOptions.RemoveEmptyEntries);
                Parallel.ForEach(items, invNo =>
                {
                    if (invNo.Length == 10)
                    {
                        doProcess(invNo, storedPath);
                    }
                });
            }

            void doProcess(String invoiceNo, String storedPath)
            {
                using (ModelSource<InvoiceItem> mgr = new ModelSource<InvoiceItem>())
                {
                    var item = mgr.EntityList.Where(i => i.TrackCode == invoiceNo.Substring(0, 2)
                        && i.No == invoiceNo.Substring(2)).FirstOrDefault();
                    if (item != null)
                    {
                        doC0701(mgr, item, storedPath);
                    }

                }
            }

            void doC0701(ModelSource<InvoiceItem> mgr, InvoiceItem item, String storedPath)
            {
                storedMIG(item, storedPath);
                if (!MIG_Only)
                {
                    mgr.ExecuteCommand(@"DELETE FROM CDS_Document
                                            FROM    DerivedDocument INNER JOIN
                                                    CDS_Document ON DerivedDocument.DocID = CDS_Document.DocID
                                            WHERE   (DerivedDocument.SourceID = {0})", item.InvoiceID);
                    mgr.ExecuteCommand("delete CDS_Document where DocID={0}", item.InvoiceID);
                }
                Console.WriteLine($"{item.TrackCode}{item.No} done!!");
            }

            void storedMIG(InvoiceItem item, String storedPath)
            {
                String invoiceNo = item.TrackCode + item.No;
                item.CreateC0401().ConvertToXml().Save(Path.Combine(storedPath, "C0401_" + invoiceNo + ".xml"));
                (new Model.Schema.TurnKey.C0701.VoidInvoice
                {
                    VoidInvoiceNumber = invoiceNo,
                    InvoiceDate = String.Format("{0:yyyyMMdd}", item.InvoiceDate),
                    BuyerId = item.InvoiceBuyer.ReceiptNo,
                    SellerId = item.InvoiceSeller.ReceiptNo,
                    VoidDate = DateTime.Now.Date.ToString("yyyyMMdd"),
                    VoidTime = DateTime.Now,
                    VoidReason = "註銷重開",
                    Remark = ""
                }).ConvertToXml().Save(Path.Combine(Model.Properties.Settings.Default.C0701Outbound, "C0701_" + item.TrackCode + item.No + ".xml"));

            }
        }


        private static void test08()
        {
            String[] files = Directory.GetFiles("D:\\Download");
            var mode = DateTime.Now.Ticks % 10 + 1;
            var list = files.OrderBy(f => Path.GetFileName(f).Sum(c => (int)c) % mode).Take(10).ToArray();
        }

        private static void test07()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var items = models.GetTable<InvoiceBuyer>().GroupJoin(models.GetTable<Organization>(), b => b.BuyerID, o => o.CompanyID,
                    (b, o) => new { buyer = b, org = o });
                Console.WriteLine(items.Count());
            }
        }

        private static void test06()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                DataLoadOptions ops = new DataLoadOptions();
                ops.LoadWith<InvoiceItem>(i => i.InvoiceBuyer);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceSeller);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceCancellation);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceAmountType);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceWinningNumber);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceCarrier);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceDonation);
                ops.LoadWith<InvoiceItem>(i => i.InvoicePurchaseOrder);
                ops.LoadWith<InvoiceItem>(i => i.InvoiceDetails);
                ops.LoadWith<InvoiceDetail>(i => i.InvoiceProduct);
                ops.LoadWith<InvoiceProduct>(i => i.InvoiceProductItem);

                models.GetDataContext().LoadOptions = ops;

                var invoices = models.GetTable<InvoiceItem>()
                        .Where(i => i.SellerID == 14567);

                //foreach (var item in invoices)
                //{
                //    foreach (var d in item.InvoiceDetails)
                //    {
                //        d.InvoiceProduct.InvoiceProductItem.ToArray();
                //    }
                //}

                var items = invoices
                        .Select(i => new InvoiceEntity
                        {
                            MainItem = i,
                            ItemDetails = i.InvoiceDetails.Select(d => d.InvoiceProduct).ToList()
                        });


                var s = JsonConvert.SerializeObject(items);
                File.WriteAllText("D:\\data.json", s);
            }
        }

        private static void test05()
        {
            var t1 = Task.Run(() =>
            {
                Console.WriteLine("go to sleep...");
                Thread.Sleep(3000);
                throw new Exception("test...");
            });

            var t2 = t1.ContinueWith(ts =>
            {
                Console.WriteLine("wake up...");
            });

            //t2.Wait();
            Console.ReadKey();
        }

        private static void test04()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var item = models.GetTable<InvoiceItem>().OrderByDescending(i => i.InvoiceID).FirstOrDefault();
                var a0401 = item.CreateA0401();
                var data = JsonConvert.SerializeObject(a0401);
                var a0101 = JsonConvert.DeserializeObject<Model.Schema.TurnKey.A0101.Invoice>(data);
            }
        }

        private static void test03()
        {
            byte? b;
            String data = "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";
            CryptoUtility crypto = new CryptoUtility();
            byte[] dataToSign;
            var result = crypto.VerifyEnvelopedPKCS7(Convert.FromBase64String(data), out dataToSign);
            Console.WriteLine(result);
        }

        public enum TEST01
        {
            A,
            B
        }

        private static void test02()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");
            X509Certificate2 cert;
            cert = new X509Certificate2("30414175.p12", "111111", X509KeyStorageFlags.Exportable);
            //using (FileStream fs = new FileStream("DefaultSigner.bin", FileMode.Open, FileAccess.Read))
            //{
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        fs.CopyTo(ms);
            //        CipherDecipherSrv cipher = new CipherDecipherSrv();
            //        var certBytes = cipher.decipherCode(ms.ToArray());
            //        cert = new X509Certificate2(certBytes);
            //        ms.Close();
            //    }
            //    fs.Close();
            //}
            if (CryptoUtility.SignXmlSHA256(doc, "Microsoft Strong Cryptographic Provider", null, cert))
            {
                doc.Save("SignContext.xml");
            }
        }

        private static void test01()
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var items = models.Items.Select((i, x) => new
                {
                    RowIndex = x,
                    InvoiceNo = i.TrackCode + i.No
                }).Take(100).ToList();

            }
        }
    }
}
