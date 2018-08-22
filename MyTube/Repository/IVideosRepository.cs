using MyTube.Models;
using System;
using System.Collections.Generic;

namespace MyTube.Repository
{
    interface IVideosRepository : IDisposable
    {
        IEnumerable<Video> GetNVideos(int n);
        IEnumerable<Video> GetNVideosWithout(int n, long videoId);
        IEnumerable<Video> GetNPublicVideos(int n);
        IEnumerable<Video> GetNPublicVideosWithout(int n, long videoId);
        IEnumerable<Video> GetVideosAll();
        IEnumerable<Video> GetVideosPublic();
        IEnumerable<Video> GetVideosAllOwnedByUser(string username);
        IEnumerable<Video> GetVideosPublicOwnedByUser(string username);
        IEnumerable<Video> GetVideosAllLikedByUser(string username);
        IEnumerable<Video> GetVideosPublicLikedByUser(string username);
        IEnumerable<Video> GetVideosAllAndSearch(string searchString);
        IEnumerable<Video> GetVideosPublicAndSearch(string searchString);
        Video GetVideoById(long? id);
        void InsertVideo(Video video);
        void UpdateVideo(Video video);
        void UnblockVideo(long? id);
        void BlockVideo(long? id);
        void DeleteVideo(long? id);

    }
}
