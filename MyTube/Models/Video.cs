//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyTube.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Video
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Video()
        {
            this.Comments = new HashSet<Comment>();
            this.VideoRatings = new HashSet<VideoRating>();
        }

        public long VideoID { get; set; }

        [Required]
        [Display(Name = "Video Url")]
        public string VideoUrl { get; set; }

        [Required]
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
        public System.DateTime DatePosted { get; set; }
        public string VideoOwner { get; set; }

        public string DatePostedString { get { return DatePosted.ToShortDateString(); } }

        public static SelectList videosSortOrderSelectList = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Selected = true, Text = "Latest", Value = "latest"},
            new SelectListItem { Selected = true, Text = "Oldest", Value = "oldest"},
            new SelectListItem { Selected = true, Text = "Most Viewed", Value = "most_viewed"},
            new SelectListItem { Selected = true, Text = "Least Viewed", Value = "least_viewed"},
        }, "Value", "Text", 1);

        public static SelectList VideosSortOrderSelectList() { return videosSortOrderSelectList; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VideoRating> VideoRatings { get; set; }
        public virtual VideoType VideoType1 { get; set; }
    }
}