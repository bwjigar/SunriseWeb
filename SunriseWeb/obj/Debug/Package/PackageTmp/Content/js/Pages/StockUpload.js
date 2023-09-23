$(document).ready(function () {
    Masters();
});
function Masters() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();
    
    $.ajax({
        url: "/Settings/SupplierGet",
        async: false,
        type: "POST",
        processData: false,
        contentType: false,
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "1" && data.Data != null) {
                $('#DdlSupplierName').html("<option value=''>Select</option>");
                $('#DdlSupplierName').append("<option value='0'>SUNRISE</option>");
                _(data.Data).each(function (obj, i) {
                    $('#DdlSupplierName').append("<option value=\"" + obj.Id + "\">" + obj.sPartyName + "</option>");
                });
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