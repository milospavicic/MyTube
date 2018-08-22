using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class CommentsController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private VideosRepository videosRepository;
        private CommentsRepository commentsRepository;
        private UsersRepository usersRepository;

        public CommentsController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.commentsRepository = new CommentsRepository(new MyTubeDBEntities());
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
            CheckLoggedInUser();
        }
        private void CheckLoggedInUser()
        {
            if (Session == null)
            {
                return;
            }
            else
            {
                User loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                if (loggedInUser == null)
                {
                    Session.Abandon();
                }
                else
                {
                    Session.Add("loggedInUserUsername", loggedInUser.Username);
                    Session.Add("loggedInUserUserType", loggedInUser.UserType);
                    Session.Add("loggedInUserStatus", loggedInUser.Blocked.ToString());
                }
            }
        }
        public ActionResult CommentSection(long? id, string sortOrder)
        {
            ViewBag.VideoId = id;
            var video = videosRepository.GetVideoById(id);
            ViewBag.VideoOwner = video.VideoOwner;
            ViewBag.CommentsEnabled = video.CommentsEnabled;
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "latest" : "";
            ViewBag.Values = Comment.CommentsSortOrderSelectList();
            if (id == null)
            {
                return PartialView();
            }
            var comments = commentsRepository.GetAllCommentsForVideo((long)id);
            comments = SortComments(comments, sortOrder);
            var commentsDTO = CommentDTO.ConvertCollectionCommentToDTO(comments);
            return PartialView(commentsDTO);
        }
        public IEnumerable<Comment> SortComments(IEnumerable<Comment> comments, string sortOrder)
        {
            switch (sortOrder)
            {
                case "latest":
                    comments = comments.OrderByDescending(s => s.DatePosted);
                    break;
                case "oldest":
                    comments = comments.OrderBy(s => s.DatePosted);
                    break;
                case "most_popular":
                    comments = comments.OrderByDescending(s => s.LikesCount).ThenBy(s => s.DislikesCount);
                    break;
                case "least_popular":
                    comments = comments.OrderByDescending(s => s.DislikesCount).ThenBy(s => s.DislikesCount);
                    break;

                default:
                    comments = comments.OrderByDescending(s => s.DatePosted);
                    break;
            }
            return comments;
        }
        public ActionResult CreateComment(Comment comment)
        {
            var currentUser = (string)Session["loggedInUserUsername"];
            if (currentUser == null)
            {
                return null;
            }
            comment.CommentOwner = currentUser;
            comment.DatePosted = DateTime.Now;
            commentsRepository.CreateComment(comment);

            ViewBag.Values = Comment.CommentsSortOrderSelectList();
            var cdto = CommentDTO.ConvertCommentToDTO(comment);
            return PartialView("SingleComment", cdto);

        }
        [HttpPost]
        public ActionResult EditComment(long? id, string text)
        {
            if (id == null)
            {
                return null;
            }
            Comment comment = commentsRepository.GetCommentById((long)id);
            if (comment == null)
            {
                return null;
            }
            comment.CommentText = text;
            commentsRepository.UpdateComment(comment);
            ViewBag.Message = "Comment has been successfully edited.";
            return PartialView("MessageModal");

        }
        public ActionResult DeleteComment(long id)
        {
            commentsRepository.DeleteComment(id);
            ViewBag.Message = "Comment has been successfully deleted.";
            return PartialView("MessageModal");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
