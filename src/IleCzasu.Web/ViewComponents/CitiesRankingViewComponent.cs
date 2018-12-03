using IleCzasu.Domain.Entities;
using IleCzasu.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class CitiesRankingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<City> model)
        {


            return View(model);
        }
    }
}
