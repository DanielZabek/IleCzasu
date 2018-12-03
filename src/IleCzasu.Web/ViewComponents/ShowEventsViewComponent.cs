using IleCzasu.Application.Models;
using IleCzasu.Models;
using IleCzasu.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class ShowEventsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<PublicEventListItem> model)
        {


            return View(model);
        }

    }
}
