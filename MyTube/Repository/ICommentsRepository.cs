using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    public interface ICommentsRepository : IDisposable
    {
        IEnumerable<Comment> GetAllCommentsForVideo(long id);
        Comment GetCommentById(long id);
        void CreateComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(long id);

    }
}
