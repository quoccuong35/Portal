function AddDays(e) {
    var FromDate = $("#FromDate").val();
    var ToDate = $("#ToDate").val();
    if (!FromDate || !ToDate) {
        return;
    }
    formData = new FormData();
    formData.append("FromDate", FromDate);
    formData.append("ToDate", ToDate);

    $.ajax({
        type: "POST",
        url: "/HRMS/FurloughModel/AddDays",
        data: formData,
        processData: false,
        contentType: false,
        success: function (jsonData) {

            if (jsonData.Success == false) {
                alert(jsonData.Data);
                return;
            }
            else if (jsonData.indexOf("from-login-error") > 0) {
                ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                setTimeout(function () {
                    var url = window.location.href.toString().split(window.location.host)[1];
                    window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                }, 1000);
            }
            $("#detailLst").html(jsonData);
            Portal.TableEdit("tbl-nghiphep", -1, 0, 0, "api", ['excel', 'print']);
        },
        error: function (xhr, status, error) {
            alert(xhr.responseText);
        }
    });
}
//Update PagePermission: btn-create, btn-edit, ....
$(document).on("click", ".funcChkBox", function () {
    var func = $(this).val();
    var check = $(this).prop("checked");
    var idhidden = $(this).data("hidden");
    $("#" + idhidden).val(check);
});
function DayoffCategory(even) {
    var idHidden = even.id;
    var value = $("#" + idHidden).val();
    $("#TypeDate_" + idHidden).val(value);
}