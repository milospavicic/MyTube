using MyTube.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTube.DTO
{
    public class VideoDTO
    {
        public VideoDTO()
        {

        }

        public long VideoID { get; set; }

        [Required]
        [Display(Name = "Video Url")]
        public string VideoUrl { get; set; }

        [Display(Name = "Thumbnail Url")]
        public string ThumbnailUrl { get; set; }

        [Required]
        [Display(Name = "Video Name")]
        public string VideoName { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string VideoDescription { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string VideoType { get; set; }

        public bool Blocked { get; set; }

        public bool Deleted { get; set; }

        [Required]
        [Display(Name = "Comments Enabled")]
        public bool CommentsEnabled { get; set; }

        [Required]
        [Display(Name = "Rating Enabled")]
        public bool RatingEnabled { get; set; }

        public long LikesCount { get; set; }

        public long DislikesCount { get; set; }

        public long ViewsCount { get; set; }

        public string DatePostedString { get; set; }

        public string VideoOwner { get; set; }

        public UserDTO VideoOwnerDTO { get; set; }


        public static VideoDTO ConvertVideoToDTO(Video video)
        {
            VideoDTO newVDTO = new VideoDTO
            {
                VideoID = video.VideoID,
                VideoUrl = video.VideoUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                VideoName = video.VideoName,
                VideoDescription = video.VideoDescription,
                VideoType = video.VideoType,
                Blocked = video.Blocked,
                Deleted = video.Deleted,
                CommentsEnabled = video.CommentsEnabled,
                RatingEnabled = video.RatingEnabled,
                LikesCount = video.LikesCount,
                DislikesCount = video.DislikesCount,
                ViewsCount = video.ViewsCount,
                DatePostedString = video.DatePostedString,
                VideoOwner = video.VideoOwner,
                VideoOwnerDTO = UserDTO.ConvertUserToDTO(video.User)
            };
            return newVDTO;
        }
        public static IEnumerable<VideoDTO> ConvertCollectionVideoToDTO(IEnumerable<Video> videos)
        {
            List<VideoDTO> listDTO = new List<VideoDTO>();
            foreach (var item in videos)
            {
                listDTO.Add(ConvertVideoToDTO(item));
            }
            IEnumerable<VideoDTO> iListDTO = listDTO;
            return iListDTO;
        }
    }
}