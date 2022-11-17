using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;
using eIVOGo.Properties;
using MessagingToolkit.QRCode.Codec;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace eIVOGo.Helper
{
    public static class NotificationExtensionMethods
    {
        public static void NotifyToReceiveA0401(this CDS_Document item)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                EIVOPlatformFactory.NotifyToReceiveA0401(item.DocID);
            });

        }

        public static void NotifyIssuedA0401(this IEnumerable<int> docID,String mailTo = null)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                foreach (var id in docID)
                {
                    EIVOPlatformFactory.NotifyIssuedA0401(new NotifyToProcessID
                    {
                        DocID = id,
                        MailTo = mailTo,
                    });
                }
            });
        }

        public static void NotifyIssuedInvoice(this IEnumerable<int> docID,bool appendAttachment,String mailTo = null)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                foreach (var invoiceID in docID)
                {
                    EIVOPlatformFactory.NotifyIssuedInvoice(new NotifyToProcessID
                    {
                        DocID = invoiceID,
                        AppendAttachment = appendAttachment,
                        MailTo = mailTo,
                    });
                }
            });
        }

        public static void NotifyWinningInvoice(this IEnumerable<int> docID, bool appendAttachment, String mailTo = null)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                foreach (var invoiceID in docID)
                {
                    EIVOPlatformFactory.NotifyWinningInvoice(new NotifyToProcessID
                    {
                        DocID = invoiceID,
                        AppendAttachment = appendAttachment,
                        MailTo = mailTo,
                    });
                }
            });
        }

        public static void NotifyIssuedInvoiceCancellation(this IEnumerable<int> docID,String mailTo = null)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                foreach (var id in docID)
                {
                    EIVOPlatformFactory.NotifyIssuedInvoiceCancellation(new NotifyToProcessID
                    {
                        DocID = id,
                        MailTo = mailTo,
                    });
                }
            });

        }

        public static void NotifyIssuedAllowance(this IEnumerable<int> docID)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                foreach (var id in docID)
                {
                    EIVOPlatformFactory.NotifyIssuedAllowance(id);
                }
            });
        }

        public static void NotifyIssuedAllowanceCancellation(this IEnumerable<int> docID)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                foreach (var id in docID)
                {
                    EIVOPlatformFactory.NotifyIssuedAllowanceCancellation(id);
                }
            });
        }

        public static void SendSmtpMessage(this String body, String mailTo, String subject, String mailFrom = null, System.Net.Mail.Attachment[] attachment = null, String replyTo = null,CustomSmtpHost smtpSettings = null)
        {
            mailTo = mailTo?.Replace(';', ',').Replace('、', ',').GetEfficientString();
            if (mailTo == null)
                return;

            if (mailTo.IndexOf('@') < 0)
            {
                mailTo = $"{mailTo}@uxb2b.com";
            }

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(mailFrom ?? Uxnet.Web.Properties.Settings.Default.WebMaster);
                message.To.Add(mailTo);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                if (replyTo != null)
                {
                    message.ReplyToList.Add(replyTo);
                }

                if (attachment != null && attachment.Length > 0)
                {
                    foreach (var item in attachment)
                    {
                        message.Attachments.Add(item);
                    }
                }

                if (smtpSettings == null)
                {
                    using (SmtpClient smtpclient = new SmtpClient(Uxnet.Web.Properties.Settings.Default.MailServer)
                    {
                        Credentials = CredentialCache.DefaultNetworkCredentials
                    })
                    {
                        smtpclient.Send(message);
                    }
                }
                else
                {
                    using (SmtpClient smtpclient = new SmtpClient(smtpSettings.Host, smtpSettings.Port ?? 25))
                    {
                        smtpclient.EnableSsl = smtpSettings.EnableSsl == true;
                        //smtpclient.UseDefaultCredentials = false;
                        if (smtpSettings.UserName != null)
                        {
                            smtpclient.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                        }
                        if (!String.IsNullOrEmpty(smtpSettings.MailFrom))
                        {
                            message.From = new MailAddress(smtpSettings.MailFrom);
                        }
                        smtpclient.Send(message);
                    }
                }
            }
        }
    }
}