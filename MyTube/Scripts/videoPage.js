//var loggedInUser = "";
//var editCommentId = 0;
//var editVideoId = 0;
//var deleteCommentId = 0;
//var blocked = false;
//var videoBlocked = false;
//var currentVideo = null;
modalCloseReloadPage = false;
$(document).ready(function (e) {
    loadCommentsLikes();
    console.log("video.js file start");
    $('#blockVideoModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#unblockVideoModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#deleteVideoModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#editVideoModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshPage();
    });
    $('#deleteCommentModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshDeleteCommentModal();
    });
    $('#editCommentModal').on('hidden.bs.modal', function () {
        if (modalCloseReloadPage === true)
            refreshEditCommentModal();
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
            if (partialResult!==null) {
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
    var data = {
        "videoId": videoId,
        "newRating": true
    };
    $.ajax({
        url: "/VideoRatings/CreateVideoRating",
        type: 'post',
        data: data,
        dataType: "json",
        success: function (data) {
            console.log(data);
            changeButtonsActive(data.returnMessage);
            changeLikeDislikeCount(data.LikesCount, data.DislikesCount);
        }
    });
}
function videoDisliked(videoId) {

    var data = {
        "videoId": videoId,
        "newRating": false
    };
    $.ajax({
        url: "/VideoRatings/CreateVideoRating",
        type: 'post',
        data: data,
        dataType: "json",
        success: function (data) {
            console.log(data);
            changeButtonsActive(data.returnMessage);
            changeLikeDislikeCount(data.LikesCount, data.DislikesCount);
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
    var data = {
        "CommentText": commentText,
        "VideoID": videoId
    };
    $.ajax({
        url: "/Comments/CreateComment",
        type: 'post',
        data: data,
        success: function (data) {
            console.log("createNewCommentReturnData");
            if (data !== null) {
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
            console.log("return value");
            var page = 'Video has been successfully edited';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#editVideoModal").html(partialResult);
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
            var page = 'Video has been successfully deleted';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#deleteVideoModal").html(partialResult);
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
            var page = 'Video has been successfully blocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#blockVideoModal").html(partialResult);
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
            var page = 'Video has been successfully unblocked';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#unblockVideoModal").html(partialResult);
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
    data = {
        "text": newText
    };
    $.ajax({
        url: url,
        method: 'post',  // post
        data: data,
        success: function (partialResult) {
            console.log("return value");
            var page = 'Comment has been successfully edited';
            if (partialResult.includes(page)) {
                modalCloseReloadPage = true;
                $("#editCommentModal").html(partialResult);
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
            var page = 'Comment has been successfully deleted';
            if (partialResult.includes(page)) {
                $("#deleteCommentModal").html(partialResult);
                $("#comment-" + commentId).hide();
                modalCloseReloadPage = true;
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
function refreshDeleteCommentModal() {
    var newModal = '<div class="modal-dialog" role="document">' +
        '<div class="modal-content">' +
        '<div class="modal-header">' +
        '<h5 class="modal-title" id="exampleModalLabel">Are you sure?</h5>' +
        '</div>' +
        '<div class="modal-body">Select "Delete" below if you are want to delete this comment.</div>' +
        '<div class="modal-footer">' +
        '<button class="btn btn-secondary" type="button" data-dismiss="modal">Cancel</button>' +
        '<a href="@Url.Action(" DeleteComment /","Comments")" class="btn btn-danger" onclick="deleteComment(event)" id="DeleteCommentModalYesButton">Delete comment.</a>' +
        '</div>' +
        '</div>' +
        '</div>';
    $("#deleteCommentModal").html(newModal);
}
function refreshEditCommentModal() {
    var newModal = '<div class="modal-dialog">' +
        '<div class="modal-content col-md-offset-2 col-md-8 col-sm-offset-2 col-sm-8">' +
        '<div class="modal-header">' +
        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
        '<h3 class="modal-title">Edit comment</h3>' +
        '</div>' +
        '<div class="modal-header">' +
        '<div class="form-group">' +
        '<textarea style="overflow:auto;resize:vertical; max-width: 100%; width: 100%;" id="editCommentText" maxlength="500" rows="4"></textarea>' +
        '</div>' +
        '</div>' +
        '<div class="modal-footer">' +
        '<button class="btn btn-secondary" type="button" data-dismiss="modal">Cancel</button>' +
        '<a href="@Url.Action(" EditComment /","Comments")" class="btn btn-danger" onclick="commentEditSubmit(event)" id="EditCommentModalYesButton">Edit comment.</a>' +
        '</div>' +
        '</div>' +
        '</div>';
    $("#editCommentModal").html(newModal);
}
function loadCommentsLikes() {
    var url = window.location.href;
    console.log(url);
    var id = GetIdFromUrl(url);
    $.get('/CommentRatings/CommentRatingsForVideo/' + id, {}, function (data) {
        if (data === null) {
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
    var data = {
        "commentId": commId,
        "newRating": true
    };
    $.ajax({
        url: "/CommentRatings/CreateCommentRating",
        type: 'post',
        data: data,
        dataType: "json",
        success: function (data) {
            console.log(data);
            changeCommentButtonsActive(data.returnMessage, commId);
            changeCommentLikeDislikeCount(data.LikesCount, data.DislikesCount, commId);
        }
    });
}
function commentDisliked(commId) {
    var data = {
        "commentId": commId,
        "newRating": false
    };
    $.ajax({
        url: "/CommentRatings/CreateCommentRating",
        type: 'post',
        data: data,
        dataType: "json",
        success: function (data) {
            console.log(data);
            changeCommentButtonsActive(data.returnMessage, commId);
            changeCommentLikeDislikeCount(data.LikesCount, data.DislikesCount,commId);
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
//function setCommentRating(comment) {
//    console.log("setCommentRating, before if");
//    if (comment != null) {
//        var likes = '<span class="glyphicon glyphicon-thumbs-up"></span> ' + comment.likeNumber;
//        var dislikes = '<span class="glyphicon glyphicon-thumbs-down"></span> ' + comment.dislikeNumber;
//        $('#like' + comment.id + '').html(likes);
//        $('#dislike' + comment.id + '').html(dislikes);
//    }
