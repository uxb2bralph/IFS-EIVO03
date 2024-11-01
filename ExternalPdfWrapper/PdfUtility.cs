using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Uxnet.ToolAdapter.Common;
using Uxnet.ToolAdapter.Properties;

namespace ExternalPdfWrapper
{
    public class PdfUtility : IPdfUtility2
    {
        static PdfUtility()
        {
            AppSettings.Reload();
        }

        //public void ConvertHtmlToImage(string htmlFile, string imgFile, double timeOutInMinute)
        //{
        //    ProcessStartInfo info = new ProcessStartInfo
        //    {
        //        FileName = AppSettings.Default.Command,
        //        Arguments = String.Concat(" ",
        //                        String.Format(AppSettings.Default.ConvertPattern, pdfFile, htmlSource.Contains("://") ? htmlSource : $"file://{htmlSource}")/*,
        //                        " ",
        //                        args != null && args.Length > 0 ? String.Join(" ", args) : ""*/),
        //        CreateNoWindow = true,
        //        UseShellExecute = false,
        //        WindowStyle = ProcessWindowStyle.Hidden,
        //        //WorkingDirectory = AppDomain.CurrentDomain.RelativeSearchPath,
        //    };

        //    Process proc = new Process();
        //    proc.EnableRaisingEvents = true;
        //    //proc.Exited += new EventHandler(proc_Exited);

        //    //if (null != _eventHandler)
        //    //{
        //    //    proc.Exited += new EventHandler(_eventHandler);
        //    //}
        //    proc.StartInfo = info;
        //    proc.Start();
        //    proc.WaitForExit((int)timeOutInMinute * 60000);
        //}

        public void ConvertHtmlToPDF(string htmlSource, string pdfFile, double timeOutInMinute, string[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = AppSettings.Default.Command,
                Arguments = String.Concat(" ",
                                String.Format(AppSettings.Default.ConvertPattern, pdfFile, htmlSource.Contains("://") ? htmlSource : $"file://{htmlSource}")/*,
                                " ",
                                args != null && args.Length > 0 ? String.Join(" ", args) : ""*/),
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                //WorkingDirectory = AppDomain.CurrentDomain.RelativeSearchPath,
            };

            Process proc = new Process();
            proc.EnableRaisingEvents = true;
            //proc.Exited += new EventHandler(proc_Exited);

            //if (null != _eventHandler)
            //{
            //    proc.Exited += new EventHandler(_eventHandler);
            //}
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit((int)timeOutInMinute * 60000);

        }

        public void ConvertHtmlToPDF(string htmlFile, string pdfFile, double timeOutInMinute)
        {
            ConvertHtmlToPDF(htmlFile, pdfFile, timeOutInMinute, null);
        }
    }

    public class AppSettings : AppSettingsBase
    {
        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings() : base()
        {

        }

        static AppSettings _default;
        public static AppSettings Default
        {
            get
            {
                return _default;
            }
        }

        public static void Reload()
        {
            Reload<AppSettings>(ref _default, typeof(AppSettings).Namespace);
        }

        public String Command { get; set; } = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        public String ConvertPattern { get; set; } = "--headless --disable-gpu --run-all-compositor-stages-before-draw --print-to-pdf-no-header  --print-to-pdf={0} {1}";
        public String Args { get; set; }
        public String ScreenshotPattern { get; set; } = "--headless --disable-gpu --run-all-compositor-stages-before-draw --print-to-pdf-no-header  --print-to-pdf={0} {1}";
    }
}
