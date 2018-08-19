using MyTube.Models;
using MyTube.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
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


        // GET: VideoRatings
        public ActionResult Index()
        {
            var videoRatings = db.VideoRatings.Include(v => v.User).Include(v => v.Video);
            return View(videoRatings.ToList());
        }

        // GET: VideoRatings/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoRating videoRating = db.VideoRatings.Find(id);
            if (videoRating == null)
            {
                return HttpNotFound();
            }
            return View(videoRating);
        }

        // GET: VideoRatings/Create
        public ActionResult Create()
        {
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass");
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl");
            return View();
        }

        // POST: VideoRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LikeID,VideoID,LikeOwner,IsLike,LikeDate,Deleted")] VideoRating videoRating)
        {
            if (ModelState.IsValid)
            {
                db.VideoRatings.Add(videoRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", videoRating.LikeOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", videoRating.VideoID);
            return View(videoRating);
        }

        // GET: VideoRatings/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoRating videoRating = db.VideoRatings.Find(id);
            if (videoRating == null)
            {
                return HttpNotFound();
            }
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", videoRating.LikeOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", videoRating.VideoID);
            return View(videoRating);
        }

        // POST: VideoRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LikeID,VideoID,LikeOwner,IsLike,LikeDate,Deleted")] VideoRating videoRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(videoRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LikeOwner = new SelectList(db.Users, "Username", "Pass", videoRating.LikeOwner);
            ViewBag.VideoID = new SelectList(db.Videos, "VideoID", "VideoUrl", videoRating.VideoID);
            return View(videoRating);
        }

        // GET: VideoRatings/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoRating videoRating = db.VideoRatings.Find(id);
            if (videoRating == null)
            {
                return HttpNotFound();
            }
            return View(videoRating);
        }

        // POST: VideoRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VideoRating videoRating = db.VideoRatings.Find(id);
            db.VideoRatings.Remove(videoRating);
            db.SaveChanges();
            return RedirectToAction("Index");
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
