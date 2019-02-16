using IleCzasu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IleCzasu.ViewComponents
{
    public class ShowUserItemsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(UserItemsViewModel model)
        {
            return View(model);
        }

    }
}
