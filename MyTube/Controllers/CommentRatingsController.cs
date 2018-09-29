using MyTube.App_Start;
using MyTube.DTO;
using MyTube.Helpers;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    [SessionFilter]
    public class CommentRatingsController : Controller
    {
        private ICommentRatingsRepository _commentRatingsRepository;
        private ICommentsRepository _commentsRepository;

        public CommentRatingsController(ICommentRatingsRepository commentRatingsRepository, ICommentsRepository commentsRepository)
        {
            this._commentRatingsRepository = commentRatingsRepository;
            this._commentsRepository = commentsRepository;
        }
        public JsonResult CommentRatingsForVideo(long? id)
        {
            var username = UsersHelper.LoggedInUserUsername(Session);
            if (username != null)
            {
                var crs = _commentRatingsRepository.GetCommentRatingsForVideo((long)id, username);
                var crsDTO = CommentRatingsDTO.ConvertCollectionCommentToDTO(crs);
                return Json(crsDTO, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public JsonResult CreateCommentRating(long commentId, bool newRating)
        {
            if (UsersHelper.LoggedInUserUsername(Session) == null)
            {
                return null;
            }
            string username = UsersHelper.LoggedInUserUsername(Session);
            CommentRating cr = _commentRatingsRepository.GetCommentRating(commentId, username);
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
            Comment comment = _commentsRepository.GetCommentById((long)cr.CommentId);
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

            _commentsRepository.UpdateComment(comment);

            cr.IsLike = newRating;
            if (returnMessage == "neutral")
            {
                _commentRatingsRepository.DeleteCommentRating(cr.LikeID);
            }
            else
            {
                _commentRatingsRepository.UpdateCommentRating(cr);
            }

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateNewCommentRating(long commentId, bool newRating)
        {
            CommentRating cr = new CommentRating
            {
                LikeOwner = UsersHelper.LoggedInUserUsername(Session),
                CommentId = commentId,
                LikeDate = DateTime.Now,
                IsLike = newRating
            };
            _commentRatingsRepository.CreateCommentRating(cr);
            Comment comment = _commentsRepository.GetCommentById((long)cr.CommentId);
            if (newRating)
            {
                comment.LikesCount += 1;
            }
            else
            {
                comment.DislikesCount += 1;
            }
            _commentsRepository.UpdateComment(comment);

            string returnMessage = (newRating == true) ? "like" : "dislike";

            return Json(new { returnMessage, comment.LikesCount, comment.DislikesCount }, JsonRequestBehavior.AllowGet);
        }
    }
}
