using IleCzasu.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Data.Entities;

namespace IleCzasu.ViewComponents
{
    public class ShowSubcategoriesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Category> model)
        {

            return View(model);
        }

    
    }
}
