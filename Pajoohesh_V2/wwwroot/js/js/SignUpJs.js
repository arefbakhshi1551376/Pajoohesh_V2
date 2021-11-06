$(document).ready(function () {
    debugger;
    var isValid = false;
    $("#signup-form").submit(function(e) {
        if (isValid === false) {
            e.preventDefault();
        }
    });

    $("#signup-confirm-password").blur(function () {
        var password = $("#signup-password").val();
        var confirmPassword = $("#signup-confirm-password").val();
        if (password != confirmPassword) {
            $("#signup-messages").html("رمز عبور درست تکرار نشده است!");
            if ($("#signup-messages").hasClass("success-message")) {
                $("#signup-messages").removeClass("success-message");
            }
        }
    });

    $("#signup-user-name").blur(checkUserNameAndPassword);

    $("#signup-email").blur(checkUserNameAndPassword);

    function checkUserNameAndPassword() {
        var userNameLength = $(this).val().length;
        var emailLength = $("#signup-email").val().toString();

        if (userNameLength === 0 && emailLength === 0) {
            $("#signup-messages").html("");
        }
        else if (userNameLength < 8) {
            $("#signup-messages").html("نام کاربری باید حداقل 8 کاراکتر باشد!");
            if ($("#signup-messages").hasClass("success-message")) {
                $("#signup-messages").removeClass("success-message");
            }
        } else {
            if (emailLength.length !== 0) {
                $.ajax({
                    method: "POST",
                    url: "/Identity/Account/CheckUserNameAndEmail",
                    data: {
                        userName: $("#signup-user-name").val().toString(),
                        email: $("#signup-email").val().toString()
                    },
                    success: function (response) {
                        if (response === false) {
                            $("#signup-messages").html("نام کاربری و ایمیل مورد قبول است!");
                            $("#signup-messages").addClass("success-message");
                        } else {
                            $("#signup-messages").html("نام کاربری یا ایمیل قبلا انتخاب شده است!");
                            if ($("#signup-messages").hasClass("success-message")) {
                                $("#signup-messages").removeClass("success-message");
                            }
                            disableButton();
                        }
                    },
                    error: function () {
                        $("#signup-messages").html("مشکلی پیش آمده! لطفا دوباره تلاش کنید!");
                        if ($("#signup-messages").hasClass("success-message")) {
                            $("#signup-messages").removeClass("success-message");
                        }
                    }
                });
            } else {
                $("#signup-messages").html("لطفا ایمیل خود را وارد نمایید!");
                if ($("#signup-messages").hasClass("success-message")) {
                    $("#signup-messages").removeClass("success-message");
                }
            }
        }
    }
});