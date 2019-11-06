using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Schema.EIVO;
using System.Text.RegularExpressions;
using System.Globalization;
using Model.DataEntity;
using DataAccessLayer.basis;
using Utility;
using Model.InvoiceManagement.ErrorHandle;
using System.Data;
using Model.Helper;

namespace Model.InvoiceManagement.enUS
{
    public static partial class CancelInvoiceRootValidator
    {
        #region 英文訊息專區

        public static Exception CheckMandatoryFields(this CancelInvoiceRootCancelInvoice invItem, GenericManager<EIVOEntityDataContext> mgr, OrganizationToken owner, out InvoiceItem invoice, out DateTime cancelDate)
        {
            invoice = null;
            cancelDate = default(DateTime);

            if (String.IsNullOrEmpty(invItem.CancelInvoiceNumber) || !Regex.IsMatch(invItem.CancelInvoiceNumber, "^[a-zA-Z]{2}[0-9]{8}$"))
            {
                return new Exception(String.Format("Error void CancelInvoiceNumber, CancelInvoiceNumber length should be set aside for the 10 yards (including track code)，傳送資料：{0}，TAG：< CancelInvoiceNumber />", invItem.CancelInvoiceNumber));
            }
            String invNo, trackCode;
            trackCode = invItem.CancelInvoiceNumber.Substring(0, 2);
            invNo = invItem.CancelInvoiceNumber.Substring(2);

            if (string.IsNullOrEmpty(invItem.InvoiceDate))
            {
                return new Exception("InvoiceDate error, Incorrect TAG:< InvoiceDate />");
            }

            if (!DateTime.TryParseExact(invItem.InvoiceDate, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime invoiceDate))
            {
                return new Exception(String.Format("Format of InvoiceDate error(YYYY/MM/DD), Incorrect TAG:< InvoiceDate />", invItem.InvoiceDate));
            }

            invItem.SellerId = invItem.SellerId.GetEfficientString();
            invoice = mgr.GetTable<InvoiceItem>().Where(i => i.No == invNo && i.TrackCode == trackCode).FirstOrDefault();

            if (invoice == null)
            {
                return new MarkToRetryException(String.Format("Invoice No. does not exist:{0}", invItem.CancelInvoiceNumber));
            }
            else if (invoice.Organization.ReceiptNo != invItem.SellerId)
            {
                return new Exception(String.Format("Invalid SellerId:{0}", invItem.SellerId));
            }

            if (invoice.SellerID != owner.CompanyID)
            {
                return new Exception(String.Format("Non-original invoice voided invoice Liren,Cancel Invoice Number:{0}", invItem.CancelInvoiceNumber));
            }

            if (invoice.InvoiceCancellation != null)
            {
                return new Exception(String.Format("Cancel Invoice already exists,Cancel Invoice Number:{0}", invItem.CancelInvoiceNumber));
            }


            if (String.IsNullOrEmpty(invItem.SellerId))
            {
                return new Exception("SellerId can not be blank，TAG:< SellerId />");
            }

            if (String.IsNullOrEmpty(invItem.CancelDate))
            {
                return new Exception("CancelDate can not be blank，TAG：< CancelDate />");
            }

            if (String.IsNullOrEmpty(invItem.CancelTime))
            {
                return new Exception("CancelTime can not be blank，TAG：< CancelTime />");
            }

            if (!DateTime.TryParseExact(String.Format("{0} {1}", invItem.CancelDate, invItem.CancelTime), "yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out cancelDate))
            {
                return new Exception(String.Format("Format of Cancel Date or Cancel Time error(YYYY/MM/DD HH:mm:ss), Incorrect:{0} {1}", invItem.CancelDate, invItem.CancelTime));
            }

            if (String.IsNullOrEmpty(invItem.CancelReason))
            {
                return new Exception("CancelReason can not be blank，TAG：< CancelReason />");
            }

            if (invItem.CancelReason.Length > 256)
            {
                return new Exception(String.Format("At least one yard length data format, up to 20 yards，Incorrect：{0}，TAG：< CancelReason />", invItem.CancelReason));
            }

            //備註
            if (invItem.Remark != null && invItem.Remark.Length > 200)
            {
                return new Exception(String.Format("Note length can not be more than 200 data，Incorrect：{0}，TAG：< Remark />", invItem.Remark));
            }

            return null;
        }

        public enum VoidInvoiceField
        {
            Void_Invoice_No = 0,
            Seller_ID,
            Invoice_Date,
            Void_Date,
            Reason,
            Return_Tax_Document_No,
            Remark
        }
        public static Exception VoidInvoice(this DataRow item, GenericManager<EIVOEntityDataContext> models, Organization owner, ref InvoiceCancellation voidItem, ref DerivedDocument p)
        {
            InvoiceItem invoice = null;

            if (String.IsNullOrEmpty(item.GetString((int)VoidInvoiceField.Void_Invoice_No)) || !Regex.IsMatch(item.GetString((int)VoidInvoiceField.Void_Invoice_No), "^[a-zA-Z]{2}[0-9]{8}$"))
            {
                return new Exception(String.Format("Invalid void invoice number：{0}", item.GetString((int)VoidInvoiceField.Void_Invoice_No)));
            }
            String invNo, trackCode;
            trackCode = item.GetString((int)VoidInvoiceField.Void_Invoice_No).Substring(0, 2);
            invNo = item.GetString((int)VoidInvoiceField.Void_Invoice_No).Substring(2);

            DateTime? invoiceDate = item.GetData<DateTime>((int)VoidInvoiceField.Invoice_Date);
            if (!invoiceDate.HasValue)
            {
                return new Exception("Invalid Invoice Date");
            }

            String sellerID = item.GetString((int)VoidInvoiceField.Seller_ID);
            invoice = models.GetTable<InvoiceItem>().Where(i => i.No == invNo && i.TrackCode == trackCode).FirstOrDefault();

            if (invoice == null)
            {
                return new MarkToRetryException(String.Format("Invoice No. does not exist:{0}", item.GetString((int)VoidInvoiceField.Void_Invoice_No)));
            }
            else if (invoice.Organization.ReceiptNo != sellerID)
            {
                return new Exception(String.Format("Invalid Seller ID:{0}", sellerID));
            }

            //if (invoice.SellerID != owner.CompanyID)
            //{
            //    return new Exception(String.Format("Non-original invoice voided invoice Liren,Cancel Invoice Number:{0}", item.GetString((int)VoidInvoiceField.Void_Invoice_No)));
            //}

            if (invoice.InvoiceCancellation != null)
            {
                return new Exception(String.Format("Duplicated void invoice, void invoice number:{0}", item.GetString((int)VoidInvoiceField.Void_Invoice_No)));
            }

            DateTime? voidDate = item.GetData<DateTime>((int)VoidInvoiceField.Void_Date);

            if (!voidDate.HasValue)
            {
                return new Exception("Invalid void date");
            }

            String reason = item.GetString((int)VoidInvoiceField.Reason);
            if (String.IsNullOrEmpty(reason))
            {
                return new Exception("Empty reason");
            }

            //if (item.CancelReason.Length > 256)
            //{
            //    return new Exception(String.Format("At least one yard length data format, up to 20 yards，Incorrect：{0}，TAG：< CancelReason />", item.CancelReason));
            //}

            voidItem = invoice.PrepareVoidItem(models, ref p);
            voidItem.CancellationNo = item.GetString((int)VoidInvoiceField.Void_Invoice_No);
            voidItem.Remark = item.GetString((int)VoidInvoiceField.Remark);
            voidItem.ReturnTaxDocumentNo = item.GetString((int)VoidInvoiceField.Return_Tax_Document_No);
            voidItem.CancelDate = voidDate;
            voidItem.CancelReason = reason;

            models.SubmitChanges();

            return null;
        }


        #endregion
    }
}