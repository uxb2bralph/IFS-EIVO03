using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;

using eIVOGo.Properties;
using Model.DataEntity;
using Model.DocumentManagement;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO.B2B;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Newtonsoft.Json;
using System.Data;
using ModelExtension.Helper;
using Uxnet.Com.DataAccessLayer;
using DataAccessLayer.basis;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;
using System.Security;

namespace eIVOGo.Controllers.SAM
{
    public class CertificateIdentityController : SampleController<InvoiceItem>
    {
        // GET: SystemExceptionLog
        public ActionResult OrganizationCertificate(OrganizationViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
            }

            Organization item = models.GetTable<Organization>()
                .Where(u => u.CompanyID == viewModel.CompanyID)
                .FirstOrDefault();

            if (item == null)
            {
                return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DataItem = item;

            return View("~/Views/SAM/CertificateIdentity/OrganizationCertificate.cshtml", item);
        }

        public ActionResult CommitItem(OrganizationCertificateViewModel viewModel)
        {
            if (viewModel.PfxFile == null)
            {
                return Json(new { result = false, message = "未選取檔案或檔案上傳失敗" }, JsonRequestBehavior.AllowGet);
            }

            var result = OrganizationCertificate(viewModel);

            Organization item = ViewBag.DataItem as Organization;
            if (item == null)
            {
                return result;
            }

            try
            {
                KeyContainerPermission perm = new KeyContainerPermission(KeyContainerPermissionFlags.Open | KeyContainerPermissionFlags.Export);
                perm.Assert();

                byte[] buf = new byte[viewModel.PfxFile.ContentLength];
                viewModel.PfxFile.InputStream.Read(buf, 0, viewModel.PfxFile.ContentLength);
                X509Certificate2 cert = new X509Certificate2(buf, viewModel.PIN, X509KeyStorageFlags.Exportable);

                if (item.OrganizationToken == null)
                {
                    item.OrganizationToken = new OrganizationToken { };
                }

                Guid keyID = Guid.NewGuid();
                item.OrganizationToken.X509Certificate = Convert.ToBase64String(cert.RawData);
                item.OrganizationToken.Thumbprint = cert.Thumbprint;
                item.OrganizationToken.KeyID = keyID;
                item.OrganizationToken.PKCS12 = Convert.ToBase64String(cert.Export(X509ContentType.Pkcs12, keyID.ToString().Substring(0, 8)));

                models.SubmitChanges();
                return Json(new { result = true, message = $"更新憑證金鑰:{item.OrganizationToken.KeyID}" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                CodeAccessPermission.RevertAll();
            }
        }
    }
}