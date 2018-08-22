var modalCloseReloadPage = false;
$('document').ready(function (e) {
    console.log("layout.js file start");

    $('#newPasswordModal').on('shown.bs.modal', function (e) {
        $('#changeModalMenu').modal('hide');
    });
    $('#changeThumbnailModal').on('shown.bs.modal', function (e) {
        $('#linkOrLocalPicture').prop('checked', false);
        $('#link').show();
        $('#upload').hide();
        $('#changeModalMenu').modal('hide');
    });
    $('#editUserModal').on('shown.bs.modal', function (e) {
        $('#changeModalMenu').modal('hide');
    });
});
function resetModalReloadPageComparator() {
    modalCloseReloadPage = false;
}
function editMenu(userName) {
    $('#editUserButtonEditMenu').attr('onclick', 'editModal(\'' + userName + '\')');
    $('#username').val(userName);
    $('#newPasswordForm').attr('action', "/Users/NewPassword/" + userName);
    $('#changeModalMenu').modal();
}

function editModal(userName) {
    resetModalReloadPageComparator();
    userEditFill(userName);
    console.log("editModal(" + userName + ")");
    $("#userEditForm").attr("action", "/Users/EditUserForm/" + userName);
    $('#editUserModal').modal();
}
function userEditFill(userName) {
    $.ajax({
        url: "/Users/EditUserForm/" + userName,
        method: "GET",  // post
        success: function (partialResult) {
            console.log("return value");
            $("#userEditFormContainer").html(partialResult);
        }
    });
}
function userEditSubmit(event) {
    event.preventDefault();  // prevent standard form submission
    var form = $("#userEditForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            console.log("return value");
            var page = 'User has been successfully edited';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#editUserModal").modal('hide');
                loadSingleUser(GetUsernameFromUrl(form.attr("action")));
            } else {

                $("#userEditFormContainer").html(partialResult);
            }

        }
    });
}

function deleteModal(userName) {
    resetModalReloadPageComparator();
    $("#DeleteModalYesButton").attr("href", "/Users/DeleteUser/" + userName);
    $('#deleteModal').modal();
}
function deleteUser(event) {
    event.preventDefault();
    url = $("#DeleteModalYesButton").attr("href");
    console.log(url);
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully deleted';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#deleteModal").modal('hide');
                var username = GetUsernameFromUrl(url);
                $('#userDiv-' + username).hide();
            }
        }
    });
}
function blockModal(userName) {
    resetModalReloadPageComparator();
    $("#BlockModalYesButton").attr("href", "/Users/BlockUser/" + userName);
    $('#blockModal').modal();
}
function blockUser(event) {
    event.preventDefault();
    url = $("#BlockModalYesButton").attr("href");
    console.log(url);
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully blocked';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#blockModal").modal('hide');
                loadSingleUser(GetUsernameFromUrl(url));
            }
        }
    });
}

function unblockModal(userName) {
    resetModalReloadPageComparator();
    $("#UnblockModalYesButton").attr("href", "/Users/UnblockUser/" + userName);
    $('#unblockModal').modal();
}
function unblockUser(event) {
    event.preventDefault();
    url = $("#UnblockModalYesButton").attr("href");
    console.log(url);
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully unblocked';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#unblockModal").modal('hide');
                loadSingleUser(GetUsernameFromUrl(url));
            }
        }
    });
}
function GetUsernameFromUrl(url) {
    var username = url.substring(url.lastIndexOf('/') + 1);
    return username;
}
function loadSingleUser(username) {
    $.ajax({
        url: '/Users/OneUserForAdminPage/' + username,
        type: 'post',
        success: function (partialResult) {
            $('#userDiv-' + username).replaceWith(partialResult);
        }
    });
}

function refreshPage() {
    location.reload();
}

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
    if ($('#image').get(0).files.length === 0) {
        console.log("No files selected.");
        return;
    }
    $("#uploadPicForm").submit();
}
function urlPicture() {
    $("#uploadPicForm").attr("action", "/Users/ChangePictureUrl/");
    $("#uploadPicForm").submit();
}

function newPassword() {
    var form = $("#newPasswordForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            var page = 'Password has been successfully changed.';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#newPasswordModal").modal('hide');
            } else {
                $('#newPasswordModalContainer').html(partialResult);
            }
        }
    });
}
function messageModal(partial) {
    $("#messageModal").html(partial);
    $("#messageModal").modal();
}
function newPasswordModal() {
    $.ajax({
        url: '/Users/NewPassword',
        method: 'get',  // post
        success: function (partialResult) {
            $('#newPasswordModalContainer').html(partialResult);
        }
    });
    $('#newPasswordModal').modal();
}