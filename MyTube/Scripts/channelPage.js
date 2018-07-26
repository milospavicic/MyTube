var channelName = "";
var channelPage = null;
var loggedInUser = null;
var subsNumber = 0;
var blocked = false;
$('document').ready(function(e){
	var tempName = window.location.search.slice(1).split('&')[0].split('=')[1];
	if (typeof tempName == 'undefined'){
		errorPage();
		return;
	}
	channelName = tempName;
	document.title = ''+channelName+' - MyTube';
	showRecentVideos(1);
	
	$('.changeActive').click(function(){
		$('.active').removeClass('active');
		$(this).addClass('active');
	});
	$('#userVideos').click(function(){
		showOrder();
		showRecentVideos(1);
	});
	$('#userLiked').click(function(){
		hideOrder();
		showLikes();
	});
	$('#userInfo').click(function(){
		hideOrder();
		showInfo();
	});
	$('#userFollow').click(function(){
		hideOrder();
		showFollowing();
	});
	
});

function saveEditUser(){
	var firstName = $('#editFirstName').val().trim();
	var lastName = $('#editLastName').val().trim();
	var password = $('#editPassword').val().trim();
	var email = $('#editEmail').val().trim();
	var channelDescription = $('#editDescription').val().trim();
	
	var lol = false;
	if($('#lol').is(':checked')){
		lol =true;
		if($("#editUploadPic")[0].files.length == 0){
			console.log("Keep old pic");
			if(channelPage.lol==false){
				$('#uploadPicModal').modal();
				return;
			}else{
				var profileUrl=channelPage.profileUrl;
			}
			var newFile = false;
		}else{
			var profileUrl="";
			var newFile = true;
		}
	}else{
		var profileUrl = $('#editPicUrl').val().trim();
		var newFile = false;
	}
	
	if(loggedInUser!=null){
		if(loggedInUser.userType=="ADMIN"){
			var userType = $('#selectType').val();
			$.post('ChannelServlet',{"status":"edit","channelName":channelPage.userName,"firstName":firstName,"lastName":lastName,"password":password,"email":email,"profileUrl":profileUrl,"channelDescription":channelDescription,"userType":userType,"lol":lol,"newFile":newFile},function(data){
				if(data.endStatus=="editSuccess"){
					if(newFile==true){
						editUploadPicture();
					}
					$('#editChannel-modal').hide();
					$('#succesEdit').modal();
				}else{
					$('#failModal').modal();
				}
				
			});
		}else{
			$.post('ChannelServlet',{"status":"edit","channelName":channelPage.userName,"firstName":firstName,"lastName":lastName,"password":password,"email":email,"profileUrl":profileUrl,"channelDescription":channelDescription,"lol":lol,"newFile":newFile},function(data){
				if(data.endStatus=="editSuccess"){
					if(newFile==true){
						editUploadPicture();
					}
					$('#editChannel-modal').hide();
					$('#succesEdit').modal();
				}else{
					$('#failModal').modal();
				}
			});
		}
	}	
}
function deleteUser(){
	$.post('ChannelServlet',{"status":"delete","channelName":channelName},function(data){
		if(data.endStatus=="deleteSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function blockUser(){
	$.post('ChannelServlet',{"status":"block","channelName":channelName},function(data){		
		if(data.endStatus=="blockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function unblockUser(){
	$.post('ChannelServlet',{"status":"unblock","channelName":channelName},function(data){		
		if(data.endStatus=="unblockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function follow(){
	console.log("followUser")
	if(loggedInUser==null){
		$("#login-modal").modal('toggle');
		return;
	}
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	var tempName = $("#followButton");
	if(tempName.text() == "Follow"){
		$.post('FollowUserServlet',{"userName":channelName,"status":"follow"},function(data){
			if(data.endStatus=="Success"){
				tempName.text("Unfollow");
				tempName.attr('class', 'btn btn-default');
				subsNumber +=1;
				$('#followers').empty();
				$('#followers').append('<p>Followers: '+subsNumber+'</p>');
			}else{
				$('#failModal').modal();
			}
			
		});
	}else{
		$.post('FollowUserServlet',{"userName":channelName,"status":"unfollow"},function(data){
			if(data.endStatus=="Success"){
				tempName.text("Follow");
				tempName.attr('class', 'btn btn-danger');
				subsNumber -=1;
				$('#followers').empty();
				$('#followers').append('<p>Followers: '+subsNumber+'</p>');
			}else{
				$('#failModal').modal();
			}
		});
	}
}

function errorPage(){
	var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
	$('.mainDiv').empty();
	$('.mainDiv').append(errorDiv);
	$('.mainDiv').css("width", "95%");
}


function showRecentVideos(n){
	$.get('ChannelServlet',{"channelName":channelName,"status":"homepage","orderBy":n},function(data){
		if(data.channel==null){
			errorPage();
			return;
		}
		if(data.loggedInUser!=null){
			if(data.loggedInUser.blocked == true){
				$('#uploadVideo').attr("href", "#blockedModal");
				$('#deleteOptionBlocked').attr("href", "#blockedModal");
				$('#blockOptionBlocked').attr("href", "#blockedModal");
				$('#unblockOptionBlocked').attr("href", "#blockedModal");
				$('#editOptionBlocked').attr("href", "#blockedModal");
				blocked = data.loggedInUser.blocked;
			}
			if(data.channel.userName == data.loggedInUser.userName){
				$('#followButton').hide();
			}
		}
		
		
		initEdit(data.channel,data.loggedInUser,data.followThisChannel);

		channelPage = data.channel;
		subsNumber = data.channel.subsNumber;
		
		var videos = data.videos;
		var myDiv = $('#content');
		myDiv.empty();
			for(it in videos){
				if(data.videos[it].blocked==true){
					var videoName = data.videos[it].videoName + ' - <i>Blocked</i>';
				}else{
					var videoName = data.videos[it].videoName;
				}
				myDiv.append(
						'<div class="col-md-4">'+
						'<a href="videoPage.html?id='+videos[it].id+'" target="_self">'+
						'<img id="videoImage" src="'+videos[it].pictureUrl+'" alt="video" style="width:100%">'+
						'<div class="caption">'+
							'<p id="titleBar">'+videoName+'</p>'+
						'</div>'+
						'<p id="videoInfo"><a href="channelPage.html?channel='+videos[it].owner.userName+'" id="channelName">'+videos[it].owner.userName+'</a><br>'+videos[it].views+' views<br>Posted on: '+videos[it].datePosted+'</p>'+
						'</a>'+
						'</div>');
			}
	});
}
function showFollowing(){
	$.get('ChannelServlet',{"channelName":channelName,"status":"following",},function(data){
		var following = data.following;
		var myDiv = $('#content');
		myDiv.empty();
		if(following==[]){
			
		}else{
			for(it in following){
				if(following[it].lol==true){
					var profilePic = "pictures/"+following[it].profileUrl;
				}else{
					var profilePic = following[it].profileUrl;
				}
				myDiv.append('<div class="col-md-4">'+
						'<a href="channelPage.html?channel='+following[it].userName+'" target="_self">'+
						'<img id="videoImage" src="'+profilePic+'" alt="video" style="width:100%">'+
						'<div class="caption">'+
							'<p id="titleBar">'+following[it].userName+'</p>'+
						'</div>'+
						'<p id="followingInfo">Followers: '+following[it].subsNumber+' Videos: '+following[it].videosCount+'</p>'+
						'</a>'+
						'</div>');
			}
		}
	});
}
function showLikes(){
	$.get('ChannelServlet',{"channelName":channelName,"status":"likes",},function(data){
		var likes = data.likes;
		
		var myDiv = $('#content');
		myDiv.empty();
		if(likes==[]){
			console.log("noLikes");
		}else{
			for(it in likes){
				if(data.likes[it].blocked==true){
					var videoName = data.likes[it].videoName + ' - <i>Blocked</i>';
				}else{
					var videoName = data.likes[it].videoName;
				}
				myDiv.append('<div class="col-md-4">'+
						'<a href="videoPage.html?id='+likes[it].id+'" target="_self">'+
						'<img id="videoImage" src="'+likes[it].pictureUrl+'" alt="video" style="width:100%">'+
						'<div class="caption">'+
							'<p id="titleBar">'+videoName+'</p>'+
						'</div>'+
						'<p id="videoInfo"><a href="channelPage.html?channel='+likes[it].owner.userName+'" id="channelName">'+likes[it].owner.userName+'</a><br>'+likes[it].views+' views<br>Posted on: '+likes[it].datePosted+'</p>'+
						'</a>'+
						'</div>');
				}
		}
	});
}
function showInfo(){
	var channel = channelPage;
	var myDiv = $('#content');
	myDiv.empty();
	var newDiv = "";
	newDiv+='<h4 id="infoDivHeader">User info</h4>';
	newDiv+='<div class="col-xl-offset-1 col-xl-10 col-md-offset-1 col-md-10 col-sm-offset-1 col-sm-10 col-xs-offset-1 col-xs-10 thumbnail" id="infoDiv">';
	if(channel.description!=null){
		newDiv+='<p>Description:<br>'+channel.description+'</p><br>';
	}
	newDiv+='<p>First name: '+channel.firstName+'<br>'+
				'Last Name: '+channel.lastName+'<br>'+
				'Email: '+channel.email+'<br>'+
				'Joined on: '+channel.registrationDate+'<br>'+
				'Role: '+channel.userType+'<br>';
	if(channel.blocked==true){
		newDiv+='Status: Blocked<br></p>';
	}else{
		newDiv+='Status: Normal<br></p>';
	}
	myDiv.append(newDiv);
}
function initEdit(channel,currentUser,following){
	if(channel==null){
		console.log("blocked1");
		errorPage();
		return;
	}
	
	if(currentUser!=null){
		loggedInUser = currentUser;
		if(following==true){
			$("#followButton").text("Unfollow");
			$("#followButton").attr('class', 'btn btn-default');
		}
		if(channel.userName==currentUser.userName){
			$('#editChannelOption').show();
			$('#deleteOptionChannel').show();
			$('#blockOptionChannel').hide();
		}
		if(channel.blocked==true){
			$('#blockOptionChannel').hide();
			if(currentUser.userName!=channel.userName){
				if(currentUser.userType!="ADMIN"){
					errorPage();
					return;
				}else{
					$('#unblockOptionChannel').show();
				}
			}
		}else{
			$('#unblockOptionChannel').hide();
		}
	}else{
		if(channel.blocked==true){
			errorPage();
			return;
		}
	}
	
	$('#name').empty();
	$('#followers').empty();
	$('#name').append('<h4>'+channel.userName+'</h4>');
	$('#followers').append('<p>Followers: '+channel.subsNumber+'</p>');
	if(channel.lol==true){
		var profilePic = "pictures/"+channel.profileUrl;
	}else{
		var profilePic = channel.profileUrl;
	}
	$('#profileImage').attr("src",profilePic);
	
	//EDIT CHANNEL OPTION 
	$('#editFirstName').val(channel.firstName);
	$('#editLastName').val(channel.lastName);
	$('#editPassword').val(channel.password);
	$('#editEmail').val(channel.email);
	

	if(channel.lol == true){
		$("#lol").prop("checked", true);
		$('#editPicUrl').hide();
	}else{
		$('#editPicUrl').val(channel.profileUrl);
		$('#editUploadPic').hide();
		$('#editPicUrl').val(channel.profileUrl);
	}
	
	$('#editDescription').val(channel.channelDescription);
	
	if(channel.userType==("ADMIN")){
		$('#selectType option[value=2]').attr('selected',true);
	}
	if(currentUser!=null){
		if(currentUser.userType!="ADMIN" || currentUser.userName==channel.userName){
			$('#userTypeSelect').hide();
		}
	}
}
function refreshVideos(){
	var optionNumber = $('#orderVideosSelect').val();
	showRecentVideos(optionNumber);
}
function lolChanged(){
	$("#editUploadPic").toggle();
	$("#editPicUrl").toggle();
}
function editUploadPicture(){
	console.log("editUploadPicture()");
	var formData = new FormData();
	var fileField = document.querySelector("#editUploadPic");
	formData.append("file",fileField.files[0]);
	var xhr = new XMLHttpRequest();
	xhr.open("POST", "UploadPictureServlet");
	xhr.send(formData);
}
function showOrder(){
	$("#h4Tittle").show();
	$("#orderVideosSelect").show();
}
function hideOrder(){
	$("#h4Tittle").hide();
	$("#orderVideosSelect").hide();
}