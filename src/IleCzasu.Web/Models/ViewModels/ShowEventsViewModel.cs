using System.Collections.Generic;
using IleCzasu.Data.Entities;
namespace IleCzasu.Models.ViewModels
{
    public class ShowEventsViewModel
    {
        public List<PublicEvent> Events { get; set; }
        public List<Follow> Follows { get; set; }
    }
}
