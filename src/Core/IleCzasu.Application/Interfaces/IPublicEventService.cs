using IleCzasu.Application.Models;
using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IleCzasu.Application.Interfaces
{
    public interface IPublicEventService
    {
        Task<PublicEvent> GetPublicEventById(int publicEventId);
        Task<PublicEventDTO> GetPublicEvent(int publicEventId, string userId = "");
        Task<List<PublicEventListItem>> GetPublicEvents(int? categotryId = 0, string sortBy = "date", string date = "", string city = "", int pageNumber = 1, int pageSize = 25, string userId = "");
        Task AddPublicEvent(PublicEvent publicEvent);
        Task UpdatePublicEvent(PublicEvent publicEvent);
        Task DeletePublicEvent(int publicEventId);
        Task<IEnumerable<SimilarEventModel>> GetSimilarEvents(int publicEventId);
        Task<List<Category>> GetCategories(int? categoryId = 0);
        Task<Category> GetCategory(int categoryId);
        Task<IEnumerable<string>> GetCities();
        Task<IEnumerable<Tag>> GetPopularTags(int categoryId);
        Task<bool> IsEventFollowed(int publicEventId, string userId);
    }
}
