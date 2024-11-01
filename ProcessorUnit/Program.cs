﻿using Model.DataEntity;
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
using System.Threading;
using System.IO;

namespace ProcessorUnit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.OutputWritter = Console.Out;
            Logger.Info($"Process start at {DateTime.Now}");

            /// SSL憑證信任設定
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            InitializeApp.StartUp();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Q);
            Logger.Info("Process terminated..");
        }

        internal static void Terminate()
        {
            Process.GetCurrentProcess().Kill();
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
            //(new InvoiceExcelRequestProcessor
            //{
            //    ChainedExecutor = new InvoiceExcelRequestForCBEProcessor
            //    {
            //        ChainedExecutor = new InvoiceExcelRequestForVACProcessor
            //        {
            //            ChainedExecutor = new InvoiceExcelRequestForIssuerProcessor
            //            {
            //                ChainedExecutor = new VoidInvoiceExcelRequestProcessor
            //                {
            //                    ChainedExecutor = new AllowanceExcelRequestProcessor
            //                    {
            //                        ChainedExecutor = new VoidAllowanceExcelRequestProcessor { },
            //                    },
            //                }
            //            }
            //        }
            //    }
            //}).ReadyToGo();

            ExecutorForeverBase processorStart = new InvoiceExcelRequestProcessor
            {

            };

            ExecutorForeverBase chainedProcessor = new InvoiceExcelRequestForCBEProcessor
            {

            };

            processorStart.ChainedExecutor = chainedProcessor;

            chainedProcessor.ChainedExecutor = new InvoiceExcelRequestForVACProcessor
            {

            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new InvoiceExcelRequestForIssuerProcessor
            {

            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new InvoiceExcelRequestForIssuerA0401Processor
            {

            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new VoidInvoiceExcelRequestProcessor
            {

            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new AllowanceExcelRequestProcessor
            {

            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new AllowanceJsonRequestProcessor
            {
            };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new FullAllowanceExcelRequestProcessor
            {
            };
            chainedProcessor = chainedProcessor.ChainedExecutor;


            chainedProcessor.ChainedExecutor = new VoidAllowanceExcelRequestProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new ProcessExceptionNotificationProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new InvoiceJsonRequestForCBEProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new VoidInvoiceJsonRequestProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new UnassignedInvoiceNOSettlementProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new DataReportProcessor { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            chainedProcessor.ChainedExecutor = new AutoUpdater { };
            chainedProcessor = chainedProcessor.ChainedExecutor;

            processorStart.ReadyToGo();

            //(new InvoiceExcelRequestForIssuerProcessor
            //{

            //}).ReadyToGo();

        }

        //private static void Proc_Exited(object sender, EventArgs e)
        //{
        //    SettingsHelper.Instance.Save();
        //}
    }
}
