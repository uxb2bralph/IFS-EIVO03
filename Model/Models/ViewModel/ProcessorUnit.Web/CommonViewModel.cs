using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models.ViewModel.ProcessorUnit.Web
{
    public class CommonViewModel : CommonQueryViewModel
    {
        public class DataSystems : QueryViewModel
        {
            public int? AgentID { get; set; }
            public int? Sender { get; set; }
            public int? TaskID { get; set; }
            public String Comment { get; set; }
          
        }
    }
}
