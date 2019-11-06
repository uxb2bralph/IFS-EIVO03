using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;

using Model.DataEntity;
using DataAccessLayer.basis;
using Model.Security.MembershipManagement;
using Model.Locale;
using Utility;
using Uxnet.Web.Module.Common;
using Uxnet.Web.WebUI;
using eIVOGo.Properties;
using Model.InvoiceManagement;
using System.Net;
using System.Text;

namespace eIVOGo.Helper
{
    public static partial class B2BExtensionMethods
    {
        public static readonly string  TempForReceivePDF;

        static B2BExtensionMethods()
        {
            TempForReceivePDF = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "TempForReceivePDF");// Server.MapPath("~/TempForReceivePDF");
            if (!Directory.Exists(TempForReceivePDF))
                Directory.CreateDirectory(TempForReceivePDF);
        }

        public static void ReceiveInvoiceItem(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, InvoiceItem item)
        {
            userProfile.MoveToNextStep(item.CDS_Document, mgr);
            ThreadPool.QueueUserWorkItem(t =>
            {
                EIVOPlatformFactory.NotifyReceivedInvoice(userProfile, new EventArgs<InvoiceItem> { Argument = item });
            });
            
        }

        public static void IssueInvoiceItem(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, InvoiceItem item)
        {
            item.InvoiceDate = DateTime.Now;
            userProfile.MoveToNextStep(item.CDS_Document, mgr);
        }

        public static void ReceiveInvoiceCancellation(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static void IssueInvoiceCancellation(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static void IssueInvoiceAllowance(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, InvoiceAllowance item)
        {
            userProfile.MoveToNextStep(item.CDS_Document, mgr);
        }

        public static void ReceiveInvoiceAllowance(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, InvoiceAllowance item)
        {
            userProfile.MoveToNextStep(item.CDS_Document, mgr);
        }


        public static void IssueAllowanceCancellation(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static void ReceiveAllowanceCancellation(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static void ReceiveReceipt(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static void ReceiveReceiptCancellation(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, CDS_Document item)
        {
            userProfile.MoveToNextStep(item, mgr);
        }

        public static bool MoveToNextStep(this UserProfileMember userProfile, CDS_Document item, GenericManager<EIVOEntityDataContext> mgr)
        {
            var flowStep = item.DocumentFlowStep;
            if (flowStep != null)
            {
                var currentStep = mgr.GetTable<DocumentFlowControl>().Where(f => f.StepID == flowStep.CurrentFlowStep).First();
                if (currentStep.NextStep.HasValue)
                {
                    var nextStep = currentStep.NextStepItem;
                    flowStep.CurrentFlowStep = nextStep.StepID;
                    item.CurrentStep = nextStep.LevelID;

                    mgr.GetTable<DocumentProcessLog>().InsertOnSubmit(new DocumentProcessLog
                    {
                        DocID = flowStep.DocID,
                        StepDate = DateTime.Now,
                        FlowStep = nextStep.LevelID,
                        UID = userProfile.UID
                    });

                    mgr.SubmitChanges();
                    return true;
                }
            }
            return false;
        }


        public static Naming.InvoiceCenterBusinessType? CheckBusinessType(this InvoiceItem invoice, GenericManager<EIVOEntityDataContext> mgr,int companyID)
        {
            if (invoice.InvoiceSeller.SellerID == companyID)
            {
                return Naming.InvoiceCenterBusinessType.銷項;
            }
            else if (invoice.InvoiceBuyer.BuyerID == companyID)
            {
                return Naming.InvoiceCenterBusinessType.進項;
            }
            else if (mgr.GetTable<BusinessRelationship>().Any(b => b.MasterID == invoice.InvoiceSeller.SellerID && b.RelativeID == invoice.InvoiceBuyer.BuyerID))
            {
                return Naming.InvoiceCenterBusinessType.銷項;
            }
            else if (mgr.GetTable<BusinessRelationship>().Any(b => b.MasterID == invoice.InvoiceBuyer.BuyerID && b.RelativeID == invoice.InvoiceSeller.SellerID))
            {
                return Naming.InvoiceCenterBusinessType.進項;
            }
            return (Naming.InvoiceCenterBusinessType?)null;
        }

//        public static String CreateContentAsPDF(this HttpServerUtility Server, String relativePath,double timeOutInMinute)
//        {
//            String path = Server.MapPath("~/temp");
//            path.CheckStoredPath();

//            Guid uniqueID = Guid.NewGuid();
//            String saveTo = Path.Combine(path, String.Format("{0}.htm",uniqueID ));
//            String pdfFile = Path.Combine(path, String.Format("{0}.pdf", uniqueID));
//            String tempHtml = Path.Combine(Logger.LogDailyPath, String.Format("{0}.htm", uniqueID));

//            using (StreamWriter sw = new StreamWriter(tempHtml))
//            {
//                Server.Execute(relativePath, sw, true);
//                sw.Flush();
//                sw.Close();
//            }
//            File.Move(tempHtml, saveTo);

////            convertHtmlToPDF(saveTo, pdfFile, timeOutInMinute);

//            saveTo.ConvertHtmlToPDF(pdfFile, timeOutInMinute);

//            if (File.Exists(pdfFile))
//            {
//                File.Delete(saveTo);

//                bool checking = true;
//                while (checking)
//                {
//                    try
//                    {
//                        using (var fs = File.OpenRead(pdfFile))
//                        {
//                            fs.Close();
//                            checking = false;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.Error(ex);
//                    }
//                }
//                return pdfFile;
//            }

//            return null;
//        }

        private static void convertHtmlToPDF(String htmlFile, String pdfFile, double timeOutInMinute)
        {
            String pdfTrigger = Path.Combine(Logger.LogDailyPath, String.Format("{0}.txt", Path.GetFileNameWithoutExtension(htmlFile)));
            using (StreamWriter sw = new StreamWriter(pdfTrigger))
            {
                sw.Write(htmlFile);
                sw.Flush();
                sw.Close();
            }
            File.Move(pdfTrigger, Path.Combine(Settings.Default.PDFWorkingQueue, Path.GetFileName(pdfTrigger)));

            DateTime deadline = DateTime.Now.AddMinutes(timeOutInMinute);

            while (!File.Exists(pdfFile) && DateTime.Now < deadline)
            {
                Thread.Yield();
            }
        }

        public static bool CheckTokenIdentity(this UserProfileMember userProfile, GenericManager<EIVOEntityDataContext> mgr, SignContext signContext)
        {
            var item = mgr.GetTable<Organization>().Where(o => o.CompanyID == userProfile.CurrentUserRole.OrganizationCategory.CompanyID).First();
            bool result = (item.OrganizationToken != null && item.OrganizationToken.Thumbprint == signContext.SignerCertificate.Thumbprint)
                    || (item.OrganizationStatus != null && item.OrganizationStatus.TokenID.HasValue && item.OrganizationStatus.UserToken.Thumbprint == signContext.SignerCertificate.Thumbprint);
            if (!result)
            {
                signContext.AjaxAlert("您所使用的數位憑證與約定的會員數位憑證不符,請重新設定會員簽章數位憑證!!");
            }
            return result;
        }


        public static String CreatePdfFile(this InvoiceItem item,bool alwaysCreateNew = false)
        {

            String fileName = TempForReceivePDF.GetDateStylePath(item.InvoiceDate.Value) + "\\" + item.TrackCode + item.No + ".pdf";

            if (alwaysCreateNew || !File.Exists(fileName))
            {

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    String tempPDF = client.DownloadString(String.Format("{0}{1}?id={2}&nameOnly=true",
                                            Uxnet.Web.Properties.Settings.Default.HostUrl,
                                            VirtualPathUtility.ToAbsolute("~/DataView/PrintSingleInvoiceAsPDF"),
                                            item.InvoiceID));

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    //Logger.Debug($"move file: {tempPDF} -> {fileName}");

                    File.Move(tempPDF, fileName);
                }
            }

            return fileName;
        }

        public static String PrepareToDownload(this GenericManager<EIVOEntityDataContext> mgr, InvoiceItem item,bool? isMail)
        {
            String fileName = item.CreatePdfFile();

            if (File.Exists(fileName))
            {

                var docQ = mgr.GetTable<DocumentSubscriptionQueue>();

                if (item.InvoiceBuyer?.Organization?.OrganizationStatus?.EntrustToPrint == true
                    && !docQ.Any(q => q.DocID == item.InvoiceID) && (bool)!isMail)
                {
                    docQ.InsertOnSubmit(new DocumentSubscriptionQueue { DocID = item.InvoiceID });
                    mgr.SubmitChanges();
                }

                return fileName;
            }

            return null;

        }

        public static String CreatePdfFile(this InvoiceAllowance item, bool alwaysCreateNew = false)
        {

            String fileName = $"{TempForReceivePDF.GetDateStylePath(item.AllowanceDate.Value)}\\{item.AllowanceNumber}.pdf";

            if (alwaysCreateNew || !File.Exists(fileName))
            {

                using (WebClient client = new WebClient())
                {
                    String tempPDF = client.DownloadString(String.Format("{0}{1}?id={2}&nameOnly=true",
                                            Uxnet.Web.Properties.Settings.Default.HostUrl,
                                            VirtualPathUtility.ToAbsolute("~/DataView/PrintSingleAllowanceAsPDF"),
                                            item.AllowanceID));

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    File.Move(tempPDF, fileName);
                }
            }

            return fileName;
        }

        public static String PrepareToDownload(this GenericManager<EIVOEntityDataContext> mgr, InvoiceAllowance item)
        {
            String fileName = item.CreatePdfFile();

            if (File.Exists(fileName))
            {

                var docQ = mgr.GetTable<DocumentSubscriptionQueue>();

                if (item.InvoiceAllowanceBuyer?.Organization?.OrganizationStatus?.EntrustToPrint == true
                    && !docQ.Any(q => q.DocID == item.AllowanceID))
                {
                    docQ.InsertOnSubmit(new DocumentSubscriptionQueue { DocID = item.AllowanceID });
                    mgr.SubmitChanges();
                }

                return fileName;
            }

            return null;

        }


    }
}