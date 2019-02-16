using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IleCzasu.Application.Interfaces
{
    public interface IReminderSettingService
    {
        Task<ReminderSetting> GetReminderSettingById(int reminderSettingId);
        Task AddReminderSetting(ReminderSetting reminderSetting);
        Task DeleteReminderSetting(int reminderSettingId);
    }
}
