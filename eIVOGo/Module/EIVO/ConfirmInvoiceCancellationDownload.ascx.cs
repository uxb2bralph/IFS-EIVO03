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
    public partial class ConfirmInvoiceCancellationDownload : ConfirmInvoiceDownload
    {


        protected override void prepareData()
        {
            if (InvoiceID.HasValue)
            {
                var item = dsEntity.CreateDataManager().EntityList.Where(a => a.InvoiceID == InvoiceID).FirstOrDefault();
                if (item != null)
                {
                    StringBuilder sb = new StringBuilder("您欲授權作廢發票資料可再下載成大平台傳輸格式:\r\n");
                    sb.Append("作業時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                    sb.Append("發票號碼:").Append(item.TrackCode).Append(item.No).Append("\r\n");

                    dataToSign.Text = sb.ToString();
                }
            }
        }


        protected override void save()
        {
            var mgr = dsEntity.CreateDataManager();
            var item = dsEntity.CreateDataManager().EntityList.Where(a => a.InvoiceID == InvoiceID).FirstOrDefault();
            if (item != null)
                item.CDS_Document.ResetDocumentDispatch(Naming.DocumentTypeDefinition.E_InvoiceCancellation);
            mgr.SubmitChanges();
        }


    }
}