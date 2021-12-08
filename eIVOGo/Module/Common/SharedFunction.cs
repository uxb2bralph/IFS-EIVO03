using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;

namespace eIVOGo.Module.Common
{
    public static class SharedFunction
    {
        #region "Using Thread to Send Notify Mail"
        /// <summary>
        /// 定義類別參數
        /// </summary>
        public class _MailQueryState
        {
            public int setYear { get; set; }
            public int setPeriod { get; set; }
            public string allInvoiceID { get; set; }
        }

        /// <summary>
        /// 外部呼叫執行簡訊通知
        /// </summary>
        /// <param name="mailState"></param>
        public static void doSendSMSMessage(_MailQueryState mailState)
        {
            ThreadPool.QueueUserWorkItem(doSendSMSMessage, mailState);
        }

        private static void doSendSMSMessage(object stateInfo)
        {
            _MailQueryState state = (_MailQueryState)stateInfo;
            int year = state.setYear;
            int period = state.setPeriod;
            int smonth = (period * 2) - 1;
            int emonth = period * 2;
            try
            {
                ModelExtension.MessageManagement.InvoiceWinningNotificationManager smsMgr = new ModelExtension.MessageManagement.InvoiceWinningNotificationManager();
                smsMgr.Year = year;
                smsMgr.MonthFrom = smonth;
                smsMgr.MonthTo = emonth;
                smsMgr.ExceptionHandler = AlertSMSError;

                smsMgr.ProcessMessage();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion

        public static void MailWebPage(this String url, String mailTo, String subject,params string[] attachment)
        {

            System.Net.Mail.Attachment[] items = null;

            if (attachment != null && attachment.Length > 0)
            {
                items = attachment.Where(f => File.Exists(f))
                    .Select(f => new System.Net.Mail.Attachment(f, MediaTypeNames.Application.Octet))
                    .ToArray();
            }

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.DownloadString(url)
                    .SendSmtpMessage(mailTo, subject, Settings.Default.WebMaster, items, Settings.Default.ReplyTo);
                
            }
        }

        public static void MailWebPage(this String url, NameValueCollection items, String mailTo, String subject)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Encoding.GetString(wc.UploadValues(url, items))
                    .SendSmtpMessage(mailTo, subject, Settings.Default.WebMaster, null, Settings.Default.ReplyTo);
            }
        }

        public static void MailServerPage(this String relativeUrl, String mailTo, String subject)
        {
            relativeUrl.GetPageContent().SendSmtpMessage(mailTo, subject, Settings.Default.WebMaster, null, Settings.Default.ReplyTo);
        }

        public static void SendMailMessage(this String body, String mailTo, String subject)
        {
            body.SendSmtpMessage(mailTo, subject, Settings.Default.WebMaster, null, Settings.Default.ReplyTo);
        }

        public static void AlertSMSError(int id, String reason, String content)
        {
            NameValueCollection items = new NameValueCollection();
            items["id"] = id.ToString();
            items["reason"] = reason;
            items["sms"] = content;
            String url = String.Format("{0}{1}", Settings.Default.WebApDomain,
                VirtualPathUtility.ToAbsolute("~/Published/SMSExceptionNotification.aspx"));

            ThreadPool.QueueUserWorkItem(stateInfo =>
            {
                try
                {
                    url.MailWebPage(items, Settings.Default.WebMaster, "簡訊傳送失敗通知");
                }
                catch (Exception ex)
                {
                    Logger.Warn(String.Format("簡訊傳送失敗通知:{0}", url));
                    Logger.Error(ex);
                }
            });
        }

        /// <summary>
        /// GridView 控制項匯出。
        /// </summary>
        /// <param name="Encoding">編碼方式。</param>
        /// <param name="FileName">檔案名稱。</param>
        /// <param name="ContentType">內容類型標頭。</param> 
        public static void Export(GridView gv, Encoding Encoding, string FileName, string ContentType)
        {
            HttpResponse oResponse = null;
            System.IO.StringWriter oStringWriter = null;
            System.Web.UI.HtmlTextWriter oHtmlWriter = null;
            bool bAllowPaging = false;
            string sText = null;
            string sFileName = null;

            //檔案名稱需經 UrlEncode 編碼，解決中文檔名的問題
            sFileName = HttpUtility.UrlEncode(FileName, Encoding);

            oResponse = HttpContext.Current.Response;
            oResponse.Clear();
            sText = "<meta http-equiv='Content-Type'; content='text/html';charset='{0}'>";
            sText = string.Format(sText, Encoding.WebName);
            oResponse.Write(sText);
            oResponse.AddHeader("content-disposition", "attachment;filename=" + sFileName);
            oResponse.ContentEncoding = Encoding;
            oResponse.Charset = Encoding.WebName;
            oResponse.ContentType = "application/ms-excel";

            // If you want the option to open the Excel file without saving than
            // comment out the line below
            // oResponse.Cache.SetCacheability(HttpCacheability.NoCache)

            oStringWriter = new System.IO.StringWriter();
            oHtmlWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            bAllowPaging = gv.AllowPaging;
            if (bAllowPaging)
            {
                gv.AllowPaging = false;
                //if (gv.RequiresDataBinding)
                //{
                gv.DataBind();
                //}
            }

            gv.RenderControl(oHtmlWriter);

            if (bAllowPaging)
            {
                gv.AllowPaging = bAllowPaging;
            }
            oResponse.Write(oStringWriter.ToString());
            oResponse.End();
        }

        public static void SendMailMessage(this String body, String mailTo, String subject, params String[] attachment)
        {
            System.Net.Mail.Attachment[] items = null;

            if (attachment != null && attachment.Length > 0)
            {
                items = attachment.Where(f => File.Exists(f))
                    .Select(f => new System.Net.Mail.Attachment(f, MediaTypeNames.Application.Octet))
                    .ToArray();
            }

            body.SendSmtpMessage(mailTo, subject, Settings.Default.ServiceMailBox, items);

        }


        public static void SendMailMessage(this String body, String mailTo, String subject, System.Net.Mail.Attachment[] attachment)
        {
            body.SendSmtpMessage(mailTo, subject, Settings.Default.ServiceMailBox, attachment);
        }

        //public static void SendMailMessage(this String body, String mailTo, String subject, System.Net.Mail.Attachment[] attachment, System.Net.Mail.Attachment[] Ad)
        //{
        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress(Uxnet.Web.Properties.Settings.Default.WebMaster);
        //    message.To.Add(mailTo);
        //    message.Subject = subject;
        //    message.IsBodyHtml = true;
        //    //message.Body = string.Format("<img src=\"cid:attach.gif\" />{0}", body);
        //    message.BodyEncoding = Encoding.GetEncoding("utf-8");
        //    if (attachment != null && attachment.Length > 0)
        //    {
        //        foreach (var item in attachment)
        //        {
        //            message.Attachments.Add(item);
        //        }
        //    }
        //    if (Ad != null && Ad.Length > 0)
        //    {
        //        foreach (var item in Ad)
        //        {
        //            if (item != null)
        //            {
        //                // message.Body += string.Format("<img src=\" {0}\"/>", item.Name);
        //                item.ContentDisposition.Inline = true;
        //                item.ContentDisposition.DispositionType =
        //                   System.Net.Mime.DispositionTypeNames.Inline;
        //                string cid = item.ContentId;
        //                message.Attachments.Add(item);
        //                message.Body += String.Format("<img src=\" cid:{0}\"/ alt='三個月免費試用' name='三個月免費試用'><br>", cid);
        //                //message.Body += String.Format("<img src=\" {0}\"/ alt='三個月免費試用' name='三個月免費試用'><br>", item.ContentStream);  

        //            }
        //        }
        //    }
        //    message.Body += body;
        //    SmtpClient smtpclient = new SmtpClient(Uxnet.Web.Properties.Settings.Default.MailServer);
        //    smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
        //    smtpclient.Send(message);

        //}

    }
}