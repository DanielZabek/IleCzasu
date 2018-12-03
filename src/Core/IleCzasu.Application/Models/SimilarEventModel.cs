using IleCzasu.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace IleCzasu.Application.Models
{
    public class SimilarEventModel
    {
        public int PublicEventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public int SimilarPoints { get; set; }
             
    }
}
    

