modalCloseReloadPage = false;
returnToIndexPage = false;
$(document).ready(function (e) {
    $('#upload').hide();
    loadCommentsLikes();
    console.log("video.js file start");
    $('#messageModal').on('hidden.bs.modal', function () {
        if (returnToIndexPage === true) {
            indexPage();
        }
        else if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#changeThumbnailModal').on('shown.bs.modal', function (e) {
        $('#linkOrLocalPicture').prop('checked', false);
        $('#link').show();
        $('#upload').hide();
    });
    $(window).on('popstate', function (event) {
        alert("pop");
    });
});
function sortComments() {
    event.preventDefault();  // prevent standard form submission
    var form = $("#SortCommetsForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            console.log("return value");
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            else{
                $("#commentsDiv").html(partialResult);
                loadCommentsLikes();
                $('#hideShowBtn').click();
            }}
    });
}
function subUser() {
    var channelName = $('#name').text();
    $.ajax({
        url: "/Users/Subscribe/" + channelName,
        type: 'post',
        dataType: "json",
        success: function (data) {
            if (data === null || data === '') {
                errorPage();
                return;
            }
            subOrUnsubChangeButton(data);
        }
    });
}
function subOrUnsubChangeButton(data) {
    if (data.subStatus === false) {
        sub();
    }
    else if (data.subStatus === true) {
        unsub();
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
function videoLiked(videoId) {
    var newData = {
        "videoId": videoId,
        "newRating": true
    };
    $.ajax({
        url: "/VideoRatings/CreateVideoRating",
        type: 'post',
        data: newData,
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data === null || data === '') {
                errorPage();
                return;
            }
            else{
                changeButtonsActive(data.returnMessage);
                changeLikeDislikeCount(data.LikesCount, data.DislikesCount);
            }
        }
    });
}
function videoDisliked(videoId) {

    var newData = {
        "videoId": videoId,
        "newRating": false
    };
    $.ajax({
        url: "/VideoRatings/CreateVideoRating",
        type: 'post',
        data: newData,
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data === null || data === '') {
                errorPage();
                return;
            }
            else{
                changeButtonsActive(data.returnMessage);
                changeLikeDislikeCount(data.LikesCount, data.DislikesCount);
            }
        }
    });
}
function changeButtonsActive(returnMessage) {
    switch (returnMessage) {
        case "like":
            likeButtonOn();
            break;
        case "dislike":
            dislikeButtonOn();
            break;
        case "neutral":
            bothButtonsOff();
            break;
    }
}
function likeButtonOn() {
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();

    $('#likeButton').addClass("btn btn-danger");
    $('#dislikeButton').addClass("btn btn-default");
}
function dislikeButtonOn() {
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();

    $('#likeButton').addClass("btn btn-default");
    $('#dislikeButton').addClass("btn btn-danger");
}

function bothButtonsOff() {
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();

    $('#likeButton').addClass("btn btn-default");
    $('#dislikeButton').addClass("btn btn-default");
}
function changeLikeDislikeCount(likeCount, dislikeCount) {
    console.log("changeCount()");
    var likes = '<span class="glyphicon glyphicon-thumbs-up"></span> ' + likeCount;
    var dislikes = '<span class="glyphicon glyphicon-thumbs-down"></span> ' + dislikeCount;
    $('#likeButton').html(likes);
    $('#dislikeButton').html(dislikes);
}
function createNewComment(videoId) {
    var commentText = $('#myCommentText').val();
    console.log(commentText);
    var newData = {
        "CommentText": commentText,
        "VideoID": videoId
    };
    $.ajax({
        url: "/Comments/CreateComment",
        type: 'post',
        data: newData,
        success: function (data) {
            console.log("createNewCommentReturnData");
            if (data === null || data === '') {
                errorPage();
                return;
            }
            else {
                $(data).insertAfter('#commHeader');
                clearAndHideNewCommentSection();
            }

        }
    });
}
function clearAndHideNewCommentSection() {
    $('#myCommentText').val("");
    $('#myComment').collapse("hide");
}
function fillEditVideo(event, id) {
    event.preventDefault();
    console.log(id);
}
function videoEditSubmit(event) {
    event.preventDefault();  // prevent standard form submission
    var form = $("#videoEditForm");
    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),  // post
        data: form.serialize(),
        success: function (partialResult) {
            console.log(partialResult);
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            console.log("return value");
            var page = 'Video has been successfully edited';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                messageModal(partialResult);
                $("#editVideoModal").modal('hide');
            } else {
                $("#videoEditFormContainer").html(partialResult);
            }

        }
    });
}

function deleteVideo(event) {
    event.preventDefault();
    url = $("#DeleteModalYesButton").attr("href");
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            var page = 'Video has been successfully deleted';
            if (partialResult.includes(page)) {
                returnToIndexPage = true;
                messageModal(partialResult);
                $("#deleteVideoModal").modal('hide');
            }
        }
    });
}

function blockVideo(event) {
    event.preventDefault();
    url = $("#BlockModalYesButton").attr("href");
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            var page = 'Video has been successfully blocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                messageModal(partialResult);
                $("#blockVideoModal").modal('hide');
            }
        }
    });
}

function unblockVideo(event) {
    event.preventDefault();
    url = $("#UnblockModalYesButton").attr("href");
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            var page = 'Video has been successfully unblocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                messageModal(partialResult);
                $("#unblockVideoModal").modal('hide');
            }
        }
    });
}

function editCommentModal(commentId) {
    fillEditCommentModal(commentId);
    resetModalReloadPageComparator();
    $("#EditCommentModalYesButton").attr("href", "/Comments/EditComment/" + commentId);
    $('#editCommentModal').modal();

}
function fillEditCommentModal(commentId) {
    var text = $('#text-' + commentId).text();
    $('#editCommentText').text(text);
}
function deleteCommentModal(commentId) {
    resetModalReloadPageComparator();
    $("#DeleteCommentModalYesButton").attr("href", "/Comments/DeleteComment/" + commentId);
    $('#deleteCommentModal').modal();
}


function commentEditSubmit(event) {
    event.preventDefault();  // prevent standard form submission
    var newText = $("#editCommentText").val();
    var url = $("#EditCommentModalYesButton").attr("href");
    var commentId = GetIdFromUrl(url);
    newData = {
        "text": newText
    };
    $.ajax({
        url: url,
        method: 'post',  // post
        data: newData,
        success: function (partialResult) {
            console.log("return value");
            var page = 'Comment has been successfully edited';
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#editCommentModal").modal('hide');
                loadEditedComment(commentId, newText);
            } else {

                $("#videoEditFormContainer").html(partialResult);
            }

        }
    });
}
function GetIdFromUrl(url) {
    var id = url.substring(url.lastIndexOf('/') + 1);
    return id;
}
function loadEditedComment(commentId, newText) {
    $('#text-' + commentId).text(newText);
}
function deleteComment(event) {
    event.preventDefault();
    url = $("#DeleteCommentModalYesButton").attr("href");
    var commentId = GetIdFromUrl(url);
    console.log(url);
    $.ajax({
        url: url,
        type: 'post',
        success: function (partialResult) {
            if (partialResult === null || partialResult === '') {
                errorPage();
                return;
            }
            var page = 'Comment has been successfully deleted';
            if (partialResult.includes(page)) {
                messageModal(partialResult);
                $("#deleteCommentModal").modal('hide');
                $("#comment-" + commentId).hide();
            }
        }
    });
}

function resetModalReloadPageComparator() {
    modalCloseReloadPage = false;
}
function refreshPage() {
    location.reload();
}
function loadCommentsLikes() {
    var url = window.location.href;
    console.log(url);
    var id = GetIdFromUrl(url);
    $.get('/CommentRatings/CommentRatingsForVideo/' + id, {}, function (data) {
        if (data === null || data === '') {
            return;
        }
        for (it in data) {
            var like = data[it];
            console.log(like);
            if (like.IsLike) {
                console.log("commentLike" + like.CommentId);
                setCommentLikeActive(like.CommentId);
            } else {
                console.log("commentDislike" + like.CommentId);
                setCommentDislikeActive(like.CommentId);
            }
        }

    });
}

function commentLiked(commId) {
    var newData = {
        "commentId": commId,
        "newRating": true
    };
    $.ajax({
        url: "/CommentRatings/CreateCommentRating",
        type: 'post',
        data: newData,
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data === null || data === '') {
                errorPage();
                return;
            }
            else{
                changeCommentButtonsActive(data.returnMessage, commId);
                changeCommentLikeDislikeCount(data.LikesCount, data.DislikesCount, commId);
            }
        }
    });
}
function commentDisliked(commId) {
    var newData = {
        "commentId": commId,
        "newRating": false
    };
    $.ajax({
        url: "/CommentRatings/CreateCommentRating",
        type: 'post',
        data: newData,
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data === null || data === '') {
                errorPage();
                return;
            }
            else{
                changeCommentButtonsActive(data.returnMessage, commId);
                changeCommentLikeDislikeCount(data.LikesCount, data.DislikesCount, commId);
            }
        }
    });
}
function changeCommentButtonsActive(message, commId) {
    switch (message) {
        case "like":
            setCommentLikeActive(commId);
            break;
        case "dislike":
            setCommentDislikeActive(commId);
            break;
        case "neutral":
            setCommentButtonsDefault(commId);
            break;
    }
}
function changeCommentLikeDislikeCount(likeCount, dislikeCount, commId) {
    console.log("changeCommentLikeDislikeCount(" + likeCount + " " + dislikeCount + " " + commId);
    var likes = '<span class="glyphicon glyphicon-thumbs-up"></span> ' + likeCount;
    var dislikes = '<span class="glyphicon glyphicon-thumbs-down"></span> ' + dislikeCount;
    $('#likeBtn-' + commId + '').html(likes);
    $('#dislikeBtn-' + commId + '').html(dislikes);
}
function setCommentButtonsDefault(commId) {
    $('#likeBtn-' + commId + '').removeClass();
    $('#dislikeBtn-' + commId + '').removeClass();

    $('#likeBtn-' + commId + '').addClass("btn btn-default");
    $('#dislikeBtn-' + commId + '').addClass("btn btn-default");
}
function setCommentLikeActive(commId) {
    $('#likeBtn-' + commId + '').removeClass();
    $('#dislikeBtn-' + commId + '').removeClass();

    $('#likeBtn-' + commId + '').addClass("btn btn-danger");
    $('#dislikeBtn-' + commId + '').addClass("btn btn-default");
}
function setCommentDislikeActive(commId) {
    $('#likeBtn-' + commId + '').removeClass();
    $('#dislikeBtn-' + commId + '').removeClass();

    $('#likeBtn-' + commId + '').addClass("btn btn-default");
    $('#dislikeBtn-' + commId + '').addClass("btn btn-danger");
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
    var pic = $('#ThumbnailUrl').val();
    if (pic === null || pic === '') {
        return;
    }
    $("#uploadPicForm").attr("action", "/Videos/ChangePictureUrl/");
    $("#uploadPicForm").submit();
}
function messageModal(partial) {
    $("#messageModal").html(partial);
    $("#messageModal").modal();
}
function NotLoggedIn() {
    console.log("NotLoggedIn");
    $('#likeButton').attr("onclick", "LoginModal()");
    $('#dislikeButton').attr("onclick", "LoginModal()");
    $('#myFormButton').attr("onclick", "LoginModal()");
    $('.commentLikeButtons').attr("onclick", "LoginModal()");
    $('.commentDisikeButtons').attr("onclick", "LoginModal()");
}
function BlockAll() {
    console.log("blockAll");
    $("#subButton").attr("onclick", "blockedUserModal()");
    $('#likeButton').attr("onclick", "blockedUserModal()");
    $('#dislikeButton').attr("onclick", "blockedUserModal()");
    $('#myFormButton').attr("onclick", "blockedUserModal()");

    $('#changeThubnailOption').hide();
    $('#editVideoOption').hide();
    $('#blockOptionVideo').hide();
    $('#unblockOptionVideo').hide();
    $('#deleteOptionVideo').hide();

    $('.commentEditButtons').hide();
    $('.commentDeleteButtons').hide();
    $('.commentLikeButtons').attr("onclick", "blockedUserModal()");
    $('.commentDisikeButtons').attr("onclick", "blockedUserModal()");
}
function blockedUserModal() {
    $('#blockedUserModal').modal();
}
function LoginModal() {
    $('#login-modal').modal();
}
function indexPage() {
    console.log("indexPage");
    window.location.href = "/Home/Index";
}
function errorPage() {
    console.log("errorPage");
    window.location.href = "/Home/Error";
}
