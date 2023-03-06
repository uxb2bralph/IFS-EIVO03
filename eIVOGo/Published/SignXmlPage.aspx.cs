using Model.Helper;
using Model.Locale;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Uxnet.Com.Security.UseCrypto;

namespace eIVOGo.Published
{
    public partial class SignXmlPage : Page
    {
        private XmlDocument _docResult;
        protected IEnumerable<X509Certificate2> _certs;
        protected HtmlForm form1;
        protected TextBox Subject;
        protected DropDownList CertStoreName;
        protected DropDownList CertStoreLocation;
        protected Button btnListCert;
        protected TextBox StorePass;
        protected TextBox CspName;
        protected FileUpload XmlFile;
        protected Button btnUpload;
        protected FileUpload XmlSigFile;
        protected Button btnVerify;
        protected Label lblMsg;
        protected LinkButton lbViewCert;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            this.initializeData();
        }

        private void initializeData()
        {
            this.CertStoreName.DataSource = (object)Enum.GetNames(typeof(StoreName));
            this.CertStoreName.DataBind();
            this.CertStoreLocation.DataSource = (object)Enum.GetNames(typeof(StoreLocation));
            this.CertStoreLocation.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(this.SignXmlPage_PreRender);
        }

        private void SignXmlPage_PreRender(object sender, EventArgs e)
        {
            if (this._docResult == null)
                return;
            this.Response.Clear();
            this.Response.ContentType = "message/rfc822";
            this.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", (object)"SignedContext.xml"));
            this._docResult.Save(this.Response.OutputStream);
            this.Response.Flush();
            this.Response.End();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!this.XmlFile.HasFile)
                return;
            XmlDocument docMsg = new XmlDocument();
            docMsg.Load(this.XmlFile.PostedFile.InputStream);
            X509Store x509Store = new X509Store((StoreName)Enum.Parse(typeof(StoreName), this.CertStoreName.SelectedValue), (StoreLocation)Enum.Parse(typeof(StoreLocation), this.CertStoreLocation.SelectedValue));
            x509Store.Open(OpenFlags.ReadOnly);
            X509Certificate2 x509Certificate2 = (X509Certificate2)null;
            foreach (X509Certificate2 certificate in x509Store.Certificates)
            {
                if (certificate.Subject == Request["CertName"] || (this.Subject.Text.Length>0 && certificate.Subject.IndexOf(this.Subject.Text) >= 0))
                {
                    //X509Certificate2 signCert = new X509Certificate2(certificate.Export(X509ContentType.Pfx, "111111"), "111111", X509KeyStorageFlags.Exportable);
                    CryptoUtility.SignXmlSHA256(docMsg, this.CspName.Text, this.Request[this.StorePass.UniqueID], certificate, "");
                    this._docResult = docMsg;
                    x509Certificate2 = certificate;
                    break;
                }
            }
            if (x509Certificate2 == null)
                this.lblMsg.Text = "找不到簽署者的憑證!!";
            x509Store.Close();
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            this.lbViewCert.Visible = false;
            if (!this.XmlSigFile.HasFile)
                return;
            XmlDocument doc = new XmlDocument();
            doc.Load(this.XmlSigFile.PostedFile.InputStream);
            CryptoUtility cryptoUtility = new CryptoUtility();
            if (cryptoUtility.VerifyXmlSignature(doc))
            {
                this.lblMsg.Text = cryptoUtility.CA_Log.Subject;
                this.lblMsg.ForeColor = Color.Black;
                this.lbViewCert.Visible = true;
                this.ViewState["cert"] = (object)new X509Certificate((X509Certificate)cryptoUtility.SignerCertificate);
            }
            else
                this.lblMsg.Text = "驗簽失敗!!請檢查log!!";
        }

        protected void lbViewCert_Click(object sender, EventArgs e)
        {
            if (this.ViewState["cert"] == null)
                return;
            byte[] rawCertData = ((X509Certificate)this.ViewState["cert"]).GetRawCertData();
            this.Response.Clear();
            this.Response.ContentType = "message/rfc822";
            this.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", (object)"Signer.cer"));
            this.Response.AddHeader("Content-Length", rawCertData.Length.ToString());
            this.Response.OutputStream.Write(rawCertData, 0, rawCertData.Length);
            this.Response.Flush();
            this.Response.End();
        }

        protected void btnListCert_Click(object sender, EventArgs e)
        {
            X509Store x509Store = new X509Store((StoreName)Enum.Parse(typeof(StoreName), this.CertStoreName.SelectedValue), (StoreLocation)Enum.Parse(typeof(StoreLocation), this.CertStoreLocation.SelectedValue));
            x509Store.Open(OpenFlags.ReadOnly);
            this._certs = x509Store.Certificates.Cast<X509Certificate2>();
            x509Store.Close();
        }

        protected void btnVerifyCms_Click(object sender, EventArgs e)
        {
            this.lbViewCert.Visible = false;
            if (String.IsNullOrEmpty(dataToSign.Value) || String.IsNullOrEmpty(dataSignature.Value))
            {
                return;
            }

            CryptoUtility crypto = new CryptoUtility();
            if (crypto.VerifyPKCS7(dataToSign.Value, dataSignature.Value))
            {
                this.lblMsg.Text = crypto.CA_Log.Subject;
                this.lblMsg.ForeColor = Color.Black;
                this.lbViewCert.Visible = true;
                this.ViewState["cert"] = (object)new X509Certificate((X509Certificate)crypto.SignerCertificate);
            }
            else
                this.lblMsg.Text = "驗簽失敗!!請檢查log!!";
        }
    }
}
