using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface IVideosRepository : IDisposable
    {
        IEnumerable<Video> GetNRandomVideos(int n);
        IEnumerable<Video> GetNPublicRandomVideos(int n);
        IEnumerable<Video> GetVideosAll();
        IEnumerable<Video> GetVideosPublic();
        IEnumerable<Video> GetVideosAllOwnedByUser(string username);
        IEnumerable<Video> GetVideosPublicOwnedByUser(string username);
        IEnumerable<Video> GetVideosAllLikedByUser(string username);
        IEnumerable<Video> GetVideosPublicLikedByUser(string username);
        Video GetVideoById(long? id);
        void InsertVideo(Video video);
        void UpdateVideo(Video video);
        void UnblockVideo(long? id);
        void BlockVideo(long? id);
        void DeleteVideo(long? id);

    }
}
