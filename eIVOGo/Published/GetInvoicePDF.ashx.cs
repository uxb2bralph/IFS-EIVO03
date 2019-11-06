using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using eIVOGo.Helper;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using System.Xml;
using Model.Schema.TXN;
using System.IO;

namespace eIVOGo.Published
{
    /// <summary>
    ///GetInvoicePDF 的摘要描述
    /// </summary>
    public class GetInvoicePDF : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/pdf";
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;
            HttpServerUtility Server = context.Server;

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                XmlDocument sellerInfo = new XmlDocument();
                sellerInfo.Load(Request.InputStream);
                if (crypto.VerifyXmlSignature(sellerInfo))
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            Root root = sellerInfo.ConvertTo<Root>();

                            int invoiceID;
                            if (Request.Params["QUERY_STRING"] != null && int.TryParse(Request.Params["QUERY_STRING"], out invoiceID))
                            {
                                var item = mgr.GetTable<InvoiceItem>().Where(i => i.InvoiceID == invoiceID && i.CDS_Document.DocumentOwner.OwnerID == token.CompanyID).FirstOrDefault();
                                if (item != null)
                                {
                                    Response.WriteFileAsDownload(item.CreatePdfFile(false), String.Format("{0:yyyy-MM-dd}.pdf", DateTime.Today), false, "application/pdf");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


        
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}