using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class Follow
    {
        public int FollowId { get; set; }
        public int PublicEventId { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public PublicEvent Event { get; set; }
    }
}
 