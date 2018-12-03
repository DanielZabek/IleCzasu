using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class Note
    {
        public int NoteId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string NoteText { get; set; }

        public ApplicationUser User { get; set; }

    }
}
