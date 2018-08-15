﻿using System.ComponentModel.DataAnnotations;

namespace MyTube.Models
{
    public class EditUserModel
    {
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Pass { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string Lastname { get; set; }

        [Display(Name = "User type")]
        public string UserType { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Description")]
        public string UserDescription { get; set; }

        [Display(Name = "Profile Url")]
        public string ProfilePictureUrl { get; set; }

        public static User UpdateUserFromEditUserModel(EditUserModel eum, User user)
        {
            user.Pass = eum.Pass;
            user.Firstname = eum.Firstname;
            user.Lastname = eum.Lastname;
            user.Email = eum.Email;
            if (eum.UserType != null)
            {
                user.UserType = eum.UserType;
            }
            user.ProfilePictureUrl = eum.ProfilePictureUrl;
            user.UserDescription = eum.UserDescription;
            return user;
        }
        public static EditUserModel EditUserModalFromUser(User user)
        {
            EditUserModel userForEdit = new EditUserModel
            {
                Username = user.Username,
                Pass = user.Pass,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                UserType = user.UserType,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserDescription = user.UserDescription
            };
            return userForEdit;
        }
    }
}