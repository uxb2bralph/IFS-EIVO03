using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Model.DataEntity;
using Utility;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.Locale;

namespace eIVOGo.Module.EIVO.Item
{
    public partial class SOGOReceiptView : System.Web.UI.UserControl
    {
        protected ReceiptItem _item;
        protected char[] _totalAmtChar;
        protected UserProfileMember _userProfile;
        //private bool _IsUnPrint = false; //用來判斷此份收據是否為正本(因已無法用DocumentPrintLogs有無資料來判斷收據是否為正本)

        //public bool IsUnPrint //用來判斷此份收據是否為正本(未列印)
        //{
        //    get { return _IsUnPrint; }
        //    set { _IsUnPrint = value; }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        [Bindable(true)]
        public ReceiptItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                if (_item != null)
                {
                    _totalAmtChar = ((int)_item.TotalAmount.Value).GetChineseNumberSeries(8);
                }
            }
        }

        [Bindable(true)]
        public bool? IsFinal
        {
            get;
            set;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(SOGOReceiptView_PreRender);
            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
        }

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();
            if (!_item.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Receipt))
            {
                _item.CDS_Document.DocumentPrintLog.Add(new DocumentPrintLog
                {
                    PrintDate = DateTime.Now,
                    UID = _userProfile.UID,
                    TypeID = (int)Naming.DocumentTypeDefinition.E_Receipt
                });
            }

            if (_item.CDS_Document.DocumentPrintQueue != null)
            {
                mgr.GetTable<DocumentPrintQueue>().DeleteOnSubmit(_item.CDS_Document.DocumentPrintQueue);
            }
            mgr.SubmitChanges();
        }

        void SOGOReceiptView_PreRender(object sender, EventArgs e)
        {
            if (_item != null)
            {
                rpList.DataSource = _item.ReceiptDetail;
                rpList.DataBind();
                this.DataBind();
            }
        }
    }
}