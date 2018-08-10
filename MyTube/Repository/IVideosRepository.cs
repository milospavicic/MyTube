using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface IVideosRepository : IDisposable
    {
        IEnumerable<Video> GetVideosAll();
        IEnumerable<Video> GetVideosPublic();
        Video GetVideoById(long? id);
        void InsertVideo(Video video);
        void UpdateVideo(Video video);
        void BlockVideo(long? id);
        void DeleteVideo(long? id);

    }
}
