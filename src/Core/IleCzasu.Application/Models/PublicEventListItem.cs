using IleCzasu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IleCzasu.Application.Models
{
    public class PublicEventListItem
    {
        public int PublicEventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string ImagePath { get; set; }
        public int Follows { get; set; }
        public string Url { get; set; }
        public string Place { get; set; }
        public string City { get; set; }
        public string Promotor { get; set; }
        public double Price { get; set; }
        public bool IsFollowed { get; set; }
        public Category Category { get; set; }
        public ICollection<TagEvent> TagEvents { get; set; }


    }
}
