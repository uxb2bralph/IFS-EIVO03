using Model.DataEntity;
using Model.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExtension.Service
{
    public static class PdfDocumentGenerator
    {
        public static Func<RenderStyleViewModel,String> CreateInvoicePdf { get; set; }
    }
}
