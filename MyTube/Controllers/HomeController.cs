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
    public class HomeController : Controller
    {

        private IVideosRepository _videosRepository;
        private IUsersRepository _usersRepository;
        private readonly string BASIC_PICTURE = "https://blog.stylingandroid.com/wp-content/themes/lontano-pro/images/no-image-slide.png";
        public SelectList VideoTypes
        {
            get
            {
                using (var videoTypeRepo = new VideoTypesRepository(new MyTubeDBEntities()))
                {
                    var videoType = videoTypeRepo.GetVideoTypes();
                    return new SelectList(videoType, "TypeName", "TypeName");
                }
            }
        }
        public HomeController(IVideosRepository videosRepository, IUsersRepository usersRepository)
        {
            this._videosRepository = videosRepository;
            this._usersRepository = usersRepository;

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
                User loggedInUser = _usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
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
        public ActionResult Index()
        {
            IEnumerable<Video> videos = null;
            if (LoggedInUserIsAdmin())
                videos = _videosRepository.GetNVideos(6);
            else
                videos = _videosRepository.GetNPublicVideos(6);

            IEnumerable<VideoDTO> vdto = VideoDTO.ConvertCollectionVideoToDTO(videos);

            return View(vdto);
        }
        public ActionResult VideoPage(long? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Video currentVideo = _videosRepository.GetVideoById(id);
            if (currentVideo == null)
            {
                return View("Error");
            }
            if (currentVideo.Blocked == true || currentVideo.User.Blocked == true || currentVideo.VideoType == "PRIVATE")
            {
                if (!(LoggedInUserIsAdmin() && !LoggedInUserIsBlocked()) && !LoggedInUserIsOnHisPage(currentVideo.VideoOwner))
                {
                    return View("Error");
                }
            }
            currentVideo.ViewsCount += 1;
            _videosRepository.UpdateVideo(currentVideo);
            if (Session["loggedInUserUsername"] != null)
            {
                bool exists = CheckIfSubbed(currentVideo.VideoOwner);
                VideoRating rating = GetVideoRatingForVideo(id);
                ViewBag.Subbed = exists;
                ViewBag.Rating = rating?.IsLike;
            }
            var video = VideoDTO.ConvertVideoToDTO(currentVideo);
            return View(video);
        }
        public VideoRating GetVideoRatingForVideo(long? id)
        {
            if (Session["loggedInUserUsername"] != null)
            {
                using (var videoRatingRepository = new VideoRatingRepository(new MyTubeDBEntities()))
                {
                    VideoRating rating = videoRatingRepository.GetVideoRating((long)id, Session["loggedInUserUsername"].ToString());
                    return rating;
                }
            }
            return null;
        }
        public ActionResult ChannelPage(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            var user = _usersRepository.GetUserByUsername(id);
            if (user == null)
            {
                return View("Error");
            }
            if (user.Blocked == true)
            {
                if (!(LoggedInUserIsAdmin() && !LoggedInUserIsBlocked()) && !LoggedInUserIsOnHisPage(user.Username))
                {
                    return View("Error");
                }
            }
            if (!LoggedInUserIsOnHisPage(user.Username))
                ViewBag.Subbed = CheckIfSubbed(id);
            var userDTO = UserDTO.ConvertUserToDTO(user);
            return View(userDTO);
        }
        public bool CheckIfSubbed(string id)
        {
            if (Session["loggedInUserUsername"] != null)
            {
                using (var subscribeRepository = new SubscribeRepository(new MyTubeDBEntities()))
                {
                    bool exists = subscribeRepository.SubscriptionExists(id, Session["loggedInUserUsername"].ToString());
                    return exists;
                }
            }
            return false;
        }
        public ActionResult AdminPage(string sortOrder, string searchString)
        {
            if (!CheckIfPermited()) { return RedirectToAction("Index", "Home"); }

            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "username_asc" : "";
            ViewBag.SearchString = searchString;

            var users = _usersRepository.GetAndSearchUsers(searchString);
            users = SortUsers(users, sortOrder);

            ViewBag.Values = Models.User.UsersSortOrderSelectList();

            var usersDTO = UserDTO.ConvertCollectionUserToDTO(users);
            return View(usersDTO);
        }
        public bool CheckIfPermited()
        {
            var username = (string)Session["loggedInUserUsername"];
            if (username == null)
            {
                return false;
            }
            else
            {
                var user = _usersRepository.GetUserByUsername(username);
                if (user == null || user.Blocked == true)
                {
                    return false;
                }
            }
            return true;
        }
        public IEnumerable<User> SortUsers(IEnumerable<User> users, string sortOrder)
        {
            switch (sortOrder)
            {
                case "username_asc":
                    users = users.OrderBy(s => s.Username);
                    break;
                case "username_desc":
                    users = users.OrderByDescending(s => s.Username);
                    break;
                case "firstname_asc":
                    users = users.OrderBy(s => s.Firstname);
                    break;
                case "firstname_desc":
                    users = users.OrderByDescending(s => s.Firstname);
                    break;
                case "lastname_asc":
                    users = users.OrderBy(s => s.Lastname);
                    break;
                case "lastname_desc":
                    users = users.OrderByDescending(s => s.Lastname);
                    break;
                case "email_asc":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "user_type_asc":
                    users = users.OrderBy(s => s.UserType);
                    break;
                case "user_type_desc":
                    users = users.OrderByDescending(s => s.UserType);
                    break;
                case "status_asc":
                    users = users.OrderBy(s => s.Blocked);
                    break;
                case "status_desc":
                    users = users.OrderByDescending(s => s.Blocked);
                    break;

                default:
                    users = users.OrderBy(s => s.Username);
                    break;
            }
            return users;
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
            var videoDTO = VideoDTO.ConvertCollectionVideoToDTO(videos);
            return View(videoDTO);
        }
        public IEnumerable<Video> GetVideosAccordingToUserType(string searchString)
        {
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                return _videosRepository.GetVideosAllAndSearch(searchString);
            else
                return _videosRepository.GetVideosPublicAndSearch(searchString);
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
                var user = _usersRepository.GetUserByUsername(loggedInUser.ToString());
                var userDTO = UserDTO.ConvertUserToDTO(user);
                return PartialView(userDTO);
            }
            return PartialView();
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Register()
        {
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
            bool usernameTaken = _usersRepository.UsernameTaken(user.Username);
            if (usernameTaken)
            {
                ViewBag.Message = "Username is taken.";
                return View(user);
            }
            user.UserType = "USER";
            user.RegistrationDate = DateTime.Now;
            _usersRepository.InsertUser(user);
            return View("RegistrationSuccess");
        }
        public ActionResult NewVideo()
        {
            var video = new Video
            {
                CommentsEnabled = true,
                RatingEnabled = true
            };

            ViewBag.VideoType = VideoTypes;
            return View(video);
        }
        [HttpPost]
        public ActionResult NewVideo([Bind(Include = "VideoUrl,ThumbnailUrl,VideoName,VideoDescription,VideoType,CommentsEnabled,RatingEnabled")] Video video)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VideoType = VideoTypes;
                return View(video);
            }
            var videoUrl = CheckIfYTUrl(video.VideoUrl);
            if (videoUrl == null)
            {
                ViewBag.VideoType = VideoTypes;
                ViewBag.Message = "Invalid youtube url.";
                return View(video);
            }
            else
            {
                video.VideoUrl = videoUrl;
            }
            var loggedInUser = (string)Session["loggedInUserUsername"];
            if (loggedInUser == null)
            {
                ViewBag.VideoType = VideoTypes;
                return View("Index");
            }
            video.VideoOwner = loggedInUser;
            video.DatePosted = DateTime.Now;
            video.ThumbnailUrl = BASIC_PICTURE;
            _videosRepository.InsertVideo(video);
            ViewBag.VideoId = video.VideoID;
            ViewBag.VideoName = video.VideoName;
            return View("NewVideoSuccess");
        }


        public string CheckIfYTUrl(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (!response.ResponseUri.ToString().Contains("www.youtube.com"))
                    {
                        return null;
                    }
                    if (response.ResponseUri.ToString().Contains("www.youtube.com/embed/"))
                    {
                        return url;
                    }
                    if (response.ResponseUri.ToString().Contains("www.youtube.com/watch?v="))
                    {
                        return ConvertWatchToEmbedUrl(url);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        public string ConvertWatchToEmbedUrl(string url)
        {
            //https://www.youtube.com/embed/c7IFocqf8bA
            string replace = "embed/";
            string old = "watch?v=";

            string newUrl = url.Replace(old, replace);
            return newUrl;
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
        public bool LoggedInUserIsAdmin()
        {
            if ((string)Session["loggedInUserUserType"] == "ADMIN")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool LoggedInUserIsOnHisPage(string username)
        {
            if ((string)Session["loggedInUserUsername"] == username)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool LoggedInUserIsBlocked()
        {
            var status = (string)Session["loggedInUserStatus"];

            if (status == "True")
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