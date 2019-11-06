using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Schema.TXN;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Model.Helper;
using Model.Locale;
using Ionic.Zip;
using System.Threading;
using System.IO.Compression;

namespace eIVOGo.Published
{
    /// <summary>
    ///UploadAttachment 的摘要描述
    /// </summary>
    public class UploadAttachmentForGoogleByStream : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";

            var Request = context.Request;
            var Response = context.Response;

            Root result = new Root
            {
                UXB2B = "電子發票系統",
                Result = new RootResult
                {
                    timeStamp = DateTime.Now,
                    value = 0
                }
            };

            List<AutomationItem> automation = new List<AutomationItem>();

            try
            {
                String fileName = Request.Headers["X-FileName"] ?? $"{DateTime.Now.Ticks}-{Guid.NewGuid()}.zip";

                using (ZipArchive zip = new ZipArchive(Request.InputStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (!String.IsNullOrEmpty(entry.Name))
                        {
                            AutomationItem auto = new AutomationItem
                            {
                                Attachment = new AutomationItemAttachment
                                {
                                    FileName = entry.Name
                                }
                            };

                            try
                            {
                                entry.ExtractToFile(Path.Combine(GoogleInvoiceManager.AttachmentPoolPath, entry.Name), true);

                                auto.Description = "";
                                auto.Status = 1;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                                auto.Description = ex.Message;
                                auto.Status = 0;

                                ExceptionNotification.SendExceptionNotificationToSysAdmin(new Exception("PDF附件上傳：", ex));
                            }

                            automation.Add(auto);
                        }
                    }
                }

                GoogleInvoiceExtensionMethods.MatchGoogleInvoiceAttachment();

                result.Result.value = 1;
                result.Automation = automation.ToArray();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;

                ExceptionNotification.SendExceptionNotificationToSysAdmin(new Exception("PDF附件上傳：", ex));
            }

            result.ConvertToXml().Save(Response.OutputStream);

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