using MyTube.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTube.DTO
{
    public class CommentDTO
    {
        public long CommentID { get; set; }
        public long VideoID { get; set; }
        public string CommentOwner { get; set; }
        [Required]
        public string CommentText { get; set; }
        public System.DateTime DatePosted { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public bool Deleted { get; set; }

        public string DatePostedString { get { return DatePosted.ToShortDateString(); } }

        public static CommentDTO ConvertCommentToDTO(Comment comment)
        {
            CommentDTO newCDTO = new CommentDTO
            {
                CommentID = comment.CommentID,
                VideoID = comment.VideoID,
                CommentOwner = comment.CommentOwner,
                CommentText = comment.CommentText,
                DatePosted = comment.DatePosted,
                LikesCount = comment.LikesCount,
                DislikesCount = comment.DislikesCount,
                Deleted = comment.Deleted
            };
            return newCDTO;
        }
        public static IEnumerable<CommentDTO> ConvertCollectionCommentToDTO(IEnumerable<Comment> comments)
        {
            List<CommentDTO> listDTO = new List<CommentDTO>();
            foreach (var item in comments)
            {
                listDTO.Add(ConvertCommentToDTO(item));
            }
            IEnumerable<CommentDTO> iListDTO = listDTO;
            return iListDTO;
        }

    }
}