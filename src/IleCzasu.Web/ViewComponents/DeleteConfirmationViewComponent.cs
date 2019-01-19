using IleCzasu.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class DeleteConfirmationViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(PublicEvent evnt)
        {


            return View(evnt);
        }
    }
}
