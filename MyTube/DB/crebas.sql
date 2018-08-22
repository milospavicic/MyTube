CREATE DATABASE MyTube
--DROP DATABASE MyTube

CREATE TABLE UserType(
TypeName VARCHAR(10) NOT NULL CHECK (TypeName IN('USER', 'ADMIN')) PRIMARY KEY,
);
CREATE TABLE Users(
Username VARCHAR(16) NOT NULL PRIMARY KEY,
Pass VARCHAR(16) NOT NULL,
Firstname VARCHAR(15),
Lastname VARCHAR(15),
UserType VARCHAR(10) REFERENCES UserType(TypeName),
Email VARCHAR(30) NOT NULL,
UserDescription VARCHAR(500),
RegistrationDate DATE NOT NULL,
Blocked BIT NOT NULL DEFAULT 0,
Deleted BIT NOT NULL DEFAULT 0,
ProfilePictureUrl VARCHAR(300),
);
CREATE TABLE Subscribers(
SubID BIGINT PRIMARY KEY IDENTITY(1,1),
ChannelSubscribed VARCHAR(16) REFERENCES Users(Username),
Subscriber VARCHAR(16) REFERENCES Users(Username)
);
CREATE TABLE VideoType(
TypeName varchar(10) NOT NULL CHECK (TypeName IN('PRIVATE', 'PUBLIC', 'UNLISTED')) PRIMARY KEY,
);
CREATE TABLE Videos(
VideoID BIGINT PRIMARY KEY IDENTITY(1,1),
VideoUrl VARCHAR(100) NOT NULL,
ThumbnailUrl VARCHAR(200) NOT NULL,
VideoName VARCHAR(80) NOT NULL,
VideoDescription VARCHAR(500),
VideoType varchar(10) REFERENCES VideoType(TypeName),
Blocked BIT NOT NULL DEFAULT 0,
Deleted BIT NOT NULL DEFAULT 0,
CommentsEnabled BIT NOT NULL DEFAULT 1,
RatingEnabled BIT NOT NULL DEFAULT 1,
LikesCount BIGINT NOT NULL ,
DislikesCount BIGINT NOT NULL,
ViewsCount BIGINT NOT NULL,
DatePosted  DATE NOT NULL,
VideoOwner VARCHAR(16) REFERENCES Users(Username)
);
CREATE TABLE Comments(
CommentID BIGINT PRIMARY KEY IDENTITY(1,1),
VideoID BIGINT REFERENCES Videos(VideoID),
CommentOwner VARCHAR(16) REFERENCES Users(Username),
CommentText VARCHAR(500) NOT NULL,
DatePosted DATE NOT NULL,
LikesCount BIGINT NOT NULL ,
DislikesCount BIGINT NOT NULL,
Deleted BIT NOT NULL DEFAULT 0
);
CREATE TABLE VideoRating(
LikeID BIGINT PRIMARY KEY IDENTITY(1,1),
VideoID BIGINT REFERENCES Videos(VideoID),
LikeOwner VARCHAR(16) REFERENCES Users(Username),
IsLike BIT NOT NULL,
LikeDate DATE NOT NULL,
Deleted BIT NOT NULL DEFAULT 0
);
CREATE TABLE CommentRating(
LikeID BIGINT PRIMARY KEY IDENTITY(1,1),
LikeOwner VARCHAR(16) REFERENCES Users(Username),
CommentId BIGINT REFERENCES Comments(CommentID),
IsLike BIT NOT NULL,
LikeDate DATE NOT NULL,
Deleted BIT NOT NULL DEFAULT 0
);