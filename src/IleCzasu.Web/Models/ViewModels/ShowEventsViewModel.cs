using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class ShowEventsViewModel
    {
        public List<PublicEvent> Events { get; set; }
        public List<Follow> Follows { get; set; }
    }
}
