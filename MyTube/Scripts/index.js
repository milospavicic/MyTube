//$(document).ready(function (e) {
//    console.log("index.js");

//    loadVideos();
//    loadChannels();
//});
//function loadVideos() {
//    $.get('/Videos/IndexPageVideos', {}, function (data) {
//        console.log(data);
//        //	if(data.loggedInUser!=null){
//        //		if(data.loggedInUser.blocked == true){
//        //			$('#uploadVideo').attr("href", "#blockedModal");
//        //		}
//        var videosDiv = $('#recommendedDiv .row');
//        for (it in data) {
//            var video = data[it];
//            videosDiv.append(
//                '<div class="col-md-4">' +
//                '<a href="/Home/VideoPage/' + video.VideoID + '" target="_self">' +
//                '<img id="videoImage" src="' + video.ThumbnailUrl + '" alt="video" style="width:100%">' +
//                '<div class="caption">' +
//                '<p id="titleBar">' + video.VideoName + '</p>' +
//                '</div>' +
//                '<p id="videoInfo"><a href="/Home/ChannelPage/' + video.VideoOwner + '" id="channelName">' + video.VideoOwner + '</a><br>' + video.ViewsCount + ' views<br>Posted on: ' + video.DatePostedString + '</p>' +
//                '</a>' +
//                '</div>'
//            );
//        }



//    });
//}
//function loadChannels() {
//    $.get('/Users/IndexPageChannels', {}, function (data) {
//        var channelDiv = $('#popularChannels .rowOne');
//        for (it in data) {
//            var channel = data[it];
//            channelDiv.append('<div class="col-md-2 col-sm-4 col-xs-6">' +
//                '<a href="/Home/ChannelPage' + channel.UserName + '" target="_self">' +
//                '<img id="profileImage" src="' + channel.ProfilePictureUrl + '" alt="video" height="120" width="120">' +
//                '<div class="caption">' +
//                '<p id="profileName">' + channel.UserName + '</p>' +
//                '</div>' +
//                '<p >' + channel.SubscribersCount + '</p>' +
//                '</a>' +
//                '</div>'
//            );
//        }
//    });
//}
////function errorPage(){
////    var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
////    $('.mainDiv').empty();
////    $('.mainDiv').append(errorDiv);
////    $('.mainDiv').css("width", "95%");
////}
