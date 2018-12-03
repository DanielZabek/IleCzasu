using IleCzasu.Application.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace IleCzasu.Domain.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PublicEventId { get; set; }
        public string UserId { get; set; }
        public int? ReplyToId { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public int Points { get; set; }
        public ApplicationUser User { get; set; }
        public PublicEvent Event { get; set; }

        [ForeignKey("ReplyToId")]
        public List<Comment> Replies { get; set; }

        [NotMapped]
        public string TimeAgo { get; set; }

        public Comment()
        {
            this.TimeAgo = this.CreationDate.TimeAgo();
        }
    }
}
