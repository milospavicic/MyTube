using MyTube.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyTube.DTO
{
    public class UserDTO
    {

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

        [Display(Name = "Picture Url")]
        public string ProfilePictureUrl { get; set; }

        public string RegistrationDateString { get { return RegistrationDate.ToShortDateString(); } }

        public long SubscribersCount { get; set; }

        public long VideosCount { get; set; }

        public static UserDTO ConvertUserToDTO(User user)
        {
            UserDTO newUDTO = new UserDTO
            {
                Username = user.Username,
                Pass = user.Pass,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                UserType = user.UserType,
                Email = user.Email,
                UserDescription = user.UserDescription,
                RegistrationDate = user.RegistrationDate,
                Blocked = user.Blocked,
                Deleted = user.Deleted,
                ProfilePictureUrl = user.ProfilePictureUrl,
                SubscribersCount = user.SubscribersCount,
                VideosCount = user.Videos.Count()
            };
            return newUDTO;
        }
        public static IEnumerable<UserDTO> ConvertCollectionUserToDTO(IEnumerable<User> users)
        {
            List<UserDTO> listDTO = new List<UserDTO>();
            foreach (var item in users)
            {
                listDTO.Add(ConvertUserToDTO(item));
            }
            IEnumerable<UserDTO> iListDTO = listDTO;
            return iListDTO;
        }
    }
}