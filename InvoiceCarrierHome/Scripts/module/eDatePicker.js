(function () {
    /**
     * @ngdoc overview
     * @name c.datePicker
     * @description 日曆元件模組。
     */
    var datePickerMod = angular.module("c.datePicker", ['dateUtilMod', 'c.urls']);

    var my97DateConfigConverter = function () {
        return {
            convert: function (config) {
                var mapping = {
                    format: 'dateFmt',
                    startDate: 'minDate',
                    endDate: 'maxDate',
                    initDate: 'startDate'
                };
                var my97Config = $.extend({}, config);
                // 將介面上的 config 轉成 my97DateConfig
                for (var attr in mapping) {
                    if (my97Config[attr]) {
                        my97Config[mapping[attr]] = my97Config[attr];
                        delete my97Config[attr];
                    } else {
                        delete my97Config[attr];
                    }
                }

                /* 假如onpicked有值, 則轉為script.*/
                if (my97Config.onpicked) {
                    my97Config.onpicked = eval(my97Config.onpicked);
                };
                return my97Config;
            }
        };
    };

    datePickerMod.factory('my97DateConfigConverter', [my97DateConfigConverter]);
    datePickerMod.constant('cDatePickerTemplate', '<img class="cDatePicker" src="{{imgSrc}}"/><input type="text" id="cDatePickerShadow" data-ng-hide="true"/>');

    var cDatePicker = function (dateUtil, my97DateConfigConverter, $parse, $compile, $timeout, $interpolate, cDatePickerTemplate, cUrls) {

        var datePickerLink = function (scope, iElement, iAttrs, controller) {

            var keyPressFlag = false;

            // Default設定
            var config = {}, customizeSettings = {}, componentDependConfig = {};

            var buildComponentDependConfig = function () {
                config = $.extend({}, cht.global.datePicker.defaultConfig, cht.global.datePicker.my97DateDefaultConfig);
                customizeSettings = scope.$eval(iAttrs.cDatePicker);
                $.extend(config, customizeSettings);
                // #217167 自動依照 format 調整曆法
                if ((config.format.match(/y/g) || []).length === 4 && config.chrono === "Minguo") {
                    config.chrono = "ISO"
                }
                componentDependConfig = my97DateConfigConverter.convert(config);
            };

            // User設定
            if (customizeSettings) {
                try {
                    buildComponentDependConfig();
                } catch (ex) {
                    window.alert('c-date-picker 輸入參數格式錯誤, name:' + $(iElement).attr('name'));
                }
            }

            // 檢查日曆設定改變，則重新套用新設定
            scope.$watch(iAttrs.cDatePicker, function (newConfig, oldConfig) {
                buildComponentDependConfig();
            }, true);


            var EVENTNAMESPACE = '.datePicker';

            //獨立小日曆模式
            if (customizeSettings.validate === false) {
                iElement.css('display', 'none');

                var imgSrc = cUrls.expandContext('@{/scripts/libs/My97DatePicker/skin/datePicker.gif}');
                var newElement = $compile($interpolate(cDatePickerTemplate)({ imgSrc: imgSrc }))(scope);
                newElement.insertBefore(iElement);

                //調整綁定的 input 為 shadow hidden input
                componentDependConfig.el = 'cDatePickerShadow';
                componentDependConfig.onpicked = function () {
                    scope.$apply(function () {
                        $parse(iAttrs.ngModel).assign(scope, $dp.cal.getDateStr());
                    });
                };

                $('img.cDatePicker').bind('click' + EVENTNAMESPACE, function () {
                    WdatePicker(componentDependConfig);
                });
                return;
            }

            //整合 Input 輸入模式
            var completeEventName = config.trigger + EVENTNAMESPACE,
                datePickerBuilder = function () {
                    //TODO: 未來應開放成 render 介面
                    WdatePicker(componentDependConfig);
                };
            $(iElement).addClass('Wdate')
                .unbind(completeEventName).unbind('click.datePicker').unbind('keydown.datePicker')
                .bind(completeEventName, datePickerBuilder)
                .bind('keydown.datePicker', function () {
                    keyPressFlag = true;
                });

            if ('click' != config.trigger) {
                // 預設在 input 欄位上 click 時，顯示已經隱藏的日期元件
                $(iElement).bind('click.datePicker', datePickerBuilder);
            }

            if (controller) {
                $(iElement).bind('blur', function () {
                    scope.$apply(function () {
                        // 透過此方法更新 ModelValue
                        // TODO: 不呼叫 controller.$render()，因為 Render 之後，會讓 DatePicker 重新跳出來，待研究
                        var unformatter = dateUtil.unformatterFactory(config.chrono, config.format, config.converter);
                        if (unformatter) {
                            controller.$setViewValue(unformatter($(iElement).val()));
                        }
                        // bind 後第一次 keyin 修改會觸發 watch 但之後的修改則不會
                        // blur 時要把 flag 取消以正確處理 model 值 formatting
                        keyPressFlag = false;
                    });
                });
            }

            // 每當偵測到 model 值被更新，立刻進行格式化
            scope.$watch(iAttrs.ngModel, function (newValue, oldValue, scope) {
                var modelDateFmt = config.format, modelValue;
                var separator = dateUtil.identifySeparator(config.format);

                // 判斷是否進行處理
                if (newValue) {
                    modelDateFmt = config.format.split(separator).join('');
                    // 處理使用者在文字文塊直接輸入的情況
                    if (typeof (newValue) == 'string') {
                        if (keyPressFlag) {
                            keyPressFlag = false;
                            if (separator) {
                                modelValue = newValue.split(separator).join('');
                            } else {
                                modelValue = newValue;
                            }
                            // 當被編輯到與格式不符時則不處理，等待 blur 時 不合法 datepicker 會進行糾正
                            if (modelDateFmt.length != modelValue.length) return;
                        } else {
                            // 由別的東西觸發的事件，例如在 IE 下會就會再多跑這一次，還不知道為什麼
                            modelValue = newValue;
                        }
                    }
                } else {
                    if (keyPressFlag) keyPressFlag = false;
                    return;
                }

                // 根據 model 型態 (即 converter)，進行格式化 (預設前提: Model 值和 DB 一致)
                if (config.converter == 'Raw') {
                    if (separator) {
                        if (dateUtil.isCompleteDateFormat(config.format)) {
                            // 將 Model 的值依照 modelDateFmt 轉成 Date
                            var modelDate = dateUtil.parseStrToDate(modelValue, config.chrono, modelDateFmt);
                            if (modelDate) {
                                // 日期正確，重新轉換後再寫回 (例如 111/11/1 -> 111/11/01)
                                var viewDisplayValue = dateUtil.parseDateToStr(modelDate, config.chrono, config.format);
                                $(iElement).val(viewDisplayValue);
                                controller.$setViewValue(viewDisplayValue.split(separator).join(''));
                                controller.$commitViewValue();
                            } else {
                                // 日期格式錯誤，將值清空 (例如 111/11/0)
                                $(iElement).val("");
                                controller.$setViewValue(null);
                                controller.$commitViewValue();
                            }
                        } else {
                            // 不完整的日期格式不能利用 momentjs 幫忙轉，只能做字串的處理
                            $(iElement).val(dateUtil.formatDate(newValue, config.format, modelDateFmt));
                        }
                    } else {
                        $(iElement).val(newValue);
                    }
                } else if (config.converter == 'Date') {
                    $(iElement).val(dateUtil.parseDateToStr(newValue, config.chrono, config.format));
                } else {
                    window.alert('尚不支援的 Converter 型態');
                    $(iElement).val(null);
                }
            });
        };

        return {
            restrict: 'A',
            require: 'ngModel',
            link: datePickerLink
        };
    };

    /**
     * @ngdoc directive
     * @name c.datePicker.directive:cDatePicker
     * @restrict A
     * @element input
     * @param {object=} cDatePicker
     *     <table>
     *          <tr>
     *               <th>chrono</th>
     *               <td>輸入的格式要以哪種曆法解析，預設以 ISO-8601 格式解析，設定為 <code>'Minguo'</code> 則支援民國曆法。</td>
     *          </tr>
     *          <tr>
     *               <th>format</th>
     *               <td>為畫面顯示的日期格式，預設以 <code>yyyy-MM-dd</code> 顯示(民國日期格式為 <code>yyy/MM/dd</code>)，在時間上目前僅支援 24 小時制 (<code>HH:mm:ss</code>)。</td>
     *          </tr>
     *          <tr>
     *               <th>converter</th>
     *               <td>model 值的型態 (<code>Raw</code>: 字串格式、<code>Date</code>: JavaScript Date 物件)。</td>
     *          </tr>
     *          <tr>
     *               <th>trigger</th>
     *               <td>觸發日曆元件顯示的事件 (例: <code>'focus'</code>)。</td>
     *          </tr>
     *          <tr>
     *               <th>validate</th>
     *               <td>是否強制驗證 (預設為 <code>true</code>)，標註為 <code>false</code> 則會將標註的 element 隱藏，僅出現小日曆 icon。</td>
     *          </tr>
     *     </table>
     * @description 日曆元件，支援民國紀元。
     */
    datePickerMod.directive('cDatePicker', ['dateUtil', 'my97DateConfigConverter', '$parse',
        '$compile', '$timeout', '$interpolate', 'cDatePickerTemplate', 'cUrls',
        cDatePicker]);
})();
