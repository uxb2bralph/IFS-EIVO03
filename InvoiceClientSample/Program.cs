using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Uxnet.Com.Security.UseCrypto;

namespace InvoiceClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //發票號碼營業已配號
            Sample_1();

            //發票號碼加值中心配號
            Sample_2();

            Console.ReadKey();

        }

        private static void Sample_1()
        {
            //  1.準備發票文件
            XmlDocument docInv = new XmlDocument();
            docInv.Load("Sample.xml");

            //  2.準備簽章憑證
            X509Certificate2 signerCert = checkSignerCertificate();

            //  3.發票資料數位簽章
            CryptoUtility.SignXml(docInv, "", "", signerCert);

            //  4.傳送發票(營業人配號)
            using (WS_Invoice.eInvoiceService invSvc = new WS_Invoice.eInvoiceService())
            {
                XmlNode result = invSvc.UploadInvoiceV2(docInv);

                // 傳送結果
                Console.WriteLine("EIVO Server回應:");
                Console.WriteLine(result.OuterXml);
            }
        }

        private static void Sample_2()
        {
            //  1.準備發票文件
            XmlDocument docInv = new XmlDocument();
            docInv.Load("加值中心配號範例.xml");

            //  2.準備簽章憑證
            X509Certificate2 signerCert = checkSignerCertificate();

            //  3.發票資料數位簽章
            CryptoUtility.SignXml(docInv, "", "", signerCert);

            //  4.傳送發票(加值中心配號)
            using (WS_Invoice.eInvoiceService invSvc = new WS_Invoice.eInvoiceService())
            {
                XmlNode result = invSvc.UploadInvoiceAutoTrackNoV2(docInv);

                // 傳送結果
                Console.WriteLine("EIVO Server回應:");
                Console.WriteLine(result.OuterXml);
            }
        }

        private static X509Certificate2 checkSignerCertificate()
        {
            String certFile = "UXSigner.pfx";
            String activationKey = "8f8fa872-9479-4a3d-b135-f07c1bdab4fc";//"b35a50e0-40ab-4774-acd8-668685d858a8";
            X509Certificate2 signerCert = null;
            if (!File.Exists(certFile))
            {
                using (WS_Invoice.eInvoiceService invSvc = new WS_Invoice.eInvoiceService())
                {
                    String certContent = invSvc.GetSignerCertificateContent(activationKey);
                    if (!String.IsNullOrEmpty(certContent))
                    {
                        using (FileStream fs = new FileStream(certFile, FileMode.CreateNew, FileAccess.Write))
                        {
                            byte[] buf = Convert.FromBase64String(certContent);
                            fs.Write(buf, 0, buf.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }
                }
            }

            if (File.Exists(certFile))
            {
                Guid keyID;
                if (Guid.TryParse(activationKey, out keyID))
                {
                    signerCert = new X509Certificate2(certFile, keyID.ToString().Substring(0, 8));
                }
            }

            return signerCert;
        }
    }
}
