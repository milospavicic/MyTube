using MyTube.Models;
using MyTube.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace MyTube
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<UserTypesRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<IUserTypesRepository, UserTypesRepository>();

            container.RegisterType<UsersRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<IUsersRepository, UsersRepository>();

            container.RegisterType<VideosRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<IVideosRepository, VideosRepository>();

            container.RegisterType<VideoTypesRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<IVideoTypesRepository, VideoTypesRepository>();

            container.RegisterType<CommentsRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<ICommentsRepository, CommentsRepository>();

            container.RegisterType<VideoRatingRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<IVideoRatingRepository, VideoRatingRepository>();

            container.RegisterType<CommentRatingsRepository>(new InjectionConstructor(new MyTubeDBEntities()));
            container.RegisterType<ICommentRatingsRepository, CommentRatingsRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}