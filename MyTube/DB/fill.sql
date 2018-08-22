INSERT INTO UserType(TypeName)
VALUES('USER');
INSERT INTO UserType(TypeName)
VALUES('ADMIN');

INSERT INTO Users(Username,Pass,Firstname,Lastname,UserType,Email,UserDescription,RegistrationDate,Blocked,Deleted,ProfilePictureUrl,SubscribersCount)
VALUES('123','123','Marko','Markovic','USER','marko@gmail.com',null,'2018-3-1',0,0,'http://www.personalbrandingblog.com/wp-content/uploads/2017/08/blank-profile-picture-973460_640-300x300.png',0);
INSERT INTO Users(Username,Pass,Firstname,Lastname,UserType,Email,UserDescription,RegistrationDate,Blocked,Deleted,ProfilePictureUrl,SubscribersCount)
VALUES('1234','123','Pera','Peric','ADMIN','pera@gmail.com',null,'2018-1-1',0,0,'http://www.personalbrandingblog.com/wp-content/uploads/2017/08/blank-profile-picture-973460_640-300x300.png',0);

INSERT INTO Subscribers(ChannelSubscribed,Subscriber)
VALUES('1234','123');

INSERT INTO VideoType(TypeName)
VALUES('PUBLIC');
INSERT INTO VideoType(TypeName)
VALUES('PRIVATE');
INSERT INTO VideoType(TypeName)
VALUES('UNLISTED');

INSERT INTO Videos(VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,Blocked,Deleted,
			CommentsEnabled,RatingEnabled,LikesCount,DislikesCount,ViewsCount,DatePosted,VideoOwner)
VALUES('https://www.youtube.com/embed/Q0CbN8sfihY','https://s.aolcdn.com/hss/storage/midas/8c786b6e2ab90b7d527621886ee9ff4d/205751517/sw-tlj-ed.jpg',
		'Star Wars: The Last Jedi Trailer','Watch the new trailer for Star Wars: The Last Jedi and see it in theaters December 15.',
		'PUBLIC',0,0,1,1,2,0,21,'2018-3-2','123');
INSERT INTO Videos(VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,Blocked,Deleted,
			CommentsEnabled,RatingEnabled,LikesCount,DislikesCount,ViewsCount,DatePosted,VideoOwner)
VALUES('https://www.youtube.com/embed/qxjPjPzQ1iU','https://i.ytimg.com/vi/h5XQq1ulspc/maxresdefault.jpg',
		'War for the Planet of the Apes','Directed By Matt Reeves Cast: Andy Serkis, Woody Harrelson, Steve Zahn, Amiah Miller, Karin Konoval, Judy Greer and Terry Notary',
		'PRIVATE',0,0,1,1,4,0,11,'2018-3-8','123');
INSERT INTO Videos(VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,Blocked,Deleted,
			CommentsEnabled,RatingEnabled,LikesCount,DislikesCount,ViewsCount,DatePosted,VideoOwner)
VALUES('https://www.youtube.com/embed/5OVY7MmSSYs','http://demofest.org/wp-content/uploads/2017/03/Bad-Copy-4-2.jpg',
		'bad copy - esi mi dobar','"I kad mi pridje neki smarac ja mu kazem odma, esi mi Boban, e, esi mi Boban.."',
		'PUBLIC',0,0,1,1,4,0,43,'2018-3-11','123');

INSERT INTO VideoRating(VideoID,LikeOwner,IsLike,LikeDate,Deleted)
VALUES(1,'123',1,'2018-3-8',0)
INSERT INTO VideoRating(VideoID,LikeOwner,IsLike,LikeDate,Deleted)
VALUES(2,'123',1,'2018-3-8',0)

INSERT INTO VideoRating(VideoID,LikeOwner,IsLike,LikeDate,Deleted)
VALUES(1,'1234',1,'2018-3-8',0)
INSERT INTO VideoRating(VideoID,LikeOwner,IsLike,LikeDate,Deleted)
VALUES(2,'1234',1,'2018-3-8',0)


INSERT INTO CommentRating(LikeOwner,CommentId,IsLike,LikeDate,Deleted)
VALUES ('123','1',1,'2018-3-1',0)
INSERT INTO CommentRating(LikeOwner,CommentId,IsLike,LikeDate,Deleted)
VALUES ('1234','1',1,'2018-3-1',0)

INSERT INTO CommentRating(LikeOwner,CommentId,IsLike,LikeDate,Deleted)
VALUES ('123','2',0,'2018-3-1',0)
INSERT INTO CommentRating(LikeOwner,CommentId,IsLike,LikeDate,Deleted)
VALUES ('1234','2',0,'2018-3-1',0)

