using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Uxnet.Com.Helper
{
    public static class FileIOExtensions
    {
        public static  void StoreFile(this String srcName, String destName)
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
    }
}
