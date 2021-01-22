using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;

using Uxnet.Com.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Logger 的摘要描述。
    /// </summary>
    public class Logger : IDisposable
    {
        private static Logger _instance = new Logger();

        private Dictionary<String, Queue> _hashQ;
        private Dictionary<String, Queue> _standbyQ;

        private bool _disposed = false;
        private string _path;
        private ulong _fileID = 0;
        private Stream _stream;
        private int _waiting = 10;

        public readonly string[] LoggingLevel = new string[] 
        {
            "err",
            "nfo",
            "dbg",
            "wrn"
        };

        private Logger()
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
            if (!String.IsNullOrEmpty(Settings.Default.LogPath))
                _path = Settings.Default.LogPath;
            else
                _path = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "logs");

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            _hashQ = new Dictionary<string, Queue>();
            foreach(var qName in LoggingLevel)
            {
                _hashQ.Add(qName, new Queue());
            }

            _standbyQ = new Dictionary<string, Queue>();
            foreach (var qName in LoggingLevel)
            {
                _standbyQ.Add(qName, new Queue());
            }

            run();
        }

        public static void Shutdown()
        {

        }

        public static TextWriter OutputWritter { get; set; }

        public static void Error(object obj)
        {
            _instance._hashQ["err"].Enqueue(obj);
        }

        public static void Info(object obj)
        {
            _instance._hashQ["nfo"].Enqueue(obj);
        }


        public static void Warn(object obj)
        {
            _instance._hashQ["wrn"].Enqueue(obj);
        }

        public static void Debug(object obj)
        {
            _instance._hashQ["dbg"].Enqueue(obj);
        }

        public static string LogPath
        {
            get
            {
                return _instance._path;
            }
        }
        
        public static string LogDailyPath
        {
            get
            {
                string filePath = ValueValidity.GetDateStylePath(_instance._path);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                return filePath;
            }
        }

        private void run()
        {
            var t = Task.Run(() =>
            {
                writeLog();
            });

            t = t.ContinueWith(ts =>
            {
                Task.Delay(_waiting).ContinueWith(ts1 =>
                {
                    run();
                });
            });
        }

        public static void WriteLog()
        {

        }

        public static void SetStream(Stream stream)
        {
            _instance._stream = stream;
        }

        private void writeLog()
        {
            bool hasContent = false;
            foreach (var  qName in LoggingLevel)
            {
                Queue workingQ = _hashQ[qName];

                if (workingQ.Count == 0)
                    continue;

                hasContent = true;

                _hashQ[qName] = _standbyQ[qName];
                _standbyQ[qName] = workingQ;

                while (workingQ.Count > 0)
                {
                    object obj = workingQ.Dequeue();
                    if (obj == null)
                        continue;
                    string filePath = LogDailyPath;

                    StringBuilder sb = null;

                    if (obj is ILog)
                    {
                        if (obj is ILog2)
                        {
                            filePath = ((ILog2)obj).GetFileName(filePath, qName, _fileID++);
                            sb = new StringBuilder(obj.ToString());
                        }
                        else
                        {
                            filePath = String.Format("{0}\\{1:000000000000}_({3}).{2}", filePath, _fileID++, qName, ((ILog)obj).Subject);
                        }
                    }
                    else
                    {
                        filePath = String.Format("{0}\\SystemLog.{1}", filePath, qName);
                    }

                    if (OutputWritter != null)
                    {
                        if (sb == null)
                        {
                            OutputWritter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            OutputWritter.WriteLine(obj.ToString());
                        }
                        else
                        {
                            OutputWritter.WriteLine(sb.ToString());
                        }
                    }

                    using (StreamWriter sw = (_stream == null ? new StreamWriter(filePath, true) : new StreamWriter(_stream)))
                    {
                        if (sb == null)
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            sw.WriteLine(obj.ToString());
                        }
                        else
                        {
                            sw.WriteLine(sb.ToString());
                        }
                        sw.Flush();
                        //                        sw.Close();
                    }
                }
            }

            if (hasContent)
            {
                _waiting = 10;
            }
            else
            {
                _waiting = 5000;
            }
        }

        
        #region IDisposable 成員

        public void Dispose()
        {
            // TODO:  加入 Logger.Dispose 實作
            dispose(true);
        }

        #endregion

        protected void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("Object is disposing now ...");
                }
                else
                {
                    Console.WriteLine("May destructor run ...");
                }
                writeLog();
            }
            _disposed = true;
        }

        ~Logger()
        {
            dispose(false);
        }

    }
}
