//$('document').ready(function(e){
//	console.log("nav.js file start");
//	$("#registerUploadPic").hide();
//    //loginStatus();
//    $('#logInSubmit').on('click',function(event){
//        var uname = document.getElementById("usernameLogin").value.trim();
//        var pasw = document.getElementById("passwordLogin").value.trim();
//        console.log(uname+"	"+pasw);
//        var status = "";
//        if(uname =="" || pasw==""){
//            $("#loginErrorMessage").text("All fields must be filled!");
//            $("#badLoginModal").modal();
//        }else{
//        	$.post('LoginServlet',{'userName': uname, 'password': pasw},function(data){
//        		if(data.status=="failed"){
//                    $("#loginErrorMessage").text("Wrong username or password!");
//                    $("#badLoginModal").modal();
//        		}else{
//        			$("#login-modal").modal('toggle');
//        			location.reload();
//        		}
//        	});
//        }
//    });
//    //-------------------------------------------------------------------------
//    $('#registerSubmit').on('click',function(event){
//        var uname = document.getElementById("usernameRegister").value.trim();
//        var pasw = document.getElementById("passwordRegister").value.trim();
//        var fname = document.getElementById("firstNameRegister").value.trim();
//        var lname = document.getElementById("lastNameRegister").value.trim();
//        var email = document.getElementById("emailRegister").value.trim();
//    	var rlol = false;
//    	if($('#rlol').is(':checked')){
//    		rlol =true;
//    		var profileUrl = "";
//    		if($("#registerUploadPic")[0].files.length == 0){
//    			$('#regUploadPicModal').modal();
//    			return;
//    		}
//    	}else{
//    		var profileUrl = $('#registerPic').val().trim();
//    		console.log(profileUrl);
//    	}
		
//        var regStatus = "";
//        if(uname == "" || pasw=="" || email==""){
//            $("#registerErrorMessage").text("Fields with * must be filled!");
//            $("#badRegisterModal").modal(); 
//        }
//        else{
//        	$.get('RegisterServlet',{'userName': uname, 'password': pasw,'firstName': fname, 'lastName': lname,'email': email,"profileUrl":profileUrl,"lol":rlol},function(data){
//        		console.log(data.status);
//        		if(data.status=="taken"){
//                    $("#registerErrorMessage").text("Already used username!");
//                    $("#badRegisterModal").modal(); 
//        		}else{
//                    $("#registerErrorMessage").html($('<div class="alert alert-success"><strong>Successful registration!</strong> You may now procced to login.</div>'));
//                    $("#badRegisterModal").modal();
//                    $("#register-modal").modal('toggle');
//                    if(rlol==true){
//                    	uploadPicture();
//                    }
//        		}
//        	}); 	
//        } 
//    });
//});
//function loginStatus(){
//	$.get('LoginServlet',{},function(data){
//	    if(data.user == null){
			
//	        var usersTabInDropdown = document.getElementById("users");
//	        usersTabInDropdown.style.display = "none";
//	        $('#deleteOptionVideo').hide();
//	        $('#deleteOptionChannel').hide(); 
//	        $('#editVideoOption').hide();
//	        $('#editChannelOption').hide();
//	        $('#blockOptionVideo').hide();
//	        $('#blockOptionChannel').hide();
//			$('#unblockOptionVideo').hide();
//			$('#unblockOptionChannel').hide();
//	    }else{
//	    	var tempLink = 'channelPage.html?channel='+data.user.userName;
//			$('#myChannel').attr("href", tempLink);
			
//	        var y = document.getElementById("navbarNotLoggedIn");
//	        y.style.display = "none";
//	        var navbarLoggedIn = document.getElementById("navbarLoggedIn");
//	        navbarLoggedIn.style.display = "block";
//	        var temp = '<span class="glyphicon glyphicon-user"></span>'+" "+data.user.userName+" "+'<span class="caret"></span>';
//	        $("#userNameTab").html(temp);
//	        if(data.user.userType=="USER" || data.user.blocked==true){
//	            var usersTabInDropdown = document.getElementById("users");
//	            usersTabInDropdown.style.display = "none";
//		        $('#deleteOptionVideo').hide();
//		        $('#deleteOptionChannel').hide(); 
//		        $('#editVideoOption').hide();
//		        $('#editChannelOption').hide();
//		        $('#blockOptionVideo').hide();
//		        $('#blockOptionChannel').hide();
//		        $('#unblockOptionVideo').hide();
//		        $('#unblockOptionChannel').hide();
//	        }
//	    }
//	});
//}
//function logout(){
//	$.get('LogoutServlet',{},function(data){location.reload();});
//}
//function search(){
//	var text = $('#searchParameter').val().trim();
//	if(text!=""){
//		document.location.href = "searchPage.html?result_for="+text;
//	}
//	event.preventDefault()
//}
//function saveNewVideo(){
//	var videoUrl = $('#newVideoUrl').val().trim();
//	var title = $('#newVideoName').val().trim();
//	var desc = $('#newDescription').val().trim();
//	var picurl = $('#newPicUrl').val().trim();
//	var visib = $('#selectVisib').val();
//	var comm = $('#selectCE').val();
//	var rating = $('#selectRE').val();
//	var lod = false;
//	if($('#lod').is(':checked')){
//		picurl = "def";
//		lod = true;
//	}
	
//	if(videoUrl == "" || title == "" || picurl == ""){
//		$('#fillAllModal').modal();
//		return;
//	}
	
//	var newUrl = validateYouTubeUrl(videoUrl);
//	if(newUrl==null){
//		$('#invalidLinkModal').modal();
//		return;
//	}
	
//	$.post('VideoServlet',{"status":"newVideo","videoUrl":newUrl,"title":title,"desc":desc,"picurl":picurl,"visib":visib,"comm":comm,"rating":rating,"lod":lod},function(data){
//		if(data.endStatus =="addSuccess"){
//			console.log(data.videoId);
//			document.location.href = "videoPage.html?id="+data.videoId;
//		}else{
//			$('#failModal').modal();
//		}
//	});
//}
//function validateYouTubeUrl(videoUrl)
//{
//    if (videoUrl != undefined || videoUrl != '') {
//        var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|\?v=)([^#\&\?]*).*/;
//        var match = videoUrl.match(regExp);
//        if (match && match[2].length == 11) {
//            var validLink = 'https://www.youtube.com/embed/' + match[2] + '?autoplay=0';
//            return validLink;
//        }
//        else {
//            return null
//        }
//    }else{
//    	return null;
//    }
//}
//function lolChanged(){
//    $("#registerUploadPic").toggle();
//    $("#registerPic").toggle();
//}
//function uploadPicture(){
//    var formData = new FormData();
//    var fileField = document.querySelector("#registerUploadPic");
//    formData.append("file",fileField.files[0]);
//    var xhr = new XMLHttpRequest();
//    xhr.open("POST", "UploadPictureServlet");
//    xhr.send(formData);
//}

//function reloadPage(){
//	location.reload();
//}

//function hideShowLink(){
//	$('#newPicUrl').toggle();
//}