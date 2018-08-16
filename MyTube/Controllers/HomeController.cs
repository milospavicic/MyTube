using MyTube.Models;
using MyTube.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class HomeController : Controller
    {

        private VideosRepository videosRepository;
        private UsersRepository usersRepository;
        private SubscribeRepository subscribeRepository;

        public HomeController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
            this.subscribeRepository = new SubscribeRepository(new MyTubeDBEntities());
        }
        public ActionResult Index()
        {
            var username = Session["loggedInUserUsername"];
            if (username != null)
            {
                if (usersRepository.GetUserByUsername(username.ToString()).UserType == "ADMIN")
                    return View(videosRepository.GetVideosAll());
                else
                    return View(videosRepository.GetVideosPublic());
            }

            return View(videosRepository.GetVideosPublic());
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
                bool exists = subscribeRepository.SubscriptionExists(currentVideo.User.Username, Session["loggedInUserUsername"].ToString());
                ViewBag.Subbed = exists;
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
    }
}