using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InvoiceClient.Properties;
using System.Xml;
using Utility;
using InvoiceClient.Helper;
using Model.Schema.EIVO;
using System.Threading;
using Model.Schema.TXN;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InvoiceClient.Agent
{
    public class InvoiceWatcher : IDisposable
    {
        protected FileSystemWatcher _watcher;
        protected String _failedTxnPath;
        protected String _inProgressPath;
        protected String _ResponsedPath;
        protected String _requestPath;
        protected int _busyCount;
        protected bool _retryConnection;

        protected static List<String> __FailedTxnPath;

        static InvoiceWatcher()
        {
            if (!Settings.Default.AutoRetry && Settings.Default.EnableFailedUploadAlert)
            {
                __FailedTxnPath = new List<string>();
                __SendFailedTxnAlert(10000);

                ThreadPool.QueueUserWorkItem(p =>
                {
                    while (true)
                    {
                        Thread.Sleep(30 * 60 * 1000);
                        __SendFailedTxnAlert(0);
                    }

                });
            }
        }

        public InvoiceWatcher(String fullPath)
        {
            Logger.Info($"watching path:{fullPath}");
            fullPath.CheckStoredPath();
            _requestPath = fullPath;

            _watcher = new FileSystemWatcher(fullPath);
            _watcher.Created += new FileSystemEventHandler(_watcher_Created);
            _watcher.EnableRaisingEvents = true;

            prepareStorePath(fullPath);

            ThreadPool.QueueUserWorkItem(p =>
            {
                while (_watcher != null)
                {
                    _watcher.WaitForChanged(WatcherChangeTypes.Created);
                }
            });

            if(Settings.Default.AutoRetry)
            {
                ThreadPool.QueueUserWorkItem(p =>
                {
                    while (true)
                    {
                        Thread.Sleep(Settings.Default.AutoInvServiceInterval > 0 ? Settings.Default.AutoInvServiceInterval * 60 * 1000 : 1800000);
                        ThreadPool.QueueUserWorkItem(t =>
                        {
                            Retry();
                        });
                    }
                });
            }

        }

        private static void __SendFailedTxnAlert(int delayInMilliseconds)
        {
            ThreadPool.QueueUserWorkItem(p =>
            {
                if (delayInMilliseconds > 0)
                {
                    Thread.Sleep(delayInMilliseconds);
                }

                if (__FailedTxnPath != null && __FailedTxnPath.Count > 0)
                {
                    try
                    {
                        var items = __FailedTxnPath.Select(f => new KeyValuePair<String, int>(f, Directory.GetFiles(f).Length))
                            .Where(v => v.Value > 0).ToArray();
                        if (items.Length > 0)
                        {
                            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();
                            Root token = invSvc.CreateMessageToken(String.Join("\r\n", items.Select(r => r.Key + " => " + r.Value + "筆")));
                            invSvc.AlertFailedTransaction(token.ConvertToXml().Sign());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                }
            });
        }


        protected virtual void prepareStorePath(String fullPath)
        {
            _failedTxnPath = fullPath + "(傳送失敗)";
            _failedTxnPath.CheckStoredPath();

            if (__FailedTxnPath != null)
            {
                __FailedTxnPath.Add(_failedTxnPath);
            }


            _inProgressPath = Path.Combine(fullPath + "(正在處理)", $"{Process.GetCurrentProcess().Id}");
            _inProgressPath.CheckStoredPath();

            if(Settings.Default.ResponseUpload)
            {
                _ResponsedPath = fullPath + "(Response)";
                _ResponsedPath.CheckStoredPath();
            }
        }

        public String ResponsePath
        {
            get
            {
                return _ResponsedPath;
            }
            set
            {
                _ResponsedPath = value.GetEfficientString();
                if (_ResponsedPath != null)
                    _ResponsedPath.CheckStoredPath();
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            InvokeProcess();
        }

        protected InvoiceWatcher _dependentWatcher;
        protected List<InvoiceWatcher> _reponseTo;
        
        public void InitializeDependency(InvoiceWatcher dependentWatcher)
        {
            _dependentWatcher = dependentWatcher;
            dependentWatcher.addResponse(this);
        }

        private void addResponse(InvoiceWatcher response)
        {
            if (_reponseTo == null)
                _reponseTo = new List<InvoiceWatcher>();
            if (!_reponseTo.Contains(response))
            {
                _reponseTo.Add(response);
            }
        }

        private void doProcess()
        {
            if (Interlocked.Increment(ref _busyCount) == 1)
            {
                ThreadPool.QueueUserWorkItem(stateInfo =>
                {
                    //do
                    //{
                    try
                    {
                        Thread.Sleep(5000);
                        String[] files;
                        bool done = false;
                        do
                        {
                            files = Directory.GetFiles(_watcher.Path);
                            if (files != null && files.Count() > 0)
                            {
                                done = true;

                                processBatchFiles(files);

                                //if (Settings.Default.FileWatcherProcessCount > 1)
                                //{
                                //    int start = 0;
                                //    while (start < files.Length)
                                //    {
                                //        int end = Math.Min(start + Settings.Default.FileWatcherProcessCount, files.Length);
                                //        Parallel.For(start, end, (idx) =>
                                //          {
                                //              Logger.Info($"Upload Invoide[{idx}]:{files[idx]}");
                                //              processFile(files[idx]);
                                //          });
                                //        start = end;
                                //    }
                                //}
                                //else
                                //{
                                //    foreach (String fullPath in files)
                                //    {
                                //        processFile(fullPath);
                                //    }
                                //}
                            }
                        } while (files != null && files.Count() > 0);

                        if (done)
                        {
                            processComplete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    Interlocked.Exchange(ref _busyCount, 0);
                    //} while (Interlocked.Decrement(ref _busyCount) > 0);

                    if (_reponseTo != null)
                    {
                        foreach (var response in _reponseTo)
                        {
                            response.doProcess();
                        }
                    }
                });
            }
        }

        protected virtual void processBatchFiles(string[] files)
        {
            if (Settings.Default.FileWatcherProcessCount > 0)
            {
                //IEnumerable<String> workingList;
                //var mode = DateTime.Now.Ticks % Settings.Default.FileWatcherProcessCount + 1;
                //workingList = files
                //    .OrderBy(f => Path.GetFileName(f).Sum(c => (int)c) % mode)
                //    .Take(Settings.Default.FileWatcherProcessCount);

                int start = 0;
                while (start < files.Length)
                {
                    int end = Math.Min(start + Settings.Default.FileWatcherProcessCount, files.Length);
                    Parallel.For(start, end, (idx) =>
                    {
                        Logger.Info($"process file[{idx}]:{files[idx]}");
                        processFile(files[idx]);
                    });
                    start = end;
                }

                //Parallel.ForEach(workingList, fullPath =>
                //{
                //    Logger.Info($"process file:{fullPath}");
                //    processFile(fullPath);
                //});

            }
            else
            {
                foreach (String fullPath in files)
                {
                    processFile(fullPath);
                }
            }
        }

        public void InvokeProcess()
        {
            if (_dependentWatcher != null)
                _dependentWatcher.InvokeProcess();
            else
                doProcess();
        }

        protected virtual void processComplete()
        {
            WS_Invoice.eInvoiceService invSvc = InvoiceWatcher.CreateInvoiceService();

            try
            {
                Root token = this.CreateMessageToken("資料已傳送完成－通知相對營業人");
                invSvc.NotifyCounterpartBusiness(token.ConvertToXml().Sign());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        protected virtual void processFile(String invFile)
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

            WS_Invoice.eInvoiceService invSvc = CreateInvoiceService();
            Root result = new Root();
            try
            {
                XmlDocument docInv = prepareInvoiceDocument(fullPath);

                docInv.Sign();
                result = processUpload(invSvc, docInv);

                if (result.Result.value != 1)
                {
                    if (result.Response != null && result.Response.InvoiceNo != null && result.Response.InvoiceNo.Length > 0)
                    {
                        processError(result.Response.InvoiceNo, docInv, fileName);
                        storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                    }
                    else
                    {
                        processError(result.Result.message, docInv, fileName);
                        storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
                    }
                }
                else
                {
                    storeFile(fullPath, Path.Combine(Logger.LogDailyPath, fileName));
                }
            }
            catch(System.Net.WebException ex)
            {
                Logger.Error(ex);
                if (_retryConnection)
                {
                    storeFile(fullPath, Path.Combine(_requestPath, fileName));
                }
                else
                {
                    storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                storeFile(fullPath, Path.Combine(_failedTxnPath, fileName));
            }
            finally
            {
                if (Settings.Default.ResponseUpload && result.Automation != null)
                {
                    Automation auto = new Automation { Item = result.Automation };
                    auto.ConvertToXml().Save(Path.Combine(_ResponsedPath, fileName));
                }
            }

        }

        protected virtual XmlDocument prepareInvoiceDocument(String invoiceFile)
        {
            XmlDocument docInv = new XmlDocument();
            docInv.Load(invoiceFile);
            ///去除"N/A"資料
            ///
            var nodes = docInv.SelectNodes("//*[text()='N/A']");
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes.Item(i);
                node.RemoveChild(node.SelectSingleNode("text()"));
            }
            ///
            return docInv;
        }

        protected virtual void processError(string message, XmlDocument docInv, string fileName)
        {
            Logger.Warn(String.Format("在上傳發票檔({0})時,傳送失敗!!原因如下:\r\n{1}", fileName, message));
        }

        public static WS_Invoice.eInvoiceService CreateInvoiceService()
        {
            WS_Invoice.eInvoiceService invSvc = new WS_Invoice.eInvoiceService();
            invSvc.Url = Settings.Default.InvoiceClient_WS_Invoice_eInvoiceService;
            invSvc.Timeout = Settings.Default.WS_TimeoutInMilliSeconds;
            return invSvc;
        }

        protected virtual Root processUpload(WS_Invoice.eInvoiceService invSvc, XmlDocument docInv)
        {
            var result = invSvc.UploadInvoice(docInv).ConvertTo<Root>();
            return result;
        }

        protected void storeFile(String srcName,String destName)
        {
            try
            {
                if (File.Exists(destName))
                {
                    File.Delete(destName);
                }
                File.Move(srcName, destName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        protected virtual void processError(IEnumerable<RootResponseInvoiceNo> rootInvoiceNo, XmlDocument docInv, string fileName)
        {
            if (rootInvoiceNo != null && rootInvoiceNo.Count() > 0)
            {
                IEnumerable<String> message = rootInvoiceNo.Select(i => String.Format("發票號碼:{0}=>{1}", i.Value, i.Description));
                Logger.Warn(String.Format("在上傳發票檔({0})時,傳送失敗!!原因如下:\r\n{1}", fileName, String.Join("\r\n", message.ToArray())));

                InvoiceRoot invoice = docInv.ConvertTo<InvoiceRoot>();
                InvoiceRoot stored = docInv.ConvertTo<InvoiceRoot>();
                stored.Invoice = rootInvoiceNo.Where(i=>i.ItemIndexSpecified).Select(i=>invoice.Invoice[i.ItemIndex]).ToArray();

                stored.ConvertToXml().Save(Path.Combine(_failedTxnPath, Path.GetFileName(fileName)));
            }
        }

        public virtual String ReportError()
        {
            int count = Directory.GetFiles(_failedTxnPath).Length;
            return count > 0 ? String.Format("{0}筆發票資料傳送失敗!!\r\n", count) : null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        #endregion

        public void Retry()
        {
            foreach (String fileName in Directory.GetFiles(_failedTxnPath))
            {
                File.Move(fileName, Path.Combine(_watcher.Path, Path.GetFileName(fileName)));
            }
        }

        public void StartUp()
        {
            //foreach (String filePath in Directory.GetFiles(_watcher.Path))
            //{
            //    processFile(filePath);
            //}
            InvokeProcess();
        }
    }
}
