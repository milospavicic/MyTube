var parameter="";
$('document').ready(function(e){
	parameter = window.location.search.slice(1).split('&')[0].split('=')[1];
	parameter = parameter.replace("%20","%");
	console.log(parameter);
    searchVideos(1);
    
    $(':checkbox').change(function() {
    	refreshVideos();
    }); 
});
function errorPage(){
    var errorDiv = $('<div class="row"><div class="col-md-12 col-sm-12"><div class="videoAndControl thumbnail"><img id="videoPlayer" style="width=100%; height=430" src="pictures/errorPic.jpg"></img></div></div></div>')
    $('.mainDiv').empty();
    $('.mainDiv').append(errorDiv);
    $('.mainDiv').css("width", "95%");
    $('.mainDiv').css("margin-bottom", "30px");
    $('.mainDiv').removeClass('thumbnail');
}
function searchVideos(n){
	var videoNameChecked = $("#videoNameSearch").is(":checked");
	var ownerChecked = $("#ownerSearch").is(":checked");
	var viewsChecked = $("#viewCountSearch").is(":checked");
	var dateChecked = $("#dateSearch").is(":checked");
	var commentChecked = $("#commentSearch").is(":checked");
	if(videoNameChecked==false && ownerChecked==false && viewsChecked==false && dateChecked==false && commentChecked==false){
		$("#videoNameSearch").prop("checked", true);
		$("#ownerSearch").prop("checked", true);
		$("#viewCountSearch").prop("checked", true);
		$("#dateSearch").prop("checked", true);
		$("#commentSearch").prop("checked", true);
		videoNameChecked = $("#videoNameSearch").is(":checked");
		ownerChecked = $("#ownerSearch").is(":checked");
		viewsChecked = $("#viewCountSearch").is(":checked");
		dateChecked = $("#dateSearch").is(":checked");
		commentChecked = $("#commentSearch").is(":checked");
	}
	$.get('SearchServlet',{"parameter":parameter,"order":n,"videoNameChecked":videoNameChecked,"ownerChecked":ownerChecked,"viewsChecked":viewsChecked,"dateChecked":dateChecked,"commentChecked":commentChecked},function(data){
		if(data.loggedInUser!=null){
			if(data.loggedInUser.blocked == true){
				$('#uploadVideo').attr("href", "#blockedModal");
			}
		}
		
		var div =  $('.row');
		div.empty();
		for(it in data.videos){
			if(data.videos[it].blocked==true){
				var videoName = data.videos[it].videoName + ' - <i>Blocked</i>';
			}else{
				var videoName = data.videos[it].videoName;
			}
			var divContent = "";
			divContent ='<div class="col-md-12 col-sm-12 col-xs-12">'+
						'<a href="videoPage.html?id='+data.videos[it].id+'" id="picDiv">'+
						'<img src="'+data.videos[it].pictureUrl+'" alt="Lights" style="width:190px;height: 150px">'+
						'<ul id="videoInfo">'+
						'<li><h3>'+videoName+'</h3></li>'+
						'<li id="videoUserViewsDate"><p><a href="channelPage.html?channel='+data.videos[it].owner.userName+'">'+data.videos[it].owner.userName+'</a> - '+data.videos[it].views+' views - '+data.videos[it].datePosted+'</p></li>';
						
			if(data.videos[it].description!=null){
				divContent+='<li><p>'+data.videos[it].description+'</p></li>';
			}		
			divContent+='</ul>'+
						'</a>'+
						'</div>';		
			div.append(divContent);
		}
	});
}
function refreshVideos(){
	var optionNumber = $('#selectOne').val();
	searchVideos(optionNumber);
}
