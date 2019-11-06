using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uxnet.ToolAdapter.Common
{
    public interface IPdfUtility
    {
        void ConvertHtmlToPDF(String htmlFile, String pdfFile, double timeOutInMinute);
    }

    public interface IPdfUtility2 : IPdfUtility
    {
        void ConvertHtmlToPDF(String htmlFile, String pdfFile, double timeOutInMinute,String[] args);
    }

}
