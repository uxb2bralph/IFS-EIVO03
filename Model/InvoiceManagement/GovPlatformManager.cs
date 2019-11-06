﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.Locale;
using Model.Properties;
using Utility;

namespace Model.InvoiceManagement
{
    public class GovPlatformManager : EIVOEntityManager<InvoiceItem>
    {
        public GovPlatformManager() : base() { }
        public GovPlatformManager(GenericManager<EIVOEntityDataContext> mgr) : base(mgr) { }

        private Action<DocumentDispatch> _chain;
        public void TransmitInvoice()
        {
            Settings.Default.C0401Outbound.CheckStoredPath();
            Settings.Default.C0501Outbound.CheckStoredPath();
            Settings.Default.D0401Outbound.CheckStoredPath();
            Settings.Default.D0501Outbound.CheckStoredPath();

            DocumentDispatch item;
            var table = this.GetTable<DocumentDispatch>();

            while ((item = table.FirstOrDefault()) != null)
            {
                try
                {
                    saveInvoiceForGovPlatform(item);
                    while (_chain != null)
                    {
                        _chain(item);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    processFaultyDispatch(item);
                }
                this.ExecuteCommand("delete DocumentDispatch where DocID={0} and TypeID={1} ", item.DocID, item.TypeID);
            }

        }

        private void saveInvoiceForGovPlatform(DocumentDispatch item)
        {
            _chain = null;
            if (item.TypeID != (int)Naming.DocumentTypeDefinition.E_Invoice)
            {
                _chain = saveAllowanceForGovPlatform;
                return;
            }

            var invoice = item.CDS_Document.InvoiceItem;
            if (invoice.InvoiceType == (byte)Naming.InvoiceTypeDefinition.一般稅額計算之電子發票
                || invoice.InvoiceType == (byte)Naming.InvoiceTypeDefinition.特種稅額計算之電子發票)
            {

                String fileName = Path.Combine(Settings.Default.C0401Outbound, String.Concat("C0401-", invoice.TrackCode, invoice.No, "-", DateTime.Now.Ticks, ".xml"));
                invoice.CreateC0401().ConvertToXml().Save(fileName);
                //Logger.Debug(fileName);

                if (invoice.Organization.OrganizationStatus.DownloadDispatch == true
                    && item.CDS_Document.DocumentDownloadQueue == null)
                {
                    item.CDS_Document.DocumentDownloadQueue = new DocumentDownloadQueue { };
                }
                item.CDS_Document.CurrentStep = (int)Naming.B2CInvoiceLevelDefinition.已開立;

                this.SubmitChanges();

                if (invoice.Organization.OrganizationStatus.DownloadDataNumber == true)
                {
                    this.ExecuteCommand(@"INSERT INTO DocumentMappingQueue
                                            (DocID)
                                        SELECT  {0}
                                        WHERE   (NOT EXISTS
                                                (SELECT NULL
                                                    FROM DocumentMappingQueue
                                                    WHERE (DocID = {0})))", invoice.InvoiceID);
                    //if (item.CDS_Document.DocumentMappingQueue == null)
                    //    item.CDS_Document.DocumentMappingQueue = new DocumentMappingQueue { };
                }

            }
        }

        private void saveAllowanceForGovPlatform(DocumentDispatch item)
        {

            _chain = null;
            if (item.TypeID != (int)Naming.DocumentTypeDefinition.E_Allowance)
            {
                //_chain = saveInvoiceCancellationForGovPlatform;
                _chain = saveAllowancCancellationForGovPlatform;
                return;
            }

            var allowance = item.CDS_Document.InvoiceAllowance;
            String fileName = Path.Combine(Settings.Default.D0401Outbound, String.Concat("D0401-", allowance.AllowanceNumber, "-", DateTime.Now.Ticks, ".xml"));
            //Logger.Debug(fileName);

            allowance.CreateD0401().ConvertToXml().Save(fileName);
            if (item.CDS_Document.DocumentOwner.Organization.OrganizationStatus.DownloadDispatch == true
                && item.CDS_Document.DocumentDownloadQueue == null)
                item.CDS_Document.DocumentDownloadQueue = new DocumentDownloadQueue { };

            item.CDS_Document.CurrentStep = (int)Naming.B2CInvoiceLevelDefinition.已開立;
            this.SubmitChanges();
        }

        //private void saveInvoiceCancellationForGovPlatform(DocumentDispatch item)
        //{

        //    _chain = null;
        //    if (item.TypeID != (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)
        //    {
        //        _chain = saveAllowancCancellationForGovPlatform;
        //        return;
        //    }

        //    InvoiceItem invoice;
        //    CDS_Document doc;

        //    if (item.CDS_Document.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice)
        //    {
        //        invoice = item.CDS_Document.InvoiceItem;
        //        doc = item.CDS_Document.ChildDocument
        //            .Select(d => d.CDS_Document)
        //            .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation).FirstOrDefault();
        //    }
        //    else
        //    {
        //        invoice = item.CDS_Document.DerivedDocument.ParentDocument.InvoiceItem;
        //        doc = item.CDS_Document;
        //    }

        //    String fileName = Path.Combine(Settings.Default.C0501Outbound, String.Concat("C0501-", invoice.TrackCode, invoice.No, "-", DateTime.Now.Ticks, ".xml"));
        //    //Logger.Debug(fileName);

        //    invoice.CreateC0501().ConvertToXml().Save(fileName);

        //    if (doc != null)
        //    {
        //        if (invoice.Organization.OrganizationStatus.DownloadDispatch == true)
        //        {
        //            if (doc.DocumentDownloadQueue != null)
        //                doc.DocumentDownloadQueue = new DocumentDownloadQueue { };
        //        }
        //        doc.CurrentStep = (int)Naming.B2CInvoiceLevelDefinition.已開立;
        //        this.SubmitChanges();
        //    }
        //}

        private void saveAllowancCancellationForGovPlatform(DocumentDispatch item)
        {
            _chain = null;
            if (item.TypeID != (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation)
            {
                _chain = processFaultyDispatch;
                return;
            }

            InvoiceAllowance allowance;
            CDS_Document doc;

            if (item.CDS_Document.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance)
            {
                allowance = item.CDS_Document.InvoiceAllowance;
                doc = item.CDS_Document.ChildDocument
                .Select(d => d.CDS_Document)
                .Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation).FirstOrDefault();
            }
            else
            {
                allowance = item.CDS_Document.DerivedDocument.ParentDocument.InvoiceAllowance;
                doc = item.CDS_Document;
            }

            String fileName = Path.Combine(Settings.Default.D0501Outbound, String.Concat("D0501-", allowance.AllowanceNumber, "-", DateTime.Now.Ticks, ".xml"));
            //Logger.Debug(fileName);

            allowance.CreateD0501().ConvertToXml().Save(fileName);


            if(doc!=null)
            {
                if (allowance.InvoiceAllowanceSeller.Organization.OrganizationStatus.DownloadDispatch == true
                    && doc.DocumentDownloadQueue == null)
                    item.CDS_Document.DocumentDownloadQueue = new DocumentDownloadQueue { };

                doc.CurrentStep = (int)Naming.B2CInvoiceLevelDefinition.已開立;
                this.SubmitChanges();
            }

        }

        private void processFaultyDispatch(DocumentDispatch item)
        {
            _chain = null;
            Logger.Warn("DocumentDispatch Fault: DocID=>" + item.DocID + ", TypeID=>" + item.TypeID);
        }

        //private void saveBranchTrackBlankForGovPlatform(InvoiceManager mgr)
        //{
        //    Settings.Default.E0402Outbound.CheckStoredPath();
        //    int counter = Directory.GetFiles(Settings.Default.E0402Outbound).Length;

        //    if ((DateTime.Now.Month % 2).Equals(1) && DateTime.Now.Day.Equals(3))
        //    {
        //        DateTime prophase = DateTime.Now.AddMonths(-2);
        //        int year = prophase.Year;
        //        int period = prophase.Month / 2;
        //        var trackcodes = mgr.GetTable<InvoiceTrackCode>().Where(t => t.Year.Equals(year) & t.PeriodNo.Equals(period));
        //        foreach (var data in trackcodes)
        //        {
        //            foreach (var d in data.InvoiceTrackCodeAssignments)
        //            {
        //                if (d.UnassignedInvoiceNo.Count > 0 && (d.Organization.OrganizationStatus.UploadBranchTrackBlank == true))
        //                {
        //                    String fileName = Path.Combine(Settings.Default.E0402Outbound, String.Format("E0402-{0:yyyyMMddHHmmssf}-{1:00000}.xml", DateTime.Now, counter++));
        //                    d.BuildE0402().ConvertToXml().Save(fileName);
        //                }
        //            }
        //        }
        //    }
        //}

        private void writeInvoiceCancellation(Stream s, InvoiceItem item)
        {
            Encoding enc = Encoding.GetEncoding(950);
            //1	表頭	1	1	A	M	固定填”C”
            s.WriteByte((byte)'C');
            //2	作廢發票號碼
            //(CancelInvoiceNumber)	2	10	AN	M	含字軌
            //範例：QQ12345678
            s.WriteStringBytesUseLog(enc, item.TrackCode + item.No, ' ', 10);
            //3	發票日期
            //(InvoiceDate)	12	10	AN	M	YYYY/MM/DD
            //西元年月日
            s.WriteStringBytesUseLog(enc, String.Format("{0:yyyy/MM/dd}", item.InvoiceDate), ' ', 10);
            //4	買方識別碼
            //(BuyerId)	22	10	AN	M	B2B:買方-營業人統一編號(BAN)。
            //B2C:買方-填滿10位數字的 ”0”。
            Organization buyer = item.InvoiceBuyer != null ? item.InvoiceBuyer.Organization : null;
            s.WriteStringBytesUseLog(enc, buyer == null ? "0000000000" : buyer.ReceiptNo, ' ', 10);
            //5	賣方識別碼
            //(SellerId)	32	10	AN	M	B2B:賣方-營業人統一編號(BAN)。
            //B2C:賣方-營業人統一編號(BAN)。
            s.WriteStringBytesUseLog(enc, item.Organization.ReceiptNo, ' ', 10);
            //6	作廢日期
            //(CancelDate)	42	10	AN	M	YYYY/MM/DD
            //西元年月日
            //7	作廢時間
            //(CancelTime)	52	8	AN	M	HH:MM:SS; (24hr);
            //[00-23]:[00-59]:[00-59]
            //範例：23:59:59
            s.WriteStringBytesUseLog(enc, String.Format("{0:yyyy/MM/ddHH:mm:ss}", item.InvoiceCancellation.CancelDate), ' ', 18);
            //8	專案作廢核准文號
            //(ReturnTaxDocumentNumber)	60	60	AN	O	若發票的作廢時間超過申報期間，則此欄位為必填欄位。若不填寫由上傳營業人自行負責。
            s.WriteStringBytesUseLog(enc, item.InvoiceCancellation.ReturnTaxDocumentNo, ' ', 60);
            //9	備註
            //(Remark)	120	200	AN	M	請填寫作廢原因
            s.WriteStringBytesUseLog(enc, item.InvoiceCancellation.Remark, ' ', 200);
            //10	換行	320	2	AN	M	CR+LF
            s.WriteStringBytesUseLog(enc, "\r\n", ' ', 2);
        }

        private void writeInvoiceItem(Stream s, InvoiceItem item)
        {
            Encoding enc = Encoding.GetEncoding(950);

            writeInvoiceMain(s, item, enc);
            writeInvoiceDetails(s, enc, item);
            writeInvoiceSummary(s, enc, item);
        }

        private void writeInvoiceMain(Stream s, InvoiceItem item, Encoding enc)
        {
            //主檔代號
            //(Main)	1	1	A	M	固定填”M”
            s.WriteByte((byte)'M');
            //發票號碼
            //(InvoiceNumber)	2	10	AN	M	含字軌
            //範例：QQ12345678
            s.WriteStringBytesUseLog(enc, item.TrackCode + item.No, ' ', 10);
            //發票日期
            //(InvoiceDate)	12	10	AN	M	YYYY/MM/DD
            //西元年月日，平台僅接收2006/12/06後的所有發票。
            //發票時間
            //(InvoiceTime)	22	8	AN	O	HH:MM:SS (24hr);
            //[00-23]:[00-59]:[00-59]
            //範例：23:59:59
            s.WriteStringBytesUseLog(enc, String.Format("{0:yyyy/MM/ddHH:mm:ss}", item.InvoiceDate), ' ', 18);
            //識別碼
            //(Identifier)	30	10	AN	M	B2C:賣方-營業人統一編號(BAN)。
            s.WriteStringBytesUseLog(enc, item.Organization.ReceiptNo, ' ', 10);
            //名稱
            //(Name)	40	60	AN	M	B2C:賣方-營業人名稱。
            s.WriteStringBytesUseLog(enc, item.Organization.CompanyName, ' ', 60);
            //地址
            //(Address)	100	100	AN	M
            s.WriteStringBytesUseLog(enc, item.Organization.Addr, ' ', 100);
            //負責人姓名
            //(PersonInCharge)	200	12	AN	O
            s.WriteStringBytesUseLog(enc, item.Organization.UndertakerName, ' ', 12);
            //電話號碼
            //(TelephoneNumber)	212	15	AN	M
            s.WriteStringBytesUseLog(enc, item.Organization.Phone, ' ', 15);
            //傳真號碼
            //(FacsimileNumber)	227	15	AN	O
            s.WriteStringBytesUseLog(enc, item.Organization.Fax, ' ', 15);
            //電子郵件地址
            //(EmailAddress)	242	40	AN	O
            s.WriteStringBytesUseLog(enc, item.Organization.ContactEmail, ' ', 40);
            //客戶編號
            //(CustomerNumber)	282	20	AN	O	發票列印及沖銷用
            s.WriteStringBytesUseLog(enc, null, ' ', 20);
            //營業人角色註記
            //(RoleRemark)
            //    302	26	AN	O	賣方營業人角色的註記說明
            s.WriteStringBytesUseLog(enc, null, ' ', 26);
            //14	識別碼
            //(Identifier)	328	10	AN	M	
            //1.	B2B:買方-營業人統一編號(BAN)。
            //當加入BAN後，即為B2B(註：對於原為B2C上傳後，事後有報帳的需求時，正確的作法應是原發票作廢，再重新開立一張有統編的發票)
            //2.	B2C:買方-填滿10位數字的 ”0”。(若不填滿10個”0”，因無法分辨是統編或消費者的區分，會影響抽獎資格)
            Organization buyer = item.InvoiceBuyer != null ? item.InvoiceBuyer.Organization : null;
            s.WriteStringBytesUseLog(enc, buyer == null ? "0000000000" : buyer.ReceiptNo, ' ', 10);
            //名稱
            //(Name)	338	60	AN	M	B2B:買方-營業人名稱。
            //B2C虛擬:買方-業者通知消費者之個人識別碼資料。
            //B2C實體:會員卡號，無會員卡號以0000表示。共4位ASCII或2位全型中文。
            s.WriteStringBytesUseLog(enc, buyer == null ? "0000" : buyer.CompanyName, ' ', 60);
            //地址
            //(Address)	398	100	AN	O	
            s.WriteStringBytesUseLog(enc, buyer == null ? null : buyer.Addr, ' ', 100);
            //負責人姓名
            //(PersonInCharge)	498	12	AN	O	
            s.WriteStringBytesUseLog(enc, buyer == null ? null : buyer.UndertakerName, ' ', 12);
            //電話號碼
            //(TelephoneNumber)	510	15	AN	O	
            s.WriteStringBytesUseLog(enc, buyer == null ? null : buyer.Phone, ' ', 15);
            //傳真號碼
            //(FacsimileNumber)	525	15	AN	O	
            s.WriteStringBytesUseLog(enc, buyer == null ? null : buyer.Fax, ' ', 15);
            //電子郵件地址
            //(EmailAddress)	540	40	AN	O	
            s.WriteStringBytesUseLog(enc, buyer == null ? null : buyer.ContactEmail, ' ', 40);
            //客戶編號
            //(CustomerNumber)	580	20	AN	O	發票列印及沖銷用
            s.WriteStringBytesUseLog(enc, null, ' ', 20);
            //營業人角色註記
            //(RoleRemark)	600	26	AN	O	買方營業人角色的註記說明
            s.WriteStringBytesUseLog(enc, null, ' ', 26);
            //發票檢查碼
            //(CheckNumber)	626	10	AN	O	可空白
            s.WriteStringBytesUseLog(enc, item.CheckNo, ' ', 10);
            //買受人註記欄
            //(BuyerRemark)	636	1	AN	O	1：得抵扣之進貨及費用；
            //2：得抵扣之固定資產；
            //3：不得抵扣之進貨及費用；
            //4：不得抵扣之固定資產
            s.WriteStringBytesUseLog(enc, $"{item.BuyerRemark}", ' ', 1);
            //總備註
            //(MainRemark)	637	200	AN	O	
            s.WriteStringBytesUseLog(enc, item.Remark, ' ', 200);
            //通關方式註記
            //(CustomsClearanceMark)	837	1	AN	O	1：非經海關出口, 2：經海關出口(零稅率時，為必要欄位)
            s.WriteStringBytesUseLog(enc, $"{item.CustomsClearanceMark}", ' ', 1);
            //稅捐稽徵處名稱
            //(TaxCenter)	838	40	AN	O	範例：財政部臺北市國稅局大安分局
            s.WriteStringBytesUseLog(enc, item.TaxCenter, ' ', 40);
            //核准日
            //(PermitDate)	878	10	AN	O	YYYY/MM/DD
            //西元年月日(ex :2007/01/01)
            s.WriteStringBytesUseLog(enc, String.Format("{0:yyyy/MM/dd}", item.PermitDate), ' ', 10);
            //核准文
            //(PermitWord)	888	40	AN	O	範例：財北國稅大安營業字
            s.WriteStringBytesUseLog(enc, item.PermitWord, ' ', 40);
            //核准號
            //(PermitNumber)	928	20	AN	O	範例：094xxxxxx
            s.WriteStringBytesUseLog(enc, item.PermitNumber, ' ', 20);
            //(內容不加「第」「號」)
            //沖帳別
            //(Category)	948	2	AN	O	
            s.WriteStringBytesUseLog(enc, item.Category, ' ', 2);
            //相關號碼
            //(RelateNumber)	950	20	AN	O	
            s.WriteStringBytesUseLog(enc, item.RelateNumber, ' ', 20);
            //發票類別
            //(InvoiceType)	970	2	AN	M	01: 三聯式;
            //02: 二聯式;
            //03: 二聯式收銀機;
            //04. 特種稅額;
            //05: 電子計算機;
            //06: 三聯式收銀機
            s.WriteStringBytesUseLog(enc, String.Format("{0:00}", item.InvoiceType), ' ', 2);
            //彙開註記
            //(GroupMark)	972	1	AN	O	以”*”表示 彙開
            s.WriteStringBytesUseLog(enc, item.GroupMark, ' ', 1);
            //捐贈註記
            //(Donate Mark)	973	1	AN	M	以”0”表示 非捐贈發票
            //以”1”表示 為捐贈發票
            s.WriteStringBytesUseLog(enc, item.DonateMark, '0', 1);
            //36	載具類別號碼
            //(CarrierType)
            //    974	6	AN	O	1.	填入平台配發的載具類別號碼
            //2.	編碼規則(例：AA0000)
            //第一碼行業別(英文)，第二碼載具類別(英數),後四碼載具類別序號(數字)
            //3.	無載具或未加入電子發票者，一定是有列印紙本電子發票，所以本欄為空白
            s.WriteStringBytesUseLog(enc, item.InvoiceByHousehold != null ? item.InvoiceByHousehold.InvoiceUserCarrier.InvoiceUserCarrierType.CarrierType : null, ' ', 6);
            //37	載具顯碼id
            //(CarrierId1)	980	64	AN	O	1.	填入載具外碼號碼(本欄是未註冊至大平台者，提供查詢有無中獎資訊之連接欄位，default為空白)
            //2.	無載具或未加入電子發票者，一定是有列印紙本電子發票，本欄空白
            s.WriteStringBytesUseLog(enc, item.InvoiceByHousehold != null ? item.InvoiceByHousehold.InvoiceUserCarrier.CarrierNo : null, ' ', 64);
            //38	載具隱碼id
            //(CarrierId2)	1044	64	AN	O	1.	填入載具內碼號碼為該載具類別的唯一識別碼
            //2.	若載具從POS機讀出時前置0會剔除，而從Kiosk或小平台讀出時前置0卻是保留者，本欄若有上述現象者，必續事先向系統建置商先報備，以避免比對不符現象(例如：悠遊卡從POS機讀出時前置0剔除，但從kiosk讀出時前置0是保留，以致兩者比對時會有不符的情形)
            //3.	無載具或未加入電子發票者，一定是有列印紙本電子發票，本欄空白
            s.WriteStringBytesUseLog(enc, item.InvoiceByHousehold != null ? item.InvoiceByHousehold.InvoiceUserCarrier.CarrierNo2 : null, ' ', 64);
            //紙本電子發票已列印註記
            //(PrintMark)	1108	1	A	M	Y or N
            s.WriteStringBytesUseLog(enc, item.InvoicePrintAssertion != null ? "Y" : "N", ' ', 1);
            //發票捐贈對象
            //(NPOBAN)	1109	10	AN	O	受捐贈者統一編號BAN
            s.WriteStringBytesUseLog(enc, item.Donatory != null ? item.Donatory.ReceiptNo : null, ' ', 10);
            //發票防偽隨機碼
            //(RandomNumber)	1119	4	AN	M	前端隨機產生
            s.WriteStringBytesUseLog(enc, String.Format("{0,4}", item.RandomNo), ' ', 4);
            //換行	1123	2	AN	M	CR+LF
            s.WriteStringBytesUseLog(enc, "\r\n", ' ', 2);
        }

        private void writeInvoiceSummary(Stream s, Encoding enc, InvoiceItem item)
        {
            //            1	彙總代號	1	1	A	M	固定填”A”
            s.WriteByte((byte)'A');
           

            if (item.InvoiceAmountType.TaxType == 2)
            {
                //2	應稅銷售額合計(新台幣)
            //(SalesAmount)	2	12	N	M	整數(小數點以下四捨五入)
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999.9999
            //Ex.-2356
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
                //3	免稅銷售額合計(新台幣)
            //(FreeTaxSalesAmount)	14	12	N	M	整數(小數點以下四捨五入)
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999.9999
            //Ex.-2356
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
                //4	零稅率銷售額合計(新台幣)
            //(ZeroTaxSalesAmount)	26	12	N	M	整數(小數點以下四捨五入)
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999.9999
            //Ex.-2356
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.SalesAmount), ' ', 12);
            }
            if (item.InvoiceAmountType.TaxType == 3)
            {
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.SalesAmount), ' ', 12);
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
            }
            else
            {
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.SalesAmount), ' ', 12);
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
                s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", 0), ' ', 12);
            }
            //5	課稅別
            //(TaxType)	38	1	AN	M	1：應稅
            //2：零稅率
            //3：免稅
            //9：混合應稅與免稅或零稅率 (限收銀機發票無法分辨時使用)
            s.WriteStringBytesUseLog(enc, String.Format("{0,1:#0}", item.InvoiceAmountType.TaxType), ' ', 1);
            //6	稅率
            //(TaxRate)	39	6
            //(6.4)	N	M	0.0500
            //至少含整數一位
            //Ex.0 or 0.00 or 0.0000 or 0.05
            s.WriteStringBytesUseLog(enc, String.Format("{0,6:#0}", item.InvoiceAmountType.TaxRate), ' ', 6);
            //7	營業稅額
            //(TaxAmount)	45	12	N	M	整數(小數點以下四捨五入)
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999
            //Ex.-2356
            s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.TaxAmount), ' ', 12);
            //8	總計
            //(TotalAmount)	57	12	N	M	整數
            //(應稅銷售額合計+免稅銷售額合計+零稅率銷售額合計+營業稅額=此總計欄位)
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999
            //Ex.-2356
            s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.TotalAmount), ' ', 12);
            //9	扣抵金額
            //(DiscountAmount)	69	12	N	O	供交易折扣使用
            //整數部分第一位不能有零
            //負號請接於整數第一位前
            //Ex.999999999999
            //Ex.-2356
            s.WriteStringBytesUseLog(enc, String.Format("{0,12:#0}", item.InvoiceAmountType.DiscountAmount), ' ', 12);
            //10	原幣金額
            //(OriginalCurrencyAmount)	81	17
            //(17.4)	N	O	供營業人外幣報價特殊需求
            //整數部分第一位不能有零
            //小數為零時不顯示小數點
            //負號請接於整數第一位前ex.999999999999.9999
            //Ex.-123
            s.WriteStringBytesUseLog(enc, String.Format("{0,17:#0.####}", item.InvoiceAmountType.OriginalCurrencyAmount), ' ', 17);
            //11	匯率
            //(ExchangeRate)	98	13
            //(13.4)	N	O	供營業人外幣報價特殊需求
            //整數部分第一位不能有零
            //小數為零時不顯示小數點
            //負號請接於整數第一位前ex.999999999.9999
            s.WriteStringBytesUseLog(enc, String.Format("{0,13:#0.####}", item.InvoiceAmountType.ExchangeRate), ' ', 13);
            //12	幣別
            //(Currency)	111	3	AN	O	供營業人外幣報價特殊需求
            s.WriteStringBytesUseLog(enc, item.InvoiceAmountType.CurrencyType != null ? String.Format("{0,3}",
                 item.InvoiceAmountType.CurrencyType.AbbrevName) : "   ", ' ', 3);
            //13	換行	114	2	AN	M	CR+LF
            s.WriteStringBytesUseLog(enc, "\r\n", ' ', 2);
        }

        private void writeInvoiceDetails(Stream s, Encoding enc, InvoiceItem item)
        {
            if (item.InvoiceDetails == null)
            {
                return;
            }

            int index = 0;

            foreach (var d in item.InvoiceDetails)
            {
                foreach (var p in d.InvoiceProduct.InvoiceProductItem)
                {
                    //1	明細代號	1	1	A	M	固定填”D"
                    s.WriteByte((byte)'D');
                    //2	品名
                    //(Description)	2	256	AN	M	
                    s.WriteStringBytesUseLog(enc, d.InvoiceProduct.Brief, ' ', 256);
                    //3	數量
                    //(Quantity)	258	17
                    //(17.4)	N	M	整數12位,小數4位
                    //整數部分第一位不能有零
                    //小數為零時不顯示小數點
                    //負號請接於整數第一位前
                    //Ex.999999999999.9999
                    //Ex.-2356
                    s.WriteStringBytesUseLog(enc, String.Format("{0,17:#.####}", p.Piece), ' ', 17);
                    //4	單位
                    //(Unit)	275	6	AN	O	可為空白
                    s.WriteStringBytesUseLog(enc, p.PieceUnit, ' ', 6);
                    //5	單價
                    //(UnitPrice)	281	17
                    //(17.4)	N	M	原幣報價
                    //整數12位,小數4位
                    //整數部分第一位不能有零
                    //小數為零時不顯示小數點
                    //負號請接於整數第一位前
                    //Ex.999999999999.9999
                    //Ex.-2356
                    s.WriteStringBytesUseLog(enc, String.Format("{0,17:#.####}", p.UnitCost), ' ', 17);
                    //6	金額
                    //(Amount)	298	17
                    //(17.4)	N	M	整數12位,小數4位
                    //整數部分第一位不能有零
                    //小數為零時不顯示小數點
                    //負號請接於整數第一位前
                    //Ex.999999999999.9999
                    //Ex.-2356
                    s.WriteStringBytesUseLog(enc, String.Format("{0,17:#.####}", p.CostAmount), ' ', 17);
                    //7	明細排列序號
                    //(SequenceNumber)	315	3	AN	M	系統使用,不重複
                    //發票明細項目之排列序號
                    s.WriteStringBytesUseLog(enc, (++index).ToString(), ' ', 3);
                    //8	項次
                    //(Item)	318	6	AN	O	
                    s.WriteStringBytesUseLog(enc, p.No.ToString(), ' ', 6);
                    //9	單一欄位備註
                    //(Remark)	324	40	AN	O	可為空白
                    s.WriteStringBytesUseLog(enc, p.Remark, ' ', 40);
                    //10	相關號碼
                    //(RelateNumber)	364	20	AN	O	
                    s.WriteStringBytesUseLog(enc, p.RelateNumber, ' ', 20);
                    //11	課稅別
                    //(TaxType)	384	1	AN	M
                    //O	M:二聯式收銀機發票
                    //O:非二聯式收銀機發票
                    //1：應稅
                    //2：零稅率
                    //3：免稅
                    //9：混合應稅與免稅或零稅率 (限收銀機發票無法分辨時使用)
                    s.WriteByte(p.TaxType.HasValue ? (byte)(p.TaxType.Value + '0') : (byte)' ');
                    //12	換行	385	2	AN	M	CR+LF
                    s.WriteStringBytesUseLog(enc, "\r\n", ' ', 2);
                }
            }

        }

        public void CheckResponse()
        {
            //String[] items = Directory.GetFiles(Settings.Default.GOVPlatformResponse);
            //if (items != null && items.Length > 0)
            //{
            //    foreach (var item in items)
            //    {
            //        if (File.Exists(item))
            //        {
            //            String fileName = Path.GetFileName(item);
            //            if (fileName.StartsWith("C0401"))
            //            {
            //                processInvoiceItemResponse(item, Naming.DocumentTypeDefinition.E_Invoice);
            //            }
            //            else if (fileName.StartsWith("C0501"))
            //            {
            //                processInvoiceItemResponse(item, Naming.DocumentTypeDefinition.E_InvoiceCancellation);
            //            }

            //            File.Move(item, Path.Combine(Logger.LogDailyPath, fileName));
            //        }
            //    }
            //}

            ThreadPool.QueueUserWorkItem(sendNotification);
        }

        private void sendNotification(object stateInfo)
        {
            using (InvoiceManager mgr = new InvoiceManager())
            {
                var items = mgr.GetTable<ReplicationNotification>();
                if (items.Count() > 0)
                { 
                    GovPlatformFactory.SendNotification(this,new EventArgs());
                }
                items.DeleteAllOnSubmit(items);
                mgr.SubmitChanges();
            }
        }

        private void processInvoiceItemResponse(string filePath,Naming.DocumentTypeDefinition docType)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buf = fs.ReadLine();
                if (buf != null)
                {
                    _GovPlatformResponseHeader header = buf.ByteArrayToStructure<_GovPlatformResponseHeader>();
                    var items = fs.Parse<_GovPlatformResponseLine>();
                    if (items.Count > 0)
                    {
                        Encoding enc = Encoding.GetEncoding(950);
                        using (InvoiceManager mgr = new InvoiceManager())
                        {
                            var table = mgr.GetTable<DocumentReplication>();
                            foreach (var item in items)
                            {
                                var invItem = mgr.EntityList.Where(i => 
                                    i.TrackCode == Encoding.Default.GetString(item.TrackCode) &&
                                    i.No == Encoding.Default.GetString(item.ItemNo).Trim()).FirstOrDefault();
                                if (invItem != null && !table.Any(d => d.DocID == invItem.InvoiceID))
                                {
                                    table.InsertOnSubmit(new DocumentReplication
                                    {
                                        DocID = invItem.InvoiceID,
                                        TypeID = (int)docType,
                                        Message = enc.GetString(item.ErrorMessage),
                                        LastActionTime = DateTime.Now
                                    });
                                    mgr.SubmitChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _GovPlatformResponseHeader
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] MsgRocDate;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] TotalCount;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ExceptionCount;
        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _GovPlatformResponseLine
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] MsgMark;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] TrackCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] ItemNo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ItemRocYear;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] ItemMonth;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] ItemDay;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 305)]
        public byte[] ErrorMessage;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _GovWinInvoiceNOList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] PassData1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] InvoiceYYYMM;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] InvoiceAxle;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] InvoiceNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] BAN;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 328)]
        public byte[] PassData2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] PrizeType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] PrizeAmt;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] PassData3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] DepositMK;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] DataType;
    }

}
