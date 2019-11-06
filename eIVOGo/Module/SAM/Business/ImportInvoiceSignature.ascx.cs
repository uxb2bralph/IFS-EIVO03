using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.SAM.Business
{
    public partial class ImportInvoiceSignature : EditEntityItemBase<EIVOEntityDataContext, Organization>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override bool saveEntity()
        {
            if (!imgFile.HasFile)
            {
                this.AjaxAlert("請匯入印鑑圖檔!!");
                return false;
            }

            String fileName = String.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(imgFile.FileName));
            imgFile.SaveAs(Path.Combine(Server.MapPath("~/Seal"), fileName));

            var mgr = dsEntity.CreateDataManager();
            loadEntity();

            if (_entity != null)
            {
                _entity.InvoiceSignature = fileName;
                mgr.SubmitChanges();
            }

            return true;
        }

    }
}