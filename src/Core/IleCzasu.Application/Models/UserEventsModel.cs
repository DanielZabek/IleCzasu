using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IleCzasu.Application.Models
{
    public class UserEventsModel
    {
        public List<PublicEvent> PublicEvents { get; set; }
        public List<PrivateEvent> PrivateEvents { get; set; }
        public List<Note> Notes { get; set; }

        public UserEventsModel()
        {
            PublicEvents = new List<PublicEvent>();
            PrivateEvents = new List<PrivateEvent>();
            Notes = new List<Note>();

        }
    }
}
