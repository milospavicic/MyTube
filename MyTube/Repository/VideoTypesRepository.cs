using MyTube.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyTube.Repository
{
    public class VideoTypesRepository : IVideoTypesRepository
    {
        private MyTubeDBEntities db;

        public VideoTypesRepository(MyTubeDBEntities db)
        {
            this.db = db;
        }

        public IEnumerable<VideoType> GetVideoTypes()
        {
            return db.VideoTypes;
        }

        public SelectList GetVideoTypesSelectList()
        {
            return new SelectList(db.VideoTypes, "TypeName", "TypeName");
        }

        public SelectList GetVideoTypesSelectListAndSelect(string videoType)
        {
            return new SelectList(db.VideoTypes, "TypeName", "TypeName", videoType);
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}