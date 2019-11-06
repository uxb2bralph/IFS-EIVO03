using Model.DataEntity;
using ProcessorUnit.Execution;
using ProcessorUnit.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Model.ProcessorUnitHelper;

namespace ProcessorUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.OutputWritter = Console.Out;
            Logger.Info($"Process start at {DateTime.Now}");

            InitializeApp.StartUp();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Q);
            Logger.Info("Process terminated..");
        }
    }

    class InitializeApp
    {
        private static InitializeApp _instance = new InitializeApp();
        InitializeApp()
        {
            Initialize();
        }

        private void Initialize()
        {
            using(ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var item = SettingsHelper.Instance.InstanceID.RegisterProcessorUnit(models);
                SettingsHelper.Instance.ProcessorID = item.ProcessorID;
                Console.WriteLine($"Instance ID:{SettingsHelper.Instance.InstanceID}");
                Console.WriteLine($"Processor ID:{item.ProcessorID}");
            }
            Console.WriteLine($"Settings:{SettingsHelper.Instance.Save()}");
        }


        public static void StartUp()
        {
            //Process proc = Process.GetCurrentProcess();
            //proc.EnableRaisingEvents = true;
            //proc.Exited += Proc_Exited;
            (new InvoiceExcelRequestProcessor
            {
                ChainedExecutor = new InvoiceExcelRequestForCBEProcessor
                {
                    ChainedExecutor = new InvoiceExcelRequestForVACProcessor
                    {
                        ChainedExecutor = new InvoiceExcelRequestForIssuerProcessor
                        {
                            ChainedExecutor = new VoidInvoiceExcelRequestProcessor
                            {
                                ChainedExecutor = new AllowanceExcelRequestProcessor
                                {
                                    ChainedExecutor = new VoidAllowanceExcelRequestProcessor { },
                                },
                            }
                        }
                    }
                }
            }).ReadyToGo();
        }

        //private static void Proc_Exited(object sender, EventArgs e)
        //{
        //    SettingsHelper.Instance.Save();
        //}
    }
}
