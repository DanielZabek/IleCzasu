using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class TagEvent
    {
        public int TagEventId { get; set; }
        public int TagId { get; set; }
        public int PublicEventId { get; set; }

        public Tag Tag { get; set; }
        public PublicEvent Event { get; set; }

      
    }
  
}
