using MyTube.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyTube.Repository
{
    interface IUserTypesRepository : IDisposable
    {
        IEnumerable<UserType> GetUserTypes();

        SelectList GetUserTypesSelectList();

        SelectList GetUserTypesSelectListAndSelect(string userType);
    }
}
