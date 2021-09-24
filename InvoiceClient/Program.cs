using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Configuration.Install;
using Utility;
using System.ServiceProcess;
using System.IO;
using InvoiceClient.Properties;
using System.Threading;
using InvoiceClient.Agent;
using Model.Resource;

namespace InvoiceClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {     
            /// SSL憑證信任設定
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            if (!String.IsNullOrEmpty(Settings.Default.AppCulture))
            {
                Thread.CurrentThread.CurrentUICulture = MessageResources.Culture = System.Globalization.CultureInfo.GetCultureInfo(Settings.Default.AppCulture);
            }

            if (Environment.UserInteractive /*|| Debugger.IsAttached*/)
            {
                if (Settings.Default.ClearTxnPath
                        && Directory.Exists(Settings.Default.InvoiceTxnPath))
                    ClearDirectory();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                ServiceBase[] services = 
                    {
                        new InvoiceClientService() 
                    };
                ServiceBase.Run(services);
            }
            
        }

        internal static void Install(bool undo, string[] args)
        {
            try
            {
                using (AssemblyInstaller inst = new AssemblyInstaller(typeof(Program).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            inst.Uninstall(state);
                            MessageBox.Show("服務已移除!!", "服務設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            inst.Install(state);
                            inst.Commit(state);
                            MessageBox.Show("服務安裝成功!!", "服務設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch
                        {
                        }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("服務安裝失敗:\r\n" + ex.Message, "服務設定", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void ClearDirectory()
        {
            foreach(var item in Directory.GetDirectories(Settings.Default.InvoiceTxnPath))
            {
                try
                {
                    Directory.Delete(item);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
