﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement.enUS;
using Model.InvoiceManagement.InvoiceProcess;
using Model.InvoiceManagement.Validator;
using Model.Locale;
using Model.Properties;
using Model.Schema.EIVO;
using Utility;

namespace Model.InvoiceManagement
{
    public partial class GooglePlayInvoiceManager : GoogleInvoiceManagerV2
    {

        public GooglePlayInvoiceManager()
            : base()
        {

        }
        public GooglePlayInvoiceManager(GenericManager<EIVOEntityDataContext> mgr)
            : base(mgr)
        {

        }

        public override Dictionary<int, Exception> SaveUploadAllowance(AllowanceRoot item, OrganizationToken owner)
        {
            Dictionary<int, Exception> result = new Dictionary<int, Exception>();
            if (item != null && item.Allowance != null && item.Allowance.Length > 0)
            {
                GooglePlayAllowanceValidator validator = new GooglePlayAllowanceValidator(this, owner.Organization);
                var table = this.GetTable<InvoiceAllowance>();

                this.EventItems_Allowance = null;
                List<InvoiceAllowance> eventItems = new List<InvoiceAllowance>();

                for (int idx = 0; idx < item.Allowance.Length; idx++)
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
                        newItem.CDS_Document.DocumentOwner.ClientID = this.InvoiceClientID;
                        newItem.CDS_Document.ChannelID = this.ChannelID;
                        table.InsertOnSubmit(newItem);
                        D0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已開立);
                        //D0401Handler.PushStepQueueOnSubmit(this, newItem.CDS_Document, Naming.InvoiceStepDefinition.已接收資料待通知);
                        newItem.CDS_Document.DocumentSubscriptionQueue = new DocumentSubscriptionQueue { };
                        this.SubmitChanges();

                        eventItems.Add(newItem);

                    }
                    catch (Exception ex)
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
