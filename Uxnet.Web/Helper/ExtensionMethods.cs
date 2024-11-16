using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using Utility;
using Uxnet.Web.Properties;

namespace Uxnet.Web.Helper
{
    public static partial class ExtensionMethods
    {
        public static void MailServerPage(this String relativeUrl, String subject, System.Net.Mail.Attachment[] attachment, params String[] mailTo)
        {
            MailMessage message = new MailMessage();
            message.Headers["Content-Location"] = Settings.Default.HostUrl;
            message.From = new MailAddress(Settings.Default.WebMaster);
            foreach (var m in mailTo)
            {
                if (!String.IsNullOrEmpty(m))
                    message.To.Add(m.Replace(';', ',').Replace('、', ','));
            }
            message.Subject = subject;
            message.IsBodyHtml = true;
            String contentLocation = String.Format("{0}{1}", Settings.Default.HostUrl, HttpRuntime.AppDomainAppVirtualPath);
            message.Headers.Add("Content-Location", contentLocation);
            message.Headers.Add("Content-Base", contentLocation);

            message.ImportHtmlBody(relativeUrl.GetPageContent());

            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    message.Attachments.Add(item);
                }
            }

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);

        }

        public static void ImportHtmlBody(this MailMessage message, String htmlBody)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlBody);

            var items = doc.DocumentNode.SelectNodes("//img");
            if (items != null)
            {
                foreach (var imgNode in items)
                {
                    if (imgNode.Attributes["src"] == null || String.IsNullOrEmpty(imgNode.Attributes["src"].Value))
                        continue;
                    Attachment item = new Attachment(HttpContext.Current.Server.MapPath(imgNode.Attributes["src"].Value));
                    item.NameEncoding = Encoding.UTF8;
                    item.ContentId = Guid.NewGuid().ToString();
                    item.ContentDisposition.Inline = true;
                    imgNode.SetAttributeValue("src", "cid:" + item.ContentId);
                    message.Attachments.Add(item);
                }
            }

            message.BodyEncoding = Encoding.UTF8;
            message.Body = doc.DocumentNode.OuterHtml;
            
        }

        public static string ToQueryString(this object obj)
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{HttpUtility.UrlEncode(p.Name)}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");

            return string.Join("&", properties);
        }
    }
}