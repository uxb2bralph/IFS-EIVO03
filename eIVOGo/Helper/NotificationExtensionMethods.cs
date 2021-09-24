using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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


    }
}