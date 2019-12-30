using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.InvoiceManagement.InvoiceProcess;
using Model.Locale;
using Model.Resource;
using Model.Schema.EIVO;

namespace Model.InvoiceManagement.Validator
{
    public class InvoiceRootInvoiceValidatorForCBE : InvoiceRootInvoiceValidator
    {
        protected bool _pdfSubscription;
        public InvoiceRootInvoiceValidatorForCBE(GenericManager<EIVOEntityDataContext> mgr, Organization owner)
            : base(mgr, owner)
        {

        }

        protected override Exception checkBusiness()
        {
            var result = base.checkBusiness();
            if(result!=null)
            {
                return result;
            }

            if (_isCrossBorderMerchant == true)
            {
                _pdfSubscription = _seller.OrganizationStatus.SubscribeB2BInvoicePDF == true;
            }

            return null;
        }

        public InvoiceItem SaveRootInvoice(InvoiceRootInvoice invItem, out Exception exception)
        {
            if ((exception = this.Validate(invItem)) != null)
            {
                return null;
            }

            InvoiceItem newItem = this.InvoiceItem;
            _mgr.GetTable<InvoiceItem>().InsertOnSubmit(newItem);

            C0401Handler.PushStepQueueOnSubmit(_mgr, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            C0401Handler.PushStepQueueOnSubmit(_mgr, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);

            _mgr.SubmitChanges();

            if(_pdfSubscription)
            {
                _mgr.PushDocumentSubscriptionQueue(newItem.CDS_Document.DocID);
            }

            return newItem;
        }
    }
}
