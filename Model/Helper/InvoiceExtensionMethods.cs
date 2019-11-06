using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.InvoiceManagement.InvoiceProcess;
using Model.Locale;
using Uxnet.Com.DataAccessLayer;

namespace Model.Helper
{
    public static class InvoiceExtensionMethods
    {

        public static void GetInvoiceNo(this String invoiceNumber, out String invNo, out String trackCode)
        {
            if (!String.IsNullOrEmpty(invoiceNumber) && invoiceNumber.Length >= 10)
            {
                trackCode = invoiceNumber.Substring(0, 2);
                invNo = invoiceNumber.Substring(2);
            }
            else
            {
                trackCode = null;
                invNo = invoiceNumber;
            }
        }

        public static InvoiceItem ConvertToInvoiceItem(this GenericManager<EIVOEntityDataContext> models,Model.Schema.TurnKey.A0401.Invoice invoice, OrganizationToken owner)
        {
            Organization buyer = models.GetTable<Organization>().Where(o => o.ReceiptNo == invoice.Main.Buyer.Identifier).FirstOrDefault();
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
                    ReceiptNo = invoice.Main.Buyer.Identifier,
                    OrganizationStatus = new OrganizationStatus
                    {
                        IronSteelIndustry = false
                    }
                };
            }

            Organization seller = models.GetTable<Organization>().Where(o => o.ReceiptNo == invoice.Main.Seller.Identifier).FirstOrDefault();
            if (seller == null)
            {
                seller = new Organization
                {
                    Addr = invoice.Main.Seller.Address,
                    CompanyName = invoice.Main.Seller.Name,
                    UndertakerName = invoice.Main.Seller.PersonInCharge,
                    Phone = invoice.Main.Seller.TelephoneNumber,
                    Fax = invoice.Main.Seller.FacsimileNumber,
                    ContactEmail = invoice.Main.Seller.EmailAddress,
                    ReceiptNo = invoice.Main.Seller.Identifier,
                    OrganizationStatus = new OrganizationStatus
                    {
                        IronSteelIndustry = false
                    }
                };
            }
            else if (seller.OrganizationStatus == null)
            {
                seller.OrganizationStatus = new OrganizationStatus
                {
                    IronSteelIndustry = false
                };
            }

            String invNo, trackCode;
            invoice.Main.InvoiceNumber.GetInvoiceNo(out invNo, out trackCode);

            InvoiceItem newItem = new InvoiceItem
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_Invoice,
                },
                InvoiceBuyer = new InvoiceBuyer
                {
                    BuyerMark = invoice.Main.BuyerRemarkSpecified ? (int)invoice.Main.BuyerRemark : (int?)null,
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
                InvoiceDate = DateTime.ParseExact(String.Format("{0}", invoice.Main.InvoiceDate), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).Add(invoice.Main.InvoiceTime.TimeOfDay),
                InvoiceType = (byte)((int)invoice.Main.InvoiceType),
                No = invNo,
                TrackCode = trackCode,
                Organization = seller,
                BuyerRemark = (byte?)invoice.Main.BuyerRemark,
                Category = invoice.Main.Category,
                CheckNo = invoice.Main.CheckNumber,
                DonateMark = ((int)invoice.Main.DonateMark).ToString(),
                CustomsClearanceMark = (byte?)invoice.Main.CustomsClearanceMark,
                GroupMark = invoice.Main.GroupMark,
                //PermitDate = String.IsNullOrEmpty(invoice.Main.PermitDate) ? (DateTime?)null : DateTime.ParseExact(invoice.Main.PermitDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                //PermitNumber = invoice.Main.PermitNumber,
                //PermitWord = invoice.Main.PermitWord,
                RelateNumber = invoice.Main.RelateNumber,
                Remark = invoice.Main.MainRemark,
                //TaxCenter = invoice.Main.TaxCenter,
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
                },
                PrintMark = "Y",
            };

            if (owner != null)
            {
                newItem.CDS_Document.DocumentOwner = new DocumentOwner
                {
                    OwnerID = owner.CompanyID
                };
            }

            short seqNo = 1;

            var productItems = invoice.Details.Select(i => new InvoiceProductItem
            {
                InvoiceProduct = new InvoiceProduct { Brief = i.Description },
                CostAmount = i.Amount,
                ItemNo = i.SequenceNumber,
                Piece = i.Quantity,
                PieceUnit = i.Unit,
                UnitCost = i.UnitPrice,
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

        public static InvoiceItem ConvertToInvoiceItem(this GenericManager<EIVOEntityDataContext> models, InvoiceEntity invoice)
        {
            InvoiceItem item = invoice.MainItem;
            if (item.InvoiceBuyer == null)
            {
                invoice.Status = Naming.UploadStatusDefinition.匯入失敗;
                invoice.Reason = "InvoiceBuyer is null";
                return null;
            }
            Organization buyer = models.GetTable<Organization>().Where(o => o.ReceiptNo == item.InvoiceBuyer.ReceiptNo).FirstOrDefault();
            if (buyer == null)
            {
                buyer = new Organization
                {
                    Addr = item.InvoiceBuyer.Address,
                    CompanyName = item.InvoiceBuyer.Name,
                    UndertakerName = item.InvoiceBuyer.PersonInCharge,
                    Phone = item.InvoiceBuyer.Phone,
                    Fax = item.InvoiceBuyer.Fax,
                    ContactEmail = item.InvoiceBuyer.EMail,
                    ReceiptNo = item.InvoiceBuyer.ReceiptNo,
                    OrganizationStatus = new OrganizationStatus
                    {
                        IronSteelIndustry = false
                    }
                };
            }

            item.InvoiceBuyer.Organization = buyer;

            if (item.InvoiceSeller == null)
            {
                invoice.Status = Naming.UploadStatusDefinition.匯入失敗;
                invoice.Reason = "InvoiceSeller is null";
                return null;
            }

            Organization seller = models.GetTable<Organization>().Where(o => o.ReceiptNo == item.InvoiceSeller.ReceiptNo).FirstOrDefault();
            if (seller == null)
            {
                seller = new Organization
                {
                    Addr = item.InvoiceSeller.Address,
                    CompanyName = item.InvoiceSeller.Name,
                    UndertakerName = item.InvoiceSeller.PersonInCharge,
                    Phone = item.InvoiceSeller.Phone,
                    Fax = item.InvoiceSeller.Fax,
                    ContactEmail = item.InvoiceSeller.EMail,
                    ReceiptNo = item.InvoiceSeller.ReceiptNo,
                    OrganizationStatus = new OrganizationStatus
                    {
                        IronSteelIndustry = false
                    }
                };
            }
            else if (seller.OrganizationStatus == null)
            {
                seller.OrganizationStatus = new OrganizationStatus
                {
                    IronSteelIndustry = false
                };
            }
            item.Organization = seller;

            item.InvoiceSeller.Organization = seller;
            item.CDS_Document = new CDS_Document
            {
                DocDate = DateTime.Now,
                DocType = (int)Naming.DocumentTypeDefinition.E_Invoice,
            };

            //foreach(var p in invoice.ItemDetails)
            //{
            //    p.InvoiceProduct = new InvoiceProduct
            //    {
            //        Brief = p.InvoiceProduct.Brief
            //    };
            //}

            item.InvoiceDetails.Clear();
            item.InvoiceDetails.AddRange(invoice.ItemDetails.Select(p => new InvoiceDetail
            {
                InvoiceProduct = p
            }));


            return item;
        }

        public static DataSet GetInvoiceData(this IQueryable<InvoiceItem> items, GenericManager<EIVOEntityDataContext> models)
        {
            var productItems = items.Join(models.GetTable<InvoiceDetail>(), i => i.InvoiceID, d => d.InvoiceID, (i, d) => d)
                                    .SelectMany(d => d.InvoiceProduct.InvoiceProductItem);

            var dataItems = items.ToArray().Select(i => new
            {
                Invoice_No = $"{i.TrackCode}{i.No}",
                Invoice_Date = i.InvoiceDate,
                Data_ID = i.InvoicePurchaseOrder?.OrderNo,
                Data_Date = i.InvoicePurchaseOrder?.PurchaseDate,
                Seller_ID = i.InvoiceSeller.ReceiptNo,
                Buyer_Name = i.InvoiceBuyer.CustomerName,
                Buyer_ID = i.InvoiceBuyer.ReceiptNo,
                Buyer_Mark = i.BuyerRemark,
                Customs_Clearance_Mark = i.CustomsClearanceMark,
                Customer_ID = i.InvoiceBuyer.CustomerID,
                Contact_Name = i.InvoiceBuyer.ContactName,
                EMail = i.InvoiceBuyer.EMail,
                Address = i.InvoiceBuyer.Address,
                Phone = i.InvoiceBuyer.Phone,
                Sales_Amount = i.InvoiceAmountType.SalesAmount,
                Free_Tax_Sales_Amount = (decimal?)null,
                Zero_Tax_Sales_Amount = (decimal?)null,
                Invoice_Type = (int?)i.InvoiceType,
                Tax_Type = (int?)i.InvoiceAmountType.TaxType,
                Tax_Rate = i.InvoiceAmountType.TaxRate,
                Tax_Amount = i.InvoiceAmountType.TaxAmount,
                Total_Amount = i.InvoiceAmountType.TotalAmount,
                Currency = i.InvoiceAmountType.CurrencyType?.AbbrevName,
                Print_Mark = i.PrintMark,
                Carrier_Type = i.InvoiceCarrier?.CarrierType,
                Carrier_Id1 = i.InvoiceCarrier?.CarrierNo,
                Carrier_Id2 = i.InvoiceCarrier?.CarrierNo2,
                Donate_Mark = i.DonateMark,
                NPOBAN = i.InvoiceDonation?.AgencyCode,
                Random_Number = i.RandomNo,
                Main_Remark = i.Remark,
                //Process_Type = ((Naming.InvoiceProcessType?)i.CDS_Document.ProcessType).ToString(),
            });

            var detailItems = productItems.ToArray().Select(d => new
            {
                Invoice_No = $"{d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.TrackCode}{d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.No}",
                Data_ID = d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoicePurchaseOrder?.OrderNo,
                Description = d.InvoiceProduct.Brief,
                Quantity = d.Piece,
                Unit = d.PieceUnit,
                Unit_Price = d.UnitCost,
                Amount = d.CostAmount,
                Item_Tax_Type = d.TaxType,
                Remark = d.Remark,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Invoice";
            ds.Tables.Add(table);

            table = detailItems.ToDataTable();
            table.TableName = "Details";
            ds.Tables.Add(table);
            return ds;

        }

        public static DataSet GetAllowanceData(this IQueryable<InvoiceAllowance> items, GenericManager<EIVOEntityDataContext> models)
        {
            var productItems = items.Join(models.GetTable<InvoiceAllowanceDetail>(), i => i.AllowanceID, d => d.AllowanceID, (i, d) => d)
                                    .Select(d => d.InvoiceAllowanceItem);

            var dataItems = items.ToArray().Select(i => new
            {
                Allowance_No = i.AllowanceNumber,
                Allowance_Date = i.AllowanceDate,
                Seller_ID = i.InvoiceAllowanceSeller.ReceiptNo,
                //Customer_ID = i.InvoiceAllowanceBuyer.CustomerID,
                //Buyer_Name = i.InvoiceAllowanceBuyer.CustomerName,
                //Buyer_ID = i.InvoiceAllowanceBuyer.ReceiptNo,
                //Allowance_Type = i.AllowanceType,
                Contact_Name = i.InvoiceAllowanceBuyer.ContactName,
                EMail = i.InvoiceAllowanceBuyer.EMail,
                Address = i.InvoiceAllowanceBuyer.Address,
                Phone = i.InvoiceAllowanceBuyer.Phone,
                Tax_Amount = i.TaxAmount,
                Total_Amount = i.TotalAmount,
                Currency = i.CurrencyType?.AbbrevName,
            });

            var detailItems = productItems.ToArray().Select(d => new
            {
                Allowance_No = d.InvoiceAllowanceDetail.First().InvoiceAllowance.AllowanceNumber,
                Original_Invoice_Date = d.InvoiceDate,
                Original_Invoice_No = d.InvoiceNo,
                Original_Sequence_No = d.OriginalSequenceNo,
                Original_Description = d.OriginalDescription,
                Quantity = d.Piece,
                Unit_Price = d.UnitCost,
                Amount = d.Amount,
                Tax = d.Tax,
                Allowance_Sequence_No = d.No,
                Item_Tax_Type = d.TaxType,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Allowance";
            ds.Tables.Add(table);

            table = detailItems.ToDataTable();
            table.TableName = "Details";
            ds.Tables.Add(table);
            return ds;

        }

        public static DataSet GetInvoiceDataForVAC(this IQueryable<InvoiceItem> items, GenericManager<EIVOEntityDataContext> models)
        {
            var productItems = items.Join(models.GetTable<InvoiceDetail>(), i => i.InvoiceID, d => d.InvoiceID, (i, d) => d)
                                    .SelectMany(d => d.InvoiceProduct.InvoiceProductItem);

            var dataItems = items.ToArray().Select(i => new
            {
                Data_ID = i.InvoicePurchaseOrder?.OrderNo,
                Data_Date = i.InvoicePurchaseOrder?.PurchaseDate,
                Seller_ID = i.InvoiceSeller.ReceiptNo,
                Buyer_Name = i.InvoiceBuyer.CustomerName,
                Buyer_ID = i.InvoiceBuyer.ReceiptNo,
                Buyer_Mark = i.BuyerRemark,
                Customs_Clearance_Mark = i.CustomsClearanceMark,
                Customer_ID = i.InvoiceBuyer.CustomerID,
                Contact_Name = i.InvoiceBuyer.ContactName,
                EMail = i.InvoiceBuyer.EMail,
                Address = i.InvoiceBuyer.Address,
                Phone = i.InvoiceBuyer.Phone,
                Sales_Amount = i.InvoiceAmountType.SalesAmount,
                Free_Tax_Sales_Amount = (decimal?)null,
                Zero_Tax_Sales_Amount = (decimal?)null,
                Invoice_Type = (int?)i.InvoiceType,
                Tax_Type = (int?)i.InvoiceAmountType.TaxType,
                Tax_Rate = i.InvoiceAmountType.TaxRate,
                Tax_Amount = i.InvoiceAmountType.TaxAmount,
                Total_Amount = i.InvoiceAmountType.TotalAmount,
                Currency = i.InvoiceAmountType.CurrencyType?.AbbrevName,
                Print_Mark = i.PrintMark,
                Carrier_Type = i.InvoiceCarrier?.CarrierType,
                Carrier_Id1 = i.InvoiceCarrier?.CarrierNo,
                Carrier_Id2 = i.InvoiceCarrier?.CarrierNo2,
                Donate_Mark = i.DonateMark,
                NPOBAN = i.InvoiceDonation?.AgencyCode,
                Random_Number = i.RandomNo,
                Main_Remark = i.Remark,
                //Process_Type = ((Naming.InvoiceProcessType?)i.CDS_Document.ProcessType).ToString(),
            });

            var detailItems = productItems.ToArray().Select(d => new
            {
                Data_ID = d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoicePurchaseOrder?.OrderNo,
                Description = d.InvoiceProduct.Brief,
                Quantity = d.Piece,
                Unit = d.PieceUnit,
                Unit_Price = d.UnitCost,
                Amount = d.CostAmount,
                Item_Tax_Type = d.TaxType,
                Remark = d.Remark,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Invoice";
            ds.Tables.Add(table);

            table = detailItems.ToDataTable();
            table.TableName = "Details";
            ds.Tables.Add(table);
            return ds;

        }

        public static DataSet GetInvoiceDataForIssuer(this IQueryable<InvoiceItem> items, GenericManager<EIVOEntityDataContext> models)
        {
            var productItems = items.Join(models.GetTable<InvoiceDetail>(), i => i.InvoiceID, d => d.InvoiceID, (i, d) => d)
                                    .SelectMany(d => d.InvoiceProduct.InvoiceProductItem);

            var dataItems = items.ToArray().Select(i => new
            {
                Invoice_No = $"{i.TrackCode}{i.No}",
                Invoice_Date = i.InvoiceDate,
                Data_ID = i.InvoicePurchaseOrder?.OrderNo,
                Seller_ID = i.InvoiceSeller.ReceiptNo,
                Buyer_Name = i.InvoiceBuyer.CustomerName,
                Buyer_ID = i.InvoiceBuyer.ReceiptNo,
                Buyer_Mark = i.BuyerRemark,
                Customs_Clearance_Mark = i.CustomsClearanceMark,
                Customer_ID = i.InvoiceBuyer.CustomerID,
                Contact_Name = i.InvoiceBuyer.ContactName,
                EMail = i.InvoiceBuyer.EMail,
                Address = i.InvoiceBuyer.Address,
                Phone = i.InvoiceBuyer.Phone,
                Sales_Amount = i.InvoiceAmountType.SalesAmount,
                Free_Tax_Sales_Amount = (decimal?)null,
                Zero_Tax_Sales_Amount = (decimal?)null,
                Invoice_Type = (int?)i.InvoiceType,
                Tax_Type = (int?)i.InvoiceAmountType.TaxType,
                Tax_Rate = i.InvoiceAmountType.TaxRate,
                Tax_Amount = i.InvoiceAmountType.TaxAmount,
                Total_Amount = i.InvoiceAmountType.TotalAmount,
                Currency = i.InvoiceAmountType.CurrencyType?.AbbrevName,
                Print_Mark = i.PrintMark,
                Carrier_Type = i.InvoiceCarrier?.CarrierType,
                Carrier_Id1 = i.InvoiceCarrier?.CarrierNo,
                Carrier_Id2 = i.InvoiceCarrier?.CarrierNo2,
                Donate_Mark = i.DonateMark,
                NPOBAN = i.InvoiceDonation?.AgencyCode,
                Random_Number = i.RandomNo,
                Main_Remark = i.Remark,
                //Process_Type = ((Naming.InvoiceProcessType?)i.CDS_Document.ProcessType).ToString(),
            });

            var detailItems = productItems.ToArray().Select(d => new
            {
                Invoice_No = $"{d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.TrackCode}{d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.No}",
                Description = d.InvoiceProduct.Brief,
                Quantity = d.Piece,
                Unit = d.PieceUnit,
                Unit_Price = d.UnitCost,
                Amount = d.CostAmount,
                Item_Tax_Type = d.TaxType,
                Remark = d.Remark,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Invoice";
            ds.Tables.Add(table);

            table = detailItems.ToDataTable();
            table.TableName = "Details";
            ds.Tables.Add(table);
            return ds;

        }
        public static DataSet GetInvoiceDataForCBE(this IQueryable<InvoiceItem> items, GenericManager<EIVOEntityDataContext> models)
        {
            var productItems = items.Join(models.GetTable<InvoiceDetail>(), i => i.InvoiceID, d => d.InvoiceID, (i, d) => d)
                                    .SelectMany(d => d.InvoiceProduct.InvoiceProductItem);

            var dataItems = items.ToArray().Select(i => new
            {
                Data_ID = i.InvoicePurchaseOrder?.OrderNo,
                Data_Date = i.InvoicePurchaseOrder?.PurchaseDate,
                Seller_ID = i.InvoiceSeller.ReceiptNo,
                Customer_ID = i.InvoiceBuyer.CustomerID,
                Sales_Amount = i.InvoiceAmountType.SalesAmount,
                Tax_Amount = i.InvoiceAmountType.TaxAmount,
                Total_Amount = i.InvoiceAmountType.TotalAmount,
                Currency = i.InvoiceAmountType.CurrencyType?.AbbrevName,
                Carrier_Id1 = i.InvoiceCarrier?.CarrierNo,
                Main_Remark = i.Remark,
            });

            var detailItems = productItems.ToArray().Select(d => new
            {
                Data_ID = d.InvoiceProduct.InvoiceDetails.First().InvoiceItem.InvoicePurchaseOrder?.OrderNo,
                Description = d.InvoiceProduct.Brief,
                Quantity = d.Piece,
                Unit = d.PieceUnit,
                Unit_Price = d.UnitCost,
                Amount = d.CostAmount,
                Remark = d.Remark,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Invoice";
            ds.Tables.Add(table);

            table = detailItems.ToDataTable();
            table.TableName = "Details";
            ds.Tables.Add(table);
            return ds;

        }

        public static DataSet GetVoidInvoiceData(this IQueryable<InvoiceCancellation> items, GenericManager<EIVOEntityDataContext> models)
        {
            var dataItems = items.ToArray().Select(i => new
            {
                Void_Invoice_No = $"{i.InvoiceItem.TrackCode}{i.InvoiceItem.No}",
                //Buyer_ID = i.InvoiceItem.InvoiceBuyer?.ReceiptNo,
                Seller_ID = i.InvoiceItem.InvoiceSeller?.ReceiptNo,
                Invoice_Date = i.InvoiceItem.InvoiceDate,
                Void_Date = i.CancelDate,
                Reason = i.CancelReason,
                Return_Tax_Document_No = i.ReturnTaxDocumentNo,
                Remark = i.Remark,
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Void_Invoice";
            ds.Tables.Add(table);

            return ds;

        }

        public static DataSet GetVoidAllowanceData(this IQueryable<InvoiceAllowanceCancellation> items, GenericManager<EIVOEntityDataContext> models)
        {
            var dataItems = items.ToArray().Select(i => new
            {
                Void_AllowanceNo = i.InvoiceAllowance.AllowanceNumber,
                //Buyer_ID = i.InvoiceAllowance.InvoiceAllowanceBuyer?.ReceiptNo,
                Seller_ID = i.InvoiceAllowance.InvoiceAllowanceSeller?.ReceiptNo,
                Allowance_Date = i.InvoiceAllowance.AllowanceDate,
                Reason = i.CancelReason,
                Void_Date = i.CancelDate,
                Remark = i.Remark,
                //Process_Type = ((Naming.InvoiceProcessType?)i.CDS_Document.ProcessType).ToString(),
            });

            DataSet ds = new DataSet();

            DataTable table = dataItems.ToDataTable();
            table.TableName = "Void_Allowance";
            ds.Tables.Add(table);

            return ds;

        }

        public static InvoiceAllowanceCancellation PrepareVoidItem(this InvoiceAllowance allowance, GenericManager<EIVOEntityDataContext> models, ref DerivedDocument doc)
        {
            InvoiceAllowanceCancellation voidItem = new InvoiceAllowanceCancellation
            {
                InvoiceAllowance = allowance,
                AllowanceID = allowance.AllowanceID,
            };

            doc = new DerivedDocument
            {
                CDS_Document = new CDS_Document
                {
                    DocDate = DateTime.Now,
                    DocType = (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation,
                },
                SourceID = allowance.AllowanceID,
            };

            if (allowance.CDS_Document.DocumentOwner != null)
            {
                doc.CDS_Document.DocumentOwner = new DocumentOwner
                {
                    OwnerID = allowance.CDS_Document.DocumentOwner.OwnerID
                };
            }

            models.GetTable<InvoiceAllowanceCancellation>().InsertOnSubmit(voidItem);
            models.GetTable<DerivedDocument>().InsertOnSubmit(doc);

            if (allowance.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.B0401)
            {
                B0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                B0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            }
            else
            {
                D0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                D0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            }

            return voidItem;
        }

        public static InvoiceCancellation PrepareVoidItem(this InvoiceItem invoice, GenericManager<EIVOEntityDataContext> models,ref DerivedDocument doc)
        {
            InvoiceCancellation voidItem = new InvoiceCancellation
            {
                InvoiceItem = invoice,
                InvoiceID = invoice.InvoiceID,
                CancellationNo = $"{invoice.TrackCode}{invoice.No}",
            };

            doc = new DerivedDocument
            {
                CDS_Document = new CDS_Document
                {
                    DocType = (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation,
                    DocDate = DateTime.Now,
                },
                SourceID = invoice.InvoiceID
            };

            if (invoice.CDS_Document.DocumentOwner != null)
            {
                doc.CDS_Document.DocumentOwner = new DocumentOwner
                {
                    OwnerID = invoice.CDS_Document.DocumentOwner.OwnerID
                };
            }

            models.GetTable<InvoiceCancellation>().InsertOnSubmit(voidItem);
            models.GetTable<DerivedDocument>().InsertOnSubmit(doc);

            if (invoice.CDS_Document.ProcessType == (int)Naming.InvoiceProcessType.A0401)
            {
                A0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                A0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            }
            else
            {
                C0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                C0501Handler.PushStepQueueOnSubmit(models, doc.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
            }

            return voidItem;
        }

    }
}
