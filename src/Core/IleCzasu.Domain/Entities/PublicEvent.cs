using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IleCzasu.Domain.Entities
{
    public class PublicEvent
    {
        public int PublicEventId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public string Promotor { get; set; }
        public string ImagePath { get; set; }
        public int Follows { get; set; }
        public string Url { get; set; }
        public double Price { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<TagEvent> TagEvents { get; set; }
        public Category Category { get; set; }

        [NotMapped]
        public int SimilarPoints { get; set; }
        [NotMapped]
        public bool IsFollowed { get; set; }     
        [NotMapped]
        public string City { get; set; }
 
     
    }
}
