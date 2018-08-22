using MyTube.Models;
using MyTube.Repository;
using System;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class VideoRatingsController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private VideoRatingRepository videoRatingRepository;
        private VideosRepository videosRepository;

        public VideoRatingsController()
        {
            this.videoRatingRepository = new VideoRatingRepository(new MyTubeDBEntities());
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
        }
        [HttpPost]
        public JsonResult CreateVideoRating(long videoId, bool newRating)
        {
            string username = (string)Session["loggedInUserUsername"].ToString();
            VideoRating vr = videoRatingRepository.GetVideoRating(videoId, username);
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
            Video video = videosRepository.GetVideoById(vr.VideoID);
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

            videosRepository.UpdateVideo(video);

            vr.IsLike = newRating;
            if (returnMessage == "neutral")
            {
                videoRatingRepository.DeleteVideoRating(vr.LikeID);
            }
            else
            {
                videoRatingRepository.UpdateVideoRating(vr);
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
            videoRatingRepository.CreateVideoRating(vr);
            Video video = videosRepository.GetVideoById(vr.VideoID);
            if (newRating)
            {
                video.LikesCount += 1;
            }
            else
            {
                video.DislikesCount += 1;
            }
            videosRepository.UpdateVideo(video);

            string returnMessage = (newRating == true) ? "like" : "dislike";

            return Json(new { returnMessage, video.LikesCount, video.DislikesCount }, JsonRequestBehavior.AllowGet);
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
