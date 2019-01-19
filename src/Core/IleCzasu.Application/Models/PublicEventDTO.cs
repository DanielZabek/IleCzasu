using IleCzasu.Application.Models;
using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IleCzasu.Application.Models
{
    public class PublicEventDTO
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
        public string Promotor { get; set; }
        public double Price { get; set; }
        public bool IsFollowed { get; set; }

        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public IEnumerable<TagEvent> TagEvents { get; set; }

        public static Expression<Func<PublicEvent, PublicEventDTO>> Projection
        {
            get
            {
                return p => new PublicEventDTO
                {
                    PublicEventId = p.PublicEventId,
                    Name = p.Name,
                    Description = p.Description,
                    Date = p.Date,
                    CategoryId = p.CategoryId,
                    ImagePath = p.ImagePath,
                    Follows = p.Follows,
                    Url = p.Url,
                    Place = p.Place,
                    Price = p.Price,
                    Promotor = p.Promotor,
                    IsFollowed = p.IsFollowed,
                    Category = p.Category,
                    Comments = p.Comments,
                    TagEvents = p.TagEvents
                };
            }
        }
        public static PublicEventDTO Create(PublicEvent publicEvent)
        {
            return Projection.Compile().Invoke(publicEvent);
        }
    }
}
