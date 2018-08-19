using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class VideosController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private VideosRepository videosRepository;
        private VideoTypesRepository videoTypesRepository;

        public VideosController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.videoTypesRepository = new VideoTypesRepository(new MyTubeDBEntities());
        }
        public JsonResult IndexPageVideos()
        {
            IEnumerable<Video> videos = null;
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                videos = videosRepository.GetNRandomVideos(6);
            else
                videos = videosRepository.GetNPublicRandomVideos(6);

            List<VideoDTO> vdto = VideoDTO.ConvertCollectionVideoToDTO(videos);

            return Json(vdto, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChannelPageVideosPartial(string channelName, bool? ownedOrLikedVideos, string sortOrder)
        {
            IEnumerable<Video> videos = null;
            if (ownedOrLikedVideos == true)
            {
                ViewBag.SelectedView = "PostedVideos";
                ViewBag.SortValues = Video.VideosSortOrderSelectList();
                videos = VideosPostedBy(channelName);
                ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "latest" : "";
                videos = SortVideos(videos, sortOrder);
            }
            else
                videos = VideosLikedBy(channelName);


            return PartialView(videos);
        }

        public IEnumerable<Video> VideosPostedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                return videosRepository.GetVideosAllOwnedByUser(username);
            else
                return videosRepository.GetVideosPublicOwnedByUser(username);
        }
        public IEnumerable<Video> VideosLikedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                return videosRepository.GetVideosAllLikedByUser(username);
            else
                return videosRepository.GetVideosPublicLikedByUser(username);
        }
        public IEnumerable<Video> SortVideos(IEnumerable<Video> videos, string sortOrder)
        {
            switch (sortOrder)
            {
                case "latest":
                    videos = videos.OrderBy(s => s.DatePosted);
                    break;
                case "oldest":
                    videos = videos.OrderByDescending(s => s.DatePosted);
                    break;
                case "most_viewed":
                    videos = videos.OrderByDescending(s => s.ViewsCount);
                    break;
                case "least_viewed":
                    videos = videos.OrderBy(s => s.ViewsCount);
                    break;

                default:
                    videos = videos.OrderBy(s => s.DatePosted);
                    break;
            }
            return videos;
        }

        public ActionResult BlockVideo(long? id)
        {
            videosRepository.BlockVideo(id);
            ViewBag.Message = "Video has been successfully blocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult UnblockVideo(long? id)
        {
            videosRepository.UnblockVideo(id);
            ViewBag.Message = "Video has been successfully unblocked.";
            return PartialView("MessageModal");
        }

        [HttpPost]
        public ActionResult DeleteVideo(long? id)
        {
            videosRepository.DeleteVideo(id);
            ViewBag.Message = "Video has been successfully deleted.";
            return PartialView("MessageModal");
        }
        public ActionResult EditVideoForm(long? id)
        {
            var video = videosRepository.GetVideoById(id);
            EditVideoModel evm = EditVideoModel.EditVideoModalFromVideo(video);
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectListAndSelect(evm.VideoType);

            return PartialView(evm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVideoForm([Bind(Include = "VideoID,VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,CommentsEnabled,RatingEnabled")] EditVideoModel evm)
        {
            if (ModelState.IsValid)
            {
                var video = videosRepository.GetVideoById(evm.VideoID);
                video.UpdateVideoFromEditVideoModel(evm);
                videosRepository.UpdateVideo(video);
                ViewBag.Message = "Video has been successfully edited.";
                return PartialView("MessageModal");
            }
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectListAndSelect(evm.VideoType);
            return PartialView(evm);
        }
        //-------------------------------------------------------------------
        // GET: Videos
        public ActionResult Index()
        {
            var userType = Session["loggedInUserUserType"];
            if (userType != null)
            {
                if (userType.ToString() == "ADMIN")
                    return View(videosRepository.GetVideosAll());
                else
                    return View(videosRepository.GetVideosPublic());
            }

            return View(videosRepository.GetVideosPublic());
        }

        // GET: Videos/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = videosRepository.GetVideoById(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: Videos/Create
        public ActionResult Create()
        {
            ViewBag.VideoOwner = new SelectList(db.Users, "Username", "Pass");
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectList();
            return View(new Video());
        }

        // POST: Videos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VideoID,VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,Blocked,Deleted,CommentsEnabled,RatingEnabled,LikesCount,DislikesCount,ViewsCount,DatePosted,VideoOwner")] Video video)
        {
            if (ModelState.IsValid)
            {
                videosRepository.InsertVideo(video);
                return RedirectToAction("Index");
            }

            ViewBag.VideoOwner = new SelectList(db.Users, "Username", "Pass", video.VideoOwner);
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectListAndSelect(video.VideoType);
            return View(video);
        }

        // GET: Videos/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = videosRepository.GetVideoById(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            ViewBag.VideoOwner = new SelectList(db.Users, "Username", "Pass", video.VideoOwner);
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectListAndSelect(video.VideoType);
            return View(video);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VideoID,VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,Blocked,Deleted,CommentsEnabled,RatingEnabled,LikesCount,DislikesCount,ViewsCount,DatePosted,VideoOwner")] Video video)
        {
            if (ModelState.IsValid)
            {
                videosRepository.UpdateVideo(video);
                return RedirectToAction("Index");
            }
            ViewBag.VideoOwner = new SelectList(db.Users, "Username", "Pass", video.VideoOwner);
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectListAndSelect(video.VideoType);
            return View(video);
        }

        // GET: Videos/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = videosRepository.GetVideoById(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            videosRepository.DeleteVideo(id);
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
