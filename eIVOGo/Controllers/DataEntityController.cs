using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using eIVOGo.Helper;
using Model.DataEntity;
using Model.Locale;
using Newtonsoft.Json;
using Utility;

namespace eIVOGo.Controllers
{
    public class DataEntityController : SampleController<InvoiceItem>
    {
        // GET: DataFlow
        public ActionResult Organization(int? id, int? masterID)
        {
            BusinessRelationship relation = null;
            if (masterID.HasValue)
            {
                relation = models.GetTable<BusinessRelationship>()
                    .Where(r => r.MasterID == masterID)
                    .Where(r => r.RelativeID == id).FirstOrDefault();
            }

            if (relation != null)
            {
                var orgItem = relation.Counterpart;
                return Json(new
                {
                    orgItem.ReceiptNo,
                    relation.CompanyName,
                    relation.ContactEmail,
                    relation.Addr,
                    relation.Phone,
                }, JsonRequestBehavior.AllowGet);
            }

            var item = models.GetTable<Organization>().Where(o => o.CompanyID == id).FirstOrDefault();
            return Content(item.JsonStringify(), "application/json");
        }

        public ActionResult OrganizationExtension(int id)
        {
            var item = models.GetTable<OrganizationExtension>().Where(o => o.CompanyID == id).FirstOrDefault();
            return Content(item.JsonStringify(), "application/json");
        }

    }
}