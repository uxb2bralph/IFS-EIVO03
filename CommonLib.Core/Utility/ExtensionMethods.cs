﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonLib.Core.Helper;
using CommonLib.PlugInAdapter;
using Microsoft.AspNetCore.Http;

namespace CommonLib.Core.Utility
{
    public static class ExtensionMethods
    {
        public static void ConvertHtmlToPDF(this String htmlFile, String pdfFile, double timeOutInMinute, String[] args = null)
        {
            lock (typeof(ExtensionMethods))
            {
                var util = PlugInHelper.GetPdfUtility();
                if (util is IPdfUtility2)
                {
                    ((IPdfUtility2)util).ConvertHtmlToPDF(htmlFile, pdfFile, timeOutInMinute, args);
                }
                else
                {
                    util.ConvertHtmlToPDF(htmlFile, pdfFile, timeOutInMinute);
                }
            }
        }

        public static void SaveAs(this IFormFile formFile,String path)
        {
            using FileStream stream = new(path, FileMode.Create);
            formFile.CopyTo(stream);
        }

        public static async Task SaveAsAsync(this HttpRequest Request, String path,bool includeHeader = true)
        {
            //Request.EnableBuffering();
            //Request.Body.Position = 0;
            //using var streamReader = new StreamReader(Request.Body);
            //string bodyContent = await streamReader.ReadToEndAsync();

            Request.Body.Position = 0;
            using (FileStream fs = System.IO.File.Create(path))
            {
                if (includeHeader)
                {
                    using (StreamWriter writer = new StreamWriter(fs, leaveOpen: true))
                    {
                        foreach (var h in Request.Headers)
                        {
                            writer.WriteLine($"{h.Key}: {h.Value}");
                        }
                        writer.WriteLine();
                    }
                }
                await Request.Body.CopyToAsync(fs);
            }
            Request.Body.Position = 0;
        }

        public static async Task<byte[]> GetRequestBytesAsync(this HttpRequest Request)
        {
            Request.Body.Position = 0;
            using (MemoryStream fs = new MemoryStream())
            {
                await Request.Body.CopyToAsync(fs);
                Request.Body.Position = 0;
                return fs.ToArray();
            }
        }

        public static async Task<MemoryStream> GetRequestStreamAsync(this HttpRequest Request)
        {
            Request.Body.Position = 0;
            MemoryStream fs = new MemoryStream();
            await Request.Body.CopyToAsync(fs);
            fs.Position = 0;
            Request.Body.Position = 0;
            return fs;

        }

        public static async Task<String> GetRequestBodyAsync(this HttpRequest Request)
        {
            Request.Body.Position = 0;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                var data = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
                return data;
            }
        }

    }
}
