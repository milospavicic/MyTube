using MyTube.Models;
using System.Collections.Generic;

namespace MyTube.DTO
{
    public class CommentRatingsDTO
    {
        public long LikeID { get; set; }
        public string LikeOwner { get; set; }
        public long CommentId { get; set; }
        public bool IsLike { get; set; }

        public CommentRatingsDTO()
        {

        }

        public CommentRatingsDTO(long likeID, string likeOwner, long commentId, bool isLike)
        {
            LikeID = likeID;
            LikeOwner = likeOwner;
            CommentId = commentId;
            IsLike = isLike;
        }
        public static CommentRatingsDTO ConvertCommentRToDTO(CommentRating cr)
        {
            CommentRatingsDTO newCRDTO = new CommentRatingsDTO
            {
                LikeID = cr.LikeID,
                LikeOwner = cr.LikeOwner,
                CommentId = (long)cr.CommentId,
                IsLike = cr.IsLike
            };
            return newCRDTO;
        }
        public static IEnumerable<CommentRatingsDTO> ConvertCollectionCommentToDTO(IEnumerable<CommentRating> crs)
        {
            List<CommentRatingsDTO> crsDTO = new List<CommentRatingsDTO>();
            foreach (var item in crs)
            {
                crsDTO.Add(ConvertCommentRToDTO(item));
            }
            IEnumerable<CommentRatingsDTO> commentRatingsDTO = crsDTO;
            return commentRatingsDTO;
        }
    }
}