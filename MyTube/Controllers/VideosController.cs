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
    public class VideosController : Controller
    {
        private MyTubeDBEntities db = new MyTubeDBEntities();
        private VideosRepository videosRepository;
        private VideoTypesRepository videoTypesRepository;
        private UsersRepository usersRepository;

        public VideosController()
        {
            this.videosRepository = new VideosRepository(new MyTubeDBEntities());
            this.videoTypesRepository = new VideoTypesRepository(new MyTubeDBEntities());
            this.usersRepository = new UsersRepository(new MyTubeDBEntities());

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
                User loggedInUser = usersRepository.GetUserByUsername(Session["loggedInUserUsername"].ToString());
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
            var videosDTO = VideoDTO.ConvertCollectionVideoToDTO(videos);


            return PartialView(videosDTO);
        }

        public IEnumerable<Video> VideosPostedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            var loggedInUserUsername = (string)Session["loggedInUserUsername"];
            if (loggedInUserUsername == username)
                return videosRepository.GetVideosAllOwnedByUser(username);
            else if (userType == "ADMIN")
                return videosRepository.GetVideosAllOwnedByUser(username);
            else
                return videosRepository.GetVideosPublicOwnedByUser(username);
        }
        public IEnumerable<Video> VideosLikedBy(string username)
        {
            var userType = (string)Session["loggedInUserUserType"];
            var loggedInUserUsername = (string)Session["loggedInUserUsername"];
            if (loggedInUserUsername == username)
                return videosRepository.GetVideosAllLikedByUser(username);
            else if (userType == "ADMIN")
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
        public ActionResult VideoPageRecommended(long? id)
        {
            IEnumerable<Video> videos = null;
            var userType = (string)Session["loggedInUserUserType"];
            if (userType == "ADMIN")
                videos = videosRepository.GetNVideosWithout(6, (long)id);
            else
                videos = videosRepository.GetNPublicVideosWithout(6, (long)id);

            IEnumerable<VideoDTO> videosDTO = VideoDTO.ConvertCollectionVideoToDTO(videos);

            return PartialView(videosDTO);
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
        public ActionResult EditVideoForm([Bind(Include = "VideoID,VideoName,VideoDescription,VideoType,CommentsEnabled,RatingEnabled")] EditVideoModel evm)
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

        [HttpPost]
        public ActionResult ChangePictureUpload(HttpPostedFileBase image, long? videoId)
        {
            if (image == null)
            {
                return View(videosRepository.GetVideoById(videoId));
            }
            if (image.ContentLength > 0)
            {
                var extension = Path.GetExtension(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Pictures/videos"), videoId + extension);
                var finalUrl = "/Pictures/videos/" + videoId + extension;
                var video = videosRepository.GetVideoById(videoId);
                DeleteExistingPictures(videoId);
                video.ThumbnailUrl = finalUrl;
                videosRepository.UpdateVideo(video);
                image.SaveAs(path);
            }
            return RedirectToAction("VideoPage/" + videoId, "Home");
        }
        public void DeleteExistingPictures(long? videoId)
        {
            var path = Path.Combine(Server.MapPath("~/Pictures/videos"));
            string[] fileList = Directory.GetFiles(path);
            foreach (string file in fileList)
            {
                string[] subStrings = file.Split('\\');
                string fileName = subStrings[subStrings.Count() - 1];
                if (fileName.ToUpper().Contains(videoId.ToString()))
                {
                    System.IO.File.Delete(file);
                    return;
                }
            }
        }
        [HttpPost]
        public ActionResult ChangePictureUrl(long? videoId, string ThumbnailUrl)
        {
            if (videoId == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var video = videosRepository.GetVideoById(videoId);
            video.ThumbnailUrl = ThumbnailUrl;
            videosRepository.UpdateVideo(video);
            return RedirectToAction("VideoPage/" + videoId, "Home");
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
