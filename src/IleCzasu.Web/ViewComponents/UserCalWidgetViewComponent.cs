using IleCzasu.Models;
using IleCzasu.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class UserCalWidgetViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke(UserCalWidgetViewModel model)
        {
            return View(model);
        }

    }
}
