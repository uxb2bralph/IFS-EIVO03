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
        /// 外部呼叫執行Mail通知
        /// </summary>
        /// <param name="mailState"></param>
        public static void doSendMaild(_MailQueryState mailState)
        {
            ThreadPool.QueueUserWorkItem(mailWorkItem, mailState);
        }

        /// <summary>
        /// 外部呼叫執行簡訊通知
        /// </summary>
        /// <param name="mailState"></param>
        public static void doSendSMSMessage(_MailQueryState mailState)
        {
            ThreadPool.QueueUserWorkItem(doSendSMSMessage, mailState);
        }

        /// <summary>
        /// Mail內容的建立
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void mailWorkItem(object stateInfo)
        {
            _MailQueryState state = (_MailQueryState)stateInfo;
            if (string.IsNullOrEmpty(state.allInvoiceID))
            {
                int year = state.setYear;
                int period = state.setPeriod;
                try
                {
                    using (InvoiceManager models = new InvoiceManager())
                    {
                        var items = models.GetTable<InvoiceItem>()
                            .Where(i => i.InvoiceCancellation == null)
                            .Where(i => i.InvoiceDonation == null)
                            .Where(i => i.PrintMark == "N")
                            .Join(models.GetTable<Organization>()
                                .Join(models.GetTable<OrganizationStatus>().Where(s => !s.DisableWinningNotice.HasValue || s.DisableWinningNotice == false),
                                    o => o.CompanyID, s => s.CompanyID, (o, s) => o),
                                i => i.SellerID, o => o.CompanyID, (i, o) => i)
                            .Join(models.GetTable<InvoiceWinningNumber>()
                                .Join(models.GetTable<UniformInvoiceWinningNumber>()
                                    .Where(u => u.Year == year && u.Period == period),
                                    w => w.WinningID, u => u.WinningID, (w, u) => w),
                                i => i.InvoiceID, w => w.InvoiceID, (i, w) => i);

                        foreach (var d in items)
                        {
                            //if (mgr.GetTable<OrganizationCategory>().Where(og => og.CategoryID == (int)Naming.CategoryID.COMP_VIRTUAL_CHANNEL && og.CompanyID == d.InvoiceItem.SellerID).Count() > 0)
                            //{
                            var email = d.InvoiceBuyer.EMail.GetEfficientString();
                            if (email == null)
                                continue;
                            string url = String.Format("{0}{1}?{2}", Settings.Default.WebApDomain, VirtualPathUtility.ToAbsolute("~/Published/WinningInvoiceMailPageForVIRTUAL.aspx"), (new CipherDecipherSrv(16)).cipher(d.InvoiceID.ToString()));
                            sendInvoiceNotifyMail(email, url, "中獎電子發票郵件通知");
                            //}
                            //else
                            //{
                            //    string url = String.Format("{0}{1}?{2}", Settings.Default.WebApDomain, VirtualPathUtility.ToAbsolute("~/Published/WinningInvoiceMailPage.aspx"), (new CipherDecipherSrv(16)).cipher(d.InvoiceID.ToString()));
                            //    sendInvoiceNotifyMail(d.InvoiceItem.InvoiceBuyer.EMail, url, "Google中獎電子發票郵件通知");
                            //    //sendInvoiceNotifyMail("howard@uxb2b.com", url, "Google中獎電子發票郵件通知");
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            else
            {
                string[] allID = state.allInvoiceID.Split(',');
                try
                {
                    using (InvoiceManager im = new InvoiceManager())
                    {
                        foreach (var d in allID)
                        {
                            string mailTo = im.EntityList.Where(i => i.InvoiceID == int.Parse(d)).FirstOrDefault().InvoiceBuyer.EMail;
                            string url = String.Format("{0}{1}?{2}", Settings.Default.WebApDomain, VirtualPathUtility.ToAbsolute("~/Published/InvoiceCancelMailPage.aspx"), (new CipherDecipherSrv(16)).cipher(d.Trim()));
                            sendInvoiceNotifyMail(mailTo, url, "Google作廢電子發票郵件通知");
                            //sendInvoiceNotifyMail("howard@uxb2b.com", url, "Google作廢電子發票郵件通知");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
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

        public static Boolean sendInvoiceNotifyMail(string mailto, string Url, string Title)
        {
            Boolean isSuccess = true;
            try
            {
                Url.Trim().MailWebPage(mailto, Title);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        public static Boolean sendWinningAlertMail(string mailto, string Url)
        {
            Boolean isSuccess = true;
            try
            {
                Url.Trim().MailWebPage(mailto, "網際優勢電子發票獨立第三方平台 發票中獎通知");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        public static void MailWebPage(this String url, String mailTo, String subject,params string[] attachment)
        {
            if (String.IsNullOrEmpty(mailTo))
                return;

            MailMessage message = new MailMessage();
            message.ReplyToList.Add(Settings.Default.ReplyTo);
            message.From = new MailAddress(Settings.Default.WebMaster);

            String[] MailAdr = mailTo.Trim().Split(',', ';', '、');

            foreach (var addr in MailAdr)
            {
                if (!String.IsNullOrEmpty(addr))
                {
                    message.To.Add(new MailAddress(addr));
                }
            }

            message.Subject = subject;
            message.IsBodyHtml = true;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                message.Body = wc.DownloadString(url);
            }

            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    if (File.Exists(item))
                    {
                        message.Attachments.Add(new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet));
                    }
                }
            }

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);
        }

        public static void MailWebPage(this String url, NameValueCollection items, String mailTo, String subject)
        {
            MailMessage message = new MailMessage();
            message.ReplyToList.Add(Settings.Default.ReplyTo);
            message.From = new MailAddress(Settings.Default.WebMaster);
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                message.Body = wc.Encoding.GetString(wc.UploadValues(url, items));
            }

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);
        }

        public static void MailServerPage(this String relativeUrl, String mailTo, String subject)
        {
            MailMessage message = new MailMessage();
            message.ReplyToList.Add(Settings.Default.ReplyTo);
            message.From = new MailAddress(Settings.Default.WebMaster);
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;

            message.Body = relativeUrl.GetPageContent();

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer)
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };
            smtpclient.Send(message);
        }

        public static void SendMailMessage(this String body, String mailTo, String subject)
        {
            MailMessage message = new MailMessage();
            message.ReplyToList.Add(Settings.Default.ReplyTo);
            message.From = new MailAddress(Settings.Default.WebMaster);
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);
        }

        public static void SendMailMessageWithInvoice(this String body, String malTo, String subject, params String[] attachment)
        {
            MailMessage message = new MailMessage();
            message.ReplyToList.Add(Settings.Default.ReplyTo);
            String[] MailAdr = malTo.Trim().Split(',', ';', '、');

            foreach (var addr in MailAdr)
            {
                if (!String.IsNullOrEmpty(addr))
                {
                    message.To.Add(new MailAddress(addr));
                }
            }
            message.From = new MailAddress(Settings.Default.WebMaster);
            message.Subject = subject;
            message.IsBodyHtml = true;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                message.Body = wc.DownloadString(body);
            }

            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    if (File.Exists(item))
                    {
                        //message.Attachments.Add(new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet));
                        message.Attachments.Add(new System.Net.Mail.Attachment(item, Path.GetFileName(item)));
                    }
                }
            }

            SmtpClient smtpclient = new SmtpClient(Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);
        }

        public static void SendInvoiceMail(this GoogleInvoiceUploadManager manager)
        {
            if (manager.IsValid)
            {
                manager.ItemList.Select(i => i.Invoice.InvoiceID).NotifyIssuedInvoice(false);


                SendMailMessage("Google電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, "Google電子發票開立郵件通知");

                //ThreadPool.QueueUserWorkItem(stateInfo =>
                //{
                //    GoogleInvoiceUploadManager mgr = (GoogleInvoiceUploadManager)stateInfo;
                //    var cipher = new CipherDecipherSrv(16);
                //    String url = String.Format("{0}{1}", Settings.Default.WebApDomain, VirtualPathUtility.ToAbsolute(Settings.Default.InvoiceMailUrl));
                //    foreach (var item in mgr.ItemList)
                //    {
                //        try
                //        {
                //            String.Format("{0}?{1}", url, cipher.cipher(item.Invoice.InvoiceID.ToString()))
                //                .MailWebPage(String.Format("{0} <{1}>",
                //                        item.Invoice.InvoiceBuyer.ContactName, item.Invoice.InvoiceBuyer.EMail),
                //                    "Google電子發票郵件通知");

                //        }
                //        catch (Exception ex)
                //        {
                //            Logger.Warn(String.Format("Google電子發票郵件通知客戶傳送失敗,發票號碼:{0}{1}", item.Invoice.TrackCode, item.Invoice.No));
                //            Logger.Error(ex);
                //        }
                //    }
                //}, manager);
            }
        }

        public static void SendInvoiceMail(this CsvInvoiceUploadManager manager)
        {
            if (manager.IsValid)
            {
                String subject = String.Format("{0}電子發票開立郵件通知", manager.Seller.CompanyName);
                manager.ItemList.Where(i => i.Entity != null).Select(i => i.Entity.InvoiceID)
                    .NotifyIssuedInvoice(false);
                SendMailMessage(String.Format("{0}電子發票已匯入,請執行發票列印作業!!", manager.Seller.CompanyName), Settings.Default.WebMaster, subject);
            }
        }

        public static void SendGoogleInvoiceCancellationMail(this IEnumerable<int> invoiceID)
        {
            if (invoiceID != null && invoiceID.Count() > 0)
            {
                ThreadPool.QueueUserWorkItem(stateInfo =>
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        IEnumerable<int> items = (IEnumerable<int>)stateInfo;
                        var cipher = new CipherDecipherSrv(16);
                        String url = String.Format("{0}{1}", Settings.Default.WebApDomain, VirtualPathUtility.ToAbsolute(Settings.Default.InvoiceCancellationMailUrl));
                        foreach (var id in items)
                        {
                            try
                            {
                                var item = mgr.EntityList.Where(i => i.InvoiceID == id).First();
                                if (item.InvoiceCancellation != null)
                                {
                                    String.Format("{0}?{1}", url, cipher.cipher(id.ToString()))
                                        .MailWebPage(item.InvoiceBuyer.EMail, "Google作廢電子發票開立郵件通知");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn(String.Format("Google作廢電子發票郵件通知客戶傳送失敗,ID:{0}", id));
                                Logger.Error(ex);
                            }
                        }
                    }
                }, invoiceID);
            }
        }

        public static string StringMask(string OrgString, int StartReplaceNO, int ReplaceLength, char ReplaceSymbol)
        {
            return OrgString.StringMask(StartReplaceNO, ReplaceLength, ReplaceSymbol);
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
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Uxnet.Web.Properties.Settings.Default.WebMaster);

            mailTo = mailTo.Replace(';', ',').Replace('、', ',').GetEfficientString();
            if (mailTo == null)
                return;
            if (mailTo.IndexOf('@') < 0)
            {
                mailTo = $"{mailTo}@uxb2b.com"; 
            }
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    if (File.Exists(item))
                    {
                        message.Attachments.Add(new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet));
                    }
                }
            }

            using (SmtpClient smtpclient = new SmtpClient(Uxnet.Web.Properties.Settings.Default.MailServer)
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                })
            {
                smtpclient.Send(message);
            }

        }


        public static void SendMailMessage(this String body, String mailTo, String subject, System.Net.Mail.Attachment[] attachment)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Uxnet.Web.Properties.Settings.Default.WebMaster);
            mailTo = mailTo.Replace(';', ',').Replace('、', ',');
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    message.Attachments.Add(item);
                }
            }

            SmtpClient smtpclient = new SmtpClient(Uxnet.Web.Properties.Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);

        }

        public static void SendMailMessage(this String body, String mailTo, String subject, System.Net.Mail.Attachment[] attachment, System.Net.Mail.Attachment[] Ad)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Uxnet.Web.Properties.Settings.Default.WebMaster);
            message.To.Add(mailTo);
            message.Subject = subject;
            message.IsBodyHtml = true;
            //message.Body = string.Format("<img src=\"cid:attach.gif\" />{0}", body);
            message.BodyEncoding = Encoding.GetEncoding("utf-8");
            if (attachment != null && attachment.Length > 0)
            {
                foreach (var item in attachment)
                {
                    message.Attachments.Add(item);
                }
            }
            if (Ad != null && Ad.Length > 0)
            {
                foreach (var item in Ad)
                {
                    if (item != null)
                    {
                        // message.Body += string.Format("<img src=\" {0}\"/>", item.Name);
                        item.ContentDisposition.Inline = true;
                        item.ContentDisposition.DispositionType =
                           System.Net.Mime.DispositionTypeNames.Inline;
                        string cid = item.ContentId;
                        message.Attachments.Add(item);
                        message.Body += String.Format("<img src=\" cid:{0}\"/ alt='三個月免費試用' name='三個月免費試用'><br>", cid);
                        //message.Body += String.Format("<img src=\" {0}\"/ alt='三個月免費試用' name='三個月免費試用'><br>", item.ContentStream);  

                    }
                }
            }
            message.Body += body;
            SmtpClient smtpclient = new SmtpClient(Uxnet.Web.Properties.Settings.Default.MailServer);
            smtpclient.Credentials = CredentialCache.DefaultNetworkCredentials;
            smtpclient.Send(message);

        }

    }
}