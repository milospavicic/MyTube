using MyTube.DTO;
using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class HomeController : Controller
    {

        private VideosRepository videosRepository;
        private VideoTypesRepository videoTypesRepository;
        private UsersRepository usersRepository;
        private UserTypesRepository userTypesRepository;
        private SubscribeRepository subscribeRepository;
        private VideoRatingRepository videoRatingRepository;
        private readonly string BASIC_PICTURE = "https://blog.stylingandroid.com/wp-content/themes/lontano-pro/images/no-image-slide.png";

        public HomeController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.videoTypesRepository = new VideoTypesRepository(new MyTubeDBEntities());

            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
            this.userTypesRepository = new UserTypesRepository(new MyTubeDBEntities());

            this.subscribeRepository = new SubscribeRepository(new MyTubeDBEntities());
            this.videoRatingRepository = new VideoRatingRepository(new MyTubeDBEntities());
        }
        public ActionResult Index()
        {
            IEnumerable<Video> videos = null;
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                videos = videosRepository.GetNRandomVideos(6);
            else
                videos = videosRepository.GetNPublicRandomVideos(6);

            List<VideoDTO> vdto = VideoDTO.ConvertCollectionVideoToDTO(videos);

            return View(videos);
        }
        public ActionResult VideoPage(long? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Video currentVideo = videosRepository.GetVideoById(id);
            currentVideo.ViewsCount += 1;
            videosRepository.UpdateVideo(currentVideo);
            if (Session["loggedInUserUsername"] != null)
            {
                bool exists = subscribeRepository.SubscriptionExists(currentVideo.VideoOwner, Session["loggedInUserUsername"].ToString());
                VideoRating rating = videoRatingRepository.GetVideoRating((long)id, Session["loggedInUserUsername"].ToString());
                ViewBag.Subbed = exists;
                ViewBag.Rating = rating?.IsLike;
            }
            return View(currentVideo);
        }
        public ActionResult ChannelPage(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            var user = usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return View("Error");
            }
            if (Session["loggedInUserUsername"] != null)
            {
                bool exists = subscribeRepository.SubscriptionExists(id, Session["loggedInUserUsername"].ToString());
                ViewBag.Subbed = exists;
            }
            return View(user);
        }
        public ActionResult SearchPage(string id, string sortOrder)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.SortValues = Video.VideosSortOrderSelectList();
            var videos = GetVideosAccordingToUserType(id);
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "latest" : "";
            videos = SortVideos(videos, sortOrder);
            return View(videos);
        }
        public IEnumerable<Video> GetVideosAccordingToUserType(string searchString)
        {
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                return videosRepository.GetVideosAllAndSearch(searchString);
            else
                return videosRepository.GetVideosPublicAndSearch(searchString);
            //if (Session["LoggedInUserUserType"] != null)
            //{
            //    if (Session["LoggedInUserUserType"].ToString() == "ADMIN")
            //    {
            //        return videosRepository.GetVideosAllAndSearch(searchString);
            //    }
            //    else
            //    {
            //        return videosRepository.GetVideosPublicAndSearch(searchString);
            //    }
            //}
            //else
            //{
            //    return videosRepository.GetVideosPublicAndSearch(searchString);
            //}
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
        [ChildActionOnly]
        public ActionResult Navbar()
        {
            var loggedInUser = Session["loggedInUserUsername"];
            if (loggedInUser != null)
            {
                return PartialView(usersRepository.GetUserByUsername(loggedInUser.ToString()));
            }
            return PartialView();
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Register()
        {
            ViewBag.UserType = userTypesRepository.GetUserTypesSelectList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Username,Pass,Firstname,Lastname,Email")] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            bool usernameTaken = usersRepository.UsernameTaken(user.Username);
            if (usernameTaken)
            {
                ViewBag.Message = "Username is taken.";
                ViewBag.UserType = userTypesRepository.GetUserTypesSelectList();
                return View(user);
            }
            user.UserType = "USER";
            user.RegistrationDate = DateTime.Now;
            usersRepository.InsertUser(user);
            return View("RegistrationSuccess");
        }
        public ActionResult NewVideo()
        {
            ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectList();
            return View();
        }
        [HttpPost]
        public ActionResult NewVideo([Bind(Include = "VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,CommentsEnabled,RatingEnabled")] Video video)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectList();
                return View(video);
            }
            var loggedInUser = (string)Session["loggedInUserUsername"];
            if (loggedInUser == null)
            {
                ViewBag.VideoType = videoTypesRepository.GetVideoTypesSelectList();
                return View(video);
            }
            video.VideoOwner = loggedInUser;
            video.DatePosted = DateTime.Now;
            video.ThumbnailUrl = BASIC_PICTURE;
            videosRepository.InsertVideo(video);
            ViewBag.VideoId = video.VideoID;
            ViewBag.VideoName = video.VideoName;
            return View("NewVideoSuccess");
        }
        public ActionResult TestPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TestPage(HttpPostedFileBase file, long? videoId, string ThumbnailUrl)
        {
            if (file == null)
            {
                return View();
            }
            if (file.ContentLength > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                Console.WriteLine(extension);
                var path = Path.Combine(Server.MapPath("~/Pictures/uploads"), videoId + extension);
                var finalUrl = "~/Pictures/uploads/" + videoId + extension;
                Console.WriteLine(path);
                file.SaveAs(path);
            }
            return RedirectToAction("Index");
        }
    }
}