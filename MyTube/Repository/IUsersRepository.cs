using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    public interface IUsersRepository : IDisposable
    {
        IEnumerable<User> GetNUsersWithout(int n, string currentUserUsername);
        IEnumerable<User> GetAllUsersFollowedBy(string username);
        IEnumerable<User> GetAllAvaiableUsersFollowedBy(string username);
        IEnumerable<User> GetAndSearchUsers(string searchString);
        User GetUserByUsername(string username);
        void InsertUser(User user);
        void UpdateUser(User user);
        void BlockUser(string username);
        void UnblockUser(string username);
        void DeleteUser(string username);
        bool Login(User user);
        bool UsernameTaken(string username);
    }
}
