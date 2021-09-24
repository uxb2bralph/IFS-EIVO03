using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.CommonLogger;

namespace Utility
{
    public static class ExceptionLogger
    {
        private static LogWritter __Default;

        static ExceptionLogger()
        {
            __Default = new LogWritter("SystemLog.err");
        }

        public static LoggerBase Logger => __Default;
    }
}
