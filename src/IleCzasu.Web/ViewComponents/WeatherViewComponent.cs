using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class WeatherViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
