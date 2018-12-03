using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class PrivateEvent 
    {
        public int PrivateEventId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Place { get; set; }
        public string ImagePath { get; set; }
        public int Repeatable { get; set; }
        public ApplicationUser User { get; set; }

        public PrivateEvent()
        {
            Repeatable = 0;
        }
    }
}
