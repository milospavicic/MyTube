using MyTube.Models;
using System.Data.Entity;
using System.Linq;

namespace MyTube.Repository
{
    public class VideoRatingRepository : IVideoRatingRepository
    {
        private MyTubeDBEntities db;

        public VideoRatingRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }
        public VideoRating GetVideoRatingById(long id)
        {
            return db.VideoRatings.Find(id);
        }

        public VideoRating GetVideoRating(long videoId, string username)
        {
            try
            {
                return db.VideoRatings.Single(x => x.VideoID == videoId && x.LikeOwner == username && x.Deleted == false);
            }
            catch
            {
                return null;
            }

        }

        public bool RatingExistsForVideo(long videoId, string username)
        {
            return db.VideoRatings.Any(x => x.VideoID == videoId && x.LikeOwner == username && x.Deleted == false);
        }

        public void CreateVideoRating(VideoRating vr)
        {
            db.VideoRatings.Add(vr);
            db.SaveChanges();
        }
        public void UpdateVideoRating(VideoRating vr)
        {
            db.Entry(vr).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteVideoRating(long ratingId)
        {
            var vr = GetVideoRatingById(ratingId);
            vr.Deleted = true;
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}