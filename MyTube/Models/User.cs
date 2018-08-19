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
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.CommentRatings = new HashSet<CommentRating>();
            this.Comments = new HashSet<Comment>();
            this.Subscribers = new HashSet<Subscriber>();
            this.Subscribers1 = new HashSet<Subscriber>();
            this.VideoRatings = new HashSet<VideoRating>();
            this.Videos = new HashSet<Video>();
        }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Pass { get; set; }

        [Required]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Display(Name = "Type")]
        public string UserType { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Description")]
        public string UserDescription { get; set; }

        public System.DateTime RegistrationDate { get; set; }
        public bool Blocked { get; set; }
        public bool Deleted { get; set; }
        [Required]
        [Display(Name = "Picture Url")]
        public string ProfilePictureUrl { get; set; }

        public string RegistrationDateString { get { return RegistrationDate.ToShortDateString(); } }

        public long SubscribersCount { get { return Subscribers.Count(); } }

        public static SelectList usersSortOrderSelectList = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Selected = true, Text = "Username Asc", Value = "username_asc"},
            new SelectListItem { Selected = true, Text = "Username Desc", Value = "username_desc"},
            new SelectListItem { Selected = true, Text = "Firstname Asc", Value = "firstname_asc"},
            new SelectListItem { Selected = true, Text = "Firstname Desc", Value = "firstname_desc"},
            new SelectListItem { Selected = true, Text = "Lastname Asc", Value = "lastname_asc"},
            new SelectListItem { Selected = true, Text = "Lastname Desc", Value = "lastname_desc"},
            new SelectListItem { Selected = true, Text = "Email Asc", Value = "email_asc"},
            new SelectListItem { Selected = true, Text = "Email Desc", Value = "email_desc"},
            new SelectListItem { Selected = true, Text = "User Type Asc", Value = "user_type_asc"},
            new SelectListItem { Selected = true, Text = "User Type Desc", Value = "user_type_desc"},
            new SelectListItem { Selected = true, Text = "Status Asc", Value = "status_asc"},
            new SelectListItem { Selected = true, Text = "Status Desc", Value = "status_desc"},
        }, "Value", "Text", 1);

        public static SelectList UsersSortOrderSelectList() { return usersSortOrderSelectList; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<CommentRating> CommentRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Subscriber> Subscribers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscriber> Subscribers1 { get; set; }

        [JsonIgnore]
        public virtual UserType UserType1 { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VideoRating> VideoRatings { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Video> Videos { get; set; }
    }
}
