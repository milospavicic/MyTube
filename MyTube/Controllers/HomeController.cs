using MyTube.Models;
using MyTube.Repository;
using System.Web.Mvc;

namespace MyTube.Controllers
{
    public class HomeController : Controller
    {

        private VideosRepository videosRepository;
        private UsersRepository usersRepository;

        public HomeController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());
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
            return View(videosRepository.GetVideoById(id));
        }
        public ActionResult ChannelPage(string id)
        {
            return View(usersRepository.GetUserByUsername(id));
        }
        public ActionResult AdminPage()
        {
            if (!CheckIfPermited()) { return RedirectToAction("Index"); }

            ViewBag.Values = MyTube.Models.User.UsersSortOrder();
            return View(usersRepository.GetUsers());
        }
        public bool CheckIfPermited()
        {
            if (Session["loggedInUserUsername"] == null)
            {
                return false;
            }
            else
            {
                var loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
                if (loggedInUser.UserType != "ADMIN")
                {
                    return false;
                }
            }
            return true;
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
    }
}