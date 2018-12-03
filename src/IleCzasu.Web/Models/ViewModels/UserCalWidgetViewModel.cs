using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class UserCalWidgetViewModel
    {
        public List<PublicEvent> UserEvents { get; set; }
        public DateTime Today { get; set; }
        public ReminderSetting Settings { get; set; }
        public ApplicationUser User { get; set; }
       
        public UserCalWidgetViewModel()
        {
            Today = DateTime.Now;
        }
    }
}
