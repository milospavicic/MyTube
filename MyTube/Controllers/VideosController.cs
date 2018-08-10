using MyTube.Models;
using MyTube.Repository;
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
