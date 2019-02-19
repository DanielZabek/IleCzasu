using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IleCzasu.Application.Interfaces
{
    public interface IPrivateEventService
    {
        Task<PrivateEvent> GetPrivateEventById(int privateEventId);
        Task<List<PrivateEvent>> GetUserPrivateEvents(string userId, string date = "");
        Task AddPrivateEvent(PrivateEvent privateEvent);
        Task DeletePrivateEvent(int privateEventId);
    }
}
