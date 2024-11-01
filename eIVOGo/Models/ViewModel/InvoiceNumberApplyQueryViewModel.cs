using Model.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eIVOGo.Models.ViewModel
{
    public class InvoiceNumberApplyQueryViewModel: QueryViewModel
    {
        public string BusinessId { get; set; } = string.Empty;
        public DateTime ApplyUpdateTime { get; set; } = DateTime.MinValue;
    }
}