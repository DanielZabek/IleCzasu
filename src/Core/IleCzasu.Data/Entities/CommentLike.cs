using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Data.Entities
{
    public class CommentLike
    {
        public int CommentLikeId {get; set;}
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public bool IsLiked { get; set; }

        public Comment Comment { get; set; }
        public ApplicationUser User { get; set; }
    }
}
