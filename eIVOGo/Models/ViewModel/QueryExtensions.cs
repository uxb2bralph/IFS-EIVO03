﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Business.Helper;
using Model.Models.ViewModel;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using DataAccessLayer.basis;
using Utility;

namespace eIVOGo.Models.ViewModel
{
    public static class QueryExtensions
    {
        public static IQueryable<InvoiceItem> InquireInvoice(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel,  GenericManager<EIVOEntityDataContext> models)
        {
            bool effective = false;
            items = items.QueryByProcessType(viewModel, models, ref effective)
                        .QueryByInvoiceNo(viewModel, models,ref effective)
                        .QueryByInvoiceDate(viewModel, models, ref effective)
                        .QueryByBuyerReceiptNo(viewModel, models, ref effective)
                        .QueryByBuyerName(viewModel, models, ref effective)
                        .QueryByCustomerID(viewModel, models, ref effective)
                        .QueryByWinning(viewModel, models, ref effective)
                        .QueryEffective(viewModel, models, ref effective);

            IQueryable<InvoiceCarrier> carrierItems = null;
            if (!String.IsNullOrEmpty(viewModel.CarrierType))
            {
                carrierItems = models.GetTable<InvoiceCarrier>().Where(c => c.CarrierType == viewModel.CarrierType);
            }

            viewModel.CarrierNo = viewModel.CarrierNo.GetEfficientString();
            if (viewModel.CarrierNo != null)
            {
                if (carrierItems == null)
                    carrierItems = models.GetTable<InvoiceCarrier>();
                carrierItems = carrierItems.Where(c => c.CarrierNo == viewModel.CarrierNo || c.CarrierNo2 == viewModel.CarrierNo);
            }

            if (carrierItems != null)
            {
                items = items.Join(carrierItems, i => i.InvoiceID, c => c.InvoiceID, (i, c) => i);
            }

            viewModel.PrintMark = viewModel.PrintMark.GetEfficientString();
            if (viewModel.PrintMark != null)
            {
                items = items.Where(i => i.PrintMark == viewModel.PrintMark);
            }

            return items;
        }

        public static IQueryable<InvoiceItem> QueryByInvoiceNo(this IQueryable<InvoiceItem> items,InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.InvoiceNo = viewModel.InvoiceNo.GetEfficientString();
            if (viewModel.InvoiceNo != null)
            {
                String invoiceNo = viewModel.InvoiceNo;
                if (invoiceNo.Length == 10)
                {
                    String trackCode = invoiceNo.Substring(0, 2);
                    String no = invoiceNo.Substring(2);
                    items = items.Where(i => i.No == no && i.TrackCode == trackCode);

                }
                else
                {
                    items = items.Where(i => i.No == invoiceNo);
                }
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByProcessType(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.ProcessType.HasValue)
            {
                if (viewModel.ProcessType == Naming.InvoiceProcessType.C0401)
                {
                    items = items
                        .Join(models.GetTable<CDS_Document>()
                            .Where(d => !d.ProcessType.HasValue 
                                || (d.ProcessType != (int)Naming.InvoiceProcessType.A0401
                                    && d.ProcessType != (int)Naming.InvoiceProcessType.A0401_Xlsx_Allocation_ByIssuer)),
                            i => i.InvoiceID, d => d.DocID, (i, d) => i);

                    effective = true;
                }
                else if (viewModel.ProcessType == Naming.InvoiceProcessType.A0401)
                {
                    items = items
                        .Join(models.GetTable<InvoiceBuyer>()
                            .Where(d => d.ReceiptNo != "0000000000"),
                            i => i.InvoiceID, d => d.InvoiceID, (i, d) => i);
                    effective = true;
                }
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByAttachment(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.Attachment.HasValue)
            {
                var attachment = models.GetTable<Attachment>();
                if (viewModel.Attachment == 1)
                {
                    items = items.Where(i => attachment.Any(a => a.DocID == i.InvoiceID));
                    effective = true;
                }
                else if (viewModel.Attachment == 0)
                {
                    items = items.Where(i => !attachment.Any(a => a.DocID == i.InvoiceID));
                    effective = true;
                }
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByWelfare(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.AgencyCode = viewModel.AgencyCode.GetEfficientString();
            if (viewModel.AgencyCode!=null)
            {
                items = items.Where(i => i.InvoiceDonation.AgencyCode == viewModel.AgencyCode);
                effective = true;
            }

            return items;
        }

        public static IQueryable<InvoiceItem> QueryByBuyerReceiptNo(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.BuyerReceiptNo = viewModel.BuyerReceiptNo.GetEfficientString();
            if (viewModel.BuyerReceiptNo != null)
            {
                if (viewModel.BuyerReceiptNo.StartsWith("!"))
                {
                    var receiptNo = viewModel.BuyerReceiptNo.Substring(1);
                    if (!String.IsNullOrEmpty(receiptNo))
                    {
                        effective = true;
                        items = items.Where(d => d.InvoiceBuyer.ReceiptNo != receiptNo);
                    }
                }
                else
                {
                    effective = true;
                    items = items.Where(d => d.InvoiceBuyer.ReceiptNo == viewModel.BuyerReceiptNo);
                }
            }

            return items;
        }

        public static IQueryable<InvoiceItem> QueryByBuyerName(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.BuyerName = viewModel.BuyerName.GetEfficientString();
            if (viewModel.BuyerName!=null)
            {
                effective = true;
                items = items.Join(models.GetTable<InvoiceBuyer>().Where(d => d.CustomerName.Contains(viewModel.BuyerName)),
                            i => i.InvoiceID, b => b.InvoiceID, (i, b) => i);
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByCustomerID(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.CustomerID = viewModel.CustomerID.GetEfficientString();
            if (viewModel.CustomerID != null)
            {
                effective = true;
                items = items.Join(models.GetTable<InvoiceBuyer>().Where(d => d.CustomerID == viewModel.CustomerID),
                            i => i.InvoiceID, b => b.InvoiceID, (i, b) => i);
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByInvoiceDate(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.InvoiceDateFrom.HasValue)
            {
                items = items.Where(i => i.InvoiceDate >= viewModel.InvoiceDateFrom);
                effective = true;
            }

            if (viewModel.InvoiceDateTo.HasValue)
            {
                items = items.Where(i => i.InvoiceDate < viewModel.InvoiceDateTo.Value.AddDays(1));
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryByWinning(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.Winning == 1)
            {
                items = items.Where(i => i.InvoiceWinningNumber != null && i.InvoiceDonation == null);
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceItem> QueryEffective(this IQueryable<InvoiceItem> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.Cancelled.HasValue)
            {
                if (viewModel.Cancelled == true)
                {
                    items = items.Where(i => i.InvoiceCancellation != null);
                }
                else
                {
                    items = items.Where(i => i.InvoiceCancellation == null);
                }
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryEffective(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel != null && viewModel.Cancelled.HasValue)
            {
                if (viewModel.Cancelled == true)
                {
                    items = items.Where(i => i.InvoiceAllowanceCancellation != null);
                }
                else
                {
                    items = items.Where(i => i.InvoiceAllowanceCancellation == null);
                }
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceAllowance> InquireAllowance(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models)
        {
            bool effective = false;
            items = items.QueryByProcessType(viewModel, models, ref effective)
                        .QueryByDataNo(viewModel, models, ref effective)
                        .QueryByAllowanceDate(viewModel, models, ref effective)
                        .QueryByBuyerReceiptNo(viewModel, models, ref effective)
                        .QueryByBuyerName(viewModel, models, ref effective)
                        .QueryByCustomerID(viewModel, models, ref effective)
                        .QueryEffective(viewModel, models, ref effective);

            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryByBuyerName(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.BuyerName = viewModel.BuyerName.GetEfficientString();
            if (viewModel.BuyerName!=null)
            {
                effective = true;
                items = items.Where(d => d.InvoiceAllowanceBuyer.CustomerName.Contains(viewModel.BuyerName));
            }
            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryBySeller(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel != null && viewModel.CompanyID.HasValue)
            {
                effective = true;
                items = items.Where(d => d.InvoiceAllowanceSeller.SellerID == viewModel.CompanyID);
            }

            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryByCustomerID(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.CustomerID = viewModel.CustomerID.GetEfficientString();
            if (viewModel.CustomerID!=null)
            {
                effective = true;
                items = items.Where(i => i.InvoiceAllowanceBuyer.CustomerID == viewModel.CustomerID);
            }

            return items;
        }
        public static IQueryable<InvoiceAllowance> QueryByBuyerReceiptNo(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.BuyerReceiptNo = viewModel.BuyerReceiptNo.GetEfficientString();
            if (viewModel.BuyerReceiptNo!=null)
            {
                effective = true;
                items = items.Where(d => d.InvoiceAllowanceBuyer.ReceiptNo == viewModel.BuyerReceiptNo.GetEfficientString());
            }

            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryByAllowanceDate(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel != null)
            {
                if (viewModel.DateFrom.HasValue)
                {
                    items = items.Where(i => i.AllowanceDate >= viewModel.DateFrom);
                    effective = true;
                }

                if (viewModel.DateTo.HasValue)
                {
                    items = items.Where(i => i.AllowanceDate < viewModel.DateTo.Value.AddDays(1));
                    effective = true;
                }
            }
            return items;
        }


        public static IQueryable<InvoiceAllowance> QueryByDataNo(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            viewModel.DataNo = viewModel.DataNo.GetEfficientString();
            if (viewModel.DataNo != null)
            {
                items = items.Where(i => i.AllowanceNumber == viewModel.DataNo.GetEfficientString()
                                            || i.InvoiceAllowanceDetails.Any(d => d.InvoiceAllowanceItem.InvoiceNo == viewModel.DataNo));
                effective = true;
            }
            return items;
        }

        public static IQueryable<InvoiceAllowance> QueryByProcessType(this IQueryable<InvoiceAllowance> items, InquireInvoiceViewModel viewModel, GenericManager<EIVOEntityDataContext> models, ref bool effective)
        {
            if (viewModel.ProcessType.HasValue)
            {
                if (viewModel.ProcessType == Naming.InvoiceProcessType.D0401)
                {
                    items = items
                        .Join(models.GetTable<CDS_Document>()
                            .Where(d => !d.ProcessType.HasValue || d.ProcessType == (int)viewModel.ProcessType),
                            i => i.AllowanceID, d => d.DocID, (i, d) => i);
                }
                else
                {
                    items = items
                        .Join(models.GetTable<CDS_Document>()
                            .Where(d => d.ProcessType == (int)viewModel.ProcessType),
                            i => i.AllowanceID, d => d.DocID, (i, d) => i);
                }
                effective = true;
            }
            return items;
        }




    }
}