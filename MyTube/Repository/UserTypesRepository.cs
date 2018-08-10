using MyTube.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyTube.Repository
{
    public class UserTypesRepository : IUserTypesRepository
    {
        private MyTubeDBEntities db;

        public UserTypesRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }
        public IEnumerable<UserType> GetUserTypes()
        {
            return db.UserTypes.ToList();
        }
        public SelectList GetUserTypesSelectList()
        {
            return new SelectList(db.UserTypes, "TypeName", "TypeName");
        }
        public SelectList GetUserTypesSelectListAndSelect(string userType)
        {
            return new SelectList(db.UserTypes, "TypeName", "TypeName", userType);
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}