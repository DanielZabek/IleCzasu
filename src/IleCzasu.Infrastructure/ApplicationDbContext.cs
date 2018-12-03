using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IleCzasu.Domain.Entities;

namespace IleCzasu.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<PublicEvent> PublicEvents { get; set; }
        public DbSet<PrivateEvent> PrivateEvents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<TagType> TagTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagEvent> TagEvents { get; set; }
        public DbSet<ReminderSetting> ReminderSettings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<InfoForModerators> InfoForModerators { get; set; }
        public DbSet<InfoCategory> InfoCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
