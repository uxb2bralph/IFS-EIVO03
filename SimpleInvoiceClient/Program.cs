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
                Console.WriteLine("SimpleInvoiceClient <working path> [<apply invoice date, yyyy/MM/dd>]");
                return;
            }

            String fullPath = args[0];
            DateTime? applyInviceDate = null;
            DateTime d;
            if(args.Length>1 && DateTime.TryParse(args[1],out d))
            {
                applyInviceDate = d;
            }

            Logger.OutputWritter = Console.Out;
            Logger.Info($"Process start at {DateTime.Now}");
            if(applyInviceDate.HasValue)
            {
                Logger.Info($"Apply InvoiceDate at {applyInviceDate}");
            }

            var _PreInvoiceWatcher = new InvoicePGPWatcherForGoogleExpress(Path.Combine(fullPath, Settings.Default.UploadPreInvoiceFolder))
            {
                ApplyInvoiceDate = applyInviceDate,
            };
            _PreInvoiceWatcher.StartUp();

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Q);
        }
    }
}
