using IleCzasu.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class CompareNamesViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(List<PublicEvent> similarEvents)
        {
           

            return View(similarEvents);
        }
    }
}
