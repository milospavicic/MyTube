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
            return db.Comments.Find(id);
        }
        public void CreateComment(Comment comment)
        {
            db.Comments.Add(comment);
            db.SaveChanges();
        }
        public void UpdateComment(Comment comment)
        {
            db.Entry(comment).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteComment(long id)
        {
            Comment comment = GetCommentById(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}