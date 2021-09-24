using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Uxnet.Com.Helper;
using Utility.CommonLogger;

namespace Utility
{
    public class LogWritter : LoggerBase
    {
        private DateTime _daily = DateTime.Today;
        private String _logFile;

        private List<TextWriter> _appendantWriter;

        public LogWritter(string subject) : this(Utility.Logger.LogPath, subject)
        {

        }

        public LogWritter(String path, string subject) : base()
        {
            _subject = subject.GetEfficientString() ?? "App.log";
            _logFile = Path.Combine(ValueValidity.GetDateStylePath(path), _subject);
        }


        public List<TextWriter> AppendantWriter
        {
            get
            {
                if (_appendantWriter == null)
                {
                    _appendantWriter = new List<TextWriter>();
                }
                return _appendantWriter;
            }
        }

        protected override void DoFlushLog()
        {
            if (_daily < DateTime.Today)
            {
                _daily = DateTime.Today;
                _logFile = Path.Combine(Utility.Logger.LogDailyPath, _subject);
            }

            File.AppendAllText(_logFile, _standBy.ToString(), Encoding.UTF8);

            if (_appendantWriter != null && _appendantWriter.Count > 0)
            {
                foreach (var w in _appendantWriter)
                {
                    w.Write(_standBy.ToString());
                }
            }
        }

    }
}
