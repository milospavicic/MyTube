using System.ComponentModel.DataAnnotations;

namespace MyTube.Models
{
    public class EditVideoModel
    {
        public long VideoID { get; set; }

        [Required]
        [Display(Name = "Video Url")]
        public string VideoUrl { get; set; }

        [Required]
        [Display(Name = "Video Name")]
        public string VideoName { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string VideoDescription { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string VideoType { get; set; }

        [Required]
        [Display(Name = "Comments Enabled")]
        public bool CommentsEnabled { get; set; }

        [Required]
        [Display(Name = "Rating Enabled")]
        public bool RatingEnabled { get; set; }


        public static EditVideoModel EditVideoModalFromVideo(Video video)
        {
            EditVideoModel videoForEdit = new EditVideoModel
            {
                VideoID = video.VideoID,
                VideoUrl = video.VideoUrl,
                VideoName = video.VideoName,
                VideoDescription = video.VideoDescription,
                VideoType = video.VideoType,
                CommentsEnabled = video.CommentsEnabled,
                RatingEnabled = video.RatingEnabled
            };
            return videoForEdit;
        }
    }
}