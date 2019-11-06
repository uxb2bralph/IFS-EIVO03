using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Locale;
using Model.Schema.EIVO;
using Utility;

namespace Model.InvoiceManagement
{
    public static partial class InvoiceRootValidator
    {
        public readonly static String[] __InvoiceTypeList = { "01", "02", "03", "04", "05", "06" };

        public static Exception CheckInvoice(this InvoiceRootInvoice invItem, out String trackCode,out String invNo,out DateTime invoiceDate)
        {
            trackCode = null;
            invNo = null;
            invoiceDate = default(DateTime);

            if (invItem.InvoiceNumber==null || !Regex.IsMatch(invItem.InvoiceNumber,"^[a-zA-Z]{2}[0-9]{8}$"))
            {
                return new Exception(String.Format("發票號碼，傳送資料:{0}，TAG：< InvoicNumber />",invItem.InvoiceNumber));
            }

            if (String.IsNullOrEmpty(invItem.InvoiceDate))
            {
                return new Exception("發票日期，TAG：< InvoiceDate />");
            }

            if (String.IsNullOrEmpty(invItem.InvoiceTime))
            {
                return new Exception("發票時間，TAG：< InvoiceTime />");
            }

            if (!DateTime.TryParseExact(String.Format("{0} {1}", invItem.InvoiceDate,invItem.InvoiceTime), "yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out invoiceDate))
            {
                return new Exception(String.Format("發票日期、發票時間格式錯誤(YYYY/MM/DD HH:mm:ss)；上傳資料:{0} {1}", invItem.InvoiceDate,invItem.InvoiceTime));
            }

            trackCode = invItem.InvoiceNumber.Substring(0, 2);
            invNo = invItem.InvoiceNumber.Substring(2);

            return null;
        }

        public static Exception CheckDataNumber(this InvoiceRootInvoice invItem, GenericManager<EIVOEntityDataContext> mgr,out InvoicePurchaseOrder order)
        {
            order = null;
            if (String.IsNullOrEmpty(invItem.DataNumber))
            {
                return new Exception("單據號碼錯誤，TAG:< DataNumber />");
            }

            if (invItem.DataNumber.Length > 20)
            {
                return new Exception(String.Format("單據號碼資料長度最長為20碼；傳送資料:{0}，TAG:< DataNumber />", invItem.DataNumber));
            }

            if (mgr.GetTable<InvoicePurchaseOrder>().Any(d => d.OrderNo == invItem.DataNumber))
            {
                return new Exception(String.Format("單據號碼不可重複；傳送資料:{0}，TAG:< DataNumber />", invItem.DataNumber));
            }

            if (String.IsNullOrEmpty(invItem.DataDate))
            {
                return new Exception("單據日期錯誤，TAG:< DataDate />");
            }

            DateTime dataDate;
            if (!DateTime.TryParseExact(invItem.DataDate, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dataDate))
            {
                return new Exception(String.Format("單據日期格式錯誤(YYYY/MM/DD)；上傳資料:{0}", invItem.DataDate));
            }

            order = new InvoicePurchaseOrder
            {
                OrderNo = invItem.DataNumber,
                PurchaseDate = dataDate
            };

            return null;

        }

        public static Exception CheckMandatoryFields(this InvoiceRootInvoice invItem)
        {
            if (String.IsNullOrEmpty(invItem.SellerId))
            {
                return new Exception("賣方-營業人統一編號錯誤，TAG:< SellerId />");
            }

            if (String.IsNullOrEmpty(invItem.BuyerName))
            {
                return new Exception("買方-名稱錯誤，TAG:< BuyerName />");
            }

            if (String.IsNullOrEmpty(invItem.BuyerId))
            {
                return new Exception("買方-營業人統一編號錯誤，TAG:< BuyerId />");
            }

            if (String.IsNullOrEmpty(invItem.InvoiceType))
            {
                return new Exception("發票類別錯誤，TAG:< InvoiceType />");
            }

            if (String.IsNullOrEmpty(invItem.DonateMark))
            {
                return new Exception("捐贈註記錯誤，TAG:< DonateMark />");
            }

            if (String.IsNullOrEmpty(invItem.PrintMark))
            {
                return new Exception("紙本電子發票已列印註記錯誤，TAG:< PrintMark />");
            }

            if(!__InvoiceTypeList.Contains(invItem.InvoiceType))
            {
                return new Exception(String.Format("發票類別格式錯誤，請依發票種類填寫相應代號\"01\"-\"06\"，上傳資料：{0}，TAG:< InvoiceType />", invItem.InvoiceType));
            }


            //if (String.IsNullOrEmpty(invItem.SalesAmount.ToString()))
            //{
            //    return  new Exception("應稅銷售額合計(新台幣)錯誤，TAG:< SalesAmount />");
            //}

            //if (String.IsNullOrEmpty(invItem.FreeTaxSalesAmount.ToString()))
            //{
            //    return  new Exception("免稅銷售額合計(新台幣)錯誤，TAG:< FreeTaxSalesAmount />");
            //}

            //if (String.IsNullOrEmpty(invItem.ZeroTaxSalesAmount.ToString()))
            //{
            //    return  new Exception("零稅率銷售額合計(新台幣)錯誤，TAG:< ZeroTaxSalesAmount />");
            //}
            if (!Enum.IsDefined(typeof(Naming.TaxTypeDefinition), invItem.TaxType))
            {
                return new Exception("課稅別錯誤，TAG:< TaxType />");
            }

            //if (String.IsNullOrEmpty(invItem.TaxRate.ToString()))
            //{
            //    return new Exception("稅率錯誤，TAG:< TaxRate />");
            //}

            //if (String.IsNullOrEmpty(invItem.TaxAmount.ToString()))
            //{
            //    return new Exception("營業稅額錯誤，TAG:< TaxAmount />");
            //}

            //if (String.IsNullOrEmpty(invItem.TotalAmount.ToString()))
            //{
            //    return new Exception("總計錯誤，TAG:< TotalAmount />";
            //}

            if (invItem.Contact == null)
            {
                return new Exception("聯絡人資料錯誤，TAG:< Contact />");
            }

            if (String.IsNullOrEmpty(invItem.Contact.Name) || invItem.Contact.Name.Length>64)
            {
                return new Exception("聯絡人名稱錯誤，TAG:< Name />");
            }

            if (String.IsNullOrEmpty(invItem.Contact.Address) || invItem.Contact.Address.Length>64)
            {
                return new Exception("聯絡人地址錯誤，TAG:< Address />");
            }

            if (!String.IsNullOrEmpty(invItem.Contact.Email) && invItem.Contact.Email.Length>512)
            {
                return new Exception("聯絡人Email錯誤，TAG:< Email />");
            }

            if (!String.IsNullOrEmpty(invItem.Contact.TEL) && invItem.Contact.TEL.Length>64)
            {
                return new Exception("聯絡人電話號碼錯誤，TAG:< TEL />");
            }

            return null;
        }

        public static Exception CheckBusiness(this InvoiceRootInvoice invItem, GenericManager<EIVOEntityDataContext> mgr, OrganizationToken owner, out Organization seller)
        {
            seller = mgr.GetTable<Organization>().Where(o => o.ReceiptNo == invItem.SellerId).FirstOrDefault();

            if (seller == null)
            {
                return new Exception(String.Format("賣方為非註冊商家,開立人統一編號:{0}，TAG:< SellerId />", invItem.SellerId));
            }

            if (seller.CompanyID != owner.CompanyID)
            {
                return new Exception(String.Format("簽章設定人與發票開立人不符,開立人統一編號:{0}，TAG:< SellerId />", invItem.SellerId));
            }

            if (invItem.BuyerId == "0000000000")
            {
                if (invItem.BuyerName == null || Encoding.GetEncoding(950).GetBytes(invItem.BuyerName).Length != 4)
                {
                    return new Exception(String.Format("B2C買方名稱格式錯誤，長度為ASCII字元4碼或中文全形字元2碼，傳送資料：{0}，TAG:< BuyerName />", invItem.BuyerName));
                }
            }
            else if (invItem.BuyerId == null || !Regex.IsMatch(invItem.BuyerId, "^[0-9]{8}$"))
            {
                return new Exception(String.Format("買方識別碼錯誤，傳送資料：{0}，TAG:< BuyerId />", invItem.BuyerId));
            }
            else if (invItem.BuyerName.Length > 60)
            {
                return new Exception(String.Format("買方名稱格式錯誤，長度最多60碼，傳送資料：{0}，TAG:< BuyerName />", invItem.BuyerName));
            }

            if (String.IsNullOrEmpty(invItem.RandomNumber))
            {
                invItem.RandomNumber = String.Format("{0:ffff}", DateTime.Now); 
            }
            else if (!Regex.IsMatch(invItem.RandomNumber, "^[0-9]{4}$"))
            {
                return new Exception(String.Format("交易隨機碼應由4位數值構成，上傳資料：{0}，TAG:< RandomNumber />", invItem.RandomNumber));
            }


            return null;

            //if (!invItem.BuyerId.Equals("0000000000"))
            //{
            //    carrierID = true;
            //}
            //else
            //{
            //    if (seller.OrganizationStatus.PrintAll == false || seller.OrganizationStatus.PrintAll.HasValue == false)
            //    {
            //        chkPrint(invItem, ref carrierID, ref Donat);
            //    }
            //}
        }



        public static Exception CheckInvoiceDonation(this InvoiceRootInvoice invItem, bool printed,out InvoiceDonation donation)
        {
            donation = null;
            if (invItem.DonateMark == "1")
            {
                if (printed)
                {
                    return new Exception(String.Format("已標註捐贈時不得標註列印，傳送資料：{0}，TAG:< PrintMark />", invItem.PrintMark));
                }

                if (String.IsNullOrEmpty(invItem.NPOBAN))
                {
                    return new Exception(String.Format("未指定發票捐贈對象統一編號，傳送資料：{0}，TAG:< NPOBAN />", invItem.NPOBAN));
                }
                donation = new InvoiceDonation
                {
                    AgencyCode = invItem.NPOBAN
                };
            }
            else if (invItem.DonateMark == "0")
            {

            }
            else
            {
                return new Exception(String.Format("捐贈註記錯誤，傳送資料：{0}，TAG:< DonateMark />", invItem.DonateMark));
            }
            return null;
        }

        public static Exception CheckInvoiceCarrier(this InvoiceRootInvoice invItem,Organization seller,out bool printed, out InvoiceCarrier carrier)
        {
            carrier = null;
            printed = seller.OrganizationStatus.PrintAll == true || invItem.PrintMark == "Y" || invItem.PrintMark == "y";

            if (invItem.BuyerId == "0000000000")
            {
                if (printed)
                {
                    if (!String.IsNullOrEmpty(invItem.CarrierType))
                    {
                        return new Exception(String.Format("註記列印時載具類別請留空白，傳送資料：{0}，TAG:< CarrierType />", invItem.CarrierType));
                    }
                    if (!String.IsNullOrEmpty(invItem.CarrierId1))
                    {
                        return new Exception(String.Format("註記列印時載具顯碼請留空白，傳送資料：{0}，TAG:< CarrierId1 />", invItem.CarrierId1));
                    }
                    if (!String.IsNullOrEmpty(invItem.CarrierId2))
                    {
                        return new Exception(String.Format("註記列印時載具隱碼請留空白，傳送資料：{0}，TAG:< CarrierId2 />", invItem.CarrierId2));
                    }
                }
                else
                {
                    carrier = new InvoiceCarrier
                    {
                        CarrierType = String.IsNullOrEmpty(invItem.CarrierType) ? "3J0001" : invItem.CarrierType,
                        CarrierNo = String.IsNullOrEmpty(invItem.CarrierId1) ? System.Guid.NewGuid().ToString() : invItem.CarrierId1
                    };

                    if (String.IsNullOrEmpty(invItem.CarrierId2))
                    {
                        carrier.CarrierNo2 = carrier.CarrierNo;
                    }
                    else
                    {
                        carrier.CarrierNo2 = invItem.CarrierId2;
                    }
                }
            }
            return null;
        }


        public static Exception CheckAmount(this InvoiceRootInvoice invItem)
        {
            //應稅銷售額
            String strValue = String.Format("{0:.}", invItem.SalesAmount);
            if (invItem.SalesAmount < 0 || strValue != invItem.SalesAmount.ToString())
            {
                return new Exception(String.Format("應稅銷售額合計(新台幣)不可為負數且為整數，上傳資料：{0},TAG:< SalesAmount />", invItem.SalesAmount));
            }


            strValue = String.Format("{0:.}", invItem.FreeTaxSalesAmount);
            if (invItem.FreeTaxSalesAmount < 0 || strValue != invItem.FreeTaxSalesAmount.ToString())
            {
                return new Exception(String.Format("免稅銷售額合計(新台幣)不可為負數且為整數，上傳資料：{0},TAG:< FreeTaxSalesAmount />", invItem.FreeTaxSalesAmount));
            }

            strValue = String.Format("{0:.}", invItem.ZeroTaxSalesAmount);
            if (invItem.ZeroTaxSalesAmount < 0 || strValue != invItem.ZeroTaxSalesAmount.ToString())
            {
                return new Exception(String.Format("零稅率銷售額合計(新台幣)不可為負數且為整數，上傳資料：{0},TAG:< ZeroTaxSalesAmount />", invItem.ZeroTaxSalesAmount));
            }


            strValue = String.Format("{0:.}", invItem.TaxAmount);
            if (invItem.TaxAmount < 0 || strValue != invItem.TaxAmount.ToString())
            {
                return new Exception(String.Format("營業稅額不可為負數且為整數，上傳資料：{0},TAG:< TaxAmount />", invItem.TaxAmount));
            }

            strValue = String.Format("{0:.}", invItem.TotalAmount);
            if (invItem.TotalAmount < 0 || strValue != invItem.TotalAmount.ToString())
            {
                return new Exception(String.Format("總金額不可為負數且為整數，上傳資料：{0},TAG:< TaxAmount />", invItem.TotalAmount));
            }

            //課稅別
            if (!Enum.IsDefined(typeof(Naming.TaxTypeDefinition), invItem.TaxType))
            {
                return new Exception(String.Format("課稅別格式錯誤，上傳資料：{0},TAG:< TaxType />", invItem.TaxType));
            }

            if (invItem.TaxRate != 0m && invItem.TaxRate != 0.05m)
            {
                return new Exception(String.Format("稅率格式錯誤，上傳資料：{0},TAG:< TaxRate />", invItem.TaxRate));
            }

            if (invItem.TaxType == (byte)Naming.TaxTypeDefinition.零稅率)
            {
                if (String.IsNullOrEmpty(invItem.CustomsClearanceMark))
                {
                    return new Exception(String.Format("若為零稅率發票，通關方式註記(CustomsClearanceMark)為必填欄位，上傳資料：{0},TAG:< CustomsClearanceMark />", invItem.CustomsClearanceMark));
                }
                else if (invItem.CustomsClearanceMark != "1" && invItem.CustomsClearanceMark != "2")
                {
                    return new Exception(String.Format("通關方式註記格式錯誤，限填非經海關出口：\"1\"或經海關出口：\"2\"，上傳資料：{0},TAG:< CustomsClearanceMark />", invItem.CustomsClearanceMark));
                }
            }
            else if (!String.IsNullOrEmpty(invItem.CustomsClearanceMark))
            {
                if (invItem.CustomsClearanceMark != "1" && invItem.CustomsClearanceMark != "2")
                {
                    return new Exception(String.Format("通關方式註記格式錯誤，限填非經海關出口：\"1\"或經海關出口：\"2\"，上傳資料：{0},TAG:< CustomsClearanceMark />", invItem.CustomsClearanceMark));
                }
            }

            return null;
        }


        public static Exception CheckInvoiceProductItems(this InvoiceRootInvoice invItem, out IEnumerable<InvoiceProductItem> productItems)
        {
            short seqNo = 0;
            productItems = invItem.InvoiceItem.Select(i => new InvoiceProductItem
            {
                InvoiceProduct = new InvoiceProduct { Brief = i.Description },
                CostAmount = i.Amount,
                ItemNo = i.Item,
                Piece = i.Quantity,
                PieceUnit = i.Unit,
                UnitCost = i.UnitPrice,
                Remark = i.Remark,
                TaxType = i.TaxType,
                No = (seqNo++)
            });


            foreach (var product in productItems)
            {
                if (String.IsNullOrEmpty(product.InvoiceProduct.Brief) || product.InvoiceProduct.Brief.Length > 256)
                {
                    return new Exception(String.Format("品項名稱不可空白長度不得大於256，傳送資料：{0}，TAG:< Description />", product.InvoiceProduct.Brief));
                }


                if (!String.IsNullOrEmpty(product.PieceUnit) && product.PieceUnit.Length > 6)
                {
                    return new Exception(String.Format("單位格式錯誤，傳送資料：{0}，TAG:< Unit />", product.PieceUnit));
                }

                String valueStr = product.UnitCost.ToString();
                if (valueStr != String.Format("{0:.####}", product.UnitCost) || valueStr.Length > 17)
                {
                    return new Exception(String.Format("單價資料格式錯誤，傳送資料：{0}，TAG:< UnitPrice />", product.UnitCost));
                }

                valueStr = product.CostAmount.ToString();
                if (valueStr != String.Format("{0:.####}", product.CostAmount) || valueStr.Length > 17)
                {
                    return new Exception(String.Format("金額格式錯誤，傳送資料：{0}，TAG:< Amount />", product.CostAmount));
                }

            }
            return null;
        }

    }
}
