﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<!--交易畫面標題-->
<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "新增/修改發票號碼區間"); %>

<div class="border_gray" id="queryArea">
    <!--表格 開始-->
    <table class="left_title" border="0" cellspacing="0" cellpadding="0" width="100%">
        <tbody>
            <tr>
                <th class="Head_style_a" colspan="2">
                    查詢條件
                </th>
            </tr>
            <tr>
                <th width="20%" nowrap>
                    發票年度（民國年）
                </th>
                <td class="tdleft">
                    <select name="year">
                    <%  for (int yy = 2012; yy <= DateTime.Now.Year + 1; yy++)
                        { %>
                            <option value="<%= yy %>"><%= yy-1911 %></option>
                    <%  } %>
                    </select>
                    <script>
                        $(function () {
                            $('select[name="year"]').val(<%= DateTime.Now.Year %>);
                        });
                    </script>
                </td>
            </tr>
            <tr>
                <th>
                    發票期別
                </th>
                <td class="tdleft">
                    <select name="periodNo">
                        <option value="">全部</option>
                        <option value="1">01-02月</option>
                        <option value="2">03-04月</option>
                        <option value="3">05-06月</option>
                        <option value="4">07-08月</option>
                        <option value="5">09-10月</option>
                        <option value="6">11-12月</option>
                    </select>
                    <script>
                        $(function () {
                            $('select[name="periodNo"]').val(<%= (DateTime.Now.Month+1)/2 %>);
                        });
                    </script>
                </td>
            </tr>
            <tr>
                <th>
                    開立人統編
                </th>
                <td class="tdleft">
                    <%  Html.RenderPartial("~/Views/DataFlow/SellerSelector.cshtml", _profile.InitializeOrganizationQuery(models)); %>                
                </td>
            </tr>
        </tbody>
    </table>

    <!--表格 結束-->
</div>
<!--按鈕-->
<table border="0" cellspacing="0" cellpadding="0" width="100%" class="queryAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <button type="button" onclick="uiTrackCodeNo.inquire();" >查詢</button>
            </td>
        </tr>
    </tbody>
</table>
<!--表格 開始-->


<script>
    var uiTrackCodeNo;
    $(function () {
        uiTrackCodeNo = {
            $result: null,
            commitItem: function (keyID) {
                var event = event || window.event;
                var $tr = $(event.target).closest('tr');
                var $formData = $tr.find('input,select,textarea').serializeObject();
                $formData.keyID = keyID;
                clearErrors();
                $.post('<%= Url.Action("CommitItem","InvoiceNo") %>', $formData, function (data) {
                    if (data) {
                        var $data = $(data);
                        if ($data.is('tr')) {
                            $tr.before($data);
                            if (keyID) {
                                $tr.remove();
                                alert('資料已更新!!');
                            }
                        } else {
                            $('body').append($data);
                            $data.remove();
                        }
                    }
                });
            },
            inquire: function (pageNum, onPaging) {
                var event = event || window.event;
                var $form = $('#queryArea').closest('form');
                $form.ajaxForm({
                    url: "<%= Url.Action("InquireInterval","InvoiceNo") %>",
                    beforeSubmit: function () {
                        showLoading();
                    },
                    success: function (data) {
                        if (data) {
                            if (uiTrackCodeNo.$result)
                                uiTrackCodeNo.$result.remove();
                            uiTrackCodeNo.$result = $(data);
                            $('.queryAction').after(uiTrackCodeNo.$result);
                        }
                        hideLoading();
                    },
                    error: function () {
                        hideLoading();
                    }
                }).submit();
            },
            editItem: function (keyID) {
                var event = event || window.event;
                var $tr = $(event.target).closest('tr');
                $.post('<%= Url.Action("EditNoInterval","InvoiceNo") %>', { 'keyID': keyID }, function (data) {
                    if (data) {
                        var $data = $(data);
                        if ($data.is('tr')) {
                            $tr.before($data);
                            $tr.remove();
                        } else {
                            $('body').append($data);
                            $data.remove();
                        }
                    }
                });
            },
            deleteItem: function (keyID) {
                if (confirm('確定刪除此筆資料?')) {
                    var event = event || window.event;
                    var $tr = $(event.target).closest('tr');
                    $.post('<%= Url.Action("DeleteNoInterval","InvoiceNo") %>', { 'keyID': keyID }, function (data) {
                        if (data.result) {
                            alert('資料已刪除!!')
                            $tr.remove();
                        } else {
                            alert(data.message);
                        }
                    });
                }
            },
            showItem: function (keyID) {
                var event = event || window.event;
                var $tr = $(event.target).closest('tr');
                $.post('<%= Url.Action("IntervalItem","InvoiceNo") %>', { 'keyID': keyID } , function (data) {
                    if (data) {
                        var $data = $(data);
                        if ($data.is('tr')) {
                            $tr.before($data);
                            $tr.remove();
                        } else {
                            $('body').append($data);
                            $data.remove();
                        }
                    }
                });
            },
            splitItem: function (keyID) {
                if (confirm('確定分割此配號區間?')) {
                    var event = event || window.event;
                    var $tr = $(event.target).closest('tr');
                    $.post('<%= Url.Action("SplitNoInterval","InvoiceNo") %>', { 'keyID': keyID }, function (data) {
                        if (data.result) {
                            alert('配號區間已分割!!')
                            uiTrackCodeNo.inquire();
                        } else {
                            if (data.message) {
                                alert(data.message);
                            }
                        }
                    });
                }
            },
            assignPOSBooklets: function (value) {
                $.post('<%= Url.Action("EditPOSBooklets","InvoiceNo") %>', { 'intervalID': value }, function (data) {
                    $global.createTab('editPOSInterval', 'POS本組數配置', data, true);
                });
            },
            allotInterval: function (keyID) {
                showLoading();
                $.post('<%= Url.Action("AllotInterval","InvoiceNo") %>', { 'keyID': keyID }, function (data) {
                    hideLoading();
                    if ($.isPlainObject(data)) {
                        alert(data.message);
                    } else {
                        $(data).appendTo($('body'));
                    }
                });
            },

        };
    });
</script>
<script runat="server">

    Model.Security.MembershipManagement.UserProfileMember _profile;
    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _profile = Business.Helper.WebPageUtility.UserProfile;
    }
</script>
