using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IleCzasu.Data;
using IleCzasu.Models;
using IleCzasu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Hangfire;
using IleCzasu.Controllers;
using IleCzasu.Domain.Entities;
using IleCzasu.Infrastructure;
using MediatR;
using System.Reflection;
using IleCzasu.Application.Events.Queries;

namespace IleCzasu
{
    public class Startup
    {    

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
  
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true;                              
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];

            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "Identity";
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(60);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = "/Home/Index";
                options.ExpireTimeSpan = TimeSpan.FromDays(60);

            });
            services.AddMediatR(typeof(GetPublicEventByIdQueryHandler).GetTypeInfo().Assembly);


            services.Configure<SecurityStampValidatorOptions>(options =>
            options.ValidationInterval = TimeSpan.FromSeconds(20));

            //Add application services.
            services.AddScoped<IStatisticsUpdater, StatisticsUpdater>();
            services.AddScoped<IReminder, Reminder>();
            services.AddScoped<IInfoService, InfoService>();
            services.AddScoped<IEventsUpdater, EventsUpdater>();
            services.AddTransient<IEmailSender, EmailSender>();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IStatisticsUpdater statisticsUpdater, IReminder reminder, IInfoService infoService, IEventsUpdater eventsUpdater, IEmailSender emailSender)
        {
            var options = new RewriteOptions()
            .AddRedirectToHttps();
            app.UseRewriter(options);
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=PublicEvents}/{action=Index}");
              
                routes.MapRoute(
                  name: "privacy",
                  template: "PolitykaPrywatnosci",
                   defaults: new { controller = "PublicEvents", action = "PolitykaPrywatnosci" });

                routes.MapRoute(
                name: "categoryView",
                template: "Kategoria/{categoryName}",
                 defaults: new { controller = "PublicEvents", action = "CategoryView" });

                routes.MapRoute(
                name: "interval",
                template: "Interwaly",
                 defaults: new { controller = "PublicEvents", action = "IntervalTimer" });

                routes.MapRoute(
                name: "stopwatch",
                template: "Stoper",
                 defaults: new { controller = "PublicEvents", action = "Stoper" });

                routes.MapRoute(
                name: "userPanel",
                template: "Panel",
                 defaults: new { controller = "UserPanel", action = "Index" });

                routes.MapRoute(
               name: "calendar",
               template: "Kalendarz",
                defaults: new { controller = "UserPanel", action = "Calendar" });

                routes.MapRoute(
               name: "create",
               template: "Dodaj",
                defaults: new { controller = "UserPanel", action = "Create" });

               routes.MapRoute(
               name: "timezones",
               template: "StrefyCzasowe",
                defaults: new { controller = "PublicEvents", action = "TimeZones" });

                routes.MapRoute(
                name: "event",
                template: "{id}/{name}",
                defaults: new { controller = "PublicEvents", action = "Event" });


            });
            RecurringJob.AddOrUpdate(() => statisticsUpdater.UpdateStatistics(), Cron.MinuteInterval(5));
            RecurringJob.AddOrUpdate(() => eventsUpdater.GetFilmwebEventsFilms(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => eventsUpdater.GetFilmwebEventsSerials(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => eventsUpdater.GetKupBilecikEvents(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => eventsUpdater.GetTicketMasterEvents(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => eventsUpdater.GetEmpikEvents(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => reminder.SendEmail(), Cron.Daily);

        }
    }
}
