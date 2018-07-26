var loggedInUser = "";
var tempChannelName = "";
var allUsers = null;
var blocked = false;
var doSearch = false;
var tempChannel = null;
$('document').ready(function(e){
	fillPage(1);
});  
function fillPage(sort){
	if(doSearch!=false){
		console.log(doSearch);
		var input = $('#searchUsers').val().toLowerCase();
	}
	$.get('FollowUserServlet',{},function(dataOne){
		console.log(dataOne.userSubs);
		var userDiv = $('.row');
		userDiv.empty();
		$.get('GetUsersServlet',{"search":doSearch,"input":input,"sort":sort},function(data){
			if(data.loggedInUser!=null){
				if(data.loggedInUser.blocked == true){
					$('#uploadVideo').attr("href", "#blockedModal");
					blocked = data.loggedInUser.blocked;
				}
			}
		    if(data.loggedInUser==null){
		    	console.log(data.loggedInUser + "   loggedInUser");
		        window.location.href = "index.html";
		    }else{
		    	if(data.loggedInUser.userType!="ADMIN" || data.loggedInUser.blocked == true){
		    		window.location.href = "index.html";
		    	}else{
		    		loggedInUser = data.loggedInUser.userName;
		    	}
		    }
			var counter = 1;
			allUsers = data.users;
			for(it in data.users){
				if(data.users[it].userName==loggedInUser)
					continue;
				if(counter>10) break;
				var description = data.users[it].channelDescription;
				if(description==null) 
					description="";
				var divForAppend = "";
				for(user in data.userSubs){
					console.log(data.userSubs + "temp");
				}
				if(data.users[it].lol==true){
					var profilePic = "pictures/"+data.users[it].profileUrl;
				}else{
					var profilePic = data.users[it].profileUrl;
				}
				
				divForAppend = '<div class="col-md-12 col-sm-12 col-xs-12" id="div'+counter+'">'+
									'<a href="channelPage.html?channel='+data.users[it].userName+'" id="picDiv">'+
									'<img src="'+profilePic+'" alt="Lights" style="width:150px;height: 150px">'+
									'<ul id="channelInfo">'+
										'<li><h3>'+data.users[it].userName+'</h3></li>'+
										'<li id="subsNVideos'+data.users[it].userName+'"><p id="temp'+data.users[it].userName+'">Followers: '+data.users[it].subsNumber+'</p></li>'+
										'<li><p>Name: '+data.users[it].firstName+" "+data.users[it].lastName+'</p></li>'+
										'<li><p>Email: '+data.users[it].email+'</p></li>'+
										'<li><p>Role: '+data.users[it].userType+'</p></li>';
				if(data.users[it].blocked==true){
					divForAppend+='<li><p>Status: Blocked</p></li>'+
					'</ul>'+
					'</a>';
				}else{
					divForAppend+='<li><p>Status: Normal</p></li>'+
					'</ul>'+
					'</a>';
				}
										
				
				var tempStatus = "false";
				for(temp in dataOne.userSubs){
					var stringOne = "";
					var stringTwo = "";
					stringOne=dataOne.userSubs[temp];
					stringTwo=data.users[it].userName;
					
					if(stringOne==stringTwo){
						tempStatus = "true";
						break;
					}		
				}
				console.log("tempStatus "+tempStatus)
				
				if(tempStatus == "false"){
					divForAppend+='<div class="btn-group" role="group" id="channelButton">'+
										'<button type="button" class="btn btn-danger"  title="Follow channel." id="btnFollow'+counter+'" onclick="follow(this,\''+data.users[it].userName+'\',\''+data.users[it].subsNumber+'\')">Follow</button>';
				}
				else{
					divForAppend+='<div class="btn-group" role="group" id="channelButton">'+
									   '<button type="button" class="btn btn-default" title="Unfollow channel." id="btnFollow'+counter+'" onclick="follow(this,\''+data.users[it].userName+'\',\''+data.users[it].subsNumber+'\')">Unfollow</button>';
				}
				divForAppend+='<button onclick="editModal(\''+data.users[it].userName+'\')" type="button" class="btn btn-default" title="Edit channel." id="btnEdit'+counter+'" "><span class="glyphicon glyphicon-edit"></span></button>';
				
				if(data.users[it].blocked==true){
					divForAppend+='<button onclick="unblockModal(\''+data.users[it].userName+'\')" type="button" class="btn btn-default" title="Unblock channel." id="btnUnblock'+counter+'" "><span class="glyphicon glyphicon-ok-circle"></span></button>';
				}else{
					divForAppend+='<button onclick="blockModal(\''+data.users[it].userName+'\')" type="button" class="btn btn-default" title="Block channel." id="btnBlock'+counter+'" "><span class="glyphicon glyphicon-ban-circle"></span></button>';
				}
				
				divForAppend+='<button onclick="deleteModal(\''+data.users[it].userName+'\')" type="button" class="btn btn-default" title="Delete channel." id="btnDelete'+counter+'" "><span class="glyphicon glyphicon-trash"></span></button>'+
								'</div>'+
								'</div>';
	
				userDiv.append(divForAppend);
				counter++;
			}
		});
	});
}
function deleteModal(userName){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	tempChannelName = userName;
	$('#deleteModal').modal();
}
function blockModal(userName){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	tempChannelName = userName;
	$('#blockModal').modal();
}
function unblockModal(userName){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	tempChannelName = userName;
	$('#unblockModal').modal();
}
function editModal(user){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
	tempChannelName = user;
	
	console.log("editClick");
	for(it in allUsers){
		if(allUsers[it].userName==user){
			tempChannel = allUsers[it];
			$('#editFirstName').val(allUsers[it].firstName);
			$('#editLastName').val(allUsers[it].lastName);
			$('#editPassword').val(allUsers[it].password);
			$('#editEmail').val(allUsers[it].email);
			if(allUsers[it].lol == true){
				$("#lol").prop("checked", true);
				$('#editPicUrl').hide();
				$('#editUploadPic').show();
				$('#editPicUrl').val("");
			}else{
				$('#editPicUrl').val(allUsers[it].profileUrl);
				$('#editUploadPic').hide();
				$('#editPicUrl').show();
				
			}
			
			//$('#editPicUrl').val(allUsers[it].profileUrl);
			$('#editDescription').val(allUsers[it].channelDescription);
			if(allUsers[it].userType==("ADMIN")){
				$('#selectType option[value=2]').attr('selected',true);
			}
			$('#editChannel-modal').modal();
		}
	}
}
function saveEditUser(){
	console.log("saveEditUser");
	var firstName = $('#editFirstName').val().trim();
	var lastName = $('#editLastName').val().trim();
	var password = $('#editPassword').val().trim();
	var email = $('#editEmail').val().trim();
	var channelDescription = $('#editDescription').val().trim();
	var userType = $('#selectType').val();
	
	var lol = false;
	if($('#lol').is(':checked')){
		lol =true;
		if($("#editUploadPic")[0].files.length == 0){
			console.log("Keep old pic");
			if(tempChannel.lol==false){
				$('#uploadPicModal').modal();
				return;
			}else{
				var profileUrl=tempChannel.profileUrl;
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
	
	$.post('ChannelServlet',{"status":"edit","channelName":tempChannelName,"firstName":firstName,"lastName":lastName,"password":password,"email":email,"channelDescription":channelDescription,"userType":userType,"profileUrl":profileUrl,"lol":lol,"newFile":newFile},function(data){
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
function deleteUser(){
	$.post('ChannelServlet',{"status":"delete","channelName":tempChannelName},function(data){		
		if(data.endStatus=="deleteSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
	});
}
function blockUser(){
	$.post('ChannelServlet',{"status":"block","channelName":tempChannelName},function(data){
		if(data.endStatus=="blockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
		
	});
}
function unblockUser(){
	$.post('ChannelServlet',{"status":"unblock","channelName":tempChannelName},function(data){
		if(data.endStatus=="unblockSuccess"){
			location.reload();
		}else{
			$('#failModal').modal();
		}
		
	});
}
function errorPage(){
    var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
    $('.mainDiv').empty();
    $('.mainDiv').append(errorDiv);
    $('.mainDiv').css("width", "95%");
    $('.mainDiv').css("margin-bottom", "30px");
}
function follow(btn,userName,followers){
	if(blocked == true){
		$("#blockedModal").modal('toggle');
		return;
	}
    var tempName = $("#"+btn.id).text();
    var newText =  $('#subsNVideos'+userName);
    
    var tempFollowers =  $('#temp'+userName).text();
    tempFollowers = tempFollowers.substring(11);
    if(tempName == "Follow"){

        $.post('FollowUserServlet',{"userName":userName,"status":"follow"},function(data){
        	if(data.endStatus=="Success"){
	        	$("#"+btn.id).text("Unfollow");
	            $("#"+btn.id).attr('class', 'btn btn-default');
	        	tempFollowers = parseInt(tempFollowers)+1;
	            console.log(tempFollowers+" fo");
	            newText.empty();
	            newText.append('<p id="temp'+userName+'">Followers: '+tempFollowers+'</p>');
        	}else{
        		$('#failModal').modal();
        	}
        });
        
    }else{
        $.post('FollowUserServlet',{"userName":userName,"status":"unfollow"},function(data){
        	if(data.endStatus=="Success"){
                $("#"+btn.id).text("Follow");
                $("#"+btn.id).attr('class', 'btn btn-danger');
            	tempFollowers = parseInt(tempFollowers)-1;
                console.log(tempFollowers+" un");
                newText.empty();
                newText.append('<p id="temp'+userName+'">Followers: '+tempFollowers+'</p>');
        	}else{
        		$('#failModal').modal();
        	}
        });
        
    }
}
function searchUsersButton(){
	doSearch = true;
	fillPage(1);
}

function lolChanged(){
    $("#editUploadPic").toggle();
    $("#editPicUrl").toggle();
}
function refreshUsers(){
	var optionNumber = $('#selectSort').val();
	console.log(optionNumber);
	fillPage(optionNumber);
}
function editUploadPicture(){
    var formData = new FormData();
    var fileField = document.querySelector("#editUploadPic");
    formData.append("file",fileField.files[0]);
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "UploadPictureServlet");
    xhr.send(formData);
}