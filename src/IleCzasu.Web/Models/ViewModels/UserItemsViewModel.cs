using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Web.Models.ViewModels
{
    public class UserItemsViewModel
    {
        public List<PublicEvent> UserFollows { get; set; }
        public List<PrivateEvent> UserEvents { get; set; }
        public List<Note> UserNotes { get; set; }
    }
}
