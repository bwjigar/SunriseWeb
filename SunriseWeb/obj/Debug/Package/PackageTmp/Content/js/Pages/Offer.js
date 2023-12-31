﻿var ParameterList;
var isModify = 0;
var gridOptions = {};
var rowData = [];
var columnDefs = [];
var columnConfigDefs = [];
var SizeGroupList = [];
var KeyToSymbolList = [];
var CheckKeyToSymbolList = [];
var UnCheckKeyToSymbolList = [];
var ShapeList = [];
var ColorList = [];
var ClarityList = [];
var CutList = [];
var LabList = [];
var PolishList = [];
var SymList = [];
var FlouList = [];
var CaratList = [];
var BgmList = [];
var BlackList = [];
var TblInclList = [];
var TblNattsList = [];
var CrwnInclList = [];
var CrwnNattsList = [];
var LusterList = [];
var LocationList = [];
var CurencyList = [];
var obj1 = {};
var summary1 = [];
var OfferPercentage = 0;
var AllD = false;
var UploadMode = "";
var IndicatorCalled = 0;
var DiscAmtArray = [];
var Offer_StoneList = [];
var ___company_Userid = '', ___company_Fortunecode = '';

function OpenDownloadCheck() {
    if (gridOptions.api.getSelectedRows().length > 0) {
        $("#liAll_1").show();
    }
    else {
        $("#liAll_1").hide();
    }
}
function ALLDownload() {
    AllD = true;
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();

    $("#customRadio4").prop("checked", true);
    $('#hdnDownloadType').val("Image");
    DownloadMedia();
    $('#hdnDownloadType').val("Certificate");
    DownloadMedia();
    $('#hdnDownloadType').val("Video");
    DownloadMedia();
    $('#hdnDownloadType').val("Excel");
    DownloadExcel();
}
/*------------ order-history-dropdown-select ------------*/

$(document).on('click', '.browse', function () {
    var file = $(this).parent().parent().parent().find('.file');
    file.trigger('click');
});
$(document).on('change', '.file', function () {
    $(this).parent().find('.form-control').val($(this).val().replace(/C:\\fakepath\\/i, ''));
});
/*CARAT2 li*/


/*common li*/

function contentHeight() {
    var winH = $(window).height(),
        tabsmarkerHei = $(".order-title").height(),
        navbarHei = $(".navbar").height(),
        resultHei = $(".result-nav").height(),
        contentHei = winH - navbarHei - tabsmarkerHei - resultHei - 56;
    $("#myGrid").css("height", contentHei);
}

$(window).resize(function () {
    contentHeight();
});

new WOW().init();

$(document).ready(function () {
    GetCompanyList();
    $("#___txtCompanyName").focusout(function () {
        CmpnynmSelectRequired();
    });
    GetOfferCriteria();
    GetSearchParameter();
    BindColumnsSettings();
    BindKeyToSymbolList();

    var icon_selected = new Array();
    $('ul.search').on('click', ".common-ico", function () {

        var aa = $(this)
        if (!aa.is('.active')) {
            aa.addClass('active');

            var my_id = this.id;
            icon_selected.push(my_id);

        } else {
            aa.removeClass('active');
            var my_id = this.id;
            var index = icon_selected.indexOf(my_id);
            if (index > -1) {
                icon_selected.splice(index, 1);
            }
        }
    });
    $('.cntCart').click(function () {
        location.href = '/Cart/Index';
    });
    $('.cntwishlist').click(function () {
        var url = '';
        if ($('#hdnisempflg').val() == 1 || $('#hdnisadminflg').val() == 1)
            url = '/Wishlist/Admin_Wishlist';
        else
            url = '/Wishlist/Index';
        location.href = url;
    });
    $('#frmSaveOrder').validate({
        rules: {
            field: {
                required: true
            }
        },
        messages: {
            comments: "Enter Comment"
        }
    });
    $('#frmSendMail').validate({
        rules: {
            field: {
                required: true
            }
        },
        messages: {
            email: "Enter Email"
        }
    });
    $('#ConfirmOrderModal').on('hidden.bs.modal', function () {
        $('#Comments').val("");
    });
    $("#myTab").on("click", "a", function (e) {

    }).on("click", "i", function (e) {

        e.preventDefault();
        $(this).parent().hide();
        $(this).hide();
        $('.result-page-content').removeClass('active show');
        $('.result-page-content').addClass('hide');
        $("#home-tab2").removeClass('hide');
        $(".nav-link").removeClass('active');
        $("#tabhome").addClass('active');
        $("#home-tab2").addClass('active show');
        e.stopImmediatePropagation();
    });
    $('.sym-sec').on('click', function () {
        $('.sym-sec').toggleClass('active');
    });
    var li_selected = new Array();
    $('ul.common-li').on('click', "li", function () {

        var ab = $(this)
        if (!ab.hasClass('active')) {
            ab.addClass('active');

            var l_id = this.id;
            li_selected.push(l_id);

        } else {
            ab.removeClass('active');
            var l_id = this.id;
            var index = li_selected.indexOf(l_id);
            if (index > -1) {
                li_selected.splice(index, 1);
            }
        }
    });
    var li_selected = new Array();
    $('.carat2.common-li').on('click', "li", function () {

        var ab = $(this)
        if (!ab.is('.active')) {
            ab.addClass('active');

            var l_id = this.id;
            li_selected.push(l_id);

        } else {
            ab.removeClass('active');
            var l_id = this.id;
            var index = li_selected.indexOf(l_id);
            if (index > -1) {
                li_selected.splice(index, 1);
            }
        }
    });
    $(".card").click(function () {
        $(".card").removeClass("active");
        $(this).addClass("active");
    });
    $(".numeric").numeric({ decimal: ".", negative: true, decimalPlaces: 2 });
    $('#ExcelModalAll').on('show.bs.modal', function (event) {
        var count = gridOptions.api.getSelectedRows().length;
        if (count > 0) {
            $('#customRadio4').prop('checked', true);
        } else {
            $('#customRadio3').prop('checked', true);
        }
    });
    $('#EmailModal').on('show.bs.modal', function (event) {
        var count = gridOptions.api.getSelectedRows().length;

        if (count > 0) {
            $('#customRadiomail2').prop('checked', true);
        } else {
            $('#customRadiomail').prop('checked', true);
        }
    });
    $('.result-three li a.download-popup').on('click', function (event) {
        $('.download-toggle').toggleClass('active');
        event.stopPropagation();
    });
    $(document).click(function (event) {
        if (!$(event.target).hasClass('active')) {
            $(".download-toggle").removeClass("active");
        }
    });
    $('#OfferStoneModal').on('show.bs.modal', function () {
        ___company_Userid = '';
        ___company_Fortunecode = '';
        $("#___txtCompanyName").val("");
        $("#___txtCompanyName_hidden").val("");
    });
});
function GetOfferCriteria() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    $.ajax({
        url: "/Offer/GetOfferCriteria",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            OfferPercentage = data.Data[0].OfferPer;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function SetModifyParameter() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    $('#iresult').hide();
    $('.result-page-content').removeClass('active show');
    $('.result-page-content').addClass('hide');
    $("#home-tab2").removeClass('hide');
    $(".nav-link").removeClass('active');
    $("#tabhome").addClass('active');
    $("#home-tab2").addClass('active show');
    $.ajax({
        url: "/Offer/GetModifyStockParameter",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            //$('#searchcaratspecific').
            //CheckKTS,UNCheckKTS,KeyToSymbol
            if (data.IsTripalEx) {
                $('#li3ex').addClass('active');
            } else {
                $('#li3ex').removeClass('active');
            }
            if (data.IsTripalVg) {
                $('#li3vg').addClass('active');
            } else {
                $('#li3vg').removeClass('active');
            }
            var checkkts = data.CheckKTS;
            if (checkkts != null) {
                checkkts = checkkts.split(',');
                $(checkkts).each(function (i, res) {

                    CheckKeyToSymbolList.push({
                        "NewID": KeyToSymbolList.length + 1,
                        "Symbol": res,
                    });
                    $('#searchkeytosymbol input[onclick="GetCheck_KTS_List(\'' + res + '\');"]').prop('checked', true);
                });
                $('#spanselected').html('' + CheckKeyToSymbolList.length + ' - Selected');
            }
            var uncheckkts = data.UNCheckKTS;
            if (uncheckkts != null) {
                uncheckkts = uncheckkts.split(',');
                $(uncheckkts).each(function (i, res) {
                    UnCheckKeyToSymbolList.push({
                        "NewID": UnCheckKeyToSymbolList.length + 1,
                        "Symbol": res,
                    });
                    $('#searchkeytosymbol input[onclick="GetUnCheck_KTS_List(\'' + res + '\');"]').prop('checked', true);
                });
                $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + ' - Deselected');
            }

            var carattype = data.CaratType;

            var pointer = data.Pointer;
            if (pointer != null) {
                pointer = pointer.split(',');
                if (carattype == 'Specific') {
                    $(pointer).each(function (i, res) {
                        var res1 = res.split('-')
                        var NewID = SizeGroupList.length + 1;
                        SizeGroupList.push({
                            "NewID": NewID,
                            "FromCarat": res1[0],
                            "ToCarat": res1[1],
                            "Size": res,
                        });
                        //<li class="carat-li-top">1.00-1.00<i class="fa fa-plus-circle" aria-hidden="true"></i></li>
                        $('#searchcaratspecific').append('<li id="' + NewID + '" class="carat-li-top">' + res + '<i class="fa fa-times-circle" aria-hidden="true" onclick="NewSizeGroupRemove(' + NewID + ');"></i></li>');
                    });
                    $('a[href="#carat1"]').click();
                }
                else {
                    $(pointer).each(function (i, res) {
                        if (_.find(CaratList, function (num) { return num.Value == res; })) {
                            _.findWhere(CaratList, { Value: res }).ACTIVE = true;
                            $('#searchcaratgen li[onclick="SetActive(\'carat\',\'' + res + '\')"]').addClass('active');
                        }
                    });
                    $('a[href="#carat2"]').click()
                }
            }
            var shape = data.Shape;
            if (shape != null) {
                shape = shape.split(',');
                $(shape).each(function (i, res) {

                    if (_.find(ShapeList, function (num) { return num.Value == res; })) {
                        _.findWhere(ShapeList, { Value: res }).ACTIVE = true;
                        $('#searchshape li a[onclick="SetActive(\'Shape\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var color = data.Color;
            if (color != null) {
                color = color.split(',');
                $(color).each(function (i, res) {

                    if (_.find(ColorList, function (num) { return num.Value == res; })) {
                        _.findWhere(ColorList, { Value: res }).ACTIVE = true;
                        $('#searchcolor li[onclick="SetActive(\'COLOR\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var clarity = data.Clarity;
            if (clarity != null) {
                clarity = clarity.split(',');
                $(clarity).each(function (i, res) {

                    if (_.find(ClarityList, function (num) { return num.Value == res; })) {
                        _.findWhere(ClarityList, { Value: res }).ACTIVE = true;
                        $('#searchclarity li[onclick="SetActive(\'CLARITY\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var cut = data.Cut;
            if (cut != null) {
                cut = cut.split(',');
                $(cut).each(function (i, res) {

                    if (_.find(CutList, function (num) { return num.Value == res; })) {
                        _.findWhere(CutList, { Value: res }).ACTIVE = true;
                        $('#searchcut li[onclick="SetActive(\'CUT\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var polish = data.Polish;
            if (polish != null) {
                polish = polish.split(',');
                $(polish).each(function (i, res) {

                    if (_.find(PolishList, function (num) { return num.Value == res; })) {
                        _.findWhere(PolishList, { Value: res }).ACTIVE = true;
                        $('#searchpolish li[onclick="SetActive(\'POLISH\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var symm = data.Symm;
            if (symm != null) {
                symm = symm.split(',');
                $(symm).each(function (i, res) {

                    if (_.find(SymList, function (num) { return num.Value == res; })) {
                        _.findWhere(SymList, { Value: res }).ACTIVE = true;
                        $('#searchsymm li[onclick="SetActive(\'SYMM\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var fls = data.Fls;
            if (fls != null) {
                fls = fls.split(',');
                $(fls).each(function (i, res) {

                    if (_.find(FlouList, function (num) { return num.Value == res; })) {
                        _.findWhere(FlouList, { Value: res }).ACTIVE = true;
                        $('#searchfls li[onclick="SetActive(\'FLS\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var lab = data.Lab;
            if (lab != null) {
                lab = lab.split(',');
                $(lab).each(function (i, res) {

                    if (_.find(LabList, function (num) { return num.Value == res; })) {
                        _.findWhere(LabList, { Value: res }).ACTIVE = true;
                        $('#searchlab li[onclick="SetActive(\'LAB\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var inclusion = data.Inclusion;
            if (inclusion != null) {
                inclusion = inclusion.split(',');
                $(inclusion).each(function (i, res) {

                    if (_.find(TblInclList, function (num) { return num.Value == res; })) {
                        _.findWhere(TblInclList, { Value: res }).ACTIVE = true;
                        $('#searchtableincl li[onclick="SetActive(\'TABLE_INCL\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var natts = data.Natts;
            if (natts != null) {
                natts = natts.split(',');
                $(natts).each(function (i, res) {

                    if (_.find(TblNattsList, function (num) { return num.Value == res; })) {
                        _.findWhere(TblNattsList, { Value: res }).ACTIVE = true;
                        $('#searchtablenatts li[onclick="SetActive(\'TABLE_NATTS\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var crowninclusion = data.CrownInclusion;
            if (crowninclusion != null) {
                crowninclusion = crowninclusion.split(',');
                $(crowninclusion).each(function (i, res) {

                    if (_.find(CrwnInclList, function (num) { return num.Value == res; })) {
                        _.findWhere(CrwnInclList, { Value: res }).ACTIVE = true;
                        $('#searchcrownincl li[onclick="SetActive(\'CROWN_INCL\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var crownnatts = data.CrownNatts;
            if (crownnatts != null) {
                crownnatts = crownnatts.split(',');
                $(crownnatts).each(function (i, res) {

                    if (_.find(CrwnNattsList, function (num) { return num.Value == res; })) {
                        _.findWhere(CrwnNattsList, { Value: res }).ACTIVE = true;
                        $('#searchcrownnatts li[onclick="SetActive(\'CROWN_NATTS\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var location = data.Location;
            if (location != null) {
                location = location.split(',');
                $(location).each(function (i, res) {

                    if (_.find(LocationList, function (num) { return num.Value == res; })) {
                        _.findWhere(LocationList, { Value: res }).ACTIVE = true;
                        $('#searchlocation li[onclick="SetActive(\'Location\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            var bgm = data.BGM;
            if (bgm != null) {
                bgm = bgm.split(',');
                $(bgm).each(function (i, res) {

                    if (_.find(BgmList, function (num) { return num.Value == res; })) {
                        _.findWhere(BgmList, { Value: res }).ACTIVE = true;
                        $('#searchbgm li[onclick="SetActive(\'BGM\',\'' + res + '\')"]').addClass('active');
                    }
                });
            }
            $('#txtDisFrom').val(data.FormDisc);
            $('#txtDisTo').val(data.ToDisc);
            $('#txtPrCtsFrom').val(data.FormPricePerCts);
            $('#txtPrCtsTo').val(data.ToPricePerCts);
            $('#TotalAmtFrom').val(data.FormNetAmt);
            $('#TotalAmtTo').val(data.ToNetAmt);
            $('#txtLengthFrom').val(data.FormLength);
            $('#txtLengthTo').val(data.ToLength);
            $('#txtWidthFrom').val(data.FormWidth);
            $('#txtWidthTo').val(data.ToWidth);
            $('#txtDepthPerFrom').val(data.FormDepthPer);
            $('#txtDepthPerTo').val(data.ToDepthPer);
            $('#txtTablePerFrom').val(data.FormTablePer);
            $('#txtTablePerTo').val(data.ToTablePer);
            $('#txtCrAngFrom').val(data.FromCrownAngle);
            $('#txtCrAngTo').val(data.ToCrownAngle);
            $('#txtCrHtFrom').val(data.FromCrownHeight);
            $('#txtCrHtTo').val(data.ToCrownHeight);
            $('#txtPavAngFrom').val(data.FromPavAngle);
            $('#txtPavAngTo').val(data.ToPavAngle);
            $('#txtPavHtFrom').val(data.FromPavHeight);
            $('#txtPavHtTo').val(data.ToPavHeight);
            if (data.HasImage) {
                $('#SearchImage').addClass('active');
            } else {
                $('#SearchImage').removeClass('active');
            }
            if (data.HasHDMovie) {
                $('#SearchVideo').addClass('active');
            } else {
                $('#SearchVideo').removeClass('active');
            }


        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function GetSearchParameter() {

    $.ajax({
        url: "/SearchStock/GetSearchParameter",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            ParameterList = data.Data;
            if (ParameterList.length > 0) {
                _.each(ParameterList, function (itm) {
                    itm.ACTIVE = false;
                });

                ParameterList.push({
                    Id: 12, Value: "OTHERS", ListType: "SHAPE",
                    UrlValue: "https://sunrisediamonds.com.hk/Images/Shape/ROUND.svg",
                    UrlValueHov: "https://sunrisediamonds.com.hk/Images/Shape/ROUND_Trans.png"
                })
            }

            $('#searchcaratgen').html("");
            CaratList = _.filter(ParameterList, function (e) { return e.ListType == 'POINTER' });
            _(CaratList).each(function (carat, i) {
                $('#searchcaratgen').append('<li onclick="SetActive(\'carat\',\'' + carat.Value + '\')">' + carat.Value + '</li>');
            });

            $('#searchshape').html("");
            ShapeList = _.filter(ParameterList, function (e) { return e.ListType == 'SHAPE' });
            _(ShapeList).each(function (shape, i) {
                $('#searchshape').append('<li class="wow zoomIn animated" data-wow-delay="0.8s"><a href="javascript:void(0);" onclick="SetActive(\'Shape\',\'' + shape.Value + '\')" class="common-ico"><div class="icon-image one"><img src="' + shape.UrlValue + '" class="first-ico"><img src="' + shape.UrlValueHov + '" class="second-ico"></div><span>' + shape.Value + '</span></a></li>');
            });

            $('#searchcolor').html("");
            ColorList = _.filter(ParameterList, function (e) { return e.ListType == 'COLOR' });
            _(ColorList).each(function (color, i) {
                $('#searchcolor').append('<li onclick="SetActive(\'COLOR\',\'' + color.Value + '\')">' + color.Value + '</li>');
            });

            $('#searchclarity').html("");
            ClarityList = _.filter(ParameterList, function (e) { return e.ListType == 'CLARITY' });
            _(ClarityList).each(function (clarity, i) {
                $('#searchclarity').append('<li onclick="SetActive(\'CLARITY\',\'' + clarity.Value + '\')">' + clarity.Value + '</li>');
            });

            $('#searchcut').html("");
            CutList = _.filter(ParameterList, function (e) { return e.ListType == 'CUT' });
            _(CutList).each(function (cut, i) {
                $('#searchcut').append('<li onclick="SetActive(\'CUT\',\'' + cut.Value + '\')">' + cut.Value + '</li>');
            });

            $('#searchlab').html("");
            LabList = _.filter(ParameterList, function (e) { return e.ListType == 'LAB' });
            _(LabList).each(function (lab, i) {
                $('#searchlab').append('<li onclick="SetActive(\'LAB\',\'' + lab.Value + '\')">' + lab.Value + '</li>');
            });

            $('#searchpolish').html("");
            PolishList = _.filter(ParameterList, function (e) { return e.ListType == 'POLISH' });
            _(PolishList).each(function (polish, i) {
                $('#searchpolish').append('<li onclick="SetActive(\'POLISH\',\'' + polish.Value + '\')">' + polish.Value + '</li>');
            });

            $('#searchsymm').html("");
            SymList = _.filter(ParameterList, function (e) { return e.ListType == 'SYMM' });
            _(SymList).each(function (sym, i) {
                $('#searchsymm').append('<li onclick="SetActive(\'SYMM\',\'' + sym.Value + '\')">' + sym.Value + '</li>');
            });

            $('#searchfls').html("");
            FlouList = _.filter(ParameterList, function (e) { return e.ListType == 'FLS' });
            _(FlouList).each(function (fls, i) {
                $('#searchfls').append('<li onclick="SetActive(\'FLS\',\'' + fls.Value + '\')">' + fls.Value + '</li>');
            });



            $('#searchbgm').html("");
            BgmList = _.filter(ParameterList, function (e) { return e.ListType == 'BGM' });
            _(BgmList).each(function (bgm, i) {
                $('#searchbgm').append('<li onclick="SetActive(\'BGM\',\'' + bgm.Value + '\')">' + bgm.Value + '</li>');
            });

            BlackList = _.filter(ParameterList, function (e) { return e.ListType == 'BLACK' });

            $('#searchtableincl').html("");
            TblInclList = _.filter(ParameterList, function (e) { return e.ListType == 'TABLE_INCL' });
            _(TblInclList).each(function (tblincl, i) {
                $('#searchtableincl').append('<li onclick="SetActive(\'TABLE_INCL\',\'' + tblincl.Value + '\')">' + tblincl.Value + '</li>');
            });

            $('#searchtablenatts').html("");
            TblNattsList = _.filter(ParameterList, function (e) { return e.ListType == 'TABLE_NATTS' });
            _(TblNattsList).each(function (tblnatts, i) {
                $('#searchtablenatts').append('<li onclick="SetActive(\'TABLE_NATTS\',\'' + tblnatts.Value + '\')">' + tblnatts.Value + '</li>');
            });

            $('#searchcrownincl').html("");
            CrwnInclList = _.filter(ParameterList, function (e) { return e.ListType == 'CROWN_INCL' });
            _(CrwnInclList).each(function (crwnincl, i) {
                $('#searchcrownincl').append('<li onclick="SetActive(\'CROWN_INCL\',\'' + crwnincl.Value + '\')">' + crwnincl.Value + '</li>');
            });

            $('#searchcrownnatts').html("");
            CrwnNattsList = _.filter(ParameterList, function (e) { return e.ListType == 'CROWN_NATTS' });
            _(CrwnNattsList).each(function (crwnnatt, i) {
                $('#searchcrownnatts').append('<li onclick="SetActive(\'CROWN_NATTS\',\'' + crwnnatt.Value + '\')">' + crwnnatt.Value + '</li>');
            });

            LusterList = _.filter(ParameterList, function (e) { return e.ListType == 'LUSTER' });

            $('#searchlocation').html("");
            LocationList = _.filter(ParameterList, function (e) { return e.ListType == 'LOCATION' });
            _(LocationList).each(function (loc, i) {
                $('#searchlocation').append('<li onclick="SetActive(\'Location\',\'' + loc.Value + '\')">' + loc.Value + '</li>');
            });

            CurencyList = _.filter(ParameterList, function (e) { return e.ListType == 'CURRENCY' });


        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function BindKeyToSymbolList() {

    $.ajax({
        url: "/SearchStock/GetKeyToSymbolList",
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            var KeytoSymbolList = data.Data;
            $('#searchkeytosymbol').html("");
            if (KeytoSymbolList.length > 0) {
                $.each(KeytoSymbolList, function (i, itm) {
                    $('#searchkeytosymbol').append('<div class="col-12 pl-0 pr-0 ng-scope">'
                        + '<ul class="row m-0">'
                        + '<li class="carat-dropdown-chkbox">'
                        + '<div class="main-cust-check">'
                        + '<label class="cust-rdi-bx mn-check">'
                        + '<input type="radio" class="checkradio" id="CHK_KTS_Radio_' + (i + 1) + '" name="radio' + (i + 1) + '" onclick="GetCheck_KTS_List(\'' + itm.sSymbol + '\');">'
                        + '<span class="cust-rdi-check">'
                        + '<i class="fa fa-check"></i>'
                        + '</span>'
                        + '</label>'
                        + '<label class="cust-rdi-bx mn-time">'
                        + '<input type="radio" id="UNCHK_KTS_Radio_' + (i + 1) + '" class="checkradio" name="radio' + (i + 1) + '" onclick="GetUnCheck_KTS_List(\'' + itm.sSymbol + '\');">'
                        + '<span class="cust-rdi-check">'
                        + '<i class="fa fa-times"></i>'
                        + '</span>'
                        + '</label>'
                        + '</div>'
                        + '</li>'
                        + '<li class="col">'
                        + '<span>' + itm.sSymbol + '</span>'
                        + '</li>'
                        + '</ul>'
                        + '</div>')
                    //itm.ACTIVE = false;
                    //itm.INACTIVE = false;
                });
                $('#searchkeytosymbol').append('<div class="ps-scrollbar-x-rail" style="left: 0px; bottom: 0px;"><div class="ps-scrollbar-x" tabindex="0" style="left: 0px; width: 0px;"></div></div><div class="ps-scrollbar-y-rail" style="top: 0px; right: 0px;"><div class="ps-scrollbar-y" tabindex="0" style="top: 0px; height: 0px;"></div></div>');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function GetCheck_KTS_List(item) {
    var SList = _.reject(UnCheckKeyToSymbolList, function (e) { return e.Symbol == item });
    UnCheckKeyToSymbolList = SList;

    var res = _.filter(CheckKeyToSymbolList, function (e) { return (e.Symbol == item) });
    if (res.length == 0) {
        CheckKeyToSymbolList.push({
            "NewID": CheckKeyToSymbolList.length + 1,
            "Symbol": item,
        });
        $('#spanselected').html('' + CheckKeyToSymbolList.length + ' - Selected');
        $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + ' - Deselected');
    }
}
function GetUnCheck_KTS_List(item) {
    var SList = _.reject(CheckKeyToSymbolList, function (e) { return e.Symbol == item });
    CheckKeyToSymbolList = SList

    var res = _.filter(UnCheckKeyToSymbolList, function (e) { return (e.Symbol == item) });
    if (res.length == 0) {
        UnCheckKeyToSymbolList.push({
            "NewID": UnCheckKeyToSymbolList.length + 1,
            "Symbol": item,
        });
        $('#spanselected').html('' + CheckKeyToSymbolList.length + ' - Selected');
        $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + ' - Deselected');
    }
}
function CloseSendMailPopup() {
    $('#EmailModal').modal('hide');
    $('#txtemail').val("");
    $('#txtNotes').val("");
}
function validmail(e) {
    var emailID = $(e).val();
    emailID = emailID.split(',');
    for (var i = 0; i < emailID.length; i++) {
        if (!checkemail(emailID[i])) {
            toastr.error($("#hdn_Invalid_email_format").val());
            $("#txtemail").val('');
            return;
        }
    }
}
function checkemail(valemail) {
    var forgetfilter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*(;|,)\s*|\s*$)/;  ///^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (forgetfilter.test(valemail)) {

        return true;
    }
    else {

        return false;
    }
}
function ClearSendMail() {
    $('#txtemail').val("");
    $('#txtNotes').val("");
}
function NewSizeGroup() {

    fcarat = $('#txtfromcarat').val();
    tcarat = $('#txttocarat').val();

    if (fcarat == "" && tcarat == "" || fcarat == 0 && tcarat == 0) {
        toastr.warning($("#hdn_Please_Enter_Carat").val());
        return false;
    }
    if (fcarat == "") {
        fcarat = "0";
    }
    var SizeGroupList_ = [];
    SizeGroupList_.push({
        "NewID": SizeGroupList.length + 1,
        "FromCarat": fcarat,
        "ToCarat": tcarat,
        "Size": fcarat + '-' + tcarat,

    });
    var lst = _.filter(SizeGroupList, function (e) { return (e.Size == SizeGroupList_[0].Size) });
    if (lst.length == 0) {
        SizeGroupList.push({
            "NewID": SizeGroupList_[0].NewID,
            "FromCarat": SizeGroupList_[0].FromCarat,
            "ToCarat": SizeGroupList_[0].ToCarat,
            "Size": SizeGroupList_[0].Size,
        });
        //<li class="carat-li-top">1.00-1.00<i class="fa fa-plus-circle" aria-hidden="true"></i></li>
        $('#searchcaratspecific').append('<li id="' + SizeGroupList_[0].NewID + '" class="carat-li-top">' + SizeGroupList_[0].Size + '<i class="fa fa-times-circle" aria-hidden="true" onclick="NewSizeGroupRemove(' + SizeGroupList_[0].NewID + ');"></i></li>');

        $('#txtfromcarat').val("0");
        $('#txttocarat').val("0");
    }
    else {
        $('#txtfromcarat').val("0");
        $('#txttocarat').val("0");
        toastr.warning($("#hdn_Carat_is_already_exist").val());
    }
    //SetSearchParameter();
}
function NewSizeGroupRemove(id) {
    $('#' + id).remove();
    var SList = _.reject(SizeGroupList, function (e) { return e.NewID == id });
    SizeGroupList = SList;
    //SetSearchParameter();
}
function SetSearchParameter(flag) {
    if (flag == 'savesearch') {
        if ($('#txtSearchName').val() == "") {
            return toastr.warning($("#hdn_Please_Fill_SearchName").val() + '!')
        }
    }

    if (columnDefs.length == 0) {
        BindColumnsSettings();
    }

    var SizeLst = "";
    var CaratType = "";
    SaveSearchList = [];
    var lst = _.filter(CaratList, function (e) { return e.ACTIVE == true });
    if (($('#txtfromcarat').val() != "" && parseFloat($('#txtfromcarat').val()) > 0) && ($('#txttocarat').val() != "" && parseFloat($('#txttocarat').val()) > 0)) {
        NewSizeGroup();
    }
    if (SizeGroupList.length != 0) {
        SizeLst = _.pluck(SizeGroupList, 'Size').join(",");
        CaratType = 'Specific';
    }
    if (lst.length != 0) {
        SizeLst = _.pluck(_.filter(CaratList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
        CaratType = 'General';
    }
    var TripalEX = false;
    var TripalVG = false;
    var cut_Lst = _.pluck(_.filter(CutList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var polish_Lst = _.pluck(_.filter(PolishList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var sym_Lst = _.pluck(_.filter(SymList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    if (cut_Lst == "EX" && polish_Lst == "EX" && sym_Lst == "EX") {
        $('#li3ex').addClass('active');
        TripalEX = true;
    }
    else {
        $('#li3ex').removeClass('active');
        TripalEX = false;
    }
    if (cut_Lst == "EX,VG" && polish_Lst == "EX,VG" && sym_Lst == "EX,VG") {
        $('#li3vg').addClass('active');
        TripalVG = true;
    }
    else {
        $('#li3vg').removeClass('active');
        TripalVG = false;
    }

    var shapeLst = _.pluck(_.filter(ShapeList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var colorLst = _.pluck(_.filter(ColorList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var clarityLst = _.pluck(_.filter(ClarityList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var labLst = _.pluck(_.filter(LabList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var locationLst = _.pluck(_.filter(LocationList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var BGMLst = _.pluck(_.filter(BgmList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var fluoLst = _.pluck(_.filter(FlouList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var BLACKLst = _.pluck(_.filter(BlackList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var tblincLst = _.pluck(_.filter(TblInclList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var tblnattsLst = _.pluck(_.filter(TblNattsList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var crwincLst = _.pluck(_.filter(CrwnInclList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var crwnattsLst = _.pluck(_.filter(CrwnNattsList, function (e) { return e.ACTIVE == true }), 'Value').join(",");
    var milkyLst = _.pluck(_.filter(LusterList, function (e) { return e.ACTIVE == true }), 'Value').join(",");

    var KeyToSymLst_Check = _.pluck(CheckKeyToSymbolList, 'Symbol').join(",");
    var KeyToSymLst_uncheck = _.pluck(UnCheckKeyToSymbolList, 'Symbol').join(",");

    //var KeyToSymLst = KeyToSymLst_Check + '-' + KeyToSymLst_uncheck;

    //StockSearch.FromCarat = StockSearch.FromCarat == "" ? 0 : StockSearch.FromCarat == "NaN" ? 0 : StockSearch.FromCarat;
    //StockSearch.ToCarat = StockSearch.ToCarat == "" ? 0 : StockSearch.ToCarat == "NaN" ? 0 : StockSearch.ToCarat;


    var obj = {
        "Pointer": SizeLst,
        "CaratType": CaratType,
        "Shape": shapeLst,
        "Color": colorLst,
        "Clarity": clarityLst,
        "Cut": cut_Lst,
        "Lab": labLst,
        "Polish": polish_Lst,
        "Location": locationLst,
        "Symm": sym_Lst,
        "BGM": BGMLst,
        "Fls": fluoLst,
        "BLACK": BLACKLst,
        "Inclusion": tblincLst,
        "Natts": tblnattsLst,
        "CrownInclusion": crwincLst,
        "CrownNatts": crwnattsLst,
        "Luster": milkyLst,

        "FromCts": "",
        "ToCts": "",
        "FormPricePerCts": $('#txtPrCtsFrom').val(),
        "ToPricePerCts": $('#txtPrCtsTo').val(),
        "FormDisc": $('#txtDisFrom').val(),
        "ToDisc": $('#txtDisTo').val(),

        "FormRapAmt": "",
        "ToRapAmt": "",

        "FormNetAmt": $('#TotalAmtFrom').val(),
        "ToNetAmt": $('#TotalAmtTo').val(),

        "FromCrownHeight": $('#txtCrHtFrom').val(),
        "ToCrownHeight": $('#txtCrHtTo').val(),

        "FromCrownAngle": $('#txtCrAngFrom').val(),
        "ToCrownAngle": $('#txtCrAngTo').val(),

        "FromPavHeight": $('#txtPavHtFrom').val(),
        "ToPavHeight": $('#txtPavHtTo').val(),

        "FromPavAngle": $('#txtPavAngFrom').val(),
        "ToPavAngle": $('#txtPavAngTo').val(),

        "FormTablePer": $('#txtTablePerFrom').val(),
        "ToTablePer": $('#txtTablePerTo').val(),

        "FormDepthPer": $('#txtDepthPerFrom').val(),
        "ToDepthPer": $('#txtDepthPerTo').val(),

        "FormLength": $('#txtLengthFrom').val(),
        "ToLength": $('#txtLengthTo').val(),

        "FormWidth": $('#txtWidthFrom').val(),
        "ToWidth": $('#txtWidthTo').val(),

        "FormDepth": "",
        "ToDepth": "",

        "StoneID": "",

        "CertiNo": "",
        "SmartSearch": "",
        "ShapeColorPurity": $('#hdnShapeColorPurity').val(),

        "KeyToSymbol": KeyToSymLst_Check + '-' + KeyToSymLst_uncheck,//FinalList_KTS, //KeyToSymLst, //,StockSearch.KeyToSymbol == undefined ? "" : StockSearch.KeyToSymbol,
        "CheckKTS": KeyToSymLst_Check,
        "UNCheckKTS": KeyToSymLst_uncheck,
        "HasImage": $('#SearchImage').hasClass('active') ? true : "",
        "HasHDMovie": $('#SearchVideo').hasClass('active') ? true : "",
        "Shade": "",
        "ReviseStockFlag": "",
        "IsPromotion": "",
        "PageNo": 1,
        "TokenNo": "",
        "StoneStatus": "O",
        "IsTripalEx": TripalEX,
        "IsTripalVg": TripalVG,

    };
    if (flag == 'savesearch') {
        var Searchname = $('#txtSearchName').val();
        $("#save-src-modl").modal("hide");
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        $.ajax({
            url: "/SearchStock/SaveSearchData",
            type: "POST",
            data: { SearchId: $('#hdnSavedSearchId').val(), Searchname: Searchname, model: obj },
            success: function (data, textStatus, jqXHR) {
                if (data.Message.indexOf('Something Went wrong') > -1) {
                    MoveToErrorPage(0);
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
                obj1 = obj;
                GetSearch();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    } else {
        obj1 = obj;
        GetSearch();
    }
}
function BindColumnsSettings() {

    $.ajax({
        url: "/SearchStock/GetColumnsConfigForSearch",
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {

            $.each(data.Data, function (i, item) {
                var headerName = item.Caption;
                var field = item.SPSearchColumn;
                var IsVisible = false;
                var column = {};
                var width = 0;
                if (headerName.length < 6) {
                    width = 60;
                } else if (headerName.length < 10) {
                    width = 65;
                } else if (headerName.length < 13) {
                    width = 70;
                } else {
                    width = 300;
                }


                if (field == 'chk') {
                    column = {
                        headerName: "", field: "",
                        headerCheckboxSelection: true,
                        checkboxSelection: true, width: 28,
                        suppressSorting: true,
                        suppressMenu: true,
                        headerCheckboxSelectionFilteredOnly: true,
                        //headerCellRenderer: selectAllRendererDetail,
                        suppressMovable: false
                    };
                }
                else if (field == 'stone_ref_no') {
                    column = {
                        headerName: $("#hdn_Stock_Id_DNA").val(),
                        field: field,
                        hide: IsVisible,
                        width: 95,
                        cellRenderer: 'deltaIndicator',
                        sortable: false
                    };
                }
                else if (field == 'lab') {
                    column = {
                        headerName: $("#hdn_Lab").val(),
                        field: field,
                        hide: IsVisible,
                        width: 45,
                        cellRenderer: 'labIndicator',
                        sortable: false
                    };
                }
                else if (field == 'CertiTypeLink') {
                    column = {
                        headerName: "Certi Type",
                        field: field,
                        hide: IsVisible,
                        width: 70,
                        cellRenderer: 'CertiTypeLink_Indicator',
                        sortable: false,
                    };
                }
                else if (field == 'StoneStatus') {
                    column = {
                        headerName: $("#hdn_Status").val(),
                        field: field,
                        hide: IsVisible,
                        width: 50,
                        cellRenderer: 'viewIndicator',
                        sortable: false
                    };
                }
                else if (field == 'ImagesLink') {
                    column = {
                        headerName: $("#hdn_View_Image").val(),
                        field: field,
                        hide: IsVisible,
                        width: 80,
                        cellRenderer: 'viewIndicator',
                        sortable: false,
                        suppressSorting: true,
                        suppressMenu: true,
                    };
                }
                else if (field == 'certi_no') {
                    column = {
                        headerName: $("#hdn_Certi_No").val(),
                        field: field,
                        hide: IsVisible,
                        width: 80,
                        Priority: item.Priority,
                        ColumnName: item.ColumnName,
                        iColumnId: item.iColumnId,
                        cellClass: function (params) {
                            if (params.data != undefined) {
                                if ((params.data.Table_Open != null && params.data.Table_Open != 'NN') ||
                                    (params.data.Crown_Open != null && params.data.Crown_Open != 'NN') ||
                                    (params.data.Pav_Open != null && params.data.Pav_Open != 'NN') ||
                                    (params.data.Girdle_Open != null && params.data.Girdle_Open != 'NN')) {
                                    return 'tcpgopen_bg';
                                }
                            }
                        },
                        filter: false
                    };
                }
                else {
                    if (field == 'shape') {
                        width = 95;
                        headerName = $("#hdn_Shape").val();
                    }
                    else if (field == 'certi_no') {
                        width: 80;
                        headerName = $("#hdn_Certi_No").val();
                    }
                    else if (field == 'BGM') {
                        width = 75;
                        headerName = $("#hdn_BGM").val();
                    }
                    else if (field == 'price_per_cts') {
                        headerName = $("#hdn_Price_Cts").val();
                        width = 75;
                    }
                    else if (field == 'Location') {
                        width = 75;
                        headerName = $("#hdn_Location").val();
                    }
                    else if (field == 'pointer') {
                        headerName = $("#hdn_Pointer").val();
                    }
                    else if (field == 'color') {
                        headerName = $("#hdn_Color").val();
                    }
                    else if (field == 'polish') {
                        headerName = $("#hdn_Polish").val();
                    }
                    else if (field == 'fls') {
                        headerName = $("#hdn_Fls").val();
                    }
                    else if (field == 'fls') {
                        headerName = $("#hdn_Fls").val();
                    }
                    else if (field == 'clarity') {
                        headerName = $("#hdn_Clarity").val();
                    }
                    else if (field == 'cts') {
                        headerName = $("#hdn_CTS").val();
                    }
                    else if (field == 'cur_rap_rate') {
                        headerName = $("#hdn_Rap_Price_Doller").val();
                        width = 95;
                    }
                    else if (field == 'rap_amount') {
                        headerName = $("#hdn_Rap_Amt_Doller").val();
                        width = 95;
                    }
                    else if (field == 'sales_disc_per') {
                        width = 90;
                        headerName = $("#hdn_Disc_Per").val();
                    }
                    else if (field == 'net_amount') {
                        width = 95;
                        headerName = $("#hdn_Net_Amt").val();
                    }
                    else if (field == 'Final_Disc') {
                        headerName = $("#hdn_Final_Disc_Per").val();
                        width = 95;
                    }
                    else if (field == 'Final_Value') {
                        headerName = $("#hdn_Final_Value").val();
                        width = 95;
                    }
                    else if (field == 'cut') {
                        headerName = $("#hdn_Cut").val();
                    }
                    else if (field == 'symm') {
                        headerName = $("#hdn_Symm").val();
                    }
                    else if (field == 'length') {
                        headerName = $("#hdn_Length").val();
                    }
                    else if (field == 'width') {
                        headerName = $("#hdn_Width").val();
                    }
                    else if (field == 'depth') {
                        headerName = $("#hdn_Depth").val();
                    }
                    else if (field == 'depth_per') {
                        headerName = $("#hdn_Depth_Per").val();
                    }
                    else if (field == 'table_per') {
                        headerName = $("#hdn_Table_Per").val();
                    }
                    else if (field == 'symbol') {
                        headerName = $("#hdn_Key_to_symbol").val();
                    }
                    else if (field == 'sCulet') {
                        headerName = $("#hdn_Culet").val();
                    }
                    else if (field == 'table_natts') {
                        headerName = $("#hdn_Table_Black").val();
                    }
                    else if (field == 'Crown_Natts') {
                        headerName = $("#hdn_Crown_Natts").val();
                    }
                    else if (field == 'inclusion') {
                        headerName = $("#hdn_Table_White").val();
                    }
                    else if (field == 'Crown_Inclusion') {
                        headerName = $("#hdn_Crown_White").val();
                    }
                    else if (field == 'crown_angle') {
                        headerName = $("#hdn_Crown_Angle").val();
                    }
                    else if (field == 'crown_height') {
                        headerName = $("#hdn_CR_HT").val();
                    }
                    else if (field == 'pav_angle') {
                        headerName = $("#hdn_Pav_Ang").val();
                    }
                    else if (field == 'pav_height') {
                        headerName = $("#hdn_Pav_HT").val();
                    }
                    else if (field == 'pav_height') {
                        headerName = $("#hdn_Pav_HT").val();
                    }
                    else if (field == 'girdle_per') {
                        headerName = $("#hdn_girdle").val() + "(%)";
                        width = 80;
                    }
                    else if (field == 'girdle_type') {
                        headerName = $("#hdn_Girdle_Type").val();
                    }
                    else if (field == 'sInscription') {
                        headerName = $("#hdn_Laser_in_SC").val();
                    }
                    column = {
                        headerName: headerName,
                        field: field,
                        hide: IsVisible,
                        width: width,
                        sortable: true,
                        tooltip: function (params) {
                            if (field == 'cts') {
                                return parseFloat(params.value).toFixed(2);
                            }
                            else if (field == 'sales_disc_per' || field == 'net_amount' || field == 'Final_Disc' || field == 'Final_Value' || field == 'price_per_cts' || field == 'cur_rap_rate' || field == 'rap_amount' || field == 'length' || field == 'width' || field == 'depth' || field == 'depth_per' || field == 'table_per' || field == 'crown_angle' || field == 'crown_height' || field == 'pav_angle' || field == 'pav_height' || field == 'girdle_per') {
                                return formatNumber(params.value);
                            }
                            else {
                                return params.value;
                            }
                        },
                        cellRenderer: function (params) {
                            if (params.data) {
                                if (field == 'cts') {
                                    return parseFloat(params.value).toFixed(2);
                                }
                                else if (field == 'sales_disc_per' || field == 'net_amount' || field == 'Final_Disc' || field == 'Final_Value' || field == 'price_per_cts' || field == 'cur_rap_rate' || field == 'rap_amount' || field == 'length' || field == 'width' || field == 'depth' || field == 'depth_per' || field == 'table_per' || field == 'crown_angle' || field == 'crown_height' || field == 'pav_angle' || field == 'pav_height' || field == 'girdle_per') {
                                    if (params.value != "") {
                                        return formatNumber(params.value);
                                    }
                                }
                                else if (field == 'cut') {
                                    return (params.value == 'FR' ? 'F' : params.value);
                                }
                                else {
                                    return params.value;
                                }
                            }
                        },
                        cellStyle: function (params) {
                            if (params.data) {
                                if (field == 'sales_disc_per' || field == 'net_amount' || field == 'Final_Disc' || field == 'Final_Value') {
                                    return { 'color': 'red', 'font-weight': 'bold', 'font-size': '11px', 'text-align': 'center' };
                                }
                                else if (field == 'cur_rap_rate' || field == 'cts' || field == 'RATIO' || field == 'rap_amount' || field == 'price_per_cts' || field == 'length' || field == 'width' || field == 'depth' || field == 'depth_per' || field == 'table_per' || field == 'crown_angle' || field == 'crown_height' || field == 'pav_angle' || field == 'pav_height' || field == 'girdle_per') {
                                    return { 'color': '#003d66', 'font-size': '11px', 'text-align': 'center' };
                                }
                                else if (params.data.cut == '3EX' && (field == 'cut' || field == 'polish' || field == 'symm')) {
                                    return { 'font-weight': 'bold' };
                                }
                                else {
                                    return { 'font-size': '11px', 'text-align': 'center' };
                                }
                            }
                        },
                    };

                }
                if (field == "net_amount") {
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Disc.(%)",
                        field: "offerDisc",
                        hide: false,
                        width: 100,
                        sortable: false,
                        cellRenderer: 'input_OfferDisc_Indicator',
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Amt($)",
                        field: "offerAmt",
                        width: 140,
                        cellRenderer: 'input_OfferAmt_Indicator',
                        sortable: false,
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Valid Days",
                        field: "validDays",
                        width: 80,
                        cellRenderer: 'input_OfferValidDays_Indicator',
                        sortable: false,
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Remark",
                        field: "Remark",
                        width: 200,
                        cellRenderer: 'input_OfferRemark_Indicator',
                        sortable: false,
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Final Disc.(%)",
                        field: "Offer_Final_Disc",
                        width: 100,
                        cellRenderer: 'input_OfferFinalDisc_Indicator',
                        sortable: false,
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: "Offer Final Amt($)",
                        field: "Offer_Final_Amt",
                        width: 140,
                        cellRenderer: 'input_OfferFinalAmt_Indicator',
                        sortable: false,
                    };
                    columnDefs.push(column);
                    column = {
                        headerName: $("#hdn_Make_an_offer").val(),
                        field: "",
                        hide: IsVisible,
                        width: 80,
                        cellRenderer: 'button_Offer_Indicator',
                        sortable: false,
                        cellClass: ['but-input']
                    };
                    columnDefs.push(column);
                    column = {
                        field: "Maximum_Offer_Amt",
                        hide: true
                    };
                    columnDefs.push(column);
                    column = {
                        field: "Maximum_Offer_Dis",
                        hide: true
                    };
                    columnDefs.push(column);
                } else {
                    columnDefs.push(column);
                }

            });

            setTimeout(function () {
                SetSearchParameter();
                contentHeight();
            }, 2000);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}

function input_OfferDisc_Indicator(params) {
    var offerDisc = '';
    //if (UploadMode != "Excel") {
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no && DiscAmtArray[i][1] != "") {
                offerDisc = formatNumber(DiscAmtArray[i][1]);
            }
        }
    }
    //}
    //else {
    //    offerDisc = (params.data.offerDisc == "0" ? "" : parseFloat(params.data.offerDisc));
    //}

    var element = document.createElement("span");
    element.title = '';

    element.innerHTML = '<input type="text" style="text-align: center;" class="input-inc offerdisc" value = "' + offerDisc
        + '" onkeypress="return isNumberKeyWithNegative(event)" StoneNo = "' + params.data.stone_ref_no
        + '" sales_disc_per="' + params.data.sales_disc_per
        + '" rap_amount = "' + params.data.rap_amount
        + '" net_amount = "' + params.data.net_amount
        + '" Maximum_Offer_Amt = "' + params.data.Maximum_Offer_Amt
        + '" Maximum_Offer_Dis = "' + params.data.Maximum_Offer_Dis
        + '" onblur="OfferDiscount(this);">';

    //element.appendChild(document.createTextNode(params.value));
    return element;



}
function input_OfferAmt_Indicator(params) {
    var offerAmt = '';
    //if (UploadMode != "Excel") {
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no && DiscAmtArray[i][2] != "") {
                offerAmt = formatNumber(DiscAmtArray[i][2]);
            }
        }
    }
    //}
    //else {
    //    offerAmt = (params.data.offerAmt == "0" ? "" : formatNumber(params.data.offerAmt));
    //}


    var element = document.createElement("span");
    element.title = '';

    element.innerHTML = '<input type="text" style="text-align: center;" class="input-inc offeramt" value = "' + offerAmt
        + '" onkeypress="return isNumberKey(event)" StoneNo = "' + params.data.stone_ref_no
        + '" sales_disc_per="' + params.data.sales_disc_per
        + '" rap_amount = "' + params.data.rap_amount
        + '" net_amount = "' + params.data.net_amount
        + '" Maximum_Offer_Amt = "' + params.data.Maximum_Offer_Amt
        + '" Maximum_Offer_Dis = "' + params.data.Maximum_Offer_Dis
        + '" onblur = "OfferAmount(this);" > ';

    //element.appendChild(document.createTextNode(params.value));
    return element;

}
function input_OfferValidDays_Indicator(params) {

    var validDays = '';
    //if (UploadMode != "Excel") {
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no && DiscAmtArray[i][3] != "") {
                validDays = DiscAmtArray[i][3];
            }
        }
    }
    //}
    //else {
    //    validDays = (params.data.validDays == "0" ? "" : params.data.validDays);
    //}

    var element = document.createElement("span");
    element.title = '';

    element.innerHTML = '<input type="text" style="text-align: center;" class="input-inc validdays" StoneNo="' + params.data.stone_ref_no + '" onblur="OfferDays(this);" value="' + validDays
        + '" onkeypress="return isNumberKey(event)">';

    //element.appendChild(document.createTextNode(params.value));
    return element;

}
function input_OfferRemark_Indicator(params) {
    var Remark = '';
    //if (UploadMode != "Excel") {
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no) {
                Remark = (DiscAmtArray[i][4] == null ? "" : DiscAmtArray[i][4]);
            }
        }
    }
    //}
    //else {
    //    validDays = (params.data.validDays == "0" ? "" : params.data.validDays);
    //}

    var element = document.createElement("span");
    element.title = '';

    element.innerHTML = '<input type="text" style="text-align: center;width: 187px;" class="input-inc Remark" StoneNo="' + params.data.stone_ref_no + '" value="' + Remark + '" onblur="OfferRemark(this);">';

    //element.appendChild(document.createTextNode(params.value));
    return element;

}
function input_OfferFinalDisc_Indicator(params) {
    var OfferFinalDisc = '';
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no && DiscAmtArray[i][5] != "") {
                OfferFinalDisc = formatNumber(DiscAmtArray[i][5]);
            }
        }
    }

    var element = document.createElement("span");
    element.title = '';
    element.innerHTML = '<input type="text" style="text-align: center;" class="input-inc OfferFinalDisc" StoneNo="' + params.data.stone_ref_no + '" value="' + OfferFinalDisc + '" disabled>';
    return element;
}
function input_OfferFinalAmt_Indicator(params) {
    var OfferFinalAmt = '';
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == params.data.stone_ref_no && DiscAmtArray[i][6] != "") {
                OfferFinalAmt = formatNumber(DiscAmtArray[i][6]);
            }
        }
    }

    var element = document.createElement("span");
    element.title = '';
    element.innerHTML = '<input type="text" style="text-align: center;" class="input-inc OfferFinalAmt" StoneNo="' + params.data.stone_ref_no + '" value="' + OfferFinalAmt + '" disabled>';
    return element;
}


function OfferDiscount(e) {
    if ($(e).val() != 0 && $(e).val() != "") {
        var StoneNo = $(e).attr("StoneNo");
        var OfferDiscount = parseFloat($(e).val());
        var sales_disc_per = parseFloat($(e).attr("sales_disc_per"));
        var rap_amount = parseFloat($(e).attr("rap_amount"));
        var net_amount = parseFloat($(e).attr("net_amount"));
        var Maximum_Offer_Amt = parseFloat($(e).attr("Maximum_Offer_Amt"));
        var Maximum_Offer_Dis = parseFloat($(e).attr("Maximum_Offer_Dis"));

        const val = parseFloat((OfferDiscount).toFixed(2));
        const min = Math.min(parseFloat((sales_disc_per).toFixed(2)), parseFloat((Maximum_Offer_Dis).toFixed(2)));
        const max = Math.max(parseFloat((sales_disc_per).toFixed(2)), parseFloat((Maximum_Offer_Dis).toFixed(2)));
        
        debugger
        if (val >= min && val <= max) {
            debugger
            var gen_Offer_Amt = parseFloat(((100 + OfferDiscount) * rap_amount / 100).toFixed(2));
            var gen_Offer_Final_Amt = parseFloat((gen_Offer_Amt * 0.9912).toFixed(2));
            var gen_Offer_Final_Disc = parseFloat(((gen_Offer_Final_Amt * 100 / rap_amount) - 100).toFixed(2));

            const _min = Math.min(parseFloat((gen_Offer_Amt).toFixed(2)), parseFloat((Maximum_Offer_Amt).toFixed(2)));
            const _max = Math.max(parseFloat((gen_Offer_Amt).toFixed(2)), parseFloat((Maximum_Offer_Amt).toFixed(2)));

            if (_min <= _max) {
                debugger
                $(e).parent().parent().parent().find('.offeramt').val(formatNumber(gen_Offer_Amt));
                $(e).parent().parent().parent().find('.validdays').val("1");
                $(e).parent().parent().parent().find('.OfferFinalDisc').val(formatNumber(gen_Offer_Final_Disc));
                $(e).parent().parent().parent().find('.OfferFinalAmt').val(formatNumber(gen_Offer_Final_Amt));

                TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), OfferDiscount, gen_Offer_Amt, $(e).parent().parent().parent().find('.validdays').val(), $(e).parent().parent().parent().find('.Remark').val(), gen_Offer_Final_Disc, gen_Offer_Final_Amt, sales_disc_per);
            }
            else {
                debugger
                $(e).parent().parent().parent().find('.offerdisc').val("");
                $(e).parent().parent().parent().find('.offeramt').val("");
                $(e).parent().parent().parent().find('.validdays').val("");
                $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
                $(e).parent().parent().parent().find('.OfferFinalAmt').val("");
                toastr.warning("Your Offer Amount Should not be More than 5% on Net Amt($)");

                TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
            }
        }
        else {
            debugger
            $(e).parent().parent().parent().find('.offerdisc').val("");
            $(e).parent().parent().parent().find('.offeramt').val("");
            $(e).parent().parent().parent().find('.validdays').val("");
            $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
            $(e).parent().parent().parent().find('.OfferFinalAmt').val("");
            //toastr.warning("Offer Disc.(%) must be greater then " + formatNumber(sales_disc_per) + " and less then " + formatNumber(Maximum_Offer_Dis));
            toastr.warning("Your Offer Disc.(%) Should not be More than " + formatNumber(Maximum_Offer_Dis));

            TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
        }
    }
    else {
        $(e).parent().parent().parent().find('.offerdisc').val("");
        $(e).parent().parent().parent().find('.offeramt').val("");
        $(e).parent().parent().parent().find('.validdays').val("");
        $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
        $(e).parent().parent().parent().find('.OfferFinalAmt').val("");

        TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
    }
}
function OfferAmount(e) {
    if ($(e).val() != 0 && $(e).val() != "") {
        var StoneNo = $(e).attr("StoneNo");
        var OfferAmount = parseFloat($(e).val());
        var sales_disc_per = parseFloat($(e).attr("sales_disc_per"));
        var rap_amount = parseFloat($(e).attr("rap_amount"));
        var net_amount = parseFloat($(e).attr("net_amount"));
        var Maximum_Offer_Amt = parseFloat($(e).attr("Maximum_Offer_Amt"));
        var Maximum_Offer_Dis = parseFloat($(e).attr("Maximum_Offer_Dis"));

        const val = parseFloat((OfferAmount).toFixed(2));
        const min = Math.min(parseFloat((Maximum_Offer_Amt).toFixed(2)), parseFloat((net_amount).toFixed(2)));
        const max = Math.max(parseFloat((Maximum_Offer_Amt).toFixed(2)), parseFloat((net_amount).toFixed(2)));

        debugger
        if (val >= min && val <= max) {
            debugger
            var gen_Offer_Disc = parseFloat(((OfferAmount * 100 / rap_amount) - 100).toFixed(2));
            var gen_Offer_Final_Amt = parseFloat((OfferAmount * 0.9912).toFixed(2));
            var gen_Offer_Final_Disc = parseFloat(((gen_Offer_Final_Amt * 100 / rap_amount) - 100).toFixed(2));

            const _min = Math.min(parseFloat((OfferAmount).toFixed(2)), parseFloat((Maximum_Offer_Amt).toFixed(2)));
            const _max = Math.max(parseFloat((OfferAmount).toFixed(2)), parseFloat((Maximum_Offer_Amt).toFixed(2)));

            if (_min <= _max) {
                debugger
                $(e).parent().parent().parent().find('.offerdisc').val(formatNumber(gen_Offer_Disc));
                $(e).parent().parent().parent().find('.validdays').val("1");
                $(e).parent().parent().parent().find('.OfferFinalDisc').val(formatNumber(gen_Offer_Final_Disc));
                $(e).parent().parent().parent().find('.OfferFinalAmt').val(formatNumber(gen_Offer_Final_Amt));

                TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), gen_Offer_Disc, OfferAmount, $(e).parent().parent().parent().find('.validdays').val(), $(e).parent().parent().parent().find('.Remark').val(), gen_Offer_Final_Disc, gen_Offer_Final_Amt, sales_disc_per);
            }
            else {
                debugger
                $(e).parent().parent().parent().find('.offerdisc').val("");
                $(e).parent().parent().parent().find('.offeramt').val("");
                $(e).parent().parent().parent().find('.validdays').val("");
                $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
                $(e).parent().parent().parent().find('.OfferFinalAmt').val("");
                //toastr.warning("Offer Disc.(%) must be greater then " + formatNumber(sales_disc_per) + " and less then " + formatNumber(Maximum_Offer_Dis));
                toastr.warning("Your Offer Amount Should not be More than 5% on Net Amt($)");

                TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
            }
        }
        else {
            debugger
            $(e).parent().parent().parent().find('.offerdisc').val("");
            $(e).parent().parent().parent().find('.offeramt').val("");
            $(e).parent().parent().parent().find('.validdays').val("");
            $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
            $(e).parent().parent().parent().find('.OfferFinalAmt').val("");
            //toastr.warning("Offer Disc.(%) must be greater then " + formatNumber(sales_disc_per) + " and less then " + formatNumber(Maximum_Offer_Dis));
            toastr.warning("Your Offer Amount Should not be More than 5% on Net Amt($)");

            TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
        }
    }
    else {
        $(e).parent().parent().parent().find('.offerdisc').val("");
        $(e).parent().parent().parent().find('.offeramt').val("");
        $(e).parent().parent().parent().find('.validdays').val("");
        $(e).parent().parent().parent().find('.OfferFinalDisc').val("");
        $(e).parent().parent().parent().find('.OfferFinalAmt').val("");

        TEMP_SAVE("DISC_AMT", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
    }
}
function OfferDays(e) {
    TEMP_SAVE("DAY", $(e).attr("StoneNo"), "", "", $(e).parent().parent().parent().find('.validdays').val(), "", "", "", "");
}
function OfferRemark(e) {
    TEMP_SAVE("REMARK", $(e).attr("StoneNo"), "", "", "", $(e).parent().parent().parent().find('.Remark').val(), "", "", "");
}
function button_Offer_Indicator(params) {
    var element = document.createElement("span");
    element.title = '';
    element.innerHTML = '<button style="margin: 1px 2px 4px 0px;line-height: 9px;" StoneNo="' + params.data.stone_ref_no + '" onclick="SingleSaveOffer(this)" class="offer-btn">APPLY</button>';

    //element.appendChild(document.createTextNode(params.value));
    return element;

}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}
function isNumberKeyWithNegative(evt) {

    var charCode = (evt.which) ? evt.which : evt.keyCode;

    if (charCode == 45)
        return true;

    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function deltaIndicator(params) {
    //return '<div class="stock-font"><a target="_blank" href="http://cdn1.brainwaves.co.in/DNA/StoneDetail?StoneNo=' + params.data.stone_ref_no + '">' + params.value + '</a></div>';
    //return '<div class="stock-font"><a target="_blank" href="/DNA/StoneDetail?StoneNo=' + params.data.stone_ref_no + '">' + params.value + '</a></div>';
    return '<div class="stock-font"><a target="_blank" href="https://4e0s0i2r4n0u1s0.com/clientvideo/viewdetail.html?StoneNo=' + params.data.stone_ref_no + '">' + params.value + '</a></div>';
}
function viewIndicator(params) {

    return params.value;

}
function ResetOffer() {
    $('.validdays').val("");
    $('.offeramt').val("");
    $('.offerdisc').val("");
    DiscAmtArray = [];
}
function SingleSaveOffer(e) {
    debugger
    Offer_StoneList = [];
    var StoneNo = $(e).attr('StoneNo');
    for (var i = 0; i < DiscAmtArray.length; i++) {
        debugger
        if (DiscAmtArray[i][0] == StoneNo && DiscAmtArray[i][1] != "" && DiscAmtArray[i][2] != "") {
            Offer_StoneList.push({ StoneNo: DiscAmtArray[i][0] });
        }
    }
    debugger
    Offer_StoneList = multiDimensionalUnique(Offer_StoneList);
    StoneNo = _.pluck(Offer_StoneList, 'StoneNo').join(", ");
    if (StoneNo == "") {
        debugger
        toastr.warning("Offer Disc.(%) and Offer Amt($) is required");
    }
    else {
        debugger
        if ($("#hdnUserType").val() == "3") {
            debugger
            $('#OfferStoneModal').modal('hide');
            Offer_ListGenerate(Offer_StoneList);
        }
        else {
            debugger
            $('#OfferStoneModal #___pOfferStoneMsg').html("Please select a company for Offer, Stone Id " + StoneNo + "...!");
            $('#OfferStoneModal').modal('show');
        }
    }
}
function Make_Offer() {
    debugger
    var selectedRows = gridOptions.api.getSelectedRows();
    Offer_StoneList = [];
    var StoneNo = ""; debugger
    if (selectedRows.length == "") {
        debugger
        toastr.warning($("#hdn_No_stone_selected_for_Save_Offer").val());
        return false;
    }
    else {
        debugger
        for (var j = 0; j < selectedRows.length; j++) {
            debugger
            for (var i = 0; i < DiscAmtArray.length; i++) {
                debugger
                if (DiscAmtArray[i][0] == selectedRows[j].stone_ref_no && DiscAmtArray[i][1] != "" && DiscAmtArray[i][2] != "") {
                    Offer_StoneList.push({ StoneNo: DiscAmtArray[i][0] });
                }
            }
        }
        debugger
        Offer_StoneList = multiDimensionalUnique(Offer_StoneList);
        StoneNo = _.pluck(Offer_StoneList, 'StoneNo').join(", ");
        if (StoneNo == "") {
            debugger
            toastr.warning("Offer Disc.(%) and Offer Amt($) is required");
        }
        else {
            debugger
            if ($("#hdnUserType").val() == "3") {
                debugger
                $('#OfferStoneModal').modal('hide');
                Offer_ListGenerate(Offer_StoneList);
            }
            else {
                debugger
                $('#OfferStoneModal #___pOfferStoneMsg').html("Please select a company for Offer, Stone Id " + StoneNo + "...!");
                $('#OfferStoneModal').modal('show');
            }
        }
    }
}
function Offer_ListGenerate(SelectStoneList) {
    debugger
    var List = [];

    if (SelectStoneList.length > 0) {
        debugger
        for (var j = 0; j < SelectStoneList.length; j++) {
            debugger
            for (var i = 0; i < DiscAmtArray.length; i++) {
                debugger
                if (DiscAmtArray[i][0] == SelectStoneList[j].StoneNo && DiscAmtArray[i][1] != "" && DiscAmtArray[i][2] != "") {
                    List.push({
                        StoneNo: DiscAmtArray[i][0],
                        Offer_Discount: DiscAmtArray[i][1],
                        Offer_Amount: DiscAmtArray[i][2],
                        Valid_Days: DiscAmtArray[i][3],
                        Remark: DiscAmtArray[i][4],
                        Offer_Final_Discount: DiscAmtArray[i][5],
                        Offer_Final_Amount: DiscAmtArray[i][6],
                        Discount: DiscAmtArray[i][7]
                    });
                }
            }
        }
    }
    debugger
    if (List.length > 0) {
        debugger
        SaveOffer(List, (___company_Userid != "" ? ___company_Userid : $("#hdnUserId").val()));
    }
    else {
        debugger
        toastr.warning("Offer Disc.(%) and Offer Amt($) is required");
    }
}

function OfferStoneModal_BtnClick() {
    debugger
    ___company_Userid = '', ___company_Fortunecode = '';
    if ($("#___txtCompanyName_hidden").val().split("__").length != 2) {
        debugger
        $("#___txtCompanyName_hidden").val("");
        $("#___txtCompanyName").val("");
        $("#___txtCompanyName").focus();
        toastr.warning("Select Company Name");
        return;
    }
    else {
        debugger
        if ($("#___txtCompanyName_hidden").val().split("__").length == 2) {
            debugger
            ___company_Userid = $("#___txtCompanyName_hidden").val().split("__")[0];
            ___company_Fortunecode = $("#___txtCompanyName_hidden").val().split("__")[1];
            debugger
            if (___company_Userid != "" && ___company_Fortunecode != "") {
                debugger
                if (Offer_StoneList.length > 0) {
                    debugger
                    Offer_ListGenerate(Offer_StoneList);
                }
            }
        }
    }
}
function SaveOffer(List, UserID) {
    debugger
    if (List.length > 0) {
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();

        setTimeout(function () {
            var obj = {};
            obj.UserID = UserID;
            obj.StoneList = List;
            debugger

            $.ajax({
                url: "/Offer/SaveOfferCriteria",
                async: false,
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ req: obj }),
                contentType: "application/json; charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    debugger
                    data.Message = data.Message.replace('Your offer was not uploaded due to out of discount Range. Kindly try again.', $("#hdn_Your_offer_was_not_uploaded_due_to_out_of_discount_Range_Kindly_try_again").val());
                    data.Message = data.Message.replace('Your offer was partially uploaded. Below are the stones which are not uploaded.', $("#hdn_Your_offer_was_partially_uploaded_Below_are_the_stones_which_are_not_uploaded").val());
                    data.Message = data.Message.replace('Range of discount is not valid for the stones', $("#hdn_Range_of_dicount_is_not_valid_for_the_stones").val());
                    data.Message = data.Message.replace('Please change discount and reupload', $("#hdn_Please_change_discount_and_reupload").val());
                    data.Message = data.Message.replace('Offer already received for the stones', $("#hdn_Offer_already_received_for_the_stones").val());
                    data.Message = data.Message.replace('Please change discount and reupload it', $("#hdn_Please_change_discount_and_reupload_it").val());
                    data.Message = data.Message.replace('You have already upload discount for the stones', $("#hdn_You_have_already_upload_discount_for_the_stones").val());
                    data.Message = data.Message.replace('Offer uploaded Successfully', $("#hdn_Offer_uploaded_Successfully").val());
                    if (data.Status == "0") {
                        if (data.Message.indexOf('Something Went wrong') > -1) {
                            MoveToErrorPage(0);
                        }
                        toastr.error(data.Message);
                    } else {
                        toastr.success(data.Message);
                        GetSearch();
                        $('#OfferStoneModal').modal('hide');
                    }
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                }
            });
        }, 20);
    }
    else {
        toastr.warning("Offer Disc.(%) and Offer Amt($) is required");
    }
}



function CloseOfferPopup() {
    $('#ConfirmOfferModal').modal('hide');
}
function CloseExcel_Stone_InvalidPopup() {
    $('#Excel_Stone_Invalid_Modal').modal('hide');
}




function UploadExcelFile() {

    var file = document.getElementById('fileadd').files[0];
    if (file == undefined) {
        return toastr.warning($("#hdn_Please_Select_Excel_File_For_Upload").val());
    }

    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    var formData = new FormData();
    formData.append("file", file);

    formData.append('ExcelFileName', $('#txtfilename').val());
    $.ajax({
        url: "/Offer/UploadExcelforOffer",
        processData: false,
        contentType: false,
        type: "POST",
        data: formData,
        success: function (data, textStatus, jqXHR) {
            var Invalid_Stone_Body = data[data.length - 1].sComments;
            data.splice(data.length - 1);

            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
            var gridDiv = document.querySelector('#myGrid');
            if (gridOptions.api != undefined) {
                gridOptions.api.destroy();
            }
            gridOptions = {
                masterDetail: true,
                detailCellRenderer: 'myDetailCellRenderer',
                detailRowHeight: 70,
                groupDefaultExpanded: 0,
                components: {
                    //countryIndicator: countryIndicator,
                    deltaIndicator: deltaIndicator,
                    statusIndicator: statusIndicator,
                    labIndicator: labIndicator,
                    CertiTypeLink_Indicator: CertiTypeLink_Indicator,
                    viewIndicator: viewIndicator,
                    input_OfferDisc_Indicator: input_OfferDisc_Indicator,
                    input_OfferAmt_Indicator: input_OfferAmt_Indicator,
                    input_OfferValidDays_Indicator: input_OfferValidDays_Indicator,
                    input_OfferRemark_Indicator: input_OfferRemark_Indicator,
                    input_OfferFinalDisc_Indicator: input_OfferFinalDisc_Indicator,
                    input_OfferFinalAmt_Indicator: input_OfferFinalAmt_Indicator,
                    button_Offer_Indicator: button_Offer_Indicator,
                    //agColumnHeader: CustomHeader,
                    myDetailCellRenderer: DetailCellRenderer
                },
                defaultColDef: {
                    enableSorting: true,
                    sortable: true,
                    resizable: true
                },
                pagination: true,
                icons: {
                    groupExpanded:
                        '<i class="fa fa-minus-circle"/>',
                    groupContracted:
                        '<i class="fa fa-plus-circle"/>'
                },
                rowSelection: 'multiple',
                suppressRowClickSelection: true,
                onSelectionChanged: onSelectionChanged,
                columnDefs: columnDefs,
                rowData: data,
                cacheBlockSize: 50, // you can have your custom page size
                paginationPageSize: 50, //pagesize
                getContextMenuItems: getContextMenuItems,
                paginationNumberFormatter: function (params) {
                    return '[' + params.value.toLocaleString() + ']';
                }
            };
            new agGrid.Grid(gridDiv, gridOptions);
            CountSummary(data);
            $("#fileadd").val("");

            if (Invalid_Stone_Body != '' && Invalid_Stone_Body != 'undefined') {
                Excel_Stone_Invalid(Invalid_Stone_Body);
            }

            DiscAmtArray = [];
            for (var i = 0; i < data.length; i++) {
                if (data[i].offerDisc != "0") {
                    var offerDisc = (data[i].offerDisc != "" ? parseFloat(data[i].offerDisc).toFixed(2) : "");
                    var offerAmt = (data[i].offerAmt != "" ? parseFloat(data[i].offerAmt).toFixed(2) : "")
                    var Offer_Final_Discount = (data[i].Offer_Final_Discount != "" ? parseFloat(data[i].Offer_Final_Discount).toFixed(2) : "")
                    var Offer_Final_Amount = (data[i].Offer_Final_Amount != "" ? parseFloat(data[i].Offer_Final_Amount).toFixed(2) : "")
                    var sales_disc_per = (data[i].sales_disc_per != "" ? parseFloat(data[i].sales_disc_per).toFixed(2) : "")

                    DiscAmtArray.push([data[i].stone_ref_no, offerDisc, offerAmt, data[i].validDays, data[i].Offer_Remark, Offer_Final_Discount, Offer_Final_Amount, sales_disc_per]);
                }
            }

            UploadMode = "Excel";
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();

        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function Excel_Stone_Invalid(Body) {
    var str = Body;
    str = str.replace(/&lt;/g, '<');
    str = str.replace(/&gt;/g, '>');

    var final_msg = ''
    final_msg = '<label class="offerComment" style="word-break:break-word;">Below Stone(s) are out of Dis(%) Range !!</label>';
    final_msg += str;
    $("#Excel_Stone_Invalid_Modal .form-group").html(final_msg);
    $('#Excel_Stone_Invalid_Modal').modal('show');
}
function CountSummary(data) {


    var Totalpcs = 0;
    var TotalWeight = 0.0;
    var Rapusd = 0.0;
    var TotalAmt = 0.0;
    var selectedamt = 0.0;
    var RapVlaRow = 0.0;
    var TotalPricePerCts = 0.0;
    debugger

    var SearchResultData = data;//Filtered_Data.length>0?Filtered_Data:gridOptions.api.getSelectedRows();
    if (SearchResultData.length != 0) {
        for (var i = 0; i < SearchResultData.length; i++) {
            Totalpcs = Totalpcs + 1;
            TotalWeight += parseFloat(SearchResultData[i].cts);
            TotalAmt += parseFloat(SearchResultData[i].net_amount);
            RapVlaRow += parseFloat(SearchResultData[i].cur_rap_rate) * parseFloat(SearchResultData[i].cts);
        }
        TotalPricePerCts = parseFloat(TotalWeight) > 0 ? (parseFloat(TotalAmt) / parseFloat(TotalWeight).toFixed(2)) : 0;
        Rapusd = parseFloat(TotalWeight) > 0 ? (parseFloat(RapVlaRow) / parseFloat(TotalWeight).toFixed(2)) : 0;
        // AvgDis = parseFloat(Rapusd) > 0 ? ((parseFloat(parseFloat(Rapusd) - parseFloat(TotalPricePerCts))) / parseFloat(Rapusd)) * (-100).toFixed(2) : 0;
        AvgDis = parseFloat(Rapusd) > 0 ? ((parseFloat(parseFloat(Rapusd) - parseFloat(TotalPricePerCts))) / parseFloat(Rapusd)) * (-100).toFixed(2) : 0;

    }
    debugger
    setTimeout(function () {
        $('#tabcts').html(formatNumber(TotalWeight));
        $('#tabdisc').html(formatNumber(AvgDis));
        $('#tabppcts').html(formatNumber(TotalPricePerCts));
        $('#tabtotAmt').html(formatNumber(TotalAmt));
        $('#tabpcs').html(Totalpcs);
    });
}

function statusIndicator(params) {
    var value = '';
    if (params.value == 'AVAILABLE') {
        value = 'A';
    } else if (params.value == 'AVAILABLE OFFER') {
        value = 'O';
    }
    var element = document.createElement("span");
    element.appendChild(document.createTextNode(value));
    return element;
}
function labIndicator(params) {
    if (params.data != undefined) {
        if (params.value == "GIA") {
            return '<a href="http://www.gia.edu/cs/Satellite?pagename=GST%2FDispatcher&childpagename=GIA%2FPage%2FReportCheck&c=Page&cid=1355954554547&reportno=' + params.data.certi_no + '" target="_blank" style="text-decoration: underline; color :blue;">' + params.value + '</a>';
        }
        else if (params.value == "HRD") {
            return '<a href="https://my.hrdantwerp.com/?id=34&record_number=' + params.data.certi_no + '" target="_blank" style="text-decoration: underline; color :blue;">' + params.value + '</a>';
        }
        else if (params.value == "IGI") {
            return '<a href="https://www.igi.org/reports/verify-your-report?r=' + params.data.certi_no + '" target="_blank" style="text-decoration: underline; color :blue;">' + params.value + '</a>';
        }
        else {
            return '';
        }
    }
    else {
        return '';
    }
}
function isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[1] === params.column;
    return thisIsFirstColumn;
}
function StatusFilter(status) {
    obj1.StoneStatus = status;
    gridOptions.api.setServerSideDatasource(datasource1);
}
function setFromCarat() {
    if ($('#txtfromcarat').val() != "") {
        $('#txtfromcarat').val(parseFloat($('#txtfromcarat').val()).toFixed(2));
        $('#txttocarat').val(parseFloat($('#txtfromcarat').val()).toFixed(2));
    } else {
        $('#txtfromcarat').val("0");
    }
    if ($('#txttocarat').val() == "") {
        $('#txttocarat').val("0");
    }
}
function setToCarat() {
    if ($('#txttocarat').val() != "") {
        $('#txttocarat').val(parseFloat($('#txttocarat').val()).toFixed(2));
    } else {
        $('#txttocarat').val("0");
    }
    if ($('#txtfromcarat').val() == "") {
        $('#txtfromcarat').val("0");
    }
}
function resetKeytoSymbol() {
    CheckKeyToSymbolList = [];
    UnCheckKeyToSymbolList = [];
    $('#spanselected').html('' + CheckKeyToSymbolList.length + ' - Selected');
    $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + ' - Deselected');
    $('#searchkeytosymbol input[type="radio"]').prop('checked', false);
}
/*-------------------------------------------------------- GET SEARCH AND AGGRID BINDING START------------------------------------------------*/
function onSelectionChanged(params) {
    var TOT_CTS = 0;
    var AVG_SALES_DISC_PER = 0;
    var AVG_PRICE_PER_CTS = 0;
    var TOT_NET_AMOUNT = 0;
    var TOT_PCS = 0;
    var TOT_RAP_AMOUNT = 0;
    if (gridOptions.api.getSelectedRows().length > 0) {
        TOT_CTS = _.reduce(_.pluck(gridOptions.api.getSelectedRows(), 'cts'), function (memo, num) { return memo + num; }, 0);
        TOT_NET_AMOUNT = _.reduce(_.pluck(gridOptions.api.getSelectedRows(), 'net_amount'), function (memo, num) { return memo + num; }, 0);
        TOT_RAP_AMOUNT = _.reduce(_.pluck(gridOptions.api.getSelectedRows(), 'rap_amount'), function (memo, num) { return memo + num; }, 0);
        AVG_SALES_DISC_PER = (-1 * (((TOT_RAP_AMOUNT - TOT_NET_AMOUNT) / TOT_RAP_AMOUNT) * 100)).toFixed(2);
        AVG_PRICE_PER_CTS = TOT_NET_AMOUNT / TOT_CTS;
        TOT_PCS = gridOptions.api.getSelectedRows().length;
    } else {
        TOT_CTS = (summary1.TOT_CTS == undefined ? 0 : summary1.TOT_CTS);
        AVG_SALES_DISC_PER = (summary1.AVG_SALES_DISC_PER == undefined ? 0 : summary1.AVG_SALES_DISC_PER);
        AVG_PRICE_PER_CTS = (summary1.AVG_PRICE_PER_CTS == undefined ? 0 : summary1.AVG_PRICE_PER_CTS);
        TOT_NET_AMOUNT = (summary1.TOT_NET_AMOUNT == undefined ? 0 : summary1.TOT_NET_AMOUNT);
        TOT_PCS = (summary1.TOT_PCS == undefined ? 0 : summary1.TOT_PCS);
    }
    
    $('#tabcts').html(formatNumber(TOT_CTS));
    $('#tabdisc').html(formatNumber(AVG_SALES_DISC_PER));
    $('#tabppcts').html(formatNumber(AVG_PRICE_PER_CTS));
    $('#tabtotAmt').html(formatNumber(TOT_NET_AMOUNT));
    $('#tabpcs').html(formatIntNumber(TOT_PCS));
}
function columnVisible(params) {
    if (params.column.colId == 0 && params.visible) {
        $('#myGrid .ag-header-cell[col-id="0"] .ag-header-select-all').removeClass('ag-hidden');
        $($('#myGrid .ag-header-select-all')[1]).click(function () {
            if ($(this).find('.ag-icon').hasClass('ag-icon-checkbox-unchecked')) {
                gridOptions.api.forEachNode(function (node) {
                    node.setSelected(false);
                });
            } else {
                gridOptions.api.forEachNode(function (node) {
                    node.setSelected(true);
                });
            }
            onSelectionChanged();
        });

    }
}
function onBodyScroll(params) {

    $('#myGrid .ag-header-cell[col-id="0"] .ag-header-select-all').removeClass('ag-hidden');
    $('#myGrid .ag-header-cell[col-id="0"] .ag-header-select-all').click(function () {
        if ($(this).find('.ag-icon').hasClass('ag-icon-checkbox-unchecked')) {
            gridOptions.api.forEachNode(function (node) {
                node.setSelected(false);
            });
        } else {
            gridOptions.api.forEachNode(function (node) {
                node.setSelected(true);
            });
        }
        onSelectionChanged();
    });

}
function onGridReady(params) {
    var allColumnIds = [];
    gridOptions.columnApi.getAllColumns().forEach(function (column) {
        allColumnIds.push(column.colId);
    });

    gridOptions.columnApi.autoSizeColumns(allColumnIds, false);
}
function GetSearch() {

    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();

    $('#result').css('display', 'block');
    $('#result').addClass('active');
    $('#profile-tab').addClass('active show');
    $('#tabhome').removeClass('active');
    $('#home-tab2').removeClass('active show');
    $('#iresult').show();
    var gridDiv = document.querySelector('#myGrid');
    if (gridOptions.api != undefined) {
        gridOptions.api.destroy();
    }
    gridOptions = {
        masterDetail: true,
        detailCellRenderer: 'myDetailCellRenderer',
        detailRowHeight: 70,
        groupDefaultExpanded: 1,
        components: {
            deltaIndicator: deltaIndicator,
            statusIndicator: statusIndicator,
            labIndicator: labIndicator,
            CertiTypeLink_Indicator: CertiTypeLink_Indicator,
            viewIndicator: viewIndicator,
            input_OfferDisc_Indicator: input_OfferDisc_Indicator,
            input_OfferAmt_Indicator: input_OfferAmt_Indicator,
            input_OfferValidDays_Indicator: input_OfferValidDays_Indicator,
            input_OfferRemark_Indicator: input_OfferRemark_Indicator,
            input_OfferFinalDisc_Indicator: input_OfferFinalDisc_Indicator,
            input_OfferFinalAmt_Indicator: input_OfferFinalAmt_Indicator,
            button_Offer_Indicator: button_Offer_Indicator,
            //agColumnHeader: CustomHeader,
            myDetailCellRenderer: DetailCellRenderer,
            //countryIndicator: countryIndicator
        },
        defaultColDef: {
            enableSorting: true,
            sortable: true,
            resizable: true
        },
        pagination: true,
        icons: {
            groupExpanded:
                '<i class="fa fa-minus-circle"/>',
            groupContracted:
                '<i class="fa fa-plus-circle"/>'
        },
        rowSelection: 'multiple',
        overlayLoadingTemplate: '<span class="ag-overlay-loading-center">NO DATA TO SHOW..</span>',
        suppressRowClickSelection: true,
        columnDefs: columnDefs,
        rowModelType: 'serverSide',
        onColumnVisible: columnVisible,
        //onGridReady: onGridReady,
        onSelectionChanged: onSelectionChanged,
        onBodyScroll: onBodyScroll,
        cacheBlockSize: 50, // you can have your custom page size
        paginationPageSize: 50, //pagesize
        getContextMenuItems: getContextMenuItems,
        paginationNumberFormatter: function (params) {
            return '[' + params.value.toLocaleString() + ']';
        }
    };
    new agGrid.Grid(gridDiv, gridOptions);
    $('#myGrid .ag-header-cell[col-id="0"] .ag-header-select-all').click(function () {
        if ($(this).find('.ag-icon').hasClass('ag-icon-checkbox-unchecked')) {
            gridOptions.api.forEachNode(function (node) {
                node.setSelected(false);
            });
        } else {
            gridOptions.api.forEachNode(function (node) {
                node.setSelected(true);
            });
        }
        onSelectionChanged();
    });

    gridOptions.api.setServerSideDatasource(datasource1);

    setTimeout(function () {
        $('#myGrid .ag-header-cell[col-id="0"] .ag-header-select-all').removeClass('ag-hidden');
        $('.show_entry').appendTo('.ag-paging-panel');
        $('.gridpage_info').appendTo('.ag-paging-panel');
    }, 100);

    ResetActive();
    IndicatorCalled = 1;
}
/*-------------------------------------------------------- GET SEARCH AND AGGRID BINDING END--------------------------------------------------*/

/*--------------------------------------------------------SET RESET ACTIVE END--------------------------------------------------*/

/*--------------------------------------------------------SET RESET ACTIVE START------------------------------------------------*/
function ResetActive() {
    _.map(ShapeList, function (data) { return data.ACTIVE = false; });
    _.map(ColorList, function (data) { return data.ACTIVE = false; });
    _.map(ClarityList, function (data) { return data.ACTIVE = false; });
    _.map(CutList, function (data) { return data.ACTIVE = false; });
    _.map(LabList, function (data) { return data.ACTIVE = false; });
    _.map(PolishList, function (data) { return data.ACTIVE = false; });
    _.map(SymList, function (data) { return data.ACTIVE = false; });
    _.map(FlouList, function (data) { return data.ACTIVE = false; });
    _.map(BgmList, function (data) { return data.ACTIVE = false; });
    _.map(TblInclList, function (data) { return data.ACTIVE = false; });
    _.map(TblNattsList, function (data) { return data.ACTIVE = false; });
    _.map(CrwnInclList, function (data) { return data.ACTIVE = false; });
    _.map(CrwnNattsList, function (data) { return data.ACTIVE = false; });
    _.map(LocationList, function (data) { return data.ACTIVE = false; });
    _.map(CaratList, function (data) { return data.ACTIVE = false; });

    CheckKeyToSymbolList = [];
    UnCheckKeyToSymbolList = [];
    $('a[href="#carat1"]').click();
    $('#spanselected').html('' + CheckKeyToSymbolList.length + ' - Selected');
    $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + ' - Deselected');
    $('#searchkeytosymbol input[type="radio"]').prop('checked', false);
    $('#SearchImage').removeClass('active');
    $('#SearchVideo').removeClass('active');
    $('#li3ex').removeClass('active');
    $('#li3vg').removeClass('active');
    SizeGroupList = [];
    $('#searchcaratspecific').html("");

    $('#searchcaratgen').html("");
    _(CaratList).each(function (carat, i) {
        $('#searchcaratgen').append('<li onclick="SetActive(\'carat\',\'' + carat.Value + '\')">' + carat.Value + '</li>');
    });

    $('#searchshape').html("");
    _(ShapeList).each(function (shape, i) {
        $('#searchshape').append('<li class="wow zoomIn animated" data-wow-delay="0.8s"><a href="javascript:void(0);" onclick="SetActive(\'Shape\',\'' + shape.Value + '\')" class="common-ico"><div class="icon-image one"><img src="' + shape.UrlValue + '" class="first-ico"><img src="' + shape.UrlValueHov + '" class="second-ico"></div><span>' + shape.Value + '</span></a></li>');
    });

    $('#searchcolor').html("");
    _(ColorList).each(function (color, i) {
        $('#searchcolor').append('<li onclick="SetActive(\'COLOR\',\'' + color.Value + '\')">' + color.Value + '</li>');
    });

    $('#searchclarity').html("");
    _(ClarityList).each(function (clarity, i) {
        $('#searchclarity').append('<li onclick="SetActive(\'CLARITY\',\'' + clarity.Value + '\')">' + clarity.Value + '</li>');
    });

    $('#searchcut').html("");
    _(CutList).each(function (cut, i) {
        $('#searchcut').append('<li onclick="SetActive(\'CUT\',\'' + cut.Value + '\')">' + cut.Value + '</li>');
    });

    $('#searchlab').html("");
    _(LabList).each(function (lab, i) {
        $('#searchlab').append('<li onclick="SetActive(\'LAB\',\'' + lab.Value + '\')">' + lab.Value + '</li>');
    });

    $('#searchpolish').html("");
    _(PolishList).each(function (polish, i) {
        $('#searchpolish').append('<li onclick="SetActive(\'POLISH\',\'' + polish.Value + '\')">' + polish.Value + '</li>');
    });

    $('#searchsymm').html("");
    _(SymList).each(function (sym, i) {
        $('#searchsymm').append('<li onclick="SetActive(\'SYMM\',\'' + sym.Value + '\')">' + sym.Value + '</li>');
    });

    $('#searchfls').html("");
    _(FlouList).each(function (fls, i) {
        $('#searchfls').append('<li onclick="SetActive(\'FLS\',\'' + fls.Value + '\')">' + fls.Value + '</li>');
    });

    $('#searchbgm').html("");
    _(BgmList).each(function (bgm, i) {
        $('#searchbgm').append('<li onclick="SetActive(\'BGM\',\'' + bgm.Value + '\')">' + bgm.Value + '</li>');
    });

    BlackList = _.filter(ParameterList, function (e) { return e.ListType == 'BLACK' });

    $('#searchtableincl').html("");
    _(TblInclList).each(function (tblincl, i) {
        $('#searchtableincl').append('<li onclick="SetActive(\'TABLE_INCL\',\'' + tblincl.Value + '\')">' + tblincl.Value + '</li>');
    });

    $('#searchtablenatts').html("");
    _(TblNattsList).each(function (tblnatts, i) {
        $('#searchtablenatts').append('<li onclick="SetActive(\'TABLE_NATTS\',\'' + tblnatts.Value + '\')">' + tblnatts.Value + '</li>');
    });

    $('#searchcrownincl').html("");
    _(CrwnInclList).each(function (crwnincl, i) {
        $('#searchcrownincl').append('<li onclick="SetActive(\'CROWN_INCL\',\'' + crwnincl.Value + '\')">' + crwnincl.Value + '</li>');
    });

    $('#searchcrownnatts').html("");
    _(CrwnNattsList).each(function (crwnnatt, i) {
        $('#searchcrownnatts').append('<li onclick="SetActive(\'CROWN_NATTS\',\'' + crwnnatt.Value + '\')">' + crwnnatt.Value + '</li>');
    });

    $('#searchlocation').html("");
    _(LocationList).each(function (loc, i) {
        $('#searchlocation').append('<li onclick="SetActive(\'Location\',\'' + loc.Value + '\')">' + loc.Value + '</li>');
    });

    $('#hdnSavedSearchId').val("0");
    $('#txtSearchName').val("");
    $('#txtfromcarat').val("");
    $('#txttocarat').val("");
    $('#txtDisFrom').val("");
    $('#txtDisTo').val("");
    $('#txtPrCtsFrom').val("");
    $('#txtPrCtsTo').val("");
    $('#TotalAmtFrom').val("");
    $('#TotalAmtTo').val("");
    $('#txtLengthFrom').val("");
    $('#txtLengthTo').val("");
    $('#txtWidthFrom').val("");
    $('#txtWidthTo').val("");
    $('#txtDepthPerFrom').val("");
    $('#txtDepthPerTo').val("");
    $('#txtTablePerFrom').val("");
    $('#txtTablePerTo').val("");
    $('#txtCrAngFrom').val("");
    $('#txtCrAngTo').val("");
    $('#txtCrHtFrom').val("");
    $('#txtCrHtTo').val("");
    $('#txtPavAngFrom').val("");
    $('#txtPavAngTo').val("");
    $('#txtPavHtFrom').val("");
    $('#txtPavHtTo').val("");
}

function SetActive(flag, value) {

    if (flag == "Shape") {
        if (_.find(ShapeList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(ShapeList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(ShapeList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "COLOR") {
        if (_.find(ColorList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(ColorList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(ColorList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "CLARITY") {
        if (_.find(ClarityList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(ClarityList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(ClarityList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "CUT") {
        if (_.find(CutList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(CutList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(CutList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "LAB") {
        if (_.find(LabList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(LabList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(LabList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "POLISH") {
        if (_.find(PolishList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(PolishList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(PolishList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "SYMM") {
        if (_.find(SymList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(SymList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(SymList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "FLS") {
        if (_.find(FlouList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(FlouList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(FlouList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "BGM") {
        if (_.find(BgmList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(BgmList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(BgmList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "TABLE_INCL") {
        if (_.find(TblInclList, function (num) { return num.ACTIVE == true; })) {
            _.findWhere(TblInclList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(TblInclList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "TABLE_NATTS") {
        if (_.find(TblNattsList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(TblNattsList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(TblNattsList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "CROWN_INCL") {
        if (_.find(CrwnInclList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(CrwnInclList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(CrwnInclList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "CROWN_NATTS") {
        if (_.find(CrwnNattsList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(CrwnNattsList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(CrwnNattsList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "Location") {
        if (_.find(LocationList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(LocationList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(LocationList, { Value: value }).ACTIVE = true;
        }
    }
    else if (flag == "carat") {
        if (_.find(CaratList, function (num) { return num.ACTIVE == true && num.Value == value; })) {
            _.findWhere(CaratList, { Value: value }).ACTIVE = false;
        } else {
            _.findWhere(CaratList, { Value: value }).ACTIVE = true;
        }
    }

}

SetCutMaster = function (item) {
    _.each(CutList, function (itm) {
        $('#searchcut li[onclick="SetActive(\'CUT\',\'' + itm.Value + '\')"]').removeClass('active');
        itm.ACTIVE = false;
    });
    _.each(PolishList, function (itm) {
        $('#searchpolish li[onclick="SetActive(\'POLISH\',\'' + itm.Value + '\')"]').removeClass('active');
        itm.ACTIVE = false;
    });
    _.each(SymList, function (itm) {
        $('#searchsymm li[onclick="SetActive(\'SYMM\',\'' + itm.Value + '\')"]').removeClass('active');
        itm.ACTIVE = false;
    });
    if (item == '3EX' && !$('#li3ex').hasClass('active')) {
        $('#li3vg').removeClass('active');

        _.each(_.filter(CutList, function (e) { return e.Value == "EX" }), function (itm) {
            $('#searchcut li[onclick="SetActive(\'CUT\',\'EX\')"]').addClass('active');
            itm.ACTIVE = true;
        });
        _.each(_.filter(PolishList, function (e) { return e.Value == "EX" }), function (itm) {
            $('#searchpolish li[onclick="SetActive(\'POLISH\',\'EX\')"]').addClass('active');
            itm.ACTIVE = true;
        });
        _.each(_.filter(SymList, function (e) { return e.Value == "EX" }), function (itm) {
            $('#searchsymm li[onclick="SetActive(\'SYMM\',\'EX\')"]').addClass('active');
            itm.ACTIVE = true;
        });
    }
    else if (item == '3VG' && !$('#li3vg').hasClass('active')) {

        $('#li3ex').removeClass('active');
        _.each(_.filter(CutList, function (e) { return e.Value == "EX" || e.Value == "VG" }), function (itm) {
            $('#searchcut li[onclick="SetActive(\'CUT\',\'' + itm.Value + '\')"]').addClass('active');
            itm.ACTIVE = true;
        });
        _.each(_.filter(PolishList, function (e) { return e.Value == "EX" || e.Value == "VG" }), function (itm) {
            $('#searchpolish li[onclick="SetActive(\'POLISH\',\'' + itm.Value + '\')"]').addClass('active');
            itm.ACTIVE = true;
        });
        _.each(_.filter(SymList, function (e) { return e.Value == "EX" || e.Value == "VG" }), function (itm) {
            $('#searchsymm li[onclick="SetActive(\'SYMM\',\'' + itm.Value + '\')"]').addClass('active');
            itm.ACTIVE = true;
        });
    }
}
/*--------------------------------------------------------SET RESET ACTIVE END-----------------------------------------------*/

/*--------------------------------------------------------DATA SOURCE START----------------------------------------------*/
function onPageSizeChanged(id) {
    var value = $(id).val();
    gridOptions.api.paginationSetPageSize(Number(value));
}
/*--------------------------------------------------------PAGE SIZE END--------------------------------------------------*/

/*--------------------------------------------------------DATA SOURCE START------------------------------------------------*/
function formatNumber(number) {
    return (parseFloat(number).toFixed(2)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
}
function formatIntNumber(number) {
    return (parseInt(number)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
}
const datasource1 = {
    getRows(params) {
        console.log(JSON.stringify(params.request, null, 1));
        obj1.PageNo = gridOptions.api.paginationGetCurrentPage() + 1;
        if (params.request.sortModel.length > 0) {
            obj1.OrderBy = '' + params.request.sortModel[0].colId + ' ' + params.request.sortModel[0].sort + ''
        }

        DiscAmtArray = [];
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        $.ajax({
            url: "/Offer/GetSearchStock",
            async: false,
            type: "POST",
            data: { obj: obj1 },
            success: function (data, textStatus, jqXHR) {
                if (data.Message.indexOf('Something Went wrong') > -1) {
                    MoveToErrorPage(0);
                }
                if (data.Data.length > 0) {
                    DiscAmtArray = [];
                    for (var i = 0; i < data.Data[0].DataList.length; i++) {
                        if (data.Data[0].DataList[i].offerDisc != "0") {
                            var offerDisc = (data.Data[0].DataList[i].offerDisc != "" ? parseFloat(data.Data[0].DataList[i].offerDisc).toFixed(2) : "");
                            var offerAmt = (data.Data[0].DataList[i].offerAmt != "" ? parseFloat(data.Data[0].DataList[i].offerAmt).toFixed(2) : "");
                            var Offer_Final_Discount = (data.Data[0].DataList[i].Offer_Final_Discount != "" ? parseFloat(data.Data[0].DataList[i].Offer_Final_Discount).toFixed(2) : "");
                            var Offer_Final_Amount = (data.Data[0].DataList[i].Offer_Final_Amount != "" ? parseFloat(data.Data[0].DataList[i].Offer_Final_Amount).toFixed(2) : "");
                            var sales_disc_per = (data.Data[0].DataList[i].sales_disc_per != "" ? parseFloat(data.Data[0].DataList[i].sales_disc_per).toFixed(2) : "");

                            DiscAmtArray.push([data.Data[0].DataList[i].stone_ref_no, offerDisc, offerAmt, data.Data[0].DataList[i].validDays, data.Data[0].DataList[i].Offer_Remark, Offer_Final_Discount, Offer_Final_Amount, sales_disc_per]);
                        }
                    }


                    summary1 = data.Data[0].DataSummary;
                    params.successCallback(data.Data[0].DataList, summary1.TOT_PCS);
                    //$('#tab1TCount').show();
                    $('#tabcts').html(formatNumber(summary1.TOT_CTS));
                    $('#tabdisc').html(formatNumber(summary1.AVG_SALES_DISC_PER));
                    $('#tabppcts').html(formatNumber(summary1.AVG_PRICE_PER_CTS));
                    $('#tabtotAmt').html(formatNumber(summary1.TOT_NET_AMOUNT));
                    $('#tabpcs').html(formatIntNumber(summary1.TOT_PCS));
                    $('#tab1_WebDisc_t').hide();
                    $('#tab1_FinalValue_t').hide();
                    $('#tab1_FinalDisc_t').hide();
                } else {
                    params.successCallback([], 0);
                    gridOptions.api.showNoRowsOverlay();

                    //$('#tab1TCount').hide();
                    $('#tabcts').html('0');
                    $('#tabdisc').html('0');
                    $('#tabppcts').html('0');
                    $('#tabtotAmt').html('0');
                    $('#tabpcs').html('0');
                    $('#tab1_WebDisc_t').hide();
                    $('#tab1_FinalValue_t').hide();
                    $('#tab1_FinalDisc_t').hide();
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
                setInterval(function () {
                    $(".ag-header-cell-text").addClass("grid_prewrap");
                }, 30);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                params.successCallback([], 0);
                gridOptions.api.showNoRowsOverlay();
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    }
};
/*--------------------------------------------------------DATA SOURCE END--------------------------------------------------*/


/*--------------------------------------------------------ADD TO CART START--------------------------------------------------*/

function AddToCart() {
    var stoneList = gridOptions.api.getSelectedRows();

    var availabelstonelist = '';
    var offerstonelist = '';
    if ($('#hdnisadminflg').val() == '1' || $('#hdnisempflg').val() == '1') {
        availabelstonelist = _.pluck(stoneList, 'stone_ref_no').join(",");
    } else {
        availabelstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status == 'AVAILABLE' }), 'stone_ref_no').join(",");
        offerstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status != 'AVAILABLE' }), 'stone_ref_no').join(",");
    }
    if (availabelstonelist != '') {
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        $.ajax({
            url: "/SearchStock/AddToCart",
            type: "POST",
            data: { stoneNo: availabelstonelist, transType: 'A' },
            success: function (data, textStatus, jqXHR) {
                if (data.Message.indexOf('Something Went wrong') > -1) {
                    MoveToErrorPage(0);
                }
                if (data.Status == "0") {
                    toastr.error(data.Message);
                } else {
                    if (offerstonelist != '') {
                        $('#cartresMsg').html(' <div>' + offerstonelist + ' </div>' +
                            '<div>' + $("#hdn_This_Stone_is_not_Available_Stone_You_can_add_only_available_Stone_into_Cart").val() + '...!</div>' +
                            ' <div>' + $("#hdn_Other_Stones_are_added_into_cart_successfully").val() + '...!</div>')
                    } else {
                        $('#cartresMsg').html(data.Message)
                    }
                    $('#cartModal').modal('show');
                    GetDashboardCount();
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    }
    else if (offerstonelist != '') {
        toastr.warning($("#hdn_Select_Avail_Stone_for_add_to_cart").val() + '!');
    }
    else {
        toastr.warning($("#hdn_No_Stone_Selected_for_add_to_cart").val() + '!');
    }
}

/*--------------------------------------------------------ADD TO CART END----------------------------------------------------*/

/*--------------------------------------------------------ADD TO WISHLIST START----------------------------------------------*/

function AddToWishlist() {
    var stoneno = _.pluck(gridOptions.api.getSelectedRows(), 'stone_ref_no').join(",");
    var count = gridOptions.api.getSelectedRows().length;

    if (count > 0) {
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        $.ajax({
            url: "/SearchStock/AddToWishlist",
            type: "POST",
            data: { stoneNo: stoneno, transType: 'A' },
            success: function (data, textStatus, jqXHR) {
                data.Message = data.Message.replace('Stone(s) added in wishList successfully', $("#hdn_Stones_added_in_wishlist_successfully").val());
                data.Message = data.Message.replace('Stone(s) removed from wishList successfully', $("#hdn_Stones_removed_from_wishList_successfully").val());
                data.Message = data.Message.replace('Add in to wishList failed', $("#hdn_Add_in_to_wishList_failed").val());

                if (data.Status == "0") {
                    if (data.Message.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    toastr.error(data.Message);
                } else {
                    $('#wishlistresMsg').html(data.Message)
                    $('#WishlistModal').modal('show');
                    GetDashboardCount();
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    } else {
        toastr.warning($("#hdn_No_stone_selected_for_add_to_wishlist").val() + '!');
    }
}

/*--------------------------------------------------------ADD TO WISHLIST END------------------------------------------------*/

/*--------------------------------------------------------PLACE ORDER START--------------------------------------------------*/

function SaveOrder() {
    var isValid = $('#frmSaveOrder').valid();
    if (!isValid) {
        return false;
    }
    var stoneList = gridOptions.api.getSelectedRows();

    //var availabelstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status == 'AVAILABLE' }), 'stone_ref_no').join(",");
    //var offerstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status != 'AVAILABLE' }), 'stone_ref_no').join(",");

    var availabelstonelist = '';
    var offerstonelist = '';
    if ($('#hdnisadminflg').val() == '1' || $('#hdnisempflg').val() == '1') {
        availabelstonelist = _.pluck(stoneList, 'stone_ref_no').join(",");
    } else {
        availabelstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status == 'AVAILABLE' || e.status == 'NEW' }), 'stone_ref_no').join(",");
        offerstonelist = _.pluck(_.filter(stoneList, function (e) { return e.status != 'AVAILABLE' && e.status != 'NEW' }), 'stone_ref_no').join(",");
    }

    if (availabelstonelist != '') {
        $('#hdnAvailableStone').val(availabelstonelist);
        if (offerstonelist != '') {
            $('#PlaceOrderMsg').html('<div>' + offerstonelist + ' ' + $("#hdn_PlaceOrderMsg_1").val() + ' ...!</div>' +
                ' <div>' + $("#hdn_PlaceOrderMsg_2").val() + ' ? </div>');
            $('#ConfirmOrderModal').modal('hide');
            $('#ConfirmOrderWarningModal').modal('hide');
        } else {
            PlaceOrder();
        }
    }
    else if (offerstonelist != '') {
        //toastr.warning($("#hdn_Select_Avail_Stone_for_place_order").val() + '!');
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();

        $.ajax({
            url: "/Order/GetAssistPersonDetail",
            type: "POST",
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "0") {
                    if (data.Message.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    toastr.error(data.Message);
                } else {
                    $('#PlaceOrderMsg').html('<div>' + $("#hdn_Select_Avail_Stone_for_place_order").val() + '!<br>' + $("#hdn_PleaseContact").val() + data.Message + '</div>');
                    $('#ConfirmOrderModal').modal('hide');
                    $('#ConfirmOrderWarningModal').modal('show');
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    }
    else {
        toastr.warning($("#hdn_No_Stone_Selected_for_place_order").val() + '!');
    }
}

function PlaceOrder() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    $.ajax({
        url: "/SearchStock/PlaceOrder",
        type: "POST",
        data: { stoneNo: $('#hdnAvailableStone').val(), Comments: $('#Comments').val() },
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "0") {
                if (data.Message.indexOf('Something Went wrong') > -1) {
                    MoveToErrorPage(0);
                }
                toastr.error(data.Message);
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            } else {
                $('#ConfirmOrderModal').modal('hide');
                $('#ConfirmOrderWarningModal').modal('hide');

                data.Message = data.Message.replace('Your Transaction Done Successfully', $("#hdn_Your_Transaction_Done_Successfully").val());
                data.Message = data.Message.replace('This Stone(s) are subject to avaibility', $("#hdn_This_Stones_are_subject_to_availbility").val());
                data.Message = data.Message.replace('Please contact your sales person', $("#hdn_Please_contact_your_sales_person").val());

                $('#lblcheckingavailability').html(data.Message);

                $('#order-confirm-modal').modal('show');
                GetSearch();
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
/*--------------------------------------------------------PLACE ORDER END----------------------------------------------------*/
/*--------------------------------------------------------SEND EMAIL START----------------------------------------------------*/
function SendMail() {
    var isValid = $('#frmSendMail').valid();
    if (!isValid) {
        return false;
    }
    if ($('#customRadiomail').prop('checked')) {
        var sobj = obj1;
        sobj.FormName = 'Offer Stone';
        sobj.ActivityType = 'Excel Email';

        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        $.ajax({
            url: "/SearchStock/EmailAllStone",
            type: "POST",
            data: { SearchCriteria: sobj, ToAddress: $('#txtemail').val(), Comments: $('#txtNotes').val() },
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "0") {
                    if (data.Message.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    toastr.error(data.Message);
                } else {
                    data.Message = data.Message.replace('Mail sent successfully', $("#hdn_Mail_sent_successfully").val());
                    toastr.success(data.Message);
                }
                $('#EmailModal').modal('hide');
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    } else {
        var stoneno = _.pluck(gridOptions.api.getSelectedRows(), 'stone_ref_no').join(",");
        var count = gridOptions.api.getSelectedRows().length;
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
        if (count > 0) {
            $.ajax({
                url: "/SearchStock/EmailSelectedStone",
                type: "POST",
                data: {
                    StoneID: stoneno, ToAddress: $('#txtemail').val(), Comments: $('#txtNotes').val(),
                    FormName: 'Offer Stone', ActivityType: 'Excel Email'
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Status == "0") {
                        if (data.Message.indexOf('Something Went wrong') > -1) {
                            MoveToErrorPage(0);
                        }
                        toastr.error(data.Message);
                    } else {
                        data.Message = data.Message.replace('Mail sent successfully', $("#hdn_Mail_sent_successfully").val());
                        toastr.success(data.Message);
                    }
                    CloseSendMailPopup();
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                }
            });
        } else {
            toastr.warning('No stone selected to send email!');
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    }
}
/*--------------------------------------------------------SEND EMAIL END----------------------------------------------------*/
/*--------------------------------------------------------DOWNLOAD ALL START----------------------------------------------------*/
function OpenDownloadPopup(downloadType) {
    $('#hdnDownloadType').val(downloadType);
    $('#ExcelModalAll #customRadio4').prop("checked", true);
    DownloadAll();
    //$('#ExcelModalAll').modal('show');
}
function DownloadAll() {
    $('#ExcelModalAll').modal('hide');
    if ($('#hdnDownloadType').val() == "Excel") {
        DownloadExcel();
    }
    else if ($('#hdnDownloadType').val() == "Pdf") {
        DownloadMedia();
    }
    else if ($('#hdnDownloadType').val() == "Image") {
        DownloadMedia();
    }
    else if ($('#hdnDownloadType').val() == "Video") {
        DownloadMedia();
    }
    else if ($('#hdnDownloadType').val() == "Certificate") {
        DownloadMedia();
    }
    $('#customRadio3').prop('checked', true);
}
function DownloadExcel() {
    if (AllD == false) {
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();
    }
    if ($('#customRadio3').prop('checked')) {
        count = rowData.length;
        stoneno = '';
    }
    else {
        count = gridOptions.api.getSelectedRows().length;
        stoneno = _.pluck(gridOptions.api.getSelectedRows(), 'stone_ref_no').join(",");
    }
    if (count > 0) {
        obj1.StoneID = stoneno;
        obj1.FormName = 'Offer Stone';
        obj1.ActivityType = 'Excel Export';
        $.ajax({
            url: "/Offer/OfferExcelDownloadBySearchObject",
            type: "POST",
            data: obj1,
            success: function (data, textStatus, jqXHR) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
                AllD = false;
                if (data.indexOf('Something Went wrong') > -1) {
                    MoveToErrorPage(0);
                }
                else if (data.indexOf('No data found') > -1) {
                    toastr.error(data);
                }
                else {
                    location.href = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    } else {
        toastr.warning($("#hdn_No_stone_selected_for_download_as_a_excel").val() + '!');
        if (AllD == false) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    }
}

function DownloadMedia() {
    if ($('#customRadio3').prop('checked')) {
        var sobj = obj1;

        if (AllD == false) {
            $('.loading-overlay-image-container').show();
            $('.loading-overlay').show();
        }
        $.ajax({
            url: "/Common/StockMediaDownloadBySearchObject",
            type: "POST",
            data: { obj: sobj, MediaType: $('#hdnDownloadType').val(), FormName: 'Offer Stone', ActivityType: $('#hdnDownloadType').val() + ' Download' },
            success: function (data, textStatus, jqXHR) {
                if (AllD == false) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                }
                if (data.search('.zip') == -1 && data.search('.pdf') == -1) {
                    if (data.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    data = data.replace('Error to download video, video is not MP4', $("#hdn_Error_to_download_video_video_is_not_MP4").val());
                    data = data.replace('Image is not available in this stone', $("#hdn_Image_is_not_available_in_this_stone").val());
                    toastr.error(data);
                } else {
                    location.href = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    } else {
        var stoneno = _.pluck(gridOptions.api.getSelectedRows(), 'stone_ref_no').join(",");
        var count = gridOptions.api.getSelectedRows().length;
        if (AllD == false) {
            $('.loading-overlay-image-container').show();
            $('.loading-overlay').show();
        }
        if (count > 0) {
            $.ajax({
                url: "/Common/StockMediaDownloadByStoneId",
                type: "POST",
                data: { StoneID: stoneno, MediaType: $('#hdnDownloadType').val(), FormName: 'Offer Stone', ActivityType: $('#hdnDownloadType').val() + ' Download' },
                success: function (data, textStatus, jqXHR) {
                    if (data.Message.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    if (data.search('.zip') == -1 && data.search('.pdf') == -1) {
                        if (data.indexOf('Something Went wrong') > -1) {
                            MoveToErrorPage(0);
                        }
                        data = data.replace('Error to download video, video is not MP4', $("#hdn_Error_to_download_video_video_is_not_MP4").val());
                        data = data.replace('Image is not available in this stone', $("#hdn_Image_is_not_available_in_this_stone").val());
                        toastr.error(data);
                    } else {
                        location.href = data;
                    }
                    if (AllD == false) {
                        $('.loading-overlay-image-container').hide();
                        $('.loading-overlay').hide();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                }
            });
        } else {
            toastr.warning($("#hdn_No_stone_selected_for_download_as_a").val() + ' ' + $('#hdnDownloadType').val() + ' !');
            if (AllD == false) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        }
    }
}
/*--------------------------------------------------------DOWNLOAD ALL END----------------------------------------------------*/
function SaveSearch() {

    if ($('#txtSearchName').val() == "") {
        return toastr.warning('Please Fill SearchName...!')
    }
    if ($('#hdnSavedSearchId').val() == "0" || $('#hdnSavedSearchId').val() == "") {
        var SavedSearchID = parseFloat(0);
    }
    else {
        var SavedSearchID = $('#hdnSavedSearchId').val();
    }
    if ($('#hdnSavedSearchId').val() == "0" || $('#hdnSavedSearchId').val() == "") {
        var SavedSearchID = parseFloat(0);

        if (SaveSearchList.length == 0) {
            toastr.warning($("#hdn_Please_select_atlist_one_criteria").val());
            $("#save-src-modl").modal("hide");
            return;
        }
    }
    var Searchname = $('#txtSearchName').val();

    $('#ConfirmOrderModal').modal('show');
    $('#ConfirmOrderWarningModal').modal('show');
    $http({
        method: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: '/SearchStock/SaveSearch',
        data: JSON.stringify({ SearchId: SavedSearchID, Searchname: Searchname, model: SaveSearchList }),
    }).then(function (data) {
        $('#ConfirmOrderModal').modal('hide');
        $('#ConfirmOrderWarningModal').modal('hide');
        if (data.data == "Search Save data successfully.") {
            $("#save-src-modl").modal("hide");
            toastr.success($("#hdn_Record_Save_Successfully").val());
            SearchStone();
        }
        else {
            toastr.warning(data.data, 5000);
        }
        $('#txtSearchName').val("");
    });

}
function isNumberKey_ISD(evt) {

    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        if (charCode == 45) {
            return true;
        }
        return false;
    }

    return true;
}
function setToFixed(obj) {
    $(obj).val(parseFloat($(obj).val()).toFixed(2));
}
function GetDashboardCount() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    $.ajax({
        url: "/Dashboard/GetDashboardCount",
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
            $.each(data.Data, function (key, obj) {
                if (obj.Type == "MyCart") {
                    $('.cntCart').html(obj.sCnt);
                }
                else if (obj.Type == "WishList") {
                    $('.cntwishlist').html(obj.sCnt);
                }
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}
function CallingMarquee() {
    $.ajax({
        url: "/User/NewsMaster",
        type: "POST",
        data: { Flag: "SelectId", FromDate: "", ToDate: "", Description: "", FontColor: "", iID: 0 },
        success: function (data, textStatus, jqXHR) {
            if (data.Data.length != 0) {
                var _d = "";
                _d += '<marquee width="100%" direction="left" height="100%">';
                for (var i = 0; i < data.Data.length; i++) {
                    _d += "<span style=color:" + data.Data[i].FontColor + ">" + data.Data[i].Description.replace(/'/g, ' &_ ') + "</span>";
                    _d += data.Data.length != i + 1 ? "<span style=color:" + data.Data[i].FontColor + ">,&nbsp;</span>" : "";
                }
                _d += '</marquee>';

                $("#divMarquee").html(_d.replace(/ &_ /g, "'"));
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}







function TEMP_SAVE(WHEN, StoneNo, Offer_Discount, Offer_Amount, Valid_Days, Remark, Offer_Final_Discount, Offer_Final_Amount, Discount) {
    Offer_Discount = (Offer_Discount != "" ? parseFloat(Offer_Discount).toFixed(2) : "");
    Offer_Amount = (Offer_Amount != "" ? parseFloat(Offer_Amount).toFixed(2) : "");
    Offer_Final_Discount = (Offer_Final_Discount != "" ? parseFloat(Offer_Final_Discount).toFixed(2) : "");
    Offer_Final_Amount = (Offer_Final_Amount != "" ? parseFloat(Offer_Final_Amount).toFixed(2) : "");
    Discount = (Discount != "" ? parseFloat(Discount).toFixed(2) : "");

    var exists = false;
    if (DiscAmtArray.length > 0) {
        for (var i = 0; i < DiscAmtArray.length; i++) {
            if (DiscAmtArray[i][0] == StoneNo) {
                if (WHEN == "DISC_AMT") {
                    DiscAmtArray[i][1] = Offer_Discount;
                    DiscAmtArray[i][2] = Offer_Amount;
                    DiscAmtArray[i][3] = Valid_Days;
                    DiscAmtArray[i][5] = Offer_Final_Discount;
                    DiscAmtArray[i][6] = Offer_Final_Amount;
                    DiscAmtArray[i][7] = Discount;
                }
                else if (WHEN == "DAY") {
                    DiscAmtArray[i][3] = Valid_Days;
                }
                else if (WHEN == "REMARK") {
                    DiscAmtArray[i][4] = Remark;
                }

                exists = true;
            }
        }
    }
    if (exists == false) {
        DiscAmtArray.push([StoneNo, Offer_Discount, Offer_Amount, Valid_Days, Remark, Offer_Final_Discount, Offer_Final_Amount, Discount]);
    }
}

var CompanyList = [];
function GetCompanyList() {
    $.ajax({
        url: "/User/GetCompanyForHoldStonePlaceOrder",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            if (data.Data != null) {
                CompanyList = data.Data;
                for (var i = 0; i < CompanyList.length; i++) {
                    CompanyList[i].iUserid = CompanyList[i].iUserid + "__" + CompanyList[i].FortunePartyCode;
                }
                if ($("#hdnUserType").val() == "1") {
                    $('#___txtCompanyName').ejAutocomplete({
                        dataSource: CompanyList,
                        filterType: 'contains',
                        fields: { key: "iUserid" },
                        highlightSearch: true,
                        watermarkText: "Search with Company Name, Assist By, Party Code, Customer Name",
                        width: "100%",
                        showPopupButton: true,
                        multiColumnSettings: {
                            enable: true,
                            showHeader: true,
                            stringFormat: "{0}",
                            searchColumnIndices: [0, 1, 2, 3],
                            columns: [
                                { "field": "CompName", "headerText": "COMPANY NAME" },
                                { "field": "AssistBy", "headerText": "ASSIST BY" },
                                { "field": "FortunePartyCode", "headerText": "PARTY CODE" },
                                { "field": "CustName", "headerText": "CUSTOMER NAME" }
                            ]
                        }
                    });
                }
                else if ($("#hdnUserType").val() == "2") {
                    $('#___txtCompanyName').ejAutocomplete({
                        dataSource: CompanyList,
                        filterType: 'contains',
                        fields: { key: "iUserid" },
                        highlightSearch: true,
                        watermarkText: "Search with Company Name, Party Code, Customer Name",
                        width: "100%",
                        showPopupButton: true,
                        multiColumnSettings: {
                            enable: true,
                            showHeader: true,
                            stringFormat: "{0}",
                            searchColumnIndices: [0, 1, 2],
                            columns: [
                                { "field": "CompName", "headerText": "COMPANY NAME" },
                                { "field": "FortunePartyCode", "headerText": "PARTY CODE" },
                                { "field": "CustName", "headerText": "CUSTOMER NAME" }
                            ]
                        }
                    });
                }
            }
        }
    });
}
function CmpnynmRst() {
    $("#___txtCompanyName_hidden").val("");
}
function CmpnynmSelectRequired() {
    setTimeout(function () {
        if ($("#___txtCompanyName_hidden").val().split("__").length != 2) {
            $("#___txtCompanyName").val("");
            $("#___txtCompanyName_hidden").val("");
        }
    }, 250);
}
function multiDimensionalUnique(arr) {
    var uniques = [];
    var itemsFound = {};
    for (var i = 0, l = arr.length; i < l; i++) {
        var stringified = JSON.stringify(arr[i]);
        if (itemsFound[stringified]) { continue; }
        uniques.push(arr[i]);
        itemsFound[stringified] = true;
    }
    return uniques;
}
function CertiTypeLink_Indicator(params) {
    var value = "";
    //value = '<span style="color: blue;">' + (params.data.Certi_Type != null ? params.data.Certi_Type : '') + '</span>';
    value = (params.data.Certi_Type != null ? params.data.Certi_Type : '');
    if (params.data.CertiTypeLink != null && params.data.Certi_Type != null) {
        value = '<a href="' + params.data.CertiTypeLink + '" target="_blank" style="text-decoration: underline; color :blue;">' + params.data.Certi_Type + '</a>';
    }
    return value;
}