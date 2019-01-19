using IleCzasu.Models;
using IleCzasu.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Data.Entities;

namespace IleCzasu.ViewComponents
{
    public class ShowPrivateEventsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<PrivateEvent> privateEvents)
        {


            return View(privateEvents);
        }

    }
}
