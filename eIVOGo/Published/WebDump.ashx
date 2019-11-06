<%@ WebHandler Language="C#" Class="WebDump" %>

using System;
using System.Web;
using System.IO;
using Utility;
using System.Xml;

public class WebDump : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;

        Response.ContentType = "text/xml";
        Request.SaveAs(Path.Combine(Logger.LogDailyPath, String.Format("{0:yyyyMMdd-HHmmssfff}.txt", DateTime.Now)), true);

        Response.Write("<root>OK!</root>");

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


}