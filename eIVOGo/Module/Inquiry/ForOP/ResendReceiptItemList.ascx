<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.Base.ReceiptItemList" %>
<%@ Register Src="~/Module/Common/PagingControl.ascx" TagName="PagingControl" TagPrefix="uc2" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.InvoiceManagement" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/Common/ActionHandler.ascx" TagName="ActionHandler" TagPrefix="uc1" %>
<%@ Register Src="~/Module/EIVO/Action/ReceiptItemCommonView.ascx" TagName="ReceiptItemCommonView" TagPrefix="uc6" %>
<%@ Register Src="~/Module/Entity/OrganizationItem.ascx" TagName="OrganizationItem" TagPrefix="uc3" %>
<div class="border_gray">
    <asp:GridView ID="gvEntity" runat="server" AutoGenerateColumns="False" Width="100%"
        GridLines="None" CellPadding="0" CssClass="table01" AllowPaging="True" EnableViewState="False"
        DataSourceID="dsEntity" ShowFooter="True">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <input id="chkAll" name="chkAll" type="checkbox" onclick="$('input[id$=\'chkItem\']').attr('checked',$('input[id$=\'chkAll\']').is(':checked'));" />
                </HeaderTemplate>
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).DocumentPrintQueue == null ? String.Format("<input id='chkItem' name='chkItem' type='checkbox' value='{0}' />", ((CDS_Document)Container.DataItem).DocID) : "<font color='red'>已送出列印</font>"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="日期">
                <ItemTemplate>
                    <%# ValueValidity.ConvertChineseDateString(((CDS_Document)Container.DataItem).ReceiptItem.ReceiptDate)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="開立收據營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).ReceiptItem.Seller.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.Seller.CompanyName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="接收收據營業人">
                <ItemTemplate>
                    <a href="#" onclick="<%# doDisplayCompany.GetPostBackEventReference(((CDS_Document)Container.DataItem).ReceiptItem.Buyer.CompanyID.ToString()) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.Buyer.CompanyName %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據號碼">
                <ItemTemplate>
                    <a href="#" onclick="<%# getInvoiceItemView((CDS_Document)Container.DataItem) %>">
                        <%# ((CDS_Document)Container.DataItem).ReceiptItem.No %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    共<%# _totalRecordCount %>筆</FooterTemplate>
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="對帳單">
                <ItemTemplate>
                    <a href="#" onclick="<%# getStatement((CDS_Document)Container.DataItem) %>">
                        <asp:Image ID="Image1" BorderStyle="None" ImageUrl="~/images/icon_ca.gif" runat="server" Visible="<%# ((CDS_Document)Container.DataItem).Attachment.Count<=0 ? false : true %>" />
                    </a>
                </ItemTemplate>                
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="金額" FooterText="總計金額">
                <ItemTemplate>
                    <%#String.Format("{0:#,0}", ((CDS_Document)Container.DataItem).ReceiptItem.TotalAmount)%></ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
                <FooterStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="收據狀態">
                <ItemTemplate>
                    <asp:Label ID="status" runat="server" Text="<%# checkStatus((Naming.B2BInvoiceStepDefinition)((CDS_Document)Container.DataItem).ReceiptItem.CDS_Document.CurrentStep) %>"
                        EnableViewState="false" ></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
                <FooterTemplate>
                    <%# String.Format("{0:##,###,###,##0.##}", _subtotal) %></FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="列印狀態">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).DocumentPrintLog.Any(l => l.TypeID == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Receipt) ? "已列印" : "未列印"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="主動列印">
                <ItemTemplate>
                    <%# ((CDS_Document)Container.DataItem).ReceiptItem.Buyer.OrganizationStatus.EntrustToPrint == null ? "否" : (bool)((CDS_Document)Container.DataItem).ReceiptItem.Buyer.OrganizationStatus.EntrustToPrint ? "是" : "否"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
        <FooterStyle CssClass="total-count" />
        <PagerStyle HorizontalAlign="Center" />
        <SelectedRowStyle />
        <HeaderStyle />
        <AlternatingRowStyle CssClass="OldLace" />
        <PagerTemplate>
            <uc2:PagingControl ID="pagingList" runat="server" OnPageIndexChanged="PageIndexChanged" />
        </PagerTemplate>
        <RowStyle />
        <EditRowStyle />
    </asp:GridView>
    <table border="0" cellspacing="0" cellpadding="0" width="100%" runat="server" visible="false"
        enableviewstate="false" id="tblAction">
        <tbody>
            <tr>
                <td class="Bargain_btn">
                    <asp:Button ID="btnShow" runat="server" Text="重送郵件通知" OnClick="btnShow_Click" />&nbsp;&nbsp;
                </td>
            </tr>
        </tbody>
    </table>
</div>
<cc1:DocumentDataSource ID="dsEntity" runat="server">
</cc1:DocumentDataSource>
<uc1:ActionHandler ID="doDisplayReceipt" runat="server" />
<uc1:ActionHandler ID="doDisplayCompany" runat="server" />
<uc1:ActionHandler ID="doDisplayStatement" runat="server" />
<uc6:ReceiptItemCommonView  ID="printReceipt" runat="server" />
<uc3:OrganizationItem ID="orgView" runat="server" Visible="false" />
<script runat="server">

    private Model.Security.MembershipManagement.UserProfileMember _userProfile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        doDisplayReceipt.DoAction = arg =>
        {
            printReceipt.QueryExpr = i => i.ReceiptID == int.Parse(arg);
            printReceipt.BindData();
        };

        doDisplayCompany.DoAction = arg =>
            {
                orgView.QueryExpr = o => o.CompanyID == int.Parse(arg);
                orgView.BindData();
            };

        doDisplayStatement.DoAction = arg =>
        {
            //Response.WriteFileAsDownload(arg, String.Format("{0:yyyy-MM-dd}.pdf", DateTime.Today), false);
            Response.WriteFileAsDownload(arg);
        };

        _userProfile = Business.Helper.WebPageUtility.UserProfile;
    }

    private String getInvoiceItemView(CDS_Document dataItem)
    {
        return dataItem.CurrentStep != (int)Naming.B2BInvoiceStepDefinition.待開立 ?
                                                            doDisplayReceipt.GetPostBackEventReference(dataItem.DocID.ToString())
                                        : "alert('尚未開立!!');return false;";
    }

    private String getStatement(CDS_Document dataItem)
    {
        return dataItem.Attachment.Count <= 0 ? "return false;"
            : File.Exists(dataItem.Attachment.FirstOrDefault().StoredPath) ? doDisplayStatement.GetPostBackEventReference(dataItem.Attachment.FirstOrDefault().StoredPath) : "alert('對帳單檔案遺失!!');return false;";
    }

    private string checkStatus(Naming.B2BInvoiceStepDefinition step)
    {
        switch (step)
        {
            case Naming.B2BInvoiceStepDefinition.待接收:
                return (Naming.B2BInvoiceStepDefinition.待接收).ToString();
            case Naming.B2BInvoiceStepDefinition.待開立:
                return (Naming.B2BInvoiceStepDefinition.待開立).ToString();
            case Naming.B2BInvoiceStepDefinition.待傳送:
                return "已接收";
            case Naming.B2BInvoiceStepDefinition.已接收:
                return (Naming.B2BInvoiceStepDefinition.已接收).ToString();
            case Naming.B2BInvoiceStepDefinition.已開立:
                return (Naming.B2BInvoiceStepDefinition.已開立).ToString();
            case Naming.B2BInvoiceStepDefinition.已傳送:
                return "已接收";
            default:
                return "";
        }
    }
    protected void btnShow_Click(object sender, EventArgs e)
    {
        String[] ar = GetItemSelection();
        if (ar != null && ar.Count() > 0)
        {
            List<int> InvoiceID = new List<int>();

            foreach (var id in ar.Select(a => int.Parse(a)))
            {
                var mgr = dsEntity.CreateDataManager();
                var item = mgr.GetTable<CDS_Document>().Where(i => i.DocID == id);//.FirstOrDefault();

             //   InvoiceID.Add(item.FirstOrDefault().InvoiceItem.InvoiceID);
                var org = mgr.GetTable<Organization>();
                switch (item.FirstOrDefault().CurrentStep)
                {
                    case (int)Naming.B2BInvoiceStepDefinition.待開立:
                        // case (int)Naming.B2BInvoiceStepDefinition.已開立:
                        var notifyToIssue = item.Select(t => new NotifyToProcessID { MailToID = t.InvoiceItem.InvoiceSeller.SellerID, Seller = t.InvoiceItem.InvoiceSeller.Organization })
                               .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.DerivedDocument.ParentDocument.InvoiceItem.InvoiceSeller.SellerID, Seller = t.DerivedDocument.ParentDocument.InvoiceItem.InvoiceSeller.Organization }))
                               .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID, Seller = t.InvoiceAllowance.InvoiceAllowanceSeller.Organization }))
                               .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID, Seller = t.DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceSeller.Organization }))
                               .Concat(item.Select(t => new NotifyToProcessID { MailToID = (int?)t.ReceiptItem.SellerID, Seller = t.ReceiptItem.Seller }))
                               .Concat(item.Select(t => new NotifyToProcessID { MailToID = (int?)t.DerivedDocument.ParentDocument.ReceiptItem.SellerID, Seller = t.DerivedDocument.ParentDocument.ReceiptItem.Seller }))
                               .Distinct();
                        foreach (var businessID in notifyToIssue)
                        {
                            var orgitem = org.Where(o => o.CompanyID == businessID.MailToID).FirstOrDefault();
                            if (orgitem != null && (orgitem.OrganizationStatus == null || orgitem.OrganizationStatus.Entrusting != true))
                            {
                                EIVOPlatformFactory.NotifyToIssueItem(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                            }
                        }

                        break;
                    case (int)Naming.B2BInvoiceStepDefinition.待接收:
                        var notifyToReceive = item.Select(t => new NotifyToProcessID { MailToID = t.InvoiceItem.InvoiceBuyer.BuyerID, Seller = t.InvoiceItem.InvoiceSeller.Organization })
                            .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.DerivedDocument.ParentDocument.InvoiceItem.InvoiceBuyer.BuyerID, Seller = t.DerivedDocument.ParentDocument.InvoiceItem.InvoiceSeller.Organization }))
                            .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.InvoiceAllowance.InvoiceAllowanceSeller.SellerID, Seller = t.InvoiceAllowance.InvoiceAllowanceSeller.Organization }))
                            .Concat(item.Select(t => new NotifyToProcessID { MailToID = t.DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceSeller.SellerID, Seller = t.DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceSeller.Organization }))
                            .Concat(item.Select(t => new NotifyToProcessID { MailToID = (int?)t.ReceiptItem.BuyerID, Seller = t.ReceiptItem.Seller }))
                            .Concat(item.Select(t => new NotifyToProcessID { MailToID = (int?)t.DerivedDocument.ParentDocument.ReceiptItem.BuyerID, Seller = t.DerivedDocument.ParentDocument.ReceiptItem.Seller }))
                            .Distinct();

                        foreach (var businessID in notifyToReceive)
                        {
                            var orgitem = org.Where(o => o.CompanyID == businessID.MailToID).FirstOrDefault();
                            if (orgitem != null && (orgitem.OrganizationStatus == null || orgitem.OrganizationStatus.Entrusting != true))
                            {
                                EIVOPlatformFactory.NotifyToReceiveItem(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                            }
                        }
                        break;
                    case (int)Naming.B2BInvoiceStepDefinition.已接收:
                        switch ((Naming.B2BInvoiceDocumentTypeDefinition)item.FirstOrDefault().DocType.Value)
                        {
                            case Naming.B2BInvoiceDocumentTypeDefinition.電子發票:
                                if (item.FirstOrDefault().InvoiceItem.InvoiceBuyer.Organization.OrganizationStatus.Entrusting == true)
                                    EIVOPlatformFactory.NotifyCommissionedToReceiveInvoice(this, new EventArgs<NotifyMailInfo> { Argument = new NotifyMailInfo { InvoiceItem = item.FirstOrDefault().InvoiceItem, isMail = true } });
                                break;
                            case Naming.B2BInvoiceDocumentTypeDefinition.發票折讓:
                                if (item.FirstOrDefault().InvoiceAllowance.InvoiceAllowanceSeller.Organization.OrganizationStatus.Entrusting == true)
                                {
                                    var businessID = new NotifyToProcessID { MailToID = item.FirstOrDefault().InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID, Seller = item.FirstOrDefault().InvoiceAllowance.InvoiceAllowanceSeller.Organization };
                                    EIVOPlatformFactory.NotifyCommissionedToReceive(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                                }
                                break;
                            case Naming.B2BInvoiceDocumentTypeDefinition.作廢發票:
                                if (item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceItem.InvoiceBuyer.Organization.OrganizationStatus.Entrusting == true)
                                {
                                    var businessID = new NotifyToProcessID { MailToID = item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceItem.InvoiceBuyer.BuyerID, Seller = item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceItem.InvoiceSeller.Organization };
                                    EIVOPlatformFactory.NotifyCommissionedToReceive(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                                }
                                break;
                            case Naming.B2BInvoiceDocumentTypeDefinition.作廢折讓:
                                if (item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceSeller.Organization.OrganizationStatus.Entrusting == true)
                                {
                                    var businessID = new NotifyToProcessID { MailToID = item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID, Seller = item.FirstOrDefault().DerivedDocument.ParentDocument.InvoiceAllowance.InvoiceAllowanceSeller.Organization };
                                    EIVOPlatformFactory.NotifyCommissionedToReceive(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                                }
                                break;
                            case Naming.B2BInvoiceDocumentTypeDefinition.收據:
                                if (item.FirstOrDefault().ReceiptItem != null && item.FirstOrDefault().ReceiptItem.Buyer.OrganizationStatus.Entrusting == true)
                                {
                                    var businessID = new NotifyToProcessID { MailToID = item.FirstOrDefault().ReceiptItem.BuyerID, Seller = item.FirstOrDefault().ReceiptItem.Seller };
                                    EIVOPlatformFactory.NotifyCommissionedToReceive(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                                }
                                break;
                            case Naming.B2BInvoiceDocumentTypeDefinition.作廢收據:
                                if (item.FirstOrDefault().DerivedDocument.ParentDocument.ReceiptItem.Buyer.OrganizationStatus.Entrusting == true)
                                {
                                    var businessID = new NotifyToProcessID { MailToID = item.FirstOrDefault().DerivedDocument.ParentDocument.ReceiptItem.BuyerID, Seller = item.FirstOrDefault().DerivedDocument.ParentDocument.ReceiptItem.Seller };
                                    EIVOPlatformFactory.NotifyCommissionedToReceive(this, new EventArgs<NotifyToProcessID> { Argument = businessID });
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }

            }
            //傳送信件


            this.AjaxAlert("Email通知已重送!!");
            gvEntity.DataBind();
        }
        else
        {
            this.AjaxAlert("請選擇重送資料!!");
        }
    }
    public String[] GetItemSelection()
    {
        return Request.Form.GetValues("chkItem");
    }
</script>
