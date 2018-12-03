using IleCzasu.Application.Helpers;
using IleCzasu.Application.Models;
using IleCzasu.Domain.Entities;
using IleCzasu.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IleCzasu.Application.Queries
{
    public class GetPublicEventsQuery : IRequest<List<PublicEventListItem>>
    {
        public int? CategoryId { get; set; }
        public string SortBy { get; set; }
        public string Date { get; set; }
        public string City { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }

        public GetPublicEventsQuery()
        {
            CategoryId = 0;
            SortBy = "date";
            PageNumber = 1;
            PageSize = 25;
        }
    }

    public class GetPublicEventsQueryHandler : IRequestHandler<GetPublicEventsQuery, List<PublicEventListItem>>
    {
        private readonly ApplicationDbContext _context;

        public GetPublicEventsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PublicEventListItem>> Handle(GetPublicEventsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PublicEvent> publicEvents;
            CitiesHelper citiesHelper = new CitiesHelper();
            if(request.CategoryId != 0 && request.CategoryId != 10)
            {
                publicEvents = from p in _context.PublicEvents
               .Where(p => p.Category.CategoryId == request.CategoryId || p.Category.ParentCategoryId == request.CategoryId)
               .Where(e => e.Date.Date > DateTime.Now.AddDays(-2).Date)
                               select p;
            }
            else
            {
                 publicEvents = from p in _context.PublicEvents
                                .Where(e => e.Date.Date > DateTime.Now.AddDays(-2).Date)
                                select p;
            }
            if (!String.IsNullOrEmpty(request.Date))
            {
                if (request.Date.Length >= 21)
                {
                    publicEvents = publicEvents.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == request.Date || (e.Date >= DateTime.Parse(request.Date.Substring(0, 10)) && e.Date <= DateTime.Parse(request.Date.Substring(13, 10))));
                }
                else
                {
                    publicEvents = publicEvents.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == request.Date || e.Date.ToString("yyyy-MM-dd") == request.Date);
                }
            }

            if (!String.IsNullOrEmpty(request.City))
            {
          
                publicEvents = publicEvents.Where(e => citiesHelper.GetCityName(e.Place) == request.City);
            }

            switch (request.SortBy)
            {       
                case "date":
                    publicEvents = publicEvents.OrderBy(e => e.Date);
                    break;
                case "follows":
                    publicEvents = publicEvents.OrderByDescending(e => e.Follows);
                    break;            
            }

            var data = publicEvents
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory)
                .Include(te => te.TagEvents)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.TagType)
                .Skip(request.PageSize * request.PageNumber - request.PageSize).Take(request.PageSize).ToList();

            var publicEventsList = data.AsQueryable().Select(p => new PublicEventListItem {
                    PublicEventId = p.PublicEventId,
                    Name = p.Name,
                    Description = p.Description,
                    Date = p.Date,
                    CategoryId = p.CategoryId,
                    ImagePath = p.ImagePath,
                    Url = p.Url,
                    Follows = p.Follows,
                    Place = p.Place,
                    City = citiesHelper.GetCityName(p.Place),
                    Promotor = p.Promotor,
                    Price = p.Price,
                    IsFollowed = p.IsFollowed,
                    TagEvents = p.TagEvents,
                    Category = p.Category              
                }).ToList();


            if (!string.IsNullOrEmpty(request.UserId))
            {
                var user = _context.Users
                    .Include(f => f.UserFollows).SingleOrDefault(u => u.Id == request.UserId);
                foreach (var p in publicEventsList)
                {
                    if (user.UserFollows.Select(e => e.PublicEventId).Contains(p.PublicEventId))
                    {
                        p.IsFollowed = true;
                    }
                }               
            }

            return publicEventsList;
        }
    }
}

