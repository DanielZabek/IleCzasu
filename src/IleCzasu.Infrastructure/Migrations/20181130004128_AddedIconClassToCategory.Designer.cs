﻿// <auto-generated />
using System;
using IleCzasu.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IleCzasu.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181130004128_AddedIconClassToCategory")]
    partial class AddedIconClassToCategory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IleCzasu.Domain.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Avatar");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("Points");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IconClass");

                    b.Property<string>("Name");

                    b.Property<int>("NumberOfEvents");

                    b.Property<int?>("ParentCategoryId");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.City", b =>
                {
                    b.Property<int>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("NumberOfEvents");

                    b.HasKey("CityId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("Points");

                    b.Property<int>("PublicEventId");

                    b.Property<int?>("ReplyToId");

                    b.Property<string>("UserId");

                    b.HasKey("CommentId");

                    b.HasIndex("PublicEventId");

                    b.HasIndex("ReplyToId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.CommentLike", b =>
                {
                    b.Property<int>("CommentLikeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommentId");

                    b.Property<bool>("IsLiked");

                    b.Property<string>("UserId");

                    b.HasKey("CommentLikeId");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("CommentLikes");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Follow", b =>
                {
                    b.Property<int>("FollowId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PublicEventId");

                    b.Property<string>("UserId");

                    b.HasKey("FollowId");

                    b.HasIndex("PublicEventId");

                    b.HasIndex("UserId");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.InfoCategory", b =>
                {
                    b.Property<int>("InfoCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("InfoCategoryId");

                    b.ToTable("InfoCategories");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.InfoForModerators", b =>
                {
                    b.Property<int>("InfoForModeratorsId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Info");

                    b.Property<int>("InfoCategoryId");

                    b.Property<DateTime>("Time");

                    b.HasKey("InfoForModeratorsId");

                    b.ToTable("InfoForModerators");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Note", b =>
                {
                    b.Property<int>("NoteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("NoteText");

                    b.Property<string>("UserId");

                    b.HasKey("NoteId");

                    b.HasIndex("UserId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.PrivateEvent", b =>
                {
                    b.Property<int>("PrivateEventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("ImagePath");

                    b.Property<string>("Name");

                    b.Property<string>("Place");

                    b.Property<int>("Repeatable");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("UserId");

                    b.HasKey("PrivateEventId");

                    b.HasIndex("UserId");

                    b.ToTable("PrivateEvents");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.PublicEvent", b =>
                {
                    b.Property<int>("PublicEventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<int>("Follows");

                    b.Property<string>("ImagePath");

                    b.Property<string>("Name");

                    b.Property<string>("Place");

                    b.Property<double>("Price");

                    b.Property<string>("Promotor");

                    b.Property<string>("Url");

                    b.HasKey("PublicEventId");

                    b.HasIndex("CategoryId");

                    b.ToTable("PublicEvents");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.ReminderSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<int>("CategoryId");

                    b.Property<int>("DaysBefore");

                    b.Property<bool>("PrivateOnly");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ReminderSettings");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Statistic", b =>
                {
                    b.Property<int>("StatisticId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("Value");

                    b.HasKey("StatisticId");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Popularity");

                    b.Property<int>("TagTypeId");

                    b.Property<string>("Value");

                    b.HasKey("TagId");

                    b.HasIndex("TagTypeId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.TagEvent", b =>
                {
                    b.Property<int>("TagEventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PublicEventId");

                    b.Property<int>("TagId");

                    b.HasKey("TagEventId");

                    b.HasIndex("PublicEventId");

                    b.HasIndex("TagId");

                    b.ToTable("TagEvents");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.TagType", b =>
                {
                    b.Property<int>("TagTypeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("TagTypeId");

                    b.HasIndex("CategoryId");

                    b.ToTable("TagTypes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Category", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Comment", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.PublicEvent", "Event")
                        .WithMany("Comments")
                        .HasForeignKey("PublicEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IleCzasu.Domain.Entities.Comment")
                        .WithMany("Replies")
                        .HasForeignKey("ReplyToId");

                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.CommentLike", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Follow", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.PublicEvent", "Event")
                        .WithMany()
                        .HasForeignKey("PublicEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser", "User")
                        .WithMany("UserFollows")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Note", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser", "User")
                        .WithMany("UserNotes")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.PrivateEvent", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser", "User")
                        .WithMany("UserEvents")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.PublicEvent", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.Category", "Category")
                        .WithMany("Events")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.Tag", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.TagType", "TagType")
                        .WithMany("Tags")
                        .HasForeignKey("TagTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.TagEvent", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.PublicEvent", "Event")
                        .WithMany("TagEvents")
                        .HasForeignKey("PublicEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IleCzasu.Domain.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IleCzasu.Domain.Entities.TagType", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.Category")
                        .WithMany("TagTypes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("IleCzasu.Domain.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
