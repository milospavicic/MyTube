using MyTube.Models;
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

        public IEnumerable<Video> GetVideosAll()
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false);
        }

        public IEnumerable<Video> GetVideosPublic()
        {
            return db.Videos.Where(x => x.Deleted == false && x.Blocked == false && x.VideoType == PUBLIC_VIDEO);
        }

        public Video GetVideoById(long? id)
        {
            return db.Videos.Find(id);
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