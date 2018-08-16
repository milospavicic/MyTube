var loggedInStatus = false;
$('document').ready(function (e) {
    console.log("layout.js file start");
    $('#login-modal').on('hidden.bs.modal', function () {
        console.log(loggedInStatus);
        if (loggedInStatus === true)
            location.reload();
    });
});

function loginSubmit(event) {
    event.preventDefault();  // prevent standard form submission
    console.log("loginSubmitClick");

    var form = $("#loginForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            console.log("return value");
            $("#loginFormContent").html(partialResult);
            var page = 'Login Successful';
            if (partialResult.includes(page)) {
                loggedInStatus = true;
            }
        }
    });
}
function refreshPage(){
    location.reload();
}
function searchSubmit(event) {
    event.preventDefault();
    var searchParameter = $("#searchParameter").val();
    var url = "/Home/SearchPage/" + searchParameter;
    console.log(url);
    window.location.href = url;
}