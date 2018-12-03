using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;
using IleCzasu.Application.Models;

namespace IleCzasu.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<PublicEventListItem> MostPopularEvents { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public List<City> Cities { get; set; }      


    }
}
