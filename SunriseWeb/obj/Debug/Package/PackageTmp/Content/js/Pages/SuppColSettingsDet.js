﻿var Column_Mas_Select = [];
var Column_Mas_ddl = "";
var List2 = [];

$(document).ready(function () {
    SupplierNameBind();
    ColumnMasBind();
    $("#Save_btn").hide();
    $("#RefreshCurrentAPI_btn").hide();
    if ($("#hdnId").val() != "0") {
        $("#Save_btn").show();
        $("#RefreshCurrentAPI_btn").show();
        Get_SuppColSettDet($("#hdnId").val());
    }
    contentHeight();
});
function SupplierNameBind() {
    $.ajax({
        url: "/Settings/Get_SupplierMaster",
        async: false,
        type: "POST",
        data: { OrderBy: "SupplierName ASC" },
        processData: false,
        contentType: false,
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "1" && data.Data != null) {
                $('#DdlSupplierName').append("<option value=''>Select</option>");
                _(data.Data).each(function (obj, i) {
                    $('#DdlSupplierName').append("<option value=\"" + obj.Id + "\">" + obj.SupplierName + "</option>");
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function ColumnMasBind() {
    $.ajax({
        url: "/Settings/Get_Column_Mas_Select",
        async: false,
        type: "POST",
        processData: false,
        contentType: false,
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "1" && data.Data != null) {
                Column_Mas_Select = data.Data;

                Column_Mas_ddl = "<option value=''>Select</option>";
                _(Column_Mas_Select).each(function (obj, i) {
                    Column_Mas_ddl += "<option value=\"" + obj.SEQ_NO + "\">" + obj.DISPLAY_NAME + "</option>";
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}

function SupplierNameChange() {
    if ($("#DdlSupplierName").val() != "") {
        $('.loading-overlay-image-container').show();
        $('.loading-overlay').show();

        setTimeout(function () {

            $.ajax({
                url: "/Settings/SupplierColSettings_ExistorNot",
                async: false,
                type: "POST",
                data: { Id: $("#DdlSupplierName").val() },
                success: function (data, textStatus, jqXHR) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();

                    $("#Save_btn").show();
                    $("#RefreshCurrentAPI_btn").show();

                    if (data.Status == "1") {
                        $("#Save_btn").html("<i class='fa fa-save' aria-hidden='true'></i>&nbsp;Update");
                        Get_SuppColSettDet(data.Message);
                    }
                    else {
                        $("#Save_btn").html("<i class='fa fa-save' aria-hidden='true'></i>&nbsp;Save");
                        SupplierColumnsGetFromAPI();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $('.loading-overlay-image-container').hide();
                    $('.loading-overlay').hide();
                }
            });

        }, 50);
    }
    else {
        $("#Save_btn").hide();
        $("#RefreshCurrentAPI_btn").hide();
        $("#TB_ColSetting").hide();
    }
}

function Get_SuppColSettDet(SupplierColSettingsMasId) {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    setTimeout(function () {
        
        $.ajax({
            url: "/Settings/Get_SuppColSettDet",
            async: false,
            type: "POST",
            data: { Id: SupplierColSettingsMasId },
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "1" && data.Message == "SUCCESS") {
                    $("#Save_btn").html("<i class='fa fa-save' aria-hidden='true'></i>&nbsp;Update");
                    $("#DdlSupplierName").val(data.Data[0].Supplier_Mas_Id);

                    $("#TB_ColSetting").show();
                    $('#myTableBody').html("");

                    var _Column_Mas_ddl = "";
                    _(data.Data).each(function (obj, i) {
                        _Column_Mas_ddl = "<option value=''>Select</option>";
                        _(Column_Mas_Select).each(function (__obj, i) {
                            _Column_Mas_ddl += "<option value=\"" + __obj.SEQ_NO + "\"" + (parseInt(obj.Column_Mas_Id) == parseInt(__obj.SEQ_NO) ? 'Selected' : '') + ">" + __obj.DISPLAY_NAME + "</option>";
                        });


                        $('#myTableBody').append('<tr><td>' + obj.Id + '</td><td class="SupplierColumn">' + obj.SupplierColumnName +
                            '</td><td><center><select onchange="ddlOnChange(\'' + obj.Id + '\');" id="ddl_' + obj.Id + '" class="col-md-6 form-control select2 CustomColumn">' + _Column_Mas_ddl +
                            '</select></center></td></tr>');
                    });
                }
                else {
                    $("#TB_ColSetting").hide();
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    }, 50);
}

function SaveData() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();

    setTimeout(function () {
        
        var DisOrder = 1;
        $("#mytable #myTableBody tr").each(function () {
            List2.push({
                Supplier_Mas_Id: $("#DdlSupplierName").val(),
                SupplierColumnName: $(this).find('.SupplierColumn').html(),
                Column_Mas_Id: $(this).find('.CustomColumn').val(),
                DisplayOrder: DisOrder
            });

            DisOrder = parseInt(DisOrder) + 1;
        });
        
        var obj = {};
        obj.SuppColSett = List2;
        $.ajax({
            url: "/Settings/Save_SuppColSettMas",
            async: false,
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ save_supcolsetmas: obj }),
            contentType: "application/json; charset=utf-8",
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "0") {
                    toastr.error(data.Message);
                }
                else if (data.Status == "1") {
                    toastr.success($("#DdlSupplierName").children(":selected").text() + " Supplier in Column Settings Applied Successfully");

                    var result = [];
                    result = data.Message.split("_414_");
                    
                    if (result[1] == "Insert") {
                        setTimeout(function () {
                            location.href = "/Settings/SuppColSettingsDet" + "?Id=" + result[0];
                        }, 2000);
                    }
                    else if (result[1] == "Update") {
                        setTimeout(function () {
                            location.href = "/Settings/SuppColSettingsDet" + "?Id=" + result[0];
                        }, 2000);
                    }
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
                toastr.error(textStatus);
            }
        });

    }, 20);
}

function SupplierColumnsGetFromAPI() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();

    setTimeout(function () {
        
        $.ajax({
            url: "/Settings/SupplierColumnsGetFromAPI",
            async: false,
            type: "POST",
            data: { Id: $("#DdlSupplierName").val() },
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "1" && data.Message == "SUCCESS") {
                    $("#TB_ColSetting").show();
                    $('#myTableBody').html("");
                    _(data.Data).each(function (obj, i) {
                        $('#myTableBody').append('<tr><td>' + obj.Id + '</td><td class="SupplierColumn">' + obj.SupplierColumn +
                            '</td><td><center><select onchange="ddlOnChange(\'' + obj.Id + '\');" id="ddl_' + obj.Id + '" class="col-md-6 form-control select2 CustomColumn">' + Column_Mas_ddl +
                            '</select></center></td></tr>');
                    });
                }
                else {
                    toastr.error(data.Message);
                    $("#Save_btn").hide();
                    $("#RefreshCurrentAPI_btn").hide();
                    $("#TB_ColSetting").hide();
                }
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.loading-overlay-image-container').hide();
                $('.loading-overlay').hide();
            }
        });
    }, 500);
}
function Clear() {
    SupplierColumnsGetFromAPI();
}
function Supp_List_View() {
    location.href = "/Settings/SuppColSettings";
}
function ddlOnChange(id) {
    if ($("#ddl_" + id).val() != "") {
        var DisOrder = 0;
        $("#mytable #myTableBody tr").each(function () {
            DisOrder = parseInt(DisOrder) + 1;
            if ($(this).find('.CustomColumn').val() != "") {
                if (DisOrder != parseInt(id) && $("#ddl_" + id).val() == $(this).find('.CustomColumn').val()) {
                    toastr.error($("#ddl_" + id).children(":selected").text() + " Custom Column Name alredy selected.");
                    $("#ddl_" + id).val("");
                }
            }
        });
    }
}
function contentHeight() {
    var winH = $(window).height(),
        navbarHei = $(".apicol-head").height(),
        contentHei = winH - navbarHei-205;
    $("#mytable").css("height", contentHei);
}
$(window).resize(function () {
    contentHeight();
});