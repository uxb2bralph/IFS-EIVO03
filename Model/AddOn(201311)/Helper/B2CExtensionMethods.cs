using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Schema.EIVO;
using Model.DataEntity;
using Utility;
namespace Model.Helper
{
    public static class B2CExtensionMethods
    {
        public static decimal ToFix(this decimal decVal,int decimals = 2)
        {
            return Math.Round(decVal, decimals);
        }

        public static Model.Schema.TurnKey.C0401.Invoice CreateC0401(this InvoiceItem item)
        {
            var result = new Model.Schema.TurnKey.C0401.Invoice
            {
                Main = new Schema.TurnKey.C0401.Main
                {
                    Buyer = new Schema.TurnKey.C0401.MainBuyer
                    {
                        //選擇性欄位不提供給大平台
                        //Address = item.InvoiceBuyer.Address.GetEfficientStringMaxSize(0,100).InsteadOfNullOrEmpty(""),
                        //CustomerNumber = item.InvoiceBuyer.CustomerNumber.GetEfficientStringMaxSize(0, 20).InsteadOfNullOrEmpty(""),
                        //EmailAddress = item.InvoiceBuyer.EMail.GetEfficientStringMaxSize(0, 80).InsteadOfNullOrEmpty(""),
                        //FacsimileNumber = item.InvoiceBuyer.Fax.GetEfficientStringMaxSize(0, 26).InsteadOfNullOrEmpty(""),
                        Identifier = item.InvoiceBuyer.ReceiptNo,
                        Name = item.InvoiceBuyer.IsB2C()
                            ? Encoding.GetEncoding(950).GetBytes(item.InvoiceBuyer.Name.InsteadOfNullOrEmpty("")).Length == 4
                                ? item.InvoiceBuyer.Name : ValueValidity.GenerateRandomCode(4)
                            : String.IsNullOrEmpty(item.InvoiceBuyer.Name)
                                ? item.InvoiceBuyer.ReceiptNo : item.InvoiceBuyer.Name,
                        //PersonInCharge = item.InvoiceBuyer.PersonInCharge.GetEfficientStringMaxSize(0, 30).InsteadOfNullOrEmpty(""),
                        //RoleRemark = item.InvoiceBuyer.RoleRemark.GetEfficientStringMaxSize(0, 40).InsteadOfNullOrEmpty(""),
                        //TelephoneNumber = item.InvoiceBuyer.Phone.GetEfficientStringMaxSize(0, 26).InsteadOfNullOrEmpty("")
                    },
                    BuyerRemark = (Model.Schema.TurnKey.C0401.BuyerRemarkEnum?)item.BuyerRemark,
                    BuyerRemarkSpecified = item.BuyerRemark.HasValue,
                    Category = item.Category,
                    CheckNumber = item.CheckNo,
                    CustomsClearanceMark = (Model.Schema.TurnKey.C0401.CustomsClearanceMarkEnum?)item.CustomsClearanceMark,
                    CustomsClearanceMarkSpecified = item.CustomsClearanceMark.HasValue,
                    InvoiceType = (Schema.TurnKey.C0401.InvoiceTypeEnum)((int)item.InvoiceType),
                    //DonateMark = (Schema.TurnKey.C0401.DonateMarkEnum)(int.Parse(item.DonateMark)),
                    DonateMark = string.IsNullOrEmpty(item.DonateMark) ? Model.Schema.TurnKey.C0401.DonateMarkEnum.Item0 : (Schema.TurnKey.C0401.DonateMarkEnum)(int.Parse(item.DonateMark)),
                    CarrierType = item.InvoiceCarrier != null ? item.InvoiceCarrier.CarrierType : "",
                    //CarrierTypeSpecified = item.InvoiceCarrier != null ? true : false,
                    CarrierId1 = item.InvoiceCarrier != null ? item.InvoiceCarrier.CarrierNo : "",
                    CarrierId2 = item.InvoiceCarrier != null ? item.InvoiceCarrier.CarrierNo2 : "",
                    //PrintMark = item.CDS_Document.DocumentPrintLogs.Any(l => l.TypeID == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice) ? "Y"  : "N"
                    PrintMark = item.PrintMark,
                    NPOBAN = item.InvoiceDonation != null ? item.InvoiceDonation.AgencyCode : "",
                    RandomNumber = item.RandomNo,
                    GroupMark = item.GroupMark,
                    InvoiceDate = String.Format("{0:yyyyMMdd}", item.InvoiceDate.Value),
                    InvoiceTime = item.InvoiceDate.Value,
                    //InvoiceTimeSpecified = false,
                    InvoiceNumber = String.Format("{0}{1}", item.TrackCode, item.No),
                    MainRemark = item.Remark,
                    //PermitNumber = item.PermitNumber,
                    //PermitDate = item.PermitDate.HasValue ? String.Format("{0:yyyyMMdd}", item.PermitDate.Value) : null,
                    //PermitWord = item.PermitWord,
                    RelateNumber = item.RelateNumber,
                    //TaxCenter = item.TaxCenter,
                    Seller = new Schema.TurnKey.C0401.MainSeller
                    {
                        //選擇性欄位不提供給大平台
                        //Address = item.InvoiceSeller.Address.GetEfficientStringMaxSize(0, 100).InsteadOfNullOrEmpty(""),
                        //CustomerNumber = item.InvoiceSeller.CustomerID.GetEfficientStringMaxSize(0, 20).InsteadOfNullOrEmpty(""),
                        //EmailAddress = item.InvoiceSeller.EMail.GetEfficientStringMaxSize(0, 80).InsteadOfNullOrEmpty(""),
                        //FacsimileNumber = item.InvoiceSeller.Fax.GetEfficientStringMaxSize(0, 26).InsteadOfNullOrEmpty(""),
                        Identifier = item.InvoiceSeller.ReceiptNo,
                        Name = item.InvoiceSeller.CustomerName.GetEfficientStringMaxSize(0, 60).InsteadOfNullOrEmpty(""),
                        //PersonInCharge = item.Organization.UndertakerName.GetEfficientStringMaxSize(0, 30).InsteadOfNullOrEmpty(""),
                        //RoleRemark = item.InvoiceSeller.RoleRemark.GetEfficientStringMaxSize(0, 40).InsteadOfNullOrEmpty(""),
                        //TelephoneNumber = item.InvoiceSeller.Phone.GetEfficientStringMaxSize(0, 26).InsteadOfNullOrEmpty("")
                    }
                },
                Details = buildC0401Details(item),
                Amount = new Schema.TurnKey.C0401.Amount
                {
                    CurrencySpecified = false,
                    DiscountAmount = item.InvoiceAmountType.DiscountAmount.HasValue ? item.InvoiceAmountType.DiscountAmount.Value.ToFix() : 0,
                    DiscountAmountSpecified = item.InvoiceAmountType.DiscountAmount.HasValue,
                    ExchangeRateSpecified = false,
                    OriginalCurrencyAmountSpecified = false,
                    //SalesAmount = item.InvoiceAmountType.SalesAmount.HasValue ? item.InvoiceAmountType.SalesAmount.Value.ToFix() : 0,
                    SalesAmount = item.InvoiceBuyer.ReceiptNo == "0000000000" ? item.InvoiceAmountType.TotalAmount.Value.ToFix() : item.InvoiceAmountType.SalesAmount.Value.ToFix(),
                    FreeTaxSalesAmount = (item.InvoiceAmountType.TaxType == 3) ? item.InvoiceAmountType.SalesAmount.Value.ToFix() : 0,
                    ZeroTaxSalesAmount = (item.InvoiceAmountType.TaxType == 2) ? item.InvoiceAmountType.SalesAmount.Value.ToFix() : 0,
                    TaxAmount = item.InvoiceBuyer.ReceiptNo == "0000000000" ? 0 : item.InvoiceAmountType.TaxAmount.HasValue ? item.InvoiceAmountType.TaxAmount.Value.ToFix() : 0,
                    TaxRate = item.InvoiceAmountType.TaxRate.HasValue ? item.InvoiceAmountType.TaxRate.Value.ToFix() : 0.05m,
                    TaxType = (Schema.TurnKey.C0401.TaxTypeEnum)((int)item.InvoiceAmountType.TaxType.Value),
                    TotalAmount = item.InvoiceAmountType.TotalAmount.HasValue ? item.InvoiceAmountType.TotalAmount.Value.ToFix() : 0
                }
            };
            if(item.InvoiceAmountType.CurrencyID.HasValue)
            {
                result.Amount.CurrencySpecified = true;
                result.Amount.Currency = (Schema.TurnKey.C0401.CurrencyCodeEnum)Enum.Parse(typeof(Schema.TurnKey.C0401.CurrencyCodeEnum), item.InvoiceAmountType.CurrencyType.AbbrevName);
            }
            return result;
        }

        private static Schema.TurnKey.C0401.DetailsProductItem[] buildC0401Details(InvoiceItem item)
        {
            List<Model.Schema.TurnKey.C0401.DetailsProductItem> items = new List<Schema.TurnKey.C0401.DetailsProductItem>();
            foreach (var detailItem in item.InvoiceDetails)
            {
                detailItem.InvoiceProduct.InvoiceProductItem.ToList();
                foreach (var productItem in detailItem.InvoiceProduct.InvoiceProductItem)
                {
                    items.Add(new Model.Schema.TurnKey.C0401.DetailsProductItem
                    {
                        Amount = productItem.CostAmount.HasValue ? productItem.CostAmount.Value.ToFix() : 0m,
                        Description = detailItem.InvoiceProduct.Brief,
                        Quantity = productItem.Piece.HasValue ? productItem.Piece.Value.ToFix(0) : 0,
                        RelateNumber = productItem.RelateNumber,
                        Remark = productItem.Remark,
                        SequenceNumber = String.Format("{0:00}", productItem.No),
                        Unit = productItem.PieceUnit,
                        UnitPrice = productItem.UnitCost.HasValue ? productItem.UnitCost.Value.ToFix() : 0,
                    });
                }
            }
            return items.ToArray();
        }

        public static Model.Schema.TurnKey.C0501.CancelInvoice CreateC0501(this InvoiceItem item)
        {
            InvoiceCancellation InvCancel = item.InvoiceCancellation;
            if (InvCancel == null)
            {
                return null;
            }

            var result = new Model.Schema.TurnKey.C0501.CancelInvoice
            {
                CancelInvoiceNumber = InvCancel.CancellationNo,
                InvoiceDate = String.Format("{0:yyyyMMdd}", item.InvoiceDate.Value),
                BuyerId = item.InvoiceBuyer.ReceiptNo,
                SellerId = item.Organization.ReceiptNo,
                CancelDate = String.Format("{0:yyyyMMdd}", InvCancel.CancelDate.Value),
                CancelTime = InvCancel.CancelDate.Value,
                CancelReason = InvCancel.CancelReason,
                ReturnTaxDocumentNumber = InvCancel.ReturnTaxDocumentNo,
                Remark = InvCancel.Remark,
            };
            return result;
        }

        public static Model.Schema.TurnKey.D0401.Allowance CreateD0401(this InvoiceAllowance item)
        {
            var result = new Schema.TurnKey.D0401.Allowance
            {
                Main = new Schema.TurnKey.D0401.Main
                {
                    AllowanceNumber = item.AllowanceNumber.Length > 16 ? item.AllowanceNumber.Substring(0, 16) : item.AllowanceNumber,
                    AllowanceDate = String.Format("{0:yyyyMMdd}", item.AllowanceDate),
                    AllowanceType = (Schema.TurnKey.D0401.AllowanceTypeEnum)((int)item.AllowanceType),
                    Buyer = new Schema.TurnKey.D0401.MainBuyer
                    {
                        Address = string.IsNullOrEmpty(item.InvoiceAllowanceBuyer.Address) ?
                        "" :
                         item.InvoiceAllowanceBuyer.Address.Length > 100 ?
                          item.InvoiceAllowanceBuyer.Address.Substring(0, 100) :
                           item.InvoiceAllowanceBuyer.Address,
                        //CustomerNumber = item.InvoiceAllowanceBuyer.CustomerName,
                        EmailAddress = "",//item.InvoiceAllowanceBuyer.EMail,
                        FacsimileNumber = String.IsNullOrEmpty(item.InvoiceAllowanceBuyer.Fax) ?
                        "" :
                         item.InvoiceAllowanceBuyer.Fax.Length > 26 ?
                          item.InvoiceAllowanceBuyer.Fax.Substring(0, 26) :
                           item.InvoiceAllowanceBuyer.Fax,
                        Identifier = item.InvoiceAllowanceBuyer.ReceiptNo,
                        Name = item.InvoiceAllowanceBuyer.IsB2C()
                            ? Encoding.GetEncoding(950).GetBytes(item.InvoiceAllowanceBuyer.Name.InsteadOfNullOrEmpty("")).Length == 4
                                ? item.InvoiceAllowanceBuyer.Name : ValueValidity.GenerateRandomCode(4)
                            : String.IsNullOrEmpty(item.InvoiceAllowanceBuyer.Name)
                                ? item.InvoiceAllowanceBuyer.ReceiptNo : item.InvoiceAllowanceBuyer.Name,
                        PersonInCharge = String.IsNullOrEmpty(item.InvoiceAllowanceBuyer.PersonInCharge) ?
                        "" :
                        item.InvoiceAllowanceBuyer.PersonInCharge.Length > 30 ?
                        item.InvoiceAllowanceBuyer.PersonInCharge.Substring(0, 30) :
                        item.InvoiceAllowanceBuyer.PersonInCharge,
                        RoleRemark = item.InvoiceAllowanceBuyer.RoleRemark,
                        TelephoneNumber = "",//item.InvoiceAllowanceBuyer.Phone,
                    },
                    Seller = new Schema.TurnKey.D0401.MainSeller
                    {
                        Address = String.IsNullOrEmpty(item.InvoiceAllowanceSeller.Address) ?
                        "" :
                         item.InvoiceAllowanceSeller.Address.Length > 100 ?
                          item.InvoiceAllowanceSeller.Address.Substring(0, 100) :
                           item.InvoiceAllowanceSeller.Address,
                        //CustomerNumber = item.InvoiceAllowanceSeller.CustomerName,
                        EmailAddress = "",//item.InvoiceAllowanceSeller.EMail,
                        FacsimileNumber = String.IsNullOrEmpty(item.InvoiceAllowanceSeller.Fax) ?
                        "" :
                        item.InvoiceAllowanceSeller.Fax.Length > 26 ?
                        item.InvoiceAllowanceSeller.Fax.Substring(0, 26) :
                        item.InvoiceAllowanceSeller.Fax,
                        Identifier = item.InvoiceAllowanceSeller.ReceiptNo,
                        Name = item.InvoiceAllowanceSeller.Name,
                        PersonInCharge = String.IsNullOrEmpty(item.InvoiceAllowanceSeller.PersonInCharge) ?
                        "" :
                         item.InvoiceAllowanceSeller.PersonInCharge.Length > 30 ?
                          item.InvoiceAllowanceSeller.PersonInCharge.Substring(0, 30) :
                           item.InvoiceAllowanceSeller.PersonInCharge,
                        RoleRemark = item.InvoiceAllowanceSeller.RoleRemark,
                        TelephoneNumber = "",//item.InvoiceAllowanceSeller.Phone,
                    },
                },
                Amount = new Schema.TurnKey.D0401.Amount
                {
                    TaxAmount = item.TaxAmount.HasValue ? item.TaxAmount.Value.ToFix() : 0,
                    TotalAmount = item.TotalAmount.HasValue ? item.TotalAmount.Value.ToFix() : 0,
                },
            };

            int seqNo = 1;
            result.Details = item.InvoiceAllowanceDetails.Select(d => new Schema.TurnKey.D0401.DetailsProductItem
            {
                AllowanceSequenceNumber = (seqNo++).ToString(), //d.InvoiceAllowanceItem.No.ToString(),
                Amount = d.InvoiceAllowanceItem.Amount.HasValue ? d.InvoiceAllowanceItem.Amount.Value.ToFix() : 0m,
                OriginalDescription = d.InvoiceAllowanceItem.OriginalDescription,
                OriginalInvoiceDate = String.Format("{0:yyyyMMdd}", d.InvoiceAllowanceItem.InvoiceDate),
                OriginalInvoiceNumber = d.InvoiceAllowanceItem.InvoiceNo,
                OriginalSequenceNumber = d.InvoiceAllowanceItem.OriginalSequenceNo.HasValue ? d.InvoiceAllowanceItem.OriginalSequenceNo.Value.ToString() : "",
                Quantity = d.InvoiceAllowanceItem.Piece ?? 0.00000M,
                Tax = (long)d.InvoiceAllowanceItem.Tax.Value,
                TaxType = (Schema.TurnKey.D0401.TaxTypeEnum)(int)d.InvoiceAllowanceItem.TaxType,
                Unit = d.InvoiceAllowanceItem.PieceUnit,
                UnitPrice = d.InvoiceAllowanceItem.UnitCost ?? 0,
            }).ToArray();

            return result;
        }

        public static Model.Schema.TurnKey.D0501.CancelAllowance CreateD0501(this InvoiceAllowance item)
        {
            InvoiceAllowanceCancellation cancelledItem = item.InvoiceAllowanceCancellation;
            if (cancelledItem == null)
            {
                return null;
            }

            var result = new Model.Schema.TurnKey.D0501.CancelAllowance
            {
                CancelAllowanceNumber = item.AllowanceNumber,
                AllowanceDate = String.Format("{0:yyyyMMdd}",item.AllowanceDate.Value),
                BuyerId = item.BuyerId,
                SellerId = item.SellerId,
                CancelDate = String.Format("{0:yyyyMMdd}",cancelledItem.CancelDate.Value),
                CancelTime = cancelledItem.CancelDate.Value,
                CancelReason = cancelledItem.CancelReason,
                Remark = cancelledItem.Remark,
            };
            return result;
        }

        public static Model.Schema.TurnKey.E0402.BranchTrackBlank BuildE0402(this InvoiceTrackCodeAssignment item)
        {
            var result = new Model.Schema.TurnKey.E0402.BranchTrackBlank
            {
                Main = new Schema.TurnKey.E0402.Main
                {
                    HeadBan = item.Organization.ReceiptNo,
                    BranchBan = item.Organization.ReceiptNo,
                    InvoiceType = (Schema.TurnKey.E0402.InvoiceTypeEnum)item.Organization.OrganizationStatus.SettingInvoiceType.Value,
                    YearMonth = String.Format("{0:000}{1:00}", item.InvoiceTrackCode.Year - 1911, item.InvoiceTrackCode.PeriodNo * 2),
                    InvoiceTrack = item.InvoiceTrackCode.TrackCode
                },
                Details = buildE0402Details(item)
            };
            return result;
        }

        private static Schema.TurnKey.E0402.DetailsBranchTrackBlankItem[] buildE0402Details(InvoiceTrackCodeAssignment item)
        {
            List<Model.Schema.TurnKey.E0402.DetailsBranchTrackBlankItem> items = new List<Schema.TurnKey.E0402.DetailsBranchTrackBlankItem>();

            foreach (var detail in item.UnassignedInvoiceNo)
            {
                items.Add(new Model.Schema.TurnKey.E0402.DetailsBranchTrackBlankItem
                    {
                        InvoiceBeginNo = String.Format("{0:00000000}", detail.InvoiceBeginNo),
                        InvoiceEndNo = String.Format("{0:00000000}", detail.InvoiceEndNo)
                    }
                );
            }
            return items.ToArray();
        }

        public static Model.Schema.TurnKey.C0701.VoidInvoice CreateC0701(this InvoiceItem item)
        {
            return new Model.Schema.TurnKey.C0701.VoidInvoice
            {
                VoidInvoiceNumber = item.TrackCode + item.No,
                InvoiceDate = String.Format("{0:yyyyMMdd}", item.InvoiceDate),
                BuyerId = item.InvoiceBuyer.ReceiptNo,
                SellerId = item.InvoiceSeller.ReceiptNo,
                VoidDate = DateTime.Now.Date.ToString("yyyyMMdd"),
                VoidTime = DateTime.Now,
                VoidReason = "註銷重開",
                Remark = ""
            };
        }

        public static Model.Schema.TurnKey.E0402.BranchTrackBlank BuildE0402(this InvoiceTrackCodeAssignment item, IQueryable<UnassignedInvoiceNoSummary> Summary)
        {
            var result = new Model.Schema.TurnKey.E0402.BranchTrackBlank
            {
                Main = new Schema.TurnKey.E0402.Main
                {
                    HeadBan = item.Organization.ReceiptNo,
                    BranchBan = item.Organization.ReceiptNo,
                    InvoiceType = (Schema.TurnKey.E0402.InvoiceTypeEnum)(item.Organization.InvoiceItems.Where(i => i.InvoiceDate >= Convert.ToDateTime(String.Format("{0}/{1}/1", item.InvoiceTrackCode.Year, item.InvoiceTrackCode.PeriodNo * 2 - 1))).FirstOrDefault().InvoiceType),
                    YearMonth = String.Format("{0:000}{1:00}", item.InvoiceTrackCode.Year - 1911, item.InvoiceTrackCode.PeriodNo * 2),
                    InvoiceTrack = item.InvoiceTrackCode.TrackCode
                },
                Details = buildE0402Details(item, Summary)
            };
            return result;
        }

        private static Schema.TurnKey.E0402.DetailsBranchTrackBlankItem[] buildE0402Details(InvoiceTrackCodeAssignment item, IQueryable<UnassignedInvoiceNoSummary> Summary)
        {
            List<Model.Schema.TurnKey.E0402.DetailsBranchTrackBlankItem> items = new List<Schema.TurnKey.E0402.DetailsBranchTrackBlankItem>();

            foreach (var detail in Summary.ToList())
            {
                items.Add(new Model.Schema.TurnKey.E0402.DetailsBranchTrackBlankItem
                {
                    InvoiceBeginNo = String.Format("{0:00000000}", detail.StartNo),
                    InvoiceEndNo = String.Format("{0:00000000}", detail.EndNo)
                }
                );
            }
            return items.ToArray();
        }
    }
}
