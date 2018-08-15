﻿using MyTube.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyTube.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private MyTubeDBEntities db;
        public UsersRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }
        public bool Login(User user)
        {
            bool userExists = db.Users.Any(u => u.Username == user.Username && u.Pass == user.Pass && u.Deleted == false);
            return userExists;

        }
        public bool UsernameTaken(string username)
        {
            bool userExists = db.Users.Any(u => u.Username == username);
            return userExists;

        }

        public IEnumerable<User> GetAllUsersFollowedBy(string username)
        {
            IEnumerable<User> foundUsers = from users in db.Users
                                           join subs in db.Subscribers on users.Username equals subs.ChannelSubscribed
                                           where subs.Subscriber1 == username && users.Deleted == false && users.Blocked == false
                                           select users;
            return foundUsers;
        }

        public IEnumerable<User> GetAllAvaiableUsersFollowedBy(string username)
        {
            IEnumerable<User> foundUsers = from users in db.Users
                                           join subs in db.Subscribers on users.Username equals subs.ChannelSubscribed
                                           where subs.Subscriber1 == username && users.Deleted == false
                                           select users;

            return foundUsers;
        }
        public IEnumerable<User> GetUsers()
        {
            return db.Users.Where(x => x.Deleted == false);
        }

        public User GetUserByUsername(string username)
        {
            User foundUser = db.Users.Find(username);
            if (foundUser != null && foundUser.Deleted == true)
            {
                return null;
            }
            return foundUser;
        }

        public void InsertUser(User user)
        {
            if (user != null)
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void UpdateUser(User user)
        {
            if (user != null)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public void BlockUser(string username)
        {
            var found_user = GetUserByUsername(username);
            if (found_user != null)
            {
                found_user.Blocked = true;
                db.SaveChanges();
            }
        }
        public void UnblockUser(string username)
        {
            var found_user = GetUserByUsername(username);
            if (found_user != null)
            {
                found_user.Blocked = false;
                db.SaveChanges();
            }
        }
        public void DeleteUser(string username)
        {
            var found_user = GetUserByUsername(username);
            if (found_user != null)
            {
                found_user.Deleted = true;
                db.SaveChanges();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}