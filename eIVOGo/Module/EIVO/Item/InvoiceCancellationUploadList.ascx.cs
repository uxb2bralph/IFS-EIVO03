﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Locale;
using Model.Security.MembershipManagement;
using Model.InvoiceManagement;
using Business.Helper;
using Model.DataEntity;
using Uxnet.Web.Module.Common;
using System.Linq.Expressions;
using Utility;

namespace eIVOGo.Module.EIVO.Item
{
    public partial class InvoiceCancellationUploadList : InvoiceUploadList
    {

        protected override void bindData()
        {
            if (_allowPaging)
            {
                if (_mgr.IsValid)
                {
                    gvEntity.DataSource = ((GoogleInvoiceCancellationUploadManager)_mgr).ItemList.Skip(pagingList.CurrentPageIndex * pagingList.PageSize).Take(pagingList.PageSize);
                }
                else
                {
                    gvEntity.DataSource = ((GoogleInvoiceCancellationUploadManager)_mgr).ErrorList.Skip(pagingList.CurrentPageIndex * pagingList.PageSize).Take(pagingList.PageSize);
                }
                pagingList.Visible = true;
            }
            else
            {
                if (_mgr.IsValid)
                {
                    gvEntity.DataSource = ((GoogleInvoiceCancellationUploadManager)_mgr).ItemList;
                }
                else
                {
                    gvEntity.DataSource = ((GoogleInvoiceCancellationUploadManager)_mgr).ErrorList;
                }
                pagingList.Visible = false;
            }
            gvEntity.DataBind();
            _totalRecordCount = _mgr.IsValid ? ((GoogleInvoiceCancellationUploadManager)_mgr).ItemList.Count : ((GoogleInvoiceCancellationUploadManager)_mgr).ErrorList.Count;
            pagingList.RecordCount = _totalRecordCount.Value;
        }
        
    }    
}