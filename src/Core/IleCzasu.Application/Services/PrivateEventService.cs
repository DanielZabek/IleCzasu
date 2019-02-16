using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IleCzasu.Application.Interfaces;
using IleCzasu.Data;
using Microsoft.EntityFrameworkCore;

namespace IleCzasu.Application.Services
{
    public class PrivateEventService : IPrivateEventService
    {
        private readonly ApplicationDbContext _context;

        public PrivateEventService(ApplicationDbContext context, IPublicEventService publicEventService)
        {
            _context = context;

        }
        public async Task AddPrivateEvent(PrivateEvent privateEvent)
        {
            if (privateEvent != null)
                await _context.PrivateEvents.AddAsync(privateEvent);
        }

        public async Task DeletePrivateEvent(int privateEventId)
        {
            var model = await GetPrivateEventById(privateEventId);
            if (model != null)
                _context.PrivateEvents.Remove(model);
        }

        public async Task<PrivateEvent> GetPrivateEventById(int privateEventId)
        {
            return await _context.PrivateEvents.SingleOrDefaultAsync(x => x.PrivateEventId == privateEventId);
        }
    }
}
