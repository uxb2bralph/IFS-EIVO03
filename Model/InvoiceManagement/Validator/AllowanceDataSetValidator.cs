﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Schema.EIVO;
using System.Text.RegularExpressions;
using Model.Resource;
using System.Globalization;
using Model.Locale;
using Model.InvoiceManagement.ErrorHandle;
using System.Data;
using Utility;

namespace Model.InvoiceManagement.Validator
{
    public partial class AllowanceDataSetValidator : AllowanceRootAllowanceValidator
    {
        protected new DataRow _allowanceItem;
        protected IEnumerable<DataRow> _details;

        public AllowanceDataSetValidator(GenericManager<EIVOEntityDataContext> mgr, Organization owner) : base(mgr, owner)
        {

        }

        public Organization ExpectedSeller { get; set; }

        internal AllowanceFieldIndex AllowanceField = new AllowanceFieldIndex { };
        internal class AllowanceFieldIndex
        {
            public int Allowance_No { get; set; } = 0;
            public int Allowance_Date { get; set; } = 1;
            public int Seller_ID { get; set; } = 2;
            public int Customer_ID { get; set; } = 3;
            public int Buyer_Name { get; set; } = 4;
            public int Buyer_ID { get; set; } = 5;
            public int Allowance_Type { get; set; } = 6;
            public int Contact_Name { get; set; } = 7;
            public int EMail { get; set; } = 8;
            public int Address { get; set; } = 9;
            public int Phone { get; set; } = 10;
            public int Tax_Amount { get; set; } = 11;
            public int Total_Amount { get; set; } = 12;
            public int Currency { get; set; } = 13;
        }

        internal DetailsFieldIndex DetailsField = new DetailsFieldIndex { };
        internal class DetailsFieldIndex
        {
            public int Allowance_No { get; set; } = 0;
            public int Original_Invoice_Date { get; set; } = 1;
            public int Original_Invoice_No { get; set; } = 2;
            public int Original_Sequence_No { get; set; } = 3;
            public int Original_Description { get; set; } = 4;
            public int Quantity { get; set; } = 5;
            public int Unit_Price { get; set; } = 6;
            public int Amount { get; set; } = 7;
            public int Tax { get; set; } = 8;
            public int Allowance_Sequence_No { get; set; } = 9;
            public int Item_Tax_Type { get; set; } = 10;
        }

        public virtual Exception Validate(DataRow dataItem, IEnumerable<DataRow> details)
        {
            _allowanceItem = dataItem;
            _details = details;

            Exception ex;

            _seller = null;
            _newItem = null;

            if ((ex = CheckBusiness()) != null)
            {
                return ex;
            }

            if ((ex = CheckMandatoryFields()) != null)
            {
                return ex;
            }

            if ((ex = CheckAllowanceItem()) != null)
            {
                return ex;
            }

            return null;
        }

        protected override Exception CheckBusiness()
        {
            _seller = models.GetTable<Organization>().Where(o => o.ReceiptNo == _allowanceItem.GetString(AllowanceField.Seller_ID)).FirstOrDefault();

            if (_seller == null)
            {
                return new Exception(String.Format(MessageResources.AlertInvalidSeller, _allowanceItem.GetString(AllowanceField.Seller_ID)));
            }

            ExpectedSeller = _seller;

            if (_seller.CompanyID != _owner.CompanyID && !models.GetTable<InvoiceIssuerAgent>().Any(a => a.AgentID == _owner.CompanyID && a.IssuerID == _seller.CompanyID))
            {
                return new Exception(String.Format(MessageResources.AlertSellerSignature, _allowanceItem.GetString(AllowanceField.Seller_ID)));
            }

            //if (_allowanceItem.SellerName == null || _allowanceItem.SellerName.Length > 60)
            //{
            //    return new Exception(String.Format(MessageResources.AlertSellerNameLength, _allowanceItem.SellerName));
            //}

            //if (_allowanceItem.GetString(AllowanceField.Buyer_ID) == "0000000000")
            //{
            //    //if (_allowanceItem.GetString(AllowanceField.Buyer_Name) == null || Encoding.GetEncoding(950).GetBytes(_allowanceItem.GetString(AllowanceField.Buyer_Name)).Length != 4)
            //    //{
            //    //    return new Exception(String.Format(MessageResources.InvalidBuyerName, _allowanceItem.GetString(AllowanceField.Buyer_Name)));
            //    //}
            //}
            //else if (_allowanceItem.GetString(AllowanceField.Buyer_ID) == null || !Regex.IsMatch(_allowanceItem.GetString(AllowanceField.Buyer_ID), "^[0-9]{8}$"))
            //{
            //    return new Exception(String.Format(MessageResources.InvalidBuyerId, _allowanceItem.GetString(AllowanceField.Buyer_ID)));
            //}
            //else if (_allowanceItem.GetString(AllowanceField.Buyer_Name) != null && _allowanceItem.GetString(AllowanceField.Buyer_Name).Length > 60)
            //{
            //    return new Exception(String.Format(MessageResources.AlertBuyerNameLength, _allowanceItem.GetString(AllowanceField.Buyer_Name)));
            //}

            return null;
        }

        DateTime? allowanceDate;
        protected override Exception CheckMandatoryFields()
        {
            _allowanceDate = DateTime.Today;

            if (String.IsNullOrEmpty(_allowanceItem.GetString(AllowanceField.Allowance_No)))
            {
                return new Exception(MessageResources.InvalidAllowanceNo);
            }

            //折讓證明單號碼
            if (_allowanceItem.GetString(AllowanceField.Allowance_No).Length > 16)
            {
                return new Exception(String.Format(MessageResources.AlertAllowanceNoLength, _allowanceItem.GetString(AllowanceField.Allowance_No)));
            }

            var currentItem = models.GetTable<InvoiceAllowance>().Where(a => a.AllowanceNumber == _allowanceItem.GetString(AllowanceField.Allowance_No))
                        .Join(models.GetTable<InvoiceAllowanceSeller>().Where(s => s.SellerID == _seller.CompanyID),
                            a => a.AllowanceID, s => s.AllowanceID, (a, s) => a).FirstOrDefault();
            if (currentItem != null)
            {
                return new DuplicateAllowanceNumberException(String.Format(MessageResources.AlertAllowanceDuplicated, _allowanceItem.GetString(AllowanceField.Allowance_No))) { CurrentAllowance = currentItem };
            }

            allowanceDate = _allowanceItem.GetData<DateTime>(AllowanceField.Allowance_Date);
            //折讓證明單日期
            if (!allowanceDate.HasValue)
            {
                return new Exception(MessageResources.InvalidAllowanceDate);
            }

            _currency = null;
            if (!String.IsNullOrEmpty(_allowanceItem.GetString(AllowanceField.Currency)))
            {
                _currency = models.GetTable<CurrencyType>().Where(c => c.AbbrevName == _allowanceItem.GetString(AllowanceField.Currency)).FirstOrDefault();
                if (_currency == null)
                {
                    return new Exception($"Invalid currency code：{_allowanceItem.GetString(AllowanceField.Currency)}，TAG：<Currency/>");
                }
            }

            return null;
        }

        InvoiceAllowanceBuyer _allowanceBuyer = null;
        protected override Exception CheckAllowanceItem()
        {
            if (_details == null || _details.Count() == 0)
            {
                return new Exception("Allowance details not found.");
            }

            byte? allowanceType = _allowanceItem.GetData<byte>(AllowanceField.Allowance_Type);
            //課稅別
            if (!allowanceType.HasValue || !Enum.IsDefined(typeof(Naming.AllowanceTypeDefinition), (int)allowanceType.Value))
            {
                allowanceType = (byte)Naming.AllowanceTypeDefinition.賣方開立;
            }

            _productItems = new List<InvoiceAllowanceItem>();
            var invTable = models.GetTable<InvoiceItem>();

            foreach (var i in _details)
            {
                InvoiceItem originalInvoice = null;

                if (i.GetString(DetailsField.Original_Invoice_No) != null && i.GetString(DetailsField.Original_Invoice_No).Length == 10)
                {
                    String invNo, trackCode;
                    trackCode = i.GetString(DetailsField.Original_Invoice_No).Substring(0, 2);
                    invNo = i.GetString(DetailsField.Original_Invoice_No).Substring(2);
                    originalInvoice = invTable.Where(n => n.TrackCode == trackCode && n.No == invNo).FirstOrDefault();
                }

                if (originalInvoice == null)
                {
                    return new Exception(String.Format(MessageResources.InvalidAllowance_NoInvoiceData, i.GetString(DetailsField.Original_Invoice_No)));
                }

                if (originalInvoice.InvoiceCancellation != null)
                {
                    return new Exception(MessageResources.InvalidAllowance_InvoiceHasBeenCanceled);
                }


                DateTime? originalInvoiceDate = i.GetData<DateTime>(DetailsField.Original_Invoice_Date);
                if (!originalInvoiceDate.HasValue || originalInvoiceDate.Value.Date != originalInvoice.InvoiceDate.Value.Date)
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_InvoiceDate, originalInvoice.InvoiceDate, originalInvoiceDate));
                }

                if (originalInvoice.InvoiceSeller.ReceiptNo != _seller.ReceiptNo)
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_InvoiceSellerIsDifferent, i.GetString(DetailsField.Original_Invoice_No)));
                }

                short? originalSeqNo = i.GetData<short>(DetailsField.Original_Sequence_No);
                //原明細排列序號
                if (originalSeqNo > 1000 || originalSeqNo < 0)
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_OriginalSequenceNumber, originalSeqNo));
                }

                short? allowanceSeqNo = i.GetData<short>(DetailsField.Allowance_Sequence_No) ?? 1;
                //折讓證明單明細排列序號
                if (allowanceSeqNo > 1000 || allowanceSeqNo < 0)
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_AllowanceSequenceNumber, allowanceSeqNo));
                }

                //原品名
                if (i.GetString(DetailsField.Original_Description) == null || i.GetString(DetailsField.Original_Description).Length > 256)
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_OriginalDescription, i.GetString(DetailsField.Original_Description)));
                }

                byte? taxType = i.GetData<byte>(DetailsField.Item_Tax_Type);
                //課稅別
                if (!taxType.HasValue || !Enum.IsDefined(typeof(Naming.TaxTypeDefinition), (int)taxType.Value))
                {
                    return new Exception(String.Format(MessageResources.AlertAllowance_TaxType, taxType));
                }

                var allowanceItem = new InvoiceAllowanceItem
                {
                    Amount = i.GetData<decimal>(DetailsField.Amount),
                    InvoiceNo = i.GetString(DetailsField.Original_Invoice_No),
                    InvoiceDate = originalInvoiceDate,
                    OriginalSequenceNo = originalSeqNo,
                    Piece = i.GetData<decimal>(DetailsField.Quantity),
                    OriginalDescription = i.GetString(DetailsField.Original_Description),
                    TaxType = taxType,
                    No = allowanceSeqNo,
                    UnitCost = i.GetData<decimal>(DetailsField.Unit_Price),
                    Tax = i.GetData<decimal>(DetailsField.Tax),
                };

                if (originalInvoice != null)
                {
                    var invProductItem = models.GetTable<InvoiceItem>().Where(v => v.InvoiceID == originalInvoice.InvoiceID)
                        .Join(models.GetTable<InvoiceDetail>(), v => v.InvoiceID, d => d.InvoiceID, (v, d) => d)
                        .Join(models.GetTable<InvoiceProduct>(), d => d.ProductID, p => p.ProductID, (d, p) => p)
                        .Join(models.GetTable<InvoiceProductItem>(), p => p.ProductID, t => t.ProductID, (p, t) => t)
                        .Where(t => t.No == originalSeqNo).FirstOrDefault();
                    //var invProductItem = originalInvoice.InvoiceDetails.Join(_mgr.GetTable<InvoiceProductItem>(), d => d.ProductID, p => p.ProductID, (d, p) => p)
                    //    .Where(p => p.No == i.OriginalSequenceNumber).FirstOrDefault();
                    if (invProductItem != null)
                    {
                        allowanceItem.ItemID = invProductItem.ItemID;
                    }
                }

                if(_allowanceBuyer==null)
                {
                    _allowanceBuyer = new InvoiceAllowanceBuyer
                    {
                        Name = originalInvoice.InvoiceBuyer.Name,
                        ReceiptNo = originalInvoice.InvoiceBuyer.ReceiptNo,
                        CustomerID = originalInvoice.InvoiceBuyer.CustomerID,
                        ContactName = originalInvoice.InvoiceBuyer.ContactName,
                        CustomerName = originalInvoice.InvoiceBuyer.CustomerName,
                    };
                }
                _productItems.Add(allowanceItem);
            }

            _newItem = new InvoiceAllowance()
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Allowance
                },
                AllowanceDate = _allowanceDate,
                AllowanceNumber = _allowanceItem.GetString(AllowanceField.Allowance_No),
                AllowanceType = allowanceType,
                BuyerId = _allowanceBuyer.ReceiptNo,
                SellerId = _seller.ReceiptNo,
                TaxAmount = _allowanceItem.GetData<decimal>(AllowanceField.Tax_Amount),
                TotalAmount = _allowanceItem.GetData<decimal>(AllowanceField.Total_Amount),
                CurrencyID = _currency?.CurrencyID,
                //InvoiceID =  invTable.Where(i=>i.TrackCode + i.No == item.AllowanceItem.Select(a=>a.GetString(DetailsField.Original_Invoice_No)).FirstOrDefault()).Select(i=>i.InvoiceID).FirstOrDefault(),
                InvoiceAllowanceBuyer = _allowanceBuyer,
                InvoiceAllowanceSeller = new InvoiceAllowanceSeller
                {
                    Name = _seller.CompanyName,
                    ReceiptNo = _seller.ReceiptNo,
                    Address = _seller.Addr,
                    ContactName = _seller.ContactName,
                    CustomerName = _seller.CompanyName,
                    EMail = _seller.ContactEmail,
                    Fax = _seller.Fax,
                    Phone = _seller.Phone,
                    PersonInCharge = _seller.UndertakerName,
                    SellerID = _seller.CompanyID,
                }
            };

            _newItem.InvoiceAllowanceBuyer.Address = _allowanceItem.GetString(AllowanceField.Address);
            _newItem.InvoiceAllowanceBuyer.EMail = _allowanceItem.GetString(AllowanceField.EMail);
            _newItem.InvoiceAllowanceBuyer.ContactName = _allowanceItem.GetString(AllowanceField.Contact_Name);
            _newItem.InvoiceAllowanceBuyer.Phone = _allowanceItem.GetString(AllowanceField.Phone);

            _newItem.InvoiceAllowanceDetails.AddRange(_productItems.Select(p => new InvoiceAllowanceDetail
            {
                InvoiceAllowanceItem = p,
            }));

            if (_owner != null)
            {
                _newItem.CDS_Document.DocumentOwner = new DocumentOwner
                {
                    OwnerID = _owner.CompanyID,
                };
            }

            return null;
        }
    }
}
