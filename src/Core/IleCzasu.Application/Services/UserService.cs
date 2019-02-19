using IleCzasu.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using IleCzasu.Data.Entities;
using System.Threading.Tasks;
using IleCzasu.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IleCzasu.Application.Models;

namespace IleCzasu.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPublicEventService _publicEventService;
        private readonly IPrivateEventService _privateEventService;
        private readonly INoteService _noteService;

        public UserService( ApplicationDbContext context, 
                            IPublicEventService publicEventService,
                            IPrivateEventService privateEventService, 
                            INoteService noteService)
        {
            _noteService = noteService;
            _privateEventService = privateEventService;
            _context = context;
            _publicEventService = publicEventService;
        }

        public async Task<int> FollowEvent(string userId, int eventId)
        {
            var publicEvent = await _publicEventService.GetPublicEventById(eventId);
            if (publicEvent != null && !String.IsNullOrEmpty(userId))
            {
                _context.Follows.Add(new Follow { PublicEventId = eventId, UserId = userId });
                publicEvent.Follows += 1;
                _context.Update(publicEvent);
                await _context.SaveChangesAsync();
            }
            return publicEvent.Follows;
        }

        public async Task<int> UnfollowEvent(string userId, int eventId)
        {
            var publicEvent = await _publicEventService.GetPublicEventById(eventId);
            if (publicEvent != null && !String.IsNullOrEmpty(userId))
            {
                var follow = await _context.Follows.SingleOrDefaultAsync(f => f.UserId == userId && f.PublicEventId == eventId);
                _context.Follows.Remove(follow);
                publicEvent.Follows -= 1;
                _context.Update(publicEvent);
                await _context.SaveChangesAsync();
            }

            return publicEvent.Follows;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser> GetUserWithItems(string userId)
        {
            return await _context.Users
                .Include(u => u.UserFollows)
                    .ThenInclude(uf => uf.Event)
                    .ThenInclude(p => p.Category)
                    .ThenInclude(c => c.ParentCategory)
                .Include(u => u.UserFollows)
                    .ThenInclude(uf => uf.Event)
                    .ThenInclude(te => te.TagEvents)
                    .ThenInclude(t => t.Tag)
                    .ThenInclude(t => t.TagType)
                .Include(u => u.UserEvents)
                .Include(u => u.UserNotes)
                .Include(u => u.UserReminderSettings)
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<PublicEvent>> GetUserFollows(string userId, string date = "")
        {
            var userFollows = _context.Follows
                .Include(uf => uf.Event)
                    .ThenInclude(p => p.Category)
                    .ThenInclude(c => c.ParentCategory)
                .Include(uf => uf.Event)
                    .ThenInclude(te => te.TagEvents)
                    .ThenInclude(t => t.Tag)
                    .ThenInclude(t => t.TagType)
                .Where(uf => uf.UserId == userId)
                .Select(uf => uf.Event);

            if (!String.IsNullOrEmpty(date))
                userFollows = userFollows.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == date || e.Date.ToString("yyyy-MM-dd") == date);
      
            return await userFollows.ToListAsync();
        }

        public async Task<List<PrivateEvent>> GetUserEvents(string userId, string date = "")
        {
            var privateEvents = _context.PrivateEvents
                .Where(uf => uf.UserId == userId);             

            if (!String.IsNullOrEmpty(date))
                privateEvents = privateEvents.Where(e => e.StartDate.ToString("dd'.'MM'.'yyyy") == date || e.StartDate.ToString("yyyy-MM-dd") == date);

            return await privateEvents.ToListAsync();
        }

        public async Task<UserEventsModel> GetUserEventsByDate(string userId, string date = "")
        {
            var model = new UserEventsModel()
            {
              //  PublicEvents = await _publicEventService.GetPublicEvents(date: date, userId: userId),
                PrivateEvents = await _privateEventService.GetUserPrivateEvents(userId, date),
                Notes = await _noteService.GetUserNotes(userId, date)
            };

            return model;
        }

        public async Task AddNote(Note note)
        {
            if(note != null)
            {
                await _context.Notes.AddAsync(note);
            }
        }

       
    }
}
