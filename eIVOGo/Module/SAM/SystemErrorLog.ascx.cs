﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.DataEntity;
using Model.Locale;
using Model.InvoiceManagement;
using System.Threading;
using Utility;
using eIVOGo.Properties;
using eIVOGo.services;

namespace eIVOGo.Module.SAM
{
    public partial class SystemErrorLog : System.Web.UI.UserControl
    {
        protected static bool __AUTO_TRANSFER_GOV_PLATFORM;
        protected static Thread __BackgroundService = new Thread(t => {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                try
                {
                    ServiceWorkItem.NotifyGovPlatform();
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadInterruptedException ex)
                {

                }
                catch (ThreadAbortException ex)
                { }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }    
                
                ServiceWorkItem.Reset();
            }
        });

        public static void StartUp()
        { 
        }

        public static Thread BackgroundService
        {
            get
            {
                return __BackgroundService;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

    }
}