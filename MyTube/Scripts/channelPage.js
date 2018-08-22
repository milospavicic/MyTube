var modalCloseReloadPage = false;
$('document').ready(function (e) {
    loadVideos(true);
    $('#upload').hide();
    $('.changeActive').click(function () {
        $('.active').removeClass('active');
        $(this).addClass('active');
    });

    $('#userVideos').click(function () {
        loadVideos(true);
    });
    $('#userLiked').click(function () {
        loadVideos(false);
    });
    $('#userSubs').click(function () {
        loadUsers();
    });

    $('#blockModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#unblockModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#deleteModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#editUserModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#newPasswordModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });

    $('#newPasswordModal').on('shown.bs.modal', function (e) {
        $('#changeModalMenu').modal('hide');
    });
    $('#changeThumbnailModal').on('shown.bs.modal', function (e) {
        $('#changeModalMenu').modal('hide');
    });
    $('#editUserModal').on('shown.bs.modal', function (e) {
        $('#changeModalMenu').modal('hide');
    });
});
function loadVideos(ownedOrLikedVideos) {
    var channelName = $('#name').text();
    var sortOrder = $('#sortOrder').val();
    var data =
        {
            "channelName": channelName,
            "ownedOrLikedVideos": ownedOrLikedVideos,
            "sortOrder": sortOrder
        };
    $.ajax({
        url: "/Videos/ChannelPageVideosPartial",
        method: "GET",  // post
        data: data,
        success: function (partialResult) {
            console.log("return value loadVideos");
            $("#mainDiv").html(partialResult);
        }
    });
}
function loadUsers() {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/ChannelPageUsersPartial/" + channelName,
        method: "GET",  // post
        success: function (partialResult) {
            console.log("return value loadUsers");
            $("#mainDiv").html(partialResult);
        }
    });
}
function loadInfo(event) {
    event.preventDefault();
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/ChannelPageInfoPartial/" + channelName,
        method: "GET",  // post
        success: function (partialResult) {
            console.log("return value loadInfo");
            $("#mainDiv").html(partialResult);
        }
    });
}
function loadEditModal(event) {
    event.preventDefault();
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/EditUserForm/" + channelName,
        method: "GET",  // post
        success: function (partialResult) {
            console.log("return value loadModal");
            $("#userEditFormContainer").html(partialResult);
            $("#userEditForm").attr("action", "/Users/EditUserForm/" + channelName);
            $('#editUserModal').modal();
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
                modalCloseReloadPage = true;
                $("#editUserModal").html(partialResult);
            } else {
                $("#userEditFormContainer").html(partialResult);
            }
        }
    });
}
function deleteUser(event) {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/DeleteUser/" + channelName,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully deleted';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#deleteModal").html(partialResult);
            }
        }
    });
}
function blockUser(event) {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/BlockUser/" + channelName,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully blocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#blockModal").html(partialResult);
            }
        }
    });
}

function unblockUser(event) {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/UnblockUser/" + channelName,
        type: 'post',
        success: function (partialResult) {
            var page = 'User has been successfully unblocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#unblockModal").html(partialResult);
            }
        }
    });
}
function subUser() {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/Subscribe/" + channelName,
        type: 'post',
        dataType: "json",
        success: function (data) {
            subOrUnsubChangeButton(data);
        }
    });
}
function subOrUnsubChangeButton(data) {
    if (data.subStatus === false) {
        sub();
        changeSubsCount(data.subCount);
    }
    else if (data.subStatus === true) {
        unsub();
        changeSubsCount(data.subCount);
    }
}
function sub() {
    var tempName = $("#subButton");
    tempName.text("Unsubscribe"); 
    tempName.attr('class', 'btn btn-default');
}

function unsub() {
    var tempName = $("#subButton");
    tempName.text("Subscribe");
    tempName.attr('class', 'btn btn-danger');
}

function blockedUserModal() {
    $('#blockedUserModal').modal();
}

function changeSubsCount(subsCount) {
    $('#subscribers').text("Subscribers: "+subsCount);
}

function refreshPage() {
    location.reload();
}
function BlockAll(IsOnHisPage) {
    console.log("blockAll");
    if (IsOnHisPage === "False") {
        $("#deleteOptionBlocked").hide();
        $("#editOptionBlocked").hide();
    }
    $("#subButton").attr("onclick", "blockedUserModal()");
    $("#blockOptionBlocked").hide();
    $("#unblockOptionBlocked").hide();
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
    $("#uploadPicForm").submit();
}
function urlPicture() {
    $("#uploadPicForm").attr("action", "/Users/ChangePictureUrl/");
    $("#uploadPicForm").submit();
}
function newPassword() {
    var channelName = $('#name').text();
    var form = $("#newPasswordForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            var page = 'Password has been successfully changed.';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#newPasswordModal").html(partialResult);
            } else {
                $('#newPasswordModalContainer').html(partialResult);
            }
        }
    });
}
