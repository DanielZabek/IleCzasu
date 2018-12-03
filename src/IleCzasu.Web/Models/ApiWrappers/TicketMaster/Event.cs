using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Models.ApiWrappers.TicketMaster
{
    public class Event
    {
        public String name { get; set; }
        public String url { get; set; }
        public List<Image> images { get; set; }
        public Dates dates { get; set; }
        public List<Classification> classifications { get; set; }
        public _embedded _embedded { get; set; }

    }
    public class _embedded
    {
        public List<Attraction> attractions { get; set; }
        public List<Venue> venues { get; set; }

    }
}
