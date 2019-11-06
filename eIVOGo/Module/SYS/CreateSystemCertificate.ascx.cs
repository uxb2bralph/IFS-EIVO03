using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using eIVOGo.Helper;
using eIVOGo.Module.Entity;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Uxnet.Web.WebUI;
using Model.DataEntity;

namespace eIVOGo.Module.SYS
{
    public partial class CreateSystemCertificate : OrganizationEntrustingTokenItem
    {

        protected override bool saveEntity()
        {
            if (base.saveEntity())
            {
                var mgr = dsEntity.CreateDataManager();
                var token = mgr.GetTable<UserToken>().Where(t => t.Token == _tokenID).First();

                if (_entity.OrganizationToken == null)
                {
                    _entity.OrganizationToken = new OrganizationToken { };
                }

                _entity.OrganizationToken.X509Certificate = token.X509Certificate;
                _entity.OrganizationToken.Thumbprint = token.Thumbprint;
                _entity.OrganizationToken.PKCS12 = token.PKCS12;

                mgr.SubmitChanges();

                _certificate = new X509Certificate2(Convert.FromBase64String(token.PKCS12), _tokenID.ToString().Substring(0, 8), X509KeyStorageFlags.Exportable);
                AppSigner.UpdateSigner(_certificate.Export(X509ContentType.Pkcs12));

                this.AjaxAlert("系統簽章憑證已更新!!");

                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(CreateSystemCertificate_PreRender);
        }

        void CreateSystemCertificate_PreRender(object sender, EventArgs e)
        {
            if (AppSigner.SignerCertificate != null)
            {
                msg.Text = AppSigner.SignerCertificate.Subject;
            }
        }

        protected void lbViewCert_Click(object sender, EventArgs e)
        {
            if (this.ViewState["signerCert"] != null)
            {
                X509Certificate cert = (X509Certificate)this.ViewState["signerCert"];
                byte[] certRaw = cert.GetRawCertData();

                Response.Clear();
                Response.ContentType = "message/rfc822";
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", "Signer.cer"));
                Response.AddHeader("Content-Length", certRaw.Length.ToString());

                Response.OutputStream.Write(certRaw, 0, certRaw.Length);

                Response.Flush();
                Response.End();

            }
        }

        protected void btnSign_Click(object sender, EventArgs e)
        {
            if (AppSigner.SignerCertificate == null)
            {
                this.AjaxAlert("請先建立系統簽章憑證!!");
            }
            else if (XmlFile.HasFile)
            {
                XmlDocument docMsg = new XmlDocument();
                //                docMsg.PreserveWhitespace = true;
                docMsg.Load(XmlFile.PostedFile.InputStream);


                CryptoUtility.SignXml(docMsg, null,
                    null, AppSigner.SignerCertificate);

                Response.Clear();
                Response.ContentType = "message/rfc822";
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", "SignedContext.xml"));
                docMsg.Save(Response.OutputStream);
                Response.Flush();
                Response.End();

            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            lbViewCert.Visible = false;

            if (XmlSigFile.HasFile)
            {

                XmlDocument docMsg = new XmlDocument();
                //                docMsg.PreserveWhitespace = true;
                docMsg.Load(XmlSigFile.PostedFile.InputStream);

                CryptoUtility ca = new CryptoUtility();
                if (ca.VerifyXmlSignature(docMsg))
                {
                    lblMsg.Text = ca.CA_Log.Subject;
                    lblMsg.ForeColor = System.Drawing.Color.Black;
                    lbViewCert.Visible = true;

                    X509Certificate SignerCertificate = ca.SignerCertificate;
                    this.ViewState["signerCert"] = new X509Certificate(SignerCertificate);
                }
                else
                {
                    lblMsg.Text = "驗簽失敗!! 請查閱log...";
                }
            }
        }
    }
}