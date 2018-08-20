$('document').ready(function (e) {

    $('#upload').hide();

});
function toggleUrlUpload() {
    $('#link').toggle();
    $('#upload').toggle();
}

function submitPicture() {
    if ($('#linkOrLocalPicture').prop('checked')) {
        uploadPicture();
    } else {
        urlPicture();
    }
}
function uploadPicture() {
    $("#uploadPicForm").submit();
}
function urlPicture() {
    $("#uploadPicForm").attr("action", "/Videos/ChangePictureUrl/");
    $("#uploadPicForm").submit();
}