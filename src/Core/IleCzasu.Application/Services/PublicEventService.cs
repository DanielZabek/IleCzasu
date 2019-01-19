using IleCzasu.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using IleCzasu.Data.Entities;
using IleCzasu.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IleCzasu.Application.Models;
using IleCzasu.Application.Helpers;
using System.Threading.Tasks;

namespace IleCzasu.Application.Services
{
    public class PublicEventService : IPublicEventService
    {
        private readonly ApplicationDbContext _context;

        public PublicEventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPublicEvent(PublicEvent publicEvent)
        {
            _context.PublicEvents.Add(publicEvent);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePublicEvent(int publicEventId)
        {
            var eventToRemove = await _context.PublicEvents.SingleOrDefaultAsync(pe => pe.PublicEventId == publicEventId);
            if(eventToRemove != null){
                _context.PublicEvents.Remove(await GetPublicEventById(publicEventId));
                await _context.SaveChangesAsync();
            }           
        }

        public async Task<PublicEventDTO> GetPublicEvent(int publicEventId, string userId = "")
        {
            var data = await _context.PublicEvents
                .Include(c => c.Comments).ThenInclude(u => u.User)
                .Include(c => c.Category)
                .Include(te => te.TagEvents)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.TagType)
                 .Where(p => p.PublicEventId == publicEventId).ToListAsync();
            var publicEvent = data.AsQueryable()
                .Select(PublicEventDTO.Projection).SingleOrDefault();
            publicEvent.IsFollowed = await IsEventFollowed(publicEventId, userId);

            return publicEvent;
        }

        public async Task<PublicEvent> GetPublicEventById(int publicEventId)
        {
            return await _context.PublicEvents.SingleOrDefaultAsync(pe => pe.PublicEventId == publicEventId);
        }

        public async Task<List<PublicEventListItem>> GetPublicEvents(int? categoryId = 0, string sortBy = "date", string date = "", string city = "", int pageNumber = 1, int pageSize = 25, string userId = "")
        {
            IQueryable<PublicEvent> publicEvents;
            CitiesHelper citiesHelper = new CitiesHelper();
            if (categoryId != 0 && categoryId != 10)
            {
                publicEvents = from p in _context.PublicEvents
               .Where(p => p.Category.CategoryId == categoryId || p.Category.ParentCategoryId == categoryId)
               .Where(e => e.Date.Date > DateTime.Now.AddDays(-2).Date)
                               select p;
            }
            else
            {
                publicEvents = from p in _context.PublicEvents
                               .Where(e => e.Date.Date > DateTime.Now.AddDays(-2).Date)
                               select p;
            }
            if (!String.IsNullOrEmpty(date))
            {
                if (date.Length >= 21)
                {
                    publicEvents = publicEvents.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == date || (e.Date >= DateTime.Parse(date.Substring(0, 10)) && e.Date <= DateTime.Parse(date.Substring(13, 10))));
                }
                else
                {
                    publicEvents = publicEvents.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == date || e.Date.ToString("yyyy-MM-dd") == date);
                }
            }

            if (!String.IsNullOrEmpty(city))
            {
                publicEvents = publicEvents.Where(e => citiesHelper.GetCityName(e.Place) == city);
            }

            switch (sortBy)
            {
                case "date":
                    publicEvents = publicEvents.OrderBy(e => e.Date);
                    break;
                case "follows":
                    publicEvents = publicEvents.OrderByDescending(e => e.Follows);
                    break;
            }

            var data = await publicEvents
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory)
                .Include(te => te.TagEvents)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.TagType)
                .Skip(pageSize * pageNumber - pageSize)
                .Take(pageSize)
                .ToListAsync();

            var publicEventsList = data.Select(p => new PublicEventListItem
            {
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

            foreach (var pe in publicEventsList)
            {
                if (!string.IsNullOrEmpty(userId))
                    pe.IsFollowed = await IsEventFollowed(pe.PublicEventId, userId);
                else
                    pe.IsFollowed = false;                   
            }
           
            return publicEventsList;
        }

        public async Task<IEnumerable<SimilarEventModel>> GetSimilarEvents(int publicEventId)
        {
            var tagIdList = await _context.TagEvents.Where(te => te.PublicEventId == publicEventId).Select(te => te.TagId).ToListAsync();
            var similarEvents = await _context.TagEvents
                .Include(p => p.Event)
                .Where(t => tagIdList.Contains(t.TagId) && t.PublicEventId != publicEventId && t.Event.Date >= DateTime.Today)
                .Select(e => e.Event)
                .GroupBy(pe => pe)
                .Select(s => new SimilarEventModel
                {
                    PublicEventId = s.Key.PublicEventId,
                    Name = s.Key.Name,
                    Description = s.Key.Description,
                    Date = s.Key.Date,
                    ImagePath = s.Key.ImagePath,
                    SimilarPoints = s.Count()
                })
                .OrderByDescending(p => p.SimilarPoints)
                .Take(5)
                .ToListAsync();

            return similarEvents;
        }

        public async Task UpdatePublicEvent(PublicEvent publicEvent)
        {
            if (publicEvent != null)
            {
                _context.PublicEvents.Update(publicEvent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetCategories(int? categoryId)
        {
            if(categoryId != 0)
                return await _context.Categories.Where(c => c.ParentCategoryId == categoryId).ToListAsync();
            else
                return await _context.Categories.Include(c => c.SubCategories).ToListAsync();         
        }

        public async Task<IEnumerable<string>> GetCities()
        {
            return await _context.Cities.Select(c => c.Name).ToListAsync();         
        }
        public async Task<Category> GetCategory(int categoryId)
        {
            return await _context.Categories.SingleOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Tag>> GetPopularTags(int categoryId)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
            var tagTypes = _context.TagTypes.Where(t => t.CategoryId == categoryId || t.CategoryId == category.ParentCategoryId).Select(t => t.TagTypeId);

            return await _context.Tags.Where(t => tagTypes.Contains(t.TagTypeId)).OrderByDescending(t => t.Popularity).Take(20).ToListAsync();
        }

        public async Task<bool> IsEventFollowed(int publicEventId, string userId)
        {
            if (!string.IsNullOrEmpty(userId) && publicEventId != 0)
            {
                var user = await _context.Users
                    .Include(f => f.UserFollows)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    if (user.UserFollows.Select(e => e.PublicEventId).Contains(publicEventId))
                        return true;
                    else
                        return false;                   
                }
            }
            return false;
        } 
    }
}
