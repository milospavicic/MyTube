using MyTube.Models;
using MyTube.Repository;
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