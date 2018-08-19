namespace MyTube.DTO
{
    public class UsersDTO
    {

        public string Username { get; set; }


        public string Pass { get; set; }


        public string Firstname { get; set; }


        public string Lastname { get; set; }


        public string UserType { get; set; }


        public string Email { get; set; }

        public string UserDescription { get; set; }

        public System.DateTime RegistrationDate { get; set; }

        public bool Blocked { get; set; }

        public bool Deleted { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string RegistrationDateString { get { return RegistrationDate.ToShortDateString(); } }

        public long SubscribersCount { get; set; }
    }



}