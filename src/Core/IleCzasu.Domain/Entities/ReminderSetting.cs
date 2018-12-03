using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class ReminderSetting
    {
        public int Id { get; set; }     
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public int DaysBefore { get; set; }
        public bool PrivateOnly { get; set; }
        public bool Active { get; set; }
    }
}
