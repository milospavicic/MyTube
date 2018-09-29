namespace MyTube.Models
{
    public class UserSessionModel
    {
        public string Username { get; set; }
        public string UserType { get; set; }
        public bool Blocked { get; set; }
    }
}