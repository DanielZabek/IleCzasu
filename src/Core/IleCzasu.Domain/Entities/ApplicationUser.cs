using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IleCzasu.Domain.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int Points { get; set; }
        public string Avatar { get; set; }
        public List<Follow> UserFollows { get; set; }
        public List<PrivateEvent> UserEvents { get; set; }
        public List<Note> UserNotes { get; set; }
    }
}
