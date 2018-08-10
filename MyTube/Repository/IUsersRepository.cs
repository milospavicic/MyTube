using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface IUsersRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
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
