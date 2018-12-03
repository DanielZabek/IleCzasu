using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;

namespace IleCzasu.Models
{
    public class CalendarViewModel
    {
        public DateTime Current { get; set; }
        public DateTime FirstDayOfWeek { get; set; }
        public ApplicationUser User { get; set; }
    }
}
