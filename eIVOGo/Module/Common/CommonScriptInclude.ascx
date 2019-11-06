﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery-1.11.3.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery-ui-1.11.3.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/bootstrap.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery.ui.datepicker-zh-TW.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery.form.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/uxeivo.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/rwd-table.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery.twbsPagination.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/vendor/metisMenu/metisMenu.min.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/sb-admin-2.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/math.min.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery.blockUI.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/jquery.cookie.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/linq.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/linq.jquery.js") %>"></script>
<script type="text/javascript" src="<%=VirtualPathUtility.ToAbsolute("~/Scripts/stringformat-1.11.min.js") %>"></script>

<script>

    var $global = (function () {

        return {
            registerCloseEvent: function ($tab) {
                $tab.find(".closeTab").click(function () {

                    //there are multiple elements which has .closeTab icon so close the tab whose close icon is clicked
                    var tabContentId = $(this).parent().attr("href");
                    $(this).parent().parent().remove(); //remove li of tab
                    $('#masterTab a:last').tab('show'); // Select first tab
                    $(tabContentId).remove(); //remove respective tab content

                });
            },
            removeTab: function (tabId) {
                var $a = $('#masterTab a[href="#' + tabId + '"]');
                $a.parent().remove(); //remove li of tab
                $('#masterTab a:last').tab('show'); // Select first tab
                $('#' + tabId).remove(); //remove respective tab content
            },
            showTab: function (tabId) {
                $('#masterTab a[href="#' + tabId + '"]').tab('show');
            },
            createTab: function (tabId, tabText, tabContent, show) {

                this.removeTab(tabId);

                var newTab = $('<li role="presentation"></li>')
                        .append($('<a href="#masterHome" class="tab-link" role="tab" data-toggle="tab"></a>')
                            .attr('href', '#' + tabId).attr('aria-controls', tabId).text(tabText)
                            .append($('<button class="close closeTab"><i class="fa fa-times" aria-hidden="true"></i></button>')));
                newTab.appendTo($('#masterTab'));
                $('<div role="tabpanel" class="tab-pane"></div>').attr('id', tabId)
                    .append(tabContent).appendTo($('#masterTabContent'));
                this.registerCloseEvent(newTab);
                if (show)
                    this.showTab(tabId);
            },
        };
    })();

    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    $.fn.launchDownload = function (url, params, target, loading) {

        var data = this.serializeObject();
        if (params) {
            $.extend(data, params);
        }

        if (loading) {
            token = (new Date()).getTime();
            data.FileDownloadToken = token;
        }

        var form = $('<form></form>').attr('action', url).attr('method', 'post');//.attr('target', '_blank');
        if (target) {
            form.attr('target', target);
            if (window.frames[target] == null) {
                $('<iframe>')
                    .css('display', 'none')
                    .attr('name', target).appendTo($('body'));
            }
        }

        Object.keys(data).forEach(function (key) {
            var value = data[key];

            if (value instanceof Array) {
                value.forEach(function (v) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', key).attr('value', v));
                });
            } else {
                form.append($("<input></input>").attr('type', 'hidden').attr('name', key).attr('value', value));
            }

        });

        if (loading) {
            showLoading();
            fileDownloadCheckTimer = window.setInterval(function () {
                var cookieValue = $.cookie('FileDownloadToken');
                if (cookieValue == token)
                    finishDownload();
            }, 1000);
        }

        //send request
        form.appendTo('body').submit().remove();
    };

    function finishDownload() {
        window.clearInterval(fileDownloadCheckTimer);
        $.removeCookie('fileDownloadToken'); //clears this cookie value
        hideLoading();
    }



    function showLoading(autoHide,onBlock) {
        $.blockUI({
            message:  '<img src="<%= VirtualPathUtility.ToAbsolute("~/images/loading.gif") %>" /><h1>Loading</h1>', 
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            },
            // 背景圖層
            overlayCSS:  { 
                backgroundColor: '#3276B1', 
                opacity:         0.6, 
                cursor:          'wait' 
            },
            onBlock: onBlock
        });

        if(autoHide)
            setTimeout($.unblockUI, 5000);
    }

    function hideLoading() {
        $.unblockUI();
    }

    function initSort (sort,offset) {

        $('.itemList th').each(function (idx, elmt) {
            var $this = $(this);
            if(sort.indexOf(idx+offset)>=0) {
                $this.attr('aria-sort', 'ascending');
                $this.append('<i class="fa fa-sort-asc" aria-hidden="true"></i>')
                    .append($('<input type="hidden" name="sort"/>').val(idx+offset));
            } else if(sort.indexOf(-idx-offset)>=0) {
                $this.attr('aria-sort', 'desending');
                $this.append('<i class="fa fa-sort-desc" aria-hidden="true"></i>')
                    .append($('<input type="hidden" name="sort"/>').val(-idx-offset));
            }
        });
    }

    function buildSort(inquire, currentPageIndex, offset) {

        var chkBox = $(".itemList input[name='chkAll']");
        var chkItem = $(".itemList input[name='chkItem']");
        chkBox.click(function () {
            chkItem.prop('checked', chkBox.is(':checked'));
        });

        chkItem.click(function (e) {
            if (!$(this).is(':checked')) {
                chkBox.prop('checked', false);
            }
        });

        $('.itemList th').each(function (idx, elmt) {
            var $this = $(this);
            if (!$this.is('[aria-sort="other"]')) {
                if (!$this.is('[aria-sort]')) {
                    $this.append('<i class="fa fa-sort" aria-hidden="true"></i>')
                        .append('<input type="hidden" name="sort"/>');
                    $this.attr('aria-sort', 'none');
                }
                $this.on('click', function (evt) {
                    var $target = $(this);
                    $target.find('i').remove();
                    if ($target.is('[aria-sort="none"]')) {
                        $target.append('<i class="fa fa-sort-asc" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'ascending');
                        $target.find('input[name="sort"]').val(idx + offset);
                    } else if ($target.is('[aria-sort="ascending"]')) {
                        $target.append('<i class="fa fa-sort-desc" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'descending');
                        $target.find('input[name="sort"]').val(-idx - offset);
                    } else {
                        $target.append('<i class="fa fa-sort" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'none');
                        $target.find('input[name="sort"]').val('');
                    }
                    inquire(currentPageIndex, function (data) {
                        var $node = $('.itemList').next();
                        $('.itemList').remove();
                        $node.before(data);
                    });
                });
            }
        });
    }

    function initSortable($table, inquire, currentPageIndex, sort) {

        $table.find('th').each(function (index, elmt) {
            var $this = $(this);
            var idx = 0;
            if (!$this.is('[aria-sort="other"]')) {
                if (!$this.is('[aria-sort]')) {
                    $this.append('<i class="fa fa-sort" aria-hidden="true"></i>')
                    $this.attr('aria-sort', 'none');
                }

                if ($this.is('[data-sort]')) {
                    idx = $this.attr('data-sort');
                    if (!isNaN(idx)) {
                        idx = Number(idx);
                    } else {
                        idx = index > 0 ? index : 99999;
                    }
                }

                $this.on('click', function (evt) {
                    var $target = $(this);
                    $target.find('i').remove();
                    if ($target.is('[aria-sort="none"]')) {
                        $target.append('<i class="fa fa-sort-asc" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'ascending');
                        var sortIdx = idx + 0;
                        if (sort.indexOf(sortIdx) < 0) {
                            sort.push(sortIdx);
                        }
                    } else if ($target.is('[aria-sort="ascending"]')) {
                        $target.append('<i class="fa fa-sort-desc" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'descending');
                        var sortIdx = idx + 0;
                        var curr = sort.indexOf(sortIdx);
                        if (curr >= 0) {
                            sort[curr] = -sortIdx;
                        }
                    } else {
                        $target.append('<i class="fa fa-sort" aria-hidden="true"></i>');
                        $target.attr('aria-sort', 'none');
                        var sortIdx = -idx - 0;
                        var curr = sort.indexOf(sortIdx);
                        if (curr >= 0) {
                            sort.splice(curr, 1);
                        }
                    }

                    inquire(currentPageIndex);
                });
            }
        });
    }

    function datepicker($items) {
        $items.datepicker({ showButtonPanel: true, changeYear: true, changeMonth: true, yearRange: '2012:+1' });
    }

    $(function () {
        $('input[type="button"]').addClass('btn');
        $('button').addClass('btn');
        //$.datepicker.setDefaults($.datepicker.regional['zh-tw']);
        datepicker($('.form_date'));
    });

    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            if (!this.options.title) {
                title.html("&#160;");
            } else {
                title.html(this.options.title);
            }
        }
    }));

    function uploadFile($file, postData, url, callback, errorback) {

        $('<form method="post" enctype="multipart/form-data"></form>')
            .append($file).ajaxForm({
                url: url,
                data: postData,
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    callback(data);
                },
                error: function () {
                    hideLoading();
                    errorback();
                }
            }).submit();
    }

</script>