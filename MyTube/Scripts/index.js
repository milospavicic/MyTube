$(document).ready(function(e) {
	console.log("index.js");
	var videosDiv = $('#recommendedDiv .row');
	$.get('GetVideosServlet',{},function(data){
		console.log("GetVideosServlet");
		var counter = 1;
		for(it in data.videos){
			if(counter>6) break;
			if(data.videos[it].blocked==true){
				var videoName = data.videos[it].videoName + ' - <i>Blocked</i>';
			}else{
				var videoName = data.videos[it].videoName;
			}
			if(data.loggedInUser!=null){
				if(data.loggedInUser.blocked == true){
					$('#uploadVideo').attr("href", "#blockedModal");
				}
			}
			videosDiv.append(
					'<div class="col-md-4">'+
					'<a href="videoPage.html?id='+data.videos[it].id+'" target="_self">'+
					'<img id="videoImage" src="'+data.videos[it].pictureUrl+'" alt="video" style="width:100%">'+
					'<div class="caption">'+
						'<p id="titleBar">'+videoName+'</p>'+
					'</div>'+
					'<p id="videoInfo"><a href="channelPage.html?channel='+data.videos[it].owner.userName+'" id="channelName">'+data.videos[it].owner.userName+'</a><br>'+data.videos[it].views+' views<br>Posted on: '+data.videos[it].datePosted+'</p>'+
					'</a>'+
					'</div>'
					);
			counter++;
		}

		var channelDiv = $('#popularChannels .rowOne');
		for(it in data.topSixChannels){
			if(data.topSixChannels[it].lol==true){
				var profilePic = "pictures/"+data.topSixChannels[it].profileUrl;
			}else{
				var profilePic = data.topSixChannels[it].profileUrl;
			}
			
			channelDiv.append('<div class="col-md-2 col-sm-4 col-xs-6">'+
					'<a href="channelPage.html?channelName='+data.topSixChannels[it].userName+'" target="_self">'+
					'<img id="profileImage" src="'+profilePic+'" alt="video" height="120" width="120">'+
					'<div class="caption">'+
					'<p id="profileName">'+data.topSixChannels[it].userName+'</p>'+
					'</div>'+
					'<p >'+data.topSixChannels[it].subsNumber+'</p>'+
					'</a>'+
					'</div>'
					);
		}

	});
});
function errorPage(){
    var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
    $('.mainDiv').empty();
    $('.mainDiv').append(errorDiv);
    $('.mainDiv').css("width", "95%");
}
