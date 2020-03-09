using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataEntity;
using System.Diagnostics;
using Utility;
using Model.ProcessorUnitHelper;
using ProcessorUnit.Execution;
using ProcessorUnit.Models;
using System.IO;
using System.Xml.Linq;
using Euthenia;
using System.Data;
using System.ComponentModel;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Threading;
using Model.InvoiceManagement;


namespace ProcessorRecord
{


    class Program
    {
        static void Main(string[] arg)
        {


            Run();


            //Logger.OutputWritter = Console.Out;
            //Logger.Info($"Process start at {DateTime.Now}");

            //InitializeApp.StartUp();
            //ConsoleKeyInfo key;
            //do
            //{
            //    key = Console.ReadKey();
            //} while (key.Key != ConsoleKey.Q);
            //Logger.Info("Process terminated..");
        }
        private static FileSystemWatcher InvoiceRequestwatcher = new FileSystemWatcher();
        private static Queue<string> InvoiceRequestFiles = new Queue<string>();

        private static FileSystemWatcher AllowanceRequestwatcher = new FileSystemWatcher();
        private static Queue<string> AllowanceRequestFiles = new Queue<string>();

        private static FileSystemWatcher AllowanceResponsewatcher = new FileSystemWatcher();
        private static Queue<string> AllowanceResponseFiles = new Queue<string>();

        private static FileSystemWatcher AllowancePdfwatcher = new FileSystemWatcher();
        private static Queue<string> AllowancePdfFiles = new Queue<string>();

        private static ISql sql;
        private static aBase A;

        private static void Run()
        {
            int InvoiceTimes = 0;
            string InvoiceRequestwatcherPath = Properties.Settings.Default.InvoiceRequestwatcherPath;
            string AllowanceRequestwatcherPath = Properties.Settings.Default.AllowanceRequestwatcherPath;
            string AllowanceResponsewatcherPath = Properties.Settings.Default.AllowanceResponsewatcherPath;
            string AllowancePdfwatcherPath = Properties.Settings.Default.AllowancePdfPath;

            //watcher.Path = @"C:\UXB2B_EIVO\PreInvoice(ResponseToPGP)Record";
            InvoiceRequestwatcher.Path = InvoiceRequestwatcherPath;
            //設定是否監控子資料夾
            InvoiceRequestwatcher.IncludeSubdirectories = true;
            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            InvoiceRequestwatcher.EnableRaisingEvents = true;
            InvoiceRequestwatcher.Created += new FileSystemEventHandler(InvoiceRequestwatch_Created);


            AllowanceRequestwatcher.Path = AllowanceRequestwatcherPath;
            //設定是否監控子資料夾
            AllowanceRequestwatcher.IncludeSubdirectories = true;
            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            AllowanceRequestwatcher.EnableRaisingEvents = true;
            AllowanceRequestwatcher.Created += new FileSystemEventHandler(AllowanceRequestwatch_Created);


            AllowanceResponsewatcher.Path = AllowanceResponsewatcherPath;
            //設定是否監控子資料夾
            AllowanceResponsewatcher.IncludeSubdirectories = true;
            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            AllowanceResponsewatcher.EnableRaisingEvents = true;
            AllowanceResponsewatcher.Created += new FileSystemEventHandler(AllowanceResponsewatcher_Created);


            AllowancePdfwatcher.Path = AllowancePdfwatcherPath;
            //設定是否監控子資料夾
            AllowancePdfwatcher.IncludeSubdirectories = true;
            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            AllowancePdfwatcher.EnableRaisingEvents = true;
            AllowancePdfwatcher.Created += new FileSystemEventHandler(AllowancePdfwatcher_Created);







            sql = new aMsSql(Properties.Settings.Default.DB, Properties.Settings.Default.DB_Name, "eivo", "eivoeivo");
            A = new aBase(nameof(ProcessorRecord));

            //string PgpFilePath = @"D:\一帆\測試區\PGP";

            RecordHistoryData rec = new RecordHistoryData(Properties.Settings.Default.DB, Properties.Settings.Default.DB_Name);

            while (true)
            {
                Thread.Sleep(50);
                if (InvoiceRequestFiles.Count > 0)
                {
                    Action<object> action = (object obj) =>
                    {
                        if (InvoiceRequestFiles.Count > 0)
                        {
                            if (InvoiceTimes <= 0)
                            {
                                InvoiceTimes = InvoiceRequestFiles.Count;
                            }
                            string FilePath = InvoiceRequestFiles.Dequeue();
                            List<string> RecordCompleteFilesByDB = GetCompleteFiles("RecordInvoice");

                            if (RecordCompleteFilesByDB.IndexOf(Path.GetFileName(FilePath)) < 0)
                            {
                                rec.RecData(FilePath, RecordHistoryData.TableName.RecordInvoice);
                            }
                            InvoiceTimes--;
                        }
                        if (InvoiceTimes <= 0)
                        {
                            if (AllowanceRequestFiles.Count > 0)
                            {
                                string FilePath = AllowanceRequestFiles.Dequeue();
                                List<string> RecordCompleteFilesByDB = GetCompleteFiles("RecordAllowance");
                                if (RecordCompleteFilesByDB.IndexOf(Path.GetFileName(FilePath)) < 0)
                                {
                                    rec.RecData(FilePath, RecordHistoryData.TableName.RecordAllowance);
                                }
                            }

                            if (AllowanceResponseFiles.Count > 0)
                            {
                                string FilePath = AllowanceResponseFiles.Dequeue();
                                List<string> RecordCompleteFilesByDB = GetCompleteFiles("RecordAllowanceResponse");
                                if (RecordCompleteFilesByDB.IndexOf(Path.GetFileName(FilePath)) < 0)
                                {
                                    rec.RecData(FilePath, RecordHistoryData.TableName.RecordAllowanceResponse);
                                }
                            }

                            if (AllowancePdfFiles.Count > 0)
                            {
                                string FilePath = AllowancePdfFiles.Dequeue();
                                string ZipName = Directory.GetParent(FilePath).ToString().Replace(AllowancePdfwatcherPath + "\\", "") + ".Zip";
                                //C:\UXB2B_EIVO\AllowancePdfTemp\%1
                                string FileName = Path.GetFileNameWithoutExtension(FilePath);

                                rec.RecAllowancePdf(ZipName, FileName);

                            }
                        }
                    };
                    Task verifier = new Task(action, "Record");
                    verifier.Start();
                }
            }
        }



        private static List<XDocument> DecryptFile(string p)
        {

            string keyFileName = @"D:\Eivo\Key\priv.asc";
            //char[] passwd = "70762419".ToCharArray();
            //List<XDocument> result = new List<XDocument>();
            ////XDocument x = XDocument.Load(@"D:\一帆\taiwan_uxb2b_print_gui_request_P_20190416145158_84472.xml");
            //XDocument x = XDocument.Load(DecryptFileToStream(p, keyFileName, passwd));

            //result.Add(x);
            //return result;

            return DecryptFile(p, keyFileName);
        }
        private static List<XDocument> DecryptFile(string p, string keyFileName)
        {

            //string keyFileName = @"D:\Eivo\Key\priv.asc";
            char[] passwd = "70762419".ToCharArray();
            List<XDocument> result = new List<XDocument>();
            //XDocument x = XDocument.Load(@"D:\一帆\taiwan_uxb2b_print_gui_request_P_20190416145158_84472.xml");
            XDocument x = XDocument.Load(DecryptFileToStream(p, keyFileName, passwd));

            result.Add(x);
            return result;
        }

        /// <summary>
        /// 檔案解密
        /// </summary>
        /// <param name="decryptEncryptFileName">需解密檔案完整路徑</param>
        /// <param name="keyFileName">私鑰完整路徑</param>
        /// <param name="passwd">密碼</param>
        /// <param name="defaultFileName">存放解密檔案位置</param>
        private static Stream DecryptFileToStream(string decryptEncryptFileName, string keyFileName, char[] passwd)
        {
            //private1 檔案解密
            //string decryptEncryptFileName = @"D:\一帆\b.pgp";
            //string keyFileName = @"D:\Eivo\Key\priv.asc";
            //char[] passwd = "EivoEivo".ToCharArray();
            //string defaultFileName = @"D:\一帆\c.txt";
            try
            {
                StreamReader readerFileName = new StreamReader(decryptEncryptFileName, Encoding.UTF8, true);
                StreamReader readerKey = new StreamReader(keyFileName, Encoding.UTF8, true);

                return ClsPgp.DecryptFileToStream(readerFileName.BaseStream, readerKey.BaseStream, passwd);
                //  ClsPgp.DecryptFile(decryptEncryptFileName, keyFileName, passwd, defaultFileName);
                //Console.WriteLine("解密成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("解密失敗" + e.Message);
                throw e;
            }
        }



        private static void InvoiceRequestwatch_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".xml" && e.FullPath.IndexOf("request") > 0)
            {
                InvoiceRequestFiles.Enqueue(e.FullPath);
            }
        }

        private static void AllowanceRequestwatch_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".xml" && e.FullPath.IndexOf("request") > 0)
            {
                AllowanceRequestFiles.Enqueue(e.FullPath);
            }
        }

        private static void AllowanceResponsewatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".xml" && e.FullPath.IndexOf("response") > 0)
            {
                AllowanceResponseFiles.Enqueue(e.FullPath);
            }
        }


        private static void AllowancePdfwatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".pdf")
            {
                AllowancePdfFiles.Enqueue(e.FullPath);
            }
        }

        private static List<string> GetCompleteFiles(string strTableName)
        {
            string sqlstring = string.Format("select distinct FileName from {0}", strTableName);
            List<string> result = new List<string>();
            DataTable dt = sql.GetDataTable(sqlstring, strTableName);

            foreach (DataRow dtrw in dt.Rows)
            {
                result.Add(A.GetString(dtrw["FileName"]));
            }

            return result;
        }
    }

    class InitializeApp
    {
        private readonly ClsPgp clsPgp = new ClsPgp();
        public static ISql sql;
        public static aBase A;

        private static InitializeApp _instance = new InitializeApp();
        InitializeApp()
        {
            Initialize();
        }

        private void Initialize()
        {
            sql = new aMsSql("localhost", "EIVO03", "eivo", "eivoeivo");
            A = new aBase(nameof(ProcessorRecord));
        }




        public static void StartUp()
        {
            /*
            取得PGP檔案列表
            取得資料庫中LOG紀錄

            交叉比對已處理過的PGP檔案，並剔除
            解密PGP檔案
            依序處理XML檔案
            {
                讀取XML檔案
                記錄到資料庫LOG中
            }

            ※網優需修改打包後刪除的排程，修改讀取的資料夾改成讀取我們驗證完畢的資料夾

            讀取暫存PDF目錄列表
            {
                if (驗證PDF能夠被開啟)
                {
                    將暫存PDF資料夾的檔案搬移到待打包資料夾
                }
                else
                {
                    Return 整批PDF重作;
                }
            }
            */
            #region Test
#if (DEBUG)
            //做鑰匙
            //MakeKeyPair();

            //加密檔案
            //string encryptFileName = string.Format(@"D:\一帆\測試區\PGP\{0}.pgp", DateTime.Now.ToString("yyyyMMddINHHmmss"));
            //string inputFileName = @"D:\一帆\測試區\XML\print_gui_request_P_20190416145158_84473.xml";
            //string encKeyFileName = @"D:\Eivo\Key\pub.asc";
            //EncryptFile(encryptFileName, inputFileName, encKeyFileName);

#endif

            #endregion

            string PgpFilePath = @"D:\一帆\測試區\PGP";
            //string XmlFilePath = Logger.LogPath;

            var PgpFileList = Directory.EnumerateFileSystemEntries(PgpFilePath, "*IN*.pgp").ToList();
            List<string> RecordCompleteFilesByDB = GetCompleteFiles();

            for (int i = PgpFileList.Count - 1; i >= 0; i--)
            {
                if (RecordCompleteFilesByDB.IndexOf(Path.GetFileName(PgpFileList[i].ToString())) >= 0)
                {
                    PgpFileList.RemoveAt(i);
                }
            }
            RecordData(PgpFileList);

            string PDFPath = @"D:\一帆\測試區\PDF";
            var PdfFileList = Directory.EnumerateFileSystemEntries(PDFPath, "*.pdf").ToList();
            var PdfMove = @"D:\一帆\測試區\完成區";
            List<string> ErrorPdfList = new List<string>();
            foreach (var p in PdfFileList)
            {
                if (!CheckPdf(p))
                {
                    ErrorPdfList.Add(p);
                }
                else
                {
                    File.Move(p, Path.Combine(PdfMove, Path.GetFileName(p)));
                }
            }

        }

        private static bool CheckPdf(string path)
        {
            if (File.Exists(path))
            {
                switch (CheckTrueFileName(path))
                {
                    case "3780":
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string CheckTrueFileName(string path)
        {

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            string bx = " ";
            byte buffer;
            try
            {
                if (fs.Length < 1024)
                {
                    return bx;
                }
                buffer = r.ReadByte();
                bx = buffer.ToString();
                buffer = r.ReadByte();
                bx += buffer.ToString();

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                r.Close();
                fs.Close();//真實的檔案
            }
            return bx;
        }
        private static void RecordData(List<string> PgpFileList)
        {
            List<DataRecordInvoice> recList = new List<DataRecordInvoice>();

            foreach (var p in PgpFileList)
            {
                //var p = "";
                List<XDocument> xList = DecryptFile(p);
                foreach (var x in xList)
                {
                    foreach (XElement Invoice in x.Elements("InvoiceRoot").Elements("Invoice"))
                    {
                        DataRecordInvoice rec = new DataRecordInvoice();

                        rec.FileName = p == "" ? "Empty" : Path.GetFileName(p);
                        rec.DataNumber = Invoice.Element("DataNumber").Value;
                        rec.DataDate = Invoice.Element("DataDate").Value;
                        rec.GoogleId = Invoice.Element("GoogleId").Value ?? "";
                        rec.SellerId = Invoice.Element("SellerId").Value ?? "";
                        rec.BuyerName = Invoice.Element("BuyerName").Value ?? "";
                        rec.BuyerId = Invoice.Element("BuyerId").Value ?? "";
                        rec.InvoiceType = Invoice.Element("InvoiceType").Value ?? "";
                        rec.DonateMark = Invoice.Element("DonateMark").Value ?? "";
                        rec.PrintMark = Invoice.Element("PrintMark").Value ?? "";
                        foreach (XElement InvoiceItem in Invoice.Elements("InvoiceItem"))
                        {
                            rec.Description = InvoiceItem.Element("Description").Value ?? "";
                            rec.Quantity = A.GetInt(InvoiceItem.Element("Quantity").Value ?? "0");
                            rec.UnitPrice = A.GetInt(InvoiceItem.Element("UnitPrice").Value ?? "0");
                            rec.Amount = A.GetInt(InvoiceItem.Element("Amount").Value ?? "0");
                            rec.SequenceNumber = A.GetInt(InvoiceItem.Element("SequenceNumber").Value ?? "0");
                        }
                        rec.SalesAmount = A.GetInt(Invoice.Element("SalesAmount").Value ?? "0");
                        rec.FreeTaxSalesAmount = A.GetInt(Invoice.Element("FreeTaxSalesAmount").Value ?? "0");
                        rec.ZeroTaxSalesAmount = A.GetInt(Invoice.Element("ZeroTaxSalesAmount").Value ?? "0");
                        rec.TaxType = Invoice.Element("TaxType").Value ?? "";
                        rec.TaxRate = A.GetDouble(Invoice.Element("TaxRate").Value ?? "0");
                        rec.TaxAmount = A.GetInt(Invoice.Element("TaxAmount").Value ?? "0");
                        rec.TotalAmount = A.GetInt(Invoice.Element("TotalAmount").Value ?? "0");
                        foreach (XElement Contact in Invoice.Elements("Contact"))
                        {
                            rec.Name = Contact.Element("Name").Value ?? "";
                            rec.Address = Contact.Element("Address").Value ?? "";
                            rec.Email = Contact.Element("Email").Value ?? "";
                        }
                        rec.Currency = Invoice.Element("Currency").Value ?? "";

                        rec.CarrierType = Invoice.Element("CarrierType") == null ? "" : Invoice.Element("CarrierType").Value;
                        rec.CarrierId1 = Invoice.Element("CarrierId1") == null ? "" : Invoice.Element("CarrierId1").Value;
                        rec.CarrierId2 = Invoice.Element("CarrierId2") == null ? "" : Invoice.Element("CarrierId2").Value;

                        recList.Add(rec);
                    }
                }
            }

            sql.BeginTransaction();
            try
            {
                sql.InsData(ConvertToDataTable(recList.ToList()), "RecordInvoice");
                sql.Commit();
            }
            catch (Exception e)
            {
                sql.Rollback();
            }
        }

        private static void MakeKeyPair()
        {
            ClsPgp.GetKeyPair(@"D:\Eivo\Key", "EivoEivo");
        }

        /// <summary>
        /// 檔案解密
        /// </summary>
        /// <param name="decryptEncryptFileName">需解密檔案完整路徑</param>
        /// <param name="keyFileName">私鑰完整路徑</param>
        /// <param name="passwd">密碼</param>
        /// <param name="defaultFileName">存放解密檔案位置</param>
        private static Stream DecryptFileToStream(string decryptEncryptFileName, string keyFileName, char[] passwd)
        {
            //private1 檔案解密
            //string decryptEncryptFileName = @"D:\一帆\b.pgp";
            //string keyFileName = @"D:\Eivo\Key\priv.asc";
            //char[] passwd = "EivoEivo".ToCharArray();
            //string defaultFileName = @"D:\一帆\c.txt";
            try
            {
                StreamReader readerFileName = new StreamReader(decryptEncryptFileName, Encoding.UTF8, true);
                StreamReader readerKey = new StreamReader(keyFileName, Encoding.UTF8, true);

                return ClsPgp.DecryptFileToStream(readerFileName.BaseStream, readerKey.BaseStream, passwd);
                //  ClsPgp.DecryptFile(decryptEncryptFileName, keyFileName, passwd, defaultFileName);
                //Console.WriteLine("解密成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("解密失敗" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// 加密檔案
        /// </summary>
        /// <param name="encryptFileName">加密檔案位置</param>
        /// <param name="inputFileName">待加密檔案完整路徑</param>
        /// <param name="encKeyFileName">公鑰完整路徑</param>
        private static void EncryptFile(string encryptFileName, string inputFileName, string encKeyFileName)
        {
            //string encryptFileName = @"D:\一帆\b.pgp";
            //string inputFileName = @"D:\一帆\temp.txt";
            //string encKeyFileName = @"D:\Eivo\Key\pub.asc";
            bool armor = true;
            bool withIntegrityCheck = false;
            try
            {
                ClsPgp.EncryptFile(encryptFileName, inputFileName, encKeyFileName, armor, withIntegrityCheck);
                Console.WriteLine("加密成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("加密失敗" + e.Message);
            }
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private static List<XDocument> DecryptFile(string p)
        {

            string keyFileName = @"D:\Eivo\Key\priv.asc";
            char[] passwd = "EivoEivo".ToCharArray();
            List<XDocument> result = new List<XDocument>();
            //XDocument x = XDocument.Load(@"D:\一帆\taiwan_uxb2b_print_gui_request_P_20190416145158_84472.xml");
            XDocument x = XDocument.Load(DecryptFileToStream(p, keyFileName, passwd));

            result.Add(x);
            return result;
        }

        private static List<string> GetCompleteFiles()
        {
            string sqlstring = @"select distinct FileName from RecordInvoice";
            List<string> result = new List<string>();
            DataTable dt = sql.GetDataTable(sqlstring, "RecordInvoice");

            foreach (DataRow dtrw in dt.Rows)
            {
                result.Add(A.GetString(dtrw["FileName"]));
            }

            return result;
        }
    }
}
