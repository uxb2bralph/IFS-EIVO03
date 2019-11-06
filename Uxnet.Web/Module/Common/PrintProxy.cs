using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml;

namespace Uxnet.Web.Module.Common
{
    public class PrintProxy : WebControl
    {
        private List<String> _url;

        public String IntialUrl { get; set; }

        public PrintProxy()
            : base()
        { }

        public PrintProxy(HtmlTextWriterTag tag) : base(tag) { }

        public PrintProxy(String tag)
            : base(
                tag) 
        { 

        }

        public void AddUrl(string url)
        {
            if (_url == null)
            {
                _url = new List<string>();
            }
            if (!_url.Contains(url))
            {
                _url.Add(url);
            }
        }

        public void RemoveUrl(string url)
        {
            if (_url != null)
            {
                _url.Remove(url);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_url != null && _url.Count > 0)
            {
                HttpResponse Response = Context.Response;
                Response.Clear();
                Response.ContentType = "application/pxml";

                XElement pXml = new XElement("printUrl",
                    new XElement("initialUrl", IntialUrl),
                    _url.Select(u => new XElement("url", u)).ToArray());

                XmlTextWriter xtw = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
                pXml.WriteTo(xtw);
                xtw.Flush();
                xtw.Close();

                Response.Flush();
                Response.End();
                     
            }
        }

    }
}
