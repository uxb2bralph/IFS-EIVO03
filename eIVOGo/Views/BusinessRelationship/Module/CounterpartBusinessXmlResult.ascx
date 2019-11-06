<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc2" %>
<%@ Register Src="~/Module/jQuery/EnumSelector.ascx" TagName="EnumSelector" TagPrefix="uc6" %>
<%@ Register Src="ImportCounterpartBusinessList.ascx" TagName="ImportCounterpartBusinessList"
    TagPrefix="uc3" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="ModelExtension.DataExchange" %>


<table border="0" cellspacing="0" cellpadding="0" width="100%" class="tblAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <%  if (_model.HasError)
                    {   %>
                資料匯入處理有錯誤，請取回處理回應檔查明原因。<br />
                <a href="<%= Url.Action("GetResource","DataExchange",new { path = ((String)ViewBag.ImportFile).EncryptData() }) %>">點此下載回應檔</a>
                <%  }
                    else
                    {   %>
                資料匯入完成!!
                <%  } %>
            </td>
        </tr>
    </tbody>
</table>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    CounterpartBusinessExchange _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        var profile = Context.GetUser();
        _model = (CounterpartBusinessExchange)this.Model;
    }

</script>
