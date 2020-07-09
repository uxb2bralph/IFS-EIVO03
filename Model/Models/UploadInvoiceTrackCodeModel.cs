using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models.ViewModel;

namespace Model.Models
{
    public class UploadInvoiceTrackCodeModel : CommonQueryViewModel
    {
        public string ReceiptNo { get; set; }
        public short Year { get; set; }
        public int PeriodNo { get; set; }
        public string TrackCode { get; set; }
        public int? StartNo { get; set; }

        public int? EndNo { get; set; }
    }
}
