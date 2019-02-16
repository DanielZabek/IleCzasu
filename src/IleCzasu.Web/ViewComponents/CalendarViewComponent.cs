using IleCzasu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using IleCzasu.Data;
using IleCzasu.Data.Entities;

namespace IleCzasu.ViewComponents
{
    public class CalendarViewComponent: ViewComponent
    {      
        public IViewComponentResult Invoke(CalendarViewModel model)
        {                    
            return View(model);
        }
    }
}
