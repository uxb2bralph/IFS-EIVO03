using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using Model.InvoiceManagement;
using eIVOGo.Module.UI;
using Model.Locale;
using eIVOGo.Module.Inquiry.ForPrint;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class PrintReceiptForIncome : ReceiveReceipt
    {

        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var receipts = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.ReceiptItem);

                StringBuilder sb = new StringBuilder("您欲下載列印的收據資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("列印時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("收據號碼\t\t收據日期\t\t收據開立營業人\r\n");

                foreach (var receipt in receipts)
                {
                    sb.Append(receipt.No).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(receipt.ReceiptDate)).Append("\t")
                        .Append(receipt.Seller.CompanyName).Append("\r\n");
                }

                signContext.DataToSign = sb.ToString();
            }
        }

        protected override void doJob()
        {
            //Session["PrintDoc"] = _docID;
            _userProfile.EnqueueDocumentPrint(new InvoiceManager(dsEntity.CreateDataManager()), _docID);
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
            //    String.Format("window.open('{0}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/PrintReceiptAsPDF.aspx"))
            //    , true);

            LiteralControl lc = new LiteralControl(String.Format("<iframe src='{0}?printBack={1}' height='0' width='0'></iframe>"
                    , VirtualPathUtility.ToAbsolute("~/SAM/PrintReceiptAsPDF.aspx"), Request["printBack"]));
            this.Controls.Add(lc);

            //#region add DocumentPrintLogs
            
            ////1.此段原本參考發票列印的寫法寫在 SOGOReceiptView.ascx.cx 中,但因為父模板(底層)畫面流程會先執行[列印完.重新查詢畫面資料]的動作,
            ////  才在 SOGOReceiptView.ascx.cx 中執行此段加入DocumentPrintLogs至DB的動作,所以底層重新查詢的畫面資料並無法即時反映"已列印"的狀態
            ////  所以改寫再此 doJob 方法中
            ////2.由於,此處已先新增DocumentPrintLogs的資料,故接下來SOGOReceiptView.ascx 已無法再用DocumentPrintLogs有無資料來判斷收據是否為正本
            ////  所以新增一個Session["UnPrintDoc"]來記錄此份收據是否為正本,接下來SOGOReceiptView.ascx用此Session來判斷

            //List<int> unPrintID = new List<int>();
            //Session["UnPrintDoc"] = unPrintID; //用來紀錄此份收據是否為正本(未列印)

            //ReceiptDataSource dsEntity = new ReceiptDataSource();
            //var mgr = dsEntity.CreateDataManager();
            //ReceiptItem ritem = null;

            //IEnumerable<int> items = Session["PrintDoc"] as IEnumerable<int>;


            //if (items != null)
            //{
            //    foreach (var item in items)
            //    {
            //        ritem = mgr.EntityList.Where(r => r.ReceiptID == item).FirstOrDefault();

            //        if (!ritem.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Receipt))
            //        {
            //            ritem.CDS_Document.DocumentPrintLog.Add(new DocumentPrintLog
            //            {
            //                PrintDate = DateTime.Now,
            //                UID = _userProfile.UID,
            //                TypeID = (int)Naming.DocumentTypeDefinition.E_Receipt
            //            });

            //            unPrintID.Add(item); //收據是正本

            //        }
            //    }

            //    mgr.SubmitChanges();
            //}

            //#endregion

            PopupModal modal = (PopupModal)this.LoadControl("~/Module/UI/PopupModal.ascx");
            modal.InitializeAsUserControl(this.Page);
            modal.TitleName = _successfulMsg;
            _successfulMsg = null;
            this.Controls.Add(modal);
            modal.Show();
        }

    }
}