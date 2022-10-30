var Portal = {};
var ShowToast = {
    success: function (mess, time) {
        $.notify(mess, {
            type: 'success',
            close: true,
            timer: time,
            align: "center",
            verticalAlign: "middle",
            delay: time,
            icon: "check",
            background: "#20D67B",
            color: "#fff"
        });
    },
    info: function (mess, time) {
        $.notify(mess, {
            type: 'info',
            close: true,
            timer: time,
            align: "center",
            verticalAlign: "middle",
            delay: time,
            icon: "info",
            background: "#4B7EE0",
            color: "#fff"
        }
        );
    },
    warning: function (mess, time) {
        $.notify(mess, {
            type: 'warning',
            close: true,
            timer: time,
            align: "center",
            verticalAlign: "middle",
            delay: time,
            icon: "exclamation-circle",
            background: "#A5881B",
            color: "#fff"
        });
    },
    error: function (mess, time) {
        $.notify(mess, {
            type: 'error',
            close: true,
            timer: time,
            align: "center",
            verticalAlign: "middle",
            delay: time,
            icon: "exclamation-triangle",
            background: "#D44950",
            color: "#fff"
        });
    }
}
//create: Save Data
Portal.SaveData = function (controller, frmCreate, isContinue, e) {
    var $btn = $(e);
    var element = $("#" + $btn[0].id + " i").prop("class");
    /// Hide Ion save
    $("#" + $btn[0].id + " i").toggleClass("d-none");
    // show spinner
    $("#" + $btn[0].id + " span").toggleClass("d-none");
    // disabled button
    $("#" + $btn[0].id).toggleClass("disabled");
   // console.log(element);
    
    //var frm = $(frmCreate);
    var frm = $(frmCreate),
        formData = new FormData(),
        formParams = frm.serializeArray();

    if (frm.valid()) {
        $.each(frm.find('input[type="file"]'), function (i, tag) {
            $.each($(tag)[0].files, function (i, file) {
                formData.append(tag.name, file);
            });
        });

        $.each(formParams, function (i, val) {
            formData.append(val.name, val.value);
        });

        $.ajax({
            type: "POST",
            url: "/" + controller + "/Create",
            data: formData,
            processData: false,
            contentType: false,
            success: function (jsonData) {
                // Ẩn Ion save di
                $("#" + $btn[0].id + " i").toggleClass("d-none")
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                if (jsonData.Success == true) {
                    if (isContinue == true) {
                        frm[0].reset();
                       
                        if (jsonData.Data != null) {
                            ShowToast.success(jsonData.Data, 3000);

                        }
                    }
                    else {
                        //window.location.href = "/" + controller;
                        window.location.href = "/" + controller + "?message=" + jsonData.Data;
                    }
                }
                else {
                    if (jsonData.Data != null && jsonData.Data != "") {
                        ShowToast.error(jsonData.Data, 10000);
                    }
                    else if (jsonData.indexOf("from-login-error") > 0) {
                        ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                        setTimeout(function () {
                            var url = window.location.href.toString().split(window.location.host)[1];
                            window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                        }, 1000);
                    }
                }
            },
            error: function (xhr, status, error) {
                // Ẩn Ion save di
                $("#" + $btn[0].id + " i").toggleClass("d-none")
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
               // alertPopup(false, xhr.responseText);
                ShowToast.error(xhr.responseText, 10000);
            }
        });
    }
    else {
        //show error invalid
        // Ẩn Ion save di
        $("#" + $btn[0].id + " i").toggleClass("d-none")
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");
               // alertPopup(false, xhr.responseText);
        var validator = frm.validate();
        $.each(validator.errorMap, function (index, value) {
            console.log('Id: ' + index + ' Message: ' + value);
            //ShowToast.error('Id: ' + index + ' Message: ' + value, 10000);
        });
        $btn.button('reset');
    }
}

Portal.CreateInitial = function (controller) {
    $(document).on("click", "#btn-save-continue", function (e) {
        var isContinue = true;
        Portal.SaveData(controller, "#frmCreate", isContinue, this);
    });

    $(document).on("click", "#btn-save", function () {
        var isContinue = false;
        Portal.SaveData(controller, "#frmCreate", isContinue, this);
    });
}

//edit: Edit Data
Portal.EditData = function (controller, frmEdit, isContinue, e) {
    try {
        var $btn = $(e);
        // Ẩn Ion save di
        $("#" + $btn[0].id + " i").toggleClass("d-none")
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");

        var frm = $(frmEdit),
            formData = new FormData(),
            formParams = frm.serializeArray();

        if (frm.valid()) {
            $.each(frm.find('input[type="file"]'), function (i, tag) {
                $.each($(tag)[0].files, function (i, file) {
                    formData.append(tag.name, file);
                });
            });

            $.each(formParams, function (i, val) {
                formData.append(val.name, val.value);
            });

            $.ajax({
                type: "POST",
                url: "/" + controller + "/Edit",
                data: formData,
                processData: false,
                contentType: false,
                success: function (jsonData) {
                    $btn.button('reset');
                    // Ẩn Ion save di
                    $("#" + $btn[0].id + " i").toggleClass("d-none")
                    // hiện loading lên
                    $("#" + $btn[0].id + " span").toggleClass("d-none")
                    // disabled button
                    $("#" + $btn[0].id).toggleClass("disabled");
                    if (jsonData.Success == true) {
                        if (isContinue == true) {
                            if (jsonData.Data != null) {
                                ShowToast.success(jsonData.Data, 1000);
                                setTimeout(function () { window.location.href = "/" + controller + "/Create"; }, 1000);
                                // alertPopup(true, jsonData.Data);
                            }
                        }
                        else {
                            //window.location.href = "/" + controller;
                            window.location.href = "/" + controller + "?message=" + jsonData.Data;
                        }
                    }
                    else {
                        if (jsonData.Data != null && jsonData.Data != "") {
                            //alertPopup(false, jsonData.Data);
                            ShowToast.error(jsonData.Data, 10000);
                        }
                        else if (jsonData.indexOf("from-login-error") > 0) {
                            ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                            setTimeout(function () {
                                var url = window.location.href.toString().split(window.location.host)[1];
                                window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                            }, 1000);
                        }
                    }
                },
                error: function (xhr, status, error) {
                    // Ẩn Ion save di
                    $("#" + $btn[0].id + " i").toggleClass("d-none")
                    // hiện loading lên
                    $("#" + $btn[0].id + " span").toggleClass("d-none")
                    // disabled button
                    $("#" + $btn[0].id).toggleClass("disabled");
                    // alertPopup(false, xhr.responseText);
                    ShowToast.error(xhr.responseText, 10000);
                }
            });
        }
        else {
            // Ẩn Ion save di
            $("#" + $btn[0].id + " i").toggleClass("d-none")
            // hiện loading lên
            $("#" + $btn[0].id + " span").toggleClass("d-none")
            // disabled button
            $("#" + $btn[0].id).toggleClass("disabled");
            //show error invalid
            var validator = frm.validate();
            $.each(validator.errorMap, function (index, value) {
                console.log('Id: ' + index + ' Message: ' + value);
            });
            $btn.button('reset');
        }
    } catch (e) {
        console.log(e);
        // Ẩn Ion save di
        $("#" + $btn[0].id + " i").toggleClass("d-none")
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");
    }
    
}

Portal.EditInitial = function (controller) {
    $(document).on("click", "#btn-save-continue", function (e) {
        var isContinue = true;
        Portal.EditData(controller, "#frmEdit", isContinue, this);
    });

    $(document).on("click", "#btn-save", function () {
        var isContinue = false;
        Portal.EditData(controller, "#frmEdit", isContinue, this);
    });
     // send email 
    Portal.SendEmail();
}
Portal.SearchDefault = function (controller,leftColumns) {
    var $btn = $("#btn-search");
    //$btn.button('loading');
    // Ẩn Ion save di
    $("#" + $btn[0].id + " i").toggleClass("d-none")
    // hiện loading lên
    $("#" + $btn[0].id + " span").toggleClass("d-none")
    // disabled button
    $("#" + $btn[0].id).toggleClass("disabled");
    $.ajax({
        type: "POST",
        url: "/" + controller + "/_Search",
        data: $("#frmSearch").serializeArray(),
        success: function (xhr, status, error) {
            $("#" + $btn[0].id + " i").toggleClass("d-none")
            // hiện loading lên
            $("#" + $btn[0].id + " span").toggleClass("d-none")
            // disabled button
            $("#" + $btn[0].id).toggleClass("disabled");
            if (xhr.Code == 500 || xhr.Success == false) {
                //error
                //ShowToast.error(xhr.Data, 10000);
                //alertPopup(false, xhr.Data);
            } 
            else if (xhr.indexOf("from-login-error") > 0) {
                ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                setTimeout(function () {
                    var url = window.location.href.toString().split(window.location.host)[1];
                    window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                }, 1000);
            }
            else {
                //success
                $("#divSearchResult").html(xhr);
                //Portal.Table();
                //Portal.TableEdit(4);
            }

            $(document).on("click", ".btn-edit", function (e) {
                if ($(this).is('[disabled=disabled]')) {
                    e.preventDefault();
                    return false;
                }
            });
        },
        error: function (xhr, status, error) {
            $("#" + $btn[0].id + " i").toggleClass("d-none")
            // hiện loading lên
            $("#" + $btn[0].id + " span").toggleClass("d-none")
            // disabled button
            $("#" + $btn[0].id).toggleClass("disabled");
            //alertPopup(true, xhr.responseText);
            ShowToast.error(xhr.responseText, 10000);
        }
    });
}

//search initial
Portal.SearchInitialfix = function (controller, leftColumns) {
    //set btn-search event
    $("#btn-search").click(function () {
        Portal.SearchDefault(controller, leftColumns);
    });
    //click btn-search button at first time
    $("#btn-search").trigger("click");
    //set default form submit => click btn-search button
    $("#frmSearch").submit(function (e) {
        e.preventDefault();
        $("#btn-search").trigger("click");
    });

    //delete button
    Portal.Delete();

    //import button
    Portal.UploadFile(controller);
    Portal.ImportModalHideHandler();
}
Portal.SearchInitial = function (controller) {
    //set btn-search event
    $("#btn-search").click(function () {
        Portal.SearchDefault(controller,0);
    });
    //click btn-search button at first time
    $("#btn-search").trigger("click");
    //set default form submit => click btn-search button
    $("#frmSearch").submit(function (e) {
        e.preventDefault();
        $("#btn-search").trigger("click");
    });

    //delete button
    Portal.Delete();

   
    //import button
    Portal.UploadFile(controller);
    Portal.ImportModalHideHandler();
}
Portal.UploadFile = function (controller) {
    $(document).on("click", "#btn-importExcel", function () {
        var $btn = $(this);
        //$btn.button('loading');
        $("#" + $btn[0].id + " i").toggleClass("d-none")
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");
        //alert
        $(".modal-alert-message").html("");
        $(".modalAlert").hide();

        //ISD.ProgressBar.showPleaseWait(); //show dialog
        var file = document.getElementById('importexcelfile').files[0];
        var formData = new FormData();
        formData.append("importexcelfile", file);

        //#region using ajax XMLHttpRequest
        //ajax = new XMLHttpRequest();
        //ajax.upload.addEventListener("progress", progressHandler, false);
        //ajax.addEventListener("load", completeHandler, false);
        //ajax.open("POST", "/ReportTest/ImportExcelMTDynamic");
        //ajax.send(formData);
        //#endregion using ajax XMLHttpRequest

        $.ajax({
            type: "POST",
            url: "/" + controller + "/Import",
            data: formData,
            xhr: function () {
                var myXhr = $.ajaxSettings.xhr();
                if (myXhr.upload) {
                    myXhr.upload.addEventListener('progress', progressHandler, false);
                    myXhr.addEventListener("load", completeHandler, false);
                }
                return myXhr;
            },
            cache: false,
            contentType: false,
            processData: false,
            success: function (jsonData) {
                $btn.button('reset');
                $("#" + $btn[0].id + " i").toggleClass("d-none")
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                if (jsonData.Success == true) {
                    //formData[0].reset();
                    if (jsonData.Data) {
                        alertModalPopup(true, jsonData.Data);
                    }
                    setTimeout(function () {
                        $("#importexcelfile").val("");
                    }, 3000);
                }
                else {
                    if (jsonData.Data) {
                        alertModalPopup(false, jsonData.Data);
                    }
                    else if (jsonData.indexOf("from-login-error") > 0) {
                        ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                        setTimeout(function () {
                            var url = window.location.href.toString().split(window.location.host)[1];
                            window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                        }, 1000);
                    }
                }
            },
            error: function (xhr, status, error) {
                $("#" + $btn[0].id + " i").toggleClass("d-none")
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                $btn.button('reset');
                alertModalPopup(false, xhr.responseText);
            }
        });
    });

    function progressHandler(event) {
        if (event.lengthComputable) {
            var percent = Math.round((event.timeStamp / event.total) * 100);
            var loadingPercent = percent + "%";
            $('.progress-bar').width(loadingPercent); //from bootstrap bar class
        }
    }

    function completeHandler() {
        $('.progress-bar').width("100%");
        setTimeout(function () {
            //ISD.ProgressBar.hidePleaseWait(); //hide dialog
        }, 3000);
    }
}
Portal.Delete = function () {
    $(document).on("click", ".btn-delete", function () {
        var $btn = $(this);

        var controller = $btn.data("current-url");
        var itemName = $btn.data("item-name");
        var id = $btn.data("id");

        $("#divDeletePopup").modal("show");
        //set title
        $("#divDeletePopup .modal-title .item-name").html(itemName);
        //set text
        var text = $("#divDeletePopup .alert-message").html();
        //replace new text
        text = text.replace(/"([^"]*)"/g, '"' + itemName + '"');
        text = String.format(text, itemName);
        //show new text
        $("#divDeletePopup .alert-message").html(text);

        //get id, controller
        $("#divDeletePopup #id").val(id);
        $("#divDeletePopup #controller").val(controller);
    });

    //click button confirm
    $(document).on("click", "#btn-confirm-delete", function () {
        var $btn = $(".btn-confirm-delete");
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");
        var controller = $('form[id="frmConfirm"] #controller').val();
        //var del = $('form[id="frmConfirm"]').serialize();

        var formData = {};
        formData.id = $("#divDeletePopup #id").val();
        formData.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            type: "POST",
            url: "/" + controller + "/Delete",
            data: formData,
            success: function (jsonData) {
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                $btn.button('reset');
                $("#divDeletePopup").modal("hide");
                if (jsonData.Success == true) {
                    $("#btn-search").trigger("click");
                    ShowToast.success(jsonData.Data, 3000);
                }
                else {
                    if (jsonData.Data != null && jsonData.Data != "") {
                        ShowToast.error(jsonData.Data, 10000);
                    }
                    else if (jsonData.indexOf("from-login-error") > 0) {
                        ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                        setTimeout(function () {
                            var url = window.location.href.toString().split(window.location.host)[1];
                            window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                        }, 1000);
                    }
                }
            },
            error: function (xhr, status, error) {
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                $btn.button('reset');
                $("#divDeletePopup").modal("hide");
                alertPopup(false, xhr.responseText);
            }
        });
    });

    //click button cancel
    $(document).on("click", "#btn-cancel-delete", function () {
        var $btn = $(".btn-delete");
        $btn.button('reset');
        $("#divDeletePopup").modal("hide");
    });
    //click outside popup
    $('#divDeletePopup').on('hidden.bs.modal', function () {
        var $btn = $(".btn-delete");

        $btn.button('reset');
        $("#divDeletePopup").modal("hide");
    });
}

Portal.SendEmail = function () {
    $(document).on("click", ".btn-sendemail", function () {

        var $btn = $(this);

        var controller = $btn.data("current-url");
        var itemName = $btn.data("item-name");
        var id = $btn.data("id");

        $("#divSendEmailPopup").modal("show");
        //set title
        $("#divSendEmailPopup .modal-title .item-name").html(itemName);
        //set text
        var text = $("#divSendEmailPopup .alert-message").html(); 
        //replace new text
        text = text.replace(/"([^"]*)"/g, '"' + itemName + '"');
        text = String.format(text, itemName);
        //show new text
        $("#divSendEmailPopup .alert-message").html(text);

        //get id, controller
        $("#divSendEmailPopup #idSendEmail").val(id);
        $("#divSendEmailPopup #controller-sendmail").val(controller);
    });

    $(document).on("click", "#btn-confirm-sendemail", function () {
        var $btn = $(".btn-confirm-sendemail");

        $("#" + $btn[0].id + " i").toggleClass("d-none")
        // hiện loading lên
        $("#" + $btn[0].id + " span").toggleClass("d-none")
        // disabled button
        $("#" + $btn[0].id).toggleClass("disabled");

        var controller = $('form[id="frmSendmail"] #controller-sendmail').val();

        var formData = {};
        formData.idSendEmail = $("#frmSendmail #idSendEmail").val();
        formData.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
        //data: $('form[id="frmSendmail"]').serialize(),
        $.ajax({
            type: "POST",
            url: "/" + controller + "/SendEmail",
            data: formData,
            success: function (jsonData) {
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");
                $btn.button('reset');
                $("#divSendEmailPopup").modal("hide");
                if (jsonData.Success == true) {
                    //alertPopup(true, jsonData.Data);
                    ShowToast.success(jsonData.Data, 1000);
                    setTimeout(function () { window.location.reload(); }, 1000);
                }
                else {
                    if (jsonData.Data != null && jsonData.Data != "") {
                        // alertPopup(false, jsonData.Data);
                        ShowToast.error(jsonData.Data, 10000);
                    }
                    else if (jsonData.indexOf("from-login-error") > 0) {
                        ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                        setTimeout(function () {
                            var url = window.location.href.toString().split(window.location.host)[1];
                            window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                        }, 1000);
                    }
                }
            },
            error: function (xhr, status, error) {
                // hiện loading lên
                $("#" + $btn[0].id + " span").toggleClass("d-none")
                // disabled button
                $("#" + $btn[0].id).toggleClass("disabled");

                $btn.button('reset');
                $("#divSendEmailPopup").modal("hide");
                ShowToast.error(xhr.responseText, 10000);
            }
        });
    });

    //click button cancel
    $(document).on("click", "#btn-cancel-sendemail", function () {
        var $btn = $(".btn-sendemail");

        $btn.button('reset');
        $("#divSendEmailPopup").modal("hide");
    });
    //click outside popup
    $('#divSendEmailPopup').on('hidden.bs.modal', function () {
        var $btn = $(".btn-sendemail");

        $btn.button('reset');
        $("#divSendEmailPopup").modal("hide");
    });
}

Portal.Table = function () {
    $("#data-list").DataTable({
        "responsive": false,
        "autoWidth": true,
        "autoHeight": false,
        "scrollCollapse": false,
        "scrollX": true,
        "select": true,
        "fixedColumns":{
            "leftColumns": 0,
        },
        "scrollY": 300,
        "iDisplayLength": 50,
        "language": {
            "emptyTable": "Không có dữ liệu",
            "search": ""
        }
    });
};

Portal.TableEdit = function (id = "data-list", displayNume = 50, leftColumns = 0, rightColumns = 0, tyle = 'api', buttons = ['excel','print']) {
    //  style: 'single', style: 'multi'
    //butoms = ['copy', 'csv', 'excel', 'pdf', 'print']
    $("#"+id).DataTable({
        "responsive": false,
        "autoWidth": true,
        "autoHeight": true,
        "scrollCollapse": true,
        "scrollX": true,
        "select": true,
        "scrollY": 300,
        "iDisplayLength": displayNume,
        "fixedColumns": {
            "leftColumns": leftColumns,
            "rightColumns": rightColumns
        },
        "language": {
            "emptyTable": "Không có dữ liệu",
            "search": ""
        },
        select: {
            style: tyle,
        },
        dom: 'Bfrtip',
        buttons: buttons

    });
    $('.dataTables_filter input').attr("placeholder", "Tìm kiếm");
    $('.dataTables_filter label').append('<i class="fa fa-search"></i>');
};
//format string
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
}
//reset input file when modal hide
Portal.ImportModalHideHandler = function () {
    $('#importexcel-window').on('hidden.bs.modal', function (e) {
        document.getElementById("importexcelfile").value = "";

        //alert
        $(".modal-alert-message").html("");
        $(".modalAlert").hide();

        $.ajax({
            type: "GET",
            url: "/Permission/Menu/_Search",
            success: function (jsonData) {
                $("#menuTable").html(jsonData);

            },
            error: function (xhr, status, error) {
                alertModalPopup(false, xhr.responseText);
            }
        });
    })
}
//get message from current url
Portal.GetQueryString = function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

//show message on Index
Portal.ShowMessage = function (url) {
    var text = Portal.GetQueryString("message", url);
    if (text != null) {
        //alertPopup(true, text);
        ShowToast.info(text, 5000);
        history.pushState(null, '', window.location.href.split("?")[0]);
    }
}

//change password
Portal.ChangePasswordInitial = function (controller) {
    $(document).on("click", "#btn-save", function () {
        Portal.ChangePassword(controller, "#frmChangePassword", this);
    });
}

Portal.ChangePassword = function (controller, frmChangePassword, e) {
    var $btn = $(e);
    var frm = $(frmChangePassword);
    if (frm.valid()) {
        $.ajax({
            type: "POST",
            url: "/" + controller + "/ChangePassword",
            data: frm.serialize() + "&UserName=" + $("#UserName").val(),
            success: function (jsonData) {
                $btn.button('reset');
                if (jsonData.Success == true) {
                    $('input,select,textarea').not('[readonly],[disabled],:button').val('');
                    //alertPopup(true, jsonData.Data);
                    ShowToast.success(jsonData.Data, 3000);
                }
                else {
                    if (jsonData.Data != null && jsonData.Data != "") {
                        //alertPopup(false, jsonData.Data);
                        ShowToast.error(jsonData.Data, 10000);
                    }
                }
            },
            error: function (xhr, status, error) {
                $btn.button('reset');
                //alertPopup(false, xhr.responseText);
                ShowToast.error(xhr.responseText, 10000);
            }
        });
    }
    else {
        //show error invalid
        var validator = frm.validate();
        $.each(validator.errorMap, function (index, value) {
            console.log('Id: ' + index + ' Message: ' + value);
        });
        $btn.button('reset');
    }
}

// Duyệt quy trình

Portal.Selectall = function (idtable) {
    $(document).on("click", "#chk-selectall", function () {
        var data = $("#" + idtable).DataTable().rows()
            .data();
        if ($(this).is(":checked")) {
            data.rows().select();
        } else {
            data.rows().deselect();
        }
    });
    Portal.Approval(idtable)
    Portal.Cancel(idtable)
}

Portal.Approval = function (idtable = null) {
    $(document).on("click", "#btn-appval", function () {
        var $btn = $(this);
        var controller = $btn.data("url");
      
        
        var dataArr = [];
        
        if (idtable) {
            var data = $("#" + idtable).DataTable().rows({ selected: true }).data();
            if (data.length == 0) {
                ShowToast.info("Chưa chọn thông tin duyệt", 3000);
                return;
            }
            for (var i = 0; i < data.length; i++) {
                dataArr.push(data[i][1]);
            }
        }
        else {
            var id = $btn.data("id");
            dataArr.push(id);
        }
        var formData = {};
        formData.lid = dataArr;
        formData.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
        swal({
            title: 'Thông báo!',
            text: "Bạn có chắc muốn duyệt các yêu cầu đang chọn!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Duyệt',
            cancelButtonText: 'Thoát',
            showCloseButton: true,
        }).then(function () {
            $('#loading').show();
            $.ajax({
                type: "POST",
                url: "/" + controller + "/Approval",
                data: formData,
                success: function (jsonData) {
                    $('#loading').hide();
                    if (jsonData.Success == true) {
                        // duyệt chi tiết
                        if (id) {
                            window.location.href = "/" + controller + "?message=" + jsonData.Data;
                        }
                        else {
                            // duyệt nhiều cái cùng lúc
                            ShowToast.success(jsonData.Data, 2000);
                            setTimeout(function () { $("#btn-search").trigger("click"); }, 1000);
                        }
                        
                       
                    }
                    else {
                        if (jsonData.Data != null && jsonData.Data != "") {
                            ShowToast.error(jsonData.Data, 3000);
                            //setTimeout(function () { $("#btn-search").trigger("click"); }, 3000);
                        }
                        else if (jsonData.indexOf("from-login-error") > 0) {
                            ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                            setTimeout(function () {
                                var url = window.location.href.toString().split(window.location.host)[1];
                                window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                            }, 1000);
                        }
                    }
                },
                error: function (xhr, status, error) {
                    $('#loading').hide();
                    ShowToast.error(xhr.responseText, 3000);
                }
            });
        });
    })
   
}

Portal.Cancel = function (idtable) {
    $(document).on("click", "#btn-cancel", function () {
        var $btn = $(this);
        var controller = $btn.data("url");
        
        var dataArr = [];
        if (idtable) {
            var data = $("#" + idtable).DataTable().rows({ selected: true }).data();
            if (data.length == 0) {
                ShowToast.info("Chưa chọn thông tin duyệt", 3000);
                return;
            }
            for (var i = 0; i < data.length; i++) {
                dataArr.push(data[i][1]);
            }
        }
        else {
            var id = $btn.data("id");
            dataArr.push(id);
        }
        var formData = {};
        formData.lid = dataArr;
        formData.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
        swal({
            title: 'Từ chối!',
            input: 'text',
            inputPlaceholder: 'Nhập lý do hủy',
            showCancelButton: true,
            cancelButtonText: 'Thoát',
            confirmButtonText: 'Từ chối',
            preConfirm: function (value) {
                return new Promise(function (resolve, reject) {
                    if (value) {
                        resolve()
                    } else {
                        reject('Chưa nhập lý do!')
                    }
                })
            },
        }).then(function (lydo) {
            formData.lyDo = lydo;
            $('#loading').show();
            $.ajax({
                type: "POST",
                url: "/" + controller + "/Cancel",
                data: formData,
                success: function (jsonData) {
                    $('#loading').hide();
                    if (jsonData.Success == true) {
                        // duyệt chi tiết
                        if (id) {
                            window.location.href = "/" + controller + "?message=" + jsonData.Data;
                        }
                        else {
                            // duyệt nhiều cái cùng lúc
                            ShowToast.success(jsonData.Data, 2000);
                            setTimeout(function () { $("#btn-search").trigger("click"); }, 1000);
                        }


                    }
                    else {
                        if (jsonData.Data != null && jsonData.Data != "") {
                            ShowToast.error(jsonData.Data, 3000);
                            //setTimeout(function () { $("#btn-search").trigger("click"); }, 3000);
                        }
                        else if (jsonData.indexOf("from-login-error") > 0) {
                            ShowToast.error('Hết thời gian thao tác xin đăng nhập lại', 500);
                            setTimeout(function () {
                                var url = window.location.href.toString().split(window.location.host)[1];
                                window.location.href = "/Permission/Auth/Login?returnUrl=" + url;
                            }, 1000);
                        }
                    }
                },
                error: function (xhr, status, error) {
                    $('#loading').hide();
                    ShowToast.error(xhr.responseText, 3000);
                }
            });
        });

    });

}