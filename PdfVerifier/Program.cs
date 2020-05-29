using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model.InvoiceManagement;

namespace PdfVerifier
{
    class Program
    {

        static FunMsSql sql;

        static void Main(string[] args)
        {
            sql = new FunMsSql();
            Run();
        }

        private static FileSystemWatcher watcher = new FileSystemWatcher();
        private static Queue<string> Files = new Queue<string>();

        private static void Run()
        {
            string watcherPath = Properties.Settings.Default.watcherPath;
            string resultPath = Properties.Settings.Default.resultPath;
            watcher.Path = watcherPath;

            //設定是否監控子資料夾
            watcher.IncludeSubdirectories = true;

            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            watcher.EnableRaisingEvents = true;

            watcher.Created += new FileSystemEventHandler(watch_Created);

            DataTable dtError = sql.GetDataTable("select * from PdfVerifier", "PdfVerifier");
            while (true)
            {
                Thread.Sleep(1000);
                if (Files.Count > 0)
                {
                    Action<object> action = (object obj) =>
                    {
                        if (Files.Count > 0)
                        {
                            Thread.Sleep(300);
                            string FilePath = "";
                            while (Files.Count > 0)
                            {
                                FilePath = Files.Dequeue();
                                if (Path.GetExtension(FilePath) == ".pdf")
                                    break;
                            };
                            if (CheckPdf(FilePath))
                            {
                                if (File.Exists(FilePath.Replace(watcherPath, resultPath)))
                                {
                                    File.Delete(FilePath);
                                }
                                else
                                {
                                    File.Move(FilePath, FilePath.Replace(watcherPath, resultPath));
                                }
                                Console.WriteLine("Move Complete. File：{0}", FilePath);
                            }
                            else
                            {
                                Console.WriteLine("Record Error Msg and Insert DB. File：{0}", FilePath);

                                DataRow dtrw = dtError.Select(string.Format("PdfName = '{0}'", Path.GetFileName(FilePath))).FirstOrDefault();

                                DataTable Queue = sql.GetDataTable("select * from DocumentSubscriptionQueue where 1=2", "DocumentSubscriptionQueue");

                                if (dtrw is null || dtrw["PdfName"].ToString() == "")
                                {
                                    DataRow dtrwNew = dtError.NewRow();
                                    dtrwNew["PdfName"] = Path.GetFileName(FilePath);
                                    dtrwNew["ErrorTime"] = 1;

                                    dtError.Rows.Add(dtrwNew);

                                    DataRow dtrwQueue = Queue.NewRow();
                                    dtrwQueue["DocID"] = GetDocID(Path.GetFileNameWithoutExtension(FilePath));
                                    dtrwQueue["Status"] = "0";

                                    Queue.Rows.Add(dtrwQueue);


                                    sql.BeginTransaction();
                                    try
                                    {
                                        sql.InsData(Queue, "DocumentSubscriptionQueue");
                                        sql.InsData(dtError, "PdfVerifier");
                                        sql.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        sql.Rollback();
                                        Console.WriteLine("Send DB Notification {0}", ex);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Send Notification");
                                }

                            }
                        }
                    };
                    Task verifier = new Task(action, "verifier");
                    verifier.Start();
                }
            }
        }

        private static int GetDocID(string PdfName)
        {
            string Sqlstring = "select InvoiceID from InvoiceItem where TrackCode=@TrackCode and No=@No";

            DataTable dt = sql.GetDataTable(Sqlstring, "InvoiceItem", "@TrackCode", PdfName.Substring(0, 2), "@No", PdfName.Substring(2));

            if (dt.Rows.Count < 1)
            {
                return 0;
            }
            else
            {
                return int.Parse(dt.Rows[0]["InvoiceID"].ToString());
            }
        }

        private static void watch_Created(object sender, FileSystemEventArgs e)
        {
            Files.Enqueue(e.FullPath);
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
                r = null;
                fs.Close();//真實的檔案
                fs = null;
            }
            return bx;
        }

    }
}
