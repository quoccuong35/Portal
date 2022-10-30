
var Overtime = {};
Overtime.Init = function (controller) {
    Overtime.AddEmployee(controller);
    Overtime.DelEmployee(controller);
    Overtime.InportExcel(controller);
    Portal.TableEdit("table-nhansutangca", -1, 0, 0, "api", ['excel', 'print']);
}

Overtime.AddEmployee = function (controller) {
    $(document).on("click", "#btn-addemployee", function () {
        //var btn = this;
        var employeeCode = $("#txt-manhanvien").val();
        if (!employeeCode) {
            ShowToast.error("Chưa nhập mã nhân viên", 1000);
            return;
        }
        var data = $("#table-nhansutangca").DataTable().rows().data();
        var listOT = [];
        if (data.length > 0) {
            var item = {};
            for (var i = 0; i < data.length; i++) {
                item = {};
                item.OvertimeId = data[i][1];
                item.EmployeeId = data[i][2];
                item.EmployeeCode = data[i][3];
                item.FullName = data[i][4];
                item.DepartmentName = data[i][5];
                listOT.push(item);
            }
        }
        
        $.ajax({
            type: "POST",
            url: "/" + controller + "/AddEmployee",
            data: { listOT: listOT, employeeCode: employeeCode },
            success: function (jsonData) {
                if (jsonData.Success == false) {
                    ShowToast.error(jsonData.Data, 3000);
                    return;
                }
                else if (jsonData.indexOf("from-login-error") > 0) {
                    ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                    setTimeout(function () {
                        var url = window.location.href.toString().split(window.location.host)[1];
                        window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                    }, 1000);
                }
                else {
                    $("#detailLst").html(jsonData);
                    Portal.TableEdit("table-nhansutangca", -1, 0, 0, "api", ['excel', 'print']);
                }
            },
            error: function (xhr, status, error) {
                ShowToast.error(xhr.responseText, 3000);
            }
        });

    });
}
Overtime.DelEmployee = function (controller) {
    $(document).on("click", ".btn-delete", function () {
        var ok = confirm("Có muốn xóa thông tin đang chọn")
        if (!ok)
            return;
        var $btn = $(this);
        var employeeCode = $btn.data("code");
        var data = $("#table-nhansutangca").DataTable().rows().data();
        if (data.length == 0) {
            return;
        }
        var listOT = [];
        var item = {};
        for (var i = 0; i < data.length; i++) {
            var it = data[i];
            item = {};
            item.OvertimeId = data[i][1];
            item.EmployeeId = data[i][2];
            item.EmployeeCode = data[i][3];
            item.FullName = data[i][4];
            item.DepartmentName = data[i][5];
            listOT.push(item);
        }
        $.ajax({
            type: "POST",
            url: "/" + controller + "/DelEmployee",
            data: { listOT: listOT, employeeCode: employeeCode },
            success: function (jsonData) {
                if (jsonData.Success == false) {
                    ShowToast.error(jsonData.Data, 3000);
                    return;
                }
                else if (jsonData.indexOf("from-login-error") > 0) {
                    ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                    setTimeout(function () {
                        var url = window.location.href.toString().split(window.location.host)[1];
                        window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                    }, 1000);
                    
                }
                else {
                    $("#detailLst").html(jsonData);
                    Portal.TableEdit("table-nhansutangca", -1, 0, 0, "api", ['excel', 'print']);
                    ShowToast.success("Xóa thành công",1000)
                }
            },
            error: function (xhr, status, error) {
                ShowToast.error(xhr.responseText, 3000);
            }
        });
    })
}
Overtime.InportExcel = function (controller) {
    $(document).on("click", "#btn-importexcel", function () {
        var file = document.getElementById('importexcelfile').files[0];
        if (!file) {
            ShowToast.error("Chưa chọn file", 1000);
            return;
        }
        var formData = new FormData();
        formData.append("importexcelfile", file);
        $('#loading').show();
        
        $.ajax({
            type: "POST",
            url: "/" + controller + "/Import",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (jsonData) {
                $('#loading').hide();
                if (jsonData.Success == false) {
                    ShowToast.error(jsonData.Data, 3000);
                    return;
                }
                else if (jsonData.indexOf("from-login-error") > 0) {
                    ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                    setTimeout(function () {
                        var url = window.location.href.toString().split(window.location.host)[1];
                        window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                    }, 1000);
                }
                else {
                    $("#detailLst").html(jsonData);
                    Portal.TableEdit("table-nhansutangca", -1, 0, 0, "api", ['excel', 'print']);
                }
            },
            error: function (xhr, status, error) {
                $('#loading').hide();
                ShowToast.error(xhr.responseText,2000);
            }
        });
    })
}