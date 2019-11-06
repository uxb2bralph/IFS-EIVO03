﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement.enUS;
using Model.InvoiceManagement.ErrorHandle;
using Model.InvoiceManagement.InvoiceProcess;
using Model.InvoiceManagement.Validator;
using Model.Locale;
using Model.Properties;
using Model.Schema.EIVO;
using Utility;

namespace Model.InvoiceManagement
{
    public partial class GoogleInvoiceManagerV2 : GoogleInvoiceManager
    {

        public GoogleInvoiceManagerV2()
            : base()
        {

        }
        public GoogleInvoiceManagerV2(GenericManager<EIVOEntityDataContext> mgr)
            : base(mgr)
        {

        }

        public override Dictionary<int, Exception> SaveUploadInvoiceAutoTrackNo(InvoiceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();

            if (item != null && item.Invoice != null && item.Invoice.Length > 0)
            {
                //Organization donatory = owner.Organization.InvoiceWelfareAgencies.Select(w => w.WelfareAgency.Organization).FirstOrDefault();
                EventItems = null;
                List<InvoiceItem> eventItems = new List<InvoiceItem>();
                bool forTerms = ChannelID.HasValue && ChannelID == (int)Naming.ChannelIDType.ForGoogleTerms;

                bool countInfo = item.Invoice.Length > 1000;
                if(countInfo)
                {
                    Console.WriteLine($"Large file process:{item.Invoice.Length}");
                }

                int count = 0, dbCheckCount = 180;
                InvoiceManager workingMgr = new InvoiceManager();
                GoogleInvoiceRootInvoiceValidator validator = new GoogleInvoiceRootInvoiceValidator(workingMgr, owner.Organization)
                {
                    UseDefaultCrossBorderMerchantCarrier = true,
                };
                validator.StartAutoTrackNo();
                for (int idx = 0; idx < item.Invoice.Length; idx++, count++)
                {
                    if (count == dbCheckCount)
                    {
                        count = 0;
                        validator.EndAutoTrackNo();
                        workingMgr.Dispose();
                        workingMgr = new InvoiceManager();
                        validator = new GoogleInvoiceRootInvoiceValidator(workingMgr, owner.Organization)
                        {
                            UseDefaultCrossBorderMerchantCarrier = true,
                        };
                        validator.StartAutoTrackNo();
                    }

                    try
                    {
                        var invItem = item.Invoice[idx];

                        InvoiceItem newItem = validator.SaveRootInvoice(invItem, forTerms, InvoiceClientID, ChannelID, out Exception ex);
                        if (countInfo)
                        {
                            Console.Write("+");
                        }

                        if (ex != null)
                        {
                            if (IgnoreDuplicateDataNumberException && (ex is DuplicateDataNumberException))
                            {
                                newItem = ((DuplicateDataNumberException)ex).CurrentPO.InvoiceItem;
                                if (newItem == null)
                                {
                                    result.Add(idx, ex);
                                    continue;
                                }
                            }
                            else
                            {
                                result.Add(idx, ex);
                                continue;
                            }
                        }

                        eventItems.Add(newItem);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }
                validator.EndAutoTrackNo();
                workingMgr.Dispose();

                if (eventItems.Count > 0)
                {
                    HasItem = true;
                    if (forTerms)
                    {
                        GoogleInvoiceExtensionMethods.MatchGoogleInvoiceAttachment();
                    }
                }
                EventItems = eventItems;
            }

            return result;
        }

        public override Dictionary<int, Exception> SaveUploadAllowance(AllowanceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if(item!=null && item.Allowance!=null&&item.Allowance.Length>0)
            {
                AllowanceRootAllowanceValidator validator = new AllowanceRootAllowanceValidator(this, owner.Organization);
                var table = this.GetTable<InvoiceAllowance>();

                this.EventItems_Allowance = null;
                List<InvoiceAllowance> eventItems = new List<InvoiceAllowance>();

                for(int idx = 0;idx < item.Allowance.Length;idx++)
                {
                    try
                    {
                        var allowanceItem = item.Allowance[idx];
                        Exception ex;
                        if ((ex = validator.Validate(allowanceItem)) != null)
                        {
                            result.Add(idx, ex);
                            continue;
                        }

                        InvoiceAllowance newItem = validator.Allowance;
                        table.InsertOnSubmit(newItem);
                        D0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                        D0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);

                        this.SubmitChanges();

                        eventItems.Add(newItem);

                    }
                    catch(Exception ex)
                    {
                        Logger.Error(ex);
                        result.Add(idx, ex);
                    }
                }

                if (eventItems.Count > 0)
                {
                    HasItem = true;
                }

                EventItems_Allowance = eventItems;

            }

            return result;
        }
    }
}
