using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uxnet.Com.WS_DocumentService;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace DocmentServer
{
    public partial class MergePdfDocument : IDisposable
    {
        public string PdfFileName { get; protected set; }
        protected bool _bDisposed = false; 
        protected string _srcUrl;
        public string MergePDF(int? PurchaseID, int fileCount, string[] pdfList, string filePath)
        {
            var PdfFileName = String.Format("{0:yyyyMMddHHmmssf}.pdf", DateTime.Now); 
            var returnFile = mergePDFFilesByA4_horizontal(pdfList, filePath, PdfFileName);
            return returnFile;
        }

        private static string mergePDFFilesByA4_horizontal(string[] fileList, string filePath, string NewFile)
        {
            NewFile = filePath + NewFile;
            int f = fileList.Length-1;
            PdfReader reader= new PdfReader(filePath + "/" + fileList[f]);
            Document document = new Document();
            document = new Document(reader.GetPageSizeWithRotation(1));
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(NewFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            int rotation = 0;
            //
            for (int i = 0; i < fileList.Length; i++)
            {
                reader = new PdfReader(filePath + "/" + fileList[i]);
                int iPageNum = reader.NumberOfPages;

                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    rotation = reader.GetPageRotation(j);
                    if (rotation == 90 || rotation == 270)
                    {
                        cb.AddTemplate(newPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb.AddTemplate(newPage, 1f, 0, 0, 1f, 0, 0);
                    }
                }
            }
            document.Close();

            return NewFile;
        }
        private static string mergePDFFilesByA4_Vertical(string[] fileList, string filePath, string NewFile)
        {
            NewFile = filePath + NewFile;
            PdfReader reader;
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(NewFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            //
            for (int i = 0; i < fileList.Length; i++)
            {
                reader = new PdfReader(filePath + "/" + fileList[i]);
                int iPageNum = reader.NumberOfPages;

                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();

            return NewFile;
        }

        public void Dispose()
        {
            dispose(true);
        }

        private void dispose(bool disposing)
        {
            if (!_bDisposed)
            {
                if (disposing)
                {
                    if (!String.IsNullOrEmpty(PdfFileName) && File.Exists(PdfFileName))
                    {
                        File.Delete(PdfFileName);
                    }
                }

                _bDisposed = true;
            }
        }

        ~MergePdfDocument()
        {
            dispose(false);
        }

    }
}
