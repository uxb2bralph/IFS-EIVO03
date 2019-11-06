using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Schema.EIVO.B2B;
using Model.Locale;
using Utility;
using Model.Schema.TurnKey;

namespace Model.InvoiceManagement
{
    public partial class UXInvoiceManager : InvoiceManager
    {
        public UXInvoiceManager() : base() { }
        public UXInvoiceManager(GenericManager<EIVOEntityDataContext> mgr) : base(mgr) { }

        public virtual Dictionary<int, Exception> SaveUploadInvoice(SellerInvoiceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if (item != null && item.Invoice != null && item.Invoice.Length > 0)
            {
                for (int idx = 0; idx < item.Invoice.Length;idx++)
                {
                    var invItem = item.Invoice[idx];
                    try
                    {
                        Exception ex;
                        InvoiceItem newItem = ConvertToInvoiceItem(owner, invItem,out ex);

                        if (newItem == null)
                        {
                            result.Add(idx, ex);
                            continue;
                        }

                        this.EntityList.InsertOnSubmit(newItem);

                        this.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }
            }

            return result;
        }

        public InvoiceItem ConvertToInvoiceItem(OrganizationToken owner, SellerInvoiceRootInvoice invItem,out Exception ex)
        {
            ex = null;
            String invNo, trackCode;
            getInvoiceNo(invItem.InvoiceNumber, out invNo, out trackCode);

            var seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == invItem.SellerId).FirstOrDefault();
            if (seller == null)
            {
                ex =  new Exception(String.Format("發票開立人為非註冊會員,統一編號:{0}", invItem.SellerId));
                return null;
            }

            var buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == invItem.BuyerId).FirstOrDefault();
            if (buyer == null)
            {
                ex =  new Exception(String.Format("發票買受人為非註冊會員,統一編號:{0}", invItem.BuyerId));
                return null;
            }

            if (this.EntityList.Any(i => i.No == invNo && i.TrackCode == trackCode))
            {
                ex = new Exception(String.Format("發票號碼已存在:{0}", invItem.InvoiceNumber));
                return null;
            }

            InvoiceItem newItem = new InvoiceItem
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Invoice,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    },
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送
                },
                InvoiceBuyer = new InvoiceBuyer
                {
                    Name = invItem.BuyerName,
                    ReceiptNo = invItem.BuyerId,
                    Address = buyer.Addr,
                    ContactName = buyer.ContactName,
                    CustomerName = buyer.CompanyName,
                    EMail = buyer.ContactEmail,
                    Fax = buyer.Fax,
                    Phone = buyer.Phone,
                    PersonInCharge = buyer.UndertakerName,
                    BuyerID = buyer.CompanyID
                },
                InvoiceSeller = new InvoiceSeller
                {
                    Name = seller.CompanyName,
                    ReceiptNo = seller.ReceiptNo,
                    Address = seller.Addr,
                    ContactName = seller.ContactName,
                    CustomerName = seller.CompanyName,
                    EMail = seller.ContactEmail,
                    Fax = seller.Fax,
                    Phone = seller.Phone,
                    PersonInCharge = seller.UndertakerName,
                    SellerID = seller.CompanyID
                },
                InvoiceDate = String.IsNullOrEmpty(invItem.InvoiceTime) ? DateTime.ParseExact(invItem.InvoiceDate, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture) : DateTime.ParseExact(String.Format("{0} {1}", invItem.InvoiceDate, invItem.InvoiceTime), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture),
                InvoiceType = byte.Parse(invItem.InvoiceType),
                No = invNo,
                TrackCode = trackCode,
                SellerID = seller.CompanyID,
                InvoiceAmountType = new InvoiceAmountType
                {
                    DiscountAmount = invItem.DiscountAmount,
                    SalesAmount = invItem.SalesAmount,
                    TaxAmount = invItem.TaxAmount,
                    TaxType = invItem.TaxType,
                    TotalAmount = invItem.TotalAmount,
                    TotalAmountInChinese = Utility.ValueValidity.MoneyShow(invItem.TotalAmount)
                }
            };

            short seqNo = 0;

            var productItems = invItem.InvoiceItem.Select(i => new InvoiceProductItem
            {
                InvoiceProduct = new InvoiceProduct { Brief = i.Description },
                CostAmount = i.Amount,
                ItemNo = String.Format("{0}", i.SequenceNumber),
                Piece = (int?)i.Quantity,
                Piece2 = (int?)i.Quantity2,
                PieceUnit = i.Unit,
                PieceUnit2 = i.Unit2,
                UnitCost = i.UnitPrice,
                UnitCost2 = i.UnitPrice2,
                Remark = i.Remark,
                TaxType = invItem.TaxType,
                No = (seqNo++)
            });

            newItem.InvoiceDetails.AddRange(productItems.Select(p => new InvoiceDetail
            {
                InvoiceProduct = p.InvoiceProduct
            }));
            return newItem;
        }

        public Dictionary<int, Exception> SaveUploadInvoiceCancellation(CancelInvoiceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if (item != null && item.CancelInvoice != null && item.CancelInvoice.Length > 0)
            {
                for (int idx = 0; idx < item.CancelInvoice.Length;idx++ )
                {
                    var invItem = item.CancelInvoice[idx];
                    try
                    {
                        Exception ex;
                        InvoiceCancellation cancelItem = ConvertToInvoiceCancellation(owner, invItem, out ex);

                        if (cancelItem == null)
                        {
                            result.Add(idx, ex);
                            continue;
                        }

                        this.GetTable<InvoiceCancellation>().InsertOnSubmit(cancelItem);

                        var doc = new DerivedDocument
                        {
                            CDS_Document = new CDS_Document
                            {
                                DocType = (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation,
                                DocDate = DateTime.Now,
                                CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送,
                                DocumentOwner = new DocumentOwner
                                {
                                    OwnerID = owner.CompanyID
                                }
                            },
                            SourceID = cancelItem.InvoiceID
                        };
                        this.GetTable<DerivedDocument>().InsertOnSubmit(doc);

                        this.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }
            }

            return result;
        }

        public InvoiceCancellation ConvertToInvoiceCancellation(OrganizationToken owner, CancelInvoiceRootCancelInvoice cancellation,out Exception ex)
        {
            ex = null;

            String invNo, trackCode;
            if (cancellation.CancelInvoiceNumber.Length >= 10)
            {
                trackCode = cancellation.CancelInvoiceNumber.Substring(0, 2);
                invNo = cancellation.CancelInvoiceNumber.Substring(2);
            }
            else
            {
                trackCode = null;
                invNo = cancellation.CancelInvoiceNumber;
            }
            var invoice = this.EntityList.Where(i => i.No == invNo && i.TrackCode == trackCode).FirstOrDefault();

            if (invoice == null)
            {
                ex = new Exception(String.Format("發票號碼不存在:{0}", cancellation.CancelInvoiceNumber));
                return null;
            }

            if (invoice.InvoiceCancellation != null)
            {
                ex = new Exception(String.Format("作廢發票已存在,發票號碼:{0}", cancellation.CancelInvoiceNumber));
                return null;
            }

            InvoiceCancellation cancelItem = new InvoiceCancellation
            {
                InvoiceID = invoice.InvoiceID,
                CancellationNo = cancellation.CancelInvoiceNumber,
                Remark = String.Format("{0}{1}", cancellation.CancelReason, cancellation.Remark),
                ReturnTaxDocumentNo = cancellation.ReturnTaxDocumentNumber,
                CancelDate = DateTime.ParseExact(String.Format("{0} {1}", cancellation.CancelDate, cancellation.CancelTime), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
            };

            return cancelItem;
        }

        public virtual Dictionary<int, Exception> SaveUploadInvoice(BuyerInvoiceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if (item != null && item.Invoice != null && item.Invoice.Length > 0)
            {
                for (int idx = 0; idx < item.Invoice.Length; idx++)
                {
                    var invItem = item.Invoice[idx];
                    try
                    {

                        var seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == invItem.SellerId).FirstOrDefault();
                        if (seller == null)
                        {
                            result.Add(idx, new Exception(String.Format("發票開立人為非註冊會員,統一編號:{0}", invItem.SellerId)));
                            continue;
                        }

                        var buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == invItem.BuyerId).FirstOrDefault();
                        if (buyer == null)
                        {
                            result.Add(idx, new Exception(String.Format("發票買受人為非註冊會員,統一編號:{0}", invItem.BuyerId)));
                            continue;
                        }

                        InvoiceItem newItem = new InvoiceItem
                        {
                            CDS_Document = new CDS_Document
                            {
                                DocDate = DateTime.Now,
                                DocType = (int)Naming.DocumentTypeDefinition.E_Invoice,
                                DocumentOwner = new DocumentOwner
                                {
                                    OwnerID = owner.CompanyID
                                },
                                CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待開立
                            },
                            InvoiceBuyer = new InvoiceBuyer
                            {
                                Name = buyer.CompanyName,
                                ReceiptNo = invItem.BuyerId,
                                BuyerID = buyer.CompanyID
                            },
                            InvoiceType = byte.Parse(invItem.InvoiceType),
                            SellerID = seller.CompanyID,
                            InvoiceAmountType = new InvoiceAmountType
                            {
                                DiscountAmount = invItem.DiscountAmount,
                                SalesAmount = invItem.SalesAmount,
                                TaxAmount = invItem.TaxAmount,
                                TaxType = invItem.TaxType,
                                TotalAmount = invItem.TotalAmount,
                                TotalAmountInChinese = Utility.ValueValidity.MoneyShow(invItem.TotalAmount)
                            },
                            B2BBuyerInvoiceTag = new B2BBuyerInvoiceTag
                            {
                                DataNumber = invItem.DataNumber
                            }
                        };

                        short seqNo = 0;

                        var productItems = invItem.InvoiceItem.Select(i => new InvoiceProductItem
                        {
                            InvoiceProduct = new InvoiceProduct { Brief = i.Description },
                            CostAmount = i.Amount,
                            ItemNo = String.Format("{0}", i.SequenceNumber),
                            Piece = (int?)i.Quantity,
                            Piece2 = (int?)i.Quantity2,
                            PieceUnit = i.Unit,
                            PieceUnit2 = i.Unit2,
                            UnitCost = i.UnitPrice,
                            UnitCost2 = i.UnitPrice2,
                            Remark = i.Remark,
                            TaxType = invItem.TaxType,
                            No = (seqNo++)
                        });

                        newItem.InvoiceDetails.AddRange(productItems.Select(p => new InvoiceDetail
                        {
                            InvoiceProduct = p.InvoiceProduct
                        }));

                        this.EntityList.InsertOnSubmit(newItem);

                        this.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }
            }

            return result;
        }

        public InvoiceAllowance ConvertToInvoiceAllowance(OrganizationToken owner, AllowanceRootAllowance item, out Exception ex)
        {
            ex = null; DateTime dt; Decimal taxamount=0; Decimal totalamount=0;
          
            if (item.AllowanceNumber.Trim().Length == 0)
            {
                ex = new Exception(String.Format("折讓證明單號碼不得為空白"));
                return null;
            }
            //折讓證明單日期
            if (item.AllowanceDate.Trim().Length == 0)
            {
                ex = new Exception(String.Format("折讓證明單日期不得為空白"));
                return null;
            }
            else
            {
               if (!DateTime.TryParse(item.AllowanceDate, out dt))
                {
                    ex = new Exception(String.Format("折讓證明單日期格式有錯:{0}", item.AllowanceDate));
                    return null;
                }
            }
            //發票開立人
            if (string.IsNullOrEmpty(item.SellerId))
            {
                ex = new Exception(String.Format("發票開立人不得為空白"));
                return null;
            }
            var seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.SellerId).FirstOrDefault();
            if (seller == null)
            {
                ex = new Exception(String.Format("發票開立人為非註冊會員,統一編號:{0}", item.SellerId));
                return null;
            }
            //發票開立人名稱
            if (string.IsNullOrEmpty( item.SellerName ))
            {
                ex = new Exception(String.Format("發票開立人名稱不得為空白"));
                return null;
            }
            //發票買受人
            //發票開立人
            if (string.IsNullOrEmpty(item.BuyerId))
            {
                ex = new Exception(String.Format("發票買受人不得為空白"));
                return null;
            }
            var buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.BuyerId).FirstOrDefault();
            if (buyer == null)
            {
                ex = new Exception(String.Format("發票買受人為非註冊會員,統一編號:{0}", item.BuyerId));
                return null;
            }
            //發票買受人名稱
            if (string.IsNullOrEmpty(item.BuyerName))
            {
                ex = new Exception(String.Format("發票買受人名稱不得為空白"));
                return null;
            }
            //折讓種類
            if ((item.AllowanceType != 1) && (item.AllowanceType != 2))
            {
                ex = new Exception(String.Format("折讓證明單種類錯誤,號碼:{0}", item.AllowanceNumber));
                return null;
            }
            //買方開立折讓
            if (item.AllowanceType == 1)
            {
                if (this.GetTable<InvoiceAllowance>().Any(i => i.AllowanceNumber == item.AllowanceNumber.Trim() && i.BuyerId == item.BuyerId))
                {
                    ex = new Exception(String.Format("折讓證明單號碼已存在:{0}", item.AllowanceNumber));
                    return null;
                }
            }
            //賣方開立折讓
            if (item.AllowanceType == 2)
            {
                if (this.GetTable<InvoiceAllowance>().Any(i => i.AllowanceNumber == item.AllowanceNumber && i.SellerId == item.SellerId))
                {
                    ex = new Exception(String.Format("折讓證明單號碼已存在:{0}", item.AllowanceNumber));
                    return null;
                }
            }
           
            //折讓單明細發票號碼
            if (item.AllowanceItem.Where(i =>string.IsNullOrEmpty( i.OriginalInvoiceNumber ) ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票號碼不得為空白"));
                return null;
            }
            if (item.AllowanceItem.Where(i => i.OriginalInvoiceNumber.Trim().Length != 10).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票號碼長度有錯"));
                return null;
            }
            //折讓單明細發票原品名
            if (item.AllowanceItem.Where(i => string.IsNullOrEmpty(i.OriginalDescription)).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細原品名不得為空白"));
                return null;
            }
            //折讓單明細發票數量
            if (item.AllowanceItem.Where(i => i.Quantity > decimal.Parse("999999999999.9999") || i.Quantity < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票數量超過核可範圍"));
                return null;
            }
            //折讓單明細發票數量2
            if (item.AllowanceItem.Where(i => i.Quantity2 > decimal.Parse("999999999999.9999") || i.Quantity2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票數量2超過核可範圍"));
                return null;
            }
            //折讓單明細發票單價
            if (item.AllowanceItem.Where(i => i.UnitPrice > decimal.Parse("999999999999.9999") || i.UnitPrice < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票單價超過核可範圍"));
                return null;
            }
            //折讓單明細發票單價2
            if (item.AllowanceItem.Where(i => i.UnitPrice2 > decimal.Parse("999999999999.9999") || i.UnitPrice2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票單價2超過核可範圍"));
                return null;
            }
            //折讓單明細發票金額
            if (item.AllowanceItem.Where(i => i.Amount > decimal.Parse("999999999999.9999") || i.Amount < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票金額超過核可範圍"));
                return null;
            }
            //折讓單明細發票金額2
            if (item.AllowanceItem.Where(i => i.Amount2 > decimal.Parse("999999999999.9999") || i.Amount2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票金額2超過核可範圍"));
                return null;
            }
            //折讓單明細發票稅額
            if (item.AllowanceItem.Where(i => i.Tax > decimal.Parse("999999999999") || i.Tax < 0 || i.Tax.ToString ().IndexOf ('.') > 0 ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票稅額資料錯誤"));
                return null;
            }
            //折讓單明細發票課稅別
            if (item.AllowanceItem.Where(i => i.TaxType.ToString () != "1" && i.TaxType.ToString () != "2" &&  i.TaxType.ToString () != "3"  ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票課稅別資料錯誤"));
                return null;
            }
            //折讓單營業稅額合計
            if (item.TaxAmount  > decimal.Parse("999999999999") || item.TaxAmount < 0 ||  item.TaxAmount.ToString ().IndexOf ('.') > 0 )
            {
                ex = new Exception(String.Format("折讓單營業稅額合計資料錯誤"));
                return null;
            }
              //折讓單金額合計
            if (item.TotalAmount   > decimal.Parse("999999999999") || item.TotalAmount < 0 ||  item.TotalAmount.ToString ().IndexOf ('.') > 0 )
            {
                ex = new Exception(String.Format("折讓單金額合計資料錯誤"));
                return null;
            }

            //折讓單明細發票日期
            if (item.AllowanceItem.Where(i =>string.IsNullOrEmpty( i.OriginalInvoiceDate)).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票日期不得為空白"));
                return null;
            }
            else if (item.AllowanceItem.Where(i => i.OriginalInvoiceDate.Trim().Length != 10).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票日期長度有錯"));
                return null;
            }
            else
            {
                foreach (var AllowanceItem in item.AllowanceItem)
                {
                    taxamount += AllowanceItem.Tax;
                    totalamount += AllowanceItem.Amount + AllowanceItem.Amount2;
                    //折讓單明細發票日期格式
                    if (!DateTime.TryParse(AllowanceItem.OriginalInvoiceDate, out dt))
                    {
                        ex = new Exception(String.Format("折讓單明細發票日期格式有錯"));
                        return null;
                    }
                    else
                    {
                        if (DateTime.Parse(item.AllowanceDate) < DateTime.Parse(AllowanceItem.OriginalInvoiceDate))
                        {
                            ex = new Exception(String.Format("折讓單證明日期大於原發票日期"));
                            return null;

                        }

                    }
                    if( this.EntityList.Where(v => v.TrackCode + v.No == AllowanceItem.OriginalInvoiceNumber && v.InvoiceCancellation == null  ).Count () == 0 )
                    {
                         ex = new Exception(String.Format("折讓證明單明細之原始發票號碼不存在,折讓證明單號碼:{0}", item.AllowanceNumber));
                       return null;
                    }
                   if(item.AllowanceItem .Where (w=>w.AllowanceSequenceNumber == AllowanceItem .AllowanceSequenceNumber ).Count ()>1 )
                    {
                          ex = new Exception(String.Format("折讓單證明單明細序號重覆"));
                            return null;
                    }
                }
            }
            InvoiceAllowance newItem = new InvoiceAllowance
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Allowance,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    },
                    CurrentStep = item.AllowanceType == 1 ? (int)Naming.B2BInvoiceStepDefinition.待傳送 : (int)Naming.B2BInvoiceStepDefinition.待開立
                },
                AllowanceDate = DateTime.ParseExact(item.AllowanceDate, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture),
                AllowanceNumber = item.AllowanceNumber,
                AllowanceType = item.AllowanceType,
                BuyerId = item.BuyerId,
                SellerId = item.SellerId,
                TaxAmount = item.TaxAmount,
                TotalAmount = item.TotalAmount,
                InvoiceAllowanceSeller = new InvoiceAllowanceSeller
                {
                    SellerID = seller.CompanyID
                },
                InvoiceAllowanceBuyer = new InvoiceAllowanceBuyer
                {
                    BuyerID = buyer.CompanyID,
                    CustomerName = item.BuyerName
                }
            };

            //bool invalid = false;
            //foreach (var i in item.AllowanceItem)
            //{
            //    String invNo, trackCode;
            //    getInvoiceNo(i.OriginalInvoiceNumber, out invNo, out trackCode);

            //    var originalInvoice = this.EntityList.Where(v => v.TrackCode == trackCode && v.No == invNo).FirstOrDefault();
            //    if (originalInvoice == null)
            //    {
            //        invalid = true;
            //        break;
            //    }
            //}

            //if (invalid)
            //{
            //    ex = new Exception(String.Format("折讓證明單明細之原始發票號碼不存在,折讓證明單號碼:{0}", item.AllowanceNumber));
            //    return null;
            //}


            List<InvoiceAllowanceItem> productItems = new List<InvoiceAllowanceItem>();
            foreach (var i in item.AllowanceItem)
            {
                var allowanceItem = new InvoiceAllowanceItem
                {
                    Amount = i.Amount,
                    Amount2 = i.Amount2,
                    InvoiceNo = i.OriginalInvoiceNumber,
                    InvoiceDate = DateTime.ParseExact(i.OriginalInvoiceDate, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture),
                    OriginalSequenceNo = !String.IsNullOrEmpty(i.OriginalSequenceNumber) ? short.Parse(i.OriginalSequenceNumber) : (short?)null,
                    Piece = i.Quantity,
                    Piece2 = i.Quantity2,
                    PieceUnit = i.Unit,
                    PieceUnit2 = i.Unit2,
                    OriginalDescription = i.OriginalDescription,
                    TaxType = i.TaxType,
                    No = !String.IsNullOrEmpty(i.AllowanceSequenceNumber) ? short.Parse(i.AllowanceSequenceNumber) : (short?)null,
                    UnitCost = i.UnitPrice,
                    UnitCost2 = i.UnitPrice2
                };

                productItems.Add(allowanceItem);
            }

            newItem.InvoiceAllowanceDetails.AddRange(productItems.Select(p => new InvoiceAllowanceDetail
            {
                InvoiceAllowanceItem = p
            }));

            return newItem;
        }

        public InvoiceAllowance ConvertToInvoiceAllowance(OrganizationToken owner, Schema.TurnKey.B1101.Allowance allowance)
        {
            Organization buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == allowance.Main.Buyer.Identifier).FirstOrDefault();
            if (buyer == null)
            {
                buyer = new Organization
                {
                    Addr = allowance.Main.Buyer.Address,
                    CompanyName = allowance.Main.Buyer.Name,
                    UndertakerName = allowance.Main.Buyer.PersonInCharge,
                    Phone = allowance.Main.Buyer.TelephoneNumber,
                    Fax = allowance.Main.Buyer.FacsimileNumber,
                    ContactEmail = allowance.Main.Buyer.EmailAddress,
                    ReceiptNo = allowance.Main.Buyer.Identifier
                };
            }

            Organization seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == allowance.Main.Seller.Identifier).FirstOrDefault();
            if (buyer == null)
            {
                seller = new Organization
                {
                    Addr = allowance.Main.Seller.Address,
                    CompanyName = allowance.Main.Seller.Name,
                    UndertakerName = allowance.Main.Seller.PersonInCharge,
                    Phone = allowance.Main.Seller.TelephoneNumber,
                    Fax = allowance.Main.Seller.FacsimileNumber,
                    ContactEmail = allowance.Main.Seller.EmailAddress,
                    ReceiptNo = allowance.Main.Seller.Identifier
                };
            }

            InvoiceAllowance newItem = new InvoiceAllowance
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Allowance,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    },
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送
                },
                AllowanceDate = DateTime.ParseExact(allowance.Main.AllowanceDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                AllowanceNumber = allowance.Main.AllowanceNumber,
                AllowanceType = (byte)((int)allowance.Main.AllowanceType),
                BuyerId = allowance.Main.Buyer.Identifier,
                SellerId = allowance.Main.Seller.Identifier,
                TaxAmount = allowance.Amount.TaxAmount,
                TotalAmount = allowance.Amount.TotalAmount,
                InvoiceAllowanceSeller = new InvoiceAllowanceSeller
                {
                    Name = allowance.Main.Seller.Name,
                    ReceiptNo = allowance.Main.Seller.Identifier,
                    ContactName = allowance.Main.Seller.PersonInCharge,
                    Address = allowance.Main.Seller.Address,
                    CustomerID = allowance.Main.Seller.CustomerNumber,
                    CustomerName = allowance.Main.Seller.Name,
                    EMail = allowance.Main.Seller.EmailAddress,
                    Fax = allowance.Main.Seller.FacsimileNumber,
                    PersonInCharge = allowance.Main.Seller.PersonInCharge,
                    Phone = allowance.Main.Seller.TelephoneNumber,
                    RoleRemark = allowance.Main.Seller.RoleRemark,
                    Organization = seller
                },
                InvoiceAllowanceBuyer = new InvoiceAllowanceBuyer
                {
                    Name = allowance.Main.Buyer.Name,
                    ReceiptNo = allowance.Main.Buyer.Identifier,
                    ContactName = allowance.Main.Buyer.PersonInCharge,
                    Address = allowance.Main.Buyer.Address,
                    CustomerID = allowance.Main.Buyer.CustomerNumber,
                    CustomerName = allowance.Main.Buyer.Name,
                    EMail = allowance.Main.Buyer.EmailAddress,
                    Fax = allowance.Main.Buyer.FacsimileNumber,
                    PersonInCharge = allowance.Main.Buyer.PersonInCharge,
                    Phone = allowance.Main.Buyer.TelephoneNumber,
                    RoleRemark = allowance.Main.Buyer.RoleRemark,
                    Organization = buyer
                }
            };

            //bool invalid = false;
            //foreach (var i in allowance.Details)
            //{
            //    String invNo, trackCode;
            //    getInvoiceNo(i.OriginalInvoiceNumber, out invNo, out trackCode);

            //    var originalInvoice = this.EntityList.Where(v => v.TrackCode == trackCode && v.No == invNo).FirstOrDefault();
            //    if (originalInvoice == null)
            //    {
            //        invalid = true;
            //        break;
            //    }
            //}

            //if (invalid)
            //{
            //    throw new Exception(String.Format("折讓證明單明細之原始發票號碼不存在,折讓證明單號碼:{0}", allowance.Main.AllowanceNumber));
            //}


            List<InvoiceAllowanceItem> productItems = new List<InvoiceAllowanceItem>();
            foreach (var i in allowance.Details)
            {
                var allowanceItem = new InvoiceAllowanceItem
                {
                    Amount = i.Amount,
                    Amount2 = i.Amount2,
                    InvoiceNo = i.OriginalInvoiceNumber,
                    InvoiceDate = DateTime.ParseExact(i.OriginalInvoiceDate, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture),
                    OriginalSequenceNo = !String.IsNullOrEmpty(i.OriginalSequenceNumber) ? short.Parse(i.OriginalSequenceNumber) : (short?)null,
                    Piece = i.Quantity,
                    Piece2 = i.Quantity2,
                    PieceUnit = i.Unit,
                    PieceUnit2 = i.Unit2,
                    OriginalDescription = i.OriginalDescription,
                    TaxType = (byte)i.TaxType,
                    No = !String.IsNullOrEmpty(i.AllowanceSequenceNumber) ? short.Parse(i.AllowanceSequenceNumber) : (short?)null,
                    UnitCost = i.UnitPrice,
                    UnitCost2 = i.UnitPrice2
                };

                productItems.Add(allowanceItem);
            }

            newItem.InvoiceAllowanceDetails.AddRange(productItems.Select(p => new InvoiceAllowanceDetail
            {
                InvoiceAllowanceItem = p
            }));

            return newItem;
        }

        public InvoiceAllowance ConvertToInvoiceAllowance(OrganizationToken owner, Schema.TurnKey.B1401.Allowance allowance, out Exception ex)
        {
                ex = null; DateTime dt; Decimal taxamount=0; Decimal totalamount=0;
               var item = allowance.Main  ;
          
            if (item.AllowanceNumber.Trim().Length == 0)
            {
                ex = new Exception(String.Format("折讓證明單號碼不得為空白"));
                return null;
            }
            //折讓證明單日期
            if (item.AllowanceDate.Trim().Length == 0)
            {
                ex = new Exception(String.Format("折讓證明單日期不得為空白"));
                return null;
            }
            else
            {
               if (!DateTime.TryParse(item.AllowanceDate, out dt))
                {
                    ex = new Exception(String.Format("折讓證明單日期格式有錯:{0}", item.AllowanceDate));
                    return null;
                }
            }
            //發票開立人
            if (string.IsNullOrEmpty(item.Seller.Identifier ))
            {
                ex = new Exception(String.Format("發票開立人不得為空白"));
                return null;
            }
            var seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.Seller.Identifier).FirstOrDefault();
            if (seller == null)
            {
                ex = new Exception(String.Format("發票開立人為非註冊會員,統一編號:{0}", item.Seller.Identifier));
                return null;
            }
            //發票開立人名稱
            if (string.IsNullOrEmpty( item.Seller .Name  ))
            {
                ex = new Exception(String.Format("發票開立人名稱不得為空白"));
                return null;
            }
            //發票買受人
            //發票開立人
            if (string.IsNullOrEmpty(item.Buyer .Identifier ))
            {
                ex = new Exception(String.Format("發票買受人不得為空白"));
                return null;
            }
            var buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.Buyer .Identifier ).FirstOrDefault();
            if (buyer == null)
            {
                ex = new Exception(String.Format("發票買受人為非註冊會員,統一編號:{0}", item.Buyer .Identifier ));
                return null;
            }
            //發票買受人名稱
            if (string.IsNullOrEmpty(item.Buyer .Name ))
            {
                ex = new Exception(String.Format("發票買受人名稱不得為空白"));
                return null;
            }
            //折讓種類
            if ((item.AllowanceType  != Model.Schema.TurnKey.B1401.AllowanceTypeEnum.Item1 ) && (item.AllowanceType !=Model.Schema.TurnKey.B1401.AllowanceTypeEnum.Item2))
            {
                ex = new Exception(String.Format("折讓證明單種類錯誤,號碼:{0}", item.AllowanceNumber));
                return null;
            }
            //買方開立折讓
            if (item.AllowanceType == Model.Schema.TurnKey.B1401.AllowanceTypeEnum.Item1)
            {
                if (this.GetTable<InvoiceAllowance>().Any(i => i.AllowanceNumber == item.AllowanceNumber.Trim() && i.BuyerId == item.Buyer.Identifier ))
                {
                    ex = new Exception(String.Format("折讓證明單號碼已存在:{0}", item.AllowanceNumber));
                    return null;
                }
            }
            //賣方開立折讓
            if (item.AllowanceType == Model.Schema.TurnKey.B1401.AllowanceTypeEnum.Item2)
            {
                if (this.GetTable<InvoiceAllowance>().Any(i => i.AllowanceNumber == item.AllowanceNumber && i.SellerId == item.Seller .Identifier ))
                {
                    ex = new Exception(String.Format("折讓證明單號碼已存在:{0}", item.AllowanceNumber));
                    return null;
                }
            }
           
            //折讓單明細發票號碼
            if (allowance.Details .Where(i =>string.IsNullOrEmpty( i.OriginalInvoiceNumber ) ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票號碼不得為空白"));
                return null;
            }
            if (allowance.Details .Where(i => i.OriginalInvoiceNumber.Trim().Length != 10).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票號碼長度有錯"));
                return null;
            }
            //折讓單明細發票原品名
            if (allowance.Details .Where(i => string.IsNullOrEmpty(i.OriginalDescription)).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細原品名不得為空白"));
                return null;
            }
            //折讓單明細發票數量
            if (allowance.Details .Where(i => i.Quantity > decimal.Parse("999999999999.9999") || i.Quantity < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票數量超過核可範圍"));
                return null;
            }
            //折讓單明細發票數量2
            if (allowance.Details .Where(i => i.Quantity2 > decimal.Parse("999999999999.9999") || i.Quantity2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票數量2超過核可範圍"));
                return null;
            }
            //折讓單明細發票單價
            if (allowance.Details .Where(i => i.UnitPrice > decimal.Parse("999999999999.9999") || i.UnitPrice < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票單價超過核可範圍"));
                return null;
            }
            //折讓單明細發票單價2
            if (allowance.Details .Where(i => i.UnitPrice2 > decimal.Parse("999999999999.9999") || i.UnitPrice2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票單價2超過核可範圍"));
                return null;
            }
            //折讓單明細發票金額
            if (allowance.Details .Where(i => i.Amount > decimal.Parse("999999999999.9999") || i.Amount < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票金額超過核可範圍"));
                return null;
            }
            //折讓單明細發票金額2
            if (allowance.Details .Where(i => i.Amount2 > decimal.Parse("999999999999.9999") || i.Amount2 < decimal.Parse("-999999999999.9999")).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票金額2超過核可範圍"));
                return null;
            }
            //折讓單明細發票稅額
            if (allowance.Details .Where(i => i.Tax > decimal.Parse("999999999999") || i.Tax < 0 || i.Tax.ToString ().IndexOf ('.') > 0 ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票稅額資料錯誤"));
                return null;
            }
            //折讓單明細發票課稅別
            if (allowance.Details .Where(i => i.TaxType.ToString () != "1" && i.TaxType.ToString () != "2" &&  i.TaxType.ToString () != "3"  ).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票課稅別資料錯誤"));
                return null;
            }
            //折讓單營業稅額合計
            if (allowance.Amount .TaxAmount  > decimal.Parse("999999999999") || allowance.Amount.TaxAmount < 0 ||  allowance.Amount.TaxAmount.ToString ().IndexOf ('.') > 0 )
            {
                ex = new Exception(String.Format("折讓單營業稅額合計資料錯誤"));
                return null;
            }
              //折讓單金額合計
            if (allowance.Amount.TotalAmount   > decimal.Parse("999999999999") || allowance.Amount.TotalAmount < 0 ||  allowance.Amount.TotalAmount.ToString ().IndexOf ('.') > 0 )
            {
                ex = new Exception(String.Format("折讓單金額合計資料錯誤"));
                return null;
            }

            //折讓單明細發票日期
            if (allowance.Details.Where(i =>string.IsNullOrEmpty( i.OriginalInvoiceDate)).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票日期不得為空白"));
                return null;
            }
            else if (allowance.Details.Where(i => i.OriginalInvoiceDate.Trim().Length != 10).Count() > 0)
            {
                ex = new Exception(String.Format("折讓單明細發票日期長度有錯"));
                return null;
            }
            else
            {
                foreach (var AllowanceItem in allowance.Details)
                {
                    taxamount += AllowanceItem.Tax;
                    totalamount += AllowanceItem.Amount + AllowanceItem.Amount2;
                    //折讓單明細發票日期格式
                    if (!DateTime.TryParse(AllowanceItem.OriginalInvoiceDate, out dt))
                    {
                        ex = new Exception(String.Format("折讓單明細發票日期格式有錯"));
                        return null;
                    }
                    else
                    {
                        if (DateTime.Parse(item.AllowanceDate) < DateTime.Parse(AllowanceItem.OriginalInvoiceDate))
                        {
                            ex = new Exception(String.Format("折讓單證明日期大於原發票日期"));
                            return null;

                        }

                    }
                    if( this.EntityList.Where(v => v.TrackCode + v.No == AllowanceItem.OriginalInvoiceNumber && v.InvoiceCancellation == null  ).Count () == 0 )
                    {
                         ex = new Exception(String.Format("折讓證明單明細之原始發票號碼不存在,折讓證明單號碼:{0}", item.AllowanceNumber));
                       return null;
                    }
                   if(allowance.Details .Where (w=>w.AllowanceSequenceNumber == AllowanceItem .AllowanceSequenceNumber ).Count ()>1 )
                    {
                          ex = new Exception(String.Format("折讓單證明單明細序號重覆"));
                            return null;
                    }
                }
            }
            //Organization buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == allowance.Main.Buyer.Identifier).FirstOrDefault();
            //if (buyer == null)
            //{
            //    buyer = new Organization
            //    {
            //        Addr = allowance.Main.Buyer.Address,
            //        CompanyName = allowance.Main.Buyer.Name,
            //        UndertakerName = allowance.Main.Buyer.PersonInCharge,
            //        Phone = allowance.Main.Buyer.TelephoneNumber,
            //        Fax = allowance.Main.Buyer.FacsimileNumber,
            //        ContactEmail = allowance.Main.Buyer.EmailAddress,
            //        ReceiptNo = allowance.Main.Buyer.Identifier
            //    };
            //}

           // Organization seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == allowance.Main.Seller.Identifier).FirstOrDefault();
            //if (seller == null)
            //{
            //    seller = new Organization
            //    {
            //        Addr = allowance.Main.Seller.Address,
            //        CompanyName = allowance.Main.Seller.Name,
            //        UndertakerName = allowance.Main.Seller.PersonInCharge,
            //        Phone = allowance.Main.Seller.TelephoneNumber,
            //        Fax = allowance.Main.Seller.FacsimileNumber,
            //        ContactEmail = allowance.Main.Seller.EmailAddress,
            //        ReceiptNo = allowance.Main.Seller.Identifier
            //    };
            //}

            InvoiceAllowance newItem = new InvoiceAllowance
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Allowance,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    },
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送
                },
                AllowanceDate = DateTime.ParseExact(allowance.Main.AllowanceDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                AllowanceNumber = allowance.Main.AllowanceNumber,
                AllowanceType = (byte)((int)allowance.Main.AllowanceType),
                BuyerId = allowance.Main.Buyer.Identifier,
                SellerId = allowance.Main.Seller.Identifier,
                TaxAmount = allowance.Amount.TaxAmount,
                TotalAmount = allowance.Amount.TotalAmount,
                InvoiceAllowanceSeller = new InvoiceAllowanceSeller
                {
                    Name = allowance.Main.Seller.Name,
                    ReceiptNo = allowance.Main.Seller.Identifier,
                    ContactName = allowance.Main.Seller.PersonInCharge,
                    Address = allowance.Main.Seller.Address,
                    CustomerID = allowance.Main.Seller.CustomerNumber,
                    CustomerName = allowance.Main.Seller.Name,
                    EMail = allowance.Main.Seller.EmailAddress,
                    Fax = allowance.Main.Seller.FacsimileNumber,
                    PersonInCharge = allowance.Main.Seller.PersonInCharge,
                    Phone = allowance.Main.Seller.TelephoneNumber,
                    RoleRemark = allowance.Main.Seller.RoleRemark,
                    Organization = seller
                },
                InvoiceAllowanceBuyer = new InvoiceAllowanceBuyer
                {
                    Name = allowance.Main.Buyer.Name,
                    ReceiptNo = allowance.Main.Buyer.Identifier,
                    ContactName = allowance.Main.Buyer.PersonInCharge,
                    Address = allowance.Main.Buyer.Address,
                    CustomerID = allowance.Main.Buyer.CustomerNumber,
                    CustomerName = allowance.Main.Buyer.Name,
                    EMail = allowance.Main.Buyer.EmailAddress,
                    Fax = allowance.Main.Buyer.FacsimileNumber,
                    PersonInCharge = allowance.Main.Buyer.PersonInCharge,
                    Phone = allowance.Main.Buyer.TelephoneNumber,
                    RoleRemark = allowance.Main.Buyer.RoleRemark,
                    Organization = buyer
                }
            };

            //bool invalid = false;
            //foreach (var i in allowance.Details)
            //{
            //    String invNo, trackCode;
            //    getInvoiceNo(i.OriginalInvoiceNumber, out invNo, out trackCode);

            //    var originalInvoice = this.EntityList.Where(v => v.TrackCode == trackCode && v.No == invNo).FirstOrDefault();
            //    if (originalInvoice == null)
            //    {
            //        invalid = true;
            //        break;
            //    }
            //}

            //if (invalid)
            //{
            //    throw new Exception(String.Format("折讓證明單明細之原始發票號碼不存在,折讓證明單號碼:{0}", allowance.Main.AllowanceNumber));
            //}


            List<InvoiceAllowanceItem> productItems = new List<InvoiceAllowanceItem>();
            foreach (var i in allowance.Details)
            {
                var allowanceItem = new InvoiceAllowanceItem
                {
                    Amount = i.Amount,
                    Amount2 = i.Amount2,
                    InvoiceNo = i.OriginalInvoiceNumber,
                    InvoiceDate = DateTime.ParseExact(i.OriginalInvoiceDate, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture),
                    OriginalSequenceNo = !String.IsNullOrEmpty(i.OriginalSequenceNumber) ? short.Parse(i.OriginalSequenceNumber) : (short?)null,
                    Piece = i.Quantity,
                    Piece2 = i.Quantity2,
                    PieceUnit = i.Unit,
                    PieceUnit2 = i.Unit2,
                    OriginalDescription = i.OriginalDescription,
                    TaxType = (byte)i.TaxType,
                    No = !String.IsNullOrEmpty(i.AllowanceSequenceNumber) ? short.Parse(i.AllowanceSequenceNumber) : (short?)null,
                    UnitCost = i.UnitPrice,
                    UnitCost2 = i.UnitPrice2
                };

                productItems.Add(allowanceItem);
            }

            newItem.InvoiceAllowanceDetails.AddRange(productItems.Select(p => new InvoiceAllowanceDetail
            {
                InvoiceAllowanceItem = p
            }));

            return newItem;
        }

        public Dictionary<int, Exception> SaveUploadAllowance(Model.Schema.TurnKey.B1401.Allowance allowance, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
          
                    try
                    {
                        Exception ex;
                        InvoiceAllowance newItem = ConvertToInvoiceAllowance(owner, allowance,out ex );
                         if (newItem == null)
                        {
                            result.Add(1, ex);
                            
                        }
                         else
                         {
                        this.GetTable<InvoiceAllowance>().InsertOnSubmit(newItem);
                        this.SubmitChanges();
                         }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                       
                    }
                
            

            return result;
        }

        public Dictionary<int, Exception> SaveUploadAllowanceCancellation(CancelAllowanceRoot root, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if (root != null && root.CancelAllowance != null && root.CancelAllowance.Length > 0)
            {
                var tblCancel = this.GetTable<InvoiceAllowanceCancellation>();

                for (int idx=0; idx<root.CancelAllowance.Length;idx++)
                {
                    var item = root.CancelAllowance[idx];
                    try
                    {
                        Exception ex;
                        InvoiceAllowanceCancellation cancelItem = ConvertToAllowanceCancellation(owner, item, out ex);

                        if (cancelItem == null)
                        {
                            result.Add(idx, ex);
                            continue;
                        }

                        this.GetTable<InvoiceAllowanceCancellation>().InsertOnSubmit(cancelItem);

                        var doc = new DerivedDocument
                        {
                            CDS_Document = new CDS_Document
                            {
                                DocType = (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation,
                                DocDate = DateTime.Now,
                                CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送,
                                DocumentOwner = new DocumentOwner
                                {
                                    OwnerID = owner.CompanyID
                                }
                            },
                            SourceID = cancelItem.AllowanceID
                        };

                        this.GetTable<DerivedDocument>().InsertOnSubmit(doc);

                        this.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }
            }

            return result;
        }

        public Dictionary<int, Exception> SaveUploadAllowanceCancellation(Model.Schema.TurnKey.B0501.CancelAllowance CancelAllowance, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();

            var item = CancelAllowance;
                    try
                    {
                        Exception ex;
                        InvoiceAllowanceCancellation cancelItem = ConvertToAllowanceCancellation(owner, item, out ex);

                        if (cancelItem == null)
                        {
                            result.Add(1, ex);

                        }
                        else
                        {
                            this.GetTable<InvoiceAllowanceCancellation>().InsertOnSubmit(cancelItem);

                            var doc = new DerivedDocument
                            {
                                CDS_Document = new CDS_Document
                                {
                                    DocType = (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation,
                                    DocDate = DateTime.Now,
                                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送,
                                    DocumentOwner = new DocumentOwner
                                    {
                                        OwnerID = owner.CompanyID
                                    }
                                },
                                SourceID = cancelItem.AllowanceID
                            };

                            this.GetTable<DerivedDocument>().InsertOnSubmit(doc);

                            this.SubmitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(1, ex);
                    }
                

            return result;
        }


        public InvoiceAllowanceCancellation ConvertToAllowanceCancellation(OrganizationToken owner, CancelAllowanceRootCancelAllowance item,out Exception ex)
        {
            ex = null;DateTime dt;

            var allowance = this.GetTable<InvoiceAllowance>().Where(i => i.AllowanceNumber == item.CancelAllowanceNumber ).FirstOrDefault();

            if (allowance == null)
            {
                ex =  new Exception(String.Format("折讓證明單號碼不存在:{0}", item.CancelAllowanceNumber));
                return null;
            }
            if (string.IsNullOrEmpty (item.AllowanceDate))
            {
                ex = new Exception(String.Format("折讓證明單日期不得為空白"));
                return null;
            }
            if (!DateTime.TryParse  (item.AllowanceDate,out dt))
            {
                ex = new Exception(String.Format("折讓證明單日期格式有誤"));
                return null;
            }
            if (string.IsNullOrEmpty(item.CancelDate))
            {
                ex = new Exception(String.Format("作廢折讓單日期不得為空白"));
                return null;
            }
            if (!DateTime.TryParse(item.CancelDate, out dt))
            {
                ex = new Exception(String.Format("作廢折讓單日期格式有誤"));
                return null;
            }
            if (DateTime.Parse(item.CancelDate) < DateTime.Parse(item.AllowanceDate))
            {
                ex = new Exception(String.Format("作廢折讓單日期不可小於折讓日期"));
                return null;
            }
            if (string.IsNullOrEmpty(item.CancelTime))
            {
                ex = new Exception(String.Format("作廢折讓單時間不得為空白"));
                return null;
            }
            if (!DateTime.TryParse(item.CancelDate + " " + item.CancelTime, out dt))
            {
                ex = new Exception(String.Format("作廢折讓單時間格式有誤"));
                return null;
            }
            if (allowance.AllowanceDate.Value != DateTime.Parse(item.AllowanceDate))
            {
                ex = new Exception(String.Format("折讓證明單日期不合,折讓單日期:{0}", item.AllowanceDate));
                return null;
            }
            if (allowance.InvoiceAllowanceCancellation != null)
            {
                ex =  new Exception(String.Format("作廢折讓單已存在,折讓單號碼:{0}", item.CancelAllowanceNumber));
                return null;
            }
            if (string.IsNullOrEmpty(item.CancelReason ))
            {
                ex = new Exception(String.Format("作廢折讓單原因不得為空白"));
                return null;
            }

            InvoiceAllowanceCancellation cancelItem = new InvoiceAllowanceCancellation
            {
                AllowanceID = allowance.AllowanceID,
                Remark =item.CancelReason +" "+ item.Remark,
                CancelDate = DateTime.ParseExact(String.Format("{0} {1}", item.CancelDate, item.CancelTime), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
            };

            return cancelItem;
        }

        public InvoiceItem ConvertToInvoiceItem(OrganizationToken owner, Model.Schema.TurnKey.A1101.Invoice invoice)
        {
            Organization buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == invoice.Main.Buyer.Identifier).FirstOrDefault();
            if (buyer == null)
            {
                buyer = new Organization
                {
                    Addr = invoice.Main.Buyer.Address,
                    CompanyName = invoice.Main.Buyer.Name,
                    UndertakerName = invoice.Main.Buyer.PersonInCharge,
                    Phone = invoice.Main.Buyer.TelephoneNumber,
                    Fax = invoice.Main.Buyer.FacsimileNumber,
                    ContactEmail = invoice.Main.Buyer.EmailAddress,
                    ReceiptNo = invoice.Main.Buyer.Identifier
                };
            }

            Organization seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == invoice.Main.Seller.Identifier).FirstOrDefault();
            if (buyer == null)
            {
                seller = new Organization
                {
                    Addr = invoice.Main.Seller.Address,
                    CompanyName = invoice.Main.Seller.Name,
                    UndertakerName = invoice.Main.Seller.PersonInCharge,
                    Phone = invoice.Main.Seller.TelephoneNumber,
                    Fax = invoice.Main.Seller.FacsimileNumber,
                    ContactEmail = invoice.Main.Seller.EmailAddress,
                    ReceiptNo = invoice.Main.Seller.Identifier
                };
            }

            String invNo, trackCode;
            getInvoiceNo(invoice.Main.InvoiceNumber, out invNo, out trackCode);

            InvoiceItem newItem = new InvoiceItem
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Invoice,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    },
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送
                },
                InvoiceBuyer = new InvoiceBuyer
                {
                    Name = invoice.Main.Buyer.Name,
                    ReceiptNo = invoice.Main.Buyer.Identifier,
                    ContactName = invoice.Main.Buyer.PersonInCharge,
                    Address = invoice.Main.Buyer.Address,
                    CustomerID = invoice.Main.Buyer.CustomerNumber,
                    CustomerName = invoice.Main.Buyer.Name,
                    EMail = invoice.Main.Buyer.EmailAddress,
                    Fax = invoice.Main.Buyer.FacsimileNumber,
                    PersonInCharge = invoice.Main.Buyer.PersonInCharge,
                    Phone = invoice.Main.Buyer.TelephoneNumber,
                    RoleRemark = invoice.Main.Buyer.RoleRemark,
                    Organization = buyer
                },
                InvoiceSeller = new InvoiceSeller
                {
                    Name = invoice.Main.Seller.Name,
                    ReceiptNo = invoice.Main.Seller.Identifier,
                    ContactName = invoice.Main.Seller.PersonInCharge,
                    Address = invoice.Main.Seller.Address,
                    CustomerID = invoice.Main.Seller.CustomerNumber,
                    CustomerName = invoice.Main.Seller.Name,
                    EMail = invoice.Main.Seller.EmailAddress,
                    Fax = invoice.Main.Seller.FacsimileNumber,
                    PersonInCharge = invoice.Main.Seller.PersonInCharge,
                    Phone = invoice.Main.Seller.TelephoneNumber,
                    RoleRemark = invoice.Main.Seller.RoleRemark,
                    Organization = seller
                },
                InvoiceDate = DateTime.ParseExact(String.Format("{0}", invoice.Main.InvoiceDate), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).Add(invoice.Main.InvoiceTimeSpecified ? invoice.Main.InvoiceTime.TimeOfDay : TimeSpan.Zero),
                InvoiceType = (byte)((int)invoice.Main.Invoice),
                No = invNo,
                TrackCode = trackCode,
                SellerID = seller.CompanyID,
                BuyerRemark = invoice.Main.BuyerRemarkSpecified ? String.Format("{0}", (int)invoice.Main.BuyerRemark) : null,
                Category = invoice.Main.Category,
                CheckNo = invoice.Main.CheckNumber,
                DonateMark = ((int)invoice.Main.DonateMark).ToString(),
                CustomsClearanceMark = invoice.Main.CustomsClearanceMarkSpecified ? ((int)invoice.Main.CustomsClearanceMark).ToString() : null,
                GroupMark = invoice.Main.GroupMark,
                PermitDate = String.IsNullOrEmpty(invoice.Main.PermitDate) ? (DateTime?)null : DateTime.ParseExact(invoice.Main.PermitDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                PermitNumber = invoice.Main.PermitNumber,
                PermitWord = invoice.Main.PermitWord,
                RelateNumber = invoice.Main.RelateNumber,
                Remark = invoice.Main.MainRemark,
                TaxCenter = invoice.Main.TaxCenter,
                InvoiceAmountType = new InvoiceAmountType
                {
                    DiscountAmount = invoice.Amount.DiscountAmountSpecified ? invoice.Amount.DiscountAmount : (decimal?)null,
                    SalesAmount = invoice.Amount.SalesAmount,
                    TaxAmount = invoice.Amount.TaxAmount,
                    TaxType = (byte)((int)invoice.Amount.TaxType),
                    TotalAmount = invoice.Amount.TotalAmount,
                    TotalAmountInChinese = Utility.ValueValidity.MoneyShow(invoice.Amount.TotalAmount),
                    TaxRate = invoice.Amount.TaxRate,
                    CurrencyID = invoice.Amount.CurrencySpecified ? (int)invoice.Amount.Currency : (int?)null,
                    ExchangeRate = invoice.Amount.ExchangeRateSpecified ? invoice.Amount.ExchangeRate : (decimal?)null,
                    OriginalCurrencyAmount = invoice.Amount.OriginalCurrencyAmountSpecified ? invoice.Amount.OriginalCurrencyAmount : (decimal?)null
                }
            };

            short seqNo = 0;

            var productItems = invoice.Details.Select(i => new InvoiceProductItem
            {
                InvoiceProduct = new InvoiceProduct { Brief = i.Description },
                CostAmount = i.Amount,
                CostAmount2 = i.Amount2,
                ItemNo = i.SequenceNumber,
                Piece = (int?)i.Quantity,
                Piece2 = (int?)i.Quantity2,
                PieceUnit = i.Unit,
                PieceUnit2 = i.Unit2,
                UnitCost = i.UnitPrice,
                UnitCost2 = i.UnitPrice2,
                Remark = i.Remark,
                TaxType = newItem.InvoiceAmountType.TaxType,
                RelateNumber = i.RelateNumber,
                No = (seqNo++)
            });

            newItem.InvoiceDetails.AddRange(productItems.Select(p => new InvoiceDetail
            {
                InvoiceProduct = p.InvoiceProduct
            }));
            return newItem;
        }

        public void SaveA1101(Model.Schema.TurnKey.A1101.Invoice invoice, OrganizationToken owner)
        {
            InvoiceItem newItem = ConvertToInvoiceItem(owner, invoice);
            this.EntityList.InsertOnSubmit(newItem);
            this.SubmitChanges();
        }

        public void SaveB1101(Schema.TurnKey.B1101.Allowance allowance, OrganizationToken owner)
        {
            InvoiceAllowance newItem = ConvertToInvoiceAllowance(owner, allowance);
            this.GetTable<InvoiceAllowance>().InsertOnSubmit(newItem);
            this.SubmitChanges();
        }

        public InvoiceCancellation ConvertToInvoiceCancellation(OrganizationToken owner, Model.Schema.TurnKey.A0201.CancelInvoice cancellation)
        {
            String invNo, trackCode;
            if (cancellation.CancelInvoiceNumber.Length >= 10)
            {
                trackCode = cancellation.CancelInvoiceNumber.Substring(0, 2);
                invNo = cancellation.CancelInvoiceNumber.Substring(2);
            }
            else
            {
                trackCode = null;
                invNo = cancellation.CancelInvoiceNumber;
            }
            var invoice = this.EntityList.Where(i => i.No == invNo && i.TrackCode == trackCode).FirstOrDefault();

            if (invoice == null)
            {
                throw new Exception(String.Format("發票號碼不存在:{0}", cancellation.CancelInvoiceNumber));
            }

            if (invoice.InvoiceCancellation != null)
            {
                throw new Exception(String.Format("作廢發票已存在,發票號碼:{0}", cancellation.CancelInvoiceNumber));
            }

            InvoiceCancellation cancelItem = new InvoiceCancellation
            {
                InvoiceID = invoice.InvoiceID,
                CancellationNo = cancellation.CancelInvoiceNumber,
                Remark = String.Format("{0}{1}", cancellation.CancelReason, cancellation.Remark),
                ReturnTaxDocumentNo = cancellation.ReturnTaxDocumentNumber,
                CancelDate = DateTime.ParseExact(String.Format("{0}", cancellation.CancelDate), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).Add(cancellation.CancelTime.TimeOfDay)
            };

            return cancelItem;
        }


        public void SaveA0201(Schema.TurnKey.A0201.CancelInvoice item, OrganizationToken owner)
        {
            InvoiceCancellation cancelItem = ConvertToInvoiceCancellation(owner, item);
            this.GetTable<InvoiceCancellation>().InsertOnSubmit(cancelItem);

            var doc = new DerivedDocument
            {
                CDS_Document = new CDS_Document
                {
                    DocType = (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation,
                    DocDate = DateTime.Now,
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    }
                },
                SourceID = cancelItem.InvoiceID
            };

            this.GetTable<DerivedDocument>().InsertOnSubmit(doc);
            this.SubmitChanges();
        }

        public InvoiceAllowanceCancellation ConvertToAllowanceCancellation(OrganizationToken owner, Model.Schema.TurnKey.B0201.CancelAllowance item)
        {
            var allowance = this.GetTable<InvoiceAllowance>().Where(i => i.AllowanceNumber == item.CancelAllowanceNumber).FirstOrDefault();

            if (allowance == null)
            {
                throw new Exception(String.Format("折讓證明單號碼不存在:{0}", item.CancelAllowanceNumber));
            }

            if (allowance.InvoiceAllowanceCancellation != null)
            {
                throw new Exception(String.Format("作廢折讓單已存在,折讓單號碼:{0}", item.CancelAllowanceNumber));
            }

            InvoiceAllowanceCancellation cancelItem = new InvoiceAllowanceCancellation
            {
                AllowanceID = allowance.AllowanceID,
                Remark = item.Remark,
                CancelDate = DateTime.ParseExact(String.Format("{0} {1}", item.CancelDate, item.CancelTime), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
            };

            return cancelItem;
        }

        public InvoiceAllowanceCancellation ConvertToAllowanceCancellation(OrganizationToken owner, Model.Schema.TurnKey.B0501.CancelAllowance item, out Exception ex)
        {
            ex = null; DateTime dt;
            var allowance = this.GetTable<InvoiceAllowance>().Where(i => i.AllowanceNumber == item.CancelAllowanceNumber).FirstOrDefault();

            if (allowance == null)
            {
                ex = new Exception(String.Format("折讓證明單號碼不存在:{0}", item.CancelAllowanceNumber));
                return null;
            }
            if (string.IsNullOrEmpty(item.AllowanceDate))
            {
                ex = new Exception(String.Format("折讓證明單日期不得為空白"));
                return null;
            }
            if (!DateTime.TryParse(item.AllowanceDate, out dt))
            {
                ex = new Exception(String.Format("折讓證明單日期格式有誤"));
                return null;
            }
            //發票開立人
            if (string.IsNullOrEmpty(item.SellerId))
            {
                ex = new Exception(String.Format("發票開立人不得為空白"));
                return null;
            }
            var seller = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.SellerId).FirstOrDefault();
            if (seller == null)
            {
                ex = new Exception(String.Format("發票開立人為非註冊會員,統一編號:{0}", item.SellerId ));
                return null;
            }
           
           
            //發票開立人
            if (string.IsNullOrEmpty(item.BuyerId ))
            {
                ex = new Exception(String.Format("發票買受人不得為空白"));
                return null;
            }
            var buyer = this.GetTable<Organization>().Where(o => o.ReceiptNo == item.BuyerId).FirstOrDefault();
            if (buyer == null)
            {
                ex = new Exception(String.Format("發票買受人為非註冊會員,統一編號:{0}", item.BuyerId));
                return null;
            }
            
            if (string.IsNullOrEmpty(item.CancelDate))
            {
                ex = new Exception(String.Format("作廢折讓單日期不得為空白"));
                return null;
            }
            if (!DateTime.TryParse(item.CancelDate, out dt))
            {
                ex = new Exception(String.Format("作廢折讓單日期格式有誤"));
                return null;
            }
            if (DateTime.Parse(item.CancelDate) < DateTime.Parse(item.AllowanceDate))
            {
                ex = new Exception(String.Format("作廢折讓單日期不可小於折讓日期"));
                return null;
            }
            //if (string.IsNullOrEmpty(item.CancelTime))
            //{
            //    ex = new Exception(String.Format("作廢折讓單時間不得為空白"));
            //    return null;
            //}
            if (!DateTime.TryParse(item.CancelDate + " " + item.CancelTime, out dt))
            {
                ex = new Exception(String.Format("作廢折讓單時間格式有誤"));
                return null;
            }
            if (allowance.AllowanceDate.Value != DateTime.Parse(item.AllowanceDate))
            {
                ex = new Exception(String.Format("折讓證明單日期不合,折讓單日期:{0}", item.AllowanceDate));
                return null;
            }
            if (allowance.InvoiceAllowanceCancellation != null)
            {
                ex = new Exception(String.Format("作廢折讓單已存在,折讓單號碼:{0}", item.CancelAllowanceNumber));
                return null;
            }
            if (string.IsNullOrEmpty(item.CancelReason))
            {
                ex = new Exception(String.Format("作廢折讓單原因不得為空白"));
                return null;
            }

            InvoiceAllowanceCancellation cancelItem = new InvoiceAllowanceCancellation
            {
                AllowanceID = allowance.AllowanceID,
                Remark = item.Remark,
                CancelDate = DateTime.ParseExact(String.Format("{0} {1}", item.CancelDate, item.CancelTime), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
            };

            return cancelItem;
        }

        public void SaveB0201(Schema.TurnKey.B0201.CancelAllowance item, OrganizationToken owner)
        {

            InvoiceAllowanceCancellation cancelItem = ConvertToAllowanceCancellation(owner, item);
            this.GetTable<InvoiceAllowanceCancellation>().InsertOnSubmit(cancelItem);

            var doc = new DerivedDocument
            {
                CDS_Document = new CDS_Document
                {
                    DocType = (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation,
                    DocDate = DateTime.Now,
                    CurrentStep = (int)Naming.B2BInvoiceStepDefinition.待傳送,
                    DocumentOwner = new DocumentOwner
                    {
                        OwnerID = owner.CompanyID
                    }
                },
                SourceID = cancelItem.AllowanceID
            };

            this.GetTable<DerivedDocument>().InsertOnSubmit(doc);

            this.SubmitChanges();
        }
    }
}
