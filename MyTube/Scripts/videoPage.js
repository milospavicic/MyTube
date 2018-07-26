var loggedInUser = "";
var editCommentId = 0;
var editVideoId = 0;
var deleteCommentId = 0;
var blocked = false;
var videoBlocked = false;
var currentVideo = null;
$(document).ready(function(e) {
	var videoId = window.location.search.slice(1).split('&')[0].split('=')[1];
	if(typeof videoId == 'undefined'){
		errorPage();
		return;
	}
	editVideoId = videoId.replace('#','').trim();
    console.log("start of videoPage.js : "+editVideoId);
	$.get('FollowUserServlet',{},function(dataOne){
		$.get('VideoServlet',{"videoId":editVideoId},function(data){
			if(data.video==null){
				console.log("nullerino");
				errorPage();
				return;
			}else{
				currentVideo = data.video;
			}
			if(data.loggedInUser!=null){
				loggedInUser=data.loggedInUser;
				if(data.loggedInUser.blocked == true){
					$('#uploadVideo').attr("href", "#blockedModal");
					blocked = data.loggedInUser.blocked;
				}
			}else{
				loggedInUser="false";
			}
			if(data.loggedInUser == null){
				if(data.video.visibility == "PRIVATE"){
					errorPage();
					return;
				}
				
				if(data.video.owner.blocked){
					errorPage();
					return;
				}
				if(data.video.blocked==true){
					errorPage();
					return;
				}
				if(data.video.ratingEnabled==false){
					$('#ldBtnGroup').hide();
				}
				if(data.video.commentsEnabled==false){
		            $('#myCommentButton').prop('disabled',true);
		            $('#hideShowBtn').prop('disabled',true);
				}
			}else{
				if(data.video.visibility == "PRIVATE"){
					if(data.video.owner.userName!=data.loggedInUser.userName){
						if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked==true){
							errorPage();
							return;
						}
					}
				}
				if(data.video.owner.blocked){
					console.log("userBlocked");
					if(data.video.owner.userName!=data.loggedInUser.userName){
						if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked==true){
							errorPage();
							return;
						}
					}
				}
				if(data.video.blocked==true){
					$('#blockOptionVideo').hide();
					videoBlocked = true;
					if(data.video.owner.userName!=data.loggedInUser.userName){
						if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked==true){
							errorPage();
							return;
						}else{
							$('#unblockOptionVideo').show();
						}
					}
				}else{
					$('#unblockOptionVideo').hide();
				}
				if(data.video.ratingEnabled==false){
					if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked==true)
						$('#ldBtnGroup').hide();
				}
				if(data.video.commentsEnabled==false){
					if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked==true){
						$('#myCommentButton').prop('disabled',true);
			            $('#hideShowBtn').prop('disabled',true);
					}
				}
				
				console.log(data.video.owner.userName+"  "+data.loggedInUser.userName)

				
				if(data.video.owner.userName==data.loggedInUser.userName){
					$('#editVideoOption').show();
					$('#deleteOptionVideo').show();
		            $('#myCommentButton').prop('disabled',false);
		            $('#hideShowBtn').prop('disabled',false);
		            $('#ldBtnGroup').show();
				}
				
			}			
			document.title = ''+data.video.videoName+' - MyTube';
			var videoName=$('#videoName');
			var vCounter=$('#vCounter');
			var desc=$('#desc');
			var likeButton=$('#likeButton');
			var dislikeButton=$('#dislikeButton');
			var channelsDiv = $('#recommendedVideos .row');
			var videoDiv = $('#videoDiv');

			videoDiv.append('<iframe id="videoPlayer" width="100%" height="430" src="'+data.video.videoUrl+"?autoplay=1"+'" frameborder="0" allowfullscreen></iframe>')
			
			
			if(data.video.blocked==true){
				videoName.html(data.video.videoName + " <i>( Blocked )</i>")
			}else{
				videoName.text(data.video.videoName);//works
			}
			
			vCounter.text("Views: "+data.video.views);
			likeButton.html('<span class="glyphicon glyphicon-thumbs-up"></span>'+' '+data.video.numberOfLikes);
			dislikeButton.html('<span class="glyphicon glyphicon-thumbs-down"></span>'+' '+data.video.numberOfDislikes);
			if(data.likedVideo!=null){
				if(data.likedVideo.likeOrDislike==true){
					setVideoLikeActive();
				}else{
					setVideoDislikeActive();
				}
			}
			if(data.video.owner.lol==true){
				var profilePic = "pictures/"+data.video.owner.profileUrl;
			}else{
				var profilePic = data.video.owner.profileUrl;
			}
			desc.append(
					'<a href="channelPage.html?channel='+data.video.owner.userName+'"><img src="'+profilePic+'" alt="Lights" style="width:70px;height: 70px"></a>'+
	                '<ul id="nameAndDate">'+
	                    '<li><a href="channelPage.html?channel='+data.video.owner.userName+'">'+data.video.owner.userName+'</a></li>'+
	                    '<li><p>Published on<br>'+data.video.datePosted+'</p></li>'+
	                '</ul>');
			
			var status = "false";
			for(temp in dataOne.userSubs){
				var stringOne = "";
				var stringTwo = "";
				stringOne=dataOne.userSubs[temp];
				stringTwo=data.video.owner.userName;
				if(stringOne==stringTwo){
					status = "true";
					break;
				}		
			}
			if(data.loggedInUser!=null && data.video.owner.userName!=data.loggedInUser.userName){
				console.log(status);
				if(status=="false"){
					desc.append(		
			                '<button id="btnFollow" type="button" class="btn btn-danger" onclick="follow(this,\''+data.video.owner.userName+'\')">Follow</button>');
				}else{
					desc.append(		
			                '<button id="btnFollow" type="button" class="btn btn-default" onclick="follow(this,\''+data.video.owner.userName+'\')">Unfollow</button>');
				}
			}
			desc.append('<p class="thumbnail" id="textDescription">'+data.video.description+'</p>');
			
			for(it in data.recommended){
				if(data.recommended[it].blocked==true){
					var videoName = data.recommended[it].videoName + ' - <i>Blocked</i>';
				}else{
					var videoName = data.recommended[it].videoName;
				}
				channelsDiv.append('<div class="col-md-2 col-sm-4 col-xs-6" id="test">'+
						'<a href="videoPage.html?id='+data.recommended[it].id+'" target="_self">'+
						'<img id="imgFormat" src="'+data.recommended[it].pictureUrl+'" alt="video" style="width:100%">'+
						'<div class="caption"><p id="titleBar">'+videoName+'</p></div>'+
						'<p id="stats">'+data.recommended[it].owner.userName+'<br>'+data.recommended[it].views+'<br>'+data.recommended[it].datePosted+'</p></a></div>');

			}
			loadComments(editVideoId,"id","DESC"); 
		});
	});
     
    
    $('#likeButton').click(function() {
    	console.log("likeButtonClick");
		if(loggedInUser=="false"){
    		$("#login-modal").modal('toggle');
    		return;
    	}
		if(blocked == true){
			$("#blockedModal").modal('toggle');
			return;
		}
		if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
			$("#blockedVideoModal").modal('toggle');
			return;
		}
    	$.get('LikeVideoServlet',{"videoId":videoId,"status":"liked"},function(data){
    		setVideoLikeDislikeNumbers(data.video);
    		if(data.status=="neutral"){
    			setVideoButtonsDefault();
    		}else if(data.status=="like"){
    			setVideoLikeActive();
    		}else if(data.status=="failed"){
    			console.log("failedLike");
    			$('#failModal').modal();
    		}else{
    			console.log("failedLikeWTF");
    			setVideoDislikeActive();
    		}
    	});
    });
    $('#dislikeButton').click(function() {
		if(loggedInUser=="false"){
			$("#login-modal").modal('toggle');
			return;
		}
		if(blocked == true){
			$("#blockedModal").modal('toggle');
			return;
		}
		if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
			$("#blockedVideoModal").modal('toggle');
			return;
		}
    	$.get('LikeVideoServlet',{"videoId":videoId,"status":"dislike"},function(data){
    		setVideoLikeDislikeNumbers(data.video);
    		if(data.status=="neutral"){
    			setVideoButtonsDefault();
    		}else if(data.status=="like"){
    			setVideoLikeActive();
    		}else if(data.status=="failed"){
    			console.log("failedLike1");
    			$('#failModal').modal();
    		}else{
    			console.log("failedLikeWTF1");
    			setVideoDislikeActive();
    		}
    	});
    });
    //NEW COMMENT
    $('#myFormButton').click(function(){
    	if(loggedInUser=="false"){
    		$("#login-modal").modal('toggle');
    		return;
    	}
    	if(blocked == true){
    		$("#blockedModal").modal('toggle');
    		return;
    	}
    	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
    		$("#blockedVideoModal").modal('toggle');
    		return;
    	}
    	var commentText = $('#myCommentText').val().trim();
    	if(commentText=="") return;
    	$.post('CommentsServlet',{"status":"new","videoId":videoId,"commentText":commentText},function(data){
    		if(data.endStatus=="newSuccess"){
        		$('#myCommentText').val("");
        		$('#myComment').collapse("hide");
        		refreshComments();
    		}else{
    			$('#failModal').modal();
    		}
    	});
    });
});
function errorPage(){
    var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
    $('.mainDiv').empty();
    $('.mainDiv').append(errorDiv);
    $('.mainDiv').css("width", "95%");
}
function follow(btn,userName){
	if(loggedInUser=="false"){
		$("#login-modal").modal('toggle');
		return;
	}
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
    var tempName = $("#"+btn.id).text();
    if(tempName == "Follow"){
        $.post('FollowUserServlet',{"userName":userName,"status":"follow"},function(data){
        	if(data.endStatus=="Success"){
                $("#"+btn.id).text("Unfollow");
                $("#"+btn.id).attr('class', 'btn btn-default');
        	}else{
        		$('#failModal').modal();
        	}
        });
    }else{
        $.post('FollowUserServlet',{"userName":userName,"status":"unfollow"},function(data){
        	if(data.endStatus=="Success"){
                $("#"+btn.id).text("Follow");
                $("#"+btn.id).attr('class', 'btn btn-danger');
        	}else{
        		$('#failModal').modal();
        	}
        });
    }
}

function loadComments(videoId,column,ascDsc){
	console.log("loadingComments");
    var commentsDiv = $('.commentRow');
    commentsDiv.empty();
    $.get('CommentsServlet',{"videoId":videoId,"columnName":column,"ascDes":ascDsc},function(data){
    	for(it in data.comments){
    		var rating = 0;
    		rating = data.comments[it].likeNumber-data.comments[it].dislikeNumber;
    		var newComment = "";
    		if(data.comments[it].user.lol==true){
				var profilePic = "pictures/"+data.comments[it].user.profileUrl;
			}else{
				var profilePic = data.comments[it].user.profileUrl;
			}
    		//newComment+='<a href="channelPage.html?channel='+data.comments[it].user.userName+'"><img src="'+profilePic+'" alt="Lights" style="width:20px;height: 20px"></a>';
    		newComment+='<div class="col-md-12 col-sm-12" id="div'+data.comments[it].id+'">'+
    				'<div class="thumbnail" id="comment">';
    		newComment+='<a href="channelPage.html?channel='+data.comments[it].user.userName+'" id="commentPic"><img src="'+profilePic+'" alt="Lights" style="width:20px;height: 20px"></a>';	
			newComment+='<a href="channelPage.html?channel='+data.comments[it].user.userName+'" id="commentOwner">'+data.comments[it].user.userName+'</a>'+
    				'<p id="commentDate">'+data.comments[it].datePosted+'</p>'+
    				'<div class="commentText"><p id="commentEdit'+data.comments[it].id+'">'+data.comments[it].text+'</p></div>'+
    				'<div class="btn-group" id="commLDBtnGroup">'+
    				'<button type="button" class="btn btn-default" id="like'+data.comments[it].id+'" onclick="commLike('+data.comments[it].id+')"><span class="glyphicon glyphicon-thumbs-up"></span> '+data.comments[it].likeNumber+'</button>'+
    				'<button type="button" class="btn btn-default" id="dislike'+data.comments[it].id+'" onclick="commDislike('+data.comments[it].id+')"><span class="glyphicon glyphicon-thumbs-down"></span> '+data.comments[it].dislikeNumber+'</button>';
    		if(loggedInUser!="false" && loggedInUser.blocked!=true){
    			if(loggedInUser.userType == "ADMIN" || data.comments[it].user.userName==loggedInUser.userName){
        			newComment+='<button type="button" class="btn btn-default" id="edit'+data.comments[it].id+'" onclick="editComment('+data.comments[it].id+',\''+data.comments[it].text+'\')"><span class="glyphicon glyphicon-edit"></span></button>'+
    				'<button type="button" class="btn btn-default" id="delete'+data.comments[it].id+'" onclick="deleteComment('+data.comments[it].id+')"><span class="glyphicon glyphicon-trash"></span></button>'+		
    				'</div></div></div>';
    			}
    		}else{
    			newComment+='</div></div></div>';
    		}

    		commentsDiv.append(newComment);
    		
    		for(temp in data.likes){
    			if(data.comments[it].id==data.likes[temp].comment.id){
    				if(data.likes[temp].likeOrDislike==true)
    					setCommentLikeActive(data.comments[it].id);
    				else
    					setCommentDislikeActive(data.comments[it].id);
    			}
    		}
    	}
    });
}
function editComment(commentId,commentText){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$('#editCommentText').val(commentText);
	editCommentId = commentId;
	$('#editComment-modal').modal('show');
}
function editCommentButton(){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	var commentText = $('#editCommentText').val().trim();
	$.post('CommentsServlet',{"status":"edit","commentText":commentText,"commentId":editCommentId},function(data){
		if(data.endStatus=="editSuccess"){
			$('#editComment-modal').modal('hide');
			$('#commentEdit'+editCommentId+'').text(commentText);
			$('#commEdit').modal();
		}else{
			$('#failModal').modal();
		}
	});
}

function refreshComments() {
	var videoId = window.location.search.slice(1).split('&')[0].split('=')[1];
	var option = commSelect.options[commSelect.selectedIndex].value;
	if(option=="MP"){
		loadComments(videoId,"(likeNumber-dislikeNumber)","DESC");
	}
	else if(option=="LP"){
		loadComments(videoId,"(likeNumber-dislikeNumber)","ASC");
	}else{
	    loadComments(videoId,"id",option);
	}
}
function commLike(commId){
	if(loggedInUser=="false"){
		$("#login-modal").modal('toggle');
		return;
	}
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$.get('LikeCommentServlet',{"commentId":commId,"status":"liked"},function(data){
		if(data.status=="failed"){
			$('#failModal').modal();
		}else if(data.status=="neutral"){
			setCommentButtonsDefault(data.comment.id);
			setCommentRating(data.comment);
		}else if(data.status=="like"){
			setCommentLikeActive(data.comment.id);
			setCommentRating(data.comment);
		}else{
			setCommentDislikeActive(data.comment.id);
			setCommentRating(data.comment);
		}
	});
}
function commDislike(commId){
	if(loggedInUser=="false"){
		$("#login-modal").modal('toggle');
		return;
	}
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$.get('LikeCommentServlet',{"commentId":commId,"status":"disliked"},function(data){
		setCommentRating(data.comment);
		if(data.status=="failed"){
			$('#failModal').modal();
		}else if(data.status=="neutral"){
			setCommentButtonsDefault(data.comment.id);
		}else if(data.status=="like"){
			setCommentLikeActive(data.comment.id);
		}else{
			setCommentDislikeActive(data.comment.id);
		}
	});
	
}
function fillEditVideo(){
	console.log("fillEditVideo")
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	var video = currentVideo;
	$('#editDescription').val(video.description);
	if(video.pictureUrl!="pictures/noimage.jpg"){
		$('#editPicUrl').val(video.pictureUrl);
	}
	if(video.visibility==("PUBLIC")){
		console.log("its public");
		$('#selectOne option[value=1]').attr('selected',true);
	}
	if(video.visibility==("PRIVATE")){
		console.log("its private");
		$('#selectOne option[value=2]').attr('selected',true);
	}
	if(video.visibility==("UNLISTED")){
		console.log("its unlisted");
		$('#selectOne option[value=3]').attr('selected',true);
	}
	
	if(video.commentsEnabled==true){
		$('#selectTwo option[value=3]').attr('selected',true);
	}else{
		$('#selectTwo option[value=2]').attr('selected',true);
	}
	
	if(video.ratingEnabled==true){
		$('#selectThree option[value=3]').attr('selected',true);
	}else{
		$('#selectThree option[value=2]').attr('selected',true);
	}
	$("#editVideo-modal").modal('toggle');
}
function saveEditVideo(){
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	var desc = $('#editDescription').val().trim();
	var picurl = $('#editPicUrl').val().trim();
	var visib = $('#selectOne').val();
	var comm = $('#selectTwo').val();
	var rating = $('#selectThree').val();
	console.log(editVideoId);
//	$.post('VideoServlet',{"status":"edit","videoId":editVideoId,"desc":desc,"picurl":picurl,"visib":visib,"comm":comm,"rating":rating},function(data){location.reload();});
	console.log(picurl);
	$.post('VideoServlet',{"status":"edit","videoId":editVideoId,"desc":desc,"picurl":picurl,"visib":visib,"comm":comm,"rating":rating},function(data){
		if(data.endStatus =="editSuccess"){
			$('#editVideo-modal').hide()
			$('#succesEdit').modal()
		}else{
			$('#failModal').modal();
		}
	});
}
function setVideoButtonsDefault(){
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();
    
    $('#likeButton').addClass("btn btn-default");
    $('#dislikeButton').addClass("btn btn-default");
}
function setVideoLikeActive(){
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();
    
    $('#likeButton').addClass("btn btn-danger");
    $('#dislikeButton').addClass("btn btn-default");
}
function setVideoDislikeActive(){
	console.log("dislikeActive()");
    $('#likeButton').removeClass();
    $('#dislikeButton').removeClass();
    
    $('#likeButton').addClass("btn btn-default");
    $('#dislikeButton').addClass("btn btn-danger");
}
function setVideoLikeDislikeNumbers(video){
    var likes ='<span class="glyphicon glyphicon-thumbs-up"></span> '+video.numberOfLikes;
    var dislikes ='<span class="glyphicon glyphicon-thumbs-down"></span> '+video.numberOfDislikes;
    $('#likeButton').html(likes);
    $('#dislikeButton').html(dislikes);
}
function setCommentButtonsDefault(commId){
	$('#like'+commId+'').removeClass();
	$('#dislike'+commId+'').removeClass();
	
	$('#like'+commId+'').addClass("btn btn-default");
	$('#dislike'+commId+'').addClass("btn btn-default");
}
function setCommentLikeActive(commId){
	$('#like'+commId+'').removeClass();
	$('#dislike'+commId+'').removeClass();
	
	$('#like'+commId+'').addClass("btn btn-danger");
	$('#dislike'+commId+'').addClass("btn btn-default");
}
function setCommentDislikeActive(commId){
	$('#like'+commId+'').removeClass();
	$('#dislike'+commId+'').removeClass();
	
	$('#like'+commId+'').addClass("btn btn-default");
	$('#dislike'+commId+'').addClass("btn btn-danger");
}
function setCommentRating(comment){
	console.log("setCommentRating, before if");
	if(comment!=null){
	    var likes ='<span class="glyphicon glyphicon-thumbs-up"></span> '+comment.likeNumber;
	    var dislikes ='<span class="glyphicon glyphicon-thumbs-down"></span> '+comment.dislikeNumber;
		$('#like'+comment.id+'').html(likes);
		$('#dislike'+comment.id+'').html(dislikes);
	}
	
}
function deleteComment(id){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$('#deleteCommentModal').modal();
	deleteCommentId = id;
}
function deleteCommentById(){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$.post('CommentsServlet',{"status":"delete","commentId":deleteCommentId},function(data){
		if(data.endStatus=="deleteSuccess"){
			$('#deleteCommentModal').modal('hide');
			$('#div'+deleteCommentId+'').hide();
			console.log('hiden');
			$('#commentDel').modal();
    		refreshComments();
		}else{
			$('#failModal').modal();
		}
	});
}
function blockVideo(){
	$.post('VideoServlet',{"status":"block","videoId":editVideoId},function(data){
		if(data.endStatus =="blockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function unblockVideo(){
	$.post('VideoServlet',{"status":"unblock","videoId":editVideoId},function(data){
		if(data.endStatus =="unblockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function deleteVideo(){
	$.post('VideoServlet',{"status":"delete","videoId":editVideoId},function(data){
		if(data.endStatus =="deleteSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function deleteVideoModal(){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	if(videoBlocked == true && (loggedInUser!=false && loggedInUser.userType!="ADMIN")){
		$("#blockedVideoModal").modal('toggle');
		return;
	}
	$("#deleteModal").modal('toggle');
}