﻿<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Model.DataEntity" %>
<%# ((CDS_Document)((GridViewRow)Container).DataItem).InvoiceItem.CDS_Document.SMSNotificationLogs.Any() ? "是" : "否"%>
