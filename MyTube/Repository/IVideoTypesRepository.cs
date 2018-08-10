using MyTube.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyTube.Repository
{
    public interface IVideoTypesRepository : IDisposable
    {
        IEnumerable<VideoType> GetVideoTypes();

        SelectList GetVideoTypesSelectList();

        SelectList GetVideoTypesSelectListAndSelect(string videoType);
    }
}