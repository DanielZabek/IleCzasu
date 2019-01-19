using System.Collections.Generic;
using IleCzasu.Data.Entities;
using IleCzasu.Application.Models;

namespace IleCzasu.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<PublicEventListItem> MostPopularEvents { get; set; }
        public IEnumerable<Category> Categories { get; set; } 
    }
}
