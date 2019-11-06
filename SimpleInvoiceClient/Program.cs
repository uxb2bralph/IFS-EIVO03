using InvoiceClient.Agent;
using InvoiceClient.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace SimpleInvoiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args==null || args.Length<1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("SimpleInvoiceClient <working path>");
                return;
            }

            String fullPath = args[0];

            Logger.OutputWritter = Console.Out;
            Logger.Info($"Process start at {DateTime.Now}");

            var _PreInvoiceWatcher = new InvoicePGPWatcherForGoogleExpress(Path.Combine(fullPath, Settings.Default.UploadPreInvoiceFolder));
            _PreInvoiceWatcher.StartUp();

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Q);
        }
    }
}
