using MyTube.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyTube.Repository
{
    public class VideosRepository : IVideosRepository
    {
        private MyTubeDBEntities db;
        private readonly string PUBLIC_VIDEO = "PUBLIC";

        public VideosRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }
        public IEnumerable<Video> GetNRandomVideos(int n)
        {
            Random r = new Random();

            var videos = db.Videos.Where(x => x.Deleted == false && x.Blocked == false).Take(n);
            return videos;
        }
        public IEnumerable<Video> GetNPublicRandomVideos(int n)
        {
            Random r = new Random();

            var videos = db.Videos.Where(x => x.Deleted == false && x.Blocked == false && x.VideoType == PUBLIC_VIDEO).Take(n);
            return videos;
        }

        public IEnumerable<Video> GetVideosAll()
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false);
        }

        public IEnumerable<Video> GetVideosPublic()
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false && x.VideoType == PUBLIC_VIDEO);
        }

        public IEnumerable<Video> GetVideosAllAndSearch(string searchString)
        {
            var videos = GetVideosAll();
            if (!String.IsNullOrEmpty(searchString))
            {
                videos = videos.Where(s => s.VideoName.Contains(searchString)
                                       || s.VideoDescription.Contains(searchString)
                                       || s.VideoOwner.Contains(searchString));
            }
            return videos;
        }
        public IEnumerable<Video> GetVideosPublicAndSearch(string searchString)
        {
            var videos = GetVideosPublic();
            if (!String.IsNullOrEmpty(searchString))
            {
                videos = videos.Where(s => s.VideoName.Contains(searchString)
                                       || s.VideoDescription.Contains(searchString)
                                       || s.VideoOwner.Contains(searchString));
            }
            return videos;
        }


        public IEnumerable<Video> GetVideosAllOwnedByUser(string username)
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false && x.User.Username == username);
        }

        public IEnumerable<Video> GetVideosPublicOwnedByUser(string username)
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false && x.VideoType == PUBLIC_VIDEO && x.User.Username == username);
        }

        public IEnumerable<Video> GetVideosAllLikedByUser(string username)
        {
            var likedVideos = from videos in db.Videos
                              join likes in db.VideoRatings on videos.VideoID equals likes.VideoID
                              where videos.Deleted == false && videos.Blocked == false && likes.Deleted == false && likes.LikeOwner == username
                              select videos;
            return likedVideos;
        }

        public IEnumerable<Video> GetVideosPublicLikedByUser(string username)
        {
            var likedVideos = from videos in db.Videos
                              join likes in db.VideoRatings on videos.VideoID equals likes.VideoID
                              where videos.Deleted == false && videos.Blocked == false && likes.Deleted == false && likes.LikeOwner == username && videos.VideoType == PUBLIC_VIDEO
                              select videos;

            return likedVideos;
        }

        public Video GetVideoById(long? id)
        {
            try
            {
                return db.Videos.Single(x => x.VideoID == id && x.Deleted == false);
            }
            catch
            {
                return null;
            }


        }

        public void InsertVideo(Video video)
        {
            if (video != null)
            {
                db.Videos.Add(video);
                db.SaveChanges();
            }
        }

        public void UpdateVideo(Video video)
        {
            if (video != null)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void BlockVideo(long? id)
        {
            Video video = GetVideoById(id);
            if (video != null)
            {
                video.Blocked = true;
                db.SaveChanges();
            }
        }
        public void UnblockVideo(long? id)
        {
            Video video = GetVideoById(id);
            if (video != null)
            {
                video.Blocked = false;
                db.SaveChanges();
            }
        }

        public void DeleteVideo(long? id)
        {
            Video video = GetVideoById(id);
            if (video != null)
            {
                video.Deleted = true;
                db.SaveChanges();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}