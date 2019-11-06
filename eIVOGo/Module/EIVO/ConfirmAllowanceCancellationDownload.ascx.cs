using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using System.Drawing;
using Model.Locale;

namespace eIVOGo.Module.EIVO
{
    public partial class ConfirmAllowanceCancellationDownload : ConfirmInvoiceDownload
    {

        public int? AllowanceID
        {
            get
            {
                return (int?)ViewState["allowanceID"];
            }
            set
            {
                ViewState["allowanceID"] = value;
            }
        }

        private new int? InvoiceID { get; set; }
        
        protected override void prepareData()
        {
            if (AllowanceID.HasValue)
            {
                var item = dsEntity.CreateDataManager().GetTable<InvoiceAllowance>().Where(a => a.AllowanceID == AllowanceID).FirstOrDefault();
                if (item != null)
                {
                    StringBuilder sb = new StringBuilder("您欲授權作廢發票折讓資料可再下載成大平台傳輸格式:\r\n");
                    sb.Append("作業時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                    sb.Append("折讓證明單號碼:").Append(item.AllowanceNumber).Append("\r\n");

                    dataToSign.Text = sb.ToString();
                }
            }
        }

     
        protected override void save()
        {
            var mgr = dsEntity.CreateDataManager();
            var item = dsEntity.CreateDataManager().GetTable<InvoiceAllowance>().Where(a => a.AllowanceID == AllowanceID).FirstOrDefault();
            if (item != null)
                item.CDS_Document.ResetDocumentDispatch(Naming.DocumentTypeDefinition.E_AllowanceCancellation);
            mgr.SubmitChanges();
        }
    }
}