// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
"use strict";
//languages
const FA_CULTURE = 'FA';
const AR_CULTURE = 'AR';
const EN_CULTURE = 'EN';

const gaugeChartConfig = {
    type: "gauge",
    data: {
        //labels: ['خیلی ضعیف', 'ضعیف', 'متوسط', 'خوب', 'خیلی خوب'],
        labels: ["Normal", "Warning", "Critical", "we", "sdd"],
        datasets: [
            {
                data: [460, 520, 580, 640, 900],
                value: 520,
                backgroundColor: ["red", "orange", "yellow", "yellowgreen", "green"],
                borderWidth: 2,
                minValue: 250,
            },
        ],
    },
    options: {
        legend: {
            display: true,
            position: "bottom",
            labels: {
                generateLabels: {}
            }
        },
        width: 10,
        responsive: true,
        // title: {
        //   display: true,
        //   text: "Gauge chart",
        // },
        layout: {
            padding: {
                bottom: 30,
            },
        },
        needle: {
            // Needle circle radius as the percentage of the chart area width
            radiusPercentage: 3,
            // Needle width as the percentage of the chart area width
            widthPercentage: 3,
            // Needle length as the percentage of the interval between inner radius (0%) and outer radius (100%) of the arc
            lengthPercentage: 15,
            // The color of the needle
            color: "rgba(0, 0, 0, 1)",
        },
        valueLabel: {
            formatter: Math.round,
        },
    },
};

$(document).ready(function () {
    $.extend(true, $.fn.dataTable.defaults, {
        language: {
            sEmptyTable: "هیچ داده ای در جدول وجود ندارد",
            sInfo: "نمایش _START_ تا _END_ از _TOTAL_ رکورد",
            sInfoEmpty: "نمایش 0 تا 0 از 0 رکورد",
            sInfoFiltered: "(فیلتر شده از _MAX_ رکورد)",
            sInfoPostFix: "",
            sInfoThousands: ",",
            sLengthMenu: "نمایش _MENU_ رکورد",
            sLoadingRecords: "در حال بارگزاری...",
            sProcessing: "در حال پردازش...",
            sSearch: "جستجو: ",
            sZeroRecords: "رکوردی با این مشخصات پیدا نشد",
            oPaginate: {
                sFirst: "ابتدا",
                sLast: "انتها",
                sNext: "بعدی",
                sPrevious: "قبلی",
            },
            oAria: {
                sSortAscending: ": فعال سازی نمایش به صورت صعودی",
                sSortDescending: ": فعال سازی نمایش به صورت نزولی",
            },
        },
    });
});
//const HIDDEN_SIDE_BAR = 'HiddenSideBar';
//$(function () {
//    //$(".preloader").fadeOut();
//    const hiddenSideBar = getCookie(HIDDEN_SIDE_BAR);
//    if (hiddenSideBar != null) {
//        if (hiddenSideBar == 'true')
//            hideSideBar();
//        //$('body').addClass('mini-sidebar');
//        else
//            showSideBar();
//        //$('body').removeClass('mini-sidebar');

//        //$(".sidebartoggler").click();
//    }
//});

const IsJsonString = (str) => {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}

const getExceptionMessageFromError = (error) => {
    debugger;
    if (error.responseJSON != undefined &&
        ((error.responseJSON.message != undefined && error.responseJSON.message != '') ||
            (error.responseJSON.Message != undefined && error.responseJSON.Message != ''))) {
        const jsonMessage = error.responseJSON.message != undefined && error.responseJSON.message != ''
            ? error.responseJSON.message
            : error.responseJSON.Message
        if (IsJsonString(jsonMessage))
            return JSON.parse(jsonMessage).Exception;
        else
            return jsonMessage;
    }
    return null;
}

const showWaiting = (targetId, pleaseWaitText, loading) => {
    $(`#${targetId}`).block({
        message: `${pleaseWaitText}<br/><img src="/images/loading8.gif" alt="${loading}"  />`,
        css: {
            border: "none",
            padding: "15px",
            backgroundColor: "#000",
            "-webkit-border-radius": "10px",
            "-moz-border-radius": "10px",
            opacity: 0.5,
            color: "#fff"
        }
    });
}
const hideWaiting = (targetId) => {
    $(`#${targetId}`).unblock();
}
const icons =
{
    error: "error",
    success: "success",
    warning: "warning",
    info: "info",
    question: "question",
}
const splitNumber = (digit) => {
    return digit.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
const replaceAllComma = (text) => {
    return text.replace(/,/g, '');
}

const replaceAll2 = (text, oldValue, newValue) => {
    return text.replace(`/${oldValue}/g`, newValue);
}
$('.number-thousand-separator').keyup(function (event) {
    if (event.which >= 37 && event.which <= 40) return;
    $(this).val(function (index, value) {
        return value
            // Keep only digits and decimal points:
            .replace(/[^\d]/g, "")
            // Add thousands separators:
            .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
    });
});
function showMessage(title, message, icon, buttonText, confirmFunction) {
    Swal.fire({
        title: title,
        //text: message,
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        html: message,
        icon: icon,
        confirmButtonText: buttonText
    }).then((result) => {
        if (result.isConfirmed && confirmFunction != undefined && confirmFunction != null && typeof confirmFunction === 'function') {
            confirmFunction();
        }
    });
}

function confirmMessage(title, message, icon, confirmButtonText, denyButtonText, cancelButtonText, confirmFunction, denyFunction, cancelFunction) {
    Swal.fire({
        title: title,
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        html: message,
        icon: icon,
        showCloseButton: false,
        showCancelButton: cancelButtonText != undefined && cancelButtonText != null && cancelButtonText !='',
        showDenyButton: denyButtonText != undefined && denyButtonText != null && denyButtonText !='',
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText,
        denyButtonText: denyButtonText,
    }).then((result) => {
        if (result.isConfirmed && confirmFunction != undefined && confirmFunction != null && typeof confirmFunction === 'function') {
            confirmFunction();
        } else if (result.isDenied && denyFunction != undefined && denyFunction != null && typeof denyFunction === 'function') {
            denyFunction();
        }
        else if (result.isDismissed && cancelFunction != undefined && cancelFunction != null && typeof cancelFunction === 'function') {
            cancelFunction();
        }
    });
}

const generateDocumentsLink = (documents, docTitle) => {
    let links = '';
    docTitle = docTitle == undefined || docTitle == '' || docTitle == null ? 'سند شماره' : docTitle;
    if (documents != null && documents != undefined && documents.length > 0) {
        $.map(documents, (item, index) => {
            if (links !== '') links += "<br/>";
            links += `<a target='_blank' href='${item.filePath}'>${docTitle} ${index + 1}</a>`;
        });
    }
    return links;
}
const generateAgentHistoryLogRows = (historyList, culture) => {
    debugger;
    let rows = '';
    if (historyList != null && historyList != undefined && historyList.length > 0) {
        $.map(historyList, (item, index) => {
            rows += '<tr class="' + (index % 2 == 0 ? 'table-info' : 'table-light') + '">';
            rows += "<td style=''>" + (index + 1) + "</td>";
            rows += "<td style=''>" + item.statusDescription + "</td>";
            rows += "<td style=''>" + (culture == FA_CULTURE ? item.shamsiCreateDate : item.createDate) + "</td>";
            rows += "<td style=''>" + item.creatorFullName + "</td>";
            rows += "<td style='color:red'>" + ((item.description != undefined && item.description != null) ? item.description.replace(/\n/g, '<br/>') : '') + "</td>";
            rows += '</tr>';
        });
    }
    return rows;
}
const scrollToFirstError = () => {
    if ($('.field-validation-error').length > 0) {
        $('html, body').animate({
            scrollTop: ($('.field-validation-error').offset().top - 300)
        }, 1500);
        const firstElementToError = $($('.field-validation-error')[0]).attr('data-valmsg-for');
        if (firstElementToError !== undefined)
            $(`#${firstElementToError}`).focus();
    }
}
let currentLength = 0;
$('.price-css').on('keyup',
    function (event) {
        var digit = $(this).val();
        while (digit.indexOf(',', 0) != -1) {
            digit = digit.replace(',', '');
        }
        if ($(this).val() != '') {
            $(this).val(digit.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
            if ($(this).hasClass('price-css')) {
                if (digit == undefined) return;

                //console.log('keyup : #' + this.id + ',' + wordifyRials(digit));
                //showTooltip($('#' + this.id), wordifyRials(digit));
            }
        } else {
            //if ($(this).hasClass('price-css'))
            //$('#' + this.id).tooltip('hide');
        }
    }).on('blur',
        function () {
            //if ($(this).hasClass('price-css'))
            //$('#' + this.id).tooltip('hide');
        }).on('keydown',
            function (event) {
                if (event.keyCode == 46 || event.keyCode == 8 || event.which == 8 || event.which == 46) {
                    return;
                }
                var maxLength = $(this).attr('maxlength');
                if (maxLength != undefined) {
                    var digit = $(this).val();
                    while (digit.indexOf(',', 0) != -1) {
                        digit = digit.replace(',', '');
                    }
                    currentLength = digit.length; // + 1;
                    var splitCount = parseInt(currentLength / 3, 0);
                    if (currentLength + splitCount > maxLength) {
                        event.preventDefault();
                    }
                }
            }).on('focusout',
                function () {
                    var digit = $(this).val();
                    while (digit.indexOf(',', 0) != -1) {
                        digit = digit.replace(',', '');
                    }
                    if ($(this).val() != '') {
                        $(this).val(digit.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
                        if ($(this).hasClass('price-css')) {
                            //console.log('focusout:' + wordifyRials(digit));
                            //showTooltip($('#' + this.id), wordifyRials(digit));
                        }
                    } else {
                        //if ($(this).hasClass('price-css'))
                        //$('#' + this.id).tooltip('hide');
                    }
                }).focus(function () {
                    $(this).select();
                    var digit = $(this).val();
                    while (digit.indexOf(',', 0) != -1) {
                        digit = digit.replace(',', '');
                    }
                    //showTooltip($('#' + this.id), wordifyRials(digit));
                });

$('.card-number-css').on('keyup',
    function (event) {
        debugger;
        if (event.keyCode == 46 || event.keyCode == 8 || event.which == 8 || event.which == 46) {
            return;
        }
        var digit = $(this).val();
        while (digit.indexOf('-', 0) != -1) {
            digit = digit.replace('-', '');
        }
        if (digit != '' && digit.length < 16) {
            //$(this).val(digit.replace(/(\d)(?=(\d\d\d\d)+(?!\d))/g, "$1-"));
            $(this).val(digit.replace(/(\d{4}(?!\s))/g, "$1-"));
        }
    }).on('blur',
        function () {
            //if ($(this).hasClass('price-css'))
            //$('#' + this.id).tooltip('hide');
        }).on('keydown',
            function (event) {
                if (event.keyCode == 46 || event.keyCode == 8 || event.which == 8 || event.which == 46) {
                    return;
                }
                var maxLength = $(this).attr('maxlength');
                if (maxLength != undefined) {
                    var digit = $(this).val();
                    while (digit.indexOf('-', 0) != -1) {
                        digit = digit.replace('-', '');
                    }
                    let currentLength = digit.length; // + 1;
                    var splitCount = parseInt(currentLength / 3, 0);
                    if (currentLength + splitCount > maxLength) {
                        event.preventDefault();
                    }
                }
            }).on('focusout',
                function () {
                    var digit = $(this).val();
                    while (digit.indexOf(',', 0) != -1) {
                        digit = digit.replace('-', '');
                    }
                    if ($(this).val() != '') {
                        $(this).val(digit.replace(/(\d)(?=(\d\d\d\d)+(?!\d))/g, "$1-"));
                    }
                }).focus(function () {
                    $(this).select();
                    var digit = $(this).val();
                    while (digit.indexOf(',', 0) != -1) {
                        digit = digit.replace(',', '');
                    }
                    //showTooltip($('#' + this.id), wordifyRials(digit));
                });