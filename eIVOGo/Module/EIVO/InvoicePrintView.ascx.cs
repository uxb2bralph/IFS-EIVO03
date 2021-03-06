﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.Locale;

namespace eIVOGo.Module.EIVO
{
    public partial class InvoicePrintView : System.Web.UI.UserControl
    {
        protected InvoiceItem _item;
        protected InvoiceBuyer _buyer;
        protected Organization _buyerOrg;
        protected UserProfileMember _userProfile;
        protected bool _isDuplicatedPrint;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        [Bindable(true)]
        public int? InvoiceID
        {
            get;
            set;
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
            this.PreRender += new EventHandler(InvoicePrintView_PreRender);
            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
        }

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();
            if (!_item.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice))
            {
                _item.CDS_Document.DocumentPrintLog.Add(new DocumentPrintLog
                {
                    PrintDate = DateTime.Now,
                    UID = _userProfile.UID,
                    TypeID = (int)Naming.DocumentTypeDefinition.E_Invoice
                });

            }
            //Change to DocumentPrintQueue
            //if (_item.InvoicePrintQueue != null)
            //{
            //    mgr.GetTable<InvoicePrintQueue>().DeleteOnSubmit(_item.InvoicePrintQueue);
            //}
            mgr.DeleteAnyOnSubmit<DocumentPrintQueue>(d => d.DocID == _item.InvoiceID);
            mgr.DeleteAnyOnSubmit<DocumentAuthorization>(d => d.DocID == _item.InvoiceID);
            mgr.SubmitChanges();
        }

        void InvoicePrintView_PreRender(object sender, EventArgs e)
        {
            if (InvoiceID.HasValue)
            {
                var mgr = dsEntity.CreateDataManager();
                _item = mgr.EntityList.Where(i => i.InvoiceID == InvoiceID).FirstOrDefault();
                _buyer = _item.InvoiceBuyer;
                if (_buyer != null && _buyer.BuyerID.HasValue)
                    _buyerOrg = _buyer.Organization;

                receiptView.Item = _item;
                balanceView.Item = _item;
                balanceView.Visible = !_item.InvoiceBuyer.IsB2C();
                _isDuplicatedPrint = _buyer.IsB2C() && _item.CDS_Document.DocumentPrintLog.Count > 0 && _item.CDS_Document.DocumentAuthorization == null;

                this.DataBind();
                
            }
        }
        
    }
}