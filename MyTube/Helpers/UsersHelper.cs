using MyTube.Models;
using MyTube.Repository;
using System.Web;

namespace MyTube.Helpers
{
    public static class UsersHelper
    {
        public const string loggedInUser = "loggedInUser";

        public static void RefreshLoggedInUserSession(HttpSessionStateBase currentSession)
        {
            User currentUser = null;
            string username = LoggedInUserUsername(currentSession);
            if (username == null)
            {
                return;
            }
            using (var usersRepository = new UsersRepository(new MyTubeDBEntities()))
            {
                currentUser = usersRepository.GetUserByUsername(username);
            }
            if (currentUser == null)
            {
                currentSession.Abandon();
            }
            else
            {
                var currentUserForSession = new UserSessionModel
                {
                    Username = currentUser.Username,
                    UserType = currentUser.UserType,
                    Blocked = currentUser.Blocked
                };
                currentSession.Add(loggedInUser, currentUserForSession);
            }
        }
        public static UserSessionModel LoggedInUser(HttpSessionStateBase currentSession)
        {
            return (UserSessionModel)currentSession[loggedInUser];
        }
        public static string LoggedInUserUsername(HttpSessionStateBase currentSession)
        {
            var user = (UserSessionModel)currentSession[loggedInUser];
            return user?.Username;
        }
        public static string LoggedInUserUserType(HttpSessionStateBase currentSession)
        {
            var user = (UserSessionModel)currentSession[loggedInUser];
            return user?.UserType;
        }
        public static bool LoggedInUserIsAdmin(HttpSessionStateBase currentSession)
        {
            var user = (UserSessionModel)currentSession[loggedInUser];
            return user?.UserType == "ADMIN";
        }
        public static bool LoggedInUserIsOnHisPage(HttpSessionStateBase currentSession, string username)
        {
            var user = (UserSessionModel)currentSession[loggedInUser];
            return user?.Username == username;
        }
        public static bool LoggedInUserIsBlocked(HttpSessionStateBase currentSession)
        {
            var user = (UserSessionModel)currentSession[loggedInUser];
            return user?.Blocked == true ? true : false;
        }
    }
}