﻿using Model.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Schema.TXN
{
    [Serializable]
    public class ServiceInfo
    {
        public String AgentToken { get; set; }
        public String TaskCenterUrl { get; set; }
        public int? AgentUID { get; set; }
        public Naming.InvoiceProcessType? DefaultProcessType { get; set; }
    }
}
