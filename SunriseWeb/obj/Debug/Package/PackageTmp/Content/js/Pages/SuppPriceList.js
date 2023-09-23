﻿var gridOptions = {};
var GridpgSize = 50;
var rowData = [];
var ISFirstTime = 0;
var Excel_OrderBy, Excel_PageNo, Excel_GridpgSize;
var hdnDelId = '';

var showEntryHtml = '<div class="show_entry show_entry1"><label>'
    + 'Show <select onchange = "onPageSizeChanged()" id = "ddlPagesize" class="" >'
    + '<option value="50">50</option>'
    + '<option value="100">100</option>'
    + '<option value="200">200</option>'
    + '</select> entries'
    + '</label>'
    + '</div>';

function onPageSizeChanged() {
    var value = $("#ddlPagesize").val();
    GridpgSize = Number(value);
    GetDataList();
}
var SetCurrentDate = function () {
    var m_names = new Array("Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec");
    var d = new Date();
    var curr_date = d.getDate();
    var curr_month = d.getMonth();
    var curr_year = d.getFullYear();
    var FinalDate = (curr_date + "-" + m_names[curr_month]
        + "-" + curr_year);

    return FinalDate;
}
var lastWeekDate = new Date();
var m_names = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");
var today = new Date();
var lastWeekDate = new Date(today.setDate(today.getDate() - 7));
var date = new Date(lastWeekDate),
    mnth = ("0" + (date.getMonth() + 1)).slice(-2),
    day = ("0" + date.getDate()).slice(-2);
var F_date = [day, m_names[mnth - 1], date.getFullYear()].join("-");

var Back = function () {
    window.location.href = '/Settings/SuppPriceListDet';
}

function contentHeight() {
    var winH = $(window).height(),
        header = $(".order-history-data").height(),
        contentHei = winH - (header + 170);
    $("#myGrid").css("height", contentHei);
}
function FromTo_Date() {
    $('#txtFromDate').val(F_date);
    $('#txtToDate').val(SetCurrentDate());
    $('#txtFromDate').daterangepicker({
        singleDatePicker: true,
        startDate: F_date,
        showDropdowns: true,
        locale: {
            separator: "-",
            format: 'DD-MMM-YYYY'
        },
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    }).on('change', function (e) {
        greaterThanDate(e);
    });
    $('#txtToDate').daterangepicker({
        singleDatePicker: true,
        startDate: moment(),
        showDropdowns: true,
        locale: {
            separator: "-",
            format: 'DD-MMM-YYYY'
        },
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    }).on('change', function (e) {
        greaterThanDate(e);
    });
}
var Clear = function () {
    $("#txtSearch").val("");
    FromTo_Date();

    btnSearch();
}

var columnDefs = [
    { headerName: "Sr.", field: "iSr", width: 40, sortable: false },
    { headerName: "Created Date", field: "CreateDate", width: 135, sortable: true },
    { headerName: "Id", field: "Id", hide: true },
    { headerName: "Supplier Name", field: "SupplierName", width: 130, sortable: true },
    { headerName: "Last Updated", field: "UpdateDate", width: 130, sortable: true },
    { headerName: "Action", field: "bIsAction", tooltip: function (params) { return (params.value); }, width: 70, cellRenderer: UserDetailPage, },
];

function UserDetailPage(params) {
    var element = "";
    element = '<a onclick="EditData(\'' + params.data.Id + '\')" ><i class="fa fa-pencil-square-o" aria-hidden="true" style="font-size: 17px;cursor:pointer;"></i></a>';
    element += '&nbsp;&nbsp;<a onclick="DeleteData(\'' + params.data.Id + '\')"><i class="fa fa-trash-o" aria-hidden="true" style="font-size: 17px;cursor:pointer;"></i></a>';
    return element;
}
function EditData(row) {
    location.href = '/Settings/SuppPriceListDet?Id=' + row;
}

var gridDiv = document.querySelector('#myGrid');
function GetDataList() {
    $("#loading").css("display", "block");
    if (gridOptions.api != undefined) {
        gridOptions.api.destroy();
    }

    gridOptions = {
        //rowHeight: 45,
        wrapText: true,
        masterDetail: true,
        detailCellRenderer: 'myDetailCellRenderer',
        //detailRowHeight: 70,
        groupDefaultExpanded: 1,
        //components: {
        //    dateIndicator: dateIndicator
        //},
        defaultColDef: {
            enableSorting: true,
            sortable: false,
            width: 150,
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
        cacheBlockSize: GridpgSize, // you can have your custom page size
        paginationPageSize: GridpgSize, //pagesize
        paginationNumberFormatter: function (params) {
            return '[' + params.value.toLocaleString() + ']';
        }
    };

    new agGrid.Grid(gridDiv, gridOptions);
    gridOptions.api.setServerSideDatasource(datasource1);

    showEntryVar = setInterval(function () {
        if ($('#myGrid .ag-paging-panel').length > 0) {
            $(showEntryHtml).appendTo('#myGrid .ag-paging-panel');
            $('#ddlPagesize').val(GridpgSize);
        }
        clearInterval(showEntryVar);
    }, 500);

    setTimeout(function () {
        var allColumnIds = [];
        gridOptions.columnApi.getAllColumns().forEach(function (column) {
            allColumnIds.push(column.colId);
        });
    }, 1000);
}

const datasource1 = {
    getRows(params) {
        var OrderBy = '', PageNo = gridOptions.api.paginationGetCurrentPage() + 1;

        if (params.request.sortModel.length > 0) {
            OrderBy = '' + params.request.sortModel[0].colId + ' ' + params.request.sortModel[0].sort + ''
        }
        var formData = new FormData();

        if (ISFirstTime == 1) {
            formData.append('FromDate', '01-Jan-2020');
        }
        else if (ISFirstTime == 0) {
            formData.append('FromDate', $('#txtFromDate').val());
        }
        formData.append('ToDate', $('#txtToDate').val());
        formData.append('Search', $('#txtSearch').val());
        formData.append('iPgNo', PageNo);
        formData.append('iPgSize', GridpgSize);
        formData.append('OrderBy', OrderBy);

        Excel_OrderBy = OrderBy;
        Excel_PageNo = PageNo;
        Excel_GridpgSize = GridpgSize;

        $.ajax({
            url: "/Settings/Get_Supplier_PriceList",
            async: false,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, textStatus, jqXHR) {
                if (data.Status == "1" && data.Message == "SUCCESS") {
                    if (data.Data != null && data.Data.length > 0) {
                        params.successCallback(data.Data, data.Data[0].iTotalRec);
                    }
                    else {
                        params.successCallback([], 0);
                        toastr.warning(data.Message);
                    }
                }
                else {
                    params.successCallback([], 0);
                    toastr.error(data.Message);
                }
                contentHeight();
                $("#loading").css("display", "none");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                params.successCallback([], 0);
                $("#loading").css("display", "none");
            }
        });
    }
};
function btnSearch() {
    ISFirstTime = 0;
    GetDataList();
}
var ClearRemoveModel = function () {
    $("#DltMsgBox").modal("hide");
}
var DeleteData = function (Id) {
    hdnDelId = Id;
    $("#DltMsgBox").modal("show");
}
function Delete() {
    debugger
    $.ajax({
        url: "/Settings/Supplier_PriceList_Delete",
        async: false,
        type: "POST",
        data: { SupplierPriceList_Id: hdnDelId },
        success: function (data, textStatus, jqXHR) {
            debugger
            if (data.Status == "0") {
                toastr.error(data.Message);
            }
            else if (data.Status == "1") {
                ClearRemoveModel();
                toastr.success(data.Message);
                setTimeout(function () {
                    btnSearch();
                }, 1000);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
$(document).ready(function (e) {
    FromTo_Date();

    ISFirstTime = 1;
    GetDataList();
    contentHeight();

    $.ajax({
        url: "/Settings/TabSessionSet",
        type: "POST",
        data: { Type: "StockPrefix" },
        success: function (data, textStatus, jqXHR) {
        },
    });
});
$(window).resize(function () {
    contentHeight();
});