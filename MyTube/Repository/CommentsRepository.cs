using MyTube.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyTube.Repository
{
    public class CommentsRepository : ICommentsRepository
    {
        private MyTubeDBEntities db;

        // GET: Comments
        public CommentsRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }

        public IEnumerable<Comment> GetAllCommentsForVideo(long id)
        {
            return db.Comments.Where(x => x.VideoID == id && x.Deleted == false);
        }

        public Comment GetCommentById(long id)
        {
            try
            {
                return db.Comments.Single(x => x.CommentID == id && x.Deleted == false);
            }
            catch
            {
                return null;
            }
        }
        public void CreateComment(Comment comment)
        {
            if (comment != null)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
            }
        }
        public void UpdateComment(Comment comment)
        {
            if (comment != null)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void DeleteComment(long id)
        {
            Comment comment = GetCommentById(id);
            if (comment != null)
            {
                comment.Deleted = true;
                db.SaveChanges();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}