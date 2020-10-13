(function () {
    /**
     * @ngdoc overview
     * @name dateUtilMod
     * @description 日期操作模組
     */
    var dateUtilMod = angular.module('dateUtilMod', ['c.strings']);

    var dateUtil = function (Strings) {

        var Precondition = {
            checkValidCharacter: function (dateObj) {
                // 檢查每一個字是否不為空白以及文字
                var isPassValueCheck = true;
                for (var attr in dateObj) {
                    if (dateObj.hasOwnProperty(attr)) {
                        var eachCharacter = dateObj[attr].split('');
                        for (var i = 0; i < eachCharacter.length; i++) {
                            if (!/[0-9]{1}/.test(eachCharacter[i])) {
                                isPassValueCheck = false;
                                break;
                            }
                        }
                    }
                    if (isPassValueCheck == false)
                        break;
                }
                return isPassValueCheck;
            },
            checkValueRange: function (dateObj, chrono) {
                // 做簡單的元素值域檢查，因 moment.js 會對輕微的日期錯誤自動校正，產生人類無法理解的日期
                if (dateObj.year) {
                    var intYear = parseInt(dateObj.year, 10);
                    if ('Minguo' === chrono) {
                        if (intYear < 1 || intYear > 188)
                            return false;
                    } else {
                        if (intYear < 1900 || intYear > 2099)
                            return false;
                    }
                }
                if (dateObj.month) {
                    var intMonth = parseInt(dateObj.month, 10);
                    if (intMonth < 1 || intMonth > 12)
                        return false;
                }
                if (dateObj.day) {
                    var intDay = parseInt(dateObj.day, 10);
                    if (intDay < 1 || intDay > 31)
                        return false;
                }
                return true;
            }
        };

        /**
         * @ngdoc method 
         * 
         * @description 格式化日期字串
         * @name formatDate
         * @methodOf dateUtilMod.dateUtil
         * @param {string}
         *            dateStr 日期字串
         * @param {string}
         *            format 格式
         * @param {string}
         *            modelFormat model
         * @returns {string} 分隔符號
         */
        var formatDate = function (dateStr, format, modelFormat) {
            var posObj = parseDateFmt(modelFormat);

            if (typeof (dateStr) == 'string') {
                var dateValueObj = parseDateString(dateStr, posObj);

                // 將年月日依照格式填入format中
                if (dateValueObj.year)
                    format = format.replace(/y+/, dateValueObj.year);
                if (dateValueObj.month)
                    format = format.replace(/M+/, dateValueObj.month);
                if (dateValueObj.day)
                    format = format.replace(/d+/, dateValueObj.day);
                if (dateValueObj.hour)
                    format = format.replace(/H+/, dateValueObj.hour);
                if (dateValueObj.minute)
                    format = format.replace(/m+/, dateValueObj.minute);
                if (dateValueObj.second)
                    format = format.replace(/s+/, dateValueObj.second);

                return format;
            }
        };

        /**
         * @ngdoc method 
         * @name identifySeparator
         * @methodOf dateUtilMod.dateUtil
         * @description 辨認使用的分隔符號
         * 
         * @param {string}
         *            formatStr 格式
         * 
         * @returns {string} 分隔符號
         */
        var identifySeparator = function (formatStr) {
            var popularSeperators = ['/', '-', ':', ' '];

            var usingSeperator = null;
            // 找出分隔符號
            $.each(popularSeperators, function (idx, seperator) {
                if (formatStr.indexOf(seperator) != -1) {
                    usingSeperator = popularSeperators[idx];
                }
            });
            return usingSeperator;
        };

        /**
         * @ngdoc function
         * @name isCompleteDateFormat
         * @methodOf dateUtilMod.dateUtil
         * @description 判斷是不是完整日期
         * 
         * @param {string}
         *            format 格式
         * 
         * @returns {string} 已格式化後的日期
         */
        var isCompleteDateFormat = function (format) {
            var isYrExist = format.indexOf("y") >= 0;
            var isMonthExist = format.indexOf("M") >= 0;
            var isDateExist = format.indexOf("d") >= 0;
            return isYrExist && isMonthExist && isDateExist;
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:parseDateToStr
         * @methodOf dateUtilMod.dateUtil
         * @description 將Date物件依日期格式轉成字串
         * 
         * @param {date}
         *            date 日期
         * @param {string}
         *            chrono 語系
         * @param {string}
         *            format 格式
         * 
         * @returns {string} 已格式化後的日期
         */
        var parseDateToStr = function (date, chrono, format) {
            var inputMoment = null;
            inputMoment = moment(date);

            if (!inputMoment.isValid())
                return null;

            var intYr = null, dateObj, datePosObj;
            if (chrono == 'Minguo') {
                intYr = inputMoment.year() - 1911;
            } else {
                intYr = inputMoment.year();
            }

            dateObj = {
                year: intYr,
                month: inputMoment.month() + 1,
                day: inputMoment.date(),
                hour: inputMoment.hour(),
                minute: inputMoment.minute(),
                second: inputMoment.second(),
                millisecond: inputMoment.millisecond()
            };

            datePosObj = parseDateFmt(format);

            //根據format做文字的padding
            for (var attr in dateObj) {
                if (angular.isNumber(dateObj[attr]) && datePosObj[attr]) {
                    dateObj[attr] = Strings.padStart(dateObj[attr].toString(),
                        datePosObj[attr].length, '0');
                }
            }

            return format.replace(/y+/, dateObj.year).replace(/M+/, dateObj.month).replace(/d+/,
                dateObj.day).replace(/H+/, dateObj.hour).replace(/m+/, dateObj.minute).replace(
                    /s+/, dateObj.second).replace(/S+/, dateObj.millisecond);
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:parseStrToDate
         * @methodOf dateUtilMod.dateUtil
         * @description 將字串依日期格式轉成Date物件
         * 
         * @param {string}
         *            dateStr 日期
         * @param {string}
         *            chrono 語系
         * @param {string}
         *            format 格式
         * 
         * @returns {date} 日期
         */
        var parseStrToDate = function (dateStr, chrono, format) {
            var posObj = parseDateFmt(format);
            var dateValueObj = parseDateString(dateStr, posObj);

            if (!Precondition.checkValidCharacter(dateValueObj)) return null;
            if (!Precondition.checkValueRange(dateValueObj, chrono)) return null;

            var newConstructed = {
                month: parseInt(dateValueObj.month, 10) - 1,
                day: parseInt(dateValueObj.day, 10),
                hour: parseInt(dateValueObj.hour, 10),
                minute: parseInt(dateValueObj.minute, 10),
                second: parseInt(dateValueObj.second, 10)
            };

            if (chrono == 'Minguo') {
                newConstructed.year = parseInt(dateValueObj.year, 10) + 1911;
            } else {
                newConstructed.year = parseInt(dateValueObj.year, 10);
            }

            removeEmptyField(newConstructed);

            var newMoment = moment(newConstructed);
            if (!newMoment.isValid()) {
                return null;
            } else {
                return newMoment.toDate();
            }
        };

        var removeEmptyField = function (dateObj) {
            for (attr in dateObj) {
                if (!dateObj[attr]) dateObj[attr] = undefined;
            }
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:unformatterFactory
         * @methodOf dateUtilMod.dateUtil
         * @description 將格式化Js字串轉成要回傳到後端的值
         * 
         * @param {string}
         *            chrono 語系
         * @param {string}
         *            format 格式
         * @param {string}
         *            coverterType 需轉成的格式(Raw, Date)
         * 
         * @returns {object} 反格式化函式
         */
        var unformatterFactory = function (chrono, format, coverterType) {
            var seperator = identifySeparator(format);

            if (coverterType === 'Raw') {
                return function (valStr) {
                    if (!valStr)
                        return null;
                    if (seperator) {
                        // 濾掉分隔符號
                        return valStr.split(seperator).join('');
                    } else {
                        return valStr;
                    }
                };
            } else if (coverterType === 'Date') {
                if (format.toUpperCase().indexOf('D') == -1) {
                    // 如果格式沒有到日，轉Date沒有實際上意義
                    window.alert('您所指定的格式' + format + '不適用Converter : Date型態請使用Raw');
                    return null;
                } else {
                    return function (valStr) {
                        if (!valStr) {
                            return null;
                        }
                        return parseStrToDate(valStr, chrono, format);
                    };
                }
            } else {
                window.alert('尚不支援的Converter型態');
                return null;
            }
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:maxDayOfMonth
         * @methodOf dateUtilMod.dateUtil
         * @description 取回該月份之最大日期
         * 
         * @param {string |
         *            object} date 日期
         * 
         * @returns {number} 該月份最大日期
         */
        var maxDayOfMonth = function (date) {
            if (typeof (date) == 'string') {
                var dateObj = parseStrToDate(date, 'Minguo', 'yyyMMdd');
                return moment(dateObj).add('months', 1).date(0).date();
            } else {
                return moment(date).add('months', 1).date(0).date();
            }
        };

        // 找尋某個字在字串中出現的所有位置
        var searchLetterPos = function (targetStr, letter) {
            var posArray = [];
            var pos = targetStr.indexOf(letter);
            if (pos > -1) {
                while (pos > -1) {
                    posArray.push(pos);
                    pos = targetStr.indexOf(letter, pos + 1);
                }
                return posArray;
            } else {
                return null;
            }
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:parseDateFmt
         * @methodOf dateUtilMod.dateUtil
         * @description parse日期格式字串成年月日各在什麼位置的資訊
         * 
         * @param {string}
         *            dateFmt 日期格式
         * 
         * @returns {object} 日期位置物件，表示在一個字串中日期各元素出現在哪個位置
         */
        var parseDateFmt = function (dateFmt) {
            return {
                year: searchLetterPos(dateFmt, 'y'),
                month: searchLetterPos(dateFmt, 'M'),
                day: searchLetterPos(dateFmt, 'd'),
                hour: searchLetterPos(dateFmt, 'H'),
                minute: searchLetterPos(dateFmt, 'm'),
                second: searchLetterPos(dateFmt, 's'),
                millisecond: searchLetterPos(dateFmt, 'S')
            };
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:parseDateString
         * @methodOf dateUtilMod.dateUtil
         * @description 以日期位置物件，將日期字串轉為物件
         * 
         * @param {string}
         *            dateStr 日期字串
         * @param {object}
         *            posObj 日期位置物件，可由parseDateFmt方法取得
         * 
         * @returns {object} 物件以year, month, day屬性放置日期元素
         */
        var parseDateString = function (dateStr, posObj) {
            var dateStrArr = dateStr.split('');
            var dateObj = {};
            // key : year, month, day
            for (var key in posObj) {
                // 格式不保證完整，所以進行判斷
                if (posObj[key]) {
                    var dateElementStr = '';
                    $.each(posObj[key], function (idx, pos) {
                        if (dateStrArr.length > pos) {
                            dateElementStr += dateStrArr[pos];
                        }
                    });
                    dateObj[key] = dateElementStr;
                }
            }
            return dateObj;
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:add
         * @methodOf dateUtilMod.dateUtil
         * @description 日期操作(加)
         * 
         * @param {date}
         *            date 日期
         * @param {string}
         *            dateField 日期單位
         * @param {string}
         *            amount 數量
         * @returns {date} 操作後的日期
         */
        var add = function (date, dateField, amount) {
            return moment(date).add(dateField, amount).toDate();
        };

        /**
         * @ngdoc function
         * @name dateUtilMod.module:add
         * @methodOf dateUtilMod.dateUtil
         * @description 日期操作(減)
         * 
         * @param {date}
         *            date 日期
         * @param {string}
         *            dateField 日期單位
         * @param {string}
         *            amount 數量
         * @returns {date} 操作後的日期
         */
        var substract = function (date, dateField, amount) {
            return moment(date).subtract(dateField, amount).toDate();
        };

        return {
            formatDate: formatDate,
            identifySeparator: identifySeparator,
            isCompleteDateFormat: isCompleteDateFormat,
            maxDayOfMonth: maxDayOfMonth,
            parseDateFmt: parseDateFmt,
            parseDateToStr: parseDateToStr,
            parseDateString: parseDateString,
            parseStrToDate: parseStrToDate,
            unformatterFactory: unformatterFactory,
            add: add,
            substract: substract
        };
    };

    /**
     * @ngdoc service
     * @name dateUtilMod.dateUtil
     * @requires c.strings.Strings
     * @description 提供日期相關操作的功能
     */
    dateUtilMod.factory('dateUtil', ['Strings', dateUtil]);
})();
