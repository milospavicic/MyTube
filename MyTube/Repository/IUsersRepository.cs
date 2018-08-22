using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface IUsersRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetNMostPopularUsers(int n, string currentUserUsername);
        IEnumerable<User> GetNRandomUsers(int n, string currentUserUsername);
        IEnumerable<User> GetAllUsersFollowedBy(string username);
        IEnumerable<User> GetAllAvaiableUsersFollowedBy(string username);
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
