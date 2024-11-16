<%@ WebHandler Language="C#" Class="eIVOGo.Helper.GetSample" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using Model.DataEntity;
using ModelExtension.DataExchange;
using ModelExtension.Helper;
using Utility;
using DataAccessLayer;
using ClosedXML.Excel;

namespace eIVOGo.Helper
{
    /// <summary>
    /// Summary description for LoadZipCode
    /// </summary>
    public class GetSample : IHttpHandler
    {
        HttpResponse Response;
        HttpRequest Request;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            Response = context.Response;
            Request = context.Request;

            switch (Request["data"])
            {
                case "InvoiceBuyer":
                    createSample(() =>
                    {
                        var exchange = new InvoiceBuyerExchange();
                        using (XLWorkbook xls = exchange.GetSample())
                        {
                            xls.SaveAs(Response.OutputStream);
                        }
                    }, "修改買受人資料.xlsx");
                    break;

                case "TrackCode":
                    createSample(() =>
                    {
                        var exchange = new TrackCodeExchange();
                        using (XLWorkbook xls = exchange.GetSample())
                        {
                            xls.SaveAs(Response.OutputStream);
                        }
                    }, "發票字軌資料.xlsx");
                    break;

                case "WinningNo":
                    createSample(() =>
                    {
                        DataTable table = new DataTable();
                        table.Columns.Add(new DataColumn("期別", typeof(String)));
                        table.Columns.Add(new DataColumn("字軌", typeof(String)));
                        table.Columns.Add(new DataColumn("號碼", typeof(String)));
                        table.Columns.Add(new DataColumn("中獎獎別", typeof(String)));
                        table.Columns.Add(new DataColumn("中獎獎金", typeof(int)));

                        DateTime sampleDate = (new DateTime(DateTime.Today.Year, (DateTime.Today.Month + 1) / 2 * 2, 1)).AddMonths(-2);
                        var row = table.NewRow();
                        row[0] = $"{sampleDate.Year - 1911:000}{sampleDate.Month:00}";
                        row[1] = "XX";
                        row[2] = "01234567";
                        row[3] = "D";
                        row[4] = "500";

                        table.Rows.Add(row);

                        using (DataSet ds = new DataSet())
                        {
                            table.TableName = "中獎清冊";
                            ds.Tables.Add(table);

                            using (var xls = ds.ConvertToExcel())
                            {
                                xls.SaveAs(Response.OutputStream);
                            }
                        }
                    }, "WinningSample.xlsx");
                    break;
            }

        }

        void createSample(Action creator,String fileName)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "message/rfc822";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode(fileName)));

            creator();

            Response.Flush();
            Response.End();
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