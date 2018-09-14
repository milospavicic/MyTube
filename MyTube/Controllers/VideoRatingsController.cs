using MyTube.Models;
using MyTube.Repository;
using System;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class VideoRatingsController : Controller
    {
        private IVideoRatingRepository _videoRatingRepository;
        private IVideosRepository _videosRepository;

        public VideoRatingsController(IVideoRatingRepository videoRatingRepository, IVideosRepository videosRepository)
        {
            this._videoRatingRepository = videoRatingRepository;
            this._videosRepository = videosRepository;

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
                User loggedInUser = null;
                using (var usersRepository = new UsersRepository(new MyTubeDBEntities()))
                {
                    loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                }
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
        [HttpPost]
        public JsonResult CreateVideoRating(long videoId, bool newRating)
        {
            if (!LoggedInUserExists())
            {
                return null;
            }
            string username = (string)Session["loggedInUserUsername"].ToString();
            VideoRating vr = _videoRatingRepository.GetVideoRating(videoId, username);
            if (vr != null)
            {
                return AlterExistingVideoRating(vr, newRating);
            }
            else
            {
                return CreateNewVideoRating(videoId, newRating);
            }
        }
        public JsonResult AlterExistingVideoRating(VideoRating vr, bool newRating)
        {
            string returnMessage = "";
            Video video = _videosRepository.GetVideoById(vr.VideoID);
            //true = like, false = dislike
            if (vr.IsLike)
            {
                if (newRating)
                {
                    video.LikesCount -= 1;
                    returnMessage = "neutral";
                }
                else
                {
                    video.LikesCount -= 1;
                    video.DislikesCount += 1;
                    returnMessage = "dislike";
                }
            }
            else
            {
                if (newRating)
                {
                    video.LikesCount += 1;
                    video.DislikesCount -= 1;
                    returnMessage = "like";
                }
                else
                {
                    video.DislikesCount -= 1;
                    returnMessage = "neutral";
                }
            }

            _videosRepository.UpdateVideo(video);

            vr.IsLike = newRating;
            if (returnMessage == "neutral")
            {
                _videoRatingRepository.DeleteVideoRating(vr.LikeID);
            }
            else
            {
                _videoRatingRepository.UpdateVideoRating(vr);
            }

            return Json(new { returnMessage, video.LikesCount, video.DislikesCount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateNewVideoRating(long videoId, bool newRating)
        {
            VideoRating vr = new VideoRating
            {
                LikeOwner = Session["loggedInUserUsername"].ToString(),
                VideoID = videoId,
                LikeDate = DateTime.Now,
                IsLike = newRating
            };
            _videoRatingRepository.CreateVideoRating(vr);
            Video video = _videosRepository.GetVideoById(vr.VideoID);
            if (newRating)
            {
                video.LikesCount += 1;
            }
            else
            {
                video.DislikesCount += 1;
            }
            _videosRepository.UpdateVideo(video);

            string returnMessage = (newRating == true) ? "like" : "dislike";

            return Json(new { returnMessage, video.LikesCount, video.DislikesCount }, JsonRequestBehavior.AllowGet);
        }

        public bool LoggedInUserExists()
        {
            if (Session["loggedInUserStatus"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
