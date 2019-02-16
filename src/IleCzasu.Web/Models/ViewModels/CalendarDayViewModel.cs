using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Web.Models.ViewModels
{
    public class CalendarDayViewModel
    {
        public List<PublicEvent> PublicEvents { get; set; }
        public List<PrivateEvent> PrivateEvents { get; set; }
        public List<Note> Notes { get; set; }
    }
}
