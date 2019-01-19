using IleCzasu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using IleCzasu.Data;

namespace IleCzasu.ViewComponents
{
    public class CalendarViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CalendarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(int monthsToAdd)
        {
            CalendarViewModel model = new CalendarViewModel();
            model.FirstDayOfWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.FirstDayOfWeek = model.FirstDayOfWeek.AddMonths(monthsToAdd);
            model.Current = model.FirstDayOfWeek;
            int diff = (7 + (model.FirstDayOfWeek.DayOfWeek - DayOfWeek.Monday)) % 7;
            model.FirstDayOfWeek = model.FirstDayOfWeek.AddDays(-1 * diff);

            model.User = _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(f => f.UserFollows)
                .ThenInclude(f => f.Event)
                .Include(e => e.UserEvents)
                .Include(n => n.UserNotes)
                .Single();      

            return View(model);
        }
    }
}
