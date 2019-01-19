using System;
using System.Collections.Generic;
using IleCzasu.Data.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class UserPanelViewModel
    {
        public List<PrivateEvent> UserEvents {get; set;}
        public List<Note> UserNotes { get; set; }
        public List<Category> Categories { get; set; }
        public DateTime Today { get; set; }
        public ReminderSetting Settings { get; set; }
        public ApplicationUser User { get; set; }
        public UserPanelViewModel()
        {
            Today = DateTime.Now;
        }

    }
}
