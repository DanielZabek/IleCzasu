using IleCzasu.Data.Entities;
namespace IleCzasu.Models.ViewModels
{
    public class UserNavWidgetViewModel
    {
        public ApplicationUser User { get; set; }
        public int Notifications { get; set; }
    }
}
