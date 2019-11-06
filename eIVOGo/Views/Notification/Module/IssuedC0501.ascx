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

<!DOCTYPE html>
<html>
<head>
    <style type="text/css">
        body {
            margin: 10px;
            padding: 0px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
            background: url('../images/sampleOnly.png');
            background-repeat: no-repeat;
            background-position: center;
        }

        p {
            margin: 5px;
            padding: 0px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }

        a:link {
            margin-left: 5px;
            margin-right: 5px;
            font-size: 12px; /*color:#0C419A;*/
            color: #FF6600;
            text-decoration: none;
            border-bottom: 1px dotted #666666;
        }

        a:hover {
            color: #0066CC;
            text-decoration: none;
            border-bottom: 1px solid #666666;
        }

        div.border_gray {
            border: 4px solid #DDD;
            /*background: #FFFFFF;*/
            margin: 10px 0px;
            padding: 10px;
        }

        .left_title {
            font-family: sans-serif,Geneva,Arial,Helvetica;
            border-top: 1px solid #DEB887;
            border-left: 1px solid #F5DEB3;
            border-bottom: 1px solid #F5DEB3;
            margin: 0px;
            color: #666666;
        }

            .left_title th {
                padding: 3px 5px;
                border-top: 1px solid #FFFFFF;
                font-size: 12px;
                font-weight: normal;
                line-height: 160%;
                color: #A0522D;
                background-color: #F5DEB3;
                text-align: right;
            }

                .left_title th.bordertop {
                    border-top-width: 0px;
                }

            .left_title td {
                padding: 3px 5px;
                border-right: 1px solid #F5DEB3;
                border-top: 1px solid #F5DEB3;
                font-size: 12px;
                line-height: 160%;
            }

                .left_title td.tdright {
                    text-align: right;
                }

                .left_title td.tdleft {
                    text-align: left;
                }
        /*--明細列視窗--*/
        .table01 {
            font-family: sans-serif,Geneva,Arial,Helvetica;
            border-top: 1px solid #DEB887;
            border-right: 1px solid #DDDDDD;
            margin: 0px;
            color: #666666;
        }

            .table01 th {
                padding: 3px 5px;
                border-left: 1px solid #FFF;
                border-bottom: 1px solid #FFF;
                font-size: 12px;
                font-weight: normal;
                line-height: 160%;
                color: #FFFFFF;
                background-color: #c99040;
            }

                .table01 th.borderleft {
                    padding: 3px 5px;
                    border-left: 1px solid #DEB887;
                    font-size: 12px;
                    font-weight: normal;
                    line-height: 160%;
                    color: #FFFFFF;
                    background-color: #c99040;
                }

            .table01 td {
                padding: 3px 5px;
                border-left: 1px solid #DDDDDD;
                border-bottom: 1px solid #DDDDDD;
                font-size: 12px;
                line-height: 160%;
            }

                .table01 td.tdright {
                    text-align: right;
                }

                .table01 td.tdleft {
                    text-align: left;
                }

        .OldLace {
            background-color: #FDF5E6;
        }

        .Head_style_a {
            font-size: 15px;
            color: #A0522D;
            padding-top: 5px 2px 5px 2px;
            letter-spacing: 1px;
        }

        .blue {
            color: #0066CC;
        }

        .red {
            color: #e60012;
        }

        .printBtn a {
            display: block;
            letter-spacing: 1px;
            width: 200px;
            height: 30px;
            line-height: 30px;
            font-size: 15px;
            font-weight: 700;
            text-align: center;
            vertical-align: middle;
            color: #447691;
            border-radius: 5px;
            background-color: #8ad3f3;
            text-decoration: none;
            border-left: 1px solid #489c9e;
            border-bottom: 1px solid #489c9e;
            border-top: 1px solid #b8f1f8;
            border-right: 1px solid #b8f1f8;
            margin-top: 15px;
            margin-bottom: 15px;
        }

            .printBtn a:hover {
                color: #084259;
                background-color: #8cd6f3;
                text-decoration: none;
                border-top: 1px solid #489c9e;
                border-right: 1px solid #489c9e;
                border-left: 1px solid #b8f1f8;
                border-bottom: 1px solid #b8f1f8;
                box-shadow: -1px 1px 2px #888888;
            }
    </style>
    <title>電子發票系統</title>
</head>
<body>
    <form>
        <!--交易畫面標題-->
        <%  if (!_model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.UseCustomStyle))
            {   %>
        <p>
            您的客戶編號：<span class="blue"><%= _model.InvoiceBuyer.CustomerID.StringMask(4, 3, '*')%></span> 發票已經作廢，以下為您的作廢電子發票內容：
        </p>
        <%  }   %>
        <div class="border_gray" style="width:100%">
            <!--表格 開始-->
            <%  Html.RenderPartial("~/Views/Notification/Module/C0501MailView.ascx", _model); %>
            <!--表格 結束-->
        </div>
        <p>
            此張發票已由營業人上傳作廢，敬請確認。
        </p>
        <%  Html.RenderPartial("~/Views/Notification/Shared/ServiceNotation.cshtml",_model.Organization); %>
        <!--按鈕-->
    </form>
</body>
</html>

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

