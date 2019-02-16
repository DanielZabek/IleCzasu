using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IleCzasu.Application.Interfaces;
using IleCzasu.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IleCzasu.Application.Services
{
    public class ReminderSettingService : IReminderSettingService
    {
        private readonly ApplicationDbContext _context;

        public ReminderSettingService(ApplicationDbContext context, IPublicEventService publicEventService)
        {
            _context = context;

        }
        public async Task AddReminderSetting(ReminderSetting reminderSetting)
        {
            if (reminderSetting != null)
                await _context.ReminderSettings.AddAsync(reminderSetting);
        }

        public async Task DeleteReminderSetting(int reminderSettingId)
        {
            var model = await GetReminderSettingById(reminderSettingId);
            if (model != null)
                _context.ReminderSettings.Remove(model);
        }

        public async Task<ReminderSetting> GetReminderSettingById(int reminderSettingId)
        {
            return await _context.ReminderSettings.SingleOrDefaultAsync(x => x.Id == reminderSettingId);
        }
    }
}
