<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DataAccessLayer" %>
<%@ Import Namespace="ClosedXML.Excel" %>
<%@ Import Namespace="ModelExtension.Helper" %>

<script runat="server">

    ModelSource<InvoiceItem> models;
    Model.Security.MembershipManagement.UserProfileMember _userProfile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _userProfile = Business.Helper.WebPageUtility.UserProfile;

        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        models = (ModelSource<InvoiceItem>)_userProfile["modelSource"];
        _userProfile.Remove("modelSource");
        models.GetDataContext().CommandTimeout = 86400;

        createReport(models.Items);
    }

    void createReport(IQueryable<InvoiceItem> dataSource)
    {
        var items = models.Items.GroupBy(i => i.SellerID)
            //.Select(d => new
            //{
            //    SellerID = d.Key,
            //    RecordCount = d.Count(),
            //    InvoiceDate = d.OrderBy(i => i.InvoiceID).First().InvoiceDate
            //})
            .Join(models.GetTable<Organization>(), i => i.Key, o => o.CompanyID,
                (i, o) => new
                {
                    Seller = o,
                    Items = i
                    //SellerName = o.CompanyName,
                    //SellerReceiptNo = o.ReceiptNo,
                    //o.InvoiceItems.OrderBy(n => n.InvoiceDate).First().InvoiceDate,
                    //i.RecordCount
                });


        var details = items
            .Select(item => new
            {
                開立發票營業人 = item.Seller.CompanyName,
                統編 = item.Seller.ReceiptNo,
                上線日期 = item.Seller.InvoiceItems.OrderBy(i=>i.InvoiceDate).First().InvoiceDate,
                發票筆數 = item.Items.Count(),
            });

        Response.Clear();
        Response.ClearContent();
        Response.ClearHeaders();
        Response.AddHeader("Cache-control", "max-age=1");
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("發票資料統計.xlsx")));

        using (DataSet ds = new DataSet())
        {
            DataTable table = details.ToDataTable();
            table.TableName = "發票資料統計";
            ds.Tables.Add(table);

            using (var xls = ds.ConvertToExcel())
            {
                xls.SaveAs(Response.OutputStream);
            }
        }

        Response.Flush();
        Response.End();

        //using(XLWorkbook wb = new XLWorkbook())
        //{
        //    var ws = wb.Worksheets.Add("發票資料統計");
        //    int colIdx = 1;
        //    ws.Cell(1, colIdx++).Value = "開立發票營業人";
        //    ws.Cell(1, colIdx++).Value = "統編";
        //    ws.Cell(1, colIdx++).Value = "上線日期";
        //    ws.Cell(1, colIdx++).Value = "發票筆數";

        //    var row = ws.Row(2);
        //    foreach(var item in items)
        //    {
        //        colIdx = 1;
        //        row.Cell(colIdx++).Value =  item.SellerName;
        //        row.Cell(colIdx++).Value =  item.SellerReceiptNo;
        //        row.Cell(colIdx++).Value = String.Format("{0:yyyy/MM/dd}", item.InvoiceDate);
        //        row.Cell(colIdx++).Value =  item.RecordCount;

        //        row = row.RowBelow();
        //    }

        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.AddHeader("Cache-control", "max-age=1");
        //    Response.ContentType = "message/rfc822";
        //    Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("發票資料統計.xlsx")));

        //    wb.SaveAs(Response.OutputStream);
        //    Response.Flush();
        //    Response.End();
        //}

    }

    public override void Dispose()
    {
        if (models != null)
            models.Dispose();

        base.Dispose();
    }

</script>