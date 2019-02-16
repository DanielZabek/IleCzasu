﻿using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IleCzasu.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserById(string userId);
        Task<ApplicationUser> GetUserWithItems(string userId);
        Task<int> FollowEvent(string userId, int eventId);
        Task<int> UnfollowEvent(string userId, int eventId);
        Task<List<Note>> GetUserNotes(string userId, string date = "");
        Task<List<PrivateEvent>> GetUserEvents(string userId, string date = "");
        Task<List<PublicEvent>> GetUserFollows(string userId, string date = ""); 
    }
}
