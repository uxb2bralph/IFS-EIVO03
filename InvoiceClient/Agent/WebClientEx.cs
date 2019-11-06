using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace InvoiceClient.Agent
{
    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }
        protected WebRequest _request;

        protected override WebRequest GetWebRequest(Uri address)
        {
            _request = base.GetWebRequest(address);
            _request.Timeout = Timeout;
            return _request;
        }

        public WebResponse Response => _request != null ? GetWebResponse(_request) : null;
    }
}
