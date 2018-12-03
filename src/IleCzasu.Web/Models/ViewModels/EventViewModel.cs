using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;
using IleCzasu.Application.Models;

namespace IleCzasu.Models.ViewModels
{
    public class EventViewModel
    {
        public PublicEventDTO PublicEvent { get; set; }
        public List<SimilarEventModel> SimilarEvents { get; set; }
    }
}
