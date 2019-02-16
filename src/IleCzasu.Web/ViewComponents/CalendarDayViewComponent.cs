using IleCzasu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Web.ViewComponents
{
    public class CalendarDayViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CalendarDayViewModel model)
        {
            return View(model);
        }
    }
}
