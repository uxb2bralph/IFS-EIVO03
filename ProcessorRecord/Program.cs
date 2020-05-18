using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataEntity;
using System.IO;
using System.Xml.Linq;
using System.Data;
using System.ComponentModel;
using System.Threading;
using Model.InvoiceManagement;


namespace ProcessorRecord
{
    class Program
    {
        static RecordHistoryData rec;

        static void Main(string[] arg)
        {
            rec = new RecordHistoryData();
            RunSingle(100, 2000);
        }

        private static void RunSingle(int iSingle, int iPart)
        {
            string AllowanceRequestwatcherPath = Properties.Settings.Default.AllowanceRequestwatcherPath;
            string AllowanceResponsewatcherPath = Properties.Settings.Default.AllowanceResponsewatcherPath;
            string AllowancePdfwatcherPath = Properties.Settings.Default.AllowancePdfPath;
            bool DelFile = Properties.Settings.Default.DeleteFile;

            if (!Directory.Exists(AllowanceRequestwatcherPath))
                Directory.CreateDirectory(AllowanceRequestwatcherPath);
            if (!Directory.Exists(AllowanceResponsewatcherPath))
                Directory.CreateDirectory(AllowanceResponsewatcherPath);
            if (!Directory.Exists(AllowancePdfwatcherPath))
                Directory.CreateDirectory(AllowancePdfwatcherPath);

            while (true)
            {
                string[] RequestList = Directory.GetFiles(AllowanceRequestwatcherPath, "*.xml");
                string[] ResponseList = Directory.GetFiles(AllowanceResponsewatcherPath, "*.xml");
                string[] PdfList = Directory.GetFileSystemEntries(AllowancePdfwatcherPath, "*.pdf", SearchOption.AllDirectories);

                List<string> RecordCompleteFilesByDB_Request = GetCompleteFiles("RecordAllowance");
                List<string> RecordCompleteFilesByDB_Response = GetCompleteFiles("RecordAllowanceResponse");



                Console.WriteLine("AllowanceRequestFiles.Count:{0}", RequestList.Count());
                int i = 0;
                foreach (var v in RequestList)
                {
                    Console.WriteLine("Do RecordAllowanceRequestFiles:{0}", i++);
                    if (!RecordCompleteFilesByDB_Request.Contains(Path.GetFileName(v)))
                    {
                        rec.RecData(v, RecordHistoryData.TableName.RecordAllowance);
                        if (DelFile)
                        {
                            File.Delete(v);
                        }
                        else
                        {
                            if (File.Exists(v.Replace(".xml", ".xml.bak")))
                            {
                                File.Delete(v.Replace(".xml", ".xml.bak"));
                            }
                            File.Move(v, v.Replace(".xml", ".xml.bak"));
                        }
                    }
                    else
                    {
                        if (File.Exists(v.Replace(".xml", ".xml.bak")))
                        {
                            File.Delete(v.Replace(".xml", ".xml.bak"));
                        }
                        File.Move(v, v.Replace(".xml", ".xml.bak"));
                    }
                    Thread.Sleep(iSingle);
                }
                Thread.Sleep(iPart);


                i = 0;
                Console.WriteLine("AllowanceResponseFiles.Count:{0}", ResponseList.Count());
                foreach (var v in ResponseList)
                {
                    Console.WriteLine("Do RecordAllowanceResponseFiles:{0}", i++);
                    if (!RecordCompleteFilesByDB_Response.Contains(Path.GetFileName(v)))
                    {
                        rec.RecData(v, RecordHistoryData.TableName.RecordAllowanceResponse);
                        if (DelFile)
                        {
                            File.Delete(v);
                        }
                        else
                        {
                            if (File.Exists(v.Replace(".xml", ".xml.bak")))
                            {
                                File.Delete(v.Replace(".xml", ".xml.bak"));
                            }
                            File.Move(v, v.Replace(".xml", ".xml.bak"));
                        }
                    }
                    else
                    {
                        if (File.Exists(v.Replace(".xml", ".xml.bak")))
                        {
                            File.Delete(v.Replace(".xml", ".xml.bak"));
                        }
                        File.Move(v, v.Replace(".xml", ".xml.bak"));
                    }
                    Thread.Sleep(iSingle);
                }
                Thread.Sleep(iPart);



                i = 0;
                Console.WriteLine("PdfList.Count:{0}", PdfList.Count());
                foreach (var v in PdfList)
                {
                    Console.WriteLine("Do PdfList:{0}", i++);

                    string ZipName = Directory.GetParent(v).ToString().Replace(AllowancePdfwatcherPath + "\\", "") + ".Zip";
                    //A.WriteLog(string.Format("ZipName:{0}", ZipName), aBase.LogType.Record, "AllowancePdfFiles");
                    Console.WriteLine(string.Format("ZipName:{0}", ZipName));

                    string FileName = Path.GetFileName(v);
                    //A.WriteLog(string.Format("FileName:{0}", FileName), aBase.LogType.Record, "AllowancePdfFiles");
                    Console.WriteLine(string.Format("FileName:{0}", FileName));

                    if (rec.RecAllowancePdf(ZipName, FileName))
                    {
                        if (DelFile)
                        {
                            File.Delete(v);
                        }
                        else
                        {
                            if (File.Exists(v.Replace(".pdf", ".pdf.bak")))
                            {
                                File.Delete(v.Replace(".pdf", ".pdf.bak"));
                            }
                            File.Move(v, v.Replace(".pdf", ".pdf.bak"));
                        }
                        Thread.Sleep(iSingle);
                    }
                }
                Thread.Sleep(iPart);
            }
        }

        private static List<string> GetCompleteFiles(string strTableName)
        {
            string sqlstring = string.Format("select distinct FileName from {0}", strTableName);
            List<string> result = new List<string>();
            DataTable dt =rec.msSql.GetDataTable(sqlstring, strTableName);

            foreach (DataRow dtrw in dt.Rows)
            {
                result.Add(dtrw["FileName"].ToString());
            }

            return result;
        }
    }
}
