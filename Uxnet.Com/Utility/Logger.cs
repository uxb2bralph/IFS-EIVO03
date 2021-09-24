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
        private static Logger _instance;

        private bool _disposed = false;
        private string _path;
        private long _fileID = 0;

        private LogWritter _err, _nfo, _dbg, _wrn;
        private TextWriter _output;

        static Logger()
        {
            _instance = new Logger();
        }

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

            _dbg = new LogWritter(_path, "SystemLog.dbg");
            _err = new LogWritter(_path, "SystemLog.err");
            _nfo = new LogWritter(_path, "SystemLog.nfo");
            _wrn = new LogWritter(_path, "SystemLog.wrn");
        }


        public static TextWriter OutputWritter
        {
            get => _instance._output;
            set
            {
                _instance._output = value;
                if (value != null)
                {
                    _instance._dbg.AppendantWriter.Add(value);
                    _instance._err.AppendantWriter.Add(value);
                    _instance._nfo.AppendantWriter.Add(value);
                    _instance._wrn.AppendantWriter.Add(value);
                }
            }
        }

        public static void Error(object obj)
        {
            String log = _instance.GetLogContent(obj, "err");
            if (log != null)
            {
                _instance._err.WriteLog(log);
            }
        }

        public static void Info(object obj)
        {
            String log = _instance.GetLogContent(obj, "nfo");
            if (log != null)
            {
                _instance._nfo.WriteLog(log);
            }
        }


        public static void Warn(object obj)
        {
            String log = _instance.GetLogContent(obj, "wrn");
            if (log != null)
            {
                _instance._wrn.WriteLog(log);
            }
        }

        public static void Debug(object obj)
        {
            String log = _instance.GetLogContent(obj, "dbg");
            if (log != null)
            {
                _instance._dbg.WriteLog(log);
            }
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

        public static void WriteLog()
        {

        }

        private String GetLogContent(object obj, String qName)
        {
            if (obj == null)
                return null;

            String result = null;
            if (obj is ILog)
            {
                string filePath;
                if (obj is ILog2)
                {
                    filePath = ((ILog2)obj).GetFileName(LogDailyPath, qName, (ulong)Interlocked.Increment(ref _fileID));
                    File.WriteAllText(filePath, obj.ToString(), Encoding.UTF8);
                    return null;
                }
                else
                {
                    filePath = String.Format("{0}\\{1:000000000000}_({3}).{2}", LogDailyPath, Interlocked.Increment(ref _fileID), qName, ((ILog)obj).Subject);
                    result = obj.ToString();

                    File.AppendAllText(filePath, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss}\r\n", Encoding.UTF8);
                    File.AppendAllText(filePath, result, Encoding.UTF8);
                }
            }
            else
            {
                result = obj.ToString();
            }

            return result;
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

                _dbg.Dispose();
                _err.Dispose();
                _nfo.Dispose();
                _wrn.Dispose();

            }
            _disposed = true;
        }

        ~Logger()
        {
            dispose(false);
        }

    }
}
