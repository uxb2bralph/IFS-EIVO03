﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Schema.EIVO" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<%= _model.InvoiceSeller.CustomerName %>
已開出您本期發票，電子發票系統已自動協助您完成接收發票作業，<br />
請於附件(PDF)列印本期的電子發票，如附件列印有問題再麻煩請您登入<br />
『電子發票服務平台』進行後續列印作業，登入電子發票網址 <a href="https://eivo.uxifs.com">https://eivo.uxifs.com</a>。<br />
<%  if (_model.InvoiceSeller.ReceiptNo == "70762419")
    {   %>
<br />
<br />
有關付款作業，請依照以下指示辦理，再次感謝您的配合。 
    <br />
一、採用銀行電匯款方式： 
    <br />
匯款人：  <%= _model.InvoiceBuyer.CustomerName %>
<br />
受款人：  <%= _model.InvoiceSeller.CustomerName %>
<br />
受款銀行：台北富邦南門分行(012-5103) 
    <br />
受款帳號：510102019561 
    <br />
二、採用支票付款方式： 
    <br />
採用支票付款之公司行號，請開立支票掛號郵寄， 
    <br />
台北市中正區南海路20號6樓管理部收。 
    <br />
<br />
網際優勢－電子發票服務平台感謝您。。。。 
    <br />
<br />
為提昇 貴公司與本公司發票作業效率，依財政部「電子發票實施作業要點」將全面實施使用 
    <br />
網際網路傳輸統一發票（簡稱「電子發票」），系統會主動寄發電子發票檔至貴單位e_Mail信箱， 
    <br />
不再郵寄傳統紙本發票。
    <br />
報稅方面，若 貴公司是人工申報，請列印電子發票申報；若是媒體申報，則沒有任何改變。
<%  }
    else if (_model.Organization.EnterpriseGroupMember.Any(g => g.EnterpriseID == 1))
    {%>
<br />
<br />
致太平洋崇光百貨股份有限公司各設櫃廠商：
    <br />
<br />
本公司會計部辦公室於2015年7月6日已搬遷至芙蓉大樓，
    <br />
各項相關事宜聯絡請至新址洽辦，感謝配合(含請款憑證郵寄)，
    <br />
不便之處，敬請見諒。
    <br />
新 址：106臺北市大安區仁愛路三段136號14樓(芙蓉大樓)
    <br />
電 話：(02)7713-5555 分機8667、8668、8669  
    <br />
傳 真 機：(02)2706-1521
    <br />
謹頌 商祺
    <br />
太平洋崇光百貨股份有限公司 會計部
    
<%  } %>
<br />
<br />
配合財政部B2B電子發票證明聯新格式法規規定，系統將於2/4日上版，屆時各會員所列印的電子發票即為B2B電子發票證明聯新格式，特此公告。
<br />
<br />
信件寄送時間：<%= DateTime.Now %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InvoiceItem _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceItem)this.Model;

    }

</script>


