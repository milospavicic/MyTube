using MyTube.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyTube.Repository
{
    public class CommentRatingsRepository : ICommentRatingsRepository
    {
        private MyTubeDBEntities db;

        public CommentRatingsRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }
        public CommentRating GetCommentRatingById(long id)
        {
            try
            {
                return db.CommentRatings.Single(x => x.LikeID == id && x.Deleted == false);
            }
            catch
            {
                return null;
            }
        }

        public CommentRating GetCommentRating(long commentId, string username)
        {
            try
            {
                return db.CommentRatings.Single(x => x.CommentId == commentId && x.LikeOwner == username && x.Deleted == false);
            }
            catch
            {
                return null;
            }
        }
        public IEnumerable<CommentRating> GetCommentRatingsForVideo(long videoId, string username)
        {
            IEnumerable<CommentRating> crs = from cr in db.CommentRatings
                                             join c in db.Comments on cr.CommentId equals c.CommentID
                                             where c.VideoID == videoId && cr.Deleted == false && c.Deleted == false && cr.LikeOwner == username
                                             select cr;
            return crs;
        }

        public bool RatingExistsForComment(long commentId, string username)
        {
            return db.CommentRatings.Any(x => x.CommentId == commentId && x.LikeOwner == username && x.Deleted == false);
        }

        public void CreateCommentRating(CommentRating cr)
        {
            db.CommentRatings.Add(cr);
            db.SaveChanges();
        }

        public void UpdateCommentRating(CommentRating cr)
        {
            db.Entry(cr).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void DeleteCommentRating(long ratingId)
        {
            var cr = GetCommentRatingById(ratingId);
            cr.Deleted = true;
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}