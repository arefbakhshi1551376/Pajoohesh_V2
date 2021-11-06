$(document).ready(function () {
    $(".form-file-input").change(function () {
        debugger;
        var fileName = $(this).val().split('\\').pop();
        var fileLabel = $(this).parent().children(".form-file-name");
        $(fileLabel).html(fileName);
    });
});

function changeSideMenuVisibility() {
    $("#side-menu").toggleClass("hide");
    console.log($("#side-menu"));
}