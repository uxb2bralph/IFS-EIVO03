using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TestConnection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Usage:");
                Console.WriteLine($"TestConnection <Uri>");
                return;
            }

            bool DoTest()
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        Console.WriteLine($"proxy => {client.Proxy.GetProxy(new Uri(args[0]))}");
                        var result = client.DownloadString(args[0]);
                        Console.WriteLine("connection successful !!");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            if (!DoTest())
            {
                Console.WriteLine("Use System Proxy...");
                WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
                DoTest();
            }

        }
    }
}
