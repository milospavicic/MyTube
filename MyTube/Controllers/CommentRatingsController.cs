using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class CommentRatingsController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private CommentRatingsRepository commentRatingsRepository;
        private CommentsRepository commentsRepository;
        private UsersRepository usersRepository;

        public CommentRatingsController()
        {
            this.commentRatingsRepository = new CommentRatingsRepository(new MyTubeDBEntities());
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
        public JsonResult CommentRatingsForVideo(long? id)
        {
            var username = (string)Session["loggedInUserUsername"];
            if (username != null)
            {
                var crs = commentRatingsRepository.GetCommentRatingsForVideo((long)id, username);
                var crsDTO = CommentRatingsDTO.ConvertCollectionCommentToDTO(crs);
                return Json(crsDTO, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public JsonResult CreateCommentRating(long commentId, bool newRating)
        {
            string username = (string)Session["loggedInUserUsername"].ToString();
            CommentRating cr = commentRatingsRepository.GetCommentRating(commentId, username);
            if (cr != null)
            {
                return AlterExistingCommentRating(cr, newRating);
            }
            else
            {
                return CreateNewCommentRating(commentId, newRating);
            }
        }
        public JsonResult AlterExistingCommentRating(CommentRating cr, bool newRating)
        {
            string returnMessage = "";
            Comment comment = commentsRepository.GetCommentById((long)cr.CommentId);
            //true = like, false = dislike
            if (cr.IsLike)
            {
                if (newRating)
                {
                    comment.LikesCount -= 1;
                    returnMessage = "neutral";
                }
                else
                {
                    comment.LikesCount -= 1;
                    comment.DislikesCount += 1;
                    returnMessage = "dislike";
                }
            }
            else
            {
                if (newRating)
                {
                    comment.LikesCount += 1;
                    comment.DislikesCount -= 1;
                    returnMessage = "like";
                }
                else
                {
                    comment.DislikesCount -= 1;
                    returnMessage = "neutral";
                }
            }

            commentsRepository.UpdateComment(comment);

            cr.IsLike = newRating;
            if (returnMessage == "neutral")
            {
                commentRatingsRepository.DeleteCommentRating(cr.LikeID);
            }
            else
            {
                commentRatingsRepository.UpdateCommentRating(cr);
            }

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateNewCommentRating(long commentId, bool newRating)
        {
            CommentRating cr = new CommentRating
            {
                LikeOwner = Session["loggedInUserUsername"].ToString(),
                CommentId = commentId,
                LikeDate = DateTime.Now,
                IsLike = newRating
            };
            commentRatingsRepository.CreateCommentRating(cr);
            Comment comment = commentsRepository.GetCommentById((long)cr.CommentId);
            if (newRating)
            {
                comment.LikesCount += 1;
            }
            else
            {
                comment.DislikesCount += 1;
            }
            commentsRepository.UpdateComment(comment);

            string returnMessage = (newRating == true) ? "like" : "dislike";

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);
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
