using System.Collections.Generic;
using IleCzasu.Application.Models;
using IleCzasu.Data.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class EventViewModel
    {
        public PublicEventDTO PublicEvent { get; set; }
        public IEnumerable<SimilarEventModel> SimilarEvents { get; set; }
    }
}
