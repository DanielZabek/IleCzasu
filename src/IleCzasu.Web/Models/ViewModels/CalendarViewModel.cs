using System;
using IleCzasu.Data.Entities;
namespace IleCzasu.Models
{
    public class CalendarViewModel
    {
        public DateTime Current { get; set; }
        public DateTime FirstDayOfWeek { get; set; }
        public ApplicationUser User { get; set; }
    }
}
