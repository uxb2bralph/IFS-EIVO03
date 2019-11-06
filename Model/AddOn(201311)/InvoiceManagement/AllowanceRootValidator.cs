using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Schema.EIVO;
using System.Globalization;
using DataAccessLayer.basis;
using Model.DataEntity;
using System.Text.RegularExpressions;
using Model.Locale;

namespace Model.InvoiceManagement
{
    public static partial class AllowanceRootValidator
    {

        public static Exception CheckBusiness(this AllowanceRootAllowance item, GenericManager<EIVOEntityDataContext> mgr, OrganizationToken owner, out Organization seller)
        {
            seller = mgr.GetTable<Organization>().Where(o => o.ReceiptNo == item.SellerId).FirstOrDefault();

            if (seller == null)
            {
                return new Exception(String.Format("賣方為非註冊商家,開立人統一編號:{0}，TAG:< SellerId />", item.SellerId));
            }

            if (seller.CompanyID != owner.CompanyID)
            {
                return new Exception(String.Format("簽章設定人與發票開立人不符,開立人統一編號:{0}，TAG:< SellerId />", item.SellerId));
            }

            if (item.SellerName == null || item.SellerName.Length > 60)
            {
                return new Exception(String.Format("賣方-營業人名稱錯誤，長度最多60碼，傳送資料：{0}，TAG：< SellerName />", item.SellerName));
            }

            if (item.BuyerId == "0000000000")
            {
                if (item.BuyerName == null || Encoding.GetEncoding(950).GetBytes(item.BuyerName).Length != 4)
                {
                    return new Exception(String.Format("B2C買方名稱格式錯誤，長度為ASCII字元4碼或中文全形字元2碼，傳送資料：{0}，TAG:< BuyerName />", item.BuyerName));
                }
            }
            else if (item.BuyerId == null || !Regex.IsMatch(item.BuyerId, "^[0-9]{8}$"))
            {
                return new Exception(String.Format("買方識別碼錯誤，傳送資料：{0}，TAG:< BuyerId />", item.BuyerId));
            }
            else if (item.BuyerName.Length > 60)
            {
                return new Exception(String.Format("買方名稱格式錯誤，長度最多60碼，傳送資料：{0}，TAG:< BuyerName />", item.BuyerName));
            }

            return null;
        }

        //檢查基本必填項目(折讓單)
        public static Exception CheckMandatoryFields(this AllowanceRootAllowance item, out DateTime allowanceDate)
        {
            allowanceDate = default(DateTime);

            if (String.IsNullOrEmpty(item.AllowanceNumber))
            {
                return new Exception("折讓證明單號碼，TAG：< AllowanceNumber />");
            }

            //折讓證明單號碼
            if (item.AllowanceNumber.Length > 16)
            {
                return new Exception(String.Format("折讓證明單號碼長度超過16碼，傳送資料：{0}，TAG：< AllowanceNumber />", item.AllowanceNumber));
            }


            if (String.IsNullOrEmpty(item.AllowanceDate))
            {
                return new Exception("折讓證明單日期，TAG：< AllowanceDate />");
            }

            if (!DateTime.TryParseExact(item.AllowanceDate, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out allowanceDate))
            {
                return new Exception(String.Format("折讓證明單日期錯誤(YYYY/MM/DD)；傳送資料:{0}", item.AllowanceDate));
            }

            //折讓種類
            if (item.AllowanceType != "1" && item.AllowanceType != "2")
            {
                return new Exception(String.Format("折讓種類格式僅有 買方開立為\"1\"、賣方開立為\"2\"，傳送資料：{0}", item.AllowanceType));
            }

            return null;
        }

        public static Exception CheckAllowanceItem(this AllowanceRootAllowance item, GenericManager<EIVOEntityDataContext> mgr, out List<InvoiceAllowanceItem> productItems)
        {
            productItems = new List<InvoiceAllowanceItem>();
            var invTable = mgr.GetTable<InvoiceItem>();

            foreach (var i in item.AllowanceItem)
            {
                InvoiceItem originalInvoice = null;

                if (i.OriginalInvoiceNumber != null && i.OriginalInvoiceNumber.Length == 10)
                {
                    String invNo, trackCode;
                    trackCode = i.OriginalInvoiceNumber.Substring(0, 2);
                    invNo = i.OriginalInvoiceNumber.Substring(2);
                    originalInvoice = invTable.Where(n => n.TrackCode == trackCode && n.No == invNo).FirstOrDefault();
                }

                if (originalInvoice == null)
                {
                    return new Exception(String.Format("發票資料不存在，傳送資料：{0}，TAG：< OriginalInvoiceNumber />", i.OriginalInvoiceNumber));
                }

                if (originalInvoice.InvoiceBuyer.ReceiptNo != item.BuyerId && item.BuyerId != "0000000000")
                {
                    return new Exception(String.Format("發票買方統一編號錯誤，傳送資料：{0}", i.OriginalInvoiceNumber));
                }

                if (originalInvoice.InvoiceSeller.ReceiptNo != item.SellerId)
                {
                    return new Exception(String.Format("折讓單賣方與發票賣方資料不符，傳送資料：{0}", i.OriginalInvoiceNumber));
                }

                //原明細排列序號
                if (i.OriginalSequenceNumber > 1000 || i.OriginalSequenceNumber < 0)
                {
                    return new Exception(String.Format("原明細排列序號長度最多3碼，傳送資料：{0}，TAG：< OriginalInvoiceNumber />", i.OriginalSequenceNumber));
                }

                //折讓證明單明細排列序號
                if (i.AllowanceSequenceNumber > 1000 || i.AllowanceSequenceNumber < 0)
                {
                    return new Exception(String.Format("折讓證明單明細排列序號長度最多3碼，傳送資料：{0}，TAG：< AllowanceSequenceNumber />", i.AllowanceSequenceNumber));
                }

                //原品名
                if (i.OriginalDescription == null || i.OriginalDescription.Length > 256)
                {
                    return new Exception(String.Format("原品名錯誤，原品名長度最多256碼，傳送資料：{0}，TAG：< OriginalDescription />", i.OriginalDescription));
                }

                //單位
                if (i.Unit != null && i.Unit.Length > 6)
                {
                    return new Exception(String.Format("單位名稱長度最多6碼，傳送資料：{0}，TAG：< Unit />", i.Unit));
                }

                //課稅別
                if (!Enum.IsDefined(typeof(Naming.TaxTypeDefinition), i.TaxType))
                {
                    return new Exception(String.Format("課稅別格式錯誤，傳送資料：{0}，TAG：< TaxType />", i.TaxType));
                }

                DateTime invoiceDate;
                if (String.IsNullOrEmpty(i.OriginalInvoiceDate) || !DateTime.TryParseExact(String.Format("{0}", i.OriginalInvoiceDate), "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out invoiceDate))
                {
                    return new Exception(String.Format("原開立發票日期錯誤，格式(YYYY/MM/DD)；傳送資料:{0}", i.OriginalInvoiceDate));
                }

                var allowanceItem = new InvoiceAllowanceItem
                {
                    Amount = i.Amount,
                    InvoiceNo = i.OriginalInvoiceNumber,
                    InvoiceDate = invoiceDate,
                    ItemNo = i.Item,
                    OriginalSequenceNo = (short)i.OriginalSequenceNumber,
                    Piece = i.Quantity,
                    PieceUnit = i.Unit,
                    OriginalDescription = i.OriginalDescription,
                    TaxType = i.TaxType,
                    No = (short)i.AllowanceSequenceNumber,
                    UnitCost = i.UnitPrice,
                    Tax = i.Tax,
                };

                if (originalInvoice != null)
                {
                    var invProductItem = originalInvoice.InvoiceDetails.Join(mgr.GetTable<InvoiceProductItem>(), d => d.ProductID, p => p.ProductID, (d, p) => p)
                        .Where(p => p.No == i.OriginalSequenceNumber).FirstOrDefault();
                    if (invProductItem != null)
                    {
                        allowanceItem.ItemID = invProductItem.ItemID;
                    }
                }
                productItems.Add(allowanceItem);
            }

            return null;
        }

    }
}
