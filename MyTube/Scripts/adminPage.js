var modalCloseReloadPage = false;
$('document').ready(function (e) {
    console.log("layout.js file start");
    $('#blockModal').on('hidden.bs.modal', function () {
        console.log(loggedInStatus);
        if (modalCloseReloadPage == true)
            refreshPage();
    });
    $('#unblockModal').on('hidden.bs.modal', function () {
        console.log(loggedInStatus);
        if (modalCloseReloadPage == true)
            refreshPage();
    });
    $('#deleteModal').on('hidden.bs.modal', function () {
        console.log(loggedInStatus);
        if (modalCloseReloadPage == true)
            refreshPage();
    });
    $('#editUserModal').on('hidden.bs.modal', function () {
        console.log(loggedInStatus);
        if (modalCloseReloadPage == true)
            refreshPage();
    });
});
function resetModalReloadPageComparator(){
    modalCloseReloadPage = false;
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
                loggedInStatus = true;
                modalCloseReloadPage = true;
                $("#editUserModal").html(partialResult);
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
                loggedInStatus = true;
                modalCloseReloadPage = true;
                $("#deleteModal").html(partialResult);
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
                loggedInStatus = true;
                modalCloseReloadPage = true;
                $("#blockModal").html(partialResult);
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
                loggedInStatus = true;
                modalCloseReloadPage = true;
                $("#unblockModal").html(partialResult);
            }
        }
    });
}

function refreshPage() {
    location.reload();
}
