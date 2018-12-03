using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.ViewComponents
{
    public class CroppAvatarViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(string imagePath)
        {
            

            return View("Default", imagePath);
        }
    }
}
