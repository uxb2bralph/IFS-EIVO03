﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;

using eIVOGo.Properties;
using eIVOGo.Helper;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using System.IO;
using System.Data;
using Model.InvoiceManagement.ErrorHandle;
using Business.Helper.InvoiceProcessor;
using eIVOGo.Module.Common;

namespace eIVOGo.Published
{

    public partial class eInvoiceService
    {

        static partial void AddOn()
        {
            EIVOPlatformFactory.DefaultUserCarrierType = Settings.Default.DefaultUserCarrierType;
            InvoiceEnterpriseManager.CreateMatchedDefaultUser = (item) =>
            {
                ThreadPool.QueueUserWorkItem(stateInfo =>
                {
                    using (EIVOEntityManager<Organization> mgr = new EIVOEntityManager<Organization>())
                    {
                        var orgItem = mgr.EntityList.Where(o => o.CompanyID == item.CompanyID).FirstOrDefault();
                        if (orgItem == null || orgItem.OrganizationCategory.Count == 0)
                            return;

                        var orgaCate = orgItem.OrganizationCategory.First();

                        var userProfile = new UserProfile
                        {
                            PID = orgItem.ReceiptNo,
                            Phone = orgItem.Phone,
                            EMail = orgItem.ContactEmail,
                            Address = orgItem.Addr,
                            UserProfileExtension = new UserProfileExtension
                            {
                                IDNo = orgItem.ReceiptNo
                            },
                            UserProfileStatus = new UserProfileStatus
                            {
                                CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check
                            }
                        };

                        mgr.GetTable<UserRole>().InsertOnSubmit(new UserRole
                        {
                            RoleID = (int)Naming.RoleID.ROLE_SELLER,
                            UserProfile = userProfile,
                            OrganizationCategory = orgaCate
                        });

                        mgr.SubmitChanges();

                        userProfile.NotifyToActivate();

                    }

                });
            };

        }

        protected Naming.InvoiceProcessType? processType;

        [WebMethod]
        public virtual XmlDocument UploadInvoiceAutoTrackNoByClient(XmlDocument uploadData, String clientID, int channelID)
        {
            _clientID = clientID;
            _channelID = channelID;
            return UploadInvoiceAutoTrackNoV2(uploadData);
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceByClient(XmlDocument uploadData, String clientID, int channelID,bool autoTrackNo,int processType)
        {
            _clientID = clientID;
            _channelID = channelID;
            this.processType = (Naming.InvoiceProcessType)processType;
            if (autoTrackNo)
            {
                return UploadInvoiceAutoTrackNoV2(uploadData);
            }
            else
            {
                return UploadInvoiceV2(uploadData);
            }
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowanceByClient(XmlDocument uploadData, String clientID, int channelID)
        {
            _clientID = clientID;
            _channelID = channelID;
            return UploadAllowanceV2(uploadData);
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceAutoTrackNoV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { InvoiceClientID = _clientID, ProcessType = processType })
            {
                OrganizationToken token;
                manager.UploadInvoiceAutoTrackNo(uploadData, result, out token);
                if (token!=null && manager.HasItem && token.Organization.OrganizationStatus.PrintAll == true)
                {
                    SharedFunction.SendMailMessage($"{token.Organization.CompanyName}電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, $"{token.Organization.CompanyName}電子發票開立郵件通知");
                }
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceCancellationV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                manager.UploadInvoiceCancellation(uploadData, result);
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowanceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                manager.UploadAllowance(uploadData, result);
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowanceCancellationV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                manager.UploadAllowanceCancellation(uploadData, result);
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceV2(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { InvoiceClientID = _clientID, ProcessType = processType })
            {
                OrganizationToken token;
                manager.UploadInvoice(uploadData, result, out token);
                if (token != null && manager.HasItem && token.Organization.OrganizationStatus.PrintAll == true)
                {
                    SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                }
            }

            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceCmsCSVAutoTrackNoV2(byte[] uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                byte[] dataToSign;
                if (crypto.VerifyEnvelopedPKCS7(uploadData, out dataToSign))
                {
                    String fileName = Path.Combine(Logger.LogDailyPath, String.Format("Invoice_{0}.csv", Guid.NewGuid()));
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        fs.Write(dataToSign, 0, dataToSign.Length);
                        fs.Flush();
                        fs.Close();
                    }

                    using (InvoiceManagerV3 mgr = new InvoiceManagerV3())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            CsvInvoiceUploadManagerV2 csvMgr = new CsvInvoiceUploadManagerV2(mgr, token.CompanyID);
                            Encoding encoding = dataToSign.IsUtf8(dataToSign.Length) ? Encoding.UTF8 : Encoding.GetEncoding(Settings.Default.CsvUploadEncoding);
                            csvMgr.ParseData(null, fileName, encoding);
                            if (!csvMgr.Save())
                            {
                                var items = csvMgr.ErrorList;
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = d.DataContent,
                                        Description = d.Status,
                                        ItemIndexSpecified = true,
                                        ItemIndex = csvMgr.ItemList.IndexOf(d)
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        InvoiceError = items
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                                if (token.Organization.OrganizationStatus.PrintAll == true)
                                {
                                    SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                                }
                            }

                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowanceCmsCSV(byte[] uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                byte[] dataToSign;
                if (crypto.VerifyEnvelopedPKCS7(uploadData, out dataToSign))
                {
                    String fileName = Path.Combine(Logger.LogDailyPath, String.Format("Invoice_{0}.csv", Guid.NewGuid()));
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        fs.Write(dataToSign, 0, dataToSign.Length);
                        fs.Flush();
                        fs.Close();
                    }

                    using (InvoiceManagerV3 mgr = new InvoiceManagerV3())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            CsvAllowanceUploadManager csvMgr = new CsvAllowanceUploadManager(mgr, token.CompanyID);
                            Encoding encoding = dataToSign.IsUtf8(dataToSign.Length) ? Encoding.UTF8 : Encoding.GetEncoding(Settings.Default.CsvUploadEncoding);
                            csvMgr.ParseData(null, fileName, encoding);
                            if (!csvMgr.Save())
                            {
                                var items = csvMgr.ErrorList;
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = d.DataContent,
                                        Description = d.Status,
                                        ItemIndexSpecified = true,
                                        ItemIndex = csvMgr.ItemList.IndexOf(d)
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        AllowanceError = items
                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                                //if (token.Organization.OrganizationStatus.PrintAll == true)
                                //{
                                //    SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                                //}
                            }

                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }



        #region B2B寄送發票(CSV)

        public DataTable TxtConvertToDataTable(string File, string TableName, string delimiter)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            StreamReader s = new StreamReader(File, System.Text.Encoding.Default);
            //string ss = s.ReadLine();//skip the first line
            string[] columns = s.ReadLine().Split(delimiter.ToCharArray());
            ds.Tables.Add(TableName);
            foreach (string col in columns)
            {
                bool added = false;
                string next = "";
                int i = 0;
                while (!added)
                {
                    string columnname = col + next;
                    columnname = columnname.Replace("#", "");
                    columnname = columnname.Replace("'", "");
                    columnname = columnname.Replace("&", "");

                    if (!ds.Tables[TableName].Columns.Contains(columnname))
                    {
                        ds.Tables[TableName].Columns.Add(columnname.ToUpper());
                        added = true;
                    }
                    else
                    {
                        i++;
                        next = "_" + i.ToString();
                    }
                }
            }

            string AllData = s.ReadToEnd();
            string[] rows = AllData.Split("\n".ToCharArray());

            foreach (string r in rows)
            {
                string[] items = r.Split(delimiter.ToCharArray());
                ds.Tables[TableName].Rows.Add(items);
            }

            s.Close();
            dt = ds.Tables[0];

            return dt;
        }

        #endregion B2B寄送發票(CSV)

        [WebMethod]
        public virtual XmlDocument UploadBranchTrack(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    BranchTrack interval = uploadData.TrimAll().ConvertTo<BranchTrack>();

                    using (TrackNoIntervalManager mgr = new TrackNoIntervalManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            List<AutomationItem> automation = new List<AutomationItem>();
                            var items = mgr.SaveUploadBranchTrackInterval(interval, token);

                            if (items.Count > 0)
                            {
                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = interval.Main[d.Key].SellerId,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                //失敗Response
                                automation.AddRange(items.Select(d => new AutomationItem
                                {
                                    Description = d.Value.Message,
                                    Status = 0,
                                    BranchTrack = new AutomationItemBranchTrack
                                    {
                                        InvoiceBeginNo = interval.Main[d.Key].InvoiceBeginNo,
                                        InvoiceEndNo = interval.Main[d.Key].InvoiceEndNo,
                                        PeriodNo = interval.Main[d.Key].PeriodNo,
                                        TrackCode = interval.Main[d.Key].TrackCode,
                                        Year = interval.Main[d.Key].Year,
                                        SellerId = interval.Main[d.Key].SellerId
                                    }
                                }));

                                //ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                //    new ExceptionInfo
                                //    {
                                //        Token = token,
                                //        ExceptionItems = items,
                                //        InvoiceData = interval
                                //    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }

                            //成功Response
                            if (mgr.EventItems != null && mgr.EventItems.Count > 0)
                            {
                                automation.AddRange(mgr.EventItems.Select(i => new AutomationItem
                                {
                                    Description = "",
                                    Status = 1,
                                    BranchTrack = new AutomationItemBranchTrack
                                    {
                                        InvoiceBeginNo = String.Format("{0:00000000}", i.StartNo),
                                        InvoiceEndNo = String.Format("{0:00000000}", i.EndNo),
                                        PeriodNo = String.Format("{0:00}", i.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo),
                                        TrackCode = i.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode,
                                        Year = i.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year,
                                        SellerId = i.InvoiceTrackCodeAssignment.Organization.ReceiptNo
                                    }
                                }));
                            }

                            result.Automation = automation.ToArray();

                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "資料簽章不符!!";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }


        [WebMethod]
        public virtual XmlDocument UploadInvoiceV2_C0401(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    Model.Schema.TurnKey.C0401.Invoice invoice = uploadData.TrimAll().ConvertTo<Model.Schema.TurnKey.C0401.Invoice>();

                    using (MIGInvoiceManager mgr = new MIGInvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();

                        if (token != null)
                        {
                            var ex = mgr.SaveUploadInvoice(invoice, token);

                            if (ex != null)
                            {

                                Dictionary<int, Exception> items = new Dictionary<int, Exception>();
                                items.Add(0, ex);

                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = invoice.Main.InvoiceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        ExceptionItems = items,
                                        InvoiceData_C0401 = invoice
                                    });

                            }
                            else
                            {
                                result.Result.value = 1;
                            }

                            if (mgr.HasItem && token.Organization.OrganizationStatus.PrintAll == true)
                            {
                                SharedFunction.SendMailMessage(token.Organization.CompanyName + "電子發票已匯入,請執行發票列印作業!!", Settings.Default.WebMaster, token.Organization.CompanyName + "電子發票開立郵件通知");
                            }
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                EIVOPlatformFactory.Notify();
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowance_D0401(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    Model.Schema.TurnKey.D0401.Allowance allowance = uploadData.TrimAll().ConvertTo<Model.Schema.TurnKey.D0401.Allowance>();

                    using (MIGInvoiceManager mgr = new MIGInvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();

                        if (token != null)
                        {
                            var ex = mgr.SaveUploadAllowance(allowance, token);

                            if (ex != null)
                            {

                                Dictionary<int, Exception> items = new Dictionary<int, Exception>();
                                items.Add(0, ex);

                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = allowance.Main?.AllowanceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                    new ExceptionInfo
                                    {
                                        Token = token,
                                        ExceptionItems = items,
                                        AllowanceData_D0401 = allowance
                                    });

                            }
                            else
                            {
                                result.Result.value = 1;
                            }
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }

                EIVOPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceCancellationV2_C0501(XmlDocument uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    Model.Schema.TurnKey.C0501.CancelInvoice item = uploadData.TrimAll().ConvertTo<Model.Schema.TurnKey.C0501.CancelInvoice>();

                    using (MIGInvoiceManager mgr = new MIGInvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var ex = mgr.SaveUploadInvoiceCancellation(item, token);
                            if (ex!=null)
                            {
                                Dictionary<int, Exception> items = new Dictionary<int, Exception>();
                                items.Add(0, ex);

                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = item.CancelInvoiceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                                                    new ExceptionInfo
                                                                    {
                                                                        Token = token,
                                                                        ExceptionItems = items,
                                                                        CancelInvoiceData_C0501 = item
                                                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                EIVOPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadAllowanceCancellationV2_D0501(XmlDocument uploadData)
        {
            Root result = createMessageToken();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    Model.Schema.TurnKey.D0501.CancelAllowance item = uploadData.TrimAll().ConvertTo<Model.Schema.TurnKey.D0501.CancelAllowance>();

                    using (MIGInvoiceManager mgr = new MIGInvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var ex = mgr.SaveUploadAllowanceCancellation(item, token);
                            if (ex != null)
                            {
                                Dictionary<int, Exception> items = new Dictionary<int, Exception>
                                {
                                    { 0, ex }
                                };

                                result.Response = new RootResponse
                                {
                                    InvoiceNo =
                                    items.Select(d => new RootResponseInvoiceNo
                                    {
                                        Value = item.CancelAllowanceNumber,
                                        Description = d.Value.Message,
                                        ItemIndexSpecified = true,
                                        ItemIndex = d.Key
                                    }).ToArray()
                                };

                                ThreadPool.QueueUserWorkItem(ExceptionNotification.SendNotification,
                                                                    new ExceptionInfo
                                                                    {
                                                                        Token = token,
                                                                        ExceptionItems = items,
                                                                        CancelAllowanceData_D0501 = item
                                                                    });
                            }
                            else
                            {
                                result.Result.value = 1;
                            }
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }
                EIVOPlatformFactory.Notify();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual XmlDocument UploadInvoiceEnterprise(XmlDocument uploadData)
        {
            Root result = createMessageToken();
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                uploadData.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(uploadData))
                {
                    InvoiceEnterpriseRoot enterprise = uploadData.TrimAll().ConvertTo<InvoiceEnterpriseRoot>();

                    using (InvoiceEnterpriseManager mgr = new InvoiceEnterpriseManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            if (token.Organization.OrganizationCategory.Any(c => c.CategoryID == (int)Naming.CategoryID.COMP_INVOICE_AGENT))
                            {
                                List<AutomationItem> automation = new List<AutomationItem>();
                                var items = mgr.SaveInvoiceEnterprise(enterprise, token);

                                if (items.Count > 0)
                                {
                                    result.Response = new RootResponse
                                    {
                                        InvoiceNo =
                                        items.Select(d => new RootResponseInvoiceNo
                                        {
                                            Value = enterprise.InvoiceEnterprise[d.Key].SellerId,
                                            Description = d.Value.Message,
                                            ItemIndexSpecified = true,
                                            ItemIndex = d.Key
                                        }).ToArray()
                                    };

                                    //失敗Response
                                    automation.AddRange(items.Select(d => new AutomationItem
                                    {
                                        Description = d.Value.Message,
                                        Status = 0,
                                        InvoiceEnterprise = new AutomationItemInvoiceEnterprise
                                        {
                                            SellerId = enterprise.InvoiceEnterprise[d.Key].SellerId,
                                            SellerName = enterprise.InvoiceEnterprise[d.Key].SellerName,
                                            Address = enterprise.InvoiceEnterprise[d.Key].Address,
                                            ContactMobilePhone = enterprise.InvoiceEnterprise[d.Key].ContactMobilePhone,
                                            ContactName = enterprise.InvoiceEnterprise[d.Key].ContactName,
                                            ContactPhone = enterprise.InvoiceEnterprise[d.Key].ContactPhone,
                                            Email = enterprise.InvoiceEnterprise[d.Key].Email,
                                            InvoiceType = enterprise.InvoiceEnterprise[d.Key].InvoiceType,
                                            TEL = enterprise.InvoiceEnterprise[d.Key].TEL,
                                            UndertakerName = enterprise.InvoiceEnterprise[d.Key].UndertakerName
                                        }
                                    }));
                                }
                                else
                                {
                                    result.Result.value = 1;
                                }

                                //成功Response
                                if (mgr.EventItems != null && mgr.EventItems.Count > 0)
                                {
                                    automation.AddRange(mgr.EventItems.Select(i => new AutomationItem
                                    {
                                        Description = "",
                                        Status = 1,
                                        InvoiceEnterprise = new AutomationItemInvoiceEnterprise
                                        {
                                            SellerId = i.ReceiptNo,
                                            SellerName = i.CompanyName
                                        }
                                    }));
                                }

                                result.Automation = automation.ToArray();
                            }
                            else
                            {
                                result.Result.message = "營業人尚未核准代理傳送發票!!";
                            }
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "資料簽章不符!!";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sellerInfo"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual XmlDocument GetCurrentYearInvoiceTrackCode(XmlDocument sellerInfo)
        {
            RootInvoiceTrackCode result = new RootInvoiceTrackCode
            {
                UXB2B = "電子發票系統",
                Result = new RootResult
                {
                    timeStamp = DateTime.Now,
                    value = 0
                }
            };

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                sellerInfo.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(sellerInfo))
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var items = mgr.GetTable<InvoiceTrackCode>().Where(t => t.Year == DateTime.Today.Year)
                                .OrderBy(t => t.PeriodNo);

                            if (items.Count() > 0)
                            {
                                result.Response = new RootResponseForInvoiceTrackCode
                                {
                                    InvoiceTrackCodeRoot = new InvoiceTrackCodeRoot
                                    {
                                        InvoiceTrackCode = items.Select(d => new InvoiceTrackCodeRootInvoiceTrackCode
                                        {
                                            Year = d.Year,
                                            PeriodNo = String.Format("{0:00}", d.PeriodNo),
                                            TrackCode = d.TrackCode
                                        }).ToArray()
                                    }
                                };

                                result.Result.value = 1;
                            }

                            Root root = sellerInfo.ConvertTo<Root>();
                            acknowledgeReport(mgr, token, root.Request.periodicalIntervalSpecified ? root.Request.periodicalInterval : (int?)null);
                        }
                        else
                        {
                            result.Result.message = "營業人憑證資料驗證不符!!";
                        }
                    }
                }
                else
                {
                    result.Result.message = "發票資料簽章不符!!";
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result.Result.message = ex.Message;
            }
            return result.ConvertToXml();
        }

        [WebMethod]
        public virtual String[] GetCustomerIdListByAgent(XmlDocument sellerInfo)
        {
            try
            {
                CryptoUtility crypto = new CryptoUtility();
                sellerInfo.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(sellerInfo))
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            return mgr.GetQueryByAgent(token.CompanyID)
                                .Select(o => o.ReceiptNo).ToArray();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        [WebMethod]
        public virtual XmlDocument GetVacantInvoiceNo(XmlDocument sellerInfo, String receiptNo)
        {
            List<Model.Schema.TurnKey.E0402.BranchTrackBlank> result = new List<Model.Schema.TurnKey.E0402.BranchTrackBlank>();

            try
            {
                CryptoUtility crypto = new CryptoUtility();
                sellerInfo.PreserveWhitespace = true;
                if (crypto.VerifyXmlSignature(sellerInfo))
                {
                    using (InvoiceManager mgr = new InvoiceManager())
                    {
                        ///憑證資料檢查
                        ///
                        var token = mgr.GetTable<OrganizationToken>().Where(t => t.Thumbprint == crypto.SignerCertificate.Thumbprint).FirstOrDefault();
                        if (token != null)
                        {
                            var item = mgr.GetQueryByAgent(token.CompanyID)
                                .Where(o => o.ReceiptNo == receiptNo).FirstOrDefault();
                            if (item != null)
                            {
                                var queryDate = DateTime.Today.AddMonths(-2);
                                var queryPeriod = queryDate.Month / 2 + queryDate.Month % 2;
                                var items = mgr.GetTable<InvoiceTrackCode>().Where(t => t.Year == queryDate.Year && t.PeriodNo == queryPeriod)
                                    .Join(mgr.GetTable<InvoiceTrackCodeAssignment>().Where(n => n.SellerID == item.CompanyID),
                                        t => t.TrackID, n => n.TrackID, (t, n) => n)
                                        .Where(n => n.UnassignedInvoiceNo.Count > 0)
                                        .GroupBy(n => n.TrackID);
                                if (items.Count() > 0)
                                {
                                    foreach (var g in items)
                                    {
                                        Model.Schema.TurnKey.E0402.BranchTrackBlank vacantItem = new Model.Schema.TurnKey.E0402.BranchTrackBlank
                                        {
                                            Main = new Model.Schema.TurnKey.E0402.Main
                                            {
                                                HeadBan = item.ReceiptNo,
                                                BranchBan = item.ReceiptNo,
                                                InvoiceType = (Model.Schema.TurnKey.E0402.InvoiceTypeEnum)item.OrganizationStatus.SettingInvoiceType.Value,
                                                YearMonth = String.Format("{0:000}{1:00}", queryDate.Year - 1911, queryPeriod * 2),
                                                InvoiceTrack = g.First().InvoiceTrackCode.TrackCode
                                            },
                                            Details = g.Join(mgr.GetTable<UnassignedInvoiceNo>(), n => new { n.TrackID, n.SellerID }, u => new { u.TrackID, u.SellerID }, (n, u) => u)
                                                .Select(u => new Model.Schema.TurnKey.E0402.DetailsBranchTrackBlankItem
                                                {
                                                    InvoiceBeginNo = String.Format("{0:00000000}", u.InvoiceBeginNo),
                                                    InvoiceEndNo = String.Format("{0:00000000}", u.InvoiceEndNo)
                                                }).ToArray()
                                        };

                                        result.Add(vacantItem);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result.ConvertToXml();
        }


    }
}