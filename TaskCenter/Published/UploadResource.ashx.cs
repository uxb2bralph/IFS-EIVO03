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
using System.Threading;
using System.IO.Compression;
using Model.Properties;

namespace TaskCenter.Published
{
    /// <summary>
    ///UploadAttachment 的摘要描述
    /// </summary>
    public class UploadResource : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var Request = context.Request;
            var Response = context.Response;

            try
            {
                if(Request.Headers["StorageToken"]!=null)
                {
                    String fileName = Request.Headers["StorageToken"].DecryptData().WebStoreTargetPath();
                    String storePath = Path.GetDirectoryName(fileName).CheckStoredPath();
                    using(FileStream stream = File.Create(fileName))
                    {
                        Request.InputStream.CopyTo(stream);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Response.Write(ex);
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